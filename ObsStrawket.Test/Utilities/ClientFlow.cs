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
    private static readonly TimeSpan DefaultEventWaitTimeout = TimeSpan.FromSeconds(30);
    private readonly Channel<IObsEvent> _events = Channel.CreateUnbounded<IObsEvent>();

    public static ObsClientSocket GetDebugClient(ClientSocket? socket = null, bool useChannel = false) {
      var client = new ObsClientSocket(socket, useChannel);
      client.PipelineEvent += e => Debug.WriteLine(e);
      return client;
    }

    /// <summary>Reads events until one of type <typeparamref name="T"/> arrives, discarding others.</summary>
    public static async Task<T> WaitEventAsync<T>(ObsClientSocket client) where T : class, IObsEvent {
      while (true) {
        if (await client.Events.ReadAsync().ConfigureAwait(false) is T typed) {
          return typed;
        }
      }
    }

    /// <summary>
    /// Collects two correlated events regardless of arrival order. OBS may emit related events
    /// in a different order, so awaiting them one-by-one can discard an event needed later.
    /// </summary>
    public static async Task<(T1, T2)> WaitEventsAsync<T1, T2>(
        ObsClientSocket client, Func<T1, bool> match1, Func<T2, bool> match2,
        CancellationToken cancellation = default)
        where T1 : class, IObsEvent where T2 : class, IObsEvent {
      var events = await WaitEventsAsync(
        client,
        DefaultEventWaitTimeout,
        cancellation,
        e => e is T1 first && match1(first),
        e => e is T2 second && match2(second)
      ).ConfigureAwait(false);
      return ((T1)events[0], (T2)events[1]);
    }

    /// <summary>Collects three correlated events regardless of arrival order.</summary>
    public static async Task<(T1, T2, T3)> WaitEventsAsync<T1, T2, T3>(
        ObsClientSocket client, Func<T1, bool> match1, Func<T2, bool> match2, Func<T3, bool> match3,
        CancellationToken cancellation = default)
        where T1 : class, IObsEvent where T2 : class, IObsEvent where T3 : class, IObsEvent {
      var events = await WaitEventsAsync(
        client,
        DefaultEventWaitTimeout,
        cancellation,
        e => e is T1 first && match1(first),
        e => e is T2 second && match2(second),
        e => e is T3 third && match3(third)
      ).ConfigureAwait(false);
      return ((T1)events[0], (T2)events[1], (T3)events[2]);
    }

    /// <summary>Collects one event per matcher, regardless of arrival order, for flows that need
    /// four or more correlated events. Each matcher consumes the first event it accepts.</summary>
    public static Task<IReadOnlyList<IObsEvent>> WaitEventsAsync(
        ObsClientSocket client, params Predicate<IObsEvent>[] matchers
    ) => WaitEventsAsync(client, DefaultEventWaitTimeout, default, matchers);

    public static Task<IReadOnlyList<IObsEvent>> WaitEventsAsync(
        ObsClientSocket client,
        CancellationToken cancellation,
        params Predicate<IObsEvent>[] matchers
    ) => WaitEventsAsync(client, DefaultEventWaitTimeout, cancellation, matchers);

    public static async Task<IReadOnlyList<IObsEvent>> WaitEventsAsync(
        ObsClientSocket client,
        TimeSpan timeout,
        CancellationToken cancellation,
        params Predicate<IObsEvent>[] matchers) {
      if (timeout <= TimeSpan.Zero) {
        throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be positive.");
      }
      var results = new IObsEvent?[matchers.Length];
      int remaining = matchers.Length;
      cancellation = cancellation.CanBeCanceled
        ? cancellation
        : TestContext.Current.CancellationToken;
      using var timeoutCancellation = new CancellationTokenSource(timeout);
      using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
        cancellation,
        timeoutCancellation.Token
      );
      try {
        while (remaining > 0) {
          var ev = await client.Events.ReadAsync(linkedCancellation.Token).ConfigureAwait(false);
          for (int i = 0; i < matchers.Length; i++) {
            if (results[i] is null && matchers[i](ev)) {
              results[i] = ev;
              remaining--;
              break;
            }
          }
        }
      }
      catch (OperationCanceledException exception)
          when (timeoutCancellation.IsCancellationRequested && !cancellation.IsCancellationRequested) {
        var unmatched = new List<int>(remaining);
        for (int i = 0; i < results.Length; i++) {
          if (results[i] is null) {
            unmatched.Add(i);
          }
        }
        throw new TimeoutException(
          $"Timed out after {timeout} waiting for correlated events. "
            + $"Unmatched matcher indexes: {string.Join(", ", unmatched)}.",
          exception
        );
      }
      return results!;
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
          || studioMode.RequestStatus.Code != RequestStatus.Success) {
        Assert.Fail("Cannot read the response");
        throw new Exception();
      }

      await client.SetStudioModeEnabledAsync(!studioMode.StudioModeEnabled, cancellation).ConfigureAwait(false);

      var studio = await ReadEventAsync<StudioModeStateChanged>(cancellation).ConfigureAwait(false);
      Assert.Equal(!studioMode.StudioModeEnabled, studio.StudioModeEnabled);
      Assert.Equal(EventSubscription.Ui, studio.EventIntent);
      studio = await ReadEventAsync<StudioModeStateChanged>(cancellation).ConfigureAwait(false);
      Assert.Equal(!studioMode.StudioModeEnabled, studio.StudioModeEnabled);
      Assert.Equal(0, _events.Reader.Count);

      var specials = await client.GetSpecialInputsAsync(cancellation).ConfigureAwait(false);
      // A fresh OBS may have no desktop audio input (e.g. isolated or headless instance).
      if (specials.Desktop1 is string desktopAudio) {
        var inputSettings = await client.GetInputSettingsAsync(desktopAudio, cancellation: cancellation).ConfigureAwait(false);
        Assert.False(string.IsNullOrEmpty(inputSettings.InputKind), "inputKind not found");
      }

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
      while (await _events.Reader.WaitToReadAsync(cancellation).ConfigureAwait(false)) {
        var ev = await _events.Reader.ReadAsync(cancellation).ConfigureAwait(false);
        if (ev is T cast) {
          return cast;
        }
      }
      throw new Exception($"It seems the channel is closed.");
    }
  }
}
