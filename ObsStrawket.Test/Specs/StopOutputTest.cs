using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StopOutputTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StopOutputFlow()).ConfigureAwait(false);
    }
  }

  class StopOutputFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      await client.StopOutputAsync(outputName: GetOutputListFlow.OutputName).ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Stopped, (changed as VirtualcamStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""outputName"": ""virtualcam_output""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""StopOutput""
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
    ""requestType"": ""StopOutput""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": false,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STOPPED""
    },
    ""eventIntent"": 64,
    ""eventType"": ""VirtualcamStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
