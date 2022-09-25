using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetCurrentSceneTransitionCursorTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetCurrentSceneTransitionCursorFlow()).ConfigureAwait(false);
    }
  }

  class GetCurrentSceneTransitionCursorFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetCurrentSceneTransitionCursorAsync().ConfigureAwait(false);
      Assert.Equal(1.0, response.TransitionCursor);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetCurrentSceneTransitionCursor""
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
    ""requestType"": ""GetCurrentSceneTransitionCursor"",
    ""responseData"": {
      ""transitionCursor"": 1.0
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
