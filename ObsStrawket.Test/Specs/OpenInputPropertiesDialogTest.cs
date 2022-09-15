using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class OpenInputPropertiesDialogTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new OpenInputPropertiesDialogFlow()).ConfigureAwait(false);
    }
  }

  class OpenInputPropertiesDialogFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.OpenInputPropertiesDialogAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""OpenInputPropertiesDialog""
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
    ""requestType"": ""OpenInputPropertiesDialog""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
