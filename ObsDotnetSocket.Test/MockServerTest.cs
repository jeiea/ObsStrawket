using ObsDotnetSocket.DataTypes.Predefineds;
using ObsDotnetSocket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsDotnetSocket.Test {
  public class MockServerTest {
    [Fact]
    public async Task TestAgainstMockServerAsync() {
      var cancellation = new CancellationTokenSource();
      using var server = new MockServer().Run(cancellation.Token);
      await new ClientFlow().RunClientAsync(MockServer.DefaultUri, cancellation: cancellation.Token);
    }

    [Fact]
    public async Task TestTrollUserAsync() {
      var cancellation = new CancellationTokenSource();
      var client = new ObsClientSocket();
      using var server = new MockServer().Run(cancellation.Token);

      var connections = new List<bool>();
      client.Closed += (o) => {
        connections.Add(false);
      };

      async Task ConnectAsync() {
        await client.ConnectAsync(MockServer.DefaultUri, MockServer.Password, cancellation: cancellation.Token).ConfigureAwait(false);
        connections.Add(true);
      }

      var tasks = new List<Task>();
      var abort = Task.CompletedTask;
      for (int i = 0; i < 50; i++) {
        await abort.ConfigureAwait(false);

        tasks.Add(Task.Run(async () => {
          try {
            await ConnectAsync().ConfigureAwait(false);
            var version = await client.GetVersionAsync(cancellation.Token).ConfigureAwait(false);
            Assert.Contains("bmp", version.SupportedImageFormats);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        tasks.Add(Task.Run(async () => {
          try {
            await client.CloseAsync();
          }
          catch (Exception ex) {
            Debug.WriteLine(ex);
          }
        }));
        await Task.Delay(i * 4).ConfigureAwait(false);
      }
      await Task.WhenAll(tasks).ConfigureAwait(false);

      int open = connections.Where(x => x == true).Count();
      int close = connections.Count - open;
      Assert.True(Math.Abs(open - close) <= 1, "open and close count doesn't match");
    }

    [Fact]
    public async Task TestServerAbortAsync() {
      var cancellation = new CancellationTokenSource();
      var abort = Task.CompletedTask;
      var client = new ObsClientSocket();
      client.Closed += (o) => {
        Debug.WriteLine(o);
      };

      for (int i = 0; i < 50; i++) {
        cancellation = new CancellationTokenSource();
        await abort.ConfigureAwait(false);
        Debug.WriteLine($"Session {i} start");

        using var irritatedServer = new MockServer().Run(cancellation.Token);
        int milliseconds = i * 4;
        abort = Task.Run(async () => {
          await Task.Delay(milliseconds);
          irritatedServer.Abort();
        });

        try {
          await new ClientFlow().RunClientAsync(MockServer.DefaultUri, client, cancellation: cancellation.Token);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex);
        }
        cancellation.Cancel();
      }

      cancellation = new CancellationTokenSource();
      await abort.ConfigureAwait(false);
      using var normalServer = new MockServer().Run(cancellation.Token);
      await new ClientFlow().RunClientAsync(MockServer.DefaultUri, client, cancellation: cancellation.Token);
    }

    [Fact]
    public async Task TestTrollServerAsync() {
      var cancellation = new CancellationTokenSource();
      using var server = new MockServer().Run(cancellation.Token, ServeTrollAsync);

      var socket = new ClientSocket {
        SetOptions = (sock) => {
          sock.Options.KeepAliveInterval = TimeSpan.FromMilliseconds(1000);
        }
      };
      var client = new ObsClientSocket(client: socket);
      await client.ConnectAsync(MockServer.DefaultUri, MockServer.Password, cancellation: cancellation.Token).ConfigureAwait(false);

      var recordTasks = new List<Task<GetRecordDirectoryResponse>>();
      for (int i = 0; i < 30; i++) {
        recordTasks.Add(client.GetRecordDirectoryAsync(cancellation.Token));
      }

      try {
        await Task.WhenAll(recordTasks).ConfigureAwait(false);
        Assert.Fail("exception not fired");
      }
      catch (QueueCancelledException) {
        Debug.WriteLine("Huh?");
      }
    }

    private static async Task ServeTrollAsync(HttpListenerContext context, CancellationToken token) {
      var (webSocketContext, session) = await MockServer.HandshakeAsync(context, token).ConfigureAwait(false);
      using var _1 = session;

      var guids = new List<string>();
      for (int i = 0; i < 20; i++) {
        string? guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetRecordDirectory"",
    ""requestId"": ""{guid}""
  }
}").ConfigureAwait(false);
        guids.Add(guid!);
      }

      foreach (string guid in guids.Take(10)) {
        await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""GetRecordDirectory"",
    ""responseData"": {
      ""recordDirectory"": ""C:\\Users""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      }

      byte[] buffer = new byte[] { 0x01, 0x02, 0x03, 0x04 };
      await webSocketContext.WebSocket.SendAsync(
        new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, token
      ).ConfigureAwait(false);
    }
  }
}
