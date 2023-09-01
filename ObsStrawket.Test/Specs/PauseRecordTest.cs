using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class PauseRecordTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new PauseRecordFlow()).ConfigureAwait(false);
    }
  }

  class PauseRecordFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      await client.PauseRecordAsync().ConfigureAwait(false);
      var ev = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Paused, (ev as RecordStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "PauseRecord"
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
    "requestType": "PauseRecord"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "outputActive": false,
      "outputPath": null,
      "outputState": "OBS_WEBSOCKET_OUTPUT_PAUSED"
    },
    "eventIntent": 64,
    "eventType": "RecordStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
