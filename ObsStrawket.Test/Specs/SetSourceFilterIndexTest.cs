using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSourceFilterIndexTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSourceFilterIndexFlow()).ConfigureAwait(false);
    }
  }

  class SetSourceFilterIndexFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSourceFilterIndexAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: CreateSourceFilterFlow.FilterName,
        filterIndex: 1
      ).ConfigureAwait(false);

      var reindexed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSourceFilterFlow.FilterName, (reindexed as SourceFilterListReindexed)!.Filters[1].Name);
      Assert.Equal(1, (reindexed as SourceFilterListReindexed)!.Filters[1].Index);
      Assert.Equal("color_key_filter_v2", (reindexed as SourceFilterListReindexed)!.Filters[1].Kind);
      Assert.True((reindexed as SourceFilterListReindexed)!.Filters[1].Enabled);
      Assert.Equal(1.0, TestUtil.ToDoubleOrInt((reindexed as SourceFilterListReindexed)!.Filters[1].Settings["brightness"]!));
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""filterIndex"": 1,
      ""filterName"": ""test filter name"",
      ""sourceName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetSourceFilterIndex""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""filters"": [
        {
          ""filterEnabled"": true,
          ""filterIndex"": 0,
          ""filterKind"": ""color_key_filter_v2"",
          ""filterName"": ""test filter name 2"",
          ""filterSettings"": {}
        },
        {
          ""filterEnabled"": true,
          ""filterIndex"": 1,
          ""filterKind"": ""color_key_filter_v2"",
          ""filterName"": ""test filter name"",
          ""filterSettings"": {
            ""brightness"": 1.0
          }
        }
      ],
      ""sourceName"": ""Browser source""
    },
    ""eventIntent"": 32,
    ""eventType"": ""SourceFilterListReindexed""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""SetSourceFilterIndex""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
