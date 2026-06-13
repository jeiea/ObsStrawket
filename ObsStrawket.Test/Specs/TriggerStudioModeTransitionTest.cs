using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class TriggerStudioModeTransitionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new TriggerStudioModeTransitionFlow());
    }
  }

  class TriggerStudioModeTransitionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.TriggerStudioModeTransitionAsync().ConfigureAwait(false);

      await ClientFlow.WaitEventsAsync(client,
        e => e is SceneTransitionStarted started && started.TransitionName == "Fade",
        e => e is SceneTransitionVideoEnded videoEnded && videoEnded.TransitionName == "Fade",
        e => e is CurrentPreviewSceneChanged preview && preview.SceneName == CreateSceneFlow.NewScene2,
        e => e is CurrentProgramSceneChanged program && program.SceneName == CreateSceneFlow.NewScene,
        e => e is SceneTransitionEnded ended && ended.TransitionName == "Fade").ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "TriggerStudioModeTransition"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "transitionName": "Fade"
    },
    "eventIntent": 16,
    "eventType": "SceneTransitionStarted"
  },
  "op": 5
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
    "requestType": "TriggerStudioModeTransition"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "transitionName": "Fade"
    },
    "eventIntent": 16,
    "eventType": "SceneTransitionVideoEnded"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneName": "test scene 2"
    },
    "eventIntent": 4,
    "eventType": "CurrentPreviewSceneChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneName": "test scene"
    },
    "eventIntent": 4,
    "eventType": "CurrentProgramSceneChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "transitionName": "Fade"
    },
    "eventIntent": 16,
    "eventType": "SceneTransitionEnded"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
