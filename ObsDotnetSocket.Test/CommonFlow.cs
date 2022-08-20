using ObsDotnetSocket.DataTypes.Predefineds;
using ObsDotnetSocket.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Threading.Channels;
using ObsDotnetSocket.WebSocket;

namespace ObsDotnetSocket.Test {
  internal static class CommonFlow {
    public static async Task RunClientAsync(Uri uri, IClientWebSocket? socket = null, CancellationToken cancellation = default) {
      var client = new ObsClientSocket(socket);
      await client.ConnectAsync(uri, "ahrEYXzXKytCIlpI", cancellation: cancellation).ConfigureAwait(false);
      var events = Channel.CreateUnbounded<IEvent>();
      var source = new TaskCompletionSource<IEvent>();
      client.StudioModeStateChanged += (@event) => {
        _ = events.Writer.WriteAsync(@event);
      };

      var response = await client.RequestAsync(new RawRequest() {
        RequestId = "2521a51c-7040-4830-8181-492ab5477545",
        RequestType = "GetStudioModeEnabled"
      }, cancellation: cancellation).ConfigureAwait(false);
      if (response is not GetStudioModeEnabledResponse studioMode
          || studioMode.RequestStatus.Code != RequestStatusCode.Success) {
        Assert.Fail("Did not parse the request");
        throw new Exception();
      }

      await client.SetStudioModeEnabledAsync(!studioMode.StudioModeEnabled).ConfigureAwait(false);

      IEvent ev;

      ev = await events.Reader.ReadAsync(cancellation).ConfigureAwait(false);
      if (ev is not StudioModeStateChanged studio) {
        Assert.Fail("Type not converted");
        throw new Exception();
      }

      Assert.Equal(!studioMode.StudioModeEnabled, studio.StudioModeEnabled);
      Assert.Equal(0, events.Reader.Count);

      await client.CloseAsync().ConfigureAwait(false);
    }
  }
}
