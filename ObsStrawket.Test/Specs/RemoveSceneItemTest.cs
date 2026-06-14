using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class RemoveSceneItemTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new RemoveSceneItemFlow());
    }
  }

  internal class RemoveSceneItemFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.RemoveSceneItemAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: CreateSceneItemFlow.CreatedItemId
      ).ConfigureAwait(false);
      var itemRemoved = await ClientFlow.WaitEventAsync<SceneItemRemoved>(client).ConfigureAwait(false);
      Assert.Equal(CreateSceneItemFlow.CreatedItemId, itemRemoved.SceneItemId);
      Assert.Equal(CreateSceneFlow.NewScene, itemRemoved.SceneName);
      Assert.Equal(CreateInputFlow.InputName, itemRemoved.SourceName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "sceneItemId": 3,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "RemoveSceneItem"
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
    "requestType": "RemoveSceneItem"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "sceneItemId": 3,
      "sceneName": "test scene",
      "sourceName": "Browser source"
    },
    "eventIntent": 128,
    "eventType": "SceneItemRemoved"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
