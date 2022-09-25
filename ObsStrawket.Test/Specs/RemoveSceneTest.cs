using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
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
      await RemoveSceneAsync(client, CreateSceneFlow.NewScene2).ConfigureAwait(false);
      await RemoveSceneAsync(client, CreateSceneFlow.NewScene).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneName"": ""test scene 2""
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
      ""sceneName"": ""test scene 2""
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
      ""scenes"": [
        {
          ""sceneIndex"": 0,
          ""sceneName"": ""Scene""
        },
        {
          ""sceneIndex"": 1,
          ""sceneName"": ""test scene""
        }
      ]
    },
    ""eventIntent"": 4,
    ""eventType"": ""SceneListChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
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

    private static async Task RemoveSceneAsync(ObsClientSocket client, string name) {
      await client.RemoveSceneAsync(sceneName: name).ConfigureAwait(false);
      var removed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(name, (removed as SceneRemoved)!.SceneName);

      var changed = await client.Events
        .ReadAllAsync().OfType<SceneListChanged>()
        .FirstAsync().ConfigureAwait(false);
      Assert.NotEmpty(changed.Scenes);
      Assert.DoesNotContain(changed.Scenes, (x) => x.Name == name);
    }
  }
}
