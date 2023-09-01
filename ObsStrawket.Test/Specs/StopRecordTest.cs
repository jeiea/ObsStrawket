using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StopRecordTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StopRecordFlow()).ConfigureAwait(false);
    }
  }

  class StopRecordFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.StopRecordAsync().ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopping, (changed as RecordStateChanged)!.OutputState);
      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopped, (changed as RecordStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "op": 6,
  "d": {
    "requestId": "{guid}",
    "requestType": "StopRecord"
  }
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
    "requestType": "StopRecord",
    "responseData": {
      "outputPath": "{{StartRecordFlow.EscapedFileName}}"
    }
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
      "outputState": "OBS_WEBSOCKET_OUTPUT_STOPPING"
    },
    "eventIntent": 64,
    "eventType": "RecordStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "eventData": {
      "outputActive": false,
      "outputPath": "{{StartRecordFlow.EscapedFileName}}",
      "outputState": "OBS_WEBSOCKET_OUTPUT_STOPPED"
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
