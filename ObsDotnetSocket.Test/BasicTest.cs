namespace ObsDotnetSocket.Test {
  using MessagePack;
  using Newtonsoft.Json.Linq;
  using ObsDotnetSocket.DataTypes;
  using System.Net;
  using System.Net.WebSockets;
  using Xunit;

  public class BasicTest {
    [Fact]
    public async Task TestHelloAsync() {
      var cancellation = new CancellationTokenSource();
      try {
        _ = RunServerAsync(cancellation.Token);
        var connection = await Connection.ConnectAsync(
          new Uri("ws://127.0.0.1:44550"),
          password: "ahrEYXzXKytCIlpI"
        ).ConfigureAwait(false);
        var result = await connection.ReceiveAsync<IOpcodeMessage>().ConfigureAwait(false);
        Console.WriteLine(result);
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

      await socket.SendAsync(MessagePackSerializer.ConvertFromJson(@"{
  ""op"": 5,
  ""d"": {
    ""eventType"": ""RecordStateChanged"",
    ""eventIntent"": 1,
    ""eventData"": {
      ""studioModeEnabled"": true
    }
  }
}", cancellationToken: token), binary, true, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      await socket.ReceiveAsync(buffer, token).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();
      json = MessagePackSerializer.ConvertToJson(buffer, cancellationToken: token);
      TestUtil.AssertJsonEqual(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""SetCurrentProgramScene"",
    ""requestId"": ""f819dcf0-89cc-11eb-8f0e-382c4ac93b9c"",
    ""requestData"": {
      ""sceneName"": ""Scene 12""
    }
  }
}", json);
    }
  }
}
