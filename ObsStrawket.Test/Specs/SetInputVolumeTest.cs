using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputVolumeTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputVolumeFlow()).ConfigureAwait(false);
    }
  }

  class SetInputVolumeFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetInputVolumeAsync(inputName: CreateInputFlow.InputName, inputVolumeMul: 10).ConfigureAwait(false);
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateInputFlow.InputName, (changed as InputVolumeChanged)!.InputName);
      Assert.Equal(20.0, (changed as InputVolumeChanged)!.InputVolumeDb);
      Assert.Equal(10.0, (changed as InputVolumeChanged)!.InputVolumeMul);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source"",
      ""inputVolumeDb"": null,
      ""inputVolumeMul"": 10
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetInputVolume""
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
    ""requestType"": ""SetInputVolume""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Browser source"",
      ""inputVolumeDb"": 20.0,
      ""inputVolumeMul"": 10.0
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputVolumeChanged""
  },
  ""op"": 5
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
