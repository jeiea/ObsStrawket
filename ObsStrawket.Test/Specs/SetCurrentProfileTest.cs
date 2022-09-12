using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentProfileTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentProfileFlow()).ConfigureAwait(false);
    }
  }

  class SetCurrentProfileFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.SetCurrentProfileAsync(profileName: "test profile name").ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""profileName"": ""test profile name""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetCurrentProfile""
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
    ""requestType"": ""SetCurrentProfile""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
