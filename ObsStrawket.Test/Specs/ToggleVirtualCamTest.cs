using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class ToggleVirtualCamTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new ToggleVirtualCamFlow()).ConfigureAwait(false);
    }
  }

  class ToggleVirtualCamFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.ToggleVirtualCamAsync().ConfigureAwait(false);
      Assert.True(response.OutputActive, "outputActive is not true.");

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Started, (changed as VirtualcamStateChanged)!.OutputState);

      response = await client.ToggleVirtualCamAsync().ConfigureAwait(false);
      Assert.False(response.OutputActive, "outputActive is not false.");

      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Stopped, (changed as VirtualcamStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleVirtualCam""
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
    ""requestType"": ""ToggleVirtualCam"",
    ""responseData"": {
      ""outputActive"": true
    }
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
    ""eventType"": ""VirtualcamStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleVirtualCam""
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
    ""requestType"": ""ToggleVirtualCam"",
    ""responseData"": {
      ""outputActive"": false
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": true,
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
