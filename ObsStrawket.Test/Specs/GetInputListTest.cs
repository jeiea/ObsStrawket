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
      var response = await client.GetInputListAsync("browser_source").ConfigureAwait(false);
      Assert.Equal("browser_source", response.Inputs[0].UnversionedKind);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "inputKind": "browser_source"
    },
    "requestId": "{guid}",
    "requestType": "GetInputList"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "GetInputList",
    "responseData": {
      "inputs": [
        {
          "inputKind": "browser_source",
          "inputName": "Browser source",
          "unversionedInputKind": "browser_source"
        }
      ]
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
