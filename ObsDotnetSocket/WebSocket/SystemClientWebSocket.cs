namespace ObsDotnetSocket.WebSocket {
  using System;
  using System.Net.WebSockets;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Threading.Tasks;

  internal class SystemClientWebSocket : SystemWebSocket, IClientWebSocket {
    public SystemClientWebSocket(ClientWebSocket? socket = null) : base(socket ?? new ClientWebSocket()) {
      Options = new ClientOptions(((ClientWebSocket)_webSocket).Options);
    }

    public IClientWebSocketOptions Options { get; private set; }

    public Task ConnectAsync(Uri uri, CancellationToken cancellation = default) {
      return ((ClientWebSocket)_webSocket).ConnectAsync(uri, cancellation);
    }

    class ClientOptions : IClientWebSocketOptions {
      private readonly ClientWebSocketOptions _options;

      public ClientOptions(ClientWebSocketOptions options) {
        _options = options;
      }

      public void AddSubProtocol(string subProtocol) {
        _options.AddSubProtocol(subProtocol);
      }
    }
  }

  internal class SystemWebSocket : IWebSocket {
    protected readonly WebSocket _webSocket;
    private bool _isDisposed;

    public SystemWebSocket(WebSocket socket) {
      _webSocket = socket;
    }

    public Task SendAsync(ReadOnlyMemory<byte> memory, MessageType type, bool isContinued = false, CancellationToken cancellation = default) {
      return _webSocket.SendAsync(GetSegment(memory), (WebSocketMessageType)(int)type, !isContinued, cancellation);
    }

    public async Task<WebSocketReceival> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellation = default) {
      ReadOnlyMemory<byte> readOnlyMemory = buffer;
      var segment = GetSegment(readOnlyMemory);
      var result = await _webSocket!.ReceiveAsync(segment, cancellation).ConfigureAwait(false);
      return new WebSocketReceival(result.Count, (MessageType)(int)result.MessageType, !result.EndOfMessage, (int?)result.CloseStatus, result.CloseStatusDescription);
    }

    public Task CloseAsync(int? code = null, string? reason = null, CancellationToken cancellation = default) {
      return _webSocket.CloseAsync((WebSocketCloseStatus?)code ?? WebSocketCloseStatus.NormalClosure, reason, cancellation);
    }

    public void Dispose() {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          // dispose managed state (managed objects)

        }

        // free unmanaged resources (unmanaged objects) and override finalizer
        _isDisposed = true;
      }
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
