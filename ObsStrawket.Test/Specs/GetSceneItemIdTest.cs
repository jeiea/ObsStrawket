using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneItemIdTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneItemIdFlow());
    }
  }

  internal class GetSceneItemIdFlow : ITestFlow {
    public static async Task<int> GetSceneItemId(ObsClientSocket client) {
      var response = await client.GetSceneItemIdAsync(
        sceneName: CreateSceneFlow.NewScene,
        sourceName: CreateInputFlow.InputName
      ).ConfigureAwait(false);
      return response.SceneItemId;
    }

    public async Task RequestAsync(ObsClientSocket client) {
      int itemId = await GetSceneItemId(client).ConfigureAwait(false);
      Assert.NotEqual(0, itemId);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "sceneName": "test scene",
      "sourceName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "GetSceneItemId"
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
    "requestType": "GetSceneItemId",
    "responseData": {
      "sceneItemId": 1
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
