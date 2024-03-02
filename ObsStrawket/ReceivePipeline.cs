using Microsoft.Extensions.Logging;
using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {

  internal class ReceivePipeline {
    private readonly WebSocket _socket;
    private readonly ILogger? _logger;
    private readonly Channel<Task<IOpCodeMessage>> _messages = Channel.CreateUnbounded<Task<IOpCodeMessage>>();

    public ReceivePipeline(WebSocket socket, ILogger? logger = null) {
      _socket = socket;
      _logger = logger;
    }

    public ChannelReader<Task<IOpCodeMessage>> Messages { get => _messages.Reader; }

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
      using var _1 = _logger?.BeginScope(nameof(LoopWebSocketReceiveAsync));

      try {
        _logger?.LogDebug("Start.");
        var ms = new MemoryStream();
        var segment = new ArraySegment<byte>(new byte[4 * 1024]);

        while (!token.IsCancellationRequested) {
          var readResult = await _socket.ReceiveAsync(segment, token).ConfigureAwait(false);

          if (_socket.State == WebSocketState.CloseReceived && readResult.MessageType == WebSocketMessageType.Close) {
            _logger?.LogDebug("Exit by websocket close");
            switch ((int?)_socket.CloseStatus) {
            case (int?)WebSocketCloseCode.AuthenticationFailed:
              _messages.Writer.TryComplete(new AuthenticationFailureException());
              break;

            default:
              _messages.Writer.TryComplete(new WebsocketCloseReceivedException(code: (int?)_socket.CloseStatus));
              break;
            }
            break;
          }

          _logger?.LogDebug("Read {} bytes", readResult.Count);
          ms.Write(segment.Array, segment.Offset, readResult.Count);

          if (readResult.EndOfMessage) {
            var memory = ms;
            await _messages.Writer.WriteAsync(Task.Run(() => DeserializeAsync(memory), token), token).ConfigureAwait(false);
            ms = new MemoryStream();
          }
        }

        _messages.Writer.TryComplete();
        _logger?.LogDebug("Complete. IsCancellationRequested: {}", token.IsCancellationRequested);
      }
      catch (Exception exception) {
        _logger?.LogDebug(exception, "Complete with exception: {message}", exception.Message);
        _messages.Writer.TryComplete(exception);
      }
    }
  }

  internal class LazyString {
    private Func<string> _builder = () => "";
    private string? _evaluated;

    public Func<string> Builder {
      get => _builder;
      set {
        _evaluated = null;
        _builder = value;
      }
    }

    public override string ToString() {
      return _evaluated ??= Builder();
    }
  }
}
