using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StartVirtualCamTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartVirtualCamFlow());
    }
  }

  internal class StartVirtualCamFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.StartVirtualCamAsync().ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<VirtualcamStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Started, changed.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "StartVirtualCam"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "StartVirtualCam"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "outputActive": true,
      "outputState": "OBS_WEBSOCKET_OUTPUT_STARTED"
    },
    "eventIntent": 64,
    "eventType": "VirtualcamStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
