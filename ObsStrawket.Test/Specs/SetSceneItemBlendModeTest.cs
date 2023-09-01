using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneItemBlendModeTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneItemBlendModeFlow()).ConfigureAwait(false);
    }
  }

  class SetSceneItemBlendModeFlow : ITestFlow {
    public static BlendingType NewBlendingMode = BlendingType.Screen;

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSceneItemBlendModeAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: CreateSceneItemFlow.CreatedItemId,
        sceneItemBlendMode: NewBlendingMode
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneItemBlendMode": "OBS_BLEND_SCREEN",
      "sceneItemId": 3,
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "SetSceneItemBlendMode"
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
    "requestType": "SetSceneItemBlendMode"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
