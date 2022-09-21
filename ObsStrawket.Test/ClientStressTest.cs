using Microsoft.Extensions.Logging;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class ClientStressTest {
    [Fact]
    public async Task TestAsync() {
      var logger = new MemoryLogger();
      var client = ClientFlow.GetDebugClient(logger: null);
      using var server = new MockServer().Run(default);
      int openCloseDifference = 0;
      var failures = Channel.CreateUnbounded<string>();

      client.Connected += (uri) => {
        Interlocked.Increment(ref openCloseDifference);
        int difference = openCloseDifference;
        logger.LogDebug("Connected: {}", difference);
        if (Math.Abs(difference) > 1) {
          _ = failures.Writer.WriteAsync($"open close difference {difference}");
        }
      };
      client.Disconnected += (o) => {
        Interlocked.Decrement(ref openCloseDifference);
        int difference = openCloseDifference;
        logger.LogDebug("Disconnected: {}", difference);
        if (Math.Abs(difference) > 1) {
          _ = failures.Writer.WriteAsync($"open close difference {difference}");
        }
      };

      var monitor = Task.Run(async () => {
        while (await failures.Reader.WaitToReadAsync().ConfigureAwait(false)) {
          string failure = await failures.Reader.ReadAsync().ConfigureAwait(false);
          throw new Exception(failure);
        }
      });

      var tasks = new List<Task>();
      int step = 5;
      int delayMilliseconds = 150;
      int round = (int)Math.Pow(step, 3);
      for (int i = 0; i < round; i++) {
        int t = i;
        tasks.Add(Task.Run(async () => {
          try {
            await Task.Delay((int)((double)t / round * delayMilliseconds));
            await client.ConnectAsync(server.Uri, MockServer.Password).ConfigureAwait(false);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        tasks.Add(Task.Run(async () => {
          try {
            await Task.Delay((int)((double)t / step % step / step * delayMilliseconds));
            var version = await client.GetVersionAsync().ConfigureAwait(false);
            Assert.Contains("bmp", version.SupportedImageFormats);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        tasks.Add(Task.Run(async () => {
          try {
            await Task.Delay((int)((double)t % step / step * delayMilliseconds));
            await client.CloseAsync();
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        await Task.Delay(delayMilliseconds).ConfigureAwait(false);
      }
      await TestUtil.WhenAnyThrowsAsync(tasks).ConfigureAwait(false);

      try {
        failures.Writer.TryComplete();
        await monitor.ConfigureAwait(false);
      }
      catch (Exception ex) {
        logger.LogError("OpenClose mismatch: {}", ex);
        string error = logger.GetAll();
        throw new Exception(error);
      }

      Assert.InRange(openCloseDifference, 0, 1);
    }
  }
}
