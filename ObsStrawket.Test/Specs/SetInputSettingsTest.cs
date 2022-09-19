using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputSettingsFlow()).ConfigureAwait(false);
    }
  }

  class SetInputSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetInputSettingsAsync(
        inputName: CreateInputFlow.InputName,
        inputSettings: new Dictionary<string, object?> {
          { "width", 1000 },
          { "url", "about:blank" }
        }
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source"",
      ""inputSettings"": {
        ""url"": ""about:blank"",
        ""width"": 1000
      },
      ""overlay"": null
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetInputSettings""
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
    ""requestType"": ""SetInputSettings""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
