using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetRecordStatusTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new StartRecordFlow()).ConfigureAwait(false);
    }
  }

  class GetRecordStatusFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetRecordStatusAsync().ConfigureAwait(false);
      Assert.Equal("00:00:00.000", response.OutputTimecode);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetRecordStatus""
  },
  ""op"": 6
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""GetRecordStatus"",
    ""responseData"": {
      ""outputActive"": false,
      ""outputBytes"": 0,
      ""outputDuration"": 0,
      ""outputPaused"": false,
      ""outputTimecode"": ""00:00:00.000""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
