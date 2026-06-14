using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSpecialInputsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSpecialInputsFlow());
    }
  }

  internal class GetSpecialInputsFlow : ITestFlow {
    private readonly string? _desktop1;
    private readonly string? _mic1;

    public GetSpecialInputsFlow(string? desktop1 = "Desktop Audio", string? mic1 = "Mic/Aux") {
      _desktop1 = desktop1;
      _mic1 = mic1;
    }

    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSpecialInputsAsync().ConfigureAwait(false);
      Assert.Equal(_desktop1, response.Desktop1);
      Assert.Equal(_mic1, response.Mic1);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetSpecialInputs"
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
    "requestType": "GetSpecialInputs",
    "responseData": {
      "desktop1": "Desktop Audio",
      "desktop2": null,
      "mic1": "Mic/Aux",
      "mic2": null,
      "mic3": null,
      "mic4": null
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
