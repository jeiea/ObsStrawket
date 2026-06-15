using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateSceneFlow());
    }
  }

  internal class CreateSceneFlow : ITestFlow {
    public static string NewScene => "test scene";

    public static string NewScene2 => "test scene 2";

    public async Task RequestAsync(ObsClientSocket client) {
      await CreateScene(client, NewScene2).ConfigureAwait(false);
      await CreateScene(client, NewScene).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "sceneName": "test scene 2"
    },
    "requestId": "{guid}",
    "requestType": "CreateScene"
  },
  "op": 6
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
    "requestType": "CreateScene"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "scenes": [
        {
          "sceneIndex": 0,
          "sceneName": "test scene 2"
        },
        {
          "sceneIndex": 1,
          "sceneName": "Scene"
        }
      ]
    },
    "eventIntent": 4,
    "eventType": "SceneListChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "isGroup": false,
      "sceneName": "test scene 2"
    },
    "eventIntent": 4,
    "eventType": "SceneCreated"
  },
  "op": 5
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "CreateScene"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "scenes": [
        {
          "sceneIndex": 0,
          "sceneName": "test scene"
        },
        {
          "sceneIndex": 1,
          "sceneName": "test scene 2"
        },
        {
          "sceneIndex": 2,
          "sceneName": "Scene"
        }
      ]
    },
    "eventIntent": 4,
    "eventType": "SceneListChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "isGroup": false,
      "sceneName": "test scene"
    },
    "eventIntent": 4,
    "eventType": "SceneCreated"
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
    "requestType": "CreateScene"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }

    private static async Task CreateScene(ObsClientSocket client, string name) {
      try {
        _ = await client.CreateSceneAsync(sceneName: name).ConfigureAwait(false);
      }
      catch (ObsRequestException failure)
      when (failure.Response.RequestStatus.Code == RequestStatus.ResourceAlreadyExists) {
        // Studio program scene can holds removed input, so reset.
        _ = await client.SetStudioModeEnabledAsync(false).ConfigureAwait(false);
        _ = await client.RemoveSceneAsync(sceneName: name).ConfigureAwait(false);
        try {
          using var cts = new CancellationTokenSource(100);
          _ = await client.Events.ReadAllAsync(cts.Token).FirstAsync(x => x is SceneTransitionStarted).ConfigureAwait(false);
          _ = await client.Events.ReadAllAsync().FirstAsync(x => x is SceneTransitionEnded).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        _ = await client.CreateSceneAsync(sceneName: name).ConfigureAwait(false);
      }

      // SceneListChanged emission and timing differ across OBS versions;
      // SceneCreated is the reliable contract.
      var events = new List<IObsEvent>();
      do {
        events.Add(await client.Events.ReadAsync().ConfigureAwait(false));
      } while (!events.Any((x) => x is SceneCreated created && created.SceneName == name));
    }
  }
}
