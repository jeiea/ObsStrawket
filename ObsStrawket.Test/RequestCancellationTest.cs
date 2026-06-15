using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
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
  public class RequestCancellationTest {
    [Fact(Timeout = 10_000)]
    public async Task CancelsResponseWaitAndContinuesUsingConnectionAsync() {
      var requestReceived = NewCompletionSource<string>();
      var sendLateResponse = NewCompletionSource();
      var closeConnection = NewCompletionSource();
      using var serverCancellation = new CancellationTokenSource();
      using var server = new MockServer().Run(ServeRequestAsync, serverCancellation.Token);
      using var client = ClientFlow.GetDebugClient();
      var diagnostics = new ConcurrentQueue<PipelineEvent>();
      client.PipelineEvent += diagnostics.Enqueue;
      _ = await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken
      );

      using var requestCancellation = new CancellationTokenSource();
      var request = client.GetRecordDirectoryAsync(requestCancellation.Token);
      string requestId = await requestReceived.Task.WaitAsync(TestContext.Current.CancellationToken);

      requestCancellation.Cancel();
      _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(
        () => request.WaitAsync(
          TimeSpan.FromSeconds(1),
          TestContext.Current.CancellationToken
        )
      );

      sendLateResponse.SetResult();
      var version = await client.GetVersionAsync(TestContext.Current.CancellationToken);

      Assert.Equal("27.2.4", version.ObsVersion);
      Assert.Contains(
        diagnostics,
        diagnostic => diagnostic is OrphanResponse orphan && orphan.RequestId == requestId
      );
      closeConnection.SetResult();
      await client.CloseAsync();

      async Task ServeRequestAsync(HttpListenerContext context, CancellationToken token) {
        var (webSocketContext, session) = await MockServer.HandshakeAsync(context, token).ConfigureAwait(false);
        using var _ = session;
        string requestId = (await session.ReceiveRequestAsync("GetRecordDirectory").ConfigureAwait(false))!;
        requestReceived.SetResult(requestId);
        await sendLateResponse.Task.WaitAsync(token).ConfigureAwait(false);
        await session.SendGetRecordDirectoryResponseAsync(requestId).ConfigureAwait(false);
        string versionRequestId = (await session.ReceiveRequestAsync("GetVersion").ConfigureAwait(false))!;
        await session.SendGetVersionResponseAsync(versionRequestId).ConfigureAwait(false);
        await closeConnection.Task.WaitAsync(token).ConfigureAwait(false);
        await MockServer.CloseQuietlyAsync(webSocketContext.WebSocket, token).ConfigureAwait(false);
      }
    }

    [Fact(Timeout = 10_000)]
    public async Task CancelsBatchResponseWaitAsync() {
      var requestReceived = NewCompletionSource<string>();
      var sendLateResponse = NewCompletionSource();
      var closeConnection = NewCompletionSource();
      using var serverCancellation = new CancellationTokenSource();
      using var server = new MockServer().Run(ServeBatchAsync, serverCancellation.Token);
      using var client = ClientFlow.GetDebugClient();
      var diagnostics = new ConcurrentQueue<PipelineEvent>();
      client.PipelineEvent += diagnostics.Enqueue;
      _ = await client.ConnectAsync(
        server.Uri,
        MockServer.Password,
        cancellation: TestContext.Current.CancellationToken
      );

      using var requestCancellation = new CancellationTokenSource();
      var request = client.RequestAsync(
        new RequestBatch {
          Requests = {
            new GetStudioModeEnabled(),
          },
        },
        requestCancellation.Token
      );
      string requestId = await requestReceived.Task.WaitAsync(TestContext.Current.CancellationToken);

      requestCancellation.Cancel();

      _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(
        () => request.WaitAsync(
          TimeSpan.FromSeconds(1),
          TestContext.Current.CancellationToken
        )
      );
      sendLateResponse.SetResult();
      var version = await client.GetVersionAsync(TestContext.Current.CancellationToken);

      Assert.Equal("27.2.4", version.ObsVersion);
      Assert.Contains(
        diagnostics,
        diagnostic => diagnostic is OrphanBatchResponse orphan && orphan.RequestId == requestId
      );
      closeConnection.SetResult();
      await client.CloseAsync();

      async Task ServeBatchAsync(HttpListenerContext context, CancellationToken token) {
        var (webSocketContext, session) = await MockServer.HandshakeAsync(context, token).ConfigureAwait(false);
        using var _ = session;
        string json = await session.ReceiveAsync().ConfigureAwait(false);
        using var document = JsonDocument.Parse(json);
        Assert.Equal(8, document.RootElement.GetProperty("op").GetInt32());
        string requestId = document.RootElement.GetProperty("d").GetProperty("requestId").GetString()!;
        requestReceived.SetResult(requestId);
        await sendLateResponse.Task.WaitAsync(token).ConfigureAwait(false);
        await session.SendAsync($$"""
{
  "op": 9,
  "d": {
    "requestId": "{{requestId}}",
    "results": [
      {
        "requestType": "GetStudioModeEnabled",
        "requestId": "",
        "requestStatus": {
          "code": 100,
          "result": true
        },
        "responseData": {
          "studioModeEnabled": false
        }
      }
    ]
  }
}
""").ConfigureAwait(false);
        string versionRequestId = (await session.ReceiveRequestAsync("GetVersion").ConfigureAwait(false))!;
        await session.SendGetVersionResponseAsync(versionRequestId).ConfigureAwait(false);
        await closeConnection.Task.WaitAsync(token).ConfigureAwait(false);
        await MockServer.CloseQuietlyAsync(webSocketContext.WebSocket, token).ConfigureAwait(false);
      }
    }

    private static TaskCompletionSource NewCompletionSource() {
      return new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private static TaskCompletionSource<T> NewCompletionSource<T>() {
      return new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
  }
}
