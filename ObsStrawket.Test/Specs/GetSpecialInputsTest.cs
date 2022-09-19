using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSpecialInputsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSpecialInputsFlow()).ConfigureAwait(false);
    }
  }

  class GetSpecialInputsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSpecialInputsAsync().ConfigureAwait(false);
      Assert.Equal("Desktop Audio", response.Desktop1);
      Assert.Equal("Mic/Aux", response.Mic1);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetSpecialInputs""
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
    ""requestType"": ""GetSpecialInputs"",
    ""responseData"": {
      ""desktop1"": ""Desktop Audio"",
      ""desktop2"": null,
      ""mic1"": ""Mic/Aux"",
      ""mic2"": null,
      ""mic3"": null,
      ""mic4"": null
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
