using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetGroupListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetGroupListFlow()).ConfigureAwait(false);
    }
  }

  class GetGroupListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetGroupListAsync().ConfigureAwait(false);
      Assert.NotNull(response.Groups);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetGroupList"
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
    "requestType": "GetGroupList",
    "responseData": {
      "groups": [
        "test group"
      ]
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
