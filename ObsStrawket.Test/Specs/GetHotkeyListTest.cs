using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetHotkeyListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetHotkeyListFlow()).ConfigureAwait(false);
    }
  }

  class GetHotkeyListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetHotkeyListAsync().ConfigureAwait(false);
      Assert.Contains("OBSBasic.Screenshot", response.Hotkeys);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetHotkeyList""
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
    ""requestType"": ""GetHotkeyList"",
    ""responseData"": {
      ""hotkeys"": [
        ""OBSBasic.StartStreaming"",
        ""OBSBasic.StopStreaming"",
        ""OBSBasic.ForceStopStreaming"",
        ""OBSBasic.StartRecording"",
        ""OBSBasic.StopRecording"",
        ""OBSBasic.PauseRecording"",
        ""OBSBasic.UnpauseRecording"",
        ""OBSBasic.StartReplayBuffer"",
        ""OBSBasic.StopReplayBuffer"",
        ""OBSBasic.StartVirtualCam"",
        ""OBSBasic.StopVirtualCam"",
        ""OBSBasic.EnablePreview"",
        ""OBSBasic.DisablePreview"",
        ""OBSBasic.ShowContextBar"",
        ""OBSBasic.HideContextBar"",
        ""OBSBasic.TogglePreviewProgram"",
        ""OBSBasic.Transition"",
        ""OBSBasic.ResetStats"",
        ""OBSBasic.Screenshot"",
        ""OBSBasic.SelectedSourceScreenshot"",
        ""libobs.mute"",
        ""libobs.unmute"",
        ""libobs.push-to-mute"",
        ""libobs.push-to-talk"",
        ""libobs.mute"",
        ""libobs.unmute"",
        ""libobs.push-to-mute"",
        ""libobs.push-to-talk"",
        ""OBSBasic.SelectScene"",
        ""libobs.mute"",
        ""libobs.unmute"",
        ""libobs.push-to-mute"",
        ""libobs.push-to-talk"",
        ""libobs.show_scene_item.Audio Input Capture"",
        ""libobs.hide_scene_item.Audio Input Capture"",
        ""libobs.show_scene_item.Audio Output Capture"",
        ""libobs.hide_scene_item.Audio Output Capture"",
        ""OBSBasic.QuickTransition.1"",
        ""OBSBasic.QuickTransition.2"",
        ""OBSBasic.QuickTransition.3""
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
