using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetCurrentProgramSceneTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetCurrentProgramSceneFlow()).ConfigureAwait(false);
    }
  }

  class GetCurrentProgramSceneFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetCurrentProgramSceneAsync().ConfigureAwait(false);
      Assert.Equal(CreateSceneFlow.NewScene, response.CurrentProgramSceneName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetCurrentProgramScene""
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
    ""requestType"": ""GetCurrentProgramScene"",
    ""responseData"": {
      ""currentProgramSceneName"": ""test scene""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
