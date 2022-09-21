using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ObsStrawket {
  record class Deferred<I, O>(I Input, TaskCompletionSource<O> Output, CancellationToken Cancellation = default) {
    public Deferred(I Input, CancellationToken Cancellation = default) : this(Input, new(), Cancellation) { }
  }
  record class Deferred<O>(TaskCompletionSource<O> Output, CancellationToken Cancellation = default) : Deferred<object?, O>(null, Output, Cancellation) {
    public Deferred(CancellationToken Cancellation = default) : this(new(), Cancellation) { }
  }

  internal static class PipelineHelpers {
    public static async Task<ReadOnlySequence<byte>> RealAllAsync(Pipe pipe, CancellationToken token) {
      int length = (int)pipe.Writer.UnflushedBytes;

      await pipe.Writer.CompleteAsync().ConfigureAwait(false);
      var result = await pipe.Reader.ReadAtLeastAsync(length, token).ConfigureAwait(false);
      var buffer = result.Buffer.Slice(0, length);
      return buffer;
    }

    public static ArraySegment<byte> GetSegment(ReadOnlyMemory<byte> memory) {
      if (MemoryMarshal.TryGetArray(memory, out var segment)) {
        return segment;
      }
      else {
        throw new Exception("Failed to get buffer");
      }
    }
  }
}
