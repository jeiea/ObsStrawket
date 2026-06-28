using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class ChannelApiTest {
    [Fact]
    public async Task HighLevelClientPublishesEventsToChannelAndHandlers() {
      var handlerEvent = new TaskCompletionSource<StudioModeStateChanged>(
        TaskCreationOptions.RunContinuationsAsynchronously);

      using var server = new MockServer().Run(
        SendStudioModeEventAndWaitForCloseAsync,
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient();
      var events = client.Events;
      client.StudioModeStateChanged += ev => handlerEvent.TrySetResult(ev);

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      var channelEvent = Assert.IsType<StudioModeStateChanged>(
        await events.ReadAsync(TestContext.Current.CancellationToken));
      var dispatchedEvent = await handlerEvent.Task.WaitAsync(TestContext.Current.CancellationToken);

      Assert.True(channelEvent.StudioModeEnabled);
      Assert.Same(channelEvent, dispatchedEvent);
    }

    [Fact]
    public async Task CloseAsyncCompletesCurrentChannelsAndReconnectUsesNewChannels() {
      int acceptedConnections = 0;
      using var server = new MockServer().Run(
        async (context, cancellation) => {
          _ = Interlocked.Increment(ref acceptedConnections);
          await SendStudioModeEventAndWaitForCloseAsync(context, cancellation).ConfigureAwait(false);
        },
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient();

      var oldEvents = client.Events;
      var oldConnectionStates = client.ConnectionStates;

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken).WaitAsync(
          TimeSpan.FromSeconds(5),
          TestContext.Current.CancellationToken));
      _ = await oldEvents.ReadAsync(TestContext.Current.CancellationToken).AsTask().WaitAsync(
        TimeSpan.FromSeconds(5),
        TestContext.Current.CancellationToken);
      await client.CloseAsync().WaitAsync(
        TimeSpan.FromSeconds(5),
        TestContext.Current.CancellationToken);

      await oldEvents.Completion.WaitAsync(TestContext.Current.CancellationToken);
      await DrainUntilCompletedAsync(
        oldConnectionStates,
        TestContext.Current.CancellationToken);

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));
      var newEvents = client.Events;
      var newConnectionStates = client.ConnectionStates;

      Assert.NotSame(oldEvents, newEvents);
      Assert.NotSame(oldConnectionStates, newConnectionStates);

      _ = Assert.IsType<StudioModeStateChanged>(
        await newEvents.ReadAsync(TestContext.Current.CancellationToken));

      Assert.Equal(2, Volatile.Read(ref acceptedConnections));
    }

    [Fact]
    public async Task AutomaticReconnectKeepsExistingChannelsOpen() {
      var closeFirstConnection = new TaskCompletionSource(
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

          await session.SendStudioModeStateChangedAsync(true).ConfigureAwait(false);
          await WaitForClientCloseAsync(webSocketContext, cancellation).ConfigureAwait(false);
        },
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient();
      client.ReconnectOptions.Enabled = true;
      client.ReconnectOptions.InitialDelay = TimeSpan.Zero;
      client.ReconnectOptions.MaxDelay = TimeSpan.Zero;
      var events = client.Events;
      var connectionStates = client.ConnectionStates;

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));
      closeFirstConnection.SetResult();

      _ = Assert.IsType<StudioModeStateChanged>(
        await events.ReadAsync(TestContext.Current.CancellationToken));
      var connectedAfterFault = await ReadConnectedAfterFaultAsync(
        connectionStates,
        TestContext.Current.CancellationToken);

      Assert.Equal(2, Volatile.Read(ref acceptedConnections));
      Assert.Equal(server.Uri, connectedAfterFault.NewState.Uri);
      Assert.False(events.Completion.IsCompleted);
      Assert.False(connectionStates.Completion.IsCompleted);
    }

    private static async Task<ObsConnectionStateChangedEventArgs> ReadConnectedAfterFaultAsync(
      ChannelReader<ObsConnectionStateChangedEventArgs> states,
      CancellationToken cancellation
    ) {
      bool sawFaulted = false;
      while (await states.WaitToReadAsync(cancellation).ConfigureAwait(false)) {
        while (states.TryRead(out var state)) {
          if (state.NewState.Phase == ObsConnectionPhase.Faulted) {
            sawFaulted = true;
          }
          if (sawFaulted && state.NewState.Phase == ObsConnectionPhase.Connected) {
            return state;
          }
        }
      }
      throw new ChannelClosedException("Connection state channel closed before reconnect.");
    }

    private static async Task DrainUntilCompletedAsync<T>(
      ChannelReader<T> reader,
      CancellationToken cancellation
    ) {
      while (await reader.WaitToReadAsync(cancellation).ConfigureAwait(false)) {
        while (reader.TryRead(out _)) { }
      }
    }

    private static async Task SendStudioModeEventAndWaitForCloseAsync(
      HttpListenerContext context,
      CancellationToken cancellation
    ) {
      var (webSocketContext, session) = await MockServer.HandshakeAsync(
        context,
        cancellation).ConfigureAwait(false);
      using var sessionScope = session;

      await session.SendStudioModeStateChangedAsync(true).ConfigureAwait(false);
      await WaitForClientCloseAsync(webSocketContext, cancellation).ConfigureAwait(false);
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
