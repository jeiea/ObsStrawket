using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneItemTransformTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneItemTransformFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneItemTransformFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneItemTransformAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: CreateSceneItemFlow.CreatedItemId
      ).ConfigureAwait(false);
      Assert.Equal("OBS_BOUNDS_SCALE_TO_WIDTH", response.SceneItemTransform["boundsType"]);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneItemId": 3,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "GetSceneItemTransform"
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
    "requestType": "GetSceneItemTransform",
    "responseData": {
      "sceneItemTransform": {
        "alignment": 5,
        "boundsAlignment": 0,
        "boundsHeight": 0.0,
        "boundsType": "OBS_BOUNDS_SCALE_TO_WIDTH",
        "boundsWidth": 0.0,
        "cropBottom": 0,
        "cropLeft": 0,
        "cropRight": 0,
        "cropTop": 0,
        "height": 600.0,
        "positionX": 0.0,
        "positionY": 0.0,
        "rotation": 0.0,
        "scaleX": 1.0,
        "scaleY": 1.0,
        "sourceHeight": 600.0,
        "sourceWidth": 800.0,
        "width": 800.0
      }
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
