using ObsDotnetSocket.DataTypes;
using ObsDotnetSocket.DataTypes.Predefineds;
using ObsDotnetSocket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace ObsDotnetSocket.Test {
  public class MockServerParallelTest {
    private const int _parallelCount = 30;

    [Fact]
    public async Task TestParallelAsync() {
      CancellationTokenSource cancellation = new();
      using var server = new MockServer().Run(cancellation.Token, ServeEchoAsync);
      var tasks = new List<Task<IRequestResponse>>();

      try {
        var client = ClientFlow.GetDebugClient();
        await client.ConnectAsync(MockServer.DefaultUri, MockServer.Password, cancellation: cancellation.Token).ConfigureAwait(false);

        async Task<IRequestResponse> GetStudioModeEnabledAsync() {
          var result = await client.GetStudioModeEnabledAsync(cancellation.Token).ConfigureAwait(false);
          return result;
        }

        async Task<IRequestResponse> GetVersionAsync() {
          var result = await client.GetVersionAsync(cancellation.Token).ConfigureAwait(false);
          return result;
        }

        for (int i = 0; i < _parallelCount; i++) {
          // I know i is random, but if I changed this deterministic failure not occured.
          tasks.Add(Task.Run(() => (i % 2) switch {
            0 => GetStudioModeEnabledAsync(),
            _ => GetVersionAsync(),
          }));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        Assert.All(tasks, t => Assert.True(t.IsCompletedSuccessfully));
      }
      catch (Exception ex) {
        Debug.WriteLine(ex);
        throw;
      }
    }

    private async Task ServeEchoAsync(HttpListenerContext context, CancellationToken token) {
      try {
        var (webSocketContext, session) = await MockServer.HandshakeAsync(context, token).ConfigureAwait(false);
        using var _1 = session;

        await Task.Delay(500, token).ConfigureAwait(false);

        var channel = Channel.CreateUnbounded<string>();
        var receiveTask = Task.Run(async () => {
          for (int i = 0; i < _parallelCount; i++) {
            string json = await session.ReceiveAsync().ConfigureAwait(false);
            await channel.Writer.WriteAsync(json, token).ConfigureAwait(false);
          }
          channel.Writer.Complete();
        }, token);
        var sendTask = Task.Run(async () => {
          int i = 0;
          await foreach (string json in channel.Reader.ReadAllAsync(token)) {
            string guid = Regex.Match(json, @"[0-9a-f]{8}-[0-9a-f]{4}[^""]*").Value;
            if (json.Contains(@"""requestType"":""GetVersion"",")) {
              await session.SendGetVersionResponseAsync(guid).ConfigureAwait(false);
            }
            else if (json.Contains(@"""requestType"":""GetStudioModeEnabled"",")) {
              await session.SendGetStudioModeEnabledResponseAsync(guid).ConfigureAwait(false);
            }
            else {
              throw new Exception("Unexpected request");
            }
            Debug.WriteLine($"Served {++i} message");
          }
        }, token);
        await Task.WhenAll(receiveTask, sendTask).ConfigureAwait(false);

        await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", token).ConfigureAwait(false);
        Debug.WriteLine("Served all messages");
      }
      catch (Exception ex) {
        Debug.WriteLine(ex);
        throw;
      }
    }
  }
}
