using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateInputTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateInputFlow());
    }
  }

  class CreateInputFlow : ITestFlow {
    public static string InputName => "Browser source";
    public static string InputKind => "browser_source";
    public static string MediaInputName => "Media source";
    public static string MediaInputKind => "ffmpeg_source";



    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.CreateInputAsync(sceneName: CreateSceneFlow.NewScene, inputName: InputName, inputKind: "browser_source", inputSettings: [], sceneItemEnabled: true).ConfigureAwait(false);
      Assert.NotInRange(response.SceneItemId, int.MinValue, 0);
      var (created, sceneItemCreated, sceneItemSelected) =
        await ClientFlow.WaitEventsAsync<InputCreated, SceneItemCreated, SceneItemSelected>(
          client,
          e => e.InputName == InputName,
          e => e.SceneItemId == response.SceneItemId,
          e => e.SceneItemId == response.SceneItemId).ConfigureAwait(false);
      Assert.Equal(InputName, created.InputName);
      Assert.Equal(InputKind, created.UnversionedInputKind);
      Assert.Equal(InputKind, created.InputKind);
      Assert.NotEqual(0, created.InputKindCaps);
      Assert.Equal(response.SceneItemId, sceneItemCreated.SceneItemId);
      Assert.Equal(response.SceneItemId, sceneItemSelected.SceneItemId);
      Assert.Equal(CreateSceneFlow.NewScene, sceneItemSelected.SceneName);

      response = await client.CreateInputAsync(
        sceneName: CreateSceneFlow.NewScene,
        inputName: MediaInputName,
        inputKind: MediaInputKind,
        inputSettings: new Dictionary<string, JsonElement?> {
          { "hw_decode", true.ToJsonElement() },
          { "local_file", GetMp4Path().ToJsonElement() },
          { "looping", true.ToJsonElement() },
          { "restart_on_activate", false.ToJsonElement() },
        },
        sceneItemEnabled: true
      ).ConfigureAwait(false);

      Assert.NotInRange(response.SceneItemId, int.MinValue, 0);
      (created, sceneItemCreated, sceneItemSelected) =
        await ClientFlow.WaitEventsAsync<InputCreated, SceneItemCreated, SceneItemSelected>(
          client,
          e => e.InputName == MediaInputName,
          e => e.SceneItemId == response.SceneItemId,
          e => e.SceneItemId == response.SceneItemId).ConfigureAwait(false);
      Assert.Equal(MediaInputName, created.InputName);
      Assert.Equal(MediaInputKind, created.UnversionedInputKind);
      Assert.Equal(MediaInputKind, created.InputKind);
      Assert.NotEqual(0, created.InputKindCaps);
      Assert.Equal(response.SceneItemId, sceneItemCreated.SceneItemId);
      Assert.Equal(response.SceneItemId, sceneItemSelected.SceneItemId);
      Assert.Equal(CreateSceneFlow.NewScene, sceneItemSelected.SceneName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "inputKind": "browser_source",
      "inputName": "Browser source",
      "inputSettings": {},
      "sceneItemEnabled": true,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "CreateInput"
  },
  "op": 6
}
""").ConfigureAwait(false);

      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "defaultInputSettings": {
        "fps": 30,
        "fps_custom": false,
        "height": 600,
        "reroute_audio": false,
        "restart_when_active": false,
        "shutdown": false,
        "url": "https://obsproject.com/browser-source",
        "webpage_control_level": 1,
        "width": 800
      },
      "inputKind": "browser_source",
      "inputKindCaps": 65,
      "inputName": "Browser source",
      "inputSettings": {},
      "unversionedInputKind": "browser_source"
    },
    "eventIntent": 8,
    "eventType": "InputCreated"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneItemId": 1,
      "sceneItemIndex": 0,
      "sceneName": "test scene",
      "sourceName": "Browser source"
    },
    "eventIntent": 128,
    "eventType": "SceneItemCreated"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneItemId": 1,
      "sceneName": "test scene"
    },
    "eventIntent": 128,
    "eventType": "SceneItemSelected"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "CreateInput",
    "responseData": {
      "sceneItemId": 1
    }
  },
  "op": 7
}
""").ConfigureAwait(false);

      string mp4 = GetMp4Path().Replace("\\", "\\\\");
      guid = await session.ReceiveAsync($$"""
{
  "d": {
    "requestData": {
      "inputKind": "ffmpeg_source",
      "inputName": "Media source",
      "inputSettings": {
        "hw_decode": true,
        "local_file": "{{mp4}}",
        "looping": true,
        "restart_on_activate": false
      },
      "sceneItemEnabled": true,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "CreateInput"
  },
  "op": 6
}
""").ConfigureAwait(false);

      await session.SendAsync($$"""
{
  "d": {
    "eventData": {
      "defaultInputSettings": {
        "buffering_mb": 2,
        "clear_on_media_end": true,
        "is_local_file": true,
        "linear_alpha": false,
        "looping": false,
        "reconnect_delay_sec": 10,
        "restart_on_activate": true,
        "speed_percent": 100
      },
      "inputKind": "ffmpeg_source",
      "inputKindCaps": 193,
      "inputName": "Media source",
      "inputSettings": {
        "hw_decode": true,
        "local_file": "{{mp4}}",
        "looping": true,
        "restart_on_activate": false
      },
      "unversionedInputKind": "ffmpeg_source"
    },
    "eventIntent": 8,
    "eventType": "InputCreated"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneItemId": 2,
      "sceneItemIndex": 1,
      "sceneName": "test scene",
      "sourceName": "Media source"
    },
    "eventIntent": 128,
    "eventType": "SceneItemCreated"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneItemId": 2,
      "sceneName": "test scene"
    },
    "eventIntent": 128,
    "eventType": "SceneItemSelected"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "CreateInput",
    "responseData": {
      "sceneItemId": 2
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }

    private static string GetMp4Path() {
      string relativePath = "../../../../Asset/w256h144f25t20keyint25.mp4";
      string mp4Path = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, relativePath));
      return mp4Path;
    }
  }
}
