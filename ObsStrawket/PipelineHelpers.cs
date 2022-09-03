using Microsoft.Extensions.Logging;
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
    public static async Task<int> ReadLengthAsync(PipeReader reader, ILogger? logger = null, CancellationToken cancellation = default) {
      using var _1 = logger?.BeginScope(nameof(ReadLengthAsync));
      var result = await reader!.ReadAtLeastAsync(sizeof(int), cancellation).ConfigureAwait(false);
      var buffer = result.Buffer;
      try {
        if (buffer.Length < sizeof(int)) {
          logger?.LogDebug("result.IsCancelled: {}, IsCompleted: {}", result.IsCanceled, result.IsCompleted);
          return -1;
        }

        int length = ReadInt(buffer);
        logger?.LogTrace("buffer.Length: {}, messageLength: {}(0x{length:x})", buffer.Length, length, length);
        return length;
      }
      finally {
        var consumed = buffer.GetPosition(Math.Min(buffer.Length, 4));
        reader.AdvanceTo(consumed);
      }
    }

    public static ArraySegment<byte> GetSegment(ReadOnlyMemory<byte> memory) {
      if (MemoryMarshal.TryGetArray(memory, out var segment)) {
        return segment;
      }
      else {
        throw new Exception("Failed to get buffer");
      }
    }
    private static int ReadInt(ReadOnlySequence<byte> bytes) {
      Span<byte> header = stackalloc byte[sizeof(int)];
      bytes.Slice(0, sizeof(int)).CopyTo(header);
      return header[0] | header[1] << 8 | header[2] << 16 | header[3] << 24;
    }
  }
}
