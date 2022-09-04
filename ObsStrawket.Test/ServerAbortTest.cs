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
  public class ServerAbortTest {
    [Fact]
    public async Task TestAsync() {
      var cancellation = new CancellationTokenSource();
      var abort = Task.CompletedTask;
      var client = ClientFlow.GetDebugClient();
      client.Disconnected += (o) => {
        Debug.WriteLine(o);
      };

      for (int i = 0; i < 50; i++) {
        cancellation = new CancellationTokenSource();
        await abort.ConfigureAwait(false);
        Debug.WriteLine($"Session {i} start");

        using var irritatedServer = new MockServer().Run(cancellation.Token);
        int milliseconds = i * 4;
        abort = Task.Run(async () => {
          await Task.Delay(milliseconds);
          irritatedServer.Abort();
        });

        try {
          await new ClientFlow().RunClientAsync(irritatedServer.Uri, client, cancellation: cancellation.Token);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex);
        }
        cancellation.Cancel();
      }

      cancellation = new CancellationTokenSource();
      await abort.ConfigureAwait(false);
      using var normalServer = new MockServer().Run(cancellation.Token);
      await new ClientFlow().RunClientAsync(normalServer.Uri, client, cancellation: cancellation.Token);
    }
  }
}
