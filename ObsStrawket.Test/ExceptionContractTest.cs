using ObsStrawket.Test.Utilities;
using System;
using System.Net;
using System.Threading.Channels;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class ExceptionContractTest {
    [Fact]
    public async Task ProtocolFailureIsSharedByRequestAndDisconnectedEvent() {
      var disconnected = new TaskCompletionSource<Exception?>(
        TaskCreationOptions.RunContinuationsAsynchronously);
      using var server = new MockServer().Run(
        SendMalformedResponseAsync,
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient();
      client.Disconnected += exception => disconnected.TrySetResult(exception);
      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      var requestException = await Assert.ThrowsAsync<ObsProtocolException>(
        () => client.GetRecordStatusAsync(TestContext.Current.CancellationToken));
      var disconnectedException = await disconnected.Task.WaitAsync(
        TestContext.Current.CancellationToken);

      Assert.Same(requestException, disconnectedException);
      _ = Assert.IsType<JsonException>(requestException.InnerException);
    }

    [Fact]
    public async Task ConnectAsyncThrowsForNonAuthenticationFailure() {
      using var client = new ClientSocket();
      var unavailable = new Uri("ws://127.0.0.1:1/");

      var exception = await Assert.ThrowsAsync<ObsConnectionException>(
        () => client.ConnectAsync(
          unavailable,
          cancellation: TestContext.Current.CancellationToken));

      Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task EventsChannelWrapsTheSharedProtocolFailure() {
      using var server = new MockServer().Run(
        SendMalformedResponseAsync,
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient(useChannel: true);
      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      var requestException = await Assert.ThrowsAsync<ObsProtocolException>(
        () => client.GetRecordStatusAsync(TestContext.Current.CancellationToken));
      var channelException = await Assert.ThrowsAsync<ChannelClosedException>(
        () => client.Events.ReadAsync(
          TestContext.Current.CancellationToken).AsTask());

      Assert.Same(requestException, channelException.InnerException);
    }

    [Fact]
    public async Task PendingRequestFailsWithConnectionExceptionWhenServerAborts() {
      using var server = new MockServer().Run(
        AbortWithPendingRequestAsync,
        TestContext.Current.CancellationToken);
      using var client = ClientFlow.GetDebugClient(useChannel: true);
      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      var exception = await Assert.ThrowsAsync<ObsConnectionException>(
        () => client.GetRecordStatusAsync(TestContext.Current.CancellationToken));

      Assert.NotNull(exception.InnerException);
    }

    private static async Task AbortWithPendingRequestAsync(
      HttpListenerContext context,
      CancellationToken cancellation
    ) {
      var (webSocketContext, session) = await MockServer.HandshakeAsync(
        context,
        cancellation).ConfigureAwait(false);
      using var sessionScope = session;
      string requestId = (await session.ReceiveRequestAsync(
        "GetRecordStatus").ConfigureAwait(false))!;
      Assert.NotEmpty(requestId);
      webSocketContext.WebSocket.Abort();
    }

    private static async Task SendMalformedResponseAsync(
      HttpListenerContext context,
      CancellationToken cancellation
    ) {
      var (webSocketContext, session) = await MockServer.HandshakeAsync(
        context,
        cancellation).ConfigureAwait(false);
      using var _ = session;
      string requestId = (await session.ReceiveRequestAsync(
        "GetRecordStatus").ConfigureAwait(false))!;
      await session.SendAsync($$"""
{
  "op": 7,
  "d": {
    "requestType": "GetRecordStatus",
    "requestId": "{{requestId}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "responseData": {
      "outputActive": true,
      "outputBytes": 0,
      "outputDuration": 4796153459128734987162398746123978416448,
      "outputPaused": false,
      "outputTimecode": "00:00:00.000"
    }
  }
}
""").ConfigureAwait(false);
      await Task.Delay(Timeout.InfiniteTimeSpan, cancellation).ConfigureAwait(false);
      await MockServer.CloseQuietlyAsync(
        webSocketContext.WebSocket,
        cancellation).ConfigureAwait(false);
    }
  }
}
