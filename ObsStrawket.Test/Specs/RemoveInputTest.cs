using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class RemoveInputTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new RemoveInputFlow()).ConfigureAwait(false);
    }
  }

  class RemoveInputFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.RemoveInputAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      var removed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateInputFlow.InputName, (removed as InputRemoved)!.InputName);
      var itemRemoved = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (itemRemoved as SceneItemRemoved)!.SceneName);
      Assert.Equal(CreateInputFlow.InputName, (itemRemoved as SceneItemRemoved)!.SourceName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Color Source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""RemoveInput""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Color Source""
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputRemoved""
  },
  ""op"": 5
} ").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""RemoveInput""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItemId"": 1,
      ""sceneName"": ""test scene"",
      ""sourceName"": ""Color Source""
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemRemoved""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
