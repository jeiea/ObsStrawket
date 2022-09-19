using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetCurrentPreviewSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetCurrentPreviewSceneFlow()).ConfigureAwait(false);
    }
  }

  class GetCurrentPreviewSceneFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetCurrentPreviewSceneAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, response.CurrentPreviewSceneName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetCurrentPreviewScene""
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
    ""requestType"": ""GetCurrentPreviewScene"",
    ""responseData"": {
      ""currentPreviewSceneName"": ""test scene""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
