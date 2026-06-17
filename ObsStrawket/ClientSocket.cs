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
    private readonly ConcurrentDictionary<string, TaskCompletionSource<IHasRequestId>> _pendingRequests = new();
    private readonly SemaphoreSlim _connectSemaphore = new(1);
    private ClientWebSocket _clientWebSocket = new();
    private CancellationTokenSource? _cancellation;
    private bool _isOpen;
    private SendPipeline _sender;
    private ReceivePipeline _receiver;
    private Channel<IObsEvent> _events = Channel.CreateUnbounded<IObsEvent>();

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
    public Action<ClientWebSocket> SetOptions { get; set; } = static delegate { };

    internal static Uri DefaultUri => new("ws://127.0.0.1:4455");

    /// <summary>
    ///  Connect to OBS websocket server.
    /// </summary>
    /// <param name="uri">Destination uri. Use <c>ws://localhost:4455</c> if null.</param>
    /// <param name="password">Password for handshake.</param>
    /// <param name="events">Event categories to subscribe.</param>
    /// <param name="cancellation">Token for cancellation.</param>
    /// <returns><see langword="true"/> when connected; <see langword="false"/> when authentication fails.</returns>
    public async Task<bool> ConnectAsync(
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

        _ = LoopReceiveAsync(messages, _cancellation.Token);
        _isOpen = true;
        Emit(new PipelineTrace(PipelineLevel.Info, $"ConnectAsync to {url} complete."));
        return true;
      }
      catch (ChannelClosedException closed) when (closed.InnerException is AuthenticationFailureException) {
        Reset();
        return false;
      }
      catch (AuthenticationFailureException) {
        Reset();
        return false;
      }
      finally {
        _ = _connectSemaphore.Release();
      }
    }

    /// <summary>
    /// Low level request method. It can send <see cref="RawRequest"/>.
    /// </summary>
    /// <param name="request">Request data.</param>
    /// <param name="cancellation">Token for cancellation</param>
    /// <returns>Response from websocket server.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<IRequestResponse> RequestAsync(IRequest request, CancellationToken cancellation = default) {
      return RequestAsync<IRequestResponse>(request, cancellation);
    }

    /// <summary>
    /// Batch request method. It can send <see cref="RequestBatch"/>.
    /// </summary>
    /// <param name="batchRequest">Requests to batch.</param>
    /// <param name="cancellation">Token for cancellation.</param>
    /// <returns>Response from websocket server.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<IRequestBatchResponse> RequestAsync(IRequestBatch batchRequest, CancellationToken cancellation = default) {
      return RequestAsync<IRequestBatchResponse>(batchRequest, cancellation);
    }

    private async Task<TResponse> RequestAsync<TResponse>(
      IHasRequestId request,
      CancellationToken cancellation
    ) where TResponse : class, IHasRequestId {
      if (!IsConnected) {
        throw new InvalidOperationException("WebSocket is not connected");
      }

      using var operationCancellation = LinkInstanceCancellation(cancellation);
      operationCancellation.Token.ThrowIfCancellationRequested();
      request.RequestId = $"{Guid.NewGuid()}";
      string traceName = request is IRequestBatch ? "batch" : request.GetType().Name;
      Emit(new PipelineTrace(PipelineLevel.Info, $"RequestAsync {traceName} start."));

      var waiter = new TaskCompletionSource<IHasRequestId>(
        TaskCreationOptions.RunContinuationsAsynchronously
      );
      if (!_pendingRequests.TryAdd(request.RequestId, waiter)) {
        throw new InvalidOperationException($"Duplicate request id: {request.RequestId}");
      }

      using var registration = cancellation.Register(() => {
        _ = TryCompletePendingRequest(
          request.RequestId,
          pendingRequest => pendingRequest.TrySetCanceled(cancellation)
        );
      });

      try {
        try {
          await _sender.SendAsync(request, operationCancellation.Token).ConfigureAwait(false);
        }
        catch (Exception exception) {
          _ = TryCompletePendingRequest(
            request.RequestId,
            pendingRequest => pendingRequest.TrySetException(exception)
          );
        }
        var response = await waiter.Task.ConfigureAwait(false);
        Emit(new PipelineTrace(PipelineLevel.Info, $"RequestAsync {traceName} finished."));
        return response as TResponse
          ?? throw new UnexpectedResponseException(
            $"Expected {typeof(TResponse).Name}, but received {response.GetType().Name}."
          );
      }
      finally {
        _ = _pendingRequests.TryRemove(request.RequestId, out _);
      }
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
        _ = _connectSemaphore.Release();
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
      return await deserialization.ConfigureAwait(false);
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
      byte[] data = Encoding.UTF8.GetBytes(rawData);
#if NETSTANDARD2_0
      using var sha256Hash = SHA256.Create();
      byte[] bytes = sha256Hash.ComputeHash(data);
#else
      byte[] bytes = SHA256.HashData(data);
#endif
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
        if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived) {
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

      case IHasRequestId response:
        if (response is RawRequestResponse rawResponse && PipelineEvent != null) {
          Emit(new RawResponseReceived(JsonSerializer.Serialize(rawResponse)));
        }
        bool responseMatched = TryCompletePendingRequest(
          response.RequestId,
          request => response is IRequestResponse { RequestStatus.Result: false } failed
            ? request.TrySetException(new FailureResponseException(failed))
            : request.TrySetResult(response)
        );
        if (!responseMatched) {
          Emit(response is IRequestBatchResponse
            ? new OrphanBatchResponse(response.RequestId)
            : new OrphanResponse(response.RequestId));
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

      CompletePendingRequests(exception);
      _ = _events.Writer.TryComplete(exception);

      _clientWebSocket.Dispose();
    }

    private bool TryCompletePendingRequest(
      string requestId,
      Func<TaskCompletionSource<IHasRequestId>, bool> complete
    ) {
      return _pendingRequests.TryRemove(requestId, out var pendingRequest)
        && complete(pendingRequest);
    }

    private void CompletePendingRequests(Exception? exception) {
      foreach (string requestId in _pendingRequests.Keys) {
        _ = TryCompletePendingRequest(
          requestId,
          request => exception == null
            ? request.TrySetCanceled()
            : request.TrySetException(exception)
        );
      }
    }

    private string? GetCloseMessage() {
      return _clientWebSocket.CloseStatus == null ? null : $"{_clientWebSocket.CloseStatus}: {_clientWebSocket.CloseStatusDescription}";
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
