using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateSceneFlow()).ConfigureAwait(false);
    }
  }

  class CreateSceneFlow : ITestFlow {
    public static string NewScene { get => "test scene"; }

    public static string NewScene2 { get => "test scene 2"; }

    public async Task RequestAsync(ObsClientSocket client) {
      await CreateScene(client, NewScene2).ConfigureAwait(false);
      await CreateScene(client, NewScene).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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

      guid = await session.ReceiveAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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
        await client.CreateSceneAsync(sceneName: name).ConfigureAwait(false);
      }
      catch (FailureResponseException failure)
      when (failure.Response.RequestStatus.Code == RequestStatus.ResourceAlreadyExists) {
        // Studio program scene can holds removed input, so reset.
        await client.SetStudioModeEnabledAsync(false).ConfigureAwait(false);
        await client.RemoveSceneAsync(sceneName: name).ConfigureAwait(false);
        try {
          using var cts = new CancellationTokenSource(100);
          await client.Events.ReadAllAsync(cts.Token).FirstAsync(x => x is SceneTransitionStarted).ConfigureAwait(false);
          await client.Events.ReadAllAsync().FirstAsync(x => x is SceneTransitionEnded).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        await client.CreateSceneAsync(sceneName: name).ConfigureAwait(false);
      }

      var events = await client.Events.ReadAllAsync()
        .Where((x) => x is SceneListChanged || x is SceneCreated)
        .Take(2).ToListAsync().ConfigureAwait(false);
      Assert.Contains(events, (x) => x is SceneListChanged changed && changed.Scenes[0].Name == name);
      Assert.Contains(events, (x) => x is SceneCreated created && created.SceneName == name);
    }
  }
}
