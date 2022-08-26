namespace ObsDotnetSocket.Test {
  using MessagePack;
  using ObsDotnetSocket.DataTypes;
  using ObsDotnetSocket.DataTypes.Predefineds;
  using ObsDotnetSocket.Serialization;
  using ObsDotnetSocket.WebSocket4Net;
  using System;
  using System.Linq;
  using System.Net;
  using System.Net.WebSockets;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Channels;
  using System.Threading.Tasks;
  using Xunit;
  using Xunit.Abstractions;

  public class MockServerTest {
    private readonly ITestOutputHelper _output;

    public MockServerTest(ITestOutputHelper output) {
      _output = output;
    }

    [Fact]
    public async Task TestSystemNetSocketsAsync() {
      var cancellation = new CancellationTokenSource();
      try {
        var tasks = new[] {
          RunMockServerAsync(cancellation.Token),
          new CommonFlow().RunClientAsync(new Uri("ws://127.0.0.1:44550"), cancellation: cancellation.Token),
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
    public async Task TestWebSocket4NetAsync() {
      var cancellation = new CancellationTokenSource();
      try {
        var tasks = new[] {
          RunMockServerAsync(cancellation.Token),
          new CommonFlow().RunClientAsync(new Uri("ws://127.0.0.1:44550"), new WebSocket4NetSocket(), cancellation: cancellation.Token),
        };
        await await Task.WhenAny(tasks).ConfigureAwait(false);
        await Task.WhenAll(tasks).ConfigureAwait(false);
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

      var buffer = new ArraySegment<byte>(new byte[1024]);

      var context = await httpListener.GetContextAsync().ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      Assert.True(context.Request.IsWebSocketRequest);
      string subProtocol = "obswebsocket.msgpack";
      Assert.Equal(subProtocol, context.Request.Headers["Sec-WebSocket-Protocol"]);

      var webSocketContext = await context.AcceptWebSocketAsync(subProtocol).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      using var socket = webSocketContext.WebSocket;
      var binary = WebSocketMessageType.Binary;
      await socket.SendAsync(new ArraySegment<byte>(MessagePackSerializer.ConvertFromJson(@"{
  ""op"": 0,
  ""d"": {
    ""obsWebSocketVersion"": ""5.0.1"",
    ""rpcVersion"": 1,
    ""authentication"": {
      ""challenge"": ""+IxH4CnCiqpX1rM9scsNynZzbOe4KhDeYcTNS3PDaeY="",
      ""salt"": ""lM1GncleQOaCu9lT1yeUZhFYnqhsLLP1G5lAGo3ixaI=""
    }
  }
}", cancellationToken: token)), binary, true, token).ConfigureAwait(false);
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

      await socket.SendAsync(new ArraySegment<byte>(MessagePackSerializer.ConvertFromJson(@"{
  ""op"": 2,
  ""d"": {
    ""negotiatedRpcVersion"": 1
  }
}", cancellationToken: token)), binary, true, token).ConfigureAwait(false);
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
      await socket.SendAsync(new ArraySegment<byte>(MessagePackSerializer.ConvertFromJson(@"{
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
}".Replace("{guid}", guid), cancellationToken: token)), binary, true, token).ConfigureAwait(false);
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

      await socket.SendAsync(new ArraySegment<byte>(MessagePackSerializer.ConvertFromJson(@"{
  ""op"": 5,
  ""d"": {
    ""eventType"": ""StudioModeStateChanged"",
    ""eventIntent"": 1,
    ""eventData"": {
      ""studioModeEnabled"": true,
    }
  }
}", cancellationToken: token)), binary, true, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      httpListener.Close();
    }
  }
}
