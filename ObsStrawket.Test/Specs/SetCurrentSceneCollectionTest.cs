using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentSceneCollectionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentSceneCollectionFlow()).ConfigureAwait(false);
    }
  }

  class SetCurrentSceneCollectionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetCurrentSceneCollectionAsync(sceneCollectionName: CreateSceneCollectionFlow.NewSceneCollection).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneCollectionName": "test scene collection"
    },
    "requestId": "{guid}",
    "requestType": "SetCurrentSceneCollection"
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
    "requestType": "SetCurrentSceneCollection"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
