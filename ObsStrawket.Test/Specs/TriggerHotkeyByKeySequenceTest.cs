using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class TriggerHotkeyByKeySequenceTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new TriggerHotkeyByKeySequenceFlow());
    }
  }

  internal class TriggerHotkeyByKeySequenceFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.TriggerHotkeyByKeySequenceAsync("OBS_KEY_P", shift: true).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "keyId": "OBS_KEY_P",
      "keyModifiers": {
        "shift": true
      }
    },
    "requestId": "{guid}",
    "requestType": "TriggerHotkeyByKeySequence"
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
    "requestType": "TriggerHotkeyByKeySequence"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
