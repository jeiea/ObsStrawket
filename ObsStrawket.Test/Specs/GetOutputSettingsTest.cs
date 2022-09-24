using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetOutputSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetOutputSettingsFlow()).ConfigureAwait(false);
    }
  }

  class GetOutputSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetOutputSettingsAsync(outputName: GetOutputListFlow.OutputName).ConfigureAwait(false);
      Assert.Equal(SetOutputSettingsFlow.PropValue, response.OutputSettings[SetOutputSettingsFlow.PropName]);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""outputName"": ""virtualcam_output""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetOutputSettings""
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
    ""requestType"": ""GetOutputSettings"",
    ""responseData"": {
      ""outputSettings"": {
        ""test_prop"": ""unused""
      }
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
