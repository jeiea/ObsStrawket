using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneListFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneListAsync().ConfigureAwait(false);
      Assert.NotEmpty(response.Scenes);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetSceneList""
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
    ""requestType"": ""GetSceneList"",
    ""responseData"": {
      ""currentPreviewSceneName"": null,
      ""currentProgramSceneName"": ""Scene"",
      ""scenes"": [
        {
          ""sceneIndex"": 0,
          ""sceneName"": ""test scene""
        },
        {
          ""sceneIndex"": 1,
          ""sceneName"": ""Scene""
        }
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
