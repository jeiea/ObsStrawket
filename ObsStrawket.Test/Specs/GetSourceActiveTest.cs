using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSourceActiveTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSourceActiveFlow()).ConfigureAwait(false);
    }
  }

  class GetSourceActiveFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var list = await client.GetSceneListAsync().ConfigureAwait(false);
      var response = await client.GetSourceActiveAsync(sourceName: list.CurrentProgramSceneName).ConfigureAwait(false);
      Assert.True(response.VideoActive);
    }

    public async Task RespondAsync(MockServerSession session) {
      await new GetSceneListFlow().RespondAsync(session).ConfigureAwait(false);

      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sourceName": "Scene"
    },
    "requestId": "{guid}",
    "requestType": "GetSourceActive"
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
    "requestType": "GetSourceActive",
    "responseData": {
      "videoActive": true,
      "videoShowing": true
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
