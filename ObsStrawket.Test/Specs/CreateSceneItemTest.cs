using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateSceneItemTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateSceneItemFlow()).ConfigureAwait(false);
    }
  }

  class CreateSceneItemFlow : ITestFlow {
    public static int CreatedItemId => 2;

    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.CreateSceneItemAsync(
        sceneName: CreateSceneFlow.NewScene,
        sourceName: CreateInputFlow.InputName,
        sceneItemEnabled: true
      ).ConfigureAwait(false);
      Assert.Equal(CreatedItemId, response.SceneItemId);

      var created = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (created as SceneItemCreated)!.SceneName);
      Assert.Equal(CreateInputFlow.InputName, (created as SceneItemCreated)!.SourceName);
      Assert.Equal(response.SceneItemId, (created as SceneItemCreated)!.SceneItemId);

      var selected = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (selected as SceneItemSelected)!.SceneName);
      Assert.Equal(response.SceneItemId, (selected as SceneItemSelected)!.SceneItemId);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneItemEnabled"": true,
      ""sceneName"": ""test scene"",
      ""sourceName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""CreateSceneItem""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItemId"": 2,
      ""sceneItemIndex"": 1,
      ""sceneName"": ""test scene"",
      ""sourceName"": ""Browser source""
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemCreated""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""CreateSceneItem"",
    ""responseData"": {
      ""sceneItemId"": 2
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItemId"": 2,
      ""sceneName"": ""test scene""
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemSelected""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
