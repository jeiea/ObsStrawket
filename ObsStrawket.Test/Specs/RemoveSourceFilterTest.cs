using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class RemoveSourceFilterTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new RemoveSourceFilterFlow()).ConfigureAwait(false);
    }
  }

  class RemoveSourceFilterFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.RemoveSourceFilterAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: CreateSourceFilterFlow.FilterName
      ).ConfigureAwait(false);

      var removed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal("Browser source", (removed as SourceFilterRemoved)!.SourceName);
      Assert.Equal(CreateSourceFilterFlow.FilterName, (removed as SourceFilterRemoved)!.FilterName);

      await client.RemoveSourceFilterAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: SetSourceFilterNameFlow.NewFilterName
      ).ConfigureAwait(false);

      removed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal("Browser source", (removed as SourceFilterRemoved)!.SourceName);
      Assert.Equal(SetSourceFilterNameFlow.NewFilterName, (removed as SourceFilterRemoved)!.FilterName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""filterName"": ""test filter name"",
      ""sourceName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""RemoveSourceFilter""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""filterName"": ""test filter name"",
      ""sourceName"": ""Browser source""
    },
    ""eventIntent"": 32,
    ""eventType"": ""SourceFilterRemoved""
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
    ""requestType"": ""RemoveSourceFilter""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""filterName"": ""test filter name 3"",
      ""sourceName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""RemoveSourceFilter""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""filterName"": ""test filter name 3"",
      ""sourceName"": ""Browser source""
    },
    ""eventIntent"": 32,
    ""eventType"": ""SourceFilterRemoved""
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
    ""requestType"": ""RemoveSourceFilter""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
