namespace ObsDotnetSocket.Test {
  using MessagePack;
  using ObsDotnetSocket.DataTypes;
  using System.Net;
  using System.Net.WebSockets;
  using Xunit;

  public class ConnectionTest {
    [Fact]
    public async Task TestHelloAsync() {
      var cancellation = new CancellationTokenSource();
      try {
        //_ = RunServerAsync(cancellation.Token);
        var connection = await Connection.ConnectAsync(
          new Uri("ws://127.0.0.1:4455"),
          password: "ahrEYXzXKytCIlpI"
        ).ConfigureAwait(false);
        await connection.SendAsync(new Request() {
          RequestId = "2521a51c-7040-4830-8181-492ab5477545",
          RequestType = "GetVersion"
        }).ConfigureAwait(false);
        var result = (RequestResponse)await connection.ReceiveAsync().ConfigureAwait(false);
        if (result.ResponseData["availableRequests"] is not object[] availableRequests) {
          Assert.Fail("availableRequests not parsed");
          throw new Exception();
        }
        Assert.True(availableRequests[0] is string, "availableRequests are not string");
      }
      finally {
        cancellation.Cancel();
      }
    }

    private static Task RunServerAsync(CancellationToken token) {
      token.ThrowIfCancellationRequested();

      var httpListener = new HttpListener();
      httpListener.Prefixes.Add("http://127.0.0.1:44550/");
      httpListener.Start();

      return RunEventLoopAsync(httpListener, token);
    }

    private static async Task RunEventLoopAsync(HttpListener httpListener, CancellationToken token) {
      token.ThrowIfCancellationRequested();

      using var _ = httpListener;

      var buffer = new ArraySegment<byte>(new byte[1024]);

      var context = await httpListener.GetContextAsync().ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      Assert.True(context.Request.IsWebSocketRequest);
      Assert.Equal("obswebsocket.msgpack", context.Request.Headers["Sec-WebSocket-Protocol"]);

      var webSocketContext = await context.AcceptWebSocketAsync(null).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      var socket = webSocketContext.WebSocket;
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
      TestUtil.AssertJsonEqual(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetVersion"",
    ""requestId"": ""2521a51c-7040-4830-8181-492ab5477545""
  }
}", json);

      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
  ""op"": 7,
  ""d"": {
  }
}", cancellationToken: token), binary, true, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
  ""op"": 7,
  ""d"": {
    ""requestType"": ""GetVersion"",
    ""requestId"": ""2521a51c-7040-4830-8181-492ab5477545"",
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
  }
}", cancellationToken: token), binary, true, token).ConfigureAwait(false);
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
    }
  }
}
