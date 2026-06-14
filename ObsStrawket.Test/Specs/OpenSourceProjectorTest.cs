using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class OpenSourceProjectorTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new OpenSourceProjectorFlow());
    }
  }

  internal class OpenSourceProjectorFlow : ITestFlow {
    private readonly string _sourceName;

    public OpenSourceProjectorFlow(string? sourceName = null) {
      _sourceName = sourceName ?? CreateInputFlow.InputName;
    }

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.OpenSourceProjectorAsync(sourceName: _sourceName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
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
