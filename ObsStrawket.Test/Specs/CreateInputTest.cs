using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateInputTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateInputFlow()).ConfigureAwait(false);
    }
  }

  class CreateInputFlow : ITestFlow {
    public static string InputName { get => "Browser source"; }

    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.CreateInputAsync(sceneName: CreateSceneFlow.NewScene, inputName: InputName, inputKind: "browser_source", inputSettings: new Dictionary<string, object?>(), sceneItemEnabled: true).ConfigureAwait(false);
      Assert.Equal(1, response.SceneItemId);
      var created = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(InputName, (created as InputCreated)!.InputName);
      Assert.Equal("browser_source", (created as InputCreated)!.UnversionedInputKind);
      Assert.Equal("browser_source", (created as InputCreated)!.InputKind);
      var sceneItemCreated = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(1, (sceneItemCreated as SceneItemCreated)!.SceneItemId);
      var sceneItemSelected = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(1, (sceneItemSelected as SceneItemSelected)!.SceneItemId);
      Assert.Equal(CreateSceneFlow.NewScene, (sceneItemSelected as SceneItemSelected)!.SceneName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputKind"": ""browser_source"",
      ""inputName"": ""Browser source"",
      ""inputSettings"": {},
      ""sceneItemEnabled"": true,
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""CreateInput""
  },
  ""op"": 6
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""defaultInputSettings"": {
        ""color"": 4291940817,
        ""height"": 1080,
        ""width"": 1920
      },
      ""inputKind"": ""browser_source"",
      ""inputName"": ""Browser source"",
      ""inputSettings"": {},
      ""unversionedInputKind"": ""browser_source""
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputCreated""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItemId"": 1,
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemCreated""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""CreateInput"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
      ""sourceName"": ""Browser source""
    },
    ""responseData"": {
      ""sceneItemId"": 1
      ""sceneItemIndex"": 0,
      ""sceneName"": ""test scene"",
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItemId"": 1,
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
