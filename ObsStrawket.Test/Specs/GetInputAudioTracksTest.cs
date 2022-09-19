using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputAudioTracksTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputAudioTracksFlow()).ConfigureAwait(false);
    }
  }

  class GetInputAudioTracksFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputAudioTracksAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      Assert.True((response.InputAudioTracks["1"] as bool?)!);
      Assert.False((response.InputAudioTracks["2"] as bool?)!);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputAudioTracks""
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
    ""requestType"": ""GetInputAudioTracks"",
    ""responseData"": {
      ""inputAudioTracks"": {
        ""1"": true,
        ""2"": false,
        ""3"": true,
        ""4"": true,
        ""5"": true,
        ""6"": true
      }
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
