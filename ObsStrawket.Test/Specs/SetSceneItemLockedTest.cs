using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneItemLockedTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneItemLockedFlow());
    }
  }

  internal class SetSceneItemLockedFlow : ITestFlow {
    public static int LockingItemId => CreateSceneItemFlow.CreatedItemId;

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetSceneItemLockedAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: LockingItemId,
        sceneItemLocked: true
      ).ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<SceneItemLockStateChanged>(client).ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, changed.SceneName);
      Assert.Equal(LockingItemId, changed.SceneItemId);
      Assert.True(changed.SceneItemLocked);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "sceneItemId": 3,
      "sceneItemLocked": true,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "SetSceneItemLocked"
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
    "requestType": "SetSceneItemLocked"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "sceneItemId": 3,
      "sceneItemLocked": true,
      "sceneName": "test scene"
    },
    "eventIntent": 128,
    "eventType": "SceneItemLockStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
