namespace ObsStrawket.Test.Real {
  using ObsStrawket.Test.Specs;
  using ObsStrawket.Test.Utilities;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Net.Sockets;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Xunit;

  public class RealObsTest {
    private readonly bool _shouldSkip = false;
    private readonly Uri _uri = new("ws://127.0.0.1:4455");

    public RealObsTest() {
      _shouldSkip = Environment.GetEnvironmentVariable("CI") != null;
    }

    [Fact]
    public async Task TestNormalAsync() {
      if (_shouldSkip) {
        return;
      }
      await new ClientFlow().RunClientAsync(_uri).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestBadRequestAsync() {
      if (_shouldSkip) {
        return;
      }
      var client = ClientFlow.GetDebugClient(useChannel: true);
      await client.ConnectAsync(_uri, MockServer.Password).ConfigureAwait(false);

      await Assert.ThrowsAsync<FailureResponseException>(async () => {
        var response = await client.StopRecordAsync().ConfigureAwait(false);
        response = await client.StopRecordAsync().ConfigureAwait(false);
      }).ConfigureAwait(false);

      var exception = await Assert.ThrowsAsync<FailureResponseException>(async () => {
        await client.BroadcastCustomEventAsync(new Dictionary<string, object?>()).ConfigureAwait(false);
      }).ConfigureAwait(false);

      return;
    }

    [Fact]
    public async Task JustMonitorObsEventAsync() {
      if (_shouldSkip) {
        return;
      }
      var source = new TaskCompletionSource<object>();
      var client = ClientFlow.GetDebugClient();
      client.Disconnected += (e) => {
        source.TrySetResult(e);
      };

      await client.ConnectAsync(_uri, MockServer.Password).ConfigureAwait(false);
      //while (await client.Events.WaitToReadAsync().ConfigureAwait(false)) {
      //  var ev = await client.Events.ReadAsync().ConfigureAwait(false);
      //  Debug.WriteLine(ev);
      //}
      object result = await source.Task.ConfigureAwait(false);
      Debug.WriteLine(result);
      return;
    }

    [Fact]
    public async Task TestArbitraryAsync() {
      if (_shouldSkip) {
        return;
      }

      // xUnit doesn't support environment variable in visual studio runner.
      // But I want to do it.
      string envPath = "../../../env.json.tmp";
      if (File.Exists(envPath)) {
        var keyValues = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(
          File.OpenRead(envPath)
        ).ConfigureAwait(false);
        foreach (var (key, value) in keyValues!) {
          Environment.SetEnvironmentVariable(key, value);
        }
      }

      var client = ClientFlow.GetDebugClient(useChannel: true);
      await client.ConnectAsync(_uri, MockServer.Password).ConfigureAwait(false);
      var flows = new List<ITestFlow>() {
        //// General
        //new GetVersionFlow(),
        //new GetStatsFlow(),
        //new BroadcastCustomEventFlow(),
        ////new CallVendorRequestFlow(),
        //new GetHotkeyListFlow(),
        //new TriggerHotkeyByNameFlow(),
        ////new TriggerHotkeyByKeySequenceFlow(),
        ////new SleepFlow(),

        //new GetRecordDirectoryFlow(),

        //new CreateProfileFlow(), // setup sandbox profile

        //new GetProfileListFlow(),
        //new SetCurrentProfileFlow(),
        //new SetProfileParameterFlow(),
        //new GetProfileParameterFlow(),
        //new SetVideoSettingsFlow(),
        //new GetVideoSettingsFlow(),
        //new SetPersistentDataFlow(),
        //new GetPersistentDataFlow(),

        new CreateSceneCollectionFlow(), // setup sandbox scene collection
        new GetSceneCollectionListFlow(),
        new SetCurrentSceneCollectionFlow(),

        //new CreateSceneFlow(), // setup sandbox scene
        //new GetSceneListFlow(),

        //new GetGroupListFlow(),
        //new SetSceneNameFlow(),
        //new SetSceneSceneTransitionOverrideFlow(),
        //new GetSceneSceneTransitionOverrideFlow(),

// Scene items
//new GetGroupSceneItemListFlow(),

        // UI
        //new GetMonitorListFlow(),

        //new SetStudioModeEnabledFlow(), // setup studio mode
        //new SetCurrentProgramSceneFlow(),
        //new GetCurrentProgramSceneFlow(),
        //new SetCurrentPreviewSceneFlow(),
        //new GetCurrentPreviewSceneFlow(),
        //new GetStudioModeEnabledFlow(), // reset studio mode to false

        //new CreateInputFlow(),
        //new GetInputListFlow(),
        //new OpenInputPropertiesDialogFlow(),
        //new OpenInputFiltersDialogFlow(),
        //new OpenInputInteractDialogFlow(),
        //new OpenSourceProjectorFlow(),
        //new OpenVideoMixProjectorFlow(),

        // Outputs
        //new GetOutputListFlow(),

        //new StartRecordFlow(),
        //new PauseRecordFlow(),
        //new ResumeRecordFlow(),
        //new ToggleRecordPauseFlow(),
        //new GetRecordStatusFlow(),
        //new StopRecordFlow(),
        //new ToggleRecordFlow(),

        //new SetStreamServiceSettingsFlow(),
        //new GetStreamServiceSettingsFlow(),
        //new StartStreamFlow(),
        //new SendStreamCaptionFlow(),
        //new GetStreamStatusFlow(),
        //new StopStreamFlow(),
        //new ToggleStreamFlow(),

        //new GetSourceActiveFlow(),
        //new GetSourceScreenshotFlow(),
        //new SaveSourceScreenshotFlow(),

        //new RemoveInputFlow(), // cleanups
        //new RemoveSceneItemFlow(),
        //new RemoveSceneFlow(),
        //new RemoveProfileFlow(),


//new CreateSceneItemFlow(),
//new CreateSourceFilterFlow(),
//new DuplicateSceneItemFlow(),
//new GetCurrentSceneTransitionCursorFlow(),
//new GetCurrentSceneTransitionFlow(),
//new GetInputAudioBalanceFlow(),
//new GetInputAudioMonitorTypeFlow(),
//new GetInputAudioSyncOffsetFlow(),
//new GetInputAudioTracksFlow(),
//new GetInputDefaultSettingsFlow(),
//new GetInputKindListFlow(),
//new GetInputMuteFlow(),
//new GetInputPropertiesListPropertyItemsFlow(),
//new GetInputSettingsFlow(),
//new GetInputVolumeFlow(),
//new GetLastReplayBufferReplayFlow(),
//new GetMediaInputStatusFlow(),
//new GetOutputSettingsFlow(),
//new GetOutputStatusFlow(),
//new GetReplayBufferStatusFlow(),
//new GetSceneItemBlendModeFlow(),
//new GetSceneItemEnabledFlow(),
//new GetSceneItemIdFlow(),
//new GetSceneItemIndexFlow(),
//new GetSceneItemListFlow(),
//new GetSceneItemLockedFlow(),
//new GetSceneItemTransformFlow(),
//new GetSceneListFlow(),
//new GetSceneTransitionListFlow(),
//new GetSourceFilterDefaultSettingsFlow(),
//new GetSourceFilterListFlow(),
//new GetSourceFilterFlow(),
//new GetSpecialInputsFlow(),
//new GetTransitionKindListFlow(),
//new GetVirtualCamStatusFlow(),
//new OffsetMediaInputCursorFlow(),
//new PressInputPropertiesButtonFlow(),
//new RemoveSourceFilterFlow(),
//new SaveReplayBufferFlow(),
//new SetCurrentSceneTransitionDurationFlow(),
//new SetCurrentSceneTransitionSettingsFlow(),
//new SetCurrentSceneTransitionFlow(),
//new SetInputAudioBalanceFlow(),
//new SetInputAudioMonitorTypeFlow(),
//new SetInputAudioSyncOffsetFlow(),
//new SetInputAudioTracksFlow(),
//new SetInputMuteFlow(),
//new SetInputNameFlow(),
//new SetInputSettingsFlow(),
//new SetInputVolumeFlow(),
//new SetMediaInputCursorFlow(),
//new SetOutputSettingsFlow(),
//new SetSceneItemBlendModeFlow(),
//new SetSceneItemEnabledFlow(),
//new SetSceneItemIndexFlow(),
//new SetSceneItemLockedFlow(),
//new SetSceneItemTransformFlow(),
//new SetSourceFilterEnabledFlow(),
//new SetSourceFilterIndexFlow(),
//new SetSourceFilterNameFlow(),
//new SetSourceFilterSettingsFlow(),
//new SetTBarPositionFlow(),
//new StartOutputFlow(),
//new StartReplayBufferFlow(),
//new StartVirtualCamFlow(),
//new StopOutputFlow(),
//new StopReplayBufferFlow(),
//new StopVirtualCamFlow(),
//new ToggleInputMuteFlow(),
//new ToggleOutputFlow(),
//new ToggleReplayBufferFlow(),
//new ToggleVirtualCamFlow(),
//new TriggerMediaInputActionFlow(),
//new TriggerStudioModeTransitionFlow(),
      };
      foreach (var flow in flows) {
        Debug.WriteLine($"Test {flow.GetType().Name}");
        ClientFlow.DrainEvents(client);
        await flow.RequestAsync(client).ConfigureAwait(false);
      }
      return;
    }
  }
}
