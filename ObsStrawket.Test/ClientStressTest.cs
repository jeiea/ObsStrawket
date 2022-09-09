using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

      client.Disconnected += (o) => {
        Interlocked.Decrement(ref openCloseDifference);
        if (Math.Abs(openCloseDifference) > 1) {
          cancellation.Cancel();
        }
      };

      client.Connected += (uri) => {
        Interlocked.Increment(ref openCloseDifference);
        if (Math.Abs(openCloseDifference) > 1) {
          cancellation.Cancel();
        }
      };

      var tasks = new List<Task>();
      var abort = Task.CompletedTask;
      for (int i = 0; i < 50; i++) {
        await abort.ConfigureAwait(false);

        tasks.Add(Task.Run(async () => {
          try {
            await client.ConnectAsync(server.Uri, MockServer.Password, cancellation: cancellation.Token).ConfigureAwait(false);
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
      await TestUtil.WhenAnyThrowsAsync(tasks).ConfigureAwait(false);

      Assert.InRange(openCloseDifference, 0, 1);
    }
  }
}
