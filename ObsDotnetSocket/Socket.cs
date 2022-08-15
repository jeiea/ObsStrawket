namespace ObsDotnetSocket {
  using MessagePack;
  using Nerdbank.Streams;
  using ObsDotnetSocket.DataTypes;
  using System;
  using System.Net.WebSockets;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Threading.Tasks;

  internal class Socket : IDisposable {
    internal static readonly Uri defaultUri = new("ws://127.0.0.1:4455");

    private const int _transmissionSize = 1400;

    private readonly WebSocket _socket;
    private readonly Sequence<byte> _sendBuffer = new() { MinimumSpanLength = _transmissionSize };
    private readonly Sequence<byte> _receiveBuffer = new() { MinimumSpanLength = _transmissionSize };
    private readonly MessagePackSerializerOptions _serialOptions = new(OpcodeMessageResolver.Instance);

    public string? CloseDescription { get => _socket?.CloseStatusDescription; }

    public Socket(WebSocket socket) {
      _socket = socket;
    }

    public async Task<IOpcodeMessage?> ReceiveAsync(CancellationToken? cancellation = null) {
      _receiveBuffer.Reset();

      var token = cancellation ?? CancellationToken.None;
      while (true) {
        token.ThrowIfCancellationRequested();

        var memory = _receiveBuffer.GetMemory(_transmissionSize);
        ReadOnlyMemory<byte> readOnlyMemory = memory;
        var segment = GetSegment(readOnlyMemory);
        var result = await _socket!.ReceiveAsync(segment, token).ConfigureAwait(false);
        _receiveBuffer.Advance(result.Count);

        if (result.MessageType == WebSocketMessageType.Close) {
          return null;
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
          await _socket!.SendAsync(segment, WebSocketMessageType.Binary, isEnd, token).ConfigureAwait(false);
        }
        else {
          throw new Exception("Marshal failure");
        }
      }
    }

    public async Task CloseAsync(CancellationToken cancellation) {
      switch (_socket.State) {
      case WebSocketState.Open:
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellation).ConfigureAwait(false);
        break;
      case WebSocketState.CloseReceived:
        await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", cancellation).ConfigureAwait(false);
        break;
      }
    }

    public void Dispose() {
      _socket.Dispose();
    }

    private static ArraySegment<byte> GetSegment(ReadOnlyMemory<byte> memory) {
      if (MemoryMarshal.TryGetArray(memory, out var segment)) {
        return segment;
      }
      else {
        throw new Exception("Failed to get buffer");
      }
    }
  }
}
