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
using System.IO;
using System.Net.WebSockets;

namespace ObsDotnetSocket.Test {
  internal class CommonFlow {
    private readonly Channel<IEvent> _events = Channel.CreateUnbounded<IEvent>();

    public async Task RunClientAsync(Uri uri, ClientWebSocket? socket = null, CancellationToken cancellation = default) {
      var client = new ObsClientSocket(socket);
      await client.ConnectAsync(uri, "ahrEYXzXKytCIlpI", cancellation: cancellation).ConfigureAwait(false);

      var response = await client.RequestAsync(new RawRequest() {
        RequestId = "2521a51c-7040-4830-8181-492ab5477545",
        RequestType = "GetStudioModeEnabled"
      }, cancellation: cancellation).ConfigureAwait(false);
      if (response is not GetStudioModeEnabledResponse studioMode
          || studioMode.RequestStatus.Code != RequestStatusCode.Success) {
        Assert.Fail("Did not parse the request");
        throw new Exception();
      }

      client.Event += QueueEvent;
      client.StudioModeStateChanged += QueueEvent;
      await client.SetStudioModeEnabledAsync(!studioMode.StudioModeEnabled).ConfigureAwait(false);

      var studio = await ReadEventAsync<StudioModeStateChanged>(cancellation).ConfigureAwait(false);
      Assert.Equal(!studioMode.StudioModeEnabled, studio.StudioModeEnabled);
      studio = await ReadEventAsync<StudioModeStateChanged>(cancellation).ConfigureAwait(false);
      Assert.Equal(!studioMode.StudioModeEnabled, studio.StudioModeEnabled);
      Assert.Equal(0, _events.Reader.Count);

      var specials = await client.GetSpecialInputsAsync(cancellation).ConfigureAwait(false);
      var inputSettings = await client.GetInputSettingsAsync(specials.Desktop1!, cancellation).ConfigureAwait(false);
      Assert.True(inputSettings.InputSettings.ContainsKey("device_id"), "device_id not found");

      var directory = await client.GetRecordDirectoryAsync().ConfigureAwait(false);
      Assert.True(Directory.Exists(directory.RecordDirectory));

      await client.StartRecordAsync().ConfigureAwait(false);
      await ReadEventAsync<RecordStateChanged>(cancellation).ConfigureAwait(false);
      await ReadEventAsync<RecordStateChanged>(cancellation).ConfigureAwait(false);
      var recording = await client.StopRecordAsync().ConfigureAwait(false);
      await ReadEventAsync<RecordStateChanged>(cancellation).ConfigureAwait(false);
      await ReadEventAsync<RecordStateChanged>(cancellation).ConfigureAwait(false);
      Assert.True(File.Exists(recording.OutputPath));

      await client.CloseAsync().ConfigureAwait(false);
    }

    private void QueueEvent(IEvent @event) {
      _ = _events.Writer.WriteAsync(@event);
    }

    private async Task<T> ReadEventAsync<T>(CancellationToken cancellation = default) where T : class {
      T? cast;
      while (true) {
        var ev = await _events.Reader.ReadAsync(cancellation).ConfigureAwait(false);
        cast = ev as T;
        if (cast != null) {
          return cast;
        }
      }
    }
  }
}
