using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class PressInputPropertiesButtonTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new PressInputPropertiesButtonFlow());
    }
  }

  internal class PressInputPropertiesButtonFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.PressInputPropertiesButtonAsync(inputName: CreateInputFlow.InputName, propertyName: "refreshnocache").ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "inputName": "Browser source",
      "propertyName": "refreshnocache"
    },
    "requestId": "{guid}",
    "requestType": "PressInputPropertiesButton"
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
    "requestType": "PressInputPropertiesButton"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
