using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentSceneTransitionSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentSceneTransitionSettingsFlow()).ConfigureAwait(false);
    }
  }

  class SetCurrentSceneTransitionSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetCurrentSceneTransitionSettingsAsync(transitionSettings: new Dictionary<string, object?>() {
        { "direction", "up" },
      }).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "overlay": null,
      "transitionSettings": {
        "direction": "up"
      }
    },
    "requestId": "{guid}",
    "requestType": "SetCurrentSceneTransitionSettings"
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
    "requestType": "SetCurrentSceneTransitionSettings"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
