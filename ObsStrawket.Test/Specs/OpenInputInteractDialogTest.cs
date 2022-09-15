using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class OpenInputInteractDialogTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new OpenInputInteractDialogFlow()).ConfigureAwait(false);
    }
  }

  class OpenInputInteractDialogFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.OpenInputInteractDialogAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""OpenInputInteractDialog""
  },
  ""op"": 6
} ").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""OpenInputInteractDialog""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
