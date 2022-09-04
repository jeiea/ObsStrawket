using ObsStrawket.Test.Utilities;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Utilities {
  interface ITestFlow {
    Task RequestAsync(ObsClientSocket client);
    Task RespondAsync(MockServerSession session);
  }

  class SpecTester {
    public static async Task TestAsync(ITestFlow flow) {
      using var server = new MockServer().Run(default, async (context, cancellation) => {
        var (webSocketContext, session) = await MockServer.HandshakeAsync(context, cancellation).ConfigureAwait(false);
        using var _1 = session;

        await flow.RespondAsync(session).ConfigureAwait(false);
      });

      var client = ClientFlow.GetDebugClient();
      await client.ConnectAsync(server.Uri, MockServer.Password).ConfigureAwait(false);

      await flow.RequestAsync(client).ConfigureAwait(false);
    }
  }
}
