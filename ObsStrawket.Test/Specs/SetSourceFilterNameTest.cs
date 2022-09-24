using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSourceFilterNameTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSourceFilterNameFlow()).ConfigureAwait(false);
    }
  }

  class SetSourceFilterNameFlow : ITestFlow {
    public static string NewFilterName => "test filter name 3";

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSourceFilterNameAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: CreateSourceFilterFlow.FilterName2,
        newFilterName: NewFilterName
      ).ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateSourceFilterFlow.FilterName2, (changed as SourceFilterNameChanged)!.OldFilterName);
      Assert.Equal(NewFilterName, (changed as SourceFilterNameChanged)!.FilterName);
      Assert.Equal("Browser source", (changed as SourceFilterNameChanged)!.SourceName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""filterName"": ""test filter name 2"",
      ""newFilterName"": ""test filter name 3"",
      ""sourceName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetSourceFilterName""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""filterName"": ""test filter name 3"",
      ""oldFilterName"": ""test filter name 2"",
      ""sourceName"": ""Browser source""
    },
    ""eventIntent"": 32,
    ""eventType"": ""SourceFilterNameChanged""
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
    ""requestType"": ""SetSourceFilterName""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
