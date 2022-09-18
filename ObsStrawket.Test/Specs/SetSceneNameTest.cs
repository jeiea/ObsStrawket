using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneNameTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneNameFlow()).ConfigureAwait(false);
    }
  }

  class SetSceneNameFlow : ITestFlow {
    private static string ChangedName => "test scene 2";
    public async Task RequestAsync(ObsClientSocket client) {
      var scenes = await GetSceneListFlow.GetSceneListAsync(client).ConfigureAwait(false);
      string name = scenes.Scenes.Find(x => x.Name != CreateSceneFlow.NewScene)!.Name;
      await client.SetSceneNameAsync(sceneName: name, newSceneName: ChangedName).ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ChangedName, (changed as SceneNameChanged)!.SceneName);
    }

    public async Task RespondAsync(MockServerSession session) {
      await new GetSceneListFlow().RespondAsync(session).ConfigureAwait(false);

      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""newSceneName"": ""test scene 2"",
      ""sceneName"": ""Scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetSceneName""
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
    ""requestType"": ""SetSceneName""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""oldSceneName"": ""Scene"",
      ""sceneName"": ""test scene 2""
    },
    ""eventIntent"": 4,
    ""eventType"": ""SceneNameChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
