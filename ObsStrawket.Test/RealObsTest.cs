using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Specs;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  [Collection("IsolatedObs")]
  public class RealObsTest {
    private readonly IsolatedObsFixture _obs;

    private Uri Uri {
      get {
        Assert.SkipWhen(!_obs.IsAvailable, "OBS is not installed.");
        return _obs.Uri!;
      }
    }

    public RealObsTest(IsolatedObsFixture obs) {
      _obs = obs;
    }

    [Fact]
    public async Task TestNormalAsync() {
      await new ClientFlow().RunClientAsync(Uri, cancellation: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestBadRequestAsync() {
      var client = GetDebugClient();
      _ = await client.ConnectAsync(Uri, MockServer.Password, cancellation: TestContext.Current.CancellationToken);

      _ = await Assert.ThrowsAsync<ObsRequestException>(async () => {
        var response = await client.StopRecordAsync(TestContext.Current.CancellationToken);
        response = await client.StopRecordAsync(TestContext.Current.CancellationToken);
      });

      var exception = await Assert.ThrowsAsync<ObsRequestException>(async () => {
        _ = await client.BroadcastCustomEventAsync([], TestContext.Current.CancellationToken);
      });

      return;
    }

    [Fact]
    public async Task TestMissingVendorRequestAsync() {
      var client = GetDebugClient();
      _ = await client.ConnectAsync(Uri, MockServer.Password, cancellation: TestContext.Current.CancellationToken);
      var exception = await Assert.ThrowsAsync<ObsRequestException>(
        () => client.CallVendorRequestAsync(
          "ObsStrawket.MissingVendor",
          "echo",
          cancellation: TestContext.Current.CancellationToken
        )
      );
      Assert.NotEqual(RequestStatus.Success, exception.Response.RequestStatus.Code);
      await client.CloseAsync();
    }

    [Fact(Timeout = 60 * 1000)]
    public async Task TestHighVolumeEventsAsync() {
      const string sceneName = "High-volume event scene";
      const string inputName = "High-volume event input";
      string wavePath = CreateWaveFile();
      var active = NewCompletionSource<InputActiveStateChanged>();
      var showing = NewCompletionSource<InputShowStateChanged>();
      var meters = NewCompletionSource<InputVolumeMeters>();
      var transform = NewCompletionSource<SceneItemTransformChanged>();
      var client = GetDebugClient();
      client.InputActiveStateChanged += ev => CompleteWhen(
        active,
        ev.InputName == inputName && ev.VideoActive,
        ev
      );
      client.InputShowStateChanged += ev => CompleteWhen(
        showing,
        ev.InputName == inputName && ev.VideoShowing,
        ev
      );
      client.InputVolumeMeters += ev => CompleteWhen(
        meters,
        ev.Inputs.Any(input => input.Name == inputName && input.LevelsMul.Count > 0),
        ev
      );
      try {
        var subscriptions =
          EventSubscription.All
          | EventSubscription.InputActiveStateChanged
          | EventSubscription.InputShowStateChanged
          | EventSubscription.InputVolumeMeters
          | EventSubscription.SceneItemTransformChanged;
        _ = await client.ConnectAsync(
          Uri,
          MockServer.Password,
          subscriptions,
          TestContext.Current.CancellationToken
        );
        _ = await client.CreateSceneAsync(sceneName, cancellation: TestContext.Current.CancellationToken);
        _ = await client.SetCurrentProgramSceneAsync(
          sceneName,
          cancellation: TestContext.Current.CancellationToken
        );
        var created = await client.CreateInputAsync(
          inputName: inputName,
          inputKind: CreateInputFlow.MediaInputKind,
          sceneName: sceneName,
          inputSettings: new Dictionary<string, JsonElement?> {
            ["is_local_file"] = true.ToJsonElement(),
            ["local_file"] = wavePath.ToJsonElement(),
            ["looping"] = true.ToJsonElement(),
          },
          sceneItemEnabled: true,
          cancellation: TestContext.Current.CancellationToken
        );
        client.SceneItemTransformChanged += ev => CompleteWhen(
          transform,
          ev.SceneItemId == created.SceneItemId
            && ev.SceneName == sceneName
            && ev.SceneItemTransform.TryGetValue("positionX", out var positionX)
            && positionX is { ValueKind: JsonValueKind.Number } value
            && value.GetDouble() == 123.0,
          ev
        );
        _ = await client.SetSceneItemTransformAsync(
          created.SceneItemId,
          new Dictionary<string, JsonElement?> {
            ["positionX"] = 123.0.ToJsonElement(),
          },
          sceneName: sceneName,
          cancellation: TestContext.Current.CancellationToken
        );
        var activeEvent = await active.Task.WaitAsync(TestContext.Current.CancellationToken);
        var showingEvent = await showing.Task.WaitAsync(TestContext.Current.CancellationToken);
        var meterEvent = await meters.Task.WaitAsync(TestContext.Current.CancellationToken);
        var transformEvent = await transform.Task.WaitAsync(TestContext.Current.CancellationToken);
        Assert.NotEmpty(activeEvent.InputUuid);
        Assert.Equal(activeEvent.InputUuid, showingEvent.InputUuid);
        var meter = Assert.Single(meterEvent.Inputs, input => input.Name == inputName);
        Assert.Equal(activeEvent.InputUuid, meter.Uuid);
        Assert.All(meter.LevelsMul, level => Assert.Equal(3, level.Count));
        Assert.Equal(sceneName, transformEvent.SceneName);
        Assert.NotEmpty(transformEvent.SceneUuid);
        Assert.Equal(123.0, transformEvent.SceneItemTransform["positionX"]!.Value.GetDouble());
      }
      finally {
        await client.CloseAsync();
        await _obs.RestartAsync();
        File.Delete(wavePath);
      }
    }

    [Fact(Timeout = 60 * 1000)]
    public async Task TestVendorPluginAsync() {
      Assert.SkipWhen(!IsolatedObsFixture.HasVendorPlugin, "OBS test vendor plugin is not installed.");
      var started = NewCompletionSource<VendorEvent>();
      var stopped = NewCompletionSource<VendorEvent>();
      var client = GetDebugClient();
      client.VendorEvent += ev => {
        CompleteWhen(
          started,
          ev.VendorName == "AdvancedSceneSwitcher"
            && ev.VendorEventType == "AdvancedSceneSwitcherStarted",
          ev
        );
        CompleteWhen(
          stopped,
          ev.VendorName == "AdvancedSceneSwitcher"
            && ev.VendorEventType == "AdvancedSceneSwitcherStopped",
          ev
        );
      };
      try {
        _ = await client.ConnectAsync(
          Uri,
          MockServer.Password,
          EventSubscription.All,
          TestContext.Current.CancellationToken
        );
        var status = await new CallVendorRequestFlow(
          "AdvancedSceneSwitcher",
          "IsAdvancedSceneSwitcherRunning",
          requestData: []
        ).RequestForResponseAsync(client);
        Assert.Equal("AdvancedSceneSwitcher", status.VendorName);
        Assert.Equal("IsAdvancedSceneSwitcherRunning", status.VendorRequestType);
        if (status.ResponseData["isRunning"]!.Value.GetBoolean()) {
          _ = await client.CallVendorRequestAsync(
            "AdvancedSceneSwitcher",
            "AdvancedSceneSwitcherStop",
            cancellation: TestContext.Current.CancellationToken
          );
          _ = await stopped.Task.WaitAsync(TestContext.Current.CancellationToken);
          stopped = NewCompletionSource<VendorEvent>();
        }
        _ = await client.CallVendorRequestAsync(
          "AdvancedSceneSwitcher",
          "AdvancedSceneSwitcherStart",
          cancellation: TestContext.Current.CancellationToken
        );
        Assert.Empty((await started.Task.WaitAsync(TestContext.Current.CancellationToken)).EventData);
        _ = await client.CallVendorRequestAsync(
          "AdvancedSceneSwitcher",
          "AdvancedSceneSwitcherStop",
          cancellation: TestContext.Current.CancellationToken
        );
        Assert.Empty((await stopped.Task.WaitAsync(TestContext.Current.CancellationToken)).EventData);
      }
      finally {
        await client.CloseAsync();
      }
    }

    [Fact]
    public async Task TestbedAsync() {
      var client = GetDebugClient();
      bool connected = await client.ConnectAsync(
        Uri,
        "a",
        cancellation: TestContext.Current.CancellationToken
      );
      Assert.False(connected);
    }

    [Fact(Timeout = 60 * 1000)]
    public async Task TestUiAsync() {
      var client = GetDebugClient();
      try {
        _ = await client.ConnectAsync(
          Uri,
          MockServer.Password,
          cancellation: TestContext.Current.CancellationToken
        );
        const string sceneName = "UI test scene";
        const string inputName = "UI test source";
        _ = await client.CreateSceneAsync(
          sceneName,
          cancellation: TestContext.Current.CancellationToken
        );
        _ = await client.CreateInputAsync(
          inputName: inputName,
          inputKind: "browser_source",
          sceneName: sceneName,
          inputSettings: [],
          sceneItemEnabled: true,
          cancellation: TestContext.Current.CancellationToken
        );

        await _obs.ObserveWindowAsync(
          "OpenInputPropertiesDialog",
          () => new OpenInputPropertiesDialogFlow(inputName).RequestAsync(client)
        );
        await _obs.ObserveWindowAsync(
          "OpenInputFiltersDialog",
          () => new OpenInputFiltersDialogFlow(inputName).RequestAsync(client)
        );
        await _obs.ObserveWindowAsync(
          "OpenInputInteractDialog",
          () => new OpenInputInteractDialogFlow(inputName).RequestAsync(client)
        );
        await _obs.ObserveWindowAsync(
          "OpenSourceProjector",
          () => new OpenSourceProjectorFlow(inputName).RequestAsync(client)
        );
        await _obs.ObserveWindowAsync(
          "OpenVideoMixProjector",
          () => new OpenVideoMixProjectorFlow().RequestAsync(client)
        );
      }
      finally {
        await _obs.RestartAsync();
      }
    }

    [Fact(Timeout = 2 * 60 * 1000)]
    public async Task TestSequenceAsync() {
      // xUnit doesn't support environment variable in visual studio runner.
      // But I want to do it.
      string envPath = "../../../env.json.tmp";
      if (File.Exists(envPath)) {
        var keyValues = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(
          File.OpenRead(envPath),
          cancellationToken: TestContext.Current.CancellationToken
        );
        foreach (var (key, value) in keyValues!) {
          Environment.SetEnvironmentVariable(key, value);
        }
      }

      var client = GetDebugClient();
      _ = await client.ConnectAsync(Uri, MockServer.Password, cancellation: TestContext.Current.CancellationToken);
      var flows = new List<ITestFlow>() {
        //new CallVendorRequestFlow(), // test how?

        new GetVersionFlow(), // General
        new GetOutputListFlow(),
        new GetCanvasListFlow(),
        new GetStatsFlow(),
        new BroadcastCustomEventFlow(),
        new GetHotkeyListFlow(),
        new TriggerHotkeyByNameFlow(),

        new TriggerHotkeyByKeySequenceFlow(),
        new GetSpecialInputsFlow(desktop1: null, mic1: null),
        new GetGroupSceneItemListFlow(expectMissingGroup: true),

        new CreateProfileFlow(_obs.ConfirmRemoveProfileAsync),

        new SetProfileParameterFlow(), // Config
        new GetProfileParameterFlow(),
        new GetProfileListFlow(),
        new SetCurrentProfileFlow(),
        new SetVideoSettingsFlow(),
        new SetPersistentDataFlow(),
        new SetRecordDirectoryFlow(),

        // It seems that GetVideoSettings needs to be called some time after SetVideoSettings
        // See https://github.com/obsproject/obs-websocket/issues/1344 
        new GetVideoSettingsFlow(),
        new GetPersistentDataFlow(),
        new GetRecordDirectoryFlow(),

        new GetTransitionKindListFlow(),
        new SetCurrentSceneTransitionFlow(),
        new SetCurrentSceneTransitionDurationFlow(),
        new SetCurrentSceneTransitionSettingsFlow(expectUnsupported: true),
        new GetSceneTransitionListFlow(),
        new GetCurrentSceneTransitionFlow(),
        new GetCurrentSceneTransitionCursorFlow(),

        new CreateSceneCollectionFlow(),
        new GetSceneCollectionListFlow(),
        new SetCurrentSceneCollectionFlow(),

        new CreateSceneFlow(),
        new GetSceneListFlow(),

        new CreateInputFlow(),
        new InputDeinterlaceFlow(),
        new GetInputListFlow(),
        new GetInputKindListFlow(),
        new SetInputNameFlow(),
        new GetInputDefaultSettingsFlow(),
        new SetInputSettingsFlow(),
        new GetInputSettingsFlow(),
        new SetInputMuteFlow(),
        new GetInputMuteFlow(),
        new ToggleInputMuteFlow(),
        new SetInputVolumeFlow(),
        new GetInputVolumeFlow(),
        new SetInputAudioBalanceFlow(),
        new GetInputAudioBalanceFlow(),
        new SetInputAudioSyncOffsetFlow(),
        new GetInputAudioSyncOffsetFlow(),
        new SetInputAudioMonitorTypeFlow(),
        new GetInputAudioMonitorTypeFlow(),
        new SetInputAudioTracksFlow(),
        new GetInputAudioTracksFlow(),
        new GetInputPropertiesListPropertyItemsFlow(),
        new PressInputPropertiesButtonFlow(),

        new StartRecordFlow(), // Record
        new GetRecordStatusFlow(),
        new RecordFileFlow(),
        new PauseRecordFlow(),
        new ResumeRecordFlow(),
        new ToggleRecordPauseFlow(),
        new StopRecordFlow(),
        new ToggleRecordFlow(),

        new SetStreamServiceSettingsFlow(), // Stream
        new GetStreamServiceSettingsFlow(),
        //new StartStreamFlow(), // Requires a reachable stream server.
        //new SendStreamCaptionFlow(), // Requires a reachable stream server.
        //new GetStreamStatusFlow(), // Requires a reachable stream server.
        //new StopStreamFlow(), // Requires a reachable stream server.
        //new ToggleStreamFlow(), // Requires a reachable stream server.

        new SetStudioModeEnabledFlow(), // setup studio mode
        new SleepFlow(), // RequestBatch
        new SetCurrentProgramSceneFlow(),
        new GetCurrentProgramSceneFlow(),
        new SetCurrentPreviewSceneFlow(),
        new GetCurrentPreviewSceneFlow(),
        new TriggerStudioModeTransitionFlow(),
        new SetTBarPositionFlow(),
        new GetStudioModeEnabledFlow(),

        new SetMediaInputCursorFlow(), // setup
        new OffsetMediaInputCursorFlow(),
        new GetMediaInputStatusFlow(),
        new TriggerMediaInputActionFlow(expectPlaybackEvents: false),

        new CreateSourceFilterFlow(), // Filters
        new GetSourceFilterDefaultSettingsFlow(),
        new SetSourceFilterIndexFlow(),
        new GetSourceFilterListFlow(),
        new SetSourceFilterSettingsFlow(),
        new SetSourceFilterEnabledFlow(),
        new SetSourceFilterNameFlow(),
        new GetSourceFilterFlow(),
        new RemoveSourceFilterFlow(),

        new CreateSceneItemFlow(), // Setup scene item
        new DuplicateSceneItemFlow(),
        new GetSceneItemIdFlow(),
        new GetSceneItemListFlow(),
        new SetSceneItemEnabledFlow(),
        new GetSceneItemEnabledFlow(),
        new SetSceneItemLockedFlow(),
        new GetSceneItemLockedFlow(),
        new SetSceneItemTransformFlow(),
        new GetSceneItemTransformFlow(),
        new SetSceneItemBlendModeFlow(),
        new GetSceneItemBlendModeFlow(),
        new SetSceneItemIndexFlow(),
        new GetSceneItemIndexFlow(),
        new RemoveSceneItemFlow(),

        new SetOutputSettingsFlow(),
        new GetOutputSettingsFlow(),
        new StartVirtualCamFlow(),
        new GetVirtualCamStatusFlow(),
        new StopVirtualCamFlow(),
        new ToggleVirtualCamFlow(),
        new StartReplayBufferFlow(),
        new GetReplayBufferStatusFlow(),
        new SaveReplayBufferFlow(),
        new GetLastReplayBufferReplayFlow(),
        new StopReplayBufferFlow(),
        new ToggleReplayBufferFlow(),
        new StartOutputFlow(),
        new GetOutputStatusFlow(),
        new StopOutputFlow(),
        new ToggleOutputFlow(),


        new GetSourceActiveFlow(), // Sources
        new GetSourceScreenshotFlow(),
        new SaveSourceScreenshotFlow(),

        new GetGroupListFlow(),
        new SetSceneNameFlow(),
        new SetSceneSceneTransitionOverrideFlow(),
        new GetSceneSceneTransitionOverrideFlow(),

        new GetMonitorListFlow(), // UI

        new RemoveInputFlow(), // cleanup input
        new RemoveSceneFlow(),
        new RemoveProfileFlow(_obs.ConfirmRemoveProfileAsync),
      };
      foreach (var flow in flows) {
        string flowName = flow.GetType().Name;
        _obs.RecordOperation($"Starting {flowName}");
        while (client.Events.TryPeek(out _)) {
          _ = ClientFlow.DrainEvents(client);
          await Task.Delay(100, TestContext.Current.CancellationToken);
        }
        try {
          await flow.RequestAsync(client);
          _obs.RecordOperation($"Completed {flowName}");
        }
        catch (Exception ex) {
          _obs.RecordOperation($"Failed {flowName}: {ex.GetType().Name}: {ex.Message}");
          if (!_obs.HasExited) {
            throw;
          }
          var failure = _obs.AddProcessDiagnostics(ex);
          try {
            await _obs.RestartAsync();
          }
          catch (Exception restartException) {
            throw new AggregateException(failure, restartException);
          }
          throw failure;
        }
      }
      await new CanvasUuidFlow(useExistingCanvas: true).RequestAsync(client);
      return;
    }

    private ObsClientSocket GetDebugClient() {
      var client = ClientFlow.GetDebugClient();
      client.PipelineEvent += e => _obs.RecordOperation($"Pipeline {e}");
      return client;
    }

    private static TaskCompletionSource<T> NewCompletionSource<T>() {
      return new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private static void CompleteWhen<T>(TaskCompletionSource<T> source, bool condition, T value) {
      if (condition) {
        _ = source.TrySetResult(value);
      }
    }

    private static string CreateWaveFile() {
      const int sampleRate = 48_000;
      const int durationSeconds = 2;
      const short channelCount = 1;
      const short bitsPerSample = 16;
      int sampleCount = sampleRate * durationSeconds;
      int dataLength = sampleCount * channelCount * bitsPerSample / 8;
      string path = Path.Combine(Path.GetTempPath(), $"obsstrawket-{Guid.NewGuid():N}.wav");
      using var stream = File.Create(path);
      using var writer = new BinaryWriter(stream);
      writer.Write("RIFF"u8);
      writer.Write(36 + dataLength);
      writer.Write("WAVE"u8);
      writer.Write("fmt "u8);
      writer.Write(16);
      writer.Write((short)1);
      writer.Write(channelCount);
      writer.Write(sampleRate);
      writer.Write(sampleRate * channelCount * bitsPerSample / 8);
      writer.Write((short)(channelCount * bitsPerSample / 8));
      writer.Write(bitsPerSample);
      writer.Write("data"u8);
      writer.Write(dataLength);
      for (int sample = 0; sample < sampleCount; sample++) {
        double phase = 2 * Math.PI * 440 * sample / sampleRate;
        writer.Write((short)(Math.Sin(phase) * short.MaxValue / 4));
      }
      return path;
    }
  }
}
