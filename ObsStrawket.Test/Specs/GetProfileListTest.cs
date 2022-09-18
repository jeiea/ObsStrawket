using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetProfileListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetProfileListFlow()).ConfigureAwait(false);
    }
  }

  class GetProfileListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetProfileListAsync().ConfigureAwait(false);
      Assert.Equal(CreateProfileFlow.NewProfileName, response.CurrentProfileName);
      Assert.Contains(CreateProfileFlow.NewProfileName, response.Profiles);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetProfileList""
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
    ""requestType"": ""GetProfileList"",
    ""responseData"": {
      ""currentProfileName"": ""test profile name"",
      ""profiles"": [
        ""default"",
        ""test profile name""
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
