using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Diagnostics;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {

  internal class ReceivePipeline {
    private readonly WebSocket _socket;
    private readonly Action<PipelineEvent>? _emit;
    private readonly Channel<Task<IOpCodeMessage>> _messages = Channel.CreateUnbounded<Task<IOpCodeMessage>>();

    public ReceivePipeline(WebSocket socket, Action<PipelineEvent>? emit = null) {
      _socket = socket;
      _emit = emit;
    }

    public ChannelReader<Task<IOpCodeMessage>> Messages => _messages.Reader;

    public Task? ReceiveTask { get; private set; }

    public void Run(CancellationToken token) {
      ReceiveTask ??= LoopWebSocketReceiveAsync(token: token);
    }

    private static IOpCodeMessage DeserializeAsync(MemoryStream ms) {
      var span = new ReadOnlySpan<byte>(ms.GetBuffer(), 0, (int)ms.Length);
      var result = JsonSerializer.Deserialize<IOpCodeMessage>(span, (JsonSerializerOptions?)null) ?? throw new InvalidOperationException("Deserialization failed");
      ms.Dispose();
      return result;
    }

    private async Task LoopWebSocketReceiveAsync(CancellationToken token = default) {
      try {
        _emit?.Invoke(new PipelineTrace(PipelineLevel.Debug, "Start."));
        var ms = new MemoryStream();
        byte[] buffer = new byte[4 * 1024];
        var segment = new ArraySegment<byte>(buffer);

        while (!token.IsCancellationRequested) {
          var readResult = await _socket.ReceiveAsync(segment, token).ConfigureAwait(false);

          if (_socket.State == WebSocketState.CloseReceived && readResult.MessageType == WebSocketMessageType.Close) {
            _emit?.Invoke(new PipelineTrace(PipelineLevel.Debug, "Exit by websocket close"));
            _ = (int?)_socket.CloseStatus switch {
              (int?)WebSocketCloseCode.AuthenticationFailed => _messages.Writer.TryComplete(new AuthenticationFailureException()),
              _ => _messages.Writer.TryComplete(new WebsocketCloseReceivedException(code: (int?)_socket.CloseStatus)),
            };
            break;
          }

          _emit?.Invoke(new PipelineTrace(PipelineLevel.Debug, $"Read {readResult.Count} bytes"));
          ms.Write(buffer, 0, readResult.Count);

          if (readResult.EndOfMessage) {
            var memory = ms;
            await _messages.Writer.WriteAsync(Task.Run(() => DeserializeAsync(memory), token), token).ConfigureAwait(false);
            ms = new MemoryStream();
          }
        }

        _ = _messages.Writer.TryComplete();
        _emit?.Invoke(new PipelineTrace(PipelineLevel.Debug, $"Complete. IsCancellationRequested: {token.IsCancellationRequested}"));
      }
      catch (Exception exception) {
        _emit?.Invoke(new PipelineTrace(PipelineLevel.Debug, $"Complete with exception: {exception.Message}", exception));
        _ = _messages.Writer.TryComplete(exception);
      }
    }
  }

  internal class LazyString {
    private string? _evaluated;

    public Func<string> Builder {
      get;
      set {
        _evaluated = null;
        field = value;
      }
    } = static () => "";

    public override string ToString() {
      return _evaluated ??= Builder();
    }
  }
}
