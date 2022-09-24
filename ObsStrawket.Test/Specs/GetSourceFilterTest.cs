using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSourceFilterTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSourceFilterFlow()).ConfigureAwait(false);
    }
  }

  class GetSourceFilterFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSourceFilterAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: CreateSourceFilterFlow.FilterName
      ).ConfigureAwait(false);

      Assert.False(response.FilterEnabled);
      object brightness = response.FilterSettings["brightness"]!;
      Assert.Equal(1f, brightness is float ? brightness : (float)(double)brightness);
      object gamma = response.FilterSettings["gamma"]!;
      Assert.Equal(1f, brightness is float ? gamma : (float)(double)gamma);
      Assert.Equal((ushort)23000, response.FilterSettings["key_color"]);
      Assert.Equal("red", response.FilterSettings["key_color_type"]);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""filterName"": ""test filter name"",
      ""sourceName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetSourceFilter""
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
    ""requestType"": ""GetSourceFilter"",
    ""responseData"": {
      ""filterEnabled"": false,
      ""filterIndex"": 1,
      ""filterKind"": ""color_key_filter_v2"",
      ""filterSettings"": {
        ""brightness"": 1.0,
        ""gamma"": 1.0,
        ""key_color"": 23000,
        ""key_color_type"": ""red""
      }
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
