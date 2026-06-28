using ObsStrawket.Test.Utilities;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class AutoReconnectTest {
    [Fact]
    public async Task HighLevelClientDispatchesEventsAfterAutomaticReconnect() {
      var closeFirstConnection = new TaskCompletionSource(
        TaskCreationOptions.RunContinuationsAsynchronously);
      var secondConnectionAccepted = new TaskCompletionSource(
        TaskCreationOptions.RunContinuationsAsynchronously);
      var receivedReconnectedEvent = new TaskCompletionSource(
        TaskCreationOptions.RunContinuationsAsynchronously);
      var reconnectingObserved = new TaskCompletionSource(
        TaskCreationOptions.RunContinuationsAsynchronously);
      int acceptedConnections = 0;

      using var server = new MockServer().Run(
        async (context, cancellation) => {
          int connectionIndex = Interlocked.Increment(ref acceptedConnections);
          var (webSocketContext, session) = await MockServer.HandshakeAsync(
            context,
            cancellation).ConfigureAwait(false);
          using var sessionScope = session;

          if (connectionIndex == 1) {
            await closeFirstConnection.Task.WaitAsync(cancellation).ConfigureAwait(false);
            await MockServer.CloseQuietlyAsync(
              webSocketContext.WebSocket,
              cancellation).ConfigureAwait(false);
            return;
          }

          _ = secondConnectionAccepted.TrySetResult();
          await session.SendStudioModeStateChangedAsync(true).ConfigureAwait(false);
          await Task.Delay(Timeout.InfiniteTimeSpan, cancellation).ConfigureAwait(false);
        },
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient();
      client.ReconnectOptions.Enabled = true;
      client.ReconnectOptions.InitialDelay = TimeSpan.Zero;
      client.ReconnectOptions.MaxDelay = TimeSpan.Zero;
      client.ConnectionStateChanged += (_, e) => {
        if (e.NewState.Phase == ObsConnectionPhase.Faulted && client.IsReconnecting) {
          _ = reconnectingObserved.TrySetResult();
        }
      };
      client.StudioModeStateChanged += _ => receivedReconnectedEvent.TrySetResult();

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));
      closeFirstConnection.SetResult();

      await reconnectingObserved.Task.WaitAsync(TestContext.Current.CancellationToken);
      await secondConnectionAccepted.Task.WaitAsync(TestContext.Current.CancellationToken);
      await receivedReconnectedEvent.Task.WaitAsync(TestContext.Current.CancellationToken);

      Assert.Equal(2, Volatile.Read(ref acceptedConnections));
      Assert.False(client.IsReconnecting);
      Assert.Equal(ObsConnectionPhase.Connected, client.ConnectionState.Phase);
    }

    [Fact]
    public async Task AutomaticReconnectIgnoresSessionInvalidatedClose() {
      var maybeSecondConnectionAccepted = new TaskCompletionSource(
        TaskCreationOptions.RunContinuationsAsynchronously);
      int acceptedConnections = 0;

      using var server = new MockServer().Run(
        async (context, cancellation) => {
          int connectionIndex = Interlocked.Increment(ref acceptedConnections);
          var (webSocketContext, session) = await MockServer.HandshakeAsync(
            context,
            cancellation).ConfigureAwait(false);
          using var sessionScope = session;

          if (connectionIndex > 1) {
            _ = maybeSecondConnectionAccepted.TrySetResult();
          }

          await webSocketContext.WebSocket.CloseAsync(
            (WebSocketCloseStatus)4011,
            "Session invalidated",
            cancellation).ConfigureAwait(false);
        },
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient();
      client.ReconnectOptions.Enabled = true;
      client.ReconnectOptions.InitialDelay = TimeSpan.Zero;
      client.ReconnectOptions.MaxDelay = TimeSpan.Zero;

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      _ = await Assert.ThrowsAsync<TimeoutException>(
        () => maybeSecondConnectionAccepted.Task.WaitAsync(
          TimeSpan.FromMilliseconds(200),
          TestContext.Current.CancellationToken));

      Assert.Equal(1, Volatile.Read(ref acceptedConnections));
      Assert.Equal(ObsConnectionPhase.Faulted, client.ConnectionState.Phase);
      Assert.False(client.IsReconnecting);
      var exception = Assert.IsType<ObsConnectionClosedException>(
        client.ConnectionState.Exception);
      Assert.Equal(4011, exception.CloseCode);
    }

    [Fact]
    public async Task ManualCloseStopsAutomaticReconnect() {
      var maybeSecondConnectionAccepted = new TaskCompletionSource(
        TaskCreationOptions.RunContinuationsAsynchronously);
      int acceptedConnections = 0;

      using var server = new MockServer().Run(
        async (context, cancellation) => {
          int connectionIndex = Interlocked.Increment(ref acceptedConnections);
          var (webSocketContext, session) = await MockServer.HandshakeAsync(
            context,
            cancellation).ConfigureAwait(false);
          using var sessionScope = session;

          if (connectionIndex > 1) {
            _ = maybeSecondConnectionAccepted.TrySetResult();
          }

          await WaitForClientCloseAsync(webSocketContext, cancellation).ConfigureAwait(false);
        },
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient();
      client.ReconnectOptions.Enabled = true;
      client.ReconnectOptions.InitialDelay = TimeSpan.Zero;
      client.ReconnectOptions.MaxDelay = TimeSpan.Zero;

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));
      await client.CloseAsync();

      _ = await Assert.ThrowsAsync<TimeoutException>(
        () => maybeSecondConnectionAccepted.Task.WaitAsync(
          TimeSpan.FromMilliseconds(200),
          TestContext.Current.CancellationToken));

      Assert.Equal(1, Volatile.Read(ref acceptedConnections));
      Assert.Equal(ObsConnectionPhase.Disconnected, client.ConnectionState.Phase);
      Assert.False(client.IsReconnecting);
    }

    [Fact]
    public async Task FailedExplicitConnectDoesNotReconnectToPreviousServer() {
      var maybeSecondConnectionAccepted = new TaskCompletionSource(
        TaskCreationOptions.RunContinuationsAsynchronously);
      int acceptedConnections = 0;

      using var server = new MockServer().Run(
        async (context, cancellation) => {
          int connectionIndex = Interlocked.Increment(ref acceptedConnections);
          var (webSocketContext, session) = await MockServer.HandshakeAsync(
            context,
            cancellation).ConfigureAwait(false);
          using var sessionScope = session;

          if (connectionIndex > 1) {
            _ = maybeSecondConnectionAccepted.TrySetResult();
          }

          await WaitForClientCloseAsync(webSocketContext, cancellation).ConfigureAwait(false);
        },
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient();
      client.ReconnectOptions.Enabled = true;
      client.ReconnectOptions.InitialDelay = TimeSpan.Zero;
      client.ReconnectOptions.MaxDelay = TimeSpan.Zero;

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      var unavailable = new Uri("ws://127.0.0.1:1/");
      _ = await Assert.ThrowsAsync<ObsConnectionException>(
        () => client.ConnectAsync(
          unavailable,
          MockServer.Password,
          cancellation: TestContext.Current.CancellationToken));

      _ = await Assert.ThrowsAsync<TimeoutException>(
        () => maybeSecondConnectionAccepted.Task.WaitAsync(
          TimeSpan.FromMilliseconds(200),
          TestContext.Current.CancellationToken));

      Assert.Equal(1, Volatile.Read(ref acceptedConnections));
      Assert.False(client.IsReconnecting);
      Assert.Equal(ObsConnectionPhase.Faulted, client.ConnectionState.Phase);
      Assert.Equal(unavailable, client.ConnectionState.Uri);
    }

    private static async Task WaitForClientCloseAsync(
      WebSocketContext context,
      CancellationToken cancellation
    ) {
      byte[] buffer = new byte[1];
      var result = await context.WebSocket.ReceiveAsync(
        new ArraySegment<byte>(buffer),
        cancellation).ConfigureAwait(false);
      if (result.MessageType == WebSocketMessageType.Close) {
        await MockServer.CloseQuietlyAsync(context.WebSocket, cancellation).ConfigureAwait(false);
      }
    }
  }
}
