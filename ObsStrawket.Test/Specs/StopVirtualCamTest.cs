using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StopVirtualCamTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StopVirtualCamFlow()).ConfigureAwait(false);
    }
  }

  class StopVirtualCamFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.StopVirtualCamAsync().ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopped, (changed as VirtualcamStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "StopVirtualCam"
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
    "requestType": "StopVirtualCam"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "outputActive": false,
      "outputState": "OBS_WEBSOCKET_OUTPUT_STOPPED"
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
