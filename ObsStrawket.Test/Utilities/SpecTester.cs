using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ObsStrawket.Test.Utilities {
  interface ITestFlow {
    Task RequestAsync(ObsClientSocket client);
    Task RespondAsync(MockServerSession session);
  }

  class SpecTester {
    public static async Task TestAsync(ITestFlow flow) {
      var taskSource = new TaskCompletionSource();
      using var server = new MockServer().Run(default, async (context, cancellation) => {
        try {
          var (webSocketContext, session) = await MockServer.HandshakeAsync(context, cancellation).ConfigureAwait(false);
          await flow.RespondAsync(session).ConfigureAwait(false);
          await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, default).ConfigureAwait(false);
          taskSource.SetResult();
        }
        catch (Exception ex) {
          taskSource.SetException(ex);
        }
      });

      async Task RunClientAsync() {
        var client = ClientFlow.GetDebugClient(useChannel: true);
        await client.ConnectAsync(server!.Uri, MockServer.Password).ConfigureAwait(false);
        await flow.RequestAsync(client).ConfigureAwait(false);
        await client.CloseAsync().ConfigureAwait(false);
      }

      await Task.WhenAll(taskSource.Task, RunClientAsync()).ConfigureAwait(false);
    }
  }
}
