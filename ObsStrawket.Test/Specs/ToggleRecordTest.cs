using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class ToggleRecordTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new ToggleRecordFlow()).ConfigureAwait(false);
    }
  }

  class ToggleRecordFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      var response = await client.ToggleRecordAsync().ConfigureAwait(false);
      Assert.True(response.OutputActive, "outputActive is not true.");

      RecordStateChanged changed;
      changed = (RecordStateChanged)await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Starting, changed.OutputState);
      changed = (RecordStateChanged)await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Started, changed.OutputState);

      await Task.Delay(100).ConfigureAwait(false);
      response = await client.ToggleRecordAsync().ConfigureAwait(false);
      Assert.False(response.OutputActive, "outputActive is not false.");

      changed = (RecordStateChanged)await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Stopping, changed.OutputState);
      changed = (RecordStateChanged)await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Stopped, changed.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleRecord""
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
    ""requestType"": ""ToggleRecord"",
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
      ""outputPath"": null,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STARTING""
    },
    ""eventIntent"": 64,
    ""eventType"": ""RecordStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": true,
      ""outputPath"": ""{file}"",
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STARTED""
    },
    ""eventIntent"": 64,
    ""eventType"": ""RecordStateChanged""
  },
  ""op"": 5
}".Replace("{file}", StartRecordFlow.EscapedFileName)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleRecord""
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
    ""requestType"": ""ToggleRecord"",
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
      ""outputPath"": null,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STOPPING""
    },
    ""eventIntent"": 64,
    ""eventType"": ""RecordStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": true,
      ""outputPath"": ""{file}"",
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STOPPED""
    },
    ""eventIntent"": 64,
    ""eventType"": ""RecordStateChanged""
  },
  ""op"": 5
}".Replace("{file}", StartRecordFlow.EscapedFileName)).ConfigureAwait(false);
    }
  }
}
