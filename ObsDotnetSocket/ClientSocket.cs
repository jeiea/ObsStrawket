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
    public event Action<IEvent> OnEvent = delegate { };

    private const int _supportedRpcVersion = 1;

    private readonly Socket _socket;
    private readonly ClientWebSocket _clientWebSocket;
    private readonly SemaphoreSlim _sendSemaphore = new(1, 1);
    private readonly ConcurrentDictionary<string, TaskCompletionSource<RequestResponse>> _requests = new();

    private CancellationTokenSource _cancellation = new();

    public string? CloseDescription { get => _clientWebSocket.CloseStatusDescription; }

    public ClientSocket(ClientWebSocket? client = null) {
      _clientWebSocket = client ?? new ClientWebSocket();
      _clientWebSocket.Options.AddSubProtocol("obswebsocket.msgpack");
      _socket = new Socket(_clientWebSocket);
    }

    public async Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken? cancellation = null
    ) {
      var token = cancellation ?? CancellationToken.None;
      await _clientWebSocket.ConnectAsync(uri ?? Socket.defaultUri, token).ConfigureAwait(false);

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
      try {
        var identified = (Identified)(await _socket.ReceiveAsync(token))!;
        _cancellation = new();
        _ = RunReceiveLoopAsync();
      }
      catch (Exception ex) {
        throw ex;
      }
    }

    public async Task CloseAsync() {
      await _sendSemaphore.WaitAsync(_cancellation.Token).ConfigureAwait(false);
      try {
        await _socket.CloseAsync(_cancellation.Token).ConfigureAwait(false);
      }
      finally {
        _sendSemaphore.Release();
      }
    }

    private async Task RunReceiveLoopAsync() {
      while (!_cancellation.IsCancellationRequested) {
        var message = await _socket.ReceiveAsync(_cancellation.Token).ConfigureAwait(false);
        if (_cancellation.IsCancellationRequested) {
          break;
        }
        if (message == null) {
          await CloseAsync().ConfigureAwait(false);
          _requests.Clear();
          _cancellation.Cancel();
          return;
        }

        _ = Task.Run(() => Dispatch(message));
      }
    }

    private void Dispatch(IOpcodeMessage message) {
      switch (message) {
      case IEvent obsEvent:
        OnEvent(obsEvent);
        break;
      case RequestResponse response:
        _requests[response.RequestId].SetResult(response);
        break;
      default:
        // TODO: Log
        break;
      }
    }

    public async Task<RequestResponse> RequestAsync(Request request, CancellationToken? cancellation = null) {
      using var source = CancellationTokenSource.CreateLinkedTokenSource(
        cancellation ?? CancellationToken.None,
        _cancellation.Token
      );
      var token = source.Token;
      token.ThrowIfCancellationRequested();

      string guid = $"{Guid.NewGuid()}";
      request.RequestId = guid;
      var waiter = new TaskCompletionSource<RequestResponse>();
      _requests[guid] = waiter;
      await _sendSemaphore.WaitAsync(token).ConfigureAwait(false);

      try {
        token.ThrowIfCancellationRequested();

        await _socket.SendAsync(request, token).ConfigureAwait(false);
      }
      finally {
        _sendSemaphore.Release();
      }
      return await waiter.Task.ConfigureAwait(false);
    }

    public void Dispose() {
      _cancellation.Cancel();
      _cancellation.Dispose();
      _sendSemaphore.Dispose();
      _socket.Dispose();
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