using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputAudioMonitorTypeTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputAudioMonitorTypeFlow()).ConfigureAwait(false);
    }
  }

  class SetInputAudioMonitorTypeFlow : ITestFlow {
    public static readonly MonitoringType MonitoringType = MonitoringType.Only;

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetInputAudioMonitorTypeAsync(
        inputName: CreateInputFlow.InputName, monitorType: MonitoringType
      ).ConfigureAwait(false);
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateInputFlow.InputName, (changed as InputAudioMonitorTypeChanged)!.InputName);
      Assert.Equal(MonitoringType, (changed as InputAudioMonitorTypeChanged)!.MonitorType);

    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source"",
      ""monitorType"": ""OBS_MONITORING_TYPE_MONITOR_ONLY""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetInputAudioMonitorType""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Browser source"",
      ""monitorType"": ""OBS_MONITORING_TYPE_MONITOR_ONLY""
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputAudioMonitorTypeChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""SetInputAudioMonitorType""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
