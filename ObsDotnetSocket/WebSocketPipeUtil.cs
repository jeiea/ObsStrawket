namespace ObsDotnetSocket {
  using Microsoft;
  using Nerdbank.Streams;
  using System;
  using System.IO.Pipelines;
  using System.Net.WebSockets;
  using System.Runtime.CompilerServices;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Threading.Tasks;

  static class WebSocketPipeUtil {

    public static PipeReader UsePipeReader(WebSocket webSocket, int sizeHint = 1400, PipeOptions? pipeOptions = null, CancellationToken cancellation = default) {
      Requires.NotNull(webSocket, nameof(webSocket));

      var pipe = new Pipe(pipeOptions ?? PipeOptions.Default);
      _ = Task.Run(async delegate {
        try {
          while (true) {
            cancellation.ThrowIfCancellationRequested();
            var memory = pipe.Writer.GetMemory(sizeHint);
            using var handle = memory.Pin();
            var segment = GetSegment(memory.Slice(4));
            var shifted = new ArraySegment<byte>(segment.Array, segment.Offset, segment.Count);
            var readResult = await webSocket.ReceiveAsync(shifted, cancellation).ConfigureAwait(false);

            if (readResult.Count == 0) {
              // Tell the PipeReader that there's no more data coming
              await pipe.Writer.CompleteAsync().ConfigureAwait(false);
              break;
            }

            byte[] encodedLength = BitConverter.GetBytes(readResult.Count);
            encodedLength.CopyTo(memory.Span.Slice(0, 4));

            pipe.Writer.Advance(sizeof(int) + readResult.Count);
            if (readResult.EndOfMessage) {
              var result = await pipe.Writer.FlushAsync(cancellation).ConfigureAwait(false);
              if (result.IsCompleted) {
                break;
              }
            }
          }
        }
        catch (Exception ex) {
          // Propagate the exception to the reader.
          await pipe.Writer.CompleteAsync(ex).ConfigureAwait(false);
          System.Diagnostics.Debug.WriteLine($"UsePipeReader: {ex}");
        }
      }, cancellation);

      return pipe.Reader;
    }

    public static PipeWriter UsePipeWriter(WebSocket webSocket, PipeOptions? pipeOptions = null, CancellationToken cancellation = default) {
      Requires.NotNull(webSocket, nameof(webSocket));

      var pipe = new Pipe(pipeOptions ?? PipeOptions.Default);

      _ = Task.Run(async delegate {
        try {
          while (true) {
            cancellation.ThrowIfCancellationRequested();
            var readResult = await pipe.Reader.ReadAsync(cancellation).ConfigureAwait(false);
            foreach (var segment in readResult.Buffer) {
              cancellation.ThrowIfCancellationRequested();
              await webSocket.SendAsync(GetSegment(segment), WebSocketMessageType.Binary, endOfMessage: true, cancellation).ConfigureAwait(false);
            }

            pipe.Reader.AdvanceTo(readResult.Buffer.End);
            if (readResult.IsCompleted) {
              break;
            }
          }

          await pipe.Reader.CompleteAsync().ConfigureAwait(false);
        }
        catch (Exception ex) {
          // Propagate the exception to the writer.
          await pipe.Reader.CompleteAsync(ex).ConfigureAwait(false);
        }
      }, cancellation);

      return pipe.Writer;
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
