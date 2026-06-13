using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Specs;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Real {
  [Collection("IsolatedObs")]
  public class RealObsTest {
    private readonly IsolatedObsFixture _obs;

    private Uri _uri {
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
      await new ClientFlow().RunClientAsync(_uri, cancellation: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TestBadRequestAsync() {
      var client = ClientFlow.GetDebugClient(useChannel: true);
      await client.ConnectAsync(_uri, MockServer.Password, cancellation: TestContext.Current.CancellationToken);

      await Assert.ThrowsAsync<FailureResponseException>(async () => {
        var response = await client.StopRecordAsync(TestContext.Current.CancellationToken);
        response = await client.StopRecordAsync(TestContext.Current.CancellationToken);
      });

      var exception = await Assert.ThrowsAsync<FailureResponseException>(async () => {
        await client.BroadcastCustomEventAsync(new Dictionary<string, JsonElement?>(), TestContext.Current.CancellationToken);
      });

      return;
    }

    // Manual monitoring testbed. It never ends until OBS quits, so opt-in only.
    [Fact(Explicit = true)]
    public async Task JustMonitorObsEventAsync() {
      var source = new TaskCompletionSource<object?>();
      var client = ClientFlow.GetDebugClient();
      client.Disconnected += (e) => {
        source.TrySetResult(e);
      };

      await client.ConnectAsync(_uri, MockServer.Password, cancellation: TestContext.Current.CancellationToken);
      //while (await client.Events.WaitToReadAsync().ConfigureAwait(false)) {
      //  var ev = await client.Events.ReadAsync().ConfigureAwait(false);
      //  Debug.WriteLine(ev);
      //}
      object? result = await source.Task;
      Debug.WriteLine(result);
      return;
    }

    [Fact]
    public async Task TestbedAsync() {
      var client = ClientFlow.GetDebugClient(useChannel: true);
      await Assert.ThrowsAsync<AuthenticationFailureException>(
        () => client.ConnectAsync(_uri, "a", cancellation: TestContext.Current.CancellationToken)
      );
    }

    [Fact(Timeout = 60 * 1000)]
    public async Task TestUiAsync() {
      var client = ClientFlow.GetDebugClient(useChannel: true);
      try {
        await client.ConnectAsync(_uri, MockServer.Password, cancellation: TestContext.Current.CancellationToken);
        const string sceneName = "UI test scene";
        const string inputName = "UI test source";
        await client.CreateSceneAsync(
          sceneName,
          cancellation: TestContext.Current.CancellationToken
        );
        await client.CreateInputAsync(
          inputName: inputName,
          inputKind: "browser_source",
          sceneName: sceneName,
          inputSettings: new Dictionary<string, JsonElement?>(),
          sceneItemEnabled: true,
          cancellation: TestContext.Current.CancellationToken
        );

        await _obs.ObserveWindowAsync(
          "OpenInputPropertiesDialog",
          () => client.OpenInputPropertiesDialogAsync(
            inputName: inputName,
            cancellation: TestContext.Current.CancellationToken
          )
        );
        await _obs.ObserveWindowAsync(
          "OpenInputFiltersDialog",
          () => client.OpenInputFiltersDialogAsync(
            inputName: inputName,
            cancellation: TestContext.Current.CancellationToken
          )
        );
        await _obs.ObserveWindowAsync(
          "OpenInputInteractDialog",
          () => client.OpenInputInteractDialogAsync(
            inputName: inputName,
            cancellation: TestContext.Current.CancellationToken
          )
        );
        await _obs.ObserveWindowAsync(
          "OpenSourceProjector",
          () => client.OpenSourceProjectorAsync(
            sourceName: inputName,
            cancellation: TestContext.Current.CancellationToken
          )
        );
        await _obs.ObserveWindowAsync(
          "OpenVideoMixProjector",
          () => client.OpenVideoMixProjectorAsync(
            VideoMixType.Program,
            cancellation: TestContext.Current.CancellationToken
          )
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

      var client = ClientFlow.GetDebugClient(useChannel: true);
      await client.ConnectAsync(_uri, MockServer.Password, cancellation: TestContext.Current.CancellationToken);
      var flows = new List<ITestFlow>() {
        //new CallVendorRequestFlow(), // test how?
        //new SleepFlow(), // Not implemented

        new GetVersionFlow(), // General
        new GetStatsFlow(),
        new BroadcastCustomEventFlow(),
        new GetHotkeyListFlow(),
        new TriggerHotkeyByNameFlow(),

        new TriggerHotkeyByKeySequenceFlow(), // Requires manual setup.
        //new GetSpecialInputsFlow(), // Isolated OBS has no global audio sources.
        //new GetGroupSceneItemListFlow(), // obs-websocket cannot create a group.

        new CreateProfileFlow(), // setup sandbox profile

        new SetProfileParameterFlow(), // Config
        new GetProfileParameterFlow(),
        new GetProfileListFlow(),
        new SetCurrentProfileFlow(),
        new SetVideoSettingsFlow(),
        new GetVideoSettingsFlow(),
        new SetPersistentDataFlow(),
        new GetPersistentDataFlow(),
        new GetRecordDirectoryFlow(),
        new SetRecordDirectoryFlow(),

        new GetTransitionKindListFlow(),
        new SetCurrentSceneTransitionFlow(),
        new SetCurrentSceneTransitionDurationFlow(),
        //new SetCurrentSceneTransitionSettingsFlow(), // Default transitions are not configurable.
        new GetSceneTransitionListFlow(),
        new GetCurrentSceneTransitionFlow(),
        new GetCurrentSceneTransitionCursorFlow(),

        new CreateSceneCollectionFlow(), // setup sandbox scene collection
        new GetSceneCollectionListFlow(),
        new SetCurrentSceneCollectionFlow(),

        new CreateSceneFlow(), // setup sandbox scene
        new GetSceneListFlow(),

        new CreateInputFlow(), // Setup inputs
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
        new SetCurrentProgramSceneFlow(),
        new GetCurrentProgramSceneFlow(),
        new SetCurrentPreviewSceneFlow(),
        new GetCurrentPreviewSceneFlow(),
        new TriggerStudioModeTransitionFlow(),
        new SetTBarPositionFlow(),
        new GetStudioModeEnabledFlow(), // reset studio mode to false

        new SetMediaInputCursorFlow(), // setup
        new OffsetMediaInputCursorFlow(),
        new GetMediaInputStatusFlow(),
        //new TriggerMediaInputActionFlow(), // Requires a real media source to emit action events.

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

        new GetOutputListFlow(), // Outputs
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
        new RemoveProfileFlow(),
      };
      foreach (var flow in flows) {
        Debug.WriteLine($"Test {flow.GetType().Name}");
        while (client.Events.TryPeek(out var ev)) {
          ClientFlow.DrainEvents(client);
          await Task.Delay(100, TestContext.Current.CancellationToken);
        }
        await flow.RequestAsync(client);
      }
      return;
    }
  }
}
