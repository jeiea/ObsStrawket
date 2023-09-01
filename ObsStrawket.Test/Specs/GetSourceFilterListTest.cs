using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSourceFilterListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSourceFilterListFlow()).ConfigureAwait(false);
    }
  }

  class GetSourceFilterListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSourceFilterListAsync(sourceName: CreateInputFlow.InputName).ConfigureAwait(false);

      Assert.Equal(2, response.Filters.Count);
      Assert.Equal(CreateSourceFilterFlow.FilterName2, response.Filters[0].Name);
      Assert.True(response.Filters[0].Enabled);
      Assert.Equal(0, response.Filters[0].Index);

      Assert.Equal(CreateSourceFilterFlow.FilterName, response.Filters[1].Name);
      Assert.True(response.Filters[1].Enabled);
      Assert.Equal(1, response.Filters[1].Index);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "sourceName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "GetSourceFilterList"
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
    "requestType": "GetSourceFilterList",
    "responseData": {
      "filters": [
        {
          "filterEnabled": true,
          "filterIndex": 0,
          "filterKind": "color_key_filter_v2",
          "filterName": "test filter name 2",
          "filterSettings": {}
        },
        {
          "filterEnabled": true,
          "filterIndex": 1,
          "filterKind": "color_key_filter_v2",
          "filterName": "test filter name",
          "filterSettings": {
            "brightness": 1.0
          }
        }
      ]
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
