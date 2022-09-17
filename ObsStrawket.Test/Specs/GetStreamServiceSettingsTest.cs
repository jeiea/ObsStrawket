using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetStreamServiceSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetStreamServiceSettingsFlow()).ConfigureAwait(false);
    }
  }

  class GetStreamServiceSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetStreamServiceSettingsAsync().ConfigureAwait(false);
      Assert.Equal(StreamServiceType.RtmpCommon, response.StreamServiceType);
    }

    public async Task RespondAsync(MockServerSession session) {
      var settings = SetStreamServiceSettingsFlow.GetTestSettings();

      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetStreamServiceSettings""
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
    ""requestType"": ""GetStreamServiceSettings"",
    ""responseData"": {
      ""streamServiceSettings"": {
        ""bwtest"": false,
        ""key"": ""{key}"",
        ""server"": ""{server}"",
        ""service"": ""{service}""
      },
      ""streamServiceType"": ""rtmp_common""
    }
  },
  ""op"": 7
}"
.Replace("{guid}", guid)
.Replace("{key}", settings.Key)
.Replace("{server}", settings.Server)
.Replace("{service}", settings.Service)).ConfigureAwait(false);
    }
  }
}
