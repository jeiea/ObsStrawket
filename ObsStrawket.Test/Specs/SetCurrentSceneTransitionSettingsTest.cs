using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentSceneTransitionSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentSceneTransitionSettingsFlow());
    }
  }

  internal class SetCurrentSceneTransitionSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetCurrentSceneTransitionSettingsAsync(transitionSettings: new Dictionary<string, JsonElement?>() {
        { "direction", "up".ToJsonElement() },
      }).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
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
