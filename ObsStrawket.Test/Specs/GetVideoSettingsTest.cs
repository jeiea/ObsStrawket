using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetVideoSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetVideoSettingsFlow()).ConfigureAwait(false);
    }
  }

  class GetVideoSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetVideoSettingsAsync().ConfigureAwait(false);
      Assert.Equal(1080, response.BaseHeight);
      Assert.Equal(1920, response.BaseWidth);
      Assert.Equal(1001, response.FpsDenominator);
      Assert.Equal(60000, response.FpsNumerator);
      Assert.Equal(1080, response.OutputHeight);
      Assert.Equal(1920, response.OutputWidth);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetVideoSettings""
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
    ""requestType"": ""GetVideoSettings"",
    ""responseData"": {
      ""baseHeight"": 1080,
      ""baseWidth"": 1920,
      ""fpsDenominator"": 1001,
      ""fpsNumerator"": 60000,
      ""outputHeight"": 1080,
      ""outputWidth"": 1920
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
