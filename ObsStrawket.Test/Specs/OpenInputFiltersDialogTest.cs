using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class OpenInputFiltersDialogTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new OpenInputFiltersDialogFlow()).ConfigureAwait(false);
    }
  }

  class OpenInputFiltersDialogFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.OpenInputFiltersDialogAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "inputName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "OpenInputFiltersDialog"
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
    "requestType": "OpenInputFiltersDialog"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
