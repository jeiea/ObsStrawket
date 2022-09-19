using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputMuteTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputMuteFlow()).ConfigureAwait(false);
    }
  }

  class SetInputMuteFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetInputMuteAsync(inputName: CreateInputFlow.InputName, inputMuted: true).ConfigureAwait(false);
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.True((changed as InputMuteStateChanged)!.InputMuted);
      Assert.Equal(CreateInputFlow.InputName, (changed as InputMuteStateChanged)!.InputName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputMuted"": true,
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetInputMute""
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
    ""requestType"": ""SetInputMute""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputMuted"": true,
      ""inputName"": ""Browser source""
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputMuteStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
