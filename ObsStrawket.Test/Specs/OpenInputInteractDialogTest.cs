using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class OpenInputInteractDialogTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new OpenInputInteractDialogFlow());
    }
  }

  internal class OpenInputInteractDialogFlow : ITestFlow {
    private readonly string _inputName;

    public OpenInputInteractDialogFlow(string? inputName = null) {
      _inputName = inputName ?? CreateInputFlow.InputName;
    }

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.OpenInputInteractDialogAsync(inputName: _inputName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "inputName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "OpenInputInteractDialog"
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
    "requestType": "OpenInputInteractDialog"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
