using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputAudioBalanceTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputAudioBalanceFlow()).ConfigureAwait(false);
    }
  }

  class GetInputAudioBalanceFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputAudioBalanceAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      Assert.Equal(SetInputAudioBalanceFlow.InputAudioBalance, response.InputAudioBalance, TestUtil.Epsilon);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputAudioBalance""
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
    ""requestType"": ""GetInputAudioBalance"",
    ""responseData"": {
      ""inputAudioBalance"": 0.6000000238418579
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
