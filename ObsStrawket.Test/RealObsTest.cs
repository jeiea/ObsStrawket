namespace ObsStrawket.Test.Real {
  using ObsStrawket.Test.Specs;
  using ObsStrawket.Test.Utilities;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Net.Sockets;
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
      var client = ClientFlow.GetDebugClient(useChannel: true);
      await client.ConnectAsync(_uri, MockServer.Password).ConfigureAwait(false);
      while (await client.Events.WaitToReadAsync().ConfigureAwait(false)) {
        var ev = await client.Events.ReadAsync().ConfigureAwait(false);
        Debug.WriteLine(ev);
      }
      return;
    }

    [Fact]
    public async Task TestArbitraryAsync() {
      if (_shouldSkip) {
        return;
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

        //// Config
        //new CreateProfileFlow(),
        //new SetCurrentProfileFlow(),
        //new SetPersistentDataFlow(),
        //new GetPersistentDataFlow(),
        //new RemoveProfileFlow(),

        //new GetRecordDirectoryFlow(),


        // Scenes
        //new CreateSceneCollectionFlow(),
        new CreateSceneFlow(),
        //new GetSceneListFlow(),

        // Ui
        //new GetMonitorListFlow(),
        new SetStudioModeEnabledFlow(),
        new SetCurrentProgramSceneFlow(),

        new GetStudioModeEnabledFlow(),

        new CreateInputFlow(),
        new GetInputListFlow(),
        new OpenInputPropertiesDialogFlow(),

        // Record
        //new StartRecordFlow(),
        //new PauseRecordFlow(),
        //new ResumeRecordFlow(),
        //new ToggleRecordPauseFlow(),
        //new GetRecordStatusFlow(),
        //new StopRecordFlow(),
        //new ToggleRecordFlow(),

        // Sources
        //new GetSourceActiveFlow(),
        //new GetSourceScreenshotFlow(),
        //new SaveSourceScreenshotFlow(),

        // Scenes cleanup
        new RemoveInputFlow(),
        //new RemoveSceneItemFlow(),
        new RemoveSceneFlow(),


//new CreateSceneItemFlow(),
//new CreateSourceFilterFlow(),
//new DuplicateSceneItemFlow(),
//new GetCurrentPreviewSceneFlow(),
//new GetCurrentProgramSceneFlow(),
//new GetCurrentSceneTransitionCursorFlow(),
//new GetCurrentSceneTransitionFlow(),
//new GetGroupListFlow(),
//new GetGroupSceneItemListFlow(),
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
//new GetOutputListFlow(),
//new GetOutputSettingsFlow(),
//new GetOutputStatusFlow(),
//new GetProfileListFlow(),
//new GetProfileParameterFlow(),
//new GetReplayBufferStatusFlow(),
//new GetSceneCollectionListFlow(),
//new GetSceneItemBlendModeFlow(),
//new GetSceneItemEnabledFlow(),
//new GetSceneItemIdFlow(),
//new GetSceneItemIndexFlow(),
//new GetSceneItemListFlow(),
//new GetSceneItemLockedFlow(),
//new GetSceneItemTransformFlow(),
//new GetSceneListFlow(),
//new GetSceneSceneTransitionOverrideFlow(),
//new GetSceneTransitionListFlow(),
//new GetSourceFilterDefaultSettingsFlow(),
//new GetSourceFilterListFlow(),
//new GetSourceFilterFlow(),
//new GetSpecialInputsFlow(),
//new GetStreamServiceSettingsFlow(),
//new GetStreamStatusFlow(),
//new GetTransitionKindListFlow(),
//new GetVideoSettingsFlow(),
//new GetVirtualCamStatusFlow(),
//new OffsetMediaInputCursorFlow(),
//new OpenInputFiltersDialogFlow(),
//new OpenInputInteractDialogFlow(),
//new OpenSourceProjectorFlow(),
//new OpenVideoMixProjectorFlow(),
//new PressInputPropertiesButtonFlow(),
//new RemoveSourceFilterFlow(),
//new SaveReplayBufferFlow(),
//new SendStreamCaptionFlow(),
//new SetCurrentPreviewSceneFlow(),
//new SetCurrentSceneCollectionFlow(),
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
//new SetProfileParameterFlow(),
//new SetSceneItemBlendModeFlow(),
//new SetSceneItemEnabledFlow(),
//new SetSceneItemIndexFlow(),
//new SetSceneItemLockedFlow(),
//new SetSceneItemTransformFlow(),
//new SetSceneNameFlow(),
//new SetSceneSceneTransitionOverrideFlow(),
//new SetSourceFilterEnabledFlow(),
//new SetSourceFilterIndexFlow(),
//new SetSourceFilterNameFlow(),
//new SetSourceFilterSettingsFlow(),
//new SetStreamServiceSettingsFlow(),
//new SetTBarPositionFlow(),
//new SetVideoSettingsFlow(),
//new StartOutputFlow(),
//new StartReplayBufferFlow(),
//new StartStreamFlow(),
//new StartVirtualCamFlow(),
//new StopOutputFlow(),
//new StopReplayBufferFlow(),
//new StopStreamFlow(),
//new StopVirtualCamFlow(),
//new ToggleInputMuteFlow(),
//new ToggleOutputFlow(),
//new ToggleReplayBufferFlow(),
//new ToggleStreamFlow(),
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
