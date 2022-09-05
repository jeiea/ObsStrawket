using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class StartRecordTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartRecordFlow()).ConfigureAwait(false);
    }
  }

  class StartRecordFlow : ITestFlow {
    static public string EscapedFileName { get; private set; } =
      Assembly.GetExecutingAssembly().Location.Replace(@"\", @"\\");

    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.StartRecordAsync().ConfigureAwait(false);
      Assert.Equal(RequestStatusCode.Success, response.RequestStatus.Code);

      RecordStateChanged changed;
      changed = (RecordStateChanged)await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Starting, changed.OutputState);
      changed = (RecordStateChanged)await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(OutputState.Started, changed.OutputState);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""StartRecord""
  },
  ""op"": 6
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""op"": 7,
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""StartRecord""
  }
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
}".Replace("{file}", EscapedFileName)).ConfigureAwait(false);
    }
  }
}
