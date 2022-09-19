using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class ToggleInputMuteTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new ToggleInputMuteFlow()).ConfigureAwait(false);
    }
  }

  class ToggleInputMuteFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.ToggleInputMuteAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      Assert.False(response.InputMuted);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.False((changed as InputMuteStateChanged)!.InputMuted);
      Assert.Equal(CreateInputFlow.InputName, (changed as InputMuteStateChanged)!.InputName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""ToggleInputMute""
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
    ""requestType"": ""ToggleInputMute"",
    ""responseData"": {
      ""inputMuted"": false
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputMuted"": false,
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
