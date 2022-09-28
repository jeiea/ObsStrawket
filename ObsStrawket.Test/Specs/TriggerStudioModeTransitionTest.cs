using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class TriggerStudioModeTransitionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new TriggerStudioModeTransitionFlow()).ConfigureAwait(false);
    }
  }

  class TriggerStudioModeTransitionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.TriggerStudioModeTransitionAsync().ConfigureAwait(false);

      var started = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal("Fade", (started as SceneTransitionStarted)!.TransitionName);
      var ended = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal("Fade", (ended as SceneTransitionVideoEnded)!.TransitionName);
      var events = await client.Events.ReadAllAsync().Take(3).ToListAsync().ConfigureAwait(false);
      Assert.Contains(events, (x) => x is CurrentPreviewSceneChanged changed && changed.SceneName == CreateSceneFlow.NewScene2);
      Assert.Contains(events, (x) => x is CurrentProgramSceneChanged changed && changed.SceneName == CreateSceneFlow.NewScene);
      Assert.Contains(events, (x) => x is SceneTransitionEnded ended && ended.TransitionName == "Fade");
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""TriggerStudioModeTransition""
  },
  ""op"": 6
}").ConfigureAwait(false);
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
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""TriggerStudioModeTransition""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
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
      ""sceneName"": ""test scene 2""
    },
    ""eventIntent"": 4,
    ""eventType"": ""CurrentPreviewSceneChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneName"": ""test scene""
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
