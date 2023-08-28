using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SaveReplayBufferTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SaveReplayBufferFlow()).ConfigureAwait(false);
    }
  }

  class SaveReplayBufferFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      await client.SaveReplayBufferAsync().ConfigureAwait(false);

      var saved = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.True(File.Exists((saved as ReplayBufferSaved)!.SavedReplayPath));
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""SaveReplayBuffer""
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
    ""requestType"": ""SaveReplayBuffer""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""savedReplayPath"": ""{file}""
    },
    ""eventIntent"": 64,
    ""eventType"": ""ReplayBufferSaved""
  },
  ""op"": 5
}".Replace("{file}", MockServer.EscapedFilePath)).ConfigureAwait(false);
    }
  }
}
