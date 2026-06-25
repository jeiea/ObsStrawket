using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateSceneItemTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateSceneItemFlow());
    }
  }

  internal class CreateSceneItemFlow : ITestFlow {
    public static int CreatedItemId => 3;

    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.CreateSceneItemAsync(
        sceneName: CreateSceneFlow.NewScene,
        sourceName: CreateInputFlow.InputName,
        sceneItemEnabled: true
      ).ConfigureAwait(false);
      Assert.Equal(CreatedItemId, response.SceneItemId);

      var (created, selected) = await ClientFlow.WaitEventsAsync<SceneItemCreated, SceneItemSelected>(
        client,
        e => e.SceneItemId == response.SceneItemId,
        e => e.SceneItemId == response.SceneItemId).ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, created.SceneName);
      Assert.Equal(CreateInputFlow.InputName, created.SourceName);
      Assert.Equal(response.SceneItemId, created.SceneItemId);
      Assert.Equal(CreateSceneFlow.NewScene, selected.SceneName);
      Assert.Equal(response.SceneItemId, selected.SceneItemId);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "sceneItemEnabled": true,
      "sceneName": "test scene",
      "sourceName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "CreateSceneItem"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "sceneItemId": 3,
      "sceneItemIndex": 2,
      "sceneName": "test scene",
      "sourceName": "Browser source"
    },
    "eventIntent": 128,
    "eventType": "SceneItemCreated"
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
    "requestType": "CreateSceneItem",
    "responseData": {
      "sceneItemId": 3
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "sceneItemId": 3,
      "sceneName": "test scene"
    },
    "eventIntent": 128,
    "eventType": "SceneItemSelected"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
