using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateSourceFilterTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateSourceFilterFlow()).ConfigureAwait(false);
    }
  }

  class CreateSourceFilterFlow : ITestFlow {
    public static string FilterName => "test filter name";
    public static string FilterName2 => "test filter name 2";

    public async Task RequestAsync(ObsClientSocket client) {
      await client.CreateSourceFilterAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: FilterName,
        filterKind: "color_key_filter_v2",
        filterSettings: new Dictionary<string, object?> {
          { "brightness", 1.0 },
        }
      ).ConfigureAwait(false);

      var created = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(FilterName, (created as SourceFilterCreated)!.FilterName);
      Assert.Equal(1.0, (created as SourceFilterCreated)!.FilterSettings["brightness"]);

      await client.CreateSourceFilterAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: FilterName2,
        filterKind: "color_key_filter_v2"
      ).ConfigureAwait(false);

      created = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(FilterName2, (created as SourceFilterCreated)!.FilterName);
      Assert.Empty((created as SourceFilterCreated)!.FilterSettings);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "filterKind": "color_key_filter_v2",
      "filterName": "test filter name",
      "filterSettings": {
        "brightness": 1
      },
      "sourceName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "CreateSourceFilter"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "defaultFilterSettings": {
        "brightness": 0.0,
        "contrast": 0.0,
        "gamma": 0.0,
        "key_color": 65280,
        "key_color_type": "green",
        "opacity": 1.0,
        "similarity": 80,
        "smoothness": 50
      },
      "filterIndex": 0,
      "filterKind": "color_key_filter_v2",
      "filterName": "test filter name",
      "filterSettings": {
        "brightness": 1.0
      },
      "sourceName": "Browser source"
    },
    "eventIntent": 32,
    "eventType": "SourceFilterCreated"
  },
  "op": 5
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
    "requestType": "CreateSourceFilter"
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "filterKind": "color_key_filter_v2",
      "filterName": "test filter name 2",
      "filterSettings": null,
      "sourceName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "CreateSourceFilter"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "defaultFilterSettings": {
        "brightness": 0.0,
        "contrast": 0.0,
        "gamma": 0.0,
        "key_color": 65280,
        "key_color_type": "green",
        "opacity": 1.0,
        "similarity": 80,
        "smoothness": 50
      },
      "filterIndex": 0,
      "filterKind": "color_key_filter_v2",
      "filterName": "test filter name 2",
      "filterSettings": {},
      "sourceName": "Browser source"
    },
    "eventIntent": 32,
    "eventType": "SourceFilterCreated"
  },
  "op": 5
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
    "requestType": "CreateSourceFilter"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
