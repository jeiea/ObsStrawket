using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneTransitionListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneTransitionListFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneTransitionListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneTransitionListAsync().ConfigureAwait(false);

      Assert.Equal("swipe_transition", response.CurrentSceneTransitionKind);
      Assert.Equal(SetCurrentSceneTransitionFlow.TransitionName, response.CurrentSceneTransitionName);
      Assert.Contains(response.Transitions, (x) => x.Configurable);
      Assert.Contains(response.Transitions, (x) => x.Fixed);
      Assert.Contains(response.Transitions, (x) => x.Name == SetCurrentSceneTransitionFlow.TransitionName);
      Assert.Contains(response.Transitions, (x) => x.Kind == "swipe_transition");
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetSceneTransitionList"
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
    "requestType": "GetSceneTransitionList",
    "responseData": {
      "currentSceneTransitionKind": "swipe_transition",
      "currentSceneTransitionName": "Swipe",
      "transitions": [
        {
          "transitionConfigurable": false,
          "transitionFixed": true,
          "transitionKind": "cut_transition",
          "transitionName": "Cut"
        },
        {
          "transitionConfigurable": false,
          "transitionFixed": false,
          "transitionKind": "fade_transition",
          "transitionName": "Fade"
        },
        {
          "transitionConfigurable": true,
          "transitionFixed": false,
          "transitionKind": "swipe_transition",
          "transitionName": "Swipe"
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
