using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StartReplayBufferTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartReplayBufferFlow());
    }
  }

  /// <summary>
  /// Precondition: SetProfileParameterFlow, SetCurrentProfileFlow
  /// </summary>
  internal class StartReplayBufferFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.StartReplayBufferAsync().ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<ReplayBufferStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Starting, changed.OutputState);
      changed = await ClientFlow.WaitEventAsync<ReplayBufferStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Started, changed.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "StartReplayBuffer"
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
    "requestType": "StartReplayBuffer"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "outputActive": false,
      "outputState": "OBS_WEBSOCKET_OUTPUT_STARTING"
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
      "outputActive": true,
      "outputState": "OBS_WEBSOCKET_OUTPUT_STARTED"
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
