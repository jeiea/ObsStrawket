using ObsStrawket.DataTypes;
using ObsStrawket.Diagnostics;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {

  internal class SendPipeline {
    private readonly WebSocket _socket;
    private readonly Channel<Deferred<byte[], object?>> _sendQueue = Channel.CreateUnbounded<Deferred<byte[], object?>>();

    private readonly JsonSerializerOptions _serializerOptions = new() {
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public SendPipeline(WebSocket socket) {
      _socket = socket;
    }

    public Task? SendTask { get; private set; }

    public Action<PipelineEvent>? Emit { get; set; }

    public void Start() {
      SendTask ??= Task.Run(LoopWebsocketSendAsync);
    }

    public void Stop() {
      _ = _sendQueue.Writer.TryComplete();
    }

    public async Task SendAsync(IOpCodeMessage value, CancellationToken token = default) {
      token.ThrowIfCancellationRequested();

      byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);
      token.ThrowIfCancellationRequested();

      var output = new TaskCompletionSource<object?>();
      await _sendQueue.Writer.WriteAsync(new(bytes, output, token), token).ConfigureAwait(false);

      _ = await output.Task.ConfigureAwait(false);
    }

    private async Task LoopWebsocketSendAsync() {
      var queue = _sendQueue.Reader;

      try {
        while (await queue.WaitToReadAsync().ConfigureAwait(false)) {
          var item = await queue.ReadAsync().ConfigureAwait(false);
          if (item.Cancellation.IsCancellationRequested) {
            item.Output.SetException(new OperationCanceledException(item.Cancellation));
            continue;
          }

          try {
            var segment = new ArraySegment<byte>(item.Input);
            await _socket.SendAsync(segment, WebSocketMessageType.Text, true, default).ConfigureAwait(false);
            item.Output.SetResult(null);
            Emit?.Invoke(new WebSocketPayloadSent(Encoding.UTF8.GetString(item.Input)));
          }
          catch (Exception exception) {
            item.Output.SetException(exception);
            throw;
          }
        }
      }
      catch (Exception fault) {
        _ = _sendQueue.Writer.TryComplete(fault);
        while (!queue.Completion.IsCompleted) {
          var item = await queue.ReadAsync(default).ConfigureAwait(false);
          item.Output.SetException(fault);
        }
      }
    }
  }
}
