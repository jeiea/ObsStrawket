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
    [Fact(Timeout = 3 * 60 * 1000)]
    public async Task TestAsync() {
      await StressTestHelper.RunParameterizedAsync(200, 4).ConfigureAwait(false);
    }
  }

  public class ClientStressTest2 {
    [Fact(Timeout = 3 * 60 * 1000)]
    public async Task TestAsync() {
      await StressTestHelper.RunParameterizedAsync(80, 4).ConfigureAwait(false);
    }
  }

  public class ClientStressTest3 {
    [Fact(Timeout = 3 * 60 * 1000)]
    public async Task TestAsync() {
      await StressTestHelper.RunParameterizedAsync(5, 5).ConfigureAwait(false);
    }
  }

  internal static class StressTestHelper {
    public static async Task RunParameterizedAsync(int delay, int division) {
      var logger = new MemoryLogger();
      var client = ClientFlow.GetDebugClient();
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
      foreach (
        var (connectDelayMs, requestDelayMs, closeDelayMs, nextDelayMs)
        in GeneratePermutation(delay, division)
      ) {
        //Debug.WriteLine($"{connectDelayMs} {requestDelayMs} {closeDelayMs} {nextDelayMs}");
        tasks.Add(Task.Run(async () => {
          try {
            await Task.Delay(connectDelayMs).ConfigureAwait(false);
            await client.ConnectAsync(server.Uri, MockServer.Password).ConfigureAwait(false);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        tasks.Add(Task.Run(async () => {
          try {
            await Task.Delay(requestDelayMs).ConfigureAwait(false);
            var version = await client.GetVersionAsync().ConfigureAwait(false);
            Assert.Contains("bmp", version.SupportedImageFormats);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        tasks.Add(Task.Run(async () => {
          try {
            await Task.Delay(closeDelayMs).ConfigureAwait(false);
            await client.CloseAsync().ConfigureAwait(false);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        await Task.Delay(nextDelayMs).ConfigureAwait(false);
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

    private static IEnumerable<(int, int, int, int)> GeneratePermutation(int max, int division) {
      int step = Math.Max(1, max / division);
      for (int i1 = 0; i1 < max; i1 += step) {
        for (int i2 = 0; i2 < max; i2 += step) {
          for (int i3 = 0; i3 < max; i3 += step) {
            for (int i4 = 0; i4 < max; i4 += step) {
              yield return (i1, i2, i3, i4);
            }
          }
        }
      }
    }
  }
}
