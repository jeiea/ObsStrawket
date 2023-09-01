using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSourceFilterDefaultSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSourceFilterDefaultSettingsFlow()).ConfigureAwait(false);
    }
  }

  class GetSourceFilterDefaultSettingsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSourceFilterDefaultSettingsAsync(filterKind: "color_key_filter_v2").ConfigureAwait(false);

      Assert.Equal(0.0, response.DefaultFilterSettings["brightness"]);
      Assert.Equal(65280, response.DefaultFilterSettings["key_color"]);
      Assert.Equal("green", response.DefaultFilterSettings["key_color_type"]);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "filterKind": "color_key_filter_v2"
    },
    "requestId": "{guid}",
    "requestType": "GetSourceFilterDefaultSettings"
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
    "requestType": "GetSourceFilterDefaultSettings",
    "responseData": {
      "defaultFilterSettings": {
        "brightness": 0.0,
        "contrast": 0.0,
        "gamma": 0.0,
        "key_color": 65280,
        "key_color_type": "green",
        "opacity": 1.0,
        "similarity": 80,
        "smoothness": 50
      }
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
