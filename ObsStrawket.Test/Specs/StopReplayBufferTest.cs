using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StopReplayBufferTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StopReplayBufferFlow()).ConfigureAwait(false);
    }
  }

  class StopReplayBufferFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.StopReplayBufferAsync().ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopping, (changed as ReplayBufferStateChanged)!.OutputState);
      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopped, (changed as ReplayBufferStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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
