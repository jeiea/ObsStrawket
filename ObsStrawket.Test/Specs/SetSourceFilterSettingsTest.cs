using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSourceFilterSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSourceFilterSettingsFlow()).ConfigureAwait(false);
    }
  }

  class SetSourceFilterSettingsFlow : ITestFlow {
    public static Dictionary<string, object?> FilterSettings => new() {
      { "gamma", 1.0 },
      { "key_color", 23000 },
      { "key_color_type", "red" },
    };

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSourceFilterSettingsAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: "test filter name",
        filterSettings: FilterSettings
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""filterName"": ""test filter name"",
      ""filterSettings"": {
        ""gamma"": 1,
        ""key_color"": 23000,
        ""key_color_type"": ""red""
      },
      ""overlay"": null,
      ""sourceName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetSourceFilterSettings""
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
    ""requestType"": ""SetSourceFilterSettings""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
