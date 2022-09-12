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
      Assert.True(response.OutputActive);
      Assert.StartsWith("00:00:0", response.OutputTimecode);
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
      ""outputActive"": true,
      ""outputBytes"": 1469,
      ""outputDuration"": 49,
      ""outputPaused"": true,
      ""outputTimecode"": ""00:00:00.049""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
