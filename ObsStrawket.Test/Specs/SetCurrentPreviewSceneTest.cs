using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentPreviewSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentPreviewSceneFlow());
    }
  }

  internal class SetCurrentPreviewSceneFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetCurrentPreviewSceneAsync(sceneName: CreateSceneFlow.NewScene).ConfigureAwait(false);
      var changed = await ClientFlow.WaitEventAsync<CurrentPreviewSceneChanged>(client).ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, changed.SceneName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
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
      await session.SendAsync(/*lang=json,strict*/ """
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
