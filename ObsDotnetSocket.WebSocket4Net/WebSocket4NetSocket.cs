namespace ObsDotnetSocket.WebSocket4Net {
  using ObsDotnetSocket.WebSocket;
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Channels;
  using System.Runtime.InteropServices;
  using global::WebSocket4Net;
  using System.Text;
  using System.Collections.Concurrent;
  using Nerdbank.Streams;
  using System.Buffers;

  public class WebSocket4NetSocket : IClientWebSocket {
    private readonly ClientOptions _options = new();
    private readonly Channel<EventArgs> _receivals = Channel.CreateUnbounded<EventArgs>();
    private readonly List<ReadOnlyMemory<byte>> _pendingSend = new();
    private Memory<byte>? _pendingReceival;
    private WebSocket? _socket;
    private bool _isDisposed;

    public IClientWebSocketOptions Options { get => _options; }

    public Task ConnectAsync(Uri uri, CancellationToken cancellation = default) {
      _socket = new WebSocket($"{uri}", _options.SubProtocols.FirstOrDefault());
      var taskSource = new TaskCompletionSource<bool>();
      _socket.MessageReceived += ReceiveMessage;
      return _socket.OpenAsync();
    }

#pragma warning disable VSTHRD100 // Avoid async void methods
    private async void ReceiveMessage(object sender, MessageReceivedEventArgs ev) {
      try {
        await _receivals.Writer.WriteAsync(ev).ConfigureAwait(false);
      }
      catch (Exception) {
        // TODO: log
      }
    }
#pragma warning restore VSTHRD100 // Avoid async void methods

    public Task SendAsync(ReadOnlyMemory<byte> memory, MessageType type, bool isContinued = false, CancellationToken cancellation = default) {
      _pendingSend.Add(memory);
      if (!isContinued) {
        _socket!.Send(_pendingSend.Select(GetSegment).ToList());
        _pendingSend.Clear();
      }
      return Task.CompletedTask;
    }

    public async Task<WebSocketReceival> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellation = default) {
      if (_pendingReceival is Memory<byte> pending) {
        int writeLength = CopyAsMuchAsPossible(pending, buffer.Span);
        return new WebSocketReceival(writeLength, MessageType.Binary);
      }

      EventArgs? receival;
      while (!_receivals.Reader.TryPeek(out receival)) {
        await _receivals.Reader.WaitToReadAsync(cancellation).ConfigureAwait(false);
      }

      switch (receival) {
      case MessageReceivedEventArgs _:
        throw new NotImplementedException();
      case DataReceivedEventArgs data:
        int writeLength = CopyAsMuchAsPossible(data.Data, buffer.Span);
        return new WebSocketReceival(writeLength, MessageType.Binary, _pendingReceival != null);
      }
      throw new Exception("unreachable");
    }

    private int CopyAsMuchAsPossible(Memory<byte> source, Span<byte> buffer) {
      int writeLength = Math.Min(source.Length, buffer.Length);
      source.Span.Slice(0, writeLength).CopyTo(buffer);
      if (writeLength < source.Length) {
        _pendingReceival = source.Slice(writeLength, source.Length - writeLength);
      }
      else {
        _pendingReceival = null;
      }

      return writeLength;
    }

    public async Task CloseAsync(int? code = null, string? reason = null, CancellationToken cancellation = default) {
      if (_socket == null) {
        return;
      }
      await _socket.CloseAsync().ConfigureAwait(false);
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
          _socket?.Dispose();
        }

        // free unmanaged resources (unmanaged objects) and override finalizer
        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
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

    class ClientOptions : IClientWebSocketOptions {
      public readonly HashSet<string> SubProtocols = new();

      public void AddSubProtocol(string subProtocol) {
        SubProtocols.Add(subProtocol);
      }
    }
  }
}
