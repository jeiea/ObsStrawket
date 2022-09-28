using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public static string InputName => "Browser source";
    public static string InputKind => "browser_source";
    public static string MediaInputName => "Media source";
    public static string MediaInputKind => "ffmpeg_source";



    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.CreateInputAsync(sceneName: CreateSceneFlow.NewScene, inputName: InputName, inputKind: "browser_source", inputSettings: new Dictionary<string, object?>(), sceneItemEnabled: true).ConfigureAwait(false);
      Assert.NotInRange(response.SceneItemId, int.MinValue, 0);
      var created = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(InputName, (created as InputCreated)!.InputName);
      Assert.Equal(InputKind, (created as InputCreated)!.UnversionedInputKind);
      Assert.Equal(InputKind, (created as InputCreated)!.InputKind);
      var sceneItemCreated = await client.Events.ReadAllAsync().OfType<SceneItemCreated>().FirstAsync().ConfigureAwait(false);
      Assert.Equal(response.SceneItemId, sceneItemCreated.SceneItemId);
      var sceneItemSelected = await client.Events.ReadAllAsync().OfType<SceneItemSelected>().FirstAsync().ConfigureAwait(false);
      Assert.Equal(response.SceneItemId, sceneItemSelected.SceneItemId);
      Assert.Equal(CreateSceneFlow.NewScene, sceneItemSelected.SceneName);

      string relativePath = "../../../../Asset/w256h144f25t20keyint25.mp4";
      string mp4Path = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, relativePath));
      response = await client.CreateInputAsync(
        sceneName: CreateSceneFlow.NewScene,
        inputName: MediaInputName,
        inputKind: MediaInputKind,
        inputSettings: new Dictionary<string, object?> {
          { "hw_decode", true },
          { "local_file", mp4Path },
          { "looping", true },
          { "restart_on_activate", false },
        },
        sceneItemEnabled: true
      ).ConfigureAwait(false);

      Assert.NotInRange(response.SceneItemId, int.MinValue, 0);
      created = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(MediaInputName, (created as InputCreated)!.InputName);
      Assert.Equal(MediaInputKind, (created as InputCreated)!.UnversionedInputKind);
      Assert.Equal(MediaInputKind, (created as InputCreated)!.InputKind);
      sceneItemCreated = await client.Events.ReadAllAsync().OfType<SceneItemCreated>().FirstAsync().ConfigureAwait(false);
      Assert.Equal(response.SceneItemId, sceneItemCreated.SceneItemId);
      sceneItemSelected = await client.Events.ReadAllAsync().OfType<SceneItemSelected>().FirstAsync().ConfigureAwait(false);
      Assert.Equal(response.SceneItemId, sceneItemSelected.SceneItemId);
      Assert.Equal(CreateSceneFlow.NewScene, sceneItemSelected.SceneName);
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
        ""fps"": 30,
        ""fps_custom"": false,
        ""height"": 600,
        ""reroute_audio"": false,
        ""restart_when_active"": false,
        ""shutdown"": false,
        ""url"": ""https://obsproject.com/browser-source"",
        ""webpage_control_level"": 1,
        ""width"": 800
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
      ""sceneItemIndex"": 0,
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
    ""eventData"": {
      ""sceneItemId"": 1,
      ""sceneName"": ""test scene""
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemSelected""
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
    ""requestType"": ""CreateInput"",
    ""responseData"": {
      ""sceneItemId"": 1
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputKind"": ""ffmpeg_source"",
      ""inputName"": ""Media source"",
      ""inputSettings"": {
        ""hw_decode"": true,
        ""local_file"": ""D:\\Repos\\jeiea\\2022\\08\\ObsStrawket\\ObsStrawket.Test\\Asset\\w256h144f25t20keyint25.mp4"",
        ""looping"": true,
        ""restart_on_activate"": false
      },
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
        ""buffering_mb"": 2,
        ""clear_on_media_end"": true,
        ""is_local_file"": true,
        ""linear_alpha"": false,
        ""looping"": false,
        ""reconnect_delay_sec"": 10,
        ""restart_on_activate"": true,
        ""speed_percent"": 100
      },
      ""inputKind"": ""ffmpeg_source"",
      ""inputName"": ""Media source"",
      ""inputSettings"": {
        ""hw_decode"": true,
        ""local_file"": ""ObsStrawket.Test/Asset/w256h144f25t20keyint25.mp4"",
        ""looping"": true,
        ""restart_on_activate"": false
      },
      ""unversionedInputKind"": ""ffmpeg_source""
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputCreated""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneItemId"": 2,
      ""sceneItemIndex"": 1,
      ""sceneName"": ""test scene"",
      ""sourceName"": ""Media source""
    },
    ""eventIntent"": 128,
    ""eventType"": ""SceneItemCreated""
  },
  ""op"": 5
}").ConfigureAwait(false);
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
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""CreateInput"",
    ""responseData"": {
      ""sceneItemId"": 2
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
