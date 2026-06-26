using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class ConnectionStateTest {
    [Fact]
    public async Task LowLevelClientReportsConnectAndCloseStateTransitions() {
      using var server = new MockServer().Run(
        AcceptAndWaitForClientCloseAsync,
        TestContext.Current.CancellationToken);
      using var client = new ClientSocket();
      var transitions = new ConcurrentQueue<ObsConnectionStateChangedEventArgs>();
      client.ConnectionStateChanged += (_, e) => transitions.Enqueue(e);

      Assert.Equal(ObsConnectionPhase.Disconnected, client.ConnectionState.Phase);

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));
      await client.CloseAsync();

      var status = client.ConnectionState;
      Assert.Equal(ObsConnectionPhase.Disconnected, status.Phase);
      Assert.Equal(server.Uri, status.Uri);
      Assert.Null(status.Exception);
      Assert.Equal(status, client.ConnectionState);
      Assert.Equal(
        [
          ObsConnectionPhase.Connecting,
          ObsConnectionPhase.Connected,
          ObsConnectionPhase.Closing,
          ObsConnectionPhase.Disconnected,
        ],
        transitions.Select(e => e.NewState.Phase).ToArray());
    }

    [Fact]
    public async Task HighLevelClientReportsStateWhenUsingEventChannel() {
      using var server = new MockServer().Run(
        AcceptAndWaitForClientCloseAsync,
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient(useChannel: true);
      var connected = new TaskCompletionSource<ObsConnectionStateChangedEventArgs>(
        TaskCreationOptions.RunContinuationsAsynchronously);
      client.ConnectionStateChanged += (_, e) => {
        if (e.NewState.Phase == ObsConnectionPhase.Connected) {
          _ = connected.TrySetResult(e);
        }
      };

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));
      var transition = await connected.Task.WaitAsync(TestContext.Current.CancellationToken);

      Assert.Equal(ObsConnectionPhase.Connecting, transition.OldState.Phase);
      Assert.Equal(ObsConnectionPhase.Connected, transition.NewState.Phase);
      Assert.Equal(server.Uri, transition.NewState.Uri);
      Assert.Equal(ObsConnectionPhase.Connected, client.ConnectionState.Phase);
      await client.CloseAsync();
    }

    private static async Task AcceptAndWaitForClientCloseAsync(
      HttpListenerContext context,
      CancellationToken cancellation
    ) {
      var (webSocketContext, session) = await MockServer.HandshakeAsync(
        context,
        cancellation).ConfigureAwait(false);
      using var _ = session;
      byte[] buffer = new byte[1];
      var result = await webSocketContext.WebSocket.ReceiveAsync(
        new ArraySegment<byte>(buffer),
        cancellation).ConfigureAwait(false);

      if (result.MessageType == WebSocketMessageType.Close) {
        await MockServer.CloseQuietlyAsync(
          webSocketContext.WebSocket,
          cancellation).ConfigureAwait(false);
      }
    }
  }
}
