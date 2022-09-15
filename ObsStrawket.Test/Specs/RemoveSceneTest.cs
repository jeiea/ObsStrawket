using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class RemoveSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new RemoveSceneFlow()).ConfigureAwait(false);
    }
  }

  class RemoveSceneFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.RemoveSceneAsync(sceneName: CreateSceneFlow.NewScene).ConfigureAwait(false);
      var removed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (removed as SceneRemoved)!.SceneName);
      var started = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.IsType<SceneTransitionStarted>(started);
      var sceneListChanged = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal("Scene", (sceneListChanged as SceneListChanged)!.Scenes[0].Name);
      var ended = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.IsType<SceneTransitionVideoEnded>(ended);
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal("Scene", (changed as CurrentProgramSceneChanged)!.SceneName);
      ended = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.IsType<SceneTransitionEnded>(ended);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""RemoveScene""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""isGroup"": false,
      ""sceneName"": ""test scene""
    },
    ""eventIntent"": 4,
    ""eventType"": ""SceneRemoved""
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
    ""requestType"": ""RemoveScene""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""transitionName"": ""Fade""
    },
    ""eventIntent"": 16,
    ""eventType"": ""SceneTransitionStarted""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""scenes"": [
        {
          ""sceneIndex"": 0,
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
      ""transitionName"": ""Fade""
    },
    ""eventIntent"": 16,
    ""eventType"": ""SceneTransitionVideoEnded""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneName"": ""Scene""
    },
    ""eventIntent"": 4,
    ""eventType"": ""CurrentProgramSceneChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""transitionName"": ""Fade""
    },
    ""eventIntent"": 16,
    ""eventType"": ""SceneTransitionEnded""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
