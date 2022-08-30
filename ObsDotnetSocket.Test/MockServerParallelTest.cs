using ObsDotnetSocket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace ObsDotnetSocket.Test {
  public class MockServerParallelTest {
    [Fact]
    public async Task TestParallelAsync() {
      var cancellation = new CancellationTokenSource();
      using var server = new MockServer().Run(cancellation.Token, ServeEchoAsync);
      var tasks = new List<Task>();

      try {
        var client = new ObsClientSocket();
        await client.ConnectAsync(MockServer.DefaultUri, MockServer.Password, cancellation: cancellation.Token).ConfigureAwait(false);

        for (int i = 0; i < 30; i++) {
          tasks.Add(Task.Run(() => (i % 2) switch {
            0 => client.GetStudioModeEnabledAsync(cancellation.Token),
            _ => client.GetVersionAsync(cancellation.Token),
          }));
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);
        Assert.All(tasks, t => Assert.True(t.IsCompletedSuccessfully));
      }
      catch (Exception ex) {
        Debug.WriteLine(ex);
        throw;
      }
      finally {
        cancellation.Cancel();
      }
    }

    private async Task ServeEchoAsync(HttpListenerContext context, CancellationToken token) {
      try {
        var (_, session) = await MockServer.HandshakeAsync(context, token).ConfigureAwait(false);
        using var _1 = session;

        await Task.Delay(500, token).ConfigureAwait(false);

        var channel = Channel.CreateUnbounded<string>();
        var receiveTask = Task.Run(async () => {
          for (int i = 0; i < 30; i++) {
            string json = await session.ReceiveAsync().ConfigureAwait(false);
            await channel.Writer.WriteAsync(json, token).ConfigureAwait(false);
          }
          channel.Writer.Complete();
        }, token);
        var sendTask = Task.Run(async () => {
          await foreach (string json in channel.Reader.ReadAllAsync(token)) {
            string guid = Regex.Match(json, @"[0-9a-f]{8}-[0-9a-f]{4}[^""]*").Value;
            if (json.Contains(@"""requestType"":""GetVersion"",")) {
              await session.SendGetVersionResponseAsync(guid).ConfigureAwait(false);
            }
            else if (json.Contains(@"""requestType"":""GetStudioModeEnabled"",")) {
              await session.SendGetStudioModeEnabledResponseAsync(guid).ConfigureAwait(false);
            }
            else {
              throw new Exception("Unexpected request");
            }
          }
        }, token);
        await Task.WhenAll(receiveTask, sendTask).ConfigureAwait(false);

        Debug.WriteLine("Served all messages");
        await session.ReceiveAsync().ConfigureAwait(false);
      }
      catch (Exception ex) {
        Debug.WriteLine(ex);
        throw;
      }
    }
  }
}
