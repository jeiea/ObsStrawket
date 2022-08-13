namespace ObsDotnetSocket {
  using MessagePack;
  using ObsDotnetSocket.DataTypes;
  using System;
  using System.Buffers;
  using System.IO;
  using System.Net.WebSockets;
  using System.Security.Cryptography;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  public class Connection : IDisposable {
    private const int _supportedRpcVersion = 0;
    private static readonly Uri defaultUri = new("ws://127.0.0.1:4455");

    private readonly WebSocket _socket;
    private readonly SegmentedWebSocketStream _stream;
    private readonly MemoryStream _sendBuffer = new(1024);
    private readonly MemoryStream _receiveBuffer = new(1024);
    private readonly MessagePackSerializerOptions _serialOptions = new(OpcodeMessageResolver.Instance);

    public static async Task<Connection> Connect(
      Uri? uri = null,
      CancellationToken? token = null,
      string? password = null,
      EventSubscriptions events = EventSubscriptions.All
    ) {
      var socket = new ClientWebSocket();
      socket.Options.AddSubProtocol("obswebsocket.msgpack");
      await socket.ConnectAsync(uri ?? defaultUri, token ?? CancellationToken.None).ConfigureAwait(false);

      var connection = new Connection(socket);
      var hello = await connection.Receive<Hello>().ConfigureAwait(false);
      if (hello.RpcVersion > _supportedRpcVersion) {
        // TODO: Log
      }

      await connection.Send(new Identify() {
        RpcVersion = _supportedRpcVersion,
        EventSubscriptions = events,
        Authentication = MakeOneTimePass(password, hello.Authentication),
      }).ConfigureAwait(false);
      try {
        var identified = await connection.Receive<Identified>();
      }
      catch (Exception ex) {
        throw ex;
      }
      return connection;
    }

    public Connection(WebSocket socket) {
      _socket = socket;
      _stream = new SegmentedWebSocketStream(socket);
    }

    public string? CloseDescription { get => _socket.CloseStatusDescription; }

    public async Task<T> Receive<T>(CancellationToken? cancellation = null) where T : IOpcodeMessage {
      var result = await ReceiveInternal(cancellation);
      if (result is not T value) {
        throw new Exception($"Expected {typeof(T)} retrieval, actual: {result.GetType()}");
      }
      return value;
    }

    public async Task<IOpcodeMessage> ReceiveInternal(CancellationToken? cancellation = null) {
      var token = cancellation ?? CancellationToken.None;

      WebSocketReceiveResult result;
      byte[] buffer = _receiveBuffer.GetBuffer();
      _receiveBuffer.Position = 0;
      _receiveBuffer.SetLength(buffer.Length);

      token.ThrowIfCancellationRequested();
      result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), token).ConfigureAwait(false);

      _receiveBuffer.SetLength(result.Count);

      if (result.EndOfMessage) {
        return await MessagePackSerializer.DeserializeAsync<IOpcodeMessage>(_receiveBuffer, _serialOptions, token).ConfigureAwait(false);
      }

      var secondBuffer = new ArraySegment<byte>(new byte[64 * 1024]);
      do {
        token.ThrowIfCancellationRequested();
        result = await _socket.ReceiveAsync(secondBuffer, token).ConfigureAwait(false);
        _receiveBuffer.Write(secondBuffer.Array, 0, result.Count);
      } while (!result.EndOfMessage);

      return await MessagePackSerializer.DeserializeAsync<IOpcodeMessage>(_receiveBuffer, _serialOptions, token);
    }

    public async Task Send(object value, CancellationToken? token = null) {
      MessagePackSerializer.Serialize(_sendBuffer, value);
      if (_sendBuffer.TryGetBuffer(out var buffer)) {
        await _socket.SendAsync(buffer, WebSocketMessageType.Binary, true, token ?? CancellationToken.None).ConfigureAwait(false);
        _sendBuffer.SetLength(0);
      }
      else {
        throw new Exception("Unreachable code");
      }
    }

    public void Dispose() {
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
