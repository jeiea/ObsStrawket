namespace ObsDotnetSocket.Test {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Net;
  using System.Net.WebSockets;
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Threading.Channels;
  using System.Threading.Tasks;
  using Xunit;
  using Xunit.Abstractions;

  public class MockServerTest {
    private readonly Uri _mockServerUri = new("ws://127.0.0.1:44550");
    public const string Password = "ahrEYXzXKytCIlpI";

    private readonly ITestOutputHelper _output;

    public MockServerTest(ITestOutputHelper output) {
      _output = output;
    }

    [Fact]
    public async Task TestAgainstMockServerAsync() {
      var cancellation = new CancellationTokenSource();
      try {
        var server = RunMockServer(cancellation.Token);
        await new CommonFlow().RunClientAsync(_mockServerUri, cancellation: cancellation.Token);
        server.Stop();
      }
      catch {
        cancellation.Cancel();
        throw;
      }
    }

    [Fact]
    public async Task TestStressAsync() {
      var cancellation = new CancellationTokenSource();
      var token = cancellation.Token;
      try {
        HttpListener server;
        var abort = Task.CompletedTask;
        var client = new ObsClientSocket();
        var connections = new List<bool>();
        client.Closed += (o) => {
          connections.Add(false);
        };
        server = RunMockServer(token);

        async Task ConnectAsync() {
          await client.ConnectAsync(_mockServerUri, Password, cancellation: token).ConfigureAwait(false);
          connections.Add(true);
        }

        var tasks = new List<Task>();
        for (int i = 0; i < 50; i++) {
          await abort.ConfigureAwait(false);

          tasks.Add(Task.Run(async () => {
            try {
              await ConnectAsync().ConfigureAwait(false);
              var version = await client.GetVersionAsync(token).ConfigureAwait(false);
              Assert.Contains("bmp", version.SupportedImageFormats);
            }
            catch (Exception ex) {
              Debug.WriteLine(ex);
            }
          }));
          tasks.Add(Task.Run(async () => {
            try {
              await client.CloseAsync();
            }
            catch (Exception ex) {
              Debug.WriteLine(ex);
            }
          }));
          await Task.Delay(i * 4).ConfigureAwait(false);
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);

        int open = connections.Where(x => x == true).Count();
        int close = connections.Count - open;
        Assert.True(Math.Abs(open - close) <= 1, "open and close count doesn't match");

        server.Close();
      }
      catch {
        cancellation.Cancel();
        throw;
      }
    }

    [Fact]
    public async Task TestServerAbortAsync() {
      CancellationTokenSource cancellation = new();
      try {
        HttpListener server;
        var abort = Task.CompletedTask;
        var client = new ObsClientSocket();
        client.Closed += (o) => {
          Debug.WriteLine(o);
        };
        for (int i = 0; i < 50; i++) {
          cancellation = new CancellationTokenSource();
          await abort.ConfigureAwait(false);
          Debug.WriteLine($"Session {i} start");

          server = RunMockServer(cancellation.Token);
          int milliseconds = i * 4;
          abort = Task.Run(async () => {
            await Task.Delay(milliseconds);
            server.Abort();
          });

          try {
            await new CommonFlow().RunClientAsync(_mockServerUri, client, cancellation: cancellation.Token);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
          cancellation.Cancel();
        }

        cancellation = new CancellationTokenSource();
        await abort.ConfigureAwait(false);
        server = RunMockServer(cancellation.Token);
        await new CommonFlow().RunClientAsync(_mockServerUri, client, cancellation: cancellation.Token);
        server.Close();
      }
      catch {
        cancellation.Cancel();
        throw;
      }
    }

    private static HttpListener RunMockServer(CancellationToken token) {
      token.ThrowIfCancellationRequested();

      var httpListener = new HttpListener();
      httpListener.Prefixes.Add("http://127.0.0.1:44550/");
      httpListener.Start();

      _ = Task.Run(() => ServeForeverAsync(httpListener, ServeAsync, token), token);
      return httpListener;
    }

    private static async Task ServeForeverAsync(HttpListener http, Func<HttpListenerContext, CancellationToken, Task> action, CancellationToken token) {
      while (!token.IsCancellationRequested && http.IsListening) {
        var context = await http.GetContextAsync().ConfigureAwait(false);
        _ = action(context, token);
      }
    }

    private static async Task<(WebSocketContext, MockServerSession)> HandshakeAsync(HttpListenerContext context, CancellationToken token) {
      token.ThrowIfCancellationRequested();

      Assert.True(context.Request.IsWebSocketRequest);
      string subProtocol = "obswebsocket.msgpack";
      Assert.Equal(subProtocol, context.Request.Headers["Sec-WebSocket-Protocol"]);

      var webSocketContext = await context.AcceptWebSocketAsync(subProtocol).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      var session = new MockServerSession(webSocketContext.WebSocket, token);
      await session.SendAsync(@"{
  ""op"": 0,
  ""d"": {
    ""obsWebSocketVersion"": ""5.0.1"",
    ""rpcVersion"": 1,
    ""authentication"": {
      ""challenge"": ""+IxH4CnCiqpX1rM9scsNynZzbOe4KhDeYcTNS3PDaeY="",
      ""salt"": ""lM1GncleQOaCu9lT1yeUZhFYnqhsLLP1G5lAGo3ixaI=""
    }
  }
}").ConfigureAwait(false);

      await session.ReceiveAsync(@"{
  ""op"": 1,
  ""d"": {
    ""rpcVersion"": 1,
    ""authentication"": ""J8rNSuYuYOLlFbHzDw8IHA8lTjMgL29Mq/3lFvl1sJI="",
    ""eventSubscriptions"": 2047
  }
}");

      await session.SendAsync(@"{
  ""op"": 2,
  ""d"": {
    ""negotiatedRpcVersion"": 1
  }
}").ConfigureAwait(false);

      return (webSocketContext, session);
    }

    private static async Task ServeAsync(HttpListenerContext context, CancellationToken token) {
      var (webSocketContext, session) = await HandshakeAsync(context, token).ConfigureAwait(false);
      using var _ = session;

      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetVersion""
  },
  ""op"": 6
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": { ""code"": 100, ""result"": true },
    ""requestType"": ""GetVersion"",
    ""responseData"": {
      ""availableRequests"": [""PressInputPropertiesButton"", ""GetHotkeyList"", ""OpenInputInteractDialog"", ""SaveSourceScreenshot"", ""GetVersion"", ""SetInputName"", ""SetSceneName"", ""GetStats"", ""TriggerStudioModeTransition"", ""SetInputAudioSyncOffset"", ""GetSceneCollectionList"", ""BroadcastCustomEvent"", ""Sleep"", ""SetSceneSceneTransitionOverride"", ""CallVendorRequest"", ""CreateSceneCollection"", ""SetStudioModeEnabled"", ""TriggerHotkeyByName"", ""OpenVideoMixProjector"", ""TriggerHotkeyByKeySequence"", ""GetPersistentData"", ""SetSceneItemIndex"", ""SetPersistentData"", ""SetCurrentSceneCollection"", ""SetInputMute"", ""SetCurrentPreviewScene"", ""SetCurrentProgramScene"", ""OpenSourceProjector"", ""GetProfileList"", ""SetCurrentProfile"", ""RemoveProfile"", ""CreateProfile"", ""GetProfileParameter"", ""SetProfileParameter"", ""GetInputPropertiesListPropertyItems"", ""GetInputAudioBalance"", ""GetStreamServiceSettings"", ""GetVideoSettings"", ""SetVideoSettings"", ""SetInputAudioBalance"", ""SetInputVolume"", ""SetStreamServiceSettings"", ""GetInputDefaultSettings"", ""GetSpecialInputs"", ""GetInputKindList"", ""GetRecordDirectory"", ""GetInputMute"", ""GetCurrentPreviewScene"", ""GetReplayBufferStatus"", ""GetSourceActive"", ""GetSourceScreenshot"", ""GetSourcePrivateSettings"", ""SetSourcePrivateSettings"", ""SetSourceFilterEnabled"", ""GetInputList"", ""GetSceneList"", ""GetGroupList"", ""SetInputSettings"", ""GetCurrentProgramScene"", ""GetSceneItemId"", ""RemoveScene"", ""CreateScene"", ""GetSceneSceneTransitionOverride"", ""RemoveInput"", ""CreateInput"", ""GetSceneItemLocked"", ""GetInputSettings"", ""ToggleInputMute"", ""SetCurrentSceneTransition"", ""GetInputVolume"", ""GetInputAudioSyncOffset"", ""GetInputAudioMonitorType"", ""SetInputAudioMonitorType"", ""StartVirtualCam"", ""GetInputAudioTracks"", ""SetInputAudioTracks"", ""GetTransitionKindList"", ""GetSceneItemTransform"", ""GetSceneTransitionList"", ""GetVirtualCamStatus"", ""GetCurrentSceneTransition"", ""SetCurrentSceneTransitionDuration"", ""SetCurrentSceneTransitionSettings"", ""GetCurrentSceneTransitionCursor"", ""SetTBarPosition"", ""StopOutput"", ""ToggleOutput"", ""GetSourceFilterList"", ""GetSourceFilterDefaultSettings"", ""CreateSourceFilter"", ""RemoveSourceFilter"", ""SetSourceFilterName"", ""GetSourceFilter"", ""StopRecord"", ""ToggleRecord"", ""SetSourceFilterIndex"", ""SetSourceFilterSettings"", ""SetSceneItemTransform"", ""GetSceneItemList"", ""GetGroupSceneItemList"", ""CreateSceneItem"", ""RemoveSceneItem"", ""DuplicateSceneItem"", ""GetSceneItemEnabled"", ""SetSceneItemEnabled"", ""SetSceneItemLocked"", ""GetSceneItemIndex"", ""StartReplayBuffer"", ""GetSceneItemBlendMode"", ""SetSceneItemBlendMode"", ""GetSceneItemPrivateSettings"", ""SetSceneItemPrivateSettings"", ""StopVirtualCam"", ""ToggleVirtualCam"", ""StopReplayBuffer"", ""ToggleReplayBuffer"", ""SaveReplayBuffer"", ""GetLastReplayBufferReplay"", ""GetOutputList"", ""GetOutputStatus"", ""StartOutput"", ""GetOutputSettings"", ""SetOutputSettings"", ""GetStreamStatus"", ""StopStream"", ""ToggleStream"", ""StartStream"", ""SendStreamCaption"", ""GetRecordStatus"", ""StartRecord"", ""ToggleRecordPause"", ""PauseRecord"", ""ResumeRecord"", ""SetMediaInputCursor"", ""GetMediaInputStatus"", ""OffsetMediaInputCursor"", ""TriggerMediaInputAction"", ""GetStudioModeEnabled"", ""OpenInputPropertiesDialog"", ""OpenInputFiltersDialog"", ""GetMonitorList""],
      ""obsVersion"": ""27.2.4"",
      ""obsWebSocketVersion"": ""5.0.1"",
      ""platform"": ""windows"",
      ""platformDescription"": ""Windows 11 Version 2009"",
      ""rpcVersion"": 1,
      ""supportedImageFormats"": [""bmp"", ""jpeg"", ""jpg"", ""pbm"", ""pgm"", ""png"", ""ppm"", ""xbm"", ""xpm""]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetStudioModeEnabled"",
    ""requestId"": ""{guid}"",
    ""requestData"":null
  }
}").ConfigureAwait(false);

      // In real, op follows d.
      await session.SendAsync(@"{
  ""d"": {
    ""requestType"": ""GetStudioModeEnabled"",
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""comment"": null,
      ""result"": true
    },
    ""responseData"": {
      ""studioModeEnabled"": false
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""SetStudioModeEnabled"",
    ""requestId"": ""{guid}"",
    ""requestData"": {
      ""studioModeEnabled"": true
    }
  }
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""op"": 5,
  ""d"": {
    ""eventType"": ""StudioModeStateChanged"",
    ""eventIntent"": 1,
    ""eventData"": {
      ""studioModeEnabled"": true,
    }
  }
}").ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetSpecialInputs"",
    ""requestId"": ""{guid}""
  }
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""requestType"": ""GetSpecialInputs"",
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""responseData"": {
       ""desktop1"": ""Desktop Audio"",
       ""desktop2"": null,
       ""mic1"": ""Microphone/Aux"",
       ""mic2"": null,
       ""mic3"": null,
       ""mic4"": null
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetInputSettings"",
    ""requestId"": ""{guid}"",
    ""requestData"": {
      ""inputName"": ""Desktop Audio""
    }
  }
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""requestType"": ""GetInputSettings"",
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""responseData"": {
      ""inputKind"": ""wasapi_output_capture"",
      ""inputSettings"": {
        ""device_id"": ""default"",
        ""use_device_timing"": true
      }
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetRecordDirectory"",
    ""requestId"": ""{guid}""
  }
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""GetRecordDirectory"",
    ""responseData"": {
      ""recordDirectory"": ""C:\\Users""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);


      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": [],
    ""requestId"": ""{guid}"",
    ""requestType"": ""StartRecord""
  },
  ""op"": 6
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""op"": 7,
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""StartRecord""
  }
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": false,
      ""outputPath"": null,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STARTING""
    },
    ""eventIntent"": 64,
    ""eventType"": ""RecordStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": true,
      ""outputPath"": ""C:\\Windows\\System32\\notepad.exe"",
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STARTED""
    },
    ""eventIntent"": 64,
    ""eventType"": ""RecordStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""StopRecord""
  }
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""StopRecord"",
    ""responseData"": {
      ""outputPath"": ""C:\\Windows\\System32\\notepad.exe""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": false,
      ""outputPath"": null,
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STOPPING""
    },
    ""eventIntent"": 64,
    ""eventType"": ""RecordStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""outputActive"": false,
      ""outputPath"": ""C:\\Windows\\System32\\notepad.exe"",
      ""outputState"": ""OBS_WEBSOCKET_OUTPUT_STOPPED""
    },
    ""eventIntent"": 64,
    ""eventType"": ""RecordStateChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);

      await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token).ConfigureAwait(false);
    }
  }
}
