using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneItemIndexTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneItemIndexFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneItemIndexFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneItemIndexAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: CreateSceneItemFlow.CreatedItemId
      ).ConfigureAwait(false);
      Assert.Equal(SetSceneItemIndexFlow.NewIndex, response.SceneItemIndex);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneItemId"": 2,
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetSceneItemIndex""
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
    ""requestType"": ""GetSceneItemIndex"",
    ""responseData"": {
      ""sceneItemIndex"": 2
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
