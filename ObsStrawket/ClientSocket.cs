namespace ObsStrawket {
  using Microsoft.Extensions.Logging;
  using ObsStrawket.DataTypes;
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
    private readonly SemaphoreSlim _writeSemaphore = new(1);
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

      _logger?.LogInformation("ConnectAsync to {}.", uri);
      await _writeSemaphore.WaitAsync(cancellation).ConfigureAwait(false);
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
        _logger?.LogInformation("ConnectAsync to {} complete.", uri);
      }
      finally {
        _writeSemaphore.Release();
      }
    }

    public async Task<RequestResponse?> RequestAsync(IRequest request, CancellationToken cancellation = default) {
      if (!IsConnected) {
        throw new InvalidOperationException("WebSocket is not connected");
      }

      using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellation, _cancellation.Token);
      var token = source.Token;
      token.ThrowIfCancellationRequested();

      string guid = $"{Guid.NewGuid()}";
      request.RequestId = guid;
      _logger?.LogInformation("RequestAsync {} start.", request.GetType().Name);

      bool willWaitResponse = DataTypeMapping.RequestToTypes.TryGetValue(request.RequestType, out var typeMapping)
          && typeMapping.Response != typeof(RequestResponse);
      RequestResponse? response = null;
      if (willWaitResponse) {
        var waiter = new TaskCompletionSource<RequestResponse>();
        _requests[guid] = waiter;

        await SendSafeAsync(request, token).ConfigureAwait(false);
        response = await waiter.Task.ConfigureAwait(false);
      }
      else {
        await SendSafeAsync(request, token).ConfigureAwait(false);
      }
      _logger?.LogInformation("RequestAsync {} finished.", request.GetType().Name);
      return response;
    }

    public async Task CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = "Client closed websocket", Exception? exception = null) {
      _logger?.LogInformation("CloseAsync");
      await _writeSemaphore.WaitAsync().ConfigureAwait(false);
      try {
        await CloseInternalAsync(status, description, exception).ConfigureAwait(false);
        _logger?.LogInformation("CloseAsync complete.");
      }
      finally {
        _writeSemaphore.Release();
      }
    }

    public void Dispose() {
      ClearQueue(new ObsWebSocketException("Socket disposed"));
      _writeSemaphore.Dispose();
      _clientWebSocket.Dispose();
      GC.SuppressFinalize(this);
    }

    private async Task SendSafeAsync(IRequest request, CancellationToken token) {
      await _writeSemaphore.WaitAsync(token).ConfigureAwait(false);
      try {
        await _socket.SendAsync(request, token).ConfigureAwait(false);
      }
      finally {
        _writeSemaphore.Release();
      }
    }

    private async Task CloseInternalAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = null, Exception? exception = null) {
      if (!_isOpen) {
        return;
      }

      try {
        ClearQueue(exception);
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
      using var _1 = _logger?.BeginScope(nameof(RunReceiveLoopAsync));
      try {
        while (_clientWebSocket.State != WebSocketState.Closed) {
          _cancellation.Token.ThrowIfCancellationRequested();
          var message = await _socket.ReceiveAsync(_cancellation.Token).ConfigureAwait(false);
          _logger?.LogDebug("Received message: {}", message?.GetType().Name ?? "null");
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
        _logger?.LogDebug(exception, "Queue cancelled");
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
          _logger?.LogWarning("Dispatch: Failed to remove completed task");
        }
        break;
      default:
        _logger?.LogWarning("Dispatch: Unknown message type: {}", message.GetType());
        break;
      }
    }

    private void ClearQueue(Exception? exception = null) {
      _socket.Cancel(exception);
      _socket.Dispose();
      _cancellation.Cancel();
      _cancellation.Dispose();

      if (exception != null) {
        foreach (var request in _requests.Values) {
          request.TrySetException(exception);
        }
      }
      else {
        foreach (var request in _requests.Values) {
          request.TrySetCanceled();
        }
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