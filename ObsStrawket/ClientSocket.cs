using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {

  /// <summary>
  /// Low level client interface.
  /// </summary>
  public class ClientSocket : IDisposable {
    private const int _supportedRpcVersion = 1;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<IRequestResponse>> _requests = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<IRequestBatchResponse>> _batchRequests = new();
    private readonly SemaphoreSlim _connectSemaphore = new(1);
    private ClientWebSocket _clientWebSocket = new();
    private CancellationTokenSource? _cancellation;
    private bool _isOpen = false;
    private SendPipeline _sender;
    private ReceivePipeline _receiver;
    private Channel<IObsEvent> _events = Channel.CreateUnbounded<IObsEvent>();
    private Task? _receiveLoop;

    /// <summary>
    /// Create low level OBS websocket client.
    /// </summary>
    public ClientSocket() {
      _sender = new(_clientWebSocket);
      _receiver = new(_clientWebSocket);
    }

    /// <summary>
    /// Diagnostic notifications emitted while processing the websocket pipeline.
    /// </summary>
    /// <remarks>
    /// Handlers may run concurrently from several threads and the emission order is not
    /// guaranteed; write them to be thread-safe. See <see cref="PipelineEvent"/>.
    /// </remarks>
    public event Action<PipelineEvent>? PipelineEvent;

    /// <summary>
    /// Whether it is connected to websocket server
    /// </summary>
    public bool IsConnected => _isOpen && _clientWebSocket.State == WebSocketState.Open;

    /// <summary>
    /// It emits all of received OBS events.
    /// </summary>
    public ChannelReader<IObsEvent> Events => _events.Reader;

    /// <summary>
    /// Set addition websocket options.
    /// </summary>
    public Action<ClientWebSocket> SetOptions { get; set; } = delegate { };

    internal static Uri DefaultUri => new("ws://127.0.0.1:4455");

    /// <summary>
    ///  Connect to OBS websocket server.
    /// </summary>
    /// <param name="uri">Destination uri. Use <c>ws://localhost:4455</c> if null.</param>
    /// <param name="password">Password for handshake.</param>
    /// <param name="events">Event categories to subscribe.</param>
    /// <param name="cancellation">Token for cancellation.</param>
    /// <exception cref="AuthenticationFailureException"></exception>
    public async Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken cancellation = default
    ) {
      cancellation.ThrowIfCancellationRequested();

      var url = uri ?? DefaultUri;
      Emit(new PipelineTrace(PipelineLevel.Info, $"ConnectAsync to {url}."));
      await _connectSemaphore.WaitAsync(cancellation).ConfigureAwait(false);
      try {
        if (_isOpen) {
          try {
            await CloseInternalAsync().ConfigureAwait(false);
          }
          catch (Exception ex) {
            Emit(new CloseExceptionIgnored(ex));
          }
        }

        _events = Channel.CreateUnbounded<IObsEvent>();
        _cancellation = new();
        _clientWebSocket = new ClientWebSocket();
        _clientWebSocket.Options.AddSubProtocol("obswebsocket.json");
        _sender = new(_clientWebSocket, Emit);
        _receiver = new(_clientWebSocket, Emit);
        var messages = _receiver.Messages;
        SetOptions(_clientWebSocket);

        await _clientWebSocket.ConnectAsync(url, cancellation).ConfigureAwait(false);

        _receiver.Run(_cancellation.Token);
        _sender.Start();
        var hello = (Hello)await ReceiveMessageAsync(messages, cancellation).ConfigureAwait(false) ?? throw new ObsWebSocketException(GetCloseMessage() ?? "Handshake failure");
        if (hello.RpcVersion > _supportedRpcVersion) {
          Emit(new UnsupportedRpcVersion(hello.RpcVersion, _supportedRpcVersion));
        }

        var identify = new Identify() {
          RpcVersion = _supportedRpcVersion,
          EventSubscriptions = events,
          Authentication = MakeOneTimePass(password, hello.Authentication),
        };
        await _sender.SendAsync(identify, cancellation).ConfigureAwait(false);

        var identified = await ReceiveMessageAsync(messages, cancellation).ConfigureAwait(false);
        if (identified is not Identified) {
          throw new UnexpectedResponseException($"Identified message expected, but received {identified}");
        }

        _receiveLoop = LoopReceiveAsync(messages, _cancellation.Token);
        _isOpen = true;
        Emit(new PipelineTrace(PipelineLevel.Info, $"ConnectAsync to {url} complete."));
      }
      catch (ChannelClosedException closed) when (closed.InnerException is AuthenticationFailureException) {
        throw new AuthenticationFailureException(innerException: closed);
      }
      finally {
        _connectSemaphore.Release();
      }
    }

    /// <summary>
    /// Low level request method. It can send <see cref="RawRequest"/>.
    /// </summary>
    /// <param name="request">Request data.</param>
    /// <param name="cancellation">Token for cancellation</param>
    /// <returns>Response from websocket server.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IRequestResponse> RequestAsync(IRequest request, CancellationToken cancellation = default) {
      if (!IsConnected) {
        throw new InvalidOperationException("WebSocket is not connected");
      }

      using var source = LinkInstanceCancellation(cancellation);
      var token = source.Token;
      token.ThrowIfCancellationRequested();

      string guid = $"{Guid.NewGuid()}";
      request.RequestId = guid;
      Emit(new PipelineTrace(PipelineLevel.Info, $"RequestAsync {request.GetType().Name} start."));

      var waiter = new TaskCompletionSource<IRequestResponse>();
      _requests[guid] = waiter;

      await _sender.SendAsync(request, token).ConfigureAwait(false);
      var response = await waiter.Task.ConfigureAwait(false);

      Emit(new PipelineTrace(PipelineLevel.Info, $"RequestAsync {request.GetType().Name} finished."));
      return response;
    }

    /// <summary>
    /// Batch request method. It can send <see cref="RequestBatch"/>.
    /// </summary>
    /// <param name="batchRequest">Requests to batch.</param>
    /// <param name="cancellation">Token for cancellation.</param>
    /// <returns>Response from websocket server.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IRequestBatchResponse> RequestAsync(IRequestBatch batchRequest, CancellationToken cancellation = default) {
      if (!IsConnected) {
        throw new InvalidOperationException("WebSocket is not connected");
      }

      using var source = LinkInstanceCancellation(cancellation);
      var token = source.Token;
      token.ThrowIfCancellationRequested();

      string guid = $"{Guid.NewGuid()}";
      batchRequest.RequestId = guid;
      Emit(new PipelineTrace(PipelineLevel.Info, "RequestAsync batch start."));

      var waiter = new TaskCompletionSource<IRequestBatchResponse>();
      _batchRequests[guid] = waiter;

      await _sender.SendAsync(batchRequest, token).ConfigureAwait(false);
      var response = await waiter.Task.ConfigureAwait(false);

      Emit(new PipelineTrace(PipelineLevel.Info, "RequestAsync batch finished."));
      return response;
    }

    /// <summary>
    /// Close this connection. Pending requests will be cancelled.
    /// </summary>
    public async Task CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = "Client closed websocket", Exception? exception = null) {
      Emit(new PipelineTrace(PipelineLevel.Info, $"CloseAsync exception: {exception?.Message}"));
      await _connectSemaphore.WaitAsync().ConfigureAwait(false);
      try {
        await CloseInternalAsync(status, description, exception).ConfigureAwait(false);
        Emit(new PipelineTrace(PipelineLevel.Info, "CloseAsync complete."));
      }
      finally {
        _connectSemaphore.Release();
      }
    }

    /// <summary>
    /// Dispose this forever.
    /// </summary>
    public void Dispose() {
      Reset(new ObsWebSocketException("Socket disposed"));
      _connectSemaphore.Dispose();
      GC.SuppressFinalize(this);
    }

    private static async Task<IOpCodeMessage> ReceiveMessageAsync(
      ChannelReader<Task<IOpCodeMessage>> receiver, CancellationToken cancellation
    ) {
      var deserialization = await receiver.ReadAsync(cancellation).ConfigureAwait(false);
      return await deserialization.ConfigureAwait(false)!;
    }

    private static string? MakeOneTimePass(string? password, HelloAuthentication? auth) {
      if (auth == null) {
        return null;
      }
      if (password == null) {
        throw new AuthenticationFailureException("Password requested.");
      }
      string base64Secret = ApplySha256Base64($"{password}{auth.Salt}");
      return ApplySha256Base64($"{base64Secret}{auth.Challenge}");
    }

    private static string ApplySha256Base64(string rawData) {
      using var sha256Hash = SHA256.Create();
      byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
      return Convert.ToBase64String(bytes);
    }

    private CancellationTokenSource LinkInstanceCancellation(CancellationToken cancellation) {
      var instanceCancellation = _cancellation?.Token ?? CancellationToken.None;
      return CancellationTokenSource.CreateLinkedTokenSource(cancellation, instanceCancellation);
    }

    private async Task CloseInternalAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = null, Exception? exception = null) {
      if (!_isOpen) {
        return;
      }

      try {
        var socket = _clientWebSocket;
        if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived) {
          await socket.CloseOutputAsync(status, description, default).ConfigureAwait(false);
        }
      }
      catch (OperationCanceledException) { }
      catch (WebSocketException) {
        // The connection may die between the state check and the close output send.
      }
      finally {
        // Without this, pending requests outlive the connection and their awaiters hang.
        Reset(exception);
        _isOpen = false;
      }
    }

    private async Task LoopReceiveAsync(ChannelReader<Task<IOpCodeMessage>> messages, CancellationToken cancellation = default) {
      var token = cancellation;
      Emit(new PipelineTrace(PipelineLevel.Debug, "Start"));

      try {
        var events = _events.Writer;

        while (_clientWebSocket.State != WebSocketState.Closed && !token.IsCancellationRequested) {
          bool isAvailable = await messages.WaitToReadAsync(default).ConfigureAwait(false);
          if (!isAvailable) {
            break;
          }
          var message = await ReceiveMessageAsync(messages, default).ConfigureAwait(false);
          Emit(new PipelineTrace(PipelineLevel.Debug, $"Received message: {message?.GetType().Name ?? "null"}"));
          if (message == null) {
            break;
          }

          await DispatchAsync(message, events, token).ConfigureAwait(false);
        }
        Emit(new PipelineTrace(PipelineLevel.Debug,
          $"Close. webSocket.State: {_clientWebSocket.State}, cancellation: {token.IsCancellationRequested}"));
        await CloseAsync(exception: new QueueCancelledException("Server closed socket"));
      }
      catch (Exception exception) {
        await CloseAsync(exception: new QueueCancelledException(innerException: exception)).ConfigureAwait(false);
        Emit(new PipelineTrace(PipelineLevel.Debug, $"Queue cancelled: {exception.Message}", exception));
      }
      Emit(new PipelineTrace(PipelineLevel.Debug, $"Exit. IsCancellationRequested: {token.IsCancellationRequested}"));
    }

    private async Task DispatchAsync(
      IOpCodeMessage message, ChannelWriter<IObsEvent> events, CancellationToken token
    ) {
      switch (message) {
      case IObsEvent ev:
        if (ev is RawEvent rawEvent && PipelineEvent != null) {
          Emit(new RawEventReceived(JsonSerializer.Serialize(rawEvent)));
        }
        await events.WriteAsync(ev, token).ConfigureAwait(false);
        break;

      case IRequestResponse response:
        if (response is RawRequestResponse rawResponse && PipelineEvent != null) {
          Emit(new RawResponseReceived(JsonSerializer.Serialize(rawResponse)));
        }
        if (_requests.TryRemove(response.RequestId, out var request)) {
          if (response.RequestStatus.Result) {
            request.SetResult(response);
          }
          else {
            request.SetException(new FailureResponseException(response));
          }
        }
        else {
          Emit(new OrphanResponse(response.RequestId));
        }
        break;

      case IRequestBatchResponse batchResponse:
        if (_batchRequests.TryRemove(batchResponse.RequestId, out var batchRequest)) {
          batchRequest.SetResult(batchResponse);
        }
        else {
          Emit(new OrphanBatchResponse(batchResponse.RequestId));
        }
        break;

      default:
        Emit(new UnknownMessageType(message.GetType()));
        break;
      }
    }

    private void Reset(Exception? exception = null) {
      _sender.Stop();

      _cancellation?.Cancel();
      _cancellation?.Dispose();
      _cancellation = null;

      if (exception != null) {
        foreach (var request in _requests.Values) {
          request.TrySetException(exception);
        }
        foreach (var request in _batchRequests.Values) {
          request.TrySetException(exception);
        }
      }
      else {
        foreach (var request in _requests.Values) {
          request.TrySetCanceled();
        }
        foreach (var request in _batchRequests.Values) {
          request.TrySetCanceled();
        }
      }
      _requests.Clear();
      _batchRequests.Clear();
      _events.Writer.TryComplete(exception);

      _clientWebSocket.Dispose();
    }

    private string? GetCloseMessage() {
      if (_clientWebSocket.CloseStatus == null) {
        return null;
      }
      return $"{_clientWebSocket.CloseStatus}: {_clientWebSocket.CloseStatusDescription}";
    }

    private void Emit(PipelineEvent diagnostic) {
      var handler = PipelineEvent;
      if (handler == null) {
        return;
      }
      try {
        handler(diagnostic);
      }
      catch {
        // Isolate consumer exceptions so the pipeline keeps running.
      }
    }
  }
}
