using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetVideoSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetVideoSettingsFlow()).ConfigureAwait(false);
    }
  }

  class SetVideoSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetVideoSettingsAsync(
        fpsNumerator: 60000,
        fpsDenominator: 1001,
        baseWidth: 1920,
        baseHeight: 1080,
        outputWidth: 1920,
        outputHeight: 1080
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""baseHeight"": 1080,
      ""baseWidth"": 1920,
      ""fpsDenominator"": 1001,
      ""fpsNumerator"": 60000,
      ""outputHeight"": 1080,
      ""outputWidth"": 1920
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetVideoSettings""
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
    ""requestType"": ""SetVideoSettings""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
