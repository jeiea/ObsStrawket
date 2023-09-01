using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class OpenVideoMixProjectorTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new OpenVideoMixProjectorFlow()).ConfigureAwait(false);
    }
  }

  class OpenVideoMixProjectorFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.OpenVideoMixProjectorAsync(videoMixType: VideoMixType.Program).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "monitorIndex": null,
      "projectorGeometry": null,
      "videoMixType": "OBS_WEBSOCKET_VIDEO_MIX_TYPE_PROGRAM"
    },
    "requestId": "{guid}",
    "requestType": "OpenVideoMixProjector"
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
    "requestType": "OpenVideoMixProjector"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
