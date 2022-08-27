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
  using SuperSocket.ClientEngine;

  public class WebSocket4NetSocket : IClientWebSocket {
    private readonly ClientOptions _options = new();
    private readonly List<ReadOnlyMemory<byte>> _pendingSend = new();
    private readonly Channel<EventArgs> _receivals = Channel.CreateUnbounded<EventArgs>();
    private CancellationTokenSource _cancellation = new();
    private Memory<byte>? _pendingReceival;
    private WebSocket? _socket;
    private bool _isDisposed;

    public IClientWebSocketOptions Options { get => _options; }

    public async Task ConnectAsync(Uri uri, CancellationToken cancellation = default) {
      _cancellation.Cancel();
      _cancellation.Dispose();
      _socket?.Dispose();
      _cancellation = new();

      string subProtocol = _options.SubProtocols.FirstOrDefault();
      _socket = new WebSocket($"{uri}", subProtocol);
      _socket.MessageReceived += ReceiveEvent;
      _socket.DataReceived += ReceiveEvent;
      _socket.Error += ReceiveEvent;
      _socket.Closed += ReceiveEvent;

      bool success = await _socket.OpenAsync().ConfigureAwait(false);
      if (!success) {
        var token = GetCancellationToken(cancellation);
        while (await _receivals.Reader.ReadAsync(token).ConfigureAwait(false) is ErrorEventArgs error) {
          throw error.Exception;
        }
      }
    }

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

      var token = GetCancellationToken(cancellation);
      var receival = await _receivals.Reader.ReadAsync(token).ConfigureAwait(false);
      switch (receival) {
      case ErrorEventArgs error:
        throw error.Exception;
      case MessageReceivedEventArgs _:
        throw new NotImplementedException();
      case DataReceivedEventArgs data:
        int writeLength = CopyAsMuchAsPossible(data.Data, buffer.Span);
        return new WebSocketReceival(writeLength, MessageType.Binary, _pendingReceival != null);
      default:
        return new WebSocketReceival(0, MessageType.Close, CloseStatus: 1004, CloseStatusDescription: "Closed");
      }
      throw new Exception("unreachable");
    }

    public async Task CloseAsync(int? code = null, string? reason = null, CancellationToken cancellation = default) {
      if (_socket?.State != WebSocketState.Open || _socket.State != WebSocketState.Connecting) {
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
          _cancellation.Cancel();
          _cancellation.Dispose();
          _socket?.Dispose();
        }

        // free unmanaged resources (unmanaged objects) and override finalizer
        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        _isDisposed = true;
      }
    }

    private CancellationToken GetCancellationToken(CancellationToken cancellation) {
      return CancellationTokenSource.CreateLinkedTokenSource(_cancellation.Token, cancellation).Token;
    }

#pragma warning disable VSTHRD100 // Avoid async void methods
    private async void ReceiveEvent(object sender, EventArgs ev) {
      try {
        await _receivals.Writer.WriteAsync(ev).ConfigureAwait(false);
      }
      catch (Exception) {
        // TODO: log
      }
    }
#pragma warning restore VSTHRD100 // Avoid async void methods

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
