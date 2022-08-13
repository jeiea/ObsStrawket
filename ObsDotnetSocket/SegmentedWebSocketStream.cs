namespace ObsDotnetSocket {
  using System;
  using System.IO;
  using System.Net.WebSockets;
  using System.Threading;
  using System.Threading.Tasks;

  class SegmentedWebSocketStream : Stream {
    private readonly WebSocket _socket;
    private bool _shouldEmitEndOfMessage = false;

    public SegmentedWebSocketStream(WebSocket socket) {
      _socket = socket;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
      if (_shouldEmitEndOfMessage) {
        _shouldEmitEndOfMessage = false;
        return 0;
      }

      var segment = new ArraySegment<byte>(buffer, offset, count);
      var retrieval = await _socket.ReceiveAsync(segment, cancellationToken).ConfigureAwait(false);
      if (retrieval.EndOfMessage) {
        _shouldEmitEndOfMessage = true;
      }
      return retrieval.Count;
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
      var segment = new ArraySegment<byte>(buffer, offset, count);
      return _socket.SendAsync(segment, WebSocketMessageType.Binary, false, cancellationToken);
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count) {
      throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin) {
      throw new NotImplementedException();
    }

    public override void SetLength(long value) {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count) {
      throw new NotImplementedException();
    }
  }
}
