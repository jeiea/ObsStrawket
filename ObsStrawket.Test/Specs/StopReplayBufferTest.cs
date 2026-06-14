using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StopReplayBufferTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StopReplayBufferFlow());
    }
  }

  internal class StopReplayBufferFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.StopReplayBufferAsync().ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<ReplayBufferStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopping, changed.OutputState);
      changed = await ClientFlow.WaitEventAsync<ReplayBufferStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopped, changed.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "StopReplayBuffer"
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
    "requestType": "StopReplayBuffer"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "outputActive": false,
      "outputState": "OBS_WEBSOCKET_OUTPUT_STOPPING"
    },
    "eventIntent": 64,
    "eventType": "ReplayBufferStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "outputActive": false,
      "outputState": "OBS_WEBSOCKET_OUTPUT_STOPPED"
    },
    "eventIntent": 64,
    "eventType": "ReplayBufferStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
