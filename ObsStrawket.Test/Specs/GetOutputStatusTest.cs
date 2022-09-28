using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetOutputStatusTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetOutputStatusFlow()).ConfigureAwait(false);
    }
  }

  class GetOutputStatusFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(1000).ConfigureAwait(false);
      var response = await client.GetOutputStatusAsync(outputName: GetOutputListFlow.OutputName).ConfigureAwait(false);

      Assert.True(response.OutputActive);
      Assert.InRange(response.OutputBytes, 0, int.MaxValue);
      Assert.InRange(response.OutputCongestion, 0, double.PositiveInfinity);
      Assert.InRange(response.OutputDuration, 0, int.MaxValue);
      Assert.False(response.OutputReconnecting);
      Assert.InRange(response.OutputSkippedFrames, 0, int.MaxValue);
      Assert.StartsWith("00:00:0", response.OutputTimecode);
      Assert.InRange(response.OutputTotalFrames, 0, int.MaxValue);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""outputName"": ""Replay Buffer""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetOutputStatus""
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
    ""requestType"": ""GetOutputStatus"",
    ""responseData"": {
      ""outputActive"": true,
      ""outputBytes"": 0,
      ""outputCongestion"": 0.0,
      ""outputDuration"": 483,
      ""outputReconnecting"": false,
      ""outputSkippedFrames"": 0,
      ""outputTimecode"": ""00:00:00.483"",
      ""outputTotalFrames"": 29
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
