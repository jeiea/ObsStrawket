using MessagePack;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using ObsStrawket.DataTypes;
using ObsStrawket.Serialization;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ObsStrawket {
  internal class ReceivePipeline : IDisposable {
    private static readonly MessagePackSerializerOptions _serialOptions = new(OpCodeMessageResolver.Instance);

    private readonly WebSocket _socket;
    private readonly ILogger? _logger;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly Pipe _pipe = new();

    public Task? ReceiveTask { get; private set; }

    public ReceivePipeline(WebSocket socket, ILogger? logger = null) {
      _socket = socket;
      _logger = logger;
    }

    public void Run() {
      ReceiveTask ??= ReadWebSocketPipeAsync();
    }

    public async Task<IOpCodeMessage?> ReceiveAsync(CancellationToken cancellation = default) {
      using var _1 = _logger?.BeginScope(nameof(ReceiveAsync));
      using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation, _cancellation.Token);
      var linked = linkedSource.Token;
      linked.ThrowIfCancellationRequested();

      var reader = _pipe.Reader;
      int length = await PipelineHelpers.ReadLengthAsync(reader, _logger, linked).ConfigureAwait(false);
      _logger?.LogDebug("read length: {}", length);
      if (length == -1) {
        return null;
      }

      var result = await reader.ReadAtLeastAsync(length, default).ConfigureAwait(false);
      var buffer = result.Buffer;
      try {
        if (buffer.Length < length) {
          _logger?.LogDebug("Buffer cut: {}, result.IsCancelled: {}, IsCompleted: {}",
            buffer.Length, result.IsCanceled, result.IsCompleted);
          return null;
        }

        linked.ThrowIfCancellationRequested();
        var message = buffer.Slice(0, length);
        return MessagePackSerializer.Deserialize<IOpCodeMessage>(message, _serialOptions, linked);
      }
      finally {
        reader.AdvanceTo(buffer.GetPosition(length));
      }
    }

    public void Dispose() {
      _cancellation.Cancel();
      _cancellation.Dispose();
    }

    private async Task ReadWebSocketPipeAsync(int sizeHint = 0) {
      using var _1 = _logger?.BeginScope(nameof(ReadWebSocketPipeAsync));
      var writer = _pipe.Writer;
      var token = _cancellation.Token;

      try {
        var prefixer = new PrefixingBufferWriter<byte>(writer, sizeof(int));

        while (true) {
          token.ThrowIfCancellationRequested();
          var memory = prefixer.GetMemory(sizeHint);
          using var handle = memory.Pin();
          var segment = PipelineHelpers.GetSegment(memory);
          var readResult = await _socket.ReceiveAsync(segment, token).ConfigureAwait(false);
          _logger?.LogDebug("received: {}", BitConverter.ToString(segment.Array, segment.Offset, readResult.Count));

          if (_socket.State == WebSocketState.CloseReceived && readResult.MessageType == WebSocketMessageType.Close) {
            // Tell the PipeReader that there's no more data coming
            await writer.CompleteAsync().ConfigureAwait(false);
            break;
          }

          _logger?.LogDebug("Advance WebsocketRead {}", sizeof(int) + readResult.Count);
          prefixer.Advance(readResult.Count);
          if (readResult.EndOfMessage) {
            BitConverter.GetBytes((int)prefixer.Length).CopyTo(prefixer.Prefix.Span);
            _logger?.LogDebug("flush messageLength: {}", prefixer.Length);
            prefixer.Commit();
            var result = await writer.FlushAsync(token).ConfigureAwait(false);
            if (result.IsCompleted) {
              break;
            }
          }
        }
      }
      catch (Exception exception) {
        // Propagate the exception to the reader.
        await writer.CompleteAsync(exception).ConfigureAwait(false);
        _logger?.LogDebug(exception, "Complete with exception");
      }
    }
  }
}
