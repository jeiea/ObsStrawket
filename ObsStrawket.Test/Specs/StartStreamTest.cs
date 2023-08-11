using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StartStreamTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartStreamFlow()).ConfigureAwait(false);
    }
  }

  class StartStreamFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.StartStreamAsync().ConfigureAwait(false);
      var starting = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Starting, (starting as StreamStateChanged)!.OutputState);
      var started = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(ObsOutputState.Started, (started as StreamStateChanged)!.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""StartStream""
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
    ""requestType"": ""StartStream""
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
    ""eventType"": ""StreamStateChanged""
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
    ""eventType"": ""StreamStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
