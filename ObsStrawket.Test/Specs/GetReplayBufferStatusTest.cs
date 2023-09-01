using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetReplayBufferStatusTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetReplayBufferStatusFlow()).ConfigureAwait(false);
    }
  }

  class GetReplayBufferStatusFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetReplayBufferStatusAsync().ConfigureAwait(false);
      Assert.True(response.OutputActive);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetReplayBufferStatus"
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
    "requestType": "GetReplayBufferStatus",
    "responseData": {
      "outputActive": true
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
