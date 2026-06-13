using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {

  public class BroadcastCustomEventTest {

    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new BroadcastCustomEventFlow());
    }
  }

  internal class BroadcastCustomEventFlow : ITestFlow {

    public async Task RequestAsync(ObsClientSocket client) {
      await client.BroadcastCustomEventAsync(
        eventData: new Dictionary<string, JsonElement?> { { "sample", 3.ToJsonElement() } }
      ).ConfigureAwait(false);

      var ev = await ClientFlow.WaitEventAsync<CustomEvent>(client).ConfigureAwait(false);
      Assert.IsType<CustomEvent>(ev);
      Assert.Equal(3, ev.EventData["sample"]?.GetInt32());
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "eventData": {
        "sample": 3
      }
    },
    "requestId": "{guid}",
    "requestType": "BroadcastCustomEvent"
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
    "requestType": "BroadcastCustomEvent"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sample": 3
    },
    "eventIntent": 1,
    "eventType": "CustomEvent"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
