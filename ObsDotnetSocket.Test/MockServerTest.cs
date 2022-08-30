using ObsDotnetSocket.DataTypes;
using ObsDotnetSocket.DataTypes.Predefineds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ObsDotnetSocket.Test {
  using ContextHandler = Func<HttpListenerContext, CancellationToken, Task>;

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
    public async Task TestTrollUserAsync() {
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
      var cancellation = new CancellationTokenSource();
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

    [Fact]
    public async Task TestParallelAsync() {
      var cancellation = new CancellationTokenSource();
      var server = RunMockServer(cancellation.Token, ServeEchoAsync);
      var tasks = new List<Task>();
      try {
        var client = new ObsClientSocket();
        await client.ConnectAsync(_mockServerUri, Password, cancellation: cancellation.Token).ConfigureAwait(false);

        for (int i = 0; i < 30; i++) {
          tasks.Add((i % 2) switch {
            0 => client.GetStudioModeEnabledAsync(cancellation.Token),
            _ => client.GetVersionAsync(cancellation.Token),
          });
        }
        await Task.WhenAll(tasks).ConfigureAwait(false);
        Assert.All(tasks, t => Assert.True(t.IsCompletedSuccessfully));
      }
      catch (Exception ex) {
        Debug.WriteLine(ex);
      }
      finally {
        cancellation.Cancel();
        server.Close();
      }
    }

    [Fact]
    public async Task TestTrollServerAsync() {
      var cancellation = new CancellationTokenSource();
      var server = RunMockServer(cancellation.Token, ServeTrollAsync);
      try {
        var socket = new ClientSocket {
          SetOptions = (sock) => {
            sock.Options.KeepAliveInterval = TimeSpan.FromMilliseconds(1000);
          }
        };
        var client = new ObsClientSocket(client: socket);
        await client.ConnectAsync(_mockServerUri, Password, cancellation: cancellation.Token).ConfigureAwait(false);

        var recordTasks = new List<Task<GetRecordDirectoryResponse>>();
        for (int i = 0; i < 30; i++) {
          recordTasks.Add(client.GetRecordDirectoryAsync(cancellation.Token));
        }

        try {
          await Task.WhenAll(recordTasks).ConfigureAwait(false);
          Assert.Fail("exception not fired");
        }
        catch (QueueCancelledException) {
          Debug.WriteLine("Huh?");
        }
      }
      finally {
        cancellation.Cancel();
        server.Close();
      }
    }

    private static HttpListener RunMockServer(CancellationToken token, ContextHandler? handler = null) {
      token.ThrowIfCancellationRequested();

      var httpListener = new HttpListener();
      httpListener.Prefixes.Add("http://127.0.0.1:44550/");
      httpListener.Start();

      _ = Task.Run(() => ServeForeverAsync(httpListener, handler ?? ServeAsync, token), token);
      return httpListener;
    }

    private static async Task ServeForeverAsync(HttpListener http, ContextHandler action, CancellationToken token) {
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

    private static async Task ServeTrollAsync(HttpListenerContext context, CancellationToken token) {
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

    private static async Task ServeEchoAsync(HttpListenerContext context, CancellationToken token) {
      var (_, session) = await HandshakeAsync(context, token).ConfigureAwait(false);
      using var _1 = session;

      for (int i = 0; i < 30; i++) {
        string json = await session.ReceiveAsync().ConfigureAwait(false);
        string guid = Regex.Match(json, @"[0-9a-f]{8}-[0-9a-f]{4}[^""]*").Value;
        if (json.Contains(@"""requestType"":""GetVersion"",")) {
          await session.SendGetVersionResponseAsync(guid).ConfigureAwait(false);
        }
        else if (json.Contains(@"""requestType"":""GetStudioModeEnabled"",")) {
          await session.SendGetStudioModeEnabledResponseAsync(guid).ConfigureAwait(false);
        }
        else {
          throw new Exception("Unexpected request");
        }
      }

      Debug.WriteLine("Served all messages");
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
