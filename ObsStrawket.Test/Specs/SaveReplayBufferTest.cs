using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SaveReplayBufferTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SaveReplayBufferFlow());
    }
  }

  internal class SaveReplayBufferFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      _ = await client.SaveReplayBufferAsync().ConfigureAwait(false);

      var saved = await ClientFlow.WaitEventAsync<ReplayBufferSaved>(client).ConfigureAwait(false);
      Assert.True(File.Exists(saved.SavedReplayPath));
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "SaveReplayBuffer"
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
    "requestType": "SaveReplayBuffer"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "eventData": {
      "savedReplayPath": "{{MockServer.EscapedFilePath}}"
    },
    "eventIntent": 64,
    "eventType": "ReplayBufferSaved"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
