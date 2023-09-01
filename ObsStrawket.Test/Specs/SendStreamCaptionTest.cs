using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SendStreamCaptionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SendStreamCaptionFlow()).ConfigureAwait(false);
    }
  }

  class SendStreamCaptionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.SendStreamCaptionAsync(captionText: "test caption text").ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "captionText": "test caption text"
    },
    "requestId": "{guid}",
    "requestType": "SendStreamCaption"
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
    "requestType": "SendStreamCaption"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
