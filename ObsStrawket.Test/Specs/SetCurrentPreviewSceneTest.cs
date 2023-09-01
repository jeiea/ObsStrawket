using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentPreviewSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentPreviewSceneFlow()).ConfigureAwait(false);
    }
  }

  class SetCurrentPreviewSceneFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetCurrentPreviewSceneAsync(sceneName: CreateSceneFlow.NewScene).ConfigureAwait(false);
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, (changed as CurrentPreviewSceneChanged)!.SceneName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sceneName": "test scene"
    },
    "requestId": "{guid}",
    "requestType": "SetCurrentPreviewScene"
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
    "requestType": "SetCurrentPreviewScene"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "sceneName": "test scene"
    },
    "eventIntent": 4,
    "eventType": "CurrentPreviewSceneChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
