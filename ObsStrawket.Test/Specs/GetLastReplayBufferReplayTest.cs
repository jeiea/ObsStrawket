using ObsStrawket.Test.Utilities;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetLastReplayBufferReplayTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetLastReplayBufferReplayFlow()).ConfigureAwait(false);
    }
  }

  class GetLastReplayBufferReplayFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetLastReplayBufferReplayAsync().ConfigureAwait(false);
      Assert.True(File.Exists(response.SavedReplayPath));
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetLastReplayBufferReplay""
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
    ""requestType"": ""GetLastReplayBufferReplay"",
    ""responseData"": {
      ""savedReplayPath"": ""{file}""
    }
  },
  ""op"": 7
}"
.Replace("{guid}", guid)
.Replace("{file}", MockServer.EscapedFilePath)).ConfigureAwait(false);
    }
  }
}
