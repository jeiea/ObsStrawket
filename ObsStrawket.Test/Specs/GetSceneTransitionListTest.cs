using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneTransitionListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneTransitionListFlow());
    }
  }

  internal class GetSceneTransitionListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneTransitionListAsync().ConfigureAwait(false);

      // Default transitions only: Cut (fixed) and Fade. None is configurable.
      Assert.Equal("fade_transition", response.CurrentSceneTransitionKind);
      Assert.Equal(SetCurrentSceneTransitionFlow.TransitionName, response.CurrentSceneTransitionName);
      Assert.Contains(response.Transitions, static (x) => x.Fixed);
      Assert.Contains(response.Transitions, static (x) => x.Name == SetCurrentSceneTransitionFlow.TransitionName);
      Assert.Contains(response.Transitions, static (x) => x.Kind == "fade_transition");
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
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
      "currentSceneTransitionKind": "fade_transition",
      "currentSceneTransitionName": "Fade",
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
