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
    private static readonly Uri _defaultUri = new("ws://127.0.0.1:4455");

    public event Action<IEvent> Event = delegate { };
    public event Action<object> Closed = delegate { };

    private const int _supportedRpcVersion = 1;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<RequestResponse>> _requests = new();
    private readonly SemaphoreSlim _writeSemaphore = new(1);
    private readonly ILogger? _logger;

    private ClientWebSocket _clientWebSocket = new();
    private CancellationTokenSource _cancellation = new();
    private bool _isOpen = false;
    private SendPipeline _sender;
    private ReceivePipeline _receiver;

    public bool IsConnected { get => _clientWebSocket.State == WebSocketState.Open; }

    public Action<ClientWebSocket> SetOptions { get; set; } = delegate { };

    public ClientSocket(ILogger? logger = null) {
      _logger = logger;
      _sender = new(_clientWebSocket);
      _receiver = new(_clientWebSocket);
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
            await CloseInternalAsync(exception: new ObsWebSocketException($"ConnectAsync to {uri}")).ConfigureAwait(false);
          }
          catch (Exception ex) {
            _logger?.LogWarning("Ignore close exception: {}", ex);
          }
        }

        _clientWebSocket = new ClientWebSocket();
        _clientWebSocket.Options.AddSubProtocol("obswebsocket.msgpack");
        _sender = new(_clientWebSocket, _logger);
        _receiver = new(_clientWebSocket, _logger);
        SetOptions(_clientWebSocket);

        await _clientWebSocket.ConnectAsync(uri ?? _defaultUri, cancellation).ConfigureAwait(false);

        _receiver.Run();
        _sender.Run();
        var hello = (Hello)(await _receiver.Messages.ReadAsync(cancellation).ConfigureAwait(false))!;
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
        await _sender.SendAsync(identify, cancellation).ConfigureAwait(false);

        if (await _receiver.Messages.ReadAsync(cancellation).ConfigureAwait(false) is not Identified identified) {
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
      Reset(new ObsWebSocketException("Socket disposed"));
      _receiver.Dispose();
      _writeSemaphore.Dispose();
      _clientWebSocket.Dispose();
      GC.SuppressFinalize(this);
    }

    private async Task SendSafeAsync(IRequest request, CancellationToken token) {
      await _writeSemaphore.WaitAsync(token).ConfigureAwait(false);
      try {
        await _sender.SendAsync(request, token).ConfigureAwait(false);
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
        Reset(exception);
        if (_clientWebSocket.State == WebSocketState.Open || _clientWebSocket.State == WebSocketState.CloseReceived) {
          await _clientWebSocket.CloseOutputAsync(status, description, default).ConfigureAwait(false);
        }

        _receiver.Dispose();
        var cleanup = Task.WhenAll(_sender.SendTask, _receiver.ReceiveTask);
        var completed = await Task.WhenAny(cleanup, Task.Delay(1000)).ConfigureAwait(false);
        if (completed != cleanup) {
          _logger?.LogInformation("Queue is not terminated, force close.");
        }

        _clientWebSocket.Dispose();

        if (_isOpen) {
          Closed.Invoke(GetCloseMessage() ?? description ?? exception?.Message ?? "User closed websocket");
        }
        _isOpen = false;
      }
      catch (OperationCanceledException) { }
    }

    private async Task RunReceiveLoopAsync() {
      using var _1 = _logger?.BeginScope(nameof(RunReceiveLoopAsync));
      var token = _cancellation.Token;
      try {
        while (_clientWebSocket.State != WebSocketState.Closed && !token.IsCancellationRequested) {
          bool isAvailable = await _receiver.Messages.WaitToReadAsync(token).ConfigureAwait(false);
          if (!isAvailable) {
            await CloseAsync(description: "Closed normally").ConfigureAwait(false);
            break;
          }
          var message = await _receiver.Messages.ReadAsync(token).ConfigureAwait(false);
          _logger?.LogDebug("Received message: {}", message?.GetType().Name ?? "null");
          if (message == null) {
            await CloseAsync(exception: new WebSocketException(GetCloseMessage())).ConfigureAwait(false);
            break;
          }

          var waiter = new TaskCompletionSource<object?>();
          _ = Task.Run(() => {
            try {
              Dispatch(message);
            }
            finally {
              waiter.SetResult(null);
            }
          });
          await waiter.Task.ConfigureAwait(false);
        }
        _logger?.LogDebug("Close, webSocket.State: {}, cancellation: {}",
          _clientWebSocket.State, _cancellation.IsCancellationRequested);
      }
      catch (Exception exception) {
        await CloseAsync(exception: new QueueCancelledException(innerException: exception)).ConfigureAwait(false);
        _logger?.LogDebug(exception, "Queue cancelled");
      }
      _logger?.LogTrace("Exit, IsCancellationRequested: {}", token.IsCancellationRequested);
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

    private void Reset(Exception? exception = null) {
      _sender.Dispose();
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
