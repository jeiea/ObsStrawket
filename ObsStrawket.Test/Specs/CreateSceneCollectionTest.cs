using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateSceneCollectionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateSceneCollectionFlow());
    }
  }

  class CreateSceneCollectionFlow : ITestFlow {
    public static string NewSceneCollection = "test scene collection";

    public async Task RequestAsync(ObsClientSocket client) {
      await client.CreateSceneCollectionAsync(sceneCollectionName: NewSceneCollection).ConfigureAwait(false);
      // Event makeup and order differ across OBS versions; read until the
      // guaranteed CurrentSceneCollectionChanged and verify what came with it.
      var events = new List<IObsEvent>();
      do {
        events.Add(await client.Events.ReadAsync().ConfigureAwait(false));
      } while (events[^1] is not CurrentSceneCollectionChanged);

      Assert.Contains(events, (x) => x is CurrentSceneCollectionChanging);
      Assert.Equal(NewSceneCollection, ((CurrentSceneCollectionChanged)events[^1]).SceneCollectionName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneCollectionName": "test scene collection"
    },
    "requestId": "{guid}",
    "requestType": "CreateSceneCollection"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneCollectionName": "default"
    },
    "eventIntent": 2,
    "eventType": "CurrentSceneCollectionChanging"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "inputName": "Video Source"
    },
    "eventIntent": 8,
    "eventType": "InputRemoved"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "isGroup": false,
      "sceneName": "Scene"
    },
    "eventIntent": 4,
    "eventType": "SceneCreated"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneCollections": [
        "default",
        "test scene collection"
      ]
    },
    "eventIntent": 2,
    "eventType": "SceneCollectionListChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneCollectionName": "test scene collection"
    },
    "eventIntent": 2,
    "eventType": "CurrentSceneCollectionChanged"
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
    "requestType": "CreateSceneCollection"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
