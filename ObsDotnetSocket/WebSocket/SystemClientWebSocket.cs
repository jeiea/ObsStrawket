namespace ObsDotnetSocket.WebSocket {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.WebSockets;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Threading.Tasks;

  internal class SystemClientWebSocket : SystemWebSocket, IClientWebSocket {
    public IClientWebSocketOptions Options { get => _options; }
    private ClientOptions _options = new();

    public Task ConnectAsync(Uri uri, CancellationToken cancellation = default) {
      _webSocket?.Dispose();
      var socket = new ClientWebSocket();
      socket.Options.AddSubProtocol(_options.SubProtocols.First());
      _webSocket = socket;
      return socket.ConnectAsync(uri, cancellation);
    }

    class ClientOptions : IClientWebSocketOptions {
      public HashSet<string> SubProtocols { get; private set; } = new();

      public void AddSubProtocol(string subProtocol) {
        SubProtocols.Add(subProtocol);
      }
    }
  }

  internal class SystemWebSocket : IWebSocket {
    protected WebSocket? _webSocket;
    private bool _isDisposed;

    public Task SendAsync(ReadOnlyMemory<byte> memory, MessageType type, bool isContinued = false, CancellationToken cancellation = default) {
      return _webSocket!.SendAsync(GetSegment(memory), (WebSocketMessageType)(int)type, !isContinued, cancellation);
    }

    public async Task<WebSocketReceival> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellation = default) {
      ReadOnlyMemory<byte> readOnlyMemory = buffer;
      var segment = GetSegment(readOnlyMemory);
      var result = await _webSocket!.ReceiveAsync(segment, cancellation).ConfigureAwait(false);
      return new WebSocketReceival(result.Count, (MessageType)(int)result.MessageType, !result.EndOfMessage, (int?)result.CloseStatus, result.CloseStatusDescription);
    }

    public Task CloseAsync(int? code = null, string? reason = null, CancellationToken cancellation = default) {
      return _webSocket!.CloseAsync((WebSocketCloseStatus?)code ?? WebSocketCloseStatus.NormalClosure, reason, cancellation);
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
