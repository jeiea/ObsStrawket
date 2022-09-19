using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputVolumeTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputVolumeFlow()).ConfigureAwait(false);
    }
  }

  class GetInputVolumeFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputVolumeAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      Assert.Equal(20.0, response.InputVolumeDb);
      Assert.Equal(10.0, response.InputVolumeMul);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputVolume""
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
    ""requestType"": ""GetInputVolume"",
    ""responseData"": {
      ""inputVolumeDb"": 20.0,
      ""inputVolumeMul"": 10.0
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
