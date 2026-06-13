using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentProgramSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentProgramSceneFlow());
    }
  }

  class SetCurrentProgramSceneFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetCurrentProgramSceneAsync(sceneName: CreateSceneFlow.NewScene2).ConfigureAwait(false);
      await ClientFlow.WaitEventsAsync(client,
        e => e is SceneTransitionStarted started && started.TransitionName != "",
        e => e is SceneTransitionVideoEnded videoEnded && videoEnded.TransitionName != "",
        e => e is CurrentProgramSceneChanged changed && changed.SceneName == CreateSceneFlow.NewScene2,
        e => e is SceneTransitionEnded ended && ended.TransitionName != "").ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneName": "test scene 2"
    },
    "requestId": "{guid}",
    "requestType": "SetCurrentProgramScene"
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
    "requestType": "SetCurrentProgramScene"
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
