using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetTBarPositionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetTBarPositionFlow());
    }
  }

  internal class SetTBarPositionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetTBarPositionAsync(position: 0.5, release: true).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "position": 0.5,
      "release": true
    },
    "requestId": "{guid}",
    "requestType": "SetTBarPosition"
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
    "requestType": "SetTBarPosition"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
