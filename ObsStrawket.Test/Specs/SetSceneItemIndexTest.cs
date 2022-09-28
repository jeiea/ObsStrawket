using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneItemIndexTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneItemIndexFlow()).ConfigureAwait(false);
    }
  }

  class SetSceneItemIndexFlow : ITestFlow {
    public static int NewIndex => 2;

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSceneItemIndexAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: CreateSceneItemFlow.CreatedItemId,
        sceneItemIndex: NewIndex
      ).ConfigureAwait(false);

      var reindexed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (reindexed as SceneItemListReindexed)!.SceneName);
      Assert.Contains(
        (reindexed as SceneItemListReindexed)!.SceneItems,
        (x) => x.Index == NewIndex && x.Id == CreateSceneItemFlow.CreatedItemId
      );
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneItemId"": 3,
      ""sceneItemIndex"": 2,
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetSceneItemIndex""
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
    ""requestType"": ""SetSceneItemIndex""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItems"": [
        {
          ""sceneItemId"": 1,
          ""sceneItemIndex"": 0
        },
        {
          ""sceneItemId"": 2,
          ""sceneItemIndex"": 1
        },
        {
          ""sceneItemId"": 3,
          ""sceneItemIndex"": 2
        },
        {
          ""sceneItemId"": 4,
          ""sceneItemIndex"": 3
        }
      ],
      ""sceneName"": ""test scene""
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemListReindexed""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
