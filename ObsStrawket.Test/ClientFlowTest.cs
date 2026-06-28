using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class ClientFlowTest {
    [Fact]
    public async Task WaitEventsAsyncReportsUnmatchedMatcherIndexesAsync() {
      using var server = new MockServer().Run(
        async (context, cancellation) => {
          var (webSocketContext, session) =
            await MockServer.HandshakeAsync(context, cancellation).ConfigureAwait(false);
          await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "sample": 3
    },
    "eventIntent": 1,
    "eventType": "CustomEvent"
  },
  "op": 5
}
""").ConfigureAwait(false);
          await Task.Delay(TimeSpan.FromSeconds(1), cancellation).ConfigureAwait(false);
          await MockServer.CloseQuietlyAsync(webSocketContext.WebSocket, cancellation).ConfigureAwait(false);
        },
        TestContext.Current.CancellationToken
      );
      var client = ClientFlow.GetDebugClient();
      bool connected = await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken
      );
      Assert.True(connected);

      try {
        var exception = await Assert.ThrowsAsync<TimeoutException>(
          () => ClientFlow.WaitEventsAsync(
            client,
            TimeSpan.FromMilliseconds(500),
            TestContext.Current.CancellationToken,
            e => e is CustomEvent,
            e => e is InputCreated,
            e => e is SceneItemCreated
          )
        );

        Assert.Contains("Unmatched matcher indexes: 1, 2.", exception.Message);
      }
      finally {
        await client.CloseAsync();
      }
    }
  }
}
