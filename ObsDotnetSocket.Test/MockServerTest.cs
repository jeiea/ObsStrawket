namespace ObsDotnetSocket.Test {
  using System;
  using System.Net;
  using System.Net.WebSockets;
  using System.Threading;
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
        var tasks = new[] {
          RunMockServerAsync(cancellation.Token),
          new CommonFlow().RunClientAsync(_mockServerUri, cancellation: cancellation.Token),
        };
        await await Task.WhenAny(tasks).ConfigureAwait(false);
        await Task.WhenAll(tasks).ConfigureAwait(false);
      }
      catch {
        cancellation.Cancel();
        throw;
      }
    }

    [Fact]
    public async Task TestReconnectivityAsync() {
      var cancellation = new CancellationTokenSource();
      try {
        var server = RunMockServerAsync(cancellation.Token);
        var client = new ObsClientSocket();
        try {
          await client.ConnectAsync(new Uri("ws://localhost:55595"), Password).ConfigureAwait(false);
          Assert.Fail("Connecting to existent endpoint but no exception thrown");
        }
        catch (Exception ex) {
          // expected
        }

        await client.ConnectAsync(_mockServerUri, Password).ConfigureAwait(false);
      }
      catch {
        cancellation.Cancel();
        throw;
      }
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

      using var _ = httpListener;
      var context = await httpListener.GetContextAsync().ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      Assert.True(context.Request.IsWebSocketRequest);
      string subProtocol = "obswebsocket.msgpack";
      Assert.Equal(subProtocol, context.Request.Headers["Sec-WebSocket-Protocol"]);

      var webSocketContext = await context.AcceptWebSocketAsync(subProtocol).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      using var session = new MockServerSession(webSocketContext.WebSocket, token);
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

      string? guid = await session.ReceiveAsync(@"{
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

      token.ThrowIfCancellationRequested();
      httpListener.Close();
    }
  }
}
