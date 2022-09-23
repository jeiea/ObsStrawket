using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneItemLockedTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneItemLockedFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneItemLockedFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneItemLockedAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: SetSceneItemLockedFlow.LockingItemId
      ).ConfigureAwait(false);
      Assert.True(response.SceneItemLocked);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneItemId"": 2,
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetSceneItemLocked""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""GetSceneItemLocked"",
    ""responseData"": {
      ""sceneItemLocked"": true
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
