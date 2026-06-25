using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StartRecordTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartRecordFlow());
    }
  }

  internal class StartRecordFlow : ITestFlow {
    public static string EscapedFileName => MockServer.EscapedFilePath;

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.StartRecordAsync().ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<RecordStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Starting, changed.OutputState);
      changed = await ClientFlow.WaitEventAsync<RecordStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Started, changed.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "StartRecord"
  },
  "op": 6
}
""").ConfigureAwait(false);

      await session.SendAsync($$"""
{
  "op": 7,
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "StartRecord"
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "outputActive": false,
      "outputPath": null,
      "outputState": "OBS_WEBSOCKET_OUTPUT_STARTING"
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
      "outputActive": true,
      "outputPath": "{{EscapedFileName}}",
      "outputState": "OBS_WEBSOCKET_OUTPUT_STARTED"
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
