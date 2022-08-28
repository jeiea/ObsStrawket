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

    private readonly WebSocket _socket;
    private readonly ILogger? _logger;
    private readonly MessagePackSerializerOptions _serialOptions = new(OpCodeMessageResolver.Instance);
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
      _reader = WebSocketPipeUtil.UsePipeReader(_socket, logger: _logger, cancellation: _cancellation.Token);
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
      {
        using var _ = prefixer.Prefix.Pin();
        prefixer.Prefix.Span[0] = prefix[0];
        prefixer.Prefix.Span[1] = prefix[1];
        prefixer.Prefix.Span[2] = prefix[2];
        prefixer.Prefix.Span[3] = prefix[3];
        //prefix.CopyTo(prefixer.Prefix.Slice(0, sizeof(int)).Span);
      }

      if (_cancellation.IsCancellationRequested) {
        return;
      }

      prefixer.Commit();
      var output = new TaskCompletionSource<object?>();
      await _sendQueue.Writer.WriteAsync(new(output, linked), linked).ConfigureAwait(false);
      await _writer.Writer.FlushAsync(linked).ConfigureAwait(false);

      await output.Task.ConfigureAwait(false);
    }

    public void Dispose() {
      _cancellation.Cancel();
    }

    private async Task RunSendLoopAsync() {
      while (!(_cancellation.IsCancellationRequested || _sendQueue.Reader.Completion.IsCompleted)) {
        var item = await _sendQueue.Reader.ReadAsync(_cancellation.Token);
        try {
          await SendExclusivelyAsync(item);
        }
        catch (Exception ex) {
          item.Output.SetException(ex);
        }
      }
    }

    private async Task SendExclusivelyAsync(Deferred<object?> item) {
      var (output, cancellation) = item;
      int messageLength = await ReadLengthAsync(_writer.Reader, cancellation);
      var readResult = await _writer.Reader.ReadAtLeastAsync(messageLength, cancellation);
      try {
        if (cancellation.IsCancellationRequested) {
          output.SetException(new OperationCanceledException());
          return;
        }
        var buffer = readResult.Buffer;

        long bytesRemaining = messageLength;
        foreach (var memory in buffer) {
          if (memory.Length == 0) {
            _logger?.LogInformation("memory.Length == 0, skip.");
            continue;
          }

          using var _ = memory.Pin();
          int count = Math.Min(memory.Length, (int)bytesRemaining);
          bytesRemaining -= count;
          bool isEnd = bytesRemaining <= 0;

          var segment = GetSegment(memory);
          var cut = new ArraySegment<byte>(segment.Array, segment.Offset, count);
          _logger?.LogInformation("Offset: {}, Count: {}, buffer.Length: {}, messageLength: {}, bytesRemaining: {}, isEnd: {}, Data: {}",
            cut.Offset, cut.Count, buffer.Length, messageLength, bytesRemaining, isEnd, BitConverter.ToString(cut.Array, cut.Offset, cut.Count));
          await _socket.SendAsync(cut, WebSocketMessageType.Binary, isEnd, cancellation);

          if (isEnd) {
            break;
          }
        }
        output.SetResult(null);
      }
      finally {
        _logger?.LogInformation("AdvanceTo: {}", messageLength);
        _writer.Reader.AdvanceTo(readResult.Buffer.GetPosition(messageLength));
      }
    }

    private async Task<int> ReadLengthAsync(PipeReader reader, CancellationToken cancellation) {
      var result = await reader!.ReadAsync(cancellation).ConfigureAwait(false);
      try {
        if (result.Buffer.IsEmpty) {
          return 0;
        }

        var header = result.Buffer.Slice(0, 4);
        header.CopyTo(_lengthBuffer);
        return BitConverter.ToInt32(_lengthBuffer, 0);
      }
      finally {
        var consumed = result.Buffer.GetPosition(Math.Min(result.Buffer.Length, 4));
        reader.AdvanceTo(consumed);
      }
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
