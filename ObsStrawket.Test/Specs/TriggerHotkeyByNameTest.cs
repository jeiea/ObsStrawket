using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class TriggerHotkeyByNameTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new TriggerHotkeyByNameFlow()).ConfigureAwait(false);
    }
  }

  class TriggerHotkeyByNameFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.TriggerHotkeyByNameAsync(hotkeyName: "OBSBasic.ResetStats").ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""hotkeyName"": ""OBSBasic.ResetStats""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""TriggerHotkeyByName""
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
    ""requestType"": ""TriggerHotkeyByName""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
