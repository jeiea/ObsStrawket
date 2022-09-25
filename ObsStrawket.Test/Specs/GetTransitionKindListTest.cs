using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetTransitionKindListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetTransitionKindListFlow()).ConfigureAwait(false);
    }
  }

  class GetTransitionKindListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetTransitionKindListAsync().ConfigureAwait(false);

      Assert.Contains("cut_transition", response.TransitionKinds);
      Assert.Contains("fade_transition", response.TransitionKinds);
      Assert.Contains("swipe_transition", response.TransitionKinds);
      Assert.Contains("slide_transition", response.TransitionKinds);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetTransitionKindList""
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
    ""requestType"": ""GetTransitionKindList"",
    ""responseData"": {
      ""transitionKinds"": [
        ""cut_transition"",
        ""fade_transition"",
        ""swipe_transition"",
        ""slide_transition"",
        ""obs_stinger_transition"",
        ""fade_to_color_transition"",
        ""wipe_transition""
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
