using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class ToggleRecordPauseTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new ToggleRecordPauseFlow()).ConfigureAwait(false);
    }
  }

  class ToggleRecordPauseFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      var response = await client.ToggleRecordPauseAsync().ConfigureAwait(false);
      Assert.True(response.OutputPaused);
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Paused, (changed as RecordStateChanged)!.OutputState);

      await Task.Delay(100).ConfigureAwait(false);
      response = await client.ToggleRecordPauseAsync().ConfigureAwait(false);
      Assert.False(response.OutputPaused);
      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Resumed, (changed as RecordStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "ToggleRecordPause"
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
    "requestType": "ToggleRecordPause",
    "responseData": {
      "outputPaused": true
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
      "outputState": "OBS_WEBSOCKET_OUTPUT_PAUSED"
    },
    "eventIntent": 64,
    "eventType": "RecordStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "ToggleRecordPause"
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
    "requestType": "ToggleRecordPause",
    "responseData": {
      "outputPaused": false
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "outputActive": true,
      "outputPath": null,
      "outputState": "OBS_WEBSOCKET_OUTPUT_RESUMED"
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
