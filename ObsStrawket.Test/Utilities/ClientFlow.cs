using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Specs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Utilities {
  internal class ClientFlow {
    private readonly Channel<IObsEvent> _events = Channel.CreateUnbounded<IObsEvent>();

    public static ObsClientSocket GetDebugClient(ClientSocket? socket = null, ILogger? logger = null, bool useChannel = false) {
      return new ObsClientSocket(logger ?? new DebugLoggerProvider().CreateLogger("Client"), socket, useChannel);
    }

    public async Task RunClientAsync(Uri uri, ObsClientSocket? socket = null, CancellationToken cancellation = default) {
      var client = socket ?? GetDebugClient();
      try {
        await RunClientInternalAsync(uri, client, cancellation).ConfigureAwait(false);
      }
      finally {
        client.Disconnected -= TryComplete;
        client.StudioModeStateChanged -= QueueEvent;
        client.Event -= QueueEvent;
      }
    }

    public static List<IObsEvent> DrainEvents(ObsClientSocket client) {
      var list = new List<IObsEvent>();
      while (client.Events.TryRead(out var ev)) {
        list.Add(ev);
      }
      return list;
    }

    public static async Task RequestBadAsync(Uri uri, ObsClientSocket? socket = null, CancellationToken cancellation = default) {
      var client = socket ?? GetDebugClient();
      await client.ConnectAsync(uri, MockServer.Password, cancellation: cancellation).ConfigureAwait(false);

      var taskSource = new TaskCompletionSource<object?>();
      client.RecordStateChanged += (ev) => {
        if (ev.OutputState == ObsOutputState.Stopped) {
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

    private async Task RunClientInternalAsync(Uri uri, ObsClientSocket client, CancellationToken cancellation) {
      client.Event += QueueEvent;
      client.StudioModeStateChanged += QueueEvent;
      client.Disconnected += TryComplete;

      await client.ConnectAsync(uri, MockServer.Password, cancellation: cancellation).ConfigureAwait(false);

      await new GetVersionFlow().RequestAsync(client).ConfigureAwait(false);

      var response = await client.RequestAsync(new RawRequest() {
        RequestId = "2521a51c-7040-4830-8181-492ab5477545",
        RequestType = "GetStudioModeEnabled"
      }, cancellation: cancellation).ConfigureAwait(false);
      if (response is not GetStudioModeEnabledResponse studioMode
          || studioMode.RequestStatus.Code != RequestStatusCode.Success) {
        Assert.Fail("Cannot read the response");
        throw new Exception();
      }

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
      Assert.InRange(stats.CpuUsage, 0, 100);

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

    private void TryComplete(Exception? exception) {
      _events.Writer.TryComplete(exception);
    }

    private async void QueueEvent(IObsEvent @event) {
      try {
        await _events.Writer.WriteAsync(@event).ConfigureAwait(false);
      }
      catch (ChannelClosedException) {
        // occurs in ServerAbortTest.
      }
    }

    private async Task<T> ReadEventAsync<T>(CancellationToken cancellation = default) where T : class {
      if (await _events.Reader.WaitToReadAsync(cancellation).ConfigureAwait(false)) {
        var ev = await _events.Reader.ReadAsync(cancellation).ConfigureAwait(false);
        if (ev is T cast) {
          return cast;
        }
        throw new Exception($"type mismatch: expected: {typeof(T).Name}, actual: {ev.GetType().Name}");
      }
      throw new Exception($"It seems the channel is closed.");
    }
  }
}
