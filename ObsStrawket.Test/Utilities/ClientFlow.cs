using Microsoft.Extensions.Logging.Debug;
using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Utilities {
  internal class ClientFlow {
    private readonly Channel<IEvent> _events = Channel.CreateUnbounded<IEvent>();

    public static ObsClientSocket GetDebugClient(ClientSocket? socket = null, bool useChannel = false) {
      return new ObsClientSocket(new DebugLoggerProvider().CreateLogger("Client"), socket, useChannel);
    }

    public async Task RunClientAsync(Uri uri, ObsClientSocket? socket = null, CancellationToken cancellation = default) {
      var client = socket ?? GetDebugClient();
      await client.ConnectAsync(uri, MockServer.Password, cancellation: cancellation).ConfigureAwait(false);

      var version = await client.GetVersionAsync(cancellation).ConfigureAwait(false);
      Assert.Contains("bmp", version.SupportedImageFormats);

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
      client.Disconnected += (o) => {
        _events.Writer.TryComplete(new Exception($"{o}"));
      };
      await client.SetStudioModeEnabledAsync(!studioMode.StudioModeEnabled, cancellation).ConfigureAwait(false);

      var studio = await ReadEventAsync<StudioModeStateChanged>(cancellation).ConfigureAwait(false);
      Assert.Equal(!studioMode.StudioModeEnabled, studio.StudioModeEnabled);
      studio = await ReadEventAsync<StudioModeStateChanged>(cancellation).ConfigureAwait(false);
      Assert.Equal(!studioMode.StudioModeEnabled, studio.StudioModeEnabled);
      Assert.Equal(0, _events.Reader.Count);

      var specials = await client.GetSpecialInputsAsync(cancellation).ConfigureAwait(false);
      var inputSettings = await client.GetInputSettingsAsync(specials.Desktop1!, cancellation).ConfigureAwait(false);
      Assert.True(inputSettings.InputSettings.ContainsKey("device_id"), "device_id not found");

      var directory = await client.GetRecordDirectoryAsync(cancellation).ConfigureAwait(false);
      Assert.True(Directory.Exists(directory.RecordDirectory), $"{directory.RecordDirectory} is not exists.");

      var stats = await client.GetStatsAsync(cancellation).ConfigureAwait(false);
      Assert.Equal(11.508459378338541, stats.CpuUsage);

      var startRecord = await client.StartRecordAsync(cancellation).ConfigureAwait(false);
      Assert.NotNull(startRecord);
      await ReadEventAsync<RecordStateChanged>(cancellation).ConfigureAwait(false);
      await ReadEventAsync<RecordStateChanged>(cancellation).ConfigureAwait(false);
      var recording = await client.StopRecordAsync(cancellation).ConfigureAwait(false);
      await ReadEventAsync<RecordStateChanged>(cancellation).ConfigureAwait(false);
      await ReadEventAsync<RecordStateChanged>(cancellation).ConfigureAwait(false);
      Assert.True(File.Exists(recording.OutputPath), $"{recording.OutputPath} is not exists.");

      await client.CloseAsync().ConfigureAwait(false);
    }

    public static async Task RequestBadAsync(Uri uri, ObsClientSocket? socket = null, CancellationToken cancellation = default) {
      var client = socket ?? GetDebugClient();
      await client.ConnectAsync(uri, MockServer.Password, cancellation: cancellation).ConfigureAwait(false);

      var taskSource = new TaskCompletionSource<object?>();
      client.RecordStateChanged += (ev) => {
        if (ev.OutputState == OutputState.Stopped) {
          taskSource.SetResult(null);
        }
      };
      try {
        await client.StopRecordAsync(cancellation).ConfigureAwait(false);
        await taskSource.Task.ConfigureAwait(false);
        var response = await client.StopRecordAsync(cancellation).ConfigureAwait(false);
        Assert.Fail("Unexpected response");
      }
      catch (FailureResponseException ex) {
        Debug.WriteLine(ex);
      }
      finally {
        await client.StopRecordAsync(cancellation).ConfigureAwait(false);
      }
    }

#pragma warning disable VSTHRD100 // Avoid async void methods
    private async void QueueEvent(IEvent @event) {
      try {
        await _events.Writer.WriteAsync(@event);
      }
      catch (ChannelClosedException) {
        // occurs in ServerAbortTest.
      }
    }
#pragma warning restore VSTHRD100 // Avoid async void methods

    private async Task<T> ReadEventAsync<T>(CancellationToken cancellation = default) where T : class {
      T? cast;
      while (await _events.Reader.WaitToReadAsync(cancellation).ConfigureAwait(false)) {
        var ev = await _events.Reader.ReadAsync(cancellation).ConfigureAwait(false);
        cast = ev as T;
        if (cast != null) {
          return cast;
        }
      }
      throw new Exception("Unreachable code");
    }
  }
}
