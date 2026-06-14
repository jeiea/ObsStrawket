using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class DuplicateSceneItemTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new DuplicateSceneItemFlow());
    }
  }

  internal class DuplicateSceneItemFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      int id = await GetSceneItemIdFlow.GetSceneItemId(client).ConfigureAwait(false);
      var response = await client.DuplicateSceneItemAsync(sceneName: CreateSceneFlow.NewScene, sceneItemId: id).ConfigureAwait(false);
      Assert.NotEqual(response.SceneItemId, id);

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
      await new GetSceneItemIdFlow().RespondAsync(session).ConfigureAwait(false);

      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "sceneItemId": 1,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "DuplicateSceneItem"
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
    "requestType": "DuplicateSceneItem",
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
      "sceneItemIndex": 2,
      "sceneName": "test scene",
      "sceneItemId": 3,
      "sourceName": "Browser source"
    },
    "eventIntent": 128,
    "eventType": "SceneItemCreated"
  },
  "op": 5
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
