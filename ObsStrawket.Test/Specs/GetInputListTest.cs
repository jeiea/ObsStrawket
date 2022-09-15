using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputListFlow()).ConfigureAwait(false);
    }
  }

  class GetInputListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputListAsync("color_source_v3").ConfigureAwait(false);
      Assert.Equal("color_source", response.Inputs[0].UnversionedKind);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputKind"": ""color_source_v3""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputList""
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
    ""requestType"": ""GetInputList"",
    ""responseData"": {
      ""inputs"": [
        {
          ""inputKind"": ""color_source_v3"",
          ""inputName"": ""Color Source"",
          ""unversionedInputKind"": ""color_source""
        }
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
