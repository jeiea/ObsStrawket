using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StopRecordTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StopRecordFlow()).ConfigureAwait(false);
    }
  }

  class StopRecordFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.StopRecordAsync().ConfigureAwait(false);
      Assert.Equal(RequestStatusCode.Success, response.RequestStatus.Code);

      RecordStateChanged changed;
      changed = (RecordStateChanged)await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Stopping, changed.OutputState);
      changed = (RecordStateChanged)await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Stopped, changed.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""StopRecord""
  }
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""StopRecord"",
    ""responseData"": {
      ""outputPath"": ""{file}""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid).Replace("{file}", StartRecordFlow.EscapedFileName)).ConfigureAwait(false);
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
      ""outputActive"": false,
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
