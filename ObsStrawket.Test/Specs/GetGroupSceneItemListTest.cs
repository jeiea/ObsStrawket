using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetGroupSceneItemListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetGroupSceneItemListFlow()).ConfigureAwait(false);
    }
  }

  class GetGroupSceneItemListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetGroupSceneItemListAsync(sceneName: "Group").ConfigureAwait(false);
      Assert.NotEmpty(response.SceneItems);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneName": "Group"
    },
    "requestId": "{guid}",
    "requestType": "GetGroupSceneItemList"
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
    "requestType": "GetGroupSceneItemList",
    "responseData": {
      "sceneItems": [
        {
          "inputKind": "color_source_v3",
          "isGroup": null,
          "sceneItemBlendMode": "OBS_BLEND_NORMAL",
          "sceneItemEnabled": true,
          "sceneItemId": 7,
          "sceneItemIndex": 0,
          "sceneItemLocked": false,
          "sceneItemTransform": {
            "alignment": 5,
            "boundsAlignment": 0,
            "boundsHeight": 0.0,
            "boundsType": "OBS_BOUNDS_NONE",
            "boundsWidth": 0.0,
            "cropBottom": 0,
            "cropLeft": 0,
            "cropRight": 0,
            "cropTop": 0,
            "height": 323.0,
            "positionX": 0.0,
            "positionY": 0.0,
            "rotation": 0.0,
            "scaleX": 0.29895833134651184,
            "scaleY": 0.29907408356666565,
            "sourceHeight": 1080.0,
            "sourceWidth": 1920.0,
            "width": 574.0
          },
          "sourceName": "Color Source",
          "sourceType": "OBS_SOURCE_TYPE_INPUT"
        }
      ]
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
