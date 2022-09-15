using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentProgramSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentProgramSceneFlow()).ConfigureAwait(false);
    }
  }

  class SetCurrentProgramSceneFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetCurrentProgramSceneAsync(sceneName: CreateSceneFlow.NewScene).ConfigureAwait(false);
      var started = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.IsType<SceneTransitionStarted>(started);
      var ended = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.IsType<SceneTransitionVideoEnded>(ended);
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (changed as CurrentProgramSceneChanged)!.SceneName);
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
    ""requestType"": ""SetCurrentProgramScene""
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
    ""requestType"": ""SetCurrentProgramScene""
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
