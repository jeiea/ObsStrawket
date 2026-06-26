using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {

  public class AuthenticationFailureTest {

    [Fact]
    public async Task ConnectAsyncReturnsFalseWhenAuthenticationFails() {
      using var server = new MockServer().Run(AlwaysRejectAuth, TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient(useChannel: true);

      bool connected = await client.ConnectAsync(
        server.Uri,
        "a",
        cancellation: TestContext.Current.CancellationToken
      );

      Assert.False(connected);
      Assert.False(client.IsConnected);
      Assert.Equal(ObsConnectionPhase.Faulted, client.ConnectionState.Phase);
      _ = Assert.IsType<ObsAuthenticationException>(client.ConnectionState.Exception);
    }

    [Fact]
    public async Task LowLevelConnectAsyncReturnsFalseWhenAuthenticationFails() {
      using var server = new MockServer().Run(AlwaysRejectAuth, TestContext.Current.CancellationToken);
      using var client = new ClientSocket();

      bool connected = await client.ConnectAsync(
        server.Uri,
        "a",
        cancellation: TestContext.Current.CancellationToken
      );

      Assert.False(connected);
      Assert.False(client.IsConnected);
      Assert.Equal(ObsConnectionPhase.Faulted, client.ConnectionState.Phase);
      _ = Assert.IsType<ObsAuthenticationException>(client.ConnectionState.Exception);
    }

    private static async Task AlwaysRejectAuth(HttpListenerContext context, CancellationToken token) {
      token.ThrowIfCancellationRequested();

      Assert.True(context.Request.IsWebSocketRequest);
      string subProtocol = "obswebsocket.json";
      Assert.Equal(subProtocol, context.Request.Headers["Sec-WebSocket-Protocol"]);

      var webSocketContext = await context.AcceptWebSocketAsync(subProtocol).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();

      var session = new MockServerSession(webSocketContext.WebSocket, token);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 0,
  "d": {
    "obsWebSocketVersion": "5.0.1",
    "rpcVersion": 1,
    "authentication": {
      "challenge": "+IxH4CnCiqpX1rM9scsNynZzbOe4KhDeYcTNS3PDaeY=",
      "salt": "lM1GncleQOaCu9lT1yeUZhFYnqhsLLP1G5lAGo3ixaI="
    }
  }
}
""").ConfigureAwait(false);
      _ = await session.ReceiveAsync();

      await webSocketContext.WebSocket.CloseAsync(
        (WebSocketCloseStatus)WebSocketCloseCode.AuthenticationFailed,
        "Authentication failure",
        default
      ).ConfigureAwait(false);
    }
  }
}
