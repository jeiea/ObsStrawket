using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StartOutputTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartOutputFlow());
    }
  }

  /// <summary>
  /// Precondition: ToggleReplayBufferFlow
  /// </summary>
  internal class StartOutputFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      _ = await client.StartOutputAsync(outputName: GetOutputListFlow.OutputName).ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<ReplayBufferStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Started, changed.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "outputName": "Replay Buffer"
    },
    "requestId": "{guid}",
    "requestType": "StartOutput"
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
    "requestType": "StartOutput"
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
    "eventType": "ReplayBufferStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
