using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StopStreamTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StopStreamFlow());
    }
  }

  internal class StopStreamFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.StopStreamAsync().ConfigureAwait(false);
      var stopping = await ClientFlow.WaitEventAsync<StreamStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopping, stopping.OutputState);
      var stopped = await ClientFlow.WaitEventAsync<StreamStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopped, stopped.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "StopStream"
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
    "requestType": "StopStream"
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
    "eventType": "StreamStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "outputActive": true,
      "outputState": "OBS_WEBSOCKET_OUTPUT_STOPPED"
    },
    "eventIntent": 64,
    "eventType": "StreamStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
