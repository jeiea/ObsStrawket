namespace ObsDotnetSocket {
  using MessagePack;
  using Nerdbank.Streams;
  using ObsDotnetSocket.DataTypes;
  using ObsDotnetSocket.Serialization;
  using ObsDotnetSocket.WebSocket;
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  internal class MessagePackLayer : IDisposable {
    internal static readonly Uri defaultUri = new("ws://127.0.0.1:4455");

    public WebSocketReceival? CloseMessage { get; private set; }

    private const int _transmissionSize = 1400;

    private readonly IWebSocket _socket;
    private readonly Sequence<byte> _sendBuffer = new() { MinimumSpanLength = _transmissionSize };
    private readonly Sequence<byte> _receiveBuffer = new() { MinimumSpanLength = _transmissionSize };
    private readonly MessagePackSerializerOptions _serialOptions = new(OpCodeMessageResolver.Instance);

    public MessagePackLayer(IWebSocket socket) {
      _socket = socket;
    }

    public async Task<IOpCodeMessage?> ReceiveAsync(CancellationToken? cancellation = null) {
      _receiveBuffer.Reset();

      var token = cancellation ?? CancellationToken.None;
      while (true) {
        token.ThrowIfCancellationRequested();

        var memory = _receiveBuffer.GetMemory(_transmissionSize);
        var result = await _socket!.ReceiveAsync(memory, token).ConfigureAwait(false);
        _receiveBuffer.Advance(result.Count);

        if (result.MessageType == MessageType.Close) {
          CloseMessage = result;
          return null;
        }
        if (!result.IsContinued) {
          return MessagePackSerializer.Deserialize<IOpCodeMessage>(_receiveBuffer, _serialOptions, token);
        }
      }
    }

    public async Task SendAsync(IOpCodeMessage value, CancellationToken? cancellation = null) {
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
        token.ThrowIfCancellationRequested();
        await _socket!.SendAsync(item, MessageType.Binary, isEnd, token).ConfigureAwait(false);
      }
    }

    public Task CloseAsync(CancellationToken cancellation) {
      return _socket.CloseAsync(cancellation: cancellation);
    }

    public void Dispose() {
      _socket.Dispose();
    }
  }
}
