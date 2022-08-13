namespace ObsDotnetSocket {
  using MessagePack;
  using Nerdbank.Streams;
  using ObsDotnetSocket.DataTypes;
  using System;
  using System.Net.WebSockets;
  using System.Runtime.InteropServices;
  using System.Security.Cryptography;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  public class Connection : IDisposable {
    private const int _supportedRpcVersion = 1;
    private const int _transmissionSize = 1400;
    private static readonly Uri defaultUri = new("ws://127.0.0.1:4455");

    private readonly WebSocket _socket;
    private readonly Sequence<byte> _sendBuffer = new() { MinimumSpanLength = _transmissionSize };
    private readonly Sequence<byte> _receiveBuffer = new() { MinimumSpanLength = _transmissionSize };
    private readonly MessagePackSerializerOptions _serialOptions = new(OpcodeMessageResolver.Instance);

    public static async Task<Connection> ConnectAsync(
      Uri? uri = null,
      CancellationToken? token = null,
      string? password = null,
      EventSubscriptions events = EventSubscriptions.All
    ) {
      var socket = new ClientWebSocket();
      socket.Options.AddSubProtocol("obswebsocket.msgpack");
      await socket.ConnectAsync(uri ?? defaultUri, token ?? CancellationToken.None).ConfigureAwait(false);

      var connection = new Connection(socket);
      var hello = await connection.ReceiveAsync<Hello>().ConfigureAwait(false);
      if (hello.RpcVersion > _supportedRpcVersion) {
        // TODO: Log
      }

      var identify = new Identify() {
        RpcVersion = _supportedRpcVersion,
        EventSubscriptions = events,
        Authentication = MakeOneTimePass(password, hello.Authentication),
      };
      await connection.SendAsync(identify).ConfigureAwait(false);
      try {
        var identified = await connection.ReceiveAsync<Identified>();
      }
      catch (Exception ex) {
        throw ex;
      }
      return connection;
    }

    public Connection(WebSocket socket) {
      _socket = socket;
    }

    public string? CloseDescription { get => _socket.CloseStatusDescription; }

    public async Task<T> ReceiveAsync<T>(CancellationToken? cancellation = null) where T : IOpcodeMessage {
      var result = await ReceiveInternalAsync(cancellation);
      if (result is not T value) {
        throw new Exception($"Expected {typeof(T)} retrieval, actual: {result.GetType()}");
      }
      return value;
    }

    public async Task<IOpcodeMessage> ReceiveInternalAsync(CancellationToken? cancellation = null) {
      _receiveBuffer.Reset();

      var token = cancellation ?? CancellationToken.None;
      while (true) {
        token.ThrowIfCancellationRequested();

        var memory = _receiveBuffer.GetMemory(_transmissionSize);
        ReadOnlyMemory<byte> readOnlyMemory = memory;
        var segment = GetSegment(readOnlyMemory);
        var result = await _socket.ReceiveAsync(segment, token).ConfigureAwait(false);
        _receiveBuffer.Advance(result.Count);

        if (result.MessageType == WebSocketMessageType.Close) {
          throw new Exception($"{result.CloseStatus}: {result.CloseStatusDescription}");
        }
        if (result.EndOfMessage) {
          return MessagePackSerializer.Deserialize<IOpcodeMessage>(_receiveBuffer, _serialOptions, token);
        }
      }
    }

    public async Task SendAsync(IOpcodeMessage value, CancellationToken? cancellation = null) {
      var token = cancellation ?? CancellationToken.None;
      token.ThrowIfCancellationRequested();

      _sendBuffer.Reset();
      MessagePackSerializer.Serialize(_sendBuffer, value, _serialOptions, token);

      var readOnlySequence = _sendBuffer.AsReadOnlySequence;
      long position = 0;
      long length = _sendBuffer.Length;
      foreach (var item in readOnlySequence) {
        position += item.Length;
        bool isEnd = position >= length;
        if (MemoryMarshal.TryGetArray(item, out var segment)) {
          token.ThrowIfCancellationRequested();
          await _socket.SendAsync(segment, WebSocketMessageType.Binary, isEnd, token).ConfigureAwait(false);
        }
        else {
          throw new Exception("Marshal failure");
        }
      }
    }

    public void Dispose() {
      _socket.Dispose();
    }

    private ArraySegment<byte> GetSegment(ReadOnlyMemory<byte> memory) {
      if (MemoryMarshal.TryGetArray(memory, out var segment)) {
        return segment;
      }
      else {
        throw new Exception("Failed to get buffer");
      }
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
