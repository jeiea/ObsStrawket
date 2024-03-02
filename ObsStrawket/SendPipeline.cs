using Microsoft.Extensions.Logging;
using ObsStrawket.DataTypes;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {

  internal class SendPipeline {
    private readonly WebSocket _socket;
    private readonly ILogger? _logger;
    private readonly Channel<Deferred<byte[], object?>> _sendQueue = Channel.CreateUnbounded<Deferred<byte[], object?>>();

    public SendPipeline(WebSocket socket, ILogger? logger = null) {
      _socket = socket;
      _logger = logger;
    }

    public Task? SendTask { get; private set; }

    public void Start() {
      SendTask ??= Task.Run(LoopWebsocketSendAsync);
    }

    public void Stop() {
      _sendQueue.Writer.TryComplete();
    }

    public async Task SendAsync(IOpCodeMessage value, CancellationToken token = default) {
      using var _1 = _logger?.BeginScope(nameof(SendAsync));
      token.ThrowIfCancellationRequested();

      var ms = new MemoryStream();

      byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value, (JsonSerializerOptions?)null);
      token.ThrowIfCancellationRequested();

      var output = new TaskCompletionSource<object?>();
      await _sendQueue.Writer.WriteAsync(new(bytes, output, token), token).ConfigureAwait(false);

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
            var segment = new ArraySegment<byte>(item.Input);
            await _socket.SendAsync(segment, WebSocketMessageType.Binary, true, default).ConfigureAwait(false);
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
