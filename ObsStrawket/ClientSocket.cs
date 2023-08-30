using MessagePack;
using Microsoft.Extensions.Logging;
using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {
  /// <summary>
  /// Low level client interface.
  /// </summary>
  public class ClientSocket : IDisposable {
    internal static Uri DefaultUri => new("ws://127.0.0.1:4455");

    private const int _supportedRpcVersion = 1;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<IRequestResponse>> _requests = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<IRequestBatchResponse>> _batchRequests = new();
    private readonly SemaphoreSlim _connectSemaphore = new(1);
    private readonly ILogger? _logger;

    private ClientWebSocket _clientWebSocket = new();
    private CancellationTokenSource? _cancellation;
    private bool _isOpen = false;
    private SendPipeline _sender;
    private ReceivePipeline _receiver;
    private Channel<IObsEvent> _events = Channel.CreateUnbounded<IObsEvent>();
    private Task? _receiveLoop;

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

    /// <summary>
    /// Create low level OBS websocket client.
    /// </summary>
    /// <param name="logger">Logger for library debugging.</param>
    public ClientSocket(ILogger? logger = null) {
      _logger = logger;
      _sender = new(_clientWebSocket);
      _receiver = new(_clientWebSocket);
    }

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
      _logger?.LogInformation("ConnectAsync to {}.", url);
      await _connectSemaphore.WaitAsync(cancellation).ConfigureAwait(false);
      try {
        if (_isOpen) {
          try {
            await CloseInternalAsync().ConfigureAwait(false);
          }
          catch (Exception ex) {
            _logger?.LogWarning("Ignore close exception: {}", ex);
          }
        }

        _events = Channel.CreateUnbounded<IObsEvent>();
        _cancellation = new();
        _clientWebSocket = new ClientWebSocket();
        _clientWebSocket.Options.AddSubProtocol("obswebsocket.msgpack");
        _sender = new(_clientWebSocket, _logger);
        _receiver = new(_clientWebSocket, _logger);
        var messages = _receiver.Messages;
        SetOptions(_clientWebSocket);

        await _clientWebSocket.ConnectAsync(url, cancellation).ConfigureAwait(false);

        _receiver.Run(_cancellation.Token);
        _sender.Start();
        var hello = (Hello)await ReceiveMessageAsync(messages, cancellation).ConfigureAwait(false) ?? throw new ObsWebSocketException(GetCloseMessage() ?? "Handshake failure");
        if (hello.RpcVersion > _supportedRpcVersion) {
          _logger?.LogWarning("OBS RPC version({hello}) is newer than supported version({supported}).", hello.RpcVersion, _supportedRpcVersion);
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
        _logger?.LogInformation("ConnectAsync to {} complete.", url);
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
      _logger?.LogInformation("RequestAsync {} start.", request.GetType().Name);

      var waiter = new TaskCompletionSource<IRequestResponse>();
      _requests[guid] = waiter;

      await _sender.SendAsync(request, token).ConfigureAwait(false);
      var response = await waiter.Task.ConfigureAwait(false);

      _logger?.LogInformation("RequestAsync {} finished.", request.GetType().Name);
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
      _logger?.LogInformation("RequestAsync batch start.");

      var waiter = new TaskCompletionSource<IRequestBatchResponse>();
      _batchRequests[guid] = waiter;

      await _sender.SendAsync(batchRequest, token).ConfigureAwait(false);
      var response = await waiter.Task.ConfigureAwait(false);

      _logger?.LogInformation("RequestAsync batch finished.");
      return response;
    }

    /// <summary>
    /// Close this connection. Pending requests will be cancelled.
    /// </summary>
    public async Task CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = "Client closed websocket", Exception? exception = null) {
      _logger?.LogInformation("CloseAsync exception: {}", exception?.Message);
      await _connectSemaphore.WaitAsync().ConfigureAwait(false);
      try {
        await CloseInternalAsync(status, description, exception).ConfigureAwait(false);
        _logger?.LogInformation("CloseAsync complete.");
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
        Reset(exception);
      }
      catch (OperationCanceledException) { }
      finally {
        _isOpen = false;
      }
    }

    private async Task LoopReceiveAsync(ChannelReader<Task<IOpCodeMessage>> messages, CancellationToken cancellation = default) {
      using var _1 = _logger?.BeginScope(nameof(LoopReceiveAsync));
      var token = cancellation;
      _logger?.LogDebug("Start");

      try {
        var events = _events.Writer;

        while (_clientWebSocket.State != WebSocketState.Closed && !token.IsCancellationRequested) {
          bool isAvailable = await messages.WaitToReadAsync(default).ConfigureAwait(false);
          if (!isAvailable) {
            break;
          }
          var message = await ReceiveMessageAsync(messages, default).ConfigureAwait(false);
          _logger?.LogDebug("Received message: {}", message?.GetType().Name ?? "null");
          if (message == null) {
            break;
          }

          await DispatchAsync(message, events, token).ConfigureAwait(false);
        }
        _logger?.LogDebug("Close. webSocket.State: {}, cancellation: {}",
          _clientWebSocket.State, token.IsCancellationRequested);
        await CloseAsync(exception: new QueueCancelledException("Server closed socket"));
      }
      catch (Exception exception) {
        await CloseAsync(exception: new QueueCancelledException(innerException: exception)).ConfigureAwait(false);
        _logger?.LogDebug(exception, "Queue cancelled: {}", exception.Message);
      }
      _logger?.LogDebug("Exit. IsCancellationRequested: {}", token.IsCancellationRequested);
    }

    private async static Task<IOpCodeMessage> ReceiveMessageAsync(
      ChannelReader<Task<IOpCodeMessage>> receiver, CancellationToken cancellation
    ) {
      var deserialization = await receiver.ReadAsync(cancellation).ConfigureAwait(false);
      return await deserialization.ConfigureAwait(false)!;
    }

    private async Task DispatchAsync(
      IOpCodeMessage message, ChannelWriter<IObsEvent> events, CancellationToken token
    ) {
      switch (message) {
      case IObsEvent ev:
        if (ev is RawEvent rawEvent) {
          string json = MessagePackSerializer.SerializeToJson(rawEvent, cancellationToken: default);
          _logger?.LogWarning("Received raw event: {}", json);
        }
        await events.WriteAsync(ev, token).ConfigureAwait(false);
        break;
      case IRequestResponse response:
        if (response is RawRequestResponse rawResponse) {
          string json = MessagePackSerializer.SerializeToJson(rawResponse, cancellationToken: default);
          _logger?.LogWarning("Received raw response: {}", json);
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
          _logger?.LogWarning("Dispatch: Failed to remove completed request: {}", response.RequestId);
        }
        break;
      case IRequestBatchResponse batchResponse:
        if (_batchRequests.TryRemove(batchResponse.RequestId, out var batchRequest)) {
          batchRequest.SetResult(batchResponse);
        }
        else {
          _logger?.LogWarning("Dispatch: Failed to remove completed batch: {}", batchResponse.RequestId);
        }
        break;
      default:
        _logger?.LogWarning("Dispatch: Unknown message type: {}", message.GetType());
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
  }
}
