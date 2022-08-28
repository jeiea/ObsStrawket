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

    private readonly MessagePackLayer _socket;
    private readonly ClientWebSocket _clientWebSocket;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<RequestResponse>> _requests = new();

    private CancellationTokenSource _cancellation = new();

    public string? CloseDescription { get => _clientWebSocket.CloseStatusDescription; }

    public bool IsConnected { get; private set; }

    public ClientSocket(ClientWebSocket? client = null) {
      _clientWebSocket = client ?? new ClientWebSocket();
      _clientWebSocket.Options.AddSubProtocol("obswebsocket.msgpack");
      _socket = new MessagePackLayer(_clientWebSocket);
    }

    public async Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken? cancellation = null
    ) {
      var token = cancellation ?? CancellationToken.None;
      await _clientWebSocket.ConnectAsync(uri ?? MessagePackLayer.defaultUri, token).ConfigureAwait(false);

      _socket.RunLoop();
      var hello = (Hello)(await _socket.ReceiveAsync(token).ConfigureAwait(false))!;
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
      await _socket.SendAsync(identify, token).ConfigureAwait(false);

      if (await _socket.ReceiveAsync(token).ConfigureAwait(false) is not Identified identified) {
        throw new AuthenticationFailureException(GetCloseMessage());
      }

      IsConnected = true;
      _cancellation = new();
      _ = RunReceiveLoopAsync();
    }

    public async Task CloseAsync() {
      try {
        _socket.Dispose();
        await Task.WhenAny(_socket.SendTask, Task.Delay(1000)).ConfigureAwait(false);
        try {
          if (_clientWebSocket.State == WebSocketState.Open || _clientWebSocket.State == WebSocketState.CloseReceived) {
            await _clientWebSocket.CloseOutputAsync(
              WebSocketCloseStatus.NormalClosure, "Normal closure", _cancellation.Token
            ).ConfigureAwait(false);
          }

          _requests.Clear();
          _cancellation.Cancel();
          Closed.Invoke(GetCloseMessage() ?? "Unknown close reason");
        }
        finally {
          IsConnected = false;
        }
      }
      catch (OperationCanceledException) { }
    }

    private async Task RunReceiveLoopAsync() {
      try {
        while (!_cancellation.IsCancellationRequested) {
          var message = await _socket.ReceiveAsync(_cancellation.Token).ConfigureAwait(false);
          if (_cancellation.IsCancellationRequested) {
            break;
          }
          if (message == null) {
            await CloseAsync().ConfigureAwait(false);
            return;
          }

          _ = Task.Run(() => Dispatch(message));
        }
      }
      catch (OperationCanceledException) { }
    }

    private void Dispatch(IOpCodeMessage message) {
      switch (message) {
      case IEvent obsEvent:
        Event(obsEvent);
        break;
      case RequestResponse response:
        var request = _requests[response.RequestId];
        if (response.RequestStatus.Result) {
          request.SetResult(response);
        }
        else {
          request.SetException(new FailureResponseException(response));
        }
        break;
      default:
        // TODO: Log
        break;
      }
    }

    public async Task<RequestResponse?> RequestAsync(IRequest request, CancellationToken? cancellation = null) {
      using var source = CancellationTokenSource.CreateLinkedTokenSource(
        cancellation ?? CancellationToken.None,
        _cancellation.Token
      );
      var token = source.Token;
      token.ThrowIfCancellationRequested();

      string guid = $"{Guid.NewGuid()}";
      request.RequestId = guid;

      TaskCompletionSource<RequestResponse>? waiter = null;
      bool willWaitResponse = DataTypeMapping.RequestToTypes.TryGetValue(request.RequestType, out var typeMapping)
          && typeMapping.Response != typeof(RequestResponse);
      if (willWaitResponse) {
        waiter = new();
        _requests[guid] = waiter;
      }

      token.ThrowIfCancellationRequested();
      await _socket.SendAsync(request, token).ConfigureAwait(false);

      return willWaitResponse ? await waiter!.Task.ConfigureAwait(false) : null;
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
