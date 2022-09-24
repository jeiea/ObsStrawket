using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetVirtualCamStatusTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetVirtualCamStatusFlow()).ConfigureAwait(false);
    }
  }

  class GetVirtualCamStatusFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetVirtualCamStatusAsync().ConfigureAwait(false);
      Assert.True(response.OutputActive);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetVirtualCamStatus""
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
    ""requestType"": ""GetVirtualCamStatus"",
    ""responseData"": {
      ""outputActive"": true
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
