using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
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

namespace ObsStrawket.Test {
  public class ParallelRequestTest {
    private const int _parallelCount = 30;
    private readonly TaskCompletionSource<int> _serverComplete = new();

    [Fact]
    public async Task TestAsync() {
      CancellationTokenSource cancellation = new();
      using var server = new MockServer().Run(cancellation.Token, ServeEchoAsync);
      var tasks = new List<Task<IOpCodeMessage>>();

      try {
        var client = ClientFlow.GetDebugClient(useChannel: true);
        await client.ConnectAsync(server.Uri, MockServer.Password, cancellation: cancellation.Token).ConfigureAwait(false);

        async Task<IOpCodeMessage> GetStudioModeEnabledAsync() {
          var result = await client.GetStudioModeEnabledAsync(cancellation.Token).ConfigureAwait(false);
          return result;
        }

        async Task<IOpCodeMessage> SetStudioModeEnabledAsync() {
          await client.SetStudioModeEnabledAsync(false, cancellation.Token).ConfigureAwait(false);
          var changed = await client.Events.ReadAsync(cancellation.Token).ConfigureAwait(false);
          return changed;
        }

        async Task<IOpCodeMessage> GetVersionAsync() {
          var result = await client.GetVersionAsync(cancellation.Token).ConfigureAwait(false);
          return result;
        }

        for (int i = 0; i < _parallelCount; i++) {
          tasks.Add(Task.Run(() => (i % 3) switch {
            0 => GetStudioModeEnabledAsync(),
            1 => SetStudioModeEnabledAsync(),
            _ => GetVersionAsync(),
          }));
        }

        await TestUtil.WhenAnyThrowsAsync(tasks).ConfigureAwait(false);
        Assert.All(tasks, t => Assert.True(t.IsCompletedSuccessfully));
        await _serverComplete.Task.ConfigureAwait(false);
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

        string largeString = new('D', 32 * 1024);
        var sendTask = Task.Run(async () => {
          int i = 0;
          await foreach (string json in channel.Reader.ReadAllAsync(token)) {
            string guid = Regex.Match(json, @"[0-9a-f]{8}-[0-9a-f]{4}[^""]*").Value;
            if (json.Contains(@"""requestType"":""GetVersion"",")) {
              await session.SendGetVersionResponseAsync(guid, largeString).ConfigureAwait(false);
            }
            else if (json.Contains(@"""requestType"":""GetStudioModeEnabled"",")) {
              await session.SendGetStudioModeEnabledResponseAsync(guid).ConfigureAwait(false);
            }
            else if (json.Contains(@"""requestType"":""SetStudioModeEnabled"",")) {
              await session.SendStudioModeStateChangedAsync().ConfigureAwait(false);
            }
            else {
              throw new Exception("Unexpected request");
            }
            Debug.WriteLine($"Served {++i} message");
          }
        }, token);
        await TestUtil.WhenAnyThrowsAsync(receiveTask, sendTask).ConfigureAwait(false);

        await webSocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", token).ConfigureAwait(false);
        Debug.WriteLine("Served all messages");
        _serverComplete.SetResult(0);
      }
      catch (Exception ex) {
        Debug.WriteLine(ex);
        throw;
      }
    }
  }
}
