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
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {
  internal class ReceivePipeline : IDisposable {
    private static readonly MessagePackSerializerOptions _serialOptions = new(OpCodeMessageResolver.Instance);

    private readonly WebSocket _socket;
    private readonly ILogger? _logger;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly Channel<IOpCodeMessage> _messages = Channel.CreateUnbounded<IOpCodeMessage>();
    private readonly Pipe _pipe = new();

    public ChannelReader<IOpCodeMessage> Messages { get => _messages.Reader; }

    public Task? ReceiveTask { get; private set; }

    public ReceivePipeline(WebSocket socket, ILogger? logger = null) {
      _socket = socket;
      _logger = logger;
    }

    public void Run() {
      ReceiveTask ??= Task.WhenAll(LoopWebSocketReadAsync(), LoopDeserializeAsync());
    }

    public void Dispose() {
      _cancellation.Cancel();
      _cancellation.Dispose();
    }

    private async Task LoopDeserializeAsync() {
      using var _1 = _logger?.BeginScope(nameof(LoopDeserializeAsync));
      var writer = _messages.Writer;
      var token = _cancellation.Token;

      try {
        try {
          while (!token.IsCancellationRequested) {
            var message = await ReceiveAsync(token).ConfigureAwait(false);
            if (message == null) {
              break;
            }

            await writer.WriteAsync(message).ConfigureAwait(false);
          }
        }
        catch (OperationCanceledException) {
          // Regard as normal complete
        }
        writer.Complete();
        await _pipe.Reader.CompleteAsync().ConfigureAwait(false);
        _logger?.LogTrace("Exit, IsCancellationRequested: {}", _cancellation.IsCancellationRequested);
      }
      catch (Exception exception) {
        writer.TryComplete(exception);
        await _pipe.Reader.CompleteAsync(exception).ConfigureAwait(false);
        _logger?.LogDebug("Exit, {}", exception);
      }
    }

    private async Task<IOpCodeMessage?> ReceiveAsync(CancellationToken token) {
      var reader = _pipe.Reader;
      if (token.IsCancellationRequested) {
        return null;
      }

      int length = await PipelineHelpers.ReadLengthAsync(reader, _logger, token).ConfigureAwait(false);
      _logger?.LogTrace("read length: {}", length);
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

        if (token.IsCancellationRequested) {
          return null;
        }
        var message = buffer.Slice(0, length);
        return MessagePackSerializer.Deserialize<IOpCodeMessage>(message, _serialOptions, token);
      }
      finally {
        reader.AdvanceTo(buffer.GetPosition(length));
      }
    }

    private async Task LoopWebSocketReadAsync(int sizeHint = 0) {
      using var _1 = _logger?.BeginScope(nameof(LoopWebSocketReadAsync));
      var writer = _pipe.Writer;
      var token = _cancellation.Token;

      try {
        var prefixer = new PrefixingBufferWriter<byte>(writer, sizeof(int));

        while (!token.IsCancellationRequested) {
          var memory = prefixer.GetMemory(sizeHint);
          using var handle = memory.Pin();
          var segment = PipelineHelpers.GetSegment(memory);
          var readResult = await _socket.ReceiveAsync(segment, token).ConfigureAwait(false);
          _logger?.LogDebug("received: {}", BitConverter.ToString(segment.Array, segment.Offset, readResult.Count));

          if (_socket.State == WebSocketState.CloseReceived && readResult.MessageType == WebSocketMessageType.Close) {
            _logger?.LogDebug("Exit by websocket close");
            break;
          }

          _logger?.LogTrace("Advance WebsocketRead {}", sizeof(int) + readResult.Count);
          prefixer.Advance(readResult.Count);
          if (readResult.EndOfMessage) {
            BitConverter.GetBytes((int)prefixer.Length).CopyTo(prefixer.Prefix.Span);
            _logger?.LogTrace("flush messageLength: {}", prefixer.Length);
            prefixer.Commit();
            var result = await writer.FlushAsync(token).ConfigureAwait(false);
            if (result.IsCompleted) {
              break;
            }
          }
        }
        await writer.CompleteAsync().ConfigureAwait(false);
        _logger?.LogDebug("Complete, IsCancellationRequested: {}", token.IsCancellationRequested);
      }
      catch (Exception exception) {
        await writer.CompleteAsync(exception).ConfigureAwait(false);
        _logger?.LogDebug(exception, "Complete with exception");
      }
    }
  }
}
