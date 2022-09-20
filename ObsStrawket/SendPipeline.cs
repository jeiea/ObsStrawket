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

    public void Start() {
      SendTask ??= Task.Run(LoopSendAsync);
    }

    public void Stop() {
      _sendQueue.Writer.TryComplete();
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

      _logger?.LogDebug(
        "prefixer.Length: {uncommit}, prefix: 0x{a3:x2}{a2:x2}{a1:x2}{a0:x2}",
        prefixer.Length, prefixer.Prefix.Span[3], prefixer.Prefix.Span[2], prefixer.Prefix.Span[1], prefixer.Prefix.Span[0]
      );
      prefixer.Commit();

      var output = new TaskCompletionSource<object?>();
      await _sendQueue.Writer.WriteAsync(new(output, token), token).ConfigureAwait(false);
      await _writer.Writer.FlushAsync(token).ConfigureAwait(false);

      _logger?.LogDebug("Await sending {}", value.GetType().Name);
      await output.Task.ConfigureAwait(false);
      _logger?.LogDebug("Sent {}", value.GetType().Name);
    }

    private async Task LoopSendAsync() {
      using var _1 = _logger?.BeginScope(nameof(LoopSendAsync));
      var queue = _sendQueue.Reader;
      var bytes = _writer.Reader;

      try {
        while (await queue.WaitToReadAsync().ConfigureAwait(false)) {
          int messageLength = await PipelineHelpers.ReadLengthAsync(bytes, _logger).ConfigureAwait(false);
          var readResult = await bytes.ReadAtLeastAsync(messageLength).ConfigureAwait(false);
          _logger?.LogDebug("readResult.Length: {}", readResult.Buffer.Length);
          var item = await queue.ReadAsync().ConfigureAwait(false);
          try {
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
        _logger?.LogDebug(fault, "Quit");
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
        _logger?.LogInformation("User cancelled request");
        output.SetException(new OperationCanceledException(cancellation));
        return;
      }

      var buffer = readResult.Buffer;
      long bytesRemaining = messageLength;
      foreach (var memory in buffer) {
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

      _logger?.LogDebug("Sent request: {} bytes", messageLength);
      output.SetResult(null);
    }
  }
}
