using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneItemEnabledTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneItemEnabledFlow());
    }
  }

  internal class SetSceneItemEnabledFlow : ITestFlow {
    public static int DisablingItemId => CreateSceneItemFlow.CreatedItemId;

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetSceneItemEnabledAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: DisablingItemId,
        sceneItemEnabled: false
      ).ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<SceneItemEnableStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, changed.SceneName);
      Assert.Equal(DisablingItemId, changed.SceneItemId);
      Assert.False(changed.SceneItemEnabled);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "sceneItemEnabled": false,
      "sceneItemId": 3,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "SetSceneItemEnabled"
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
    "requestType": "SetSceneItemEnabled"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "sceneItemEnabled": false,
      "sceneItemId": 3,
      "sceneName": "test scene"
    },
    "eventIntent": 128,
    "eventType": "SceneItemEnableStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
