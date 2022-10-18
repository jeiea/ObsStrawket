using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputDefaultSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputDefaultSettingsFlow()).ConfigureAwait(false);
    }
  }

  class GetInputDefaultSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputDefaultSettingsAsync(inputKind: CreateInputFlow.InputKind).ConfigureAwait(false);
      Assert.Equal("https://obsproject.com/browser-source", response.DefaultInputSettings["url"]);
      Assert.Equal(30, response.DefaultInputSettings["fps"]);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputKind"": ""browser_source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputDefaultSettings""
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
    ""requestType"": ""GetInputDefaultSettings"",
    ""responseData"": {
      ""defaultInputSettings"": {
        ""css"": """",
        ""fps"": 30,
        ""fps_custom"": false,
        ""height"": 600,
        ""reroute_audio"": false,
        ""restart_when_active"": false,
        ""shutdown"": false,
        ""url"": ""https://obsproject.com/browser-source"",
        ""webpage_control_level"": 1,
        ""width"": 800
      }
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
