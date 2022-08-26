namespace ObsDotnetSocket {
  using ObsDotnetSocket.DataTypes;
  using ObsDotnetSocket.WebSocket;
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
    private readonly IClientWebSocket _clientWebSocket;
    private readonly SemaphoreSlim _sendSemaphore = new(1, 1);
    private readonly ConcurrentDictionary<string, TaskCompletionSource<RequestResponse>> _requests = new();

    private CancellationTokenSource _cancellation = new();

    public string? CloseDescription { get => _socket.CloseMessage?.CloseStatusDescription; }

    public bool IsConnected { get; private set; }

    public ClientSocket(IClientWebSocket? client = null) {
      _clientWebSocket = client ?? GetSystemClientWebSocket();
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

      var hello = (Hello)(await _socket.ReceiveAsync(token).ConfigureAwait(false))!;
      if (hello.RpcVersion > _supportedRpcVersion) {
        // TODO: Log
      }

      var identify = new Identify() {
        RpcVersion = _supportedRpcVersion,
        EventSubscriptions = events,
        Authentication = MakeOneTimePass(password, hello.Authentication),
      };
      await _socket.SendAsync(identify, token).ConfigureAwait(false);

      var identified = await _socket.ReceiveAsync(token).ConfigureAwait(false) as Identified;
      if (identified == null) {
        var closeMessage = _socket.CloseMessage;
        throw new AuthenticationFailureException(closeMessage?.CloseStatus, closeMessage?.CloseStatusDescription ?? "Cannot read authentication response");
      }

      IsConnected = true;
      _cancellation = new();
      _ = RunReceiveLoopAsync();
    }

    public async Task CloseAsync() {
      try {
        await _sendSemaphore.WaitAsync(_cancellation.Token).ConfigureAwait(false);
        try {
          await _socket.CloseAsync(_cancellation.Token).ConfigureAwait(false);

          _requests.Clear();
          _cancellation.Cancel();
          var closeMessage = _socket.CloseMessage;
          if (closeMessage != null) {
            Closed.Invoke($"{closeMessage.CloseStatus}: {closeMessage.CloseStatusDescription}");
          }
        }
        finally {
          _sendSemaphore.Release();
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
      await _sendSemaphore.WaitAsync(token).ConfigureAwait(false);

      try {
        token.ThrowIfCancellationRequested();

        await _socket.SendAsync(request, token).ConfigureAwait(false);
      }
      finally {
        _sendSemaphore.Release();
      }

      return willWaitResponse ? await waiter!.Task.ConfigureAwait(false) : null;
    }

    public void Dispose() {
      _cancellation.Cancel();
      _cancellation.Dispose();
      _sendSemaphore.Dispose();
      _socket.Dispose();
    }

    // Method for preventing crash in Unity by type isolation.
    // Type load occurs by method unit, thus this defers assembly load.
    private static IClientWebSocket GetSystemClientWebSocket() {
      return new SystemClientWebSocket(new ClientWebSocket());
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
