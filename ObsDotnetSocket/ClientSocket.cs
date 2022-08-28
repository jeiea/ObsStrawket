namespace ObsDotnetSocket {
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

    private MessagePackLayer _socket;
    private ClientWebSocket _clientWebSocket = new();
    private CancellationTokenSource _cancellation = new();

    public string? CloseDescription { get => _clientWebSocket.CloseStatusDescription; }

    public bool IsConnected { get => _clientWebSocket.State == WebSocketState.Open; }

    public ClientSocket() {
      _socket = new(_clientWebSocket);
    }

    public async Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken cancellation = default
    ) {
      _socket.Dispose();
      _clientWebSocket.Dispose();

      _clientWebSocket = new ClientWebSocket();
      _clientWebSocket.Options.AddSubProtocol("obswebsocket.msgpack");
      _socket = new MessagePackLayer(_clientWebSocket);

      await _clientWebSocket.ConnectAsync(uri ?? MessagePackLayer.defaultUri, cancellation).ConfigureAwait(false);

      _socket.RunLoop();
      var hello = (Hello)(await _socket.ReceiveAsync(cancellation).ConfigureAwait(false))!;
      if (hello == null) {
        throw new Exception(GetCloseMessage() ?? "Handshake failure");
      }
      if (hello.RpcVersion > _supportedRpcVersion) {
        // TODO: Log
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
    }

    public async Task CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = null, Exception? exception = null) {
      try {
        _socket.Dispose();
        await Task.WhenAny(_socket.SendTask, Task.Delay(1000)).ConfigureAwait(false);
        if (_clientWebSocket.State == WebSocketState.Open || _clientWebSocket.State == WebSocketState.CloseReceived) {
          await _clientWebSocket.CloseOutputAsync(status, description, _cancellation.Token).ConfigureAwait(false);
        }

        foreach (var request in _requests.Values) {
          request.TrySetException(exception ?? new ObsWebSocketException("Websocket closed"));
        }
        _requests.Clear();
        _cancellation.Cancel();
        Closed.Invoke(GetCloseMessage() ?? "Unknown close reason");
      }
      catch (OperationCanceledException) { }
    }

    private async Task RunReceiveLoopAsync() {
      try {
        while (true) {
          _cancellation.Token.ThrowIfCancellationRequested();
          var message = await _socket.ReceiveAsync(_cancellation.Token).ConfigureAwait(false);
          if (message == null) {
            await CloseAsync(exception: new WebSocketException(GetCloseMessage())).ConfigureAwait(false);
            return;
          }

          _cancellation.Token.ThrowIfCancellationRequested();
          _ = Task.Run(() => Dispatch(message));
        }
      }
      catch (Exception ex) {
        await CloseAsync(exception: ex).ConfigureAwait(false);
        System.Diagnostics.Debug.WriteLine($"UsePipeReader: {ex}");
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
          // TODO: Log
        }
        break;
      default:
        // TODO: Log
        break;
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

    public void Dispose() {
      _cancellation.Cancel();
      _cancellation.Dispose();
      _socket.Dispose();
      GC.SuppressFinalize(this);
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
