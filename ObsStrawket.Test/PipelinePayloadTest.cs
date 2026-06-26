using ObsStrawket.DataTypes;
using ObsStrawket.Diagnostics;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {

  public class PipelinePayloadTest {
    [Fact]
    public async Task DoesNotEmitWebSocketPayloadDiagnosticsAtDefaultLevel() {
      using var server = new MockServer().Run(
        AcceptAndCloseAfterHandshakeAsync,
        TestContext.Current.CancellationToken);
      using var client = new ClientSocket();
      var diagnostics = new ConcurrentQueue<PipelineEvent>();
      client.PipelineEvent += diagnostics.Enqueue;

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      await client.PipelineCompletion.WaitAsync(TestContext.Current.CancellationToken);

      Assert.DoesNotContain(diagnostics, static diagnostic => diagnostic is WebSocketPayloadSent);
      Assert.DoesNotContain(diagnostics, static diagnostic => diagnostic is WebSocketPayloadReceived);
    }

    [Fact]
    public async Task EmitsWebSocketPayloadDiagnosticsAtInfoLevel() {
      using var server = new MockServer().Run(
        AcceptAndCloseAfterHandshakeAsync,
        TestContext.Current.CancellationToken);
      using var client = new ClientSocket();
      var diagnostics = new ConcurrentQueue<PipelineEvent>();
      client.PipelineEventLevel = PipelineLevel.Info;
      client.PipelineEvent += diagnostics.Enqueue;

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      await client.PipelineCompletion.WaitAsync(TestContext.Current.CancellationToken);

      Assert.Contains(diagnostics, static diagnostic => HasSentOp(diagnostic, 1));
      Assert.Contains(diagnostics, static diagnostic => HasReceivedOp(diagnostic, 0));
      Assert.Contains(diagnostics, static diagnostic => HasReceivedOp(diagnostic, 2));
    }

    [Fact]
    public async Task HighLevelClientForwardsWebSocketPayloadDiagnosticsWhenSubscribed() {
      using var server = new MockServer().Run(
        AcceptAndCloseAfterHandshakeAsync,
        TestContext.Current.CancellationToken);
      using var client = new ObsClientSocket();
      var diagnostics = new ConcurrentQueue<PipelineEvent>();
      client.PipelineEventLevel = PipelineLevel.Info;
      client.PipelineEvent += diagnostics.Enqueue;

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      Assert.Contains(diagnostics, static diagnostic => HasSentOp(diagnostic, 1));
      Assert.Contains(diagnostics, static diagnostic => HasReceivedOp(diagnostic, 0));
      Assert.Contains(diagnostics, static diagnostic => HasReceivedOp(diagnostic, 2));
    }

    [Fact]
    public async Task LowLevelPipelineEventHandlerExceptionDoesNotBreakConnection() {
      using var server = new MockServer().Run(
        AcceptAndCloseAfterHandshakeAsync,
        TestContext.Current.CancellationToken);
      using var client = new ClientSocket();
      client.PipelineEventLevel = PipelineLevel.Info;
      client.PipelineEvent += static _ => throw new InvalidOperationException("test handler failure");

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      await client.PipelineCompletion.WaitAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task HighLevelDispatchDiagnosticsFollowPipelineEventLevel() {
      using var server = new MockServer().Run(
        AcceptSendUnknownEventAndCloseAsync,
        TestContext.Current.CancellationToken);
      using var client = new ObsClientSocket();
      var diagnostics = new ConcurrentQueue<PipelineEvent>();
      var eventReceived = new TaskCompletionSource<IObsEvent>(
        TaskCreationOptions.RunContinuationsAsynchronously);
      client.PipelineEventLevel = PipelineLevel.Error;
      client.PipelineEvent += diagnostics.Enqueue;
      client.Event += ev => eventReceived.TrySetResult(ev);

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      _ = await eventReceived.Task.WaitAsync(TestContext.Current.CancellationToken);
      await client.CloseAsync();

      Assert.DoesNotContain(diagnostics, static diagnostic => diagnostic is UnclassifiedEventIgnored);
      Assert.DoesNotContain(diagnostics, static diagnostic => diagnostic is RawEventReceived);
    }

    [Fact]
    public async Task HighLevelDispatchDiagnosticsEmitAtDefaultLevel() {
      using var server = new MockServer().Run(
        AcceptSendUnknownEventAndCloseAsync,
        TestContext.Current.CancellationToken);
      using var client = new ObsClientSocket();
      var diagnostics = new ConcurrentQueue<PipelineEvent>();
      var eventReceived = new TaskCompletionSource<IObsEvent>(
        TaskCreationOptions.RunContinuationsAsynchronously);
      client.PipelineEvent += diagnostics.Enqueue;
      client.Event += ev => eventReceived.TrySetResult(ev);

      Assert.True(await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken));

      _ = await eventReceived.Task.WaitAsync(TestContext.Current.CancellationToken);
      await client.CloseAsync();

      Assert.Contains(diagnostics, static diagnostic => diagnostic is UnclassifiedEventIgnored);
      Assert.Contains(diagnostics, static diagnostic => diagnostic is RawEventReceived);
    }

    private static async Task AcceptAndCloseAfterHandshakeAsync(
      HttpListenerContext context,
      CancellationToken cancellation
    ) {
      var (webSocketContext, session) = await MockServer.HandshakeAsync(
        context,
        cancellation).ConfigureAwait(false);
      using var _ = session;
      await MockServer.CloseQuietlyAsync(
        webSocketContext.WebSocket,
        cancellation).ConfigureAwait(false);
    }

    private static async Task AcceptSendUnknownEventAndCloseAsync(
      HttpListenerContext context,
      CancellationToken cancellation
    ) {
      var (webSocketContext, session) = await MockServer.HandshakeAsync(
        context,
        cancellation).ConfigureAwait(false);
      using var _ = session;
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "UnknownCustomEvent",
    "eventIntent": 0,
    "eventData": {}
  }
}
""").ConfigureAwait(false);
      await MockServer.CloseQuietlyAsync(
        webSocketContext.WebSocket,
        cancellation).ConfigureAwait(false);
    }

    private static bool HasSentOp(PipelineEvent diagnostic, int op) {
      return diagnostic is WebSocketPayloadSent sent && HasOp(sent.Payload, op);
    }

    private static bool HasReceivedOp(PipelineEvent diagnostic, int op) {
      return diagnostic is WebSocketPayloadReceived received && HasOp(received.Payload, op);
    }

    private static bool HasOp(string payload, int op) {
      using var document = JsonDocument.Parse(payload);
      return document.RootElement.GetProperty("op").GetInt32() == op;
    }
  }
}
