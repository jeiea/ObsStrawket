using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneCollectionListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneCollectionListFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneCollectionListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneCollectionListAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneCollectionFlow.NewSceneCollection, response.CurrentSceneCollectionName);
      Assert.Contains(CreateSceneCollectionFlow.NewSceneCollection, response.SceneCollections);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetSceneCollectionList"
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
    "requestType": "GetSceneCollectionList",
    "responseData": {
      "currentSceneCollectionName": "test scene collection",
      "sceneCollections": [
        "default",
        "test scene collection"
      ]
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
