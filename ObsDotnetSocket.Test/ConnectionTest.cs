namespace ObsDotnetSocket.Test {
  using MessagePack;
  using ObsDotnetSocket.DataTypes;
  using ObsDotnetSocket.DataTypes.Predefineds;
  using ObsDotnetSocket.Serialization;
  using System.Net;
  using System.Net.WebSockets;
  using System.Reflection;
  using System.Text.RegularExpressions;
  using System.Threading.Channels;
  using Xunit;
  using Xunit.Abstractions;

  public class ConnectionTest {
    private readonly ITestOutputHelper _output;

    public ConnectionTest(ITestOutputHelper output) {
      _output = output;
    }

    [Fact]
    public async Task TestAgainstObsAsync() {
      await RunClientAsync(new Uri("ws://127.0.0.1:4455"), CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task JustMonitorObsEventAsync() {
      var methods = typeof(MessagePackSerializer).GetMethods().ToList();
      methods = methods.Where(x => x.Name == nameof(MessagePackSerializer.SerializeToJson)).ToList();
      var method = methods[1];
      var client = new ObsClientSocket();
      var source = new TaskCompletionSource();
      client.Event += (@event) => {
        var result = method.MakeGenericMethod(@event.GetType()).Invoke(@event, new object[] { @event, MessagePackSerializerOptions.Standard.WithResolver(OpCodeMessageResolver.Instance), default(CancellationToken) });
        _output.WriteLine($"{result}");
      };
      client.Closed += () => {
        source.SetResult();
      };
      try {
        await client.ConnectAsync(new Uri("ws://127.0.0.1:4455"), "ahrEYXzXKytCIlpI").ConfigureAwait(false);
      }
      catch (Exception ex) {
        source.TrySetException(ex);
      }
      await source.Task.ConfigureAwait(false);
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
      var client = new ObsClientSocket();
      await client.ConnectAsync(uri, "ahrEYXzXKytCIlpI", cancellation: token).ConfigureAwait(false);
      var events = Channel.CreateUnbounded<IEvent>();
      var source = new TaskCompletionSource<IEvent>();
      client.Event += (@event) => {
        _ = events.Writer.WriteAsync(@event);
      };
      client.StudioModeStateChanged += (@event) => {
        _ = events.Writer.WriteAsync(@event);
      };

      var response = await client.RequestAsync(new RawRequest() {
        RequestId = "2521a51c-7040-4830-8181-492ab5477545",
        RequestType = "GetStudioModeEnabled"
      }, cancellation: token).ConfigureAwait(false);
      if (response is not GetStudioModeEnabledResponse studioMode
          || studioMode.RequestStatus.Code != RequestStatusCode.Success) {
        Assert.Fail("Did not parse the request");
        throw new Exception();
      }

      await client.SetStudioModeEnabledAsync(!studioMode.StudioModeEnabled).ConfigureAwait(false);

      var @event = await events.Reader.ReadAsync(token).ConfigureAwait(false);
      if (@event is not StudioModeStateChanged studio) {
        Assert.Fail("Type not converted");
        throw new Exception();
      }
      @event = await events.Reader.ReadAsync(token).ConfigureAwait(false);
      if (@event is not StudioModeStateChanged) {
        Assert.Fail("Type not converted");
        throw new Exception();
      }
      Assert.Equal(!studioMode.StudioModeEnabled, studio.StudioModeEnabled);
      Assert.Equal(0, events.Reader.Count);

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
    ""requestType"": ""GetStudioModeEnabled"",
    ""requestId"": ""{guid}"",
    ""requestData"":null
  }
}".Replace("{guid}", guid), json);

      // In real, op follows d.
      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
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
}".Replace("{guid}", guid), cancellationToken: token), binary, true, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      await socket.ReceiveAsync(buffer, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();
      json = MessagePackSerializer.ConvertToJson(buffer, cancellationToken: token);
      guid = Regex.Match(json, @"[0-9a-f]{8}-[0-9a-f]{4}[^""]*").Value;
      TestUtil.AssertJsonEqual(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""SetStudioModeEnabled"",
    ""requestId"": ""{guid}"",
    ""requestData"": {
      ""studioModeEnabled"": true
    }
  }
}".Replace("{guid}", guid), json);

      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
        ""op"": 5,
        ""d"": {
          ""eventType"": ""StudioModeStateChanged"",
          ""eventIntent"": 1,
          ""eventData"": {
            ""studioModeEnabled"": true,
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
