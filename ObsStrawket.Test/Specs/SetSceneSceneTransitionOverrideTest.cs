using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneSceneTransitionOverrideTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneSceneTransitionOverrideFlow()).ConfigureAwait(false);
    }
  }

  class SetSceneSceneTransitionOverrideFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSceneSceneTransitionOverrideAsync(sceneName: CreateSceneFlow.NewScene, transitionName: "Fade", transitionDuration: 200).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneName": "test scene",
      "transitionDuration": 200,
      "transitionName": "Fade"
    },
    "requestId": "{guid}",
    "requestType": "SetSceneSceneTransitionOverride"
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
    "requestType": "SetSceneSceneTransitionOverride"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
