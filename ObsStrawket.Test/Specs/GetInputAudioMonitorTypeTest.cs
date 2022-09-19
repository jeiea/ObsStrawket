using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputAudioMonitorTypeTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputAudioMonitorTypeFlow()).ConfigureAwait(false);
    }
  }

  class GetInputAudioMonitorTypeFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputAudioMonitorTypeAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      Assert.Equal(SetInputAudioMonitorTypeFlow.MonitoringType, response.MonitorType);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputAudioMonitorType""
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
    ""requestType"": ""GetInputAudioMonitorType"",
    ""responseData"": {
      ""monitorType"": ""OBS_MONITORING_TYPE_MONITOR_ONLY""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
