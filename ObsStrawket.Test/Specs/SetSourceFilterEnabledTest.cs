using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSourceFilterEnabledTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSourceFilterEnabledFlow()).ConfigureAwait(false);
    }
  }

  class SetSourceFilterEnabledFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSourceFilterEnabledAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: CreateSourceFilterFlow.FilterName,
        filterEnabled: false
      ).ConfigureAwait(false);

      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.False((changed as SourceFilterEnableStateChanged)!.FilterEnabled);
      Assert.Equal(CreateSourceFilterFlow.FilterName, (changed as SourceFilterEnableStateChanged)!.FilterName);
      Assert.Equal("Browser source", (changed as SourceFilterEnableStateChanged)!.SourceName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""filterEnabled"": false,
      ""filterName"": ""test filter name"",
      ""sourceName"": ""Browser source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetSourceFilterEnabled""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""filterEnabled"": false,
      ""filterName"": ""test filter name"",
      ""sourceName"": ""Browser source""
    },
    ""eventIntent"": 32,
    ""eventType"": ""SourceFilterEnableStateChanged""
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
    ""requestType"": ""SetSourceFilterEnabled""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
