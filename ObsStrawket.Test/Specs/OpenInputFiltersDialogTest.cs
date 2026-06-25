using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class OpenInputFiltersDialogTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new OpenInputFiltersDialogFlow());
    }
  }

  internal class OpenInputFiltersDialogFlow : ITestFlow {
    private readonly string _inputName;

    public OpenInputFiltersDialogFlow(string? inputName = null) {
      _inputName = inputName ?? CreateInputFlow.InputName;
    }

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.OpenInputFiltersDialogAsync(inputName: _inputName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
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
