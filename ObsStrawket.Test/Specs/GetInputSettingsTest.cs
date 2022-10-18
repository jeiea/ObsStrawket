using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputSettingsFlow()).ConfigureAwait(false);
    }
  }

  class GetInputSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputSettingsAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      Assert.Equal("about:blank", response.InputSettings["url"]);
      Assert.Equal(1000, response.InputSettings["width"]);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputSettings""
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
    ""requestType"": ""GetInputSettings"",
    ""responseData"": {
      ""inputKind"": ""browser_source"",
      ""inputSettings"": {
        ""url"": ""about:blank"",
        ""width"": 1000
      }
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
