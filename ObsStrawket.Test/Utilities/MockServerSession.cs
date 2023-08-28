namespace ObsStrawket.Test.Utilities {
  using MessagePack;
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Net.WebSockets;
  using System.Text.Json;
  using System.Text.Json.JsonDiffPatch;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Tasks;
  using Xunit;

  partial class MockServerSession : IDisposable {
    [GeneratedRegex("[0-9a-f]{8}-[0-9a-f]{4}[^\"]*")]
    private static partial Regex _guidPattern();

    private readonly ArraySegment<byte> _buffer = new(new byte[1024]);
    private readonly WebSocket _webSocket;
    private readonly CancellationToken _cancellation;
    private bool _isDisposed;

    public MockServerSession(WebSocket webSocket, CancellationToken cancellation) {
      _webSocket = webSocket;
      _cancellation = cancellation;
    }

    public Task SendAsync(string json) {
      _cancellation.ThrowIfCancellationRequested();
      var binary = WebSocketMessageType.Binary;
      byte[] buffer = MessagePackSerializer.ConvertFromJson(json, cancellationToken: _cancellation);
      _ = Task.Run(() => Debug.WriteLine($"Mock send {Regex.Replace(json, @"\s+", "")}"));
      return _webSocket.SendAsync(new ArraySegment<byte>(buffer), binary, true, _cancellation);
    }

    public async Task<string> ReceiveAsync() {
      var result = await _webSocket.ReceiveAsync(_buffer, _cancellation).ConfigureAwait(false);
      _cancellation.ThrowIfCancellationRequested();
      return MessagePackSerializer.ConvertToJson(new ArraySegment<byte>(_buffer.Array!, 0, result.Count), cancellationToken: _cancellation);
    }

    public async Task<string?> ReceiveAsync(string expectedJson) {
      string json = await ReceiveAsync().ConfigureAwait(false);
      _ = Task.Run(() => Debug.WriteLine($"Mock receive {Regex.Replace(json, @"\s+", "")}"));
      string? guid = _guidPattern().Match(json)?.Value;
      AssertJsonEqual(expectedJson.Replace("{guid}", guid ?? ""), json);
      return guid;
    }

    public Task SendGetStudioModeEnabledResponseAsync(string guid, bool modeEnabled = false) {
      // In real, op follows d.
      return SendAsync(@"{
  ""d"": {
    ""requestType"": ""GetStudioModeEnabled"",
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""comment"": null,
      ""result"": true
    },
    ""responseData"": {
      ""studioModeEnabled"": {modeEnabled}
    }
  },
  ""op"": 7
}".Replace("{guid}", guid).Replace("{modeEnabled}", modeEnabled ? "true" : "false"));
    }

    public Task SendGetVersionResponseAsync(string guid, string platformDescription = "Windows 11 Version 2009") {
      return SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": { ""code"": 100, ""result"": true },
    ""requestType"": ""GetVersion"",
    ""responseData"": {
      ""availableRequests"": [""PressInputPropertiesButton"", ""GetHotkeyList"", ""OpenInputInteractDialog"", ""SaveSourceScreenshot"", ""GetVersion"", ""SetInputName"", ""SetSceneName"", ""GetStats"", ""TriggerStudioModeTransition"", ""SetInputAudioSyncOffset"", ""GetSceneCollectionList"", ""BroadcastCustomEvent"", ""Sleep"", ""SetSceneSceneTransitionOverride"", ""CallVendorRequest"", ""CreateSceneCollection"", ""SetStudioModeEnabled"", ""TriggerHotkeyByName"", ""OpenVideoMixProjector"", ""TriggerHotkeyByKeySequence"", ""GetPersistentData"", ""SetSceneItemIndex"", ""SetPersistentData"", ""SetCurrentSceneCollection"", ""SetInputMute"", ""SetCurrentPreviewScene"", ""SetCurrentProgramScene"", ""OpenSourceProjector"", ""GetProfileList"", ""SetCurrentProfile"", ""RemoveProfile"", ""CreateProfile"", ""GetProfileParameter"", ""SetProfileParameter"", ""GetInputPropertiesListPropertyItems"", ""GetInputAudioBalance"", ""GetStreamServiceSettings"", ""GetVideoSettings"", ""SetVideoSettings"", ""SetInputAudioBalance"", ""SetInputVolume"", ""SetStreamServiceSettings"", ""GetInputDefaultSettings"", ""GetSpecialInputs"", ""GetInputKindList"", ""GetRecordDirectory"", ""GetInputMute"", ""GetCurrentPreviewScene"", ""GetReplayBufferStatus"", ""GetSourceActive"", ""GetSourceScreenshot"", ""GetSourcePrivateSettings"", ""SetSourcePrivateSettings"", ""SetSourceFilterEnabled"", ""GetInputList"", ""GetSceneList"", ""GetGroupList"", ""SetInputSettings"", ""GetCurrentProgramScene"", ""GetSceneItemId"", ""RemoveScene"", ""CreateScene"", ""GetSceneSceneTransitionOverride"", ""RemoveInput"", ""CreateInput"", ""GetSceneItemLocked"", ""GetInputSettings"", ""ToggleInputMute"", ""SetCurrentSceneTransition"", ""GetInputVolume"", ""GetInputAudioSyncOffset"", ""GetInputAudioMonitorType"", ""SetInputAudioMonitorType"", ""StartVirtualCam"", ""GetInputAudioTracks"", ""SetInputAudioTracks"", ""GetTransitionKindList"", ""GetSceneItemTransform"", ""GetSceneTransitionList"", ""GetVirtualCamStatus"", ""GetCurrentSceneTransition"", ""SetCurrentSceneTransitionDuration"", ""SetCurrentSceneTransitionSettings"", ""GetCurrentSceneTransitionCursor"", ""SetTBarPosition"", ""StopOutput"", ""ToggleOutput"", ""GetSourceFilterList"", ""GetSourceFilterDefaultSettings"", ""CreateSourceFilter"", ""RemoveSourceFilter"", ""SetSourceFilterName"", ""GetSourceFilter"", ""StopRecord"", ""ToggleRecord"", ""SetSourceFilterIndex"", ""SetSourceFilterSettings"", ""SetSceneItemTransform"", ""GetSceneItemList"", ""GetGroupSceneItemList"", ""CreateSceneItem"", ""RemoveSceneItem"", ""DuplicateSceneItem"", ""GetSceneItemEnabled"", ""SetSceneItemEnabled"", ""SetSceneItemLocked"", ""GetSceneItemIndex"", ""StartReplayBuffer"", ""GetSceneItemBlendMode"", ""SetSceneItemBlendMode"", ""GetSceneItemPrivateSettings"", ""SetSceneItemPrivateSettings"", ""StopVirtualCam"", ""ToggleVirtualCam"", ""StopReplayBuffer"", ""ToggleReplayBuffer"", ""SaveReplayBuffer"", ""GetLastReplayBufferReplay"", ""GetOutputList"", ""GetOutputStatus"", ""StartOutput"", ""GetOutputSettings"", ""SetOutputSettings"", ""GetStreamStatus"", ""StopStream"", ""ToggleStream"", ""StartStream"", ""SendStreamCaption"", ""GetRecordStatus"", ""StartRecord"", ""ToggleRecordPause"", ""PauseRecord"", ""ResumeRecord"", ""SetMediaInputCursor"", ""GetMediaInputStatus"", ""OffsetMediaInputCursor"", ""TriggerMediaInputAction"", ""GetStudioModeEnabled"", ""OpenInputPropertiesDialog"", ""OpenInputFiltersDialog"", ""GetMonitorList""],
      ""obsVersion"": ""27.2.4"",
      ""obsWebSocketVersion"": ""5.0.1"",
      ""platform"": ""windows"",
      ""platformDescription"": ""{platformDescription}"",
      ""rpcVersion"": 1,
      ""supportedImageFormats"": [""bmp"", ""jpeg"", ""jpg"", ""pbm"", ""pgm"", ""png"", ""ppm"", ""xbm"", ""xpm""]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid).Replace("{platformDescription}", platformDescription));
    }

    public Task SendGetRecordDirectoryResponseAsync(string guid) {
      return SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""GetRecordDirectory"",
    ""responseData"": {
      ""recordDirectory"": ""{cd}""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid).Replace("{cd}", Directory.GetCurrentDirectory().Replace(@"\", @"\\")));
    }

    public Task SendStudioModeStateChangedAsync(bool enabled = false) {
      return SendAsync(@"{
  ""op"": 5,
  ""d"": {
    ""eventType"": ""StudioModeStateChanged"",
    ""eventIntent"": 1,
    ""eventData"": {
      ""studioModeEnabled"": {enabled},
    }
  }
}".Replace("{enabled}", enabled ? "true" : "false"));
    }

    public void Dispose() {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          // dispose managed state (managed objects)
          _webSocket.Dispose();
        }

        // free unmanaged resources (unmanaged objects) and override finalizer
        _isDisposed = true;
      }
    }

    private static void AssertJsonEqual(string expected, string actual) {
      var expectation = JsonDocument.Parse(expected);
      var reality = JsonDocument.Parse(actual);
      var options = new JsonSerializerOptions() { WriteIndented = true };
      string expectedJson = JsonSerializer.Serialize(expectation, options);
      string actualJson = JsonSerializer.Serialize(reality, options);
      Assert.True(expectation.DeepEquals(reality),
        $"JSON expected: {expectedJson}, actual: {actualJson}");
    }
  }
}
