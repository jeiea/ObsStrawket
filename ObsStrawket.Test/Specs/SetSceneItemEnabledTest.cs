using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneItemEnabledTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneItemEnabledFlow()).ConfigureAwait(false);
    }
  }

  class SetSceneItemEnabledFlow : ITestFlow {
    public static int DisablingItemId => CreateSceneItemFlow.CreatedItemId;

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSceneItemEnabledAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: DisablingItemId,
        sceneItemEnabled: false
      ).ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (changed as SceneItemEnableStateChanged)!.SceneName);
      Assert.Equal(DisablingItemId, (changed as SceneItemEnableStateChanged)!.SceneItemId);
      Assert.False((changed as SceneItemEnableStateChanged)!.SceneItemEnabled);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneItemEnabled"": false,
      ""sceneItemId"": 2,
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetSceneItemEnabled""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""SetSceneItemEnabled""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItemEnabled"": false,
      ""sceneItemId"": 2,
      ""sceneName"": ""test scene""
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemEnableStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
