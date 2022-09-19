using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputAudioTracksTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputAudioTracksFlow()).ConfigureAwait(false);
    }
  }

  class SetInputAudioTracksFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetInputAudioTracksAsync(
        inputName: CreateInputFlow.InputName,
        inputAudioTracks: new Dictionary<string, object?> {
          { "2", false },
        }
      ).ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.True((changed as InputAudioTracksChanged)!.InputAudioTracks["1"] as bool?);
      Assert.False((changed as InputAudioTracksChanged)!.InputAudioTracks["2"] as bool?);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputAudioTracks"": {
        ""2"": false
      },
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetInputAudioTracks""
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
    ""requestType"": ""SetInputAudioTracks""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputAudioTracks"": {
        ""1"": true,
        ""2"": false,
        ""3"": true,
        ""4"": true,
        ""5"": true,
        ""6"": true
      },
      ""inputName"": ""Browser source""
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputAudioTracksChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
