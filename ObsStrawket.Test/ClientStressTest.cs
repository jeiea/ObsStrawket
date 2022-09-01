using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class ClientStressTest {
    [Fact]
    public async Task TestAsync() {
      var cancellation = new CancellationTokenSource();
      var client = ClientFlow.GetDebugClient();
      using var server = new MockServer().Run(cancellation.Token);
      int openCloseDifference = 0;

      client.Closed += (o) => {
        Interlocked.Decrement(ref openCloseDifference);
        _ = Task.Run(() => Debug.WriteLine($"Disconnect: {openCloseDifference}"));
        if (Math.Abs(openCloseDifference) > 1) {
          cancellation.Cancel();
        }
      };

      async Task ConnectAsync() {
        await client.ConnectAsync(server.Uri, MockServer.Password, cancellation: cancellation.Token).ConfigureAwait(false);
        Interlocked.Increment(ref openCloseDifference);
        _ = Task.Run(() => Debug.WriteLine($"Connect: {openCloseDifference}"));
        if (Math.Abs(openCloseDifference) > 1) {
          cancellation.Cancel();
        }
      }

      var tasks = new List<Task>();
      var abort = Task.CompletedTask;
      for (int i = 0; i < 50; i++) {
        await abort.ConfigureAwait(false);

        tasks.Add(Task.Run(async () => {
          try {
            await ConnectAsync().ConfigureAwait(false);
            var version = await client.GetVersionAsync(cancellation.Token).ConfigureAwait(false);
            Assert.Contains("bmp", version.SupportedImageFormats);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        tasks.Add(Task.Run(async () => {
          try {
            await client.CloseAsync();
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        await Task.Delay(i * 4, cancellation.Token).ConfigureAwait(false);
      }
      await Task.WhenAll(tasks).ConfigureAwait(false);

      Assert.InRange(openCloseDifference, 0, 1);
    }
  }
}
