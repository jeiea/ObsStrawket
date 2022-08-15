namespace ObsDotnetSocket.Test {
  using MessagePack;
  using ObsDotnetSocket.DataTypes;
  using System.Net;
  using System.Net.WebSockets;
  using System.Text.RegularExpressions;
  using Xunit;

  public class ConnectionTest {
    [Fact]
    public async Task TestAgainstObsAsync() {
      var cancellation = new CancellationTokenSource();
      try {
        await RunClientAsync(new Uri("ws://127.0.0.1:4455"), cancellation.Token).ConfigureAwait(false);
      }
      catch {
        cancellation.Cancel();
      }
    }

    [Fact]
    public async Task TestAgainstMockServerAsync() {
      var cancellation = new CancellationTokenSource();
      try {
        var tasks = new[] {
          RunMockServerAsync(cancellation.Token),
          RunClientAsync(new Uri("ws://127.0.0.1:44550"), cancellation.Token),
        };
        await await Task.WhenAny(tasks).ConfigureAwait(false);
        await Task.WhenAll(tasks).ConfigureAwait(false);
      }
      catch {
        cancellation.Cancel();
        throw;
      }
    }

    private static async Task RunClientAsync(Uri uri, CancellationToken token) {
      var client = new ClientSocket();
      await client.ConnectAsync(uri, "ahrEYXzXKytCIlpI", cancellation: token).ConfigureAwait(false);
      var source = new TaskCompletionSource<IEvent>();
      client.OnEvent += source.SetResult;

      var result = await client.RequestAsync(new Request() {
        RequestId = "2521a51c-7040-4830-8181-492ab5477545",
        RequestType = "GetVersion"
      }, token).ConfigureAwait(false);
      if (result.ResponseData!["availableRequests"] is not object[] availableRequests) {
        Assert.Fail("availableRequests not parsed");
        throw new Exception();
      }
      Assert.True(availableRequests[0] is string, "availableRequests are not string");

      var @event = await source.Task.ConfigureAwait(false);
      if (@event is not RecordStateChanged changed) {
        Assert.Fail("Type not converted");
        throw new Exception();
      }
      Assert.True(changed.OutputActive);

      await client.CloseAsync().ConfigureAwait(false);
    }

    private static Task RunMockServerAsync(CancellationToken token) {
      token.ThrowIfCancellationRequested();

      var httpListener = new HttpListener();
      httpListener.Prefixes.Add("http://127.0.0.1:44550/");
      httpListener.Start();

      return RunOneshotServerAsync(httpListener, token);
    }

    private static async Task RunOneshotServerAsync(HttpListener httpListener, CancellationToken token) {
      token.ThrowIfCancellationRequested();

      var buffer = new ArraySegment<byte>(new byte[1024]);

      var context = await httpListener.GetContextAsync().ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      Assert.True(context.Request.IsWebSocketRequest);
      Assert.Equal("obswebsocket.msgpack", context.Request.Headers["Sec-WebSocket-Protocol"]);

      var webSocketContext = await context.AcceptWebSocketAsync(null).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      using var socket = webSocketContext.WebSocket;
      var binary = WebSocketMessageType.Binary;
      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
  ""op"": 0,
  ""d"": {
    ""obsWebSocketVersion"": ""5.0.1"",
    ""rpcVersion"": 1,
    ""authentication"": {
      ""challenge"": ""+IxH4CnCiqpX1rM9scsNynZzbOe4KhDeYcTNS3PDaeY="",
      ""salt"": ""lM1GncleQOaCu9lT1yeUZhFYnqhsLLP1G5lAGo3ixaI=""
    }
  }
}", cancellationToken: token), binary, true, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      await socket.ReceiveAsync(buffer, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();
      string json = MessagePackSerializer.ConvertToJson(buffer, cancellationToken: token);
      TestUtil.AssertJsonEqual(@"{
  ""op"": 1,
  ""d"": {
    ""rpcVersion"": 1,
    ""authentication"": ""J8rNSuYuYOLlFbHzDw8IHA8lTjMgL29Mq/3lFvl1sJI="",
    ""eventSubscriptions"": 2047
  }
}", json);

      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
  ""op"": 2,
  ""d"": {
    ""negotiatedRpcVersion"": 1
  }
}", cancellationToken: token), binary, true, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      await socket.ReceiveAsync(buffer, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();
      json = MessagePackSerializer.ConvertToJson(buffer, cancellationToken: token);
      string guid = Regex.Match(json, @"[0-9a-f]{8}-[0-9a-f]{4}[^""]*").Value;
      TestUtil.AssertJsonEqual(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetVersion"",
    ""requestId"": ""{guid}"",
    ""requestData"":null
  }
}".Replace("{guid}", guid), json);

      // In real, op follows d.
      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
  ""d"": {
    ""requestType"": ""GetVersion"",
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""comment"": null,
      ""result"": true
    },
    ""responseData"": {
      ""availableRequests"": [""PressInputPropertiesButton"", ""GetHotkeyList"", ""OpenInputInteractDialog"", ""SaveSourceScreenshot"", ""GetVersion"", ""SetInputName"", ""SetSceneName"", ""GetStats"", ""TriggerStudioModeTransition"", ""SetInputAudioSyncOffset"", ""GetSceneCollectionList"", ""BroadcastCustomEvent"", ""Sleep"", ""SetSceneSceneTransitionOverride"", ""CallVendorRequest"", ""CreateSceneCollection"", ""SetStudioModeEnabled"", ""TriggerHotkeyByName"", ""OpenVideoMixProjector"", ""TriggerHotkeyByKeySequence"", ""GetPersistentData"", ""SetSceneItemIndex"", ""SetPersistentData"", ""SetCurrentSceneCollection"", ""SetInputMute"", ""SetCurrentPreviewScene"", ""SetCurrentProgramScene"", ""OpenSourceProjector"", ""GetProfileList"", ""SetCurrentProfile"", ""RemoveProfile"", ""CreateProfile"", ""GetProfileParameter"", ""SetProfileParameter"", ""GetInputPropertiesListPropertyItems"", ""GetInputAudioBalance"", ""GetStreamServiceSettings"", ""GetVideoSettings"", ""SetVideoSettings"", ""SetInputAudioBalance"", ""SetInputVolume"", ""SetStreamServiceSettings"", ""GetInputDefaultSettings"", ""GetSpecialInputs"", ""GetInputKindList"", ""GetRecordDirectory"", ""GetInputMute"", ""GetCurrentPreviewScene"", ""GetReplayBufferStatus"", ""GetSourceActive"", ""GetSourceScreenshot"", ""GetSourcePrivateSettings"", ""SetSourcePrivateSettings"", ""SetSourceFilterEnabled"", ""GetInputList"", ""GetSceneList"", ""GetGroupList"", ""SetInputSettings"", ""GetCurrentProgramScene"", ""GetSceneItemId"", ""RemoveScene"", ""CreateScene"", ""GetSceneSceneTransitionOverride"", ""RemoveInput"", ""CreateInput"", ""GetSceneItemLocked"", ""GetInputSettings"", ""ToggleInputMute"", ""SetCurrentSceneTransition"", ""GetInputVolume"", ""GetInputAudioSyncOffset"", ""GetInputAudioMonitorType"", ""SetInputAudioMonitorType"", ""StartVirtualCam"", ""GetInputAudioTracks"", ""SetInputAudioTracks"", ""GetTransitionKindList"", ""GetSceneItemTransform"", ""GetSceneTransitionList"", ""GetVirtualCamStatus"", ""GetCurrentSceneTransition"", ""SetCurrentSceneTransitionDuration"", ""SetCurrentSceneTransitionSettings"", ""GetCurrentSceneTransitionCursor"", ""SetTBarPosition"", ""StopOutput"", ""ToggleOutput"", ""GetSourceFilterList"", ""GetSourceFilterDefaultSettings"", ""CreateSourceFilter"", ""RemoveSourceFilter"", ""SetSourceFilterName"", ""GetSourceFilter"", ""StopRecord"", ""ToggleRecord"", ""SetSourceFilterIndex"", ""SetSourceFilterSettings"", ""SetSceneItemTransform"", ""GetSceneItemList"", ""GetGroupSceneItemList"", ""CreateSceneItem"", ""RemoveSceneItem"", ""DuplicateSceneItem"", ""GetSceneItemEnabled"", ""SetSceneItemEnabled"", ""SetSceneItemLocked"", ""GetSceneItemIndex"", ""StartReplayBuffer"", ""GetSceneItemBlendMode"", ""SetSceneItemBlendMode"", ""GetSceneItemPrivateSettings"", ""SetSceneItemPrivateSettings"", ""StopVirtualCam"", ""ToggleVirtualCam"", ""StopReplayBuffer"", ""ToggleReplayBuffer"", ""SaveReplayBuffer"", ""GetLastReplayBufferReplay"", ""GetOutputList"", ""GetOutputStatus"", ""StartOutput"", ""GetOutputSettings"", ""SetOutputSettings"", ""GetStreamStatus"", ""StopStream"", ""ToggleStream"", ""StartStream"", ""SendStreamCaption"", ""GetRecordStatus"", ""StartRecord"", ""ToggleRecordPause"", ""PauseRecord"", ""ResumeRecord"", ""SetMediaInputCursor"", ""GetMediaInputStatus"", ""OffsetMediaInputCursor"", ""TriggerMediaInputAction"", ""GetStudioModeEnabled"", ""OpenInputPropertiesDialog"", ""OpenInputFiltersDialog"", ""GetMonitorList""],
      ""obsVersion"": ""28.0.0"",
      ""obsWebSocketVersion"": ""5.0.1"",
      ""platform"": ""windows"",
      ""platformDescription"": ""Windows 11 Version 2009"",
      ""rpcVersion"": 1,
      ""supportedImageFormats"": [""bmp"", ""jpeg"", ""jpg"", ""pbm"", ""pgm"", ""png"", ""ppm"", ""xbm"", ""xpm""],
    }
  },
  ""op"": 7
}".Replace("{guid}", guid), cancellationToken: token), binary, true, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
        ""op"": 5,
        ""d"": {
          ""eventType"": ""RecordStateChanged"",
          ""eventIntent"": 1,
          ""eventData"": {
            ""outputActive"": true,
            ""outputPath"": """",
            ""outputState"": null
          }
        }
      }", cancellationToken: token), binary, true, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      httpListener.Close();
    }
  }
}
