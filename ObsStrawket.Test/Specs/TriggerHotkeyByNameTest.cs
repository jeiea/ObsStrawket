using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.IO;
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
      await client.TriggerHotkeyByNameAsync(hotkeyName: "OBSBasic.Screenshot").ConfigureAwait(false);
      var saved = await client.Events.ReadAsync().ConfigureAwait(false);

      string path = (saved as ScreenshotSaved)!.SavedScreenshotPath;
      Assert.NotEmpty(path);
      if (File.Exists(path)) {
        File.Delete(path);
      }
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "hotkeyName": "OBSBasic.Screenshot"
    },
    "requestId": "{guid}",
    "requestType": "TriggerHotkeyByName"
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
    "requestType": "TriggerHotkeyByName"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "eventData": {
      "savedScreenshotPath": "C:\obs-screenshot.png"
    },
    "eventIntent": 1024,
    "eventType": "ScreenshotSaved"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
