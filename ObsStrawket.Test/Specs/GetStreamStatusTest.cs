using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetStreamStatusTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetStreamStatusFlow()).ConfigureAwait(false);
    }
  }

  class GetStreamStatusFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(1000).ConfigureAwait(false);
      var response = await client.GetStreamStatusAsync().ConfigureAwait(false);
      Assert.True(response.OutputActive);
      Assert.NotEqual(0, response.OutputBytes);
      Assert.NotEqual(0, response.OutputDuration);
      Assert.NotEqual("", response.OutputTimecode);
      Assert.NotEqual(0, response.OutputTotalFrames);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetStreamStatus""
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
    ""requestType"": ""GetStreamStatus"",
    ""responseData"": {
      ""outputActive"": true,
      ""outputBytes"": 2258,
      ""outputCongestion"": 0.0,
      ""outputDuration"": 33,
      ""outputReconnecting"": false,
      ""outputSkippedFrames"": 0,
      ""outputTimecode"": ""00:00:00.033"",
      ""outputTotalFrames"": 2
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
