using MessagePack;
using Microsoft.Extensions.Logging;
using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Serialization;
using System;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {
  internal class ReceivePipeline {
    private static readonly MessagePackSerializerOptions _serialOptions = new(OpCodeMessageResolver.Instance);

    private readonly WebSocket _socket;
    private readonly ILogger? _logger;
    private readonly Channel<Task<IOpCodeMessage>> _messages = Channel.CreateUnbounded<Task<IOpCodeMessage>>();
    private readonly LazyString bytesBuilder = new();

    public ChannelReader<Task<IOpCodeMessage>> Messages { get => _messages.Reader; }

    public Task? ReceiveTask { get; private set; }

    public ReceivePipeline(WebSocket socket, ILogger? logger = null) {
      _socket = socket;
      _logger = logger;
    }

    public void Run(CancellationToken token) {
      ReceiveTask ??= LoopWebSocketReceiveAsync(token: token);
    }

    private async Task LoopWebSocketReceiveAsync(int sizeHint = 0, CancellationToken token = default) {
      using var _1 = _logger?.BeginScope(nameof(LoopWebSocketReceiveAsync));
      var options = new PipeOptions(useSynchronizationContext: false);

      try {
        _logger?.LogDebug("Start.");

        var pipe = new Pipe(options);
        var writer = pipe.Writer;
        while (!token.IsCancellationRequested) {
          var memory = pipe.Writer.GetMemory(sizeHint);
          var segment = PipelineHelpers.GetSegment(memory);
          var readResult = await _socket.ReceiveAsync(segment, token).ConfigureAwait(false);

          bytesBuilder.Builder = () => BitConverter.ToString(segment.Array, segment.Offset, readResult.Count);
          _logger?.LogDebug("Received: {}", bytesBuilder);

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
          writer.Advance(readResult.Count);
          if (readResult.EndOfMessage) {
            var completedPipe = pipe;
            await _messages.Writer.WriteAsync(Task.Run(() => DeserializeAsync(completedPipe, token), token), token).ConfigureAwait(false);

            pipe = new Pipe(options);
            writer = pipe.Writer;
          }
        }
        _messages.Writer.TryComplete();
        _logger?.LogDebug("Complete. IsCancellationRequested: {}", token.IsCancellationRequested);
      }
      catch (Exception exception) {
        _logger?.LogDebug(exception, "Complete with exception");
        _messages.Writer.TryComplete(exception);
      }
    }

    private static async Task<IOpCodeMessage> DeserializeAsync(Pipe pipe, CancellationToken token = default) {
      var buffer = await PipelineHelpers.RealAllAsync(pipe, token).ConfigureAwait(false);
      var message = MessagePackSerializer.Deserialize<IOpCodeMessage>(buffer, _serialOptions, token);
      return message;
    }

  }

  internal class LazyString {
    Func<string> _builder = () => "";
    string? _evaluated;

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

