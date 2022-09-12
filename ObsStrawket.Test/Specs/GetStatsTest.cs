using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetStatsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetStatsFlow()).ConfigureAwait(false);
    }
  }

  class GetStatsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetStatsAsync().ConfigureAwait(false);
      Assert.NotInRange(response.ActiveFps, double.NegativeInfinity, 0);
      Assert.NotInRange(response.AvailableDiskSpace, double.NegativeInfinity, 0);
      Assert.NotInRange(response.AverageFrameRenderTime, double.NegativeInfinity, 0);
      Assert.NotInRange(response.CpuUsage, double.NegativeInfinity, 0);
      Assert.NotInRange(response.MemoryUsage, double.NegativeInfinity, 0);
      Assert.InRange(response.RenderSkippedFrames, 0, int.MaxValue);
      Assert.NotInRange(response.RenderTotalFrames, int.MinValue, 0);
      Assert.NotInRange(response.WebSocketSessionIncomingMessages, int.MinValue, 0);
      Assert.NotInRange(response.WebSocketSessionOutgoingMessages, int.MinValue, 0);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetStats""
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
    ""requestType"": ""GetStats"",
    ""responseData"": {
      ""activeFps"": 60.0000024000001,
      ""availableDiskSpace"": 193241.00390625,
      ""averageFrameRenderTime"": 0.183004,
      ""cpuUsage"": 0.5014061142054602,
      ""memoryUsage"": 214.12109375,
      ""outputSkippedFrames"": 0,
      ""outputTotalFrames"": 8,
      ""renderSkippedFrames"": 135,
      ""renderTotalFrames"": 282039,
      ""webSocketSessionIncomingMessages"": 3,
      ""webSocketSessionOutgoingMessages"": 3
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
