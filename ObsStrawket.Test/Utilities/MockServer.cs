using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Utilities {
  using ContextHandler = Func<HttpListenerContext, CancellationToken, Task>;

  class MockServer : IDisposable {
    public static readonly string Password = "ahrEYXzXKytCIlpI";

    public int Port { get; private set; }
    public Uri Uri { get => new($"ws://127.0.0.1:{Port}/"); }

    private HttpListener? _httpListener;
    private bool _isDisposed;

    public MockServer Run(CancellationToken token, ContextHandler? handler = null) {
      token.ThrowIfCancellationRequested();

      var random = new Random();

      do {
        try {
          Port = random.Next(1024, ushort.MaxValue);
          _httpListener = new HttpListener();
          _httpListener.Prefixes.Add($"http://127.0.0.1:{Port}/");
          _httpListener.Start();
        }
        catch (HttpListenerException ex) {
          // Failed to listen on prefix '' because it conflicts with an existing registration on the machine.
          if (ex.ErrorCode == 0xb7) {
            // retry
          }
        }
      } while (_httpListener?.IsListening == false);

      _ = Task.Run(() => ServeForeverAsync(handler ?? ServeAsync, token), token);
      return this;
    }

    public void Abort() {
      _httpListener!.Abort();
    }

    public static async Task<(WebSocketContext, MockServerSession)> HandshakeAsync(HttpListenerContext context, CancellationToken token) {
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

    public static async Task ServeTrollAsync(HttpListenerContext context, CancellationToken token) {
      var (webSocketContext, session) = await HandshakeAsync(context, token).ConfigureAwait(false);
      using var _1 = session;

      var guids = new List<string>();
      for (int i = 0; i < 20; i++) {
        string? guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetRecordDirectory"",
    ""requestId"": ""{guid}""
  }
}").ConfigureAwait(false);
        guids.Add(guid!);
      }

      foreach (string guid in guids.Take(10)) {
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
      }

      byte[] buffer = new byte[] { 0x01, 0x02, 0x03, 0x04 };
      await webSocketContext.WebSocket.SendAsync(
        new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, token
      ).ConfigureAwait(false);
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

      await session.SendGetVersionResponseAsync(guid!).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetStudioModeEnabled"",
    ""requestId"": ""{guid}"",
    ""requestData"":null
  }
}").ConfigureAwait(false);

      await session.SendGetStudioModeEnabledResponseAsync(guid!).ConfigureAwait(false);

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
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetStats"",
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
    ""requestType"": ""GetStats"",
    ""responseData"": {
      ""activeFps"": 60.0000024000001,
      ""availableDiskSpace"": 198612.01953125,
      ""averageFrameRenderTime"": 0.534547,
      ""cpuUsage"": 11.508459378338541,
      ""memoryUsage"": 948.94140625,
      ""outputSkippedFrames"": 0,
      ""outputTotalFrames"": 268,
      ""renderSkippedFrames"": 46,
      ""renderTotalFrames"": 558618,
      ""webSocketSessionIncomingMessages"": 13,
      ""webSocketSessionOutgoingMessages"": 13
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
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

    public void Dispose() {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          _httpListener?.Close();
        }

        _isDisposed = true;
      }
    }

    private async Task ServeForeverAsync(ContextHandler action, CancellationToken token) {
      while (!token.IsCancellationRequested && _httpListener!.IsListening) {
        var context = await _httpListener.GetContextAsync().ConfigureAwait(false);
        _ = action(context, token);
      }
    }

  }
}
