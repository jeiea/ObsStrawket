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
      ReceiveTask ??= LoopWebSocketReadAsync(token: token);
    }

    private async Task LoopWebSocketReadAsync(int sizeHint = 0, CancellationToken token = default) {
      using var _1 = _logger?.BeginScope(nameof(LoopWebSocketReadAsync));
      var options = new PipeOptions(useSynchronizationContext: false);
      var deserialization = Task.CompletedTask;

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
            break;
          }

          _logger?.LogDebug("Advancing prefixer {} bytes", readResult.Count);
          writer.Advance(readResult.Count);
          if (readResult.EndOfMessage) {
            var completedPipe = pipe;
            await _messages.Writer.WriteAsync(Task.Run(() => DeserializeAsync(completedPipe, token), token), token);

            pipe = new Pipe(options);
            writer = pipe.Writer;
          }
        }
        await deserialization.ConfigureAwait(false);
        _messages.Writer.TryComplete();
        _logger?.LogDebug("Complete. IsCancellationRequested: {}", token.IsCancellationRequested);
      }
      catch (Exception exception) {
        _logger?.LogDebug(exception, "Complete with exception");
        await deserialization.ConfigureAwait(false);
        _messages.Writer.TryComplete(exception);
      }
    }

    private static async Task<IOpCodeMessage> DeserializeAsync(Pipe pipe, CancellationToken token = default) {
      int length = (int)pipe.Writer.UnflushedBytes;

      await pipe.Writer.CompleteAsync().ConfigureAwait(false);
      var result = await pipe.Reader.ReadAtLeastAsync(length, token).ConfigureAwait(false);
      var buffer = result.Buffer.Slice(0, length);
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

