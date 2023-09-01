using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class OpenSourceProjectorTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new OpenSourceProjectorFlow()).ConfigureAwait(false);
    }
  }

  class OpenSourceProjectorFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.OpenSourceProjectorAsync(sourceName: CreateInputFlow.InputName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "monitorIndex": null,
      "projectorGeometry": null,
      "sourceName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "OpenSourceProjector"
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
    "requestType": "OpenSourceProjector"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
