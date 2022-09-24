using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class ToggleOutputTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new ToggleOutputFlow()).ConfigureAwait(false);
    }
  }

  class ToggleOutputFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.ToggleOutputAsync(outputName: GetOutputListFlow.OutputName).ConfigureAwait(false);

      //var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      //Assert.Equal(OutputState.Started, (changed as VirtualcamStateChanged)!.OutputState);

      await client.ToggleOutputAsync(outputName: GetOutputListFlow.OutputName).ConfigureAwait(false);

      //changed = await client.Events.ReadAsync().ConfigureAwait(false);
      //Assert.Equal(OutputState.Stopped, (changed as VirtualcamStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""outputName"": ""virtualcam_output""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleOutput""
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
    ""requestType"": ""ToggleOutput""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      // should be mede, but won't.
//      await session.SendAsync(@"{
//  ""d"": {
//    ""eventData"": {
//      ""outputActive"": true,
//      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STARTED""
//    },
//    ""eventIntent"": 64,
//    ""eventType"": ""VirtualcamStateChanged""
//  },
//  ""op"": 5
//}").ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""outputName"": ""virtualcam_output""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleOutput""
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
    ""requestType"": ""ToggleOutput""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      // should be mede, but won't.
//      await session.SendAsync(@"{
//  ""d"": {
//    ""eventData"": {
//      ""outputActive"": false,
//      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STOPPED""
//    },
//    ""eventIntent"": 64,
//    ""eventType"": ""VirtualcamStateChanged""
//  },
//  ""op"": 5
//}").ConfigureAwait(false);
    }
  }
}
