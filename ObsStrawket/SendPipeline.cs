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
  internal class SendPipeline {
    private static readonly MessagePackSerializerOptions _serialOptions = new(OpCodeMessageResolver.Instance);

    private readonly WebSocket _socket;
    private readonly ILogger? _logger;
    private readonly Pipe _writer = new();
    private readonly Channel<Deferred<object?>> _sendQueue = Channel.CreateBounded<Deferred<object?>>(1);

    public Task? SendTask { get; private set; }

    public SendPipeline(WebSocket socket, ILogger? logger = null) {
      _socket = socket;
      _logger = logger;
    }

    public void Run(CancellationToken cancellation = default) {
      SendTask ??= Task.Run(() => LoopSendAsync(cancellation), cancellation);
    }

    public async Task SendAsync(IOpCodeMessage value, CancellationToken cancellation = default) {
      using var _1 = _logger?.BeginScope(nameof(SendAsync));

      var token = cancellation;
      token.ThrowIfCancellationRequested();

      var prefixer = new PrefixingBufferWriter<byte>(_writer.Writer, sizeof(int));
      MessagePackSerializer.Serialize(prefixer, value, _serialOptions, token);
      byte[] prefix = BitConverter.GetBytes((int)prefixer.Length);
      prefix.CopyTo(prefixer.Prefix.Span);

      token.ThrowIfCancellationRequested();
      prefixer.Commit();

      _logger?.LogTrace(
        "prefixer.Length: {uncommit}, _writer.UnflushedBytes: {unflushed}, prefix: 0x{a3:x2}{a2:x2}{a1:x2}{a0:x2}",
        prefixer.Length, _writer.Writer.UnflushedBytes,
        prefixer.Prefix.Span[3], prefixer.Prefix.Span[2], prefixer.Prefix.Span[1], prefixer.Prefix.Span[0]
      );
      var output = new TaskCompletionSource<object?>();
      await _sendQueue.Writer.WriteAsync(new(output, token), token).ConfigureAwait(false);
      await _writer.Writer.FlushAsync(token).ConfigureAwait(false);

      _logger?.LogTrace("output await {}", value.GetType().Name);
      await output.Task.ConfigureAwait(false);
      _logger?.LogTrace("output awaited {}", value.GetType().Name);
    }

    private async Task LoopSendAsync(CancellationToken cancellation) {
      using var _1 = _logger?.BeginScope(nameof(LoopSendAsync));
      var queue = _sendQueue.Reader;
      var bytes = _writer.Reader;
      var token = cancellation;

      try {
        while (await queue.WaitToReadAsync(token).ConfigureAwait(false)) {
          int messageLength = await PipelineHelpers.ReadLengthAsync(bytes, _logger, token).ConfigureAwait(false);
          var readResult = await bytes.ReadAtLeastAsync(messageLength, token).ConfigureAwait(false);
          _logger?.LogTrace("readResult.Length: {}", readResult.Buffer.Length);
          var item = await queue.ReadAsync(token).ConfigureAwait(false);
          try {
            _logger?.LogTrace("read item");
            await SendExclusivelyAsync(item, messageLength, readResult).ConfigureAwait(false);
          }
          catch (Exception exception) {
            item.Output.SetException(exception);
            throw;
          }
          finally {
            bytes.AdvanceTo(readResult.Buffer.GetPosition(messageLength));
          }
        }
      }
      catch (Exception fault) {
        _logger?.LogDebug("quit: {}", fault);
        _sendQueue.Writer.TryComplete(fault);
        await bytes.CompleteAsync(fault).ConfigureAwait(false);
        while (!queue.Completion.IsCompleted) {
          var item = await queue.ReadAsync(default).ConfigureAwait(false);
          item.Output.SetException(fault);
        }
      }
    }

    private async Task SendExclusivelyAsync(Deferred<object?> item, int messageLength, ReadResult readResult) {
      using var _1 = _logger?.BeginScope(nameof(SendExclusivelyAsync));

      var (output, cancellation) = item;
      if (cancellation.IsCancellationRequested) {
        output.SetException(new OperationCanceledException(cancellation));
        return;
      }

      var buffer = readResult.Buffer;
      long bytesRemaining = messageLength;
      foreach (var memory in buffer) {
        using var _2 = memory.Pin();
        int count = Math.Min(memory.Length, (int)bytesRemaining);
        bytesRemaining -= count;
        bool isEnd = bytesRemaining <= 0;

        var segment = PipelineHelpers.GetSegment(memory);
        var cut = new ArraySegment<byte>(segment.Array, segment.Offset, count);
        _logger?.LogDebug("advanceTo: {}, bytesRemaining: {}, data: {}", messageLength, bytesRemaining, BitConverter.ToString(cut.Array, cut.Offset, cut.Count));
        await _socket.SendAsync(cut, WebSocketMessageType.Binary, isEnd, cancellation).ConfigureAwait(false);

        if (isEnd) {
          break;
        }
      }
      _logger?.LogTrace("SetResult null");
      output.SetResult(null);
    }
  }
}
