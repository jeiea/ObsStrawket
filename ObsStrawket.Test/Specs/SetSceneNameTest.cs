using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneNameTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneNameFlow());
    }
  }

  internal class SetSceneNameFlow : ITestFlow {
    private static string ChangedName => "test scene 3";
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetSceneNameAsync(sceneName: CreateSceneFlow.NewScene2, newSceneName: ChangedName).ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<SceneNameChanged>(client).ConfigureAwait(false);
      Assert.Equal(ChangedName, changed.SceneName);

      _ = await client.SetSceneNameAsync(sceneName: ChangedName, newSceneName: CreateSceneFlow.NewScene2).ConfigureAwait(false);

      changed = await ClientFlow.WaitEventAsync<SceneNameChanged>(client).ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene2, changed.SceneName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "newSceneName": "test scene 3",
      "sceneName": "test scene 2"
    },
    "requestId": "{guid}",
    "requestType": "SetSceneName"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "oldSceneName": "test scene 2",
      "sceneName": "test scene 3"
    },
    "eventIntent": 4,
    "eventType": "SceneNameChanged"
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
    "requestType": "SetSceneName"
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "newSceneName": "test scene 2",
      "sceneName": "test scene 3"
    },
    "requestId": "{guid}",
    "requestType": "SetSceneName"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "oldSceneName": "test scene 3",
      "sceneName": "test scene 2"
    },
    "eventIntent": 4,
    "eventType": "SceneNameChanged"
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
    "requestType": "SetSceneName"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
