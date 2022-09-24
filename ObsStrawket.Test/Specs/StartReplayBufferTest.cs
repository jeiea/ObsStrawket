using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StartReplayBufferTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartReplayBufferFlow()).ConfigureAwait(false);
    }
  }

  /// <summary>
  /// Precondition: SetProfileParameterFlow, SetCurrentProfileFlow
  /// </summary>
  class StartReplayBufferFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.StartReplayBufferAsync().ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Starting, (changed as ReplayBufferStateChanged)!.OutputState);
      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Started, (changed as ReplayBufferStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""StartReplayBuffer""
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
    ""requestType"": ""StartReplayBuffer""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": false,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STARTING""
    },
    ""eventIntent"": 64,
    ""eventType"": ""ReplayBufferStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
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
