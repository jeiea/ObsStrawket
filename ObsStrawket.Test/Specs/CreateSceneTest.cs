using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateSceneFlow()).ConfigureAwait(false);
    }
  }

  class CreateSceneFlow : ITestFlow {
    public static string NewScene { get => "test scene"; }

    public async Task RequestAsync(ObsClientSocket client) {
      try {
        await client.CreateSceneAsync(sceneName: NewScene).ConfigureAwait(false);
      }
      catch (FailureResponseException failure)
      when (failure.Response.RequestStatus.Code == RequestStatusCode.ResourceAlreadyExists) {
        await client.RemoveSceneAsync(sceneName: NewScene).ConfigureAwait(false);
        await client.Events.ReadAllAsync()
          .Where((x) => x is SceneRemoved || x is SceneListChanged)
          .Take(2).ToListAsync().ConfigureAwait(false);
        await client.CreateSceneAsync(sceneName: NewScene).ConfigureAwait(false);
      }

      var events = await client.Events.ReadAllAsync()
        .Where((x) => x is SceneListChanged || x is SceneCreated)
        .Take(2).ToListAsync().ConfigureAwait(false);
      Assert.Contains(events, (x) => x is SceneListChanged changed && changed.Scenes[0].Name == NewScene);
      Assert.Contains(events, (x) => x is SceneCreated created && created.SceneName == NewScene);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""CreateScene""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""scenes"": [
        {
          ""sceneIndex"": 0,
          ""sceneName"": ""test scene""
        },
        {
          ""sceneIndex"": 1,
          ""sceneName"": ""Scene""
        }
      ]
    },
    ""eventIntent"": 4,
    ""eventType"": ""SceneListChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""isGroup"": false,
      ""sceneName"": ""test scene""
    },
    ""eventIntent"": 4,
    ""eventType"": ""SceneCreated""
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
    ""requestType"": ""CreateScene""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
