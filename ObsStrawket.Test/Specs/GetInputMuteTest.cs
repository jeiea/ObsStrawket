using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputMuteTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputMuteFlow()).ConfigureAwait(false);
    }
  }

  class GetInputMuteFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputMuteAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      Assert.True(response.InputMuted);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputMute""
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
    ""requestType"": ""GetInputMute"",
    ""responseData"": {
      ""inputMuted"": true
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
