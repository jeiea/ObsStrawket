using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputKindListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputKindListFlow()).ConfigureAwait(false);
    }
  }

  class GetInputKindListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputKindListAsync(unversioned: true).ConfigureAwait(false);
      Assert.Contains("image_source", response.InputKinds);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "unversioned": true
    },
    "requestId": "{guid}",
    "requestType": "GetInputKindList"
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
    "requestType": "GetInputKindList",
    "responseData": {
      "inputKinds": [
        "image_source",
        "color_source",
        "slideshow",
        "browser_source",
        "ffmpeg_source",
        "text_gdiplus",
        "text_ft2_source",
        "vlc_source",
        "monitor_capture",
        "window_capture",
        "game_capture",
        "dshow_input",
        "wasapi_input_capture",
        "wasapi_output_capture",
        "wasapi_process_output_capture"
      ]
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
