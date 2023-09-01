using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneSceneTransitionOverrideTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneSceneTransitionOverrideFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneSceneTransitionOverrideFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneSceneTransitionOverrideAsync(sceneName: CreateSceneFlow.NewScene).ConfigureAwait(false);
      Assert.Equal("Fade", response.TransitionName);
      Assert.Equal(200, response.TransitionDuration);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "GetSceneSceneTransitionOverride"
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
    "requestType": "GetSceneSceneTransitionOverride",
    "responseData": {
      "transitionDuration": 200,
      "transitionName": "Fade"
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
