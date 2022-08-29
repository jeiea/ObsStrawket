namespace ObsDotnetSocket {
  using Microsoft.Extensions.Logging;
  using ObsDotnetSocket.DataTypes;
  using System;
  using System.Collections.Concurrent;
  using System.Net.WebSockets;
  using System.Security.Cryptography;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  public class ClientSocket : IDisposable {
    public event Action<IEvent> Event = delegate { };
    public event Action<object> Closed = delegate { };

    private const int _supportedRpcVersion = 1;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<RequestResponse>> _requests = new();
    private readonly SemaphoreSlim _connectSemaphore = new(1);
    private readonly ILogger? _logger;

    private MessagePackLayer _socket;
    private ClientWebSocket _clientWebSocket = new();
    private CancellationTokenSource _cancellation = new();
    private bool _isOpen = false;

    public bool IsConnected { get => _clientWebSocket.State == WebSocketState.Open; }

    public Action<ClientWebSocket> SetOptions { get; set; } = delegate { };

    public ClientSocket(ILogger? logger = null) {
      _logger = logger;
      _socket = new(_clientWebSocket);
    }

    public async Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken cancellation = default
    ) {
      cancellation.ThrowIfCancellationRequested();

      await _connectSemaphore.WaitAsync(cancellation).ConfigureAwait(false);
      try {
        if (_isOpen) {
          try {
            await CloseInternalAsync(exception: new ObsWebSocketException("Websocket connect")).ConfigureAwait(false);
          }
          catch (Exception ex) {
            _logger?.LogWarning("Ignore close exception: {}", ex);
          }
        }

        _clientWebSocket = new ClientWebSocket();
        _clientWebSocket.Options.AddSubProtocol("obswebsocket.msgpack");
        SetOptions(_clientWebSocket);
        _socket = new MessagePackLayer(_clientWebSocket, _logger);

        await _clientWebSocket.ConnectAsync(uri ?? MessagePackLayer.defaultUri, cancellation).ConfigureAwait(false);

        _socket.RunLoop();
        var hello = (Hello)(await _socket.ReceiveAsync(cancellation).ConfigureAwait(false))!;
        if (hello == null) {
          throw new Exception(GetCloseMessage() ?? "Handshake failure");
        }
        if (hello.RpcVersion > _supportedRpcVersion) {
          _logger?.LogWarning("OBS RPC version({hello}) is newer than supported version({supported}).", hello.RpcVersion, _supportedRpcVersion);
        }

        var identify = new Identify() {
          RpcVersion = _supportedRpcVersion,
          EventSubscriptions = events,
          Authentication = MakeOneTimePass(password, hello.Authentication),
        };
        await _socket.SendAsync(identify, cancellation).ConfigureAwait(false);

        if (await _socket.ReceiveAsync(cancellation).ConfigureAwait(false) is not Identified identified) {
          throw new AuthenticationFailureException(GetCloseMessage());
        }

        _cancellation = new();
        _ = RunReceiveLoopAsync();
        _isOpen = true;
      }
      finally {
        _connectSemaphore.Release();
      }
    }

    public async Task<RequestResponse?> RequestAsync(IRequest request, CancellationToken cancellation = default) {
      using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellation, _cancellation.Token);
      var token = source.Token;
      token.ThrowIfCancellationRequested();

      string guid = $"{Guid.NewGuid()}";
      request.RequestId = guid;

      bool willWaitResponse = DataTypeMapping.RequestToTypes.TryGetValue(request.RequestType, out var typeMapping)
          && typeMapping.Response != typeof(RequestResponse);
      _logger?.LogDebug("SendAsync: {}", request.GetType().Name);
      if (willWaitResponse) {
        var waiter = new TaskCompletionSource<RequestResponse>();
        _requests[guid] = waiter;

        await _socket.SendAsync(request, token).ConfigureAwait(false);
        return await waiter.Task.ConfigureAwait(false);
      }
      else {
        await _socket.SendAsync(request, token).ConfigureAwait(false);
        return null;
      }
    }

    public async Task CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = null, Exception? exception = null) {
      await _connectSemaphore.WaitAsync().ConfigureAwait(false);
      try {
        await CloseInternalAsync(status, description, exception).ConfigureAwait(false);
      }
      finally {
        _connectSemaphore.Release();
      }
    }

    public void Dispose() {
      ClearQueue(new ObsWebSocketException("Socket disposed"));
      _connectSemaphore.Dispose();
      _clientWebSocket.Dispose();
      GC.SuppressFinalize(this);
    }

    private async Task CloseInternalAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = null, Exception? exception = null) {
      if (!_isOpen) {
        return;
      }

      try {
        ClearQueue(exception ?? new ObsWebSocketException("Websocket closed"));
        await Task.WhenAny(_socket.SendTask, Task.Delay(1000)).ConfigureAwait(false);
        if (_clientWebSocket.State == WebSocketState.Open || _clientWebSocket.State == WebSocketState.CloseReceived) {
          await _clientWebSocket.CloseOutputAsync(status, description, default).ConfigureAwait(false);
        }
        _clientWebSocket.Dispose();

        if (_isOpen) {
          Closed.Invoke(GetCloseMessage() ?? "Unknown close reason");
        }
        _isOpen = false;
      }
      catch (OperationCanceledException) { }
    }

    private async Task RunReceiveLoopAsync() {
      try {
        while (_clientWebSocket.State != WebSocketState.Closed) {
          _cancellation.Token.ThrowIfCancellationRequested();
          var message = await _socket.ReceiveAsync(_cancellation.Token).ConfigureAwait(false);
          _logger?.LogInformation("Received message: {}", message?.GetType().Name ?? "null");
          if (message == null) {
            await CloseAsync(exception: new WebSocketException(GetCloseMessage())).ConfigureAwait(false);
            return;
          }

          _cancellation.Token.ThrowIfCancellationRequested();
          _ = Task.Run(() => Dispatch(message));
        }
      }
      catch (Exception exception) {
        await CloseAsync(exception: new QueueCancelledException(innerException: exception)).ConfigureAwait(false);
        _logger?.LogDebug("RunReceiveLoopAsync: {}", exception);
      }
    }

    private void Dispatch(IOpCodeMessage message) {
      switch (message) {
      case IEvent obsEvent:
        Event(obsEvent);
        break;
      case RequestResponse response:
        if (_requests.TryRemove(response.RequestId, out var request)) {
          if (response.RequestStatus.Result) {
            request.SetResult(response);
          }
          else {
            request.SetException(new FailureResponseException(response));
          }
        }
        else {
          _logger?.LogWarning("Failed to remove completed task");
        }
        break;
      default:
        _logger?.LogWarning("Unknown message type: {}", message.GetType());
        break;
      }
    }

    private void ClearQueue(Exception exception) {
      _socket.Cancel(exception);
      _socket.Dispose();
      _cancellation.Cancel();
      _cancellation.Dispose();
      foreach (var request in _requests.Values) {
        request.TrySetException(exception);
      }
      _requests.Clear();
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
        throw new Exception("Password requested.");
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
