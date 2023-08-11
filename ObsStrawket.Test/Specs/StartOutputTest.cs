using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StartOutputTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartOutputFlow()).ConfigureAwait(false);
    }
  }

  /// <summary>
  /// Precondition: ToggleReplayBufferFlow
  /// </summary>
  class StartOutputFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      await client.StartOutputAsync(outputName: GetOutputListFlow.OutputName).ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Started, (changed as ReplayBufferStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""outputName"": ""Replay Buffer""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""StartOutput""
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
    ""requestType"": ""StartOutput""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": true,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STARTED""
    },
    ""eventIntent"": 64,
    ""eventType"": ""ReplayBufferStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
