using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class ToggleReplayBufferTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new ToggleReplayBufferFlow()).ConfigureAwait(false);
    }
  }

  class ToggleReplayBufferFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      var response = await client.ToggleReplayBufferAsync().ConfigureAwait(false);
      Assert.True(response.OutputActive);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Starting, (changed as ReplayBufferStateChanged)!.OutputState);
      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Started, (changed as ReplayBufferStateChanged)!.OutputState);

      await Task.Delay(100).ConfigureAwait(false);
      response = await client.ToggleReplayBufferAsync().ConfigureAwait(false);
      Assert.False(response.OutputActive);

      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Stopping, (changed as ReplayBufferStateChanged)!.OutputState);
      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Stopped, (changed as ReplayBufferStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
     string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleReplayBuffer""
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
    ""requestType"": ""ToggleReplayBuffer"",
    ""responseData"": {
      ""outputActive"": true
    }
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

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleReplayBuffer""
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
    ""requestType"": ""ToggleReplayBuffer"",
    ""responseData"": {
      ""outputActive"": false
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": false,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STOPPING""
    },
    ""eventIntent"": 64,
    ""eventType"": ""ReplayBufferStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": false,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STOPPED""
    },
    ""eventIntent"": 64,
    ""eventType"": ""ReplayBufferStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
