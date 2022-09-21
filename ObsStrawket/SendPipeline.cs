using MessagePack;
using Microsoft.Extensions.Logging;
using ObsStrawket.DataTypes;
using ObsStrawket.Serialization;
using System;
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
    private readonly PipeOptions _pipeOptions = new(useSynchronizationContext: false);
    private readonly Channel<Deferred<Pipe, object?>> _sendQueue = Channel.CreateUnbounded<Deferred<Pipe, object?>>();

    public Task? SendTask { get; private set; }

    public SendPipeline(WebSocket socket, ILogger? logger = null) {
      _socket = socket;
      _logger = logger;
    }

    public void Start() {
      SendTask ??= Task.Run(LoopWebsocketSendAsync);
    }

    public void Stop() {
      _sendQueue.Writer.TryComplete();
    }

    public async Task SendAsync(IOpCodeMessage value, CancellationToken cancellation = default) {
      using var _1 = _logger?.BeginScope(nameof(SendAsync));

      var token = cancellation;
      token.ThrowIfCancellationRequested();

      var pipe = new Pipe(_pipeOptions);
      MessagePackSerializer.Serialize(pipe.Writer, value, _serialOptions, token);
      token.ThrowIfCancellationRequested();

      var output = new TaskCompletionSource<object?>();
      await _sendQueue.Writer.WriteAsync(new(pipe, output, token), token).ConfigureAwait(false);

      _logger?.LogDebug("Await sending {}", value.GetType().Name);
      await output.Task.ConfigureAwait(false);
      _logger?.LogDebug("Sent {}", value.GetType().Name);
    }

    private async Task LoopWebsocketSendAsync() {
      using var _1 = _logger?.BeginScope(nameof(LoopWebsocketSendAsync));
      var queue = _sendQueue.Reader;

      try {
        while (await queue.WaitToReadAsync().ConfigureAwait(false)) {
          var item = await queue.ReadAsync().ConfigureAwait(false);
          if (item.Cancellation.IsCancellationRequested) {
            _logger?.LogInformation("User cancelled request");
            item.Output.SetException(new OperationCanceledException(item.Cancellation));
            continue;
          }

          try {
            var sequence = await PipelineHelpers.RealAllAsync(item.Input, default).ConfigureAwait(false);
            long remainingBytes = sequence.Length;
            foreach (var memory in sequence) {
              var segment = PipelineHelpers.GetSegment(memory);
              var cut = new ArraySegment<byte>(segment.Array, segment.Offset, segment.Count);
              remainingBytes -= memory.Length;
              bool isEnd = remainingBytes == 0;

              _logger?.LogDebug("Send {cut} bytes: {bytes}", cut.Count, BitConverter.ToString(cut.Array, cut.Offset, cut.Count));
              await _socket.SendAsync(cut, WebSocketMessageType.Binary, isEnd, default).ConfigureAwait(false);
            }
            item.Output.SetResult(null);
          }
          catch (Exception exception) {
            item.Output.SetException(exception);
            throw;
          }
        }
      }
      catch (Exception fault) {
        _logger?.LogDebug(fault, "Quit");
        _sendQueue.Writer.TryComplete(fault);
        while (!queue.Completion.IsCompleted) {
          var item = await queue.ReadAsync(default).ConfigureAwait(false);
          item.Output.SetException(fault);
        }
      }
    }
  }
}
