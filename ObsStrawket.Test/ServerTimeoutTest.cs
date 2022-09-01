using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class ServerTimeoutTest {

    [Fact]
    public async Task TestAsync() {
      var cancellation = new CancellationTokenSource();
      using var server = new MockServer().Run(cancellation.Token, ServeTrollAsync);

      var socket = new ClientSocket {
        SetOptions = (sock) => {
          sock.Options.KeepAliveInterval = TimeSpan.FromMilliseconds(1000);
        }
      };
      var client = ClientFlow.GetDebugClient(socket);
      await client.ConnectAsync(server.Uri, MockServer.Password, cancellation: cancellation.Token).ConfigureAwait(false);

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
