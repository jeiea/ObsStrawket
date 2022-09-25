using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetCurrentSceneTransitionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetCurrentSceneTransitionFlow()).ConfigureAwait(false);
    }
  }

  class GetCurrentSceneTransitionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetCurrentSceneTransitionAsync().ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetCurrentSceneTransition""
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
    ""requestType"": ""GetCurrentSceneTransition"",
    ""responseData"": {
      ""transitionConfigurable"": true,
      ""transitionDuration"": 300,
      ""transitionFixed"": false,
      ""transitionKind"": ""swipe_transition"",
      ""transitionName"": ""Swipe"",
      ""transitionSettings"": {
        ""direction"": ""up""
      }
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
