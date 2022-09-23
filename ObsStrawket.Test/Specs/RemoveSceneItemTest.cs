using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class RemoveSceneItemTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new RemoveSceneItemFlow()).ConfigureAwait(false);
    }
  }

  class RemoveSceneItemFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.RemoveSceneItemAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: CreateSceneItemFlow.CreatedItemId
      ).ConfigureAwait(false);
      var itemRemoved = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneItemFlow.CreatedItemId, (itemRemoved as SceneItemRemoved)!.SceneItemId);
      Assert.Equal(CreateSceneFlow.NewScene, (itemRemoved as SceneItemRemoved)!.SceneName);
      Assert.Equal(CreateInputFlow.InputName, (itemRemoved as SceneItemRemoved)!.SourceName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneItemId"": 2,
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""RemoveSceneItem""
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
    ""requestType"": ""RemoveSceneItem""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItemId"": 2,
      ""sceneName"": ""test scene"",
      ""sourceName"": ""Browser source""
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemRemoved""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
