using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetRecordStatusTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetRecordStatusFlow());
    }
  }

  internal class GetRecordStatusFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(1000).ConfigureAwait(false);
      var response = await client.GetRecordStatusAsync().ConfigureAwait(false);

      Assert.True(response.OutputActive);
      Assert.NotInRange(response.OutputBytes, int.MinValue, 0);
      Assert.NotInRange(response.OutputDuration, int.MinValue, 0);
      Assert.Matches(@"^\d{2}:\d{2}:\d{2}\.\d{3}$", response.OutputTimecode);
      Assert.False(response.OutputPaused);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetRecordStatus"
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
    "requestType": "GetRecordStatus",
    "responseData": {
      "outputActive": true,
      "outputBytes": 4989218756,
      "outputDuration": 1684583,
      "outputPaused": false,
      "outputTimecode": "00:28:04.583"
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
