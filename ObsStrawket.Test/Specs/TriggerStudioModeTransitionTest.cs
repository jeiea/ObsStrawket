using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
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
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene2, (changed as CurrentPreviewSceneChanged)!.SceneName);
      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (changed as CurrentProgramSceneChanged)!.SceneName);
      ended = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal("Fade", (ended as SceneTransitionEnded)!.TransitionName);
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
