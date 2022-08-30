namespace ObsDotnetSocket {
  using MessagePack;
  using Microsoft.Extensions.Logging;
  using Nerdbank.Streams;
  using ObsDotnetSocket.DataTypes;
  using ObsDotnetSocket.Serialization;
  using System;
  using System.Buffers;
  using System.IO.Pipelines;
  using System.Net.WebSockets;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Threading.Channels;
  using System.Threading.Tasks;

  record class Deferred<I, O>(I Input, TaskCompletionSource<O> Output, CancellationToken Cancellation = default) {
    public Deferred(I Input, CancellationToken Cancellation = default) : this(Input, new(), Cancellation) { }
  }
  record class Deferred<O>(TaskCompletionSource<O> Output, CancellationToken Cancellation = default) : Deferred<object?, O>(null, Output, Cancellation) {
    public Deferred(CancellationToken Cancellation = default) : this(new(), Cancellation) { }
  }

  internal class MessagePackLayer : IDisposable {
    internal static readonly Uri defaultUri = new("ws://127.0.0.1:4455");
    private static readonly MessagePackSerializerOptions _serialOptions = new(OpCodeMessageResolver.Instance);

    private readonly WebSocket _socket;
    private readonly ILogger? _logger;
    private PipeReader? _reader;
    private readonly Pipe _writer = new();
    private readonly Channel<Deferred<object?>> _sendQueue =
      Channel.CreateBounded<Deferred<object?>>(1);
    private readonly CancellationTokenSource _cancellation = new();
    private readonly byte[] _lengthBuffer = new byte[4];

    public Task SendTask { get; private set; } = Task.CompletedTask;

    public MessagePackLayer(WebSocket socket, ILogger? logger = null) {
      _socket = socket;
      _logger = logger;
    }

    public void RunLoop() {
      if (_reader != null) {
        return;
      }
      _reader = UsePipeReader(_socket, logger: _logger, cancellation: _cancellation.Token);
      SendTask = Task.Run(RunSendLoopAsync);
    }

    public async Task<IOpCodeMessage?> ReceiveAsync(CancellationToken cancellation = default) {
      using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation, _cancellation.Token);
      var linked = linkedSource.Token;
      linked.ThrowIfCancellationRequested();

      int length = await ReadLengthAsync(_reader!, default).ConfigureAwait(false);
      _logger?.LogInformation("Receive read length: {}", length);
      var result = await _reader!.ReadAtLeastAsync(length, default).ConfigureAwait(false);
      try {
        if (result.Buffer.IsEmpty) {
          return null;
        }
        linked.ThrowIfCancellationRequested();
        var message = result.Buffer.Slice(0, length);
        return MessagePackSerializer.Deserialize<IOpCodeMessage>(message, _serialOptions, linked);
      }
      finally {
        _reader.AdvanceTo(result.Buffer.GetPosition(length));
      }
    }

    public async Task SendAsync(IOpCodeMessage value, CancellationToken cancellation = default) {
      using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation, _cancellation.Token);
      var linked = linkedSource.Token;
      linked.ThrowIfCancellationRequested();

      var prefixer = new PrefixingBufferWriter<byte>(_writer.Writer, sizeof(int));
      MessagePackSerializer.Serialize(prefixer, value, _serialOptions, linked);
      byte[] prefix = BitConverter.GetBytes((int)prefixer.Length);
      prefix.CopyTo(prefixer.Prefix.Span);

      linked.ThrowIfCancellationRequested();
      prefixer.Commit();

      _logger?.LogDebug("SendAsync prefixer: 0x{a0:x2}, 0x{a1:x2}, 0x{a2:x2}, 0x{a3:x2}",
        prefix[0], prefix[1], prefix[2], prefix[3]);
      _logger?.LogDebug("SendAsync prefixer.Prefix: 0x{a0:x2}, 0x{a1:x2}, 0x{a2:x2}, 0x{a3:x2}",
        prefixer.Prefix.Span[0], prefixer.Prefix.Span[1], prefixer.Prefix.Span[2], prefixer.Prefix.Span[3]);
      _logger?.LogDebug("SendAsync prefixer.Length: {}, _writer.UnflushedBytes: {}",
        prefixer.Length, _writer.Writer.UnflushedBytes);
      var output = new TaskCompletionSource<object?>();
      await _sendQueue.Writer.WriteAsync(new(output, linked), linked).ConfigureAwait(false);
      await _writer.Writer.FlushAsync(linked).ConfigureAwait(false);

      _logger?.LogDebug("SendAsync output await {}", value.GetType().Name);
      await output.Task.ConfigureAwait(false);
      _logger?.LogDebug("SendAsync output awaited {}", value.GetType().Name);
    }

    public void Cancel(Exception exception) {
      _sendQueue.Writer.Complete(exception);
      _writer.Writer.Complete(exception);
      _reader?.Complete(exception);
    }

    public void Dispose() {
      _cancellation.Cancel();
      _cancellation.Dispose();
    }

    private async Task RunSendLoopAsync() {
      var queue = _sendQueue.Reader;
      var bytes = _writer.Reader;

      try {
        while (await queue.WaitToReadAsync().ConfigureAwait(false)) {
          int messageLength = await ReadLengthAsync(bytes, default).ConfigureAwait(false);
          var readResult = await bytes.ReadAtLeastAsync(messageLength, default).ConfigureAwait(false);
          _logger?.LogDebug("RunSendLoopAsync readResult.Length: {}", readResult.Buffer.Length);
          var item = await queue.ReadAsync().ConfigureAwait(false);
          try {
            _logger?.LogTrace("RunSendLoopAsync read item");
            await SendExclusivelyAsync(item, messageLength, readResult).ConfigureAwait(false);
          }
          catch (Exception exception) {
            item.Output.SetException(exception);
          }
          finally {
            bytes.AdvanceTo(readResult.Buffer.GetPosition(messageLength));
          }
        }
      }
      catch (Exception fault) {
        _logger?.LogDebug("RunSendLoopAsync quit: {}", fault);
        _sendQueue.Writer.TryComplete(fault);
        await bytes.CompleteAsync(fault).ConfigureAwait(false);
        while (!queue.Completion.IsCompleted) {
          var item = await queue.ReadAsync().ConfigureAwait(false);
          item.Output.SetException(fault);
        }
      }
    }

    private async Task SendExclusivelyAsync(Deferred<object?> item, int messageLength, ReadResult readResult) {
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

        var segment = GetSegment(memory);
        var cut = new ArraySegment<byte>(segment.Array, segment.Offset, count);
        _logger?.LogDebug("SendExclusivelyAsync advanceTo: {}, bytesRemaining: {}, data: {}", messageLength, bytesRemaining, BitConverter.ToString(cut.Array, cut.Offset, cut.Count));
        await _socket.SendAsync(cut, WebSocketMessageType.Binary, isEnd, cancellation).ConfigureAwait(false);

        if (isEnd) {
          break;
        }
      }
      _logger?.LogDebug("SendExclusivelyAsync SetResult null");
      output.SetResult(null);
    }

    private async Task<int> ReadLengthAsync(PipeReader reader, CancellationToken cancellation) {
      var result = await reader!.ReadAsync(cancellation).ConfigureAwait(false);
      try {
        _logger?.LogDebug("ReadLengthAsync buffer.Length: {}, isCancelled: {}, isCompleted: {}",
          result.Buffer.Length, result.IsCanceled, result.IsCompleted);
        if (result.Buffer.IsEmpty) {
          return 0;
        }

        var header = result.Buffer.Slice(0, 4);
        header.CopyTo(_lengthBuffer);
        int length = BitConverter.ToInt32(_lengthBuffer, 0);
        _logger?.LogDebug("ReadLengthAsync header.Length: {}, 0x{a0:x2}, 0x{a1:x2}, 0x{a2:x2}, 0x{a3:x2}",
          header.Length, _lengthBuffer[0], _lengthBuffer[1], _lengthBuffer[2], _lengthBuffer[3]);
        _logger?.LogDebug("ReadLengthAsync read messageLength: {}, 0x{length:x}", length, length);
        return length;
      }
      finally {
        var consumed = result.Buffer.GetPosition(Math.Min(result.Buffer.Length, 4));
        reader.AdvanceTo(consumed);
      }
    }

    public static PipeReader UsePipeReader(WebSocket webSocket, int sizeHint = 0, PipeOptions? pipeOptions = null, ILogger? logger = null, CancellationToken cancellation = default) {
      var pipe = new Pipe(pipeOptions ?? PipeOptions.Default);
      _ = Task.Run(async delegate {
        try {
          var prefixer = new PrefixingBufferWriter<byte>(pipe.Writer, sizeof(int));

          while (true) {
            cancellation.ThrowIfCancellationRequested();
            var memory = prefixer.GetMemory(sizeHint);
            using var handle = memory.Pin();
            var segment = GetSegment(memory);
            var readResult = await webSocket.ReceiveAsync(segment, cancellation).ConfigureAwait(false);
            logger?.LogDebug("UsePipeReader received: {}", BitConverter.ToString(segment.Array, segment.Offset, readResult.Count));

            if (webSocket.State == WebSocketState.CloseReceived && readResult.MessageType == WebSocketMessageType.Close) {
              // Tell the PipeReader that there's no more data coming
              await pipe.Writer.CompleteAsync().ConfigureAwait(false);
              break;
            }

            logger?.LogDebug("Advance WebsocketRead {}", sizeof(int) + readResult.Count);
            prefixer.Advance(readResult.Count);
            if (readResult.EndOfMessage) {
              BitConverter.GetBytes((int)prefixer.Length).CopyTo(prefixer.Prefix.Span);
              logger?.LogDebug("UsePipeReader flush messageLength: {}", prefixer.Length);
              prefixer.Commit();
              var result = await pipe.Writer.FlushAsync(cancellation).ConfigureAwait(false);
              if (result.IsCompleted) {
                break;
              }
            }
          }
        }
        catch (Exception exception) {
          // Propagate the exception to the reader.
          await pipe.Writer.CompleteAsync(exception).ConfigureAwait(false);
          logger?.LogDebug("UsePipeReader: {}", exception);
        }
      }, cancellation);

      return pipe.Reader;
    }

    private static ArraySegment<byte> GetSegment(ReadOnlyMemory<byte> memory) {
      if (MemoryMarshal.TryGetArray(memory, out var segment)) {
        return segment;
      }
      else {
        throw new Exception("Failed to get buffer");
      }
    }
  }
}
