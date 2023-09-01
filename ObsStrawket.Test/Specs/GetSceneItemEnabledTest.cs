using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneItemEnabledTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneItemEnabledFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneItemEnabledFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneItemEnabledAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: SetSceneItemEnabledFlow.DisablingItemId
      ).ConfigureAwait(false);
      Assert.False(response.SceneItemEnabled);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneItemId": 3,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "GetSceneItemEnabled"
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
    "requestType": "GetSceneItemEnabled",
    "responseData": {
      "sceneItemEnabled": false
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
