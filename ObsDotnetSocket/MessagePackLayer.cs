namespace ObsDotnetSocket {
  using MessagePack;
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
    private readonly MessagePackSerializerOptions _serialOptions = new(OpCodeMessageResolver.Instance);
    private PipeReader? _reader;
    private readonly Pipe _writer = new();
    private readonly Channel<Deferred<object?>> _sendQueue =
      Channel.CreateBounded<Deferred<object?>>(1);
    private readonly CancellationTokenSource _cancellation = new();
    private readonly byte[] _lengthBuffer = new byte[4];

    public Task SendTask { get; private set; } = Task.CompletedTask;

    public MessagePackLayer(WebSocket socket) {
      _socket = socket;
    }

    public void RunLoop() {
      if (_reader != null) {
        return;
      }
      _reader = WebSocketPipeUtil.UsePipeReader(_socket, cancellation: _cancellation.Token);
      SendTask = RunSendLoopAsync();
    }

    public async Task<IOpCodeMessage?> ReceiveAsync(CancellationToken cancellation = default) {
      cancellation.ThrowIfCancellationRequested();

      int length = await ReadLengthAsync(_reader!, cancellation).ConfigureAwait(false);
      var result = await _reader!.ReadAtLeastAsync(length, cancellation).ConfigureAwait(false);
      try {
        if (result.Buffer.IsEmpty) {
          return null;
        }
        cancellation.ThrowIfCancellationRequested();
        return MessagePackSerializer.Deserialize<IOpCodeMessage>(result.Buffer, _serialOptions, cancellation);
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
      prefix.CopyTo(prefixer.Prefix.Slice(0, sizeof(int)).Span);

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
        var item = await _sendQueue.Reader.ReadAsync(_cancellation.Token).ConfigureAwait(false);
        try {
          await SendExclusively(item).ConfigureAwait(false);
        }
        catch (Exception ex) {
          item.Output.SetException(ex);
        }
      }
    }

    private async Task SendExclusively(Deferred<object?> item) {
      var (output, cancellation) = item;
      using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation, _cancellation.Token);
      var linked = linkedSource.Token;

      int length = await ReadLengthAsync(_writer.Reader, linked).ConfigureAwait(false);
      var readResult = await _writer.Reader.ReadAtLeastAsync(length, linked).ConfigureAwait(false);
      try {
        if (linked.IsCancellationRequested) {
          output.SetException(new OperationCanceledException());
          return;
        }
        var buffer = readResult.Buffer;

        long i = 0;
        foreach (var memory in buffer) {
          i += memory.Length;
          bool isEnd = i == buffer.Length;
          await _socket.SendAsync(GetSegment(memory), WebSocketMessageType.Binary, isEnd, linked).ConfigureAwait(false);
        }
        output.SetResult(null);
      }
      finally {
        _writer.Reader.AdvanceTo(readResult.Buffer.End);
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
