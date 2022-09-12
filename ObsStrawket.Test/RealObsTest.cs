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
    private readonly Uri _uri = new Uri("ws://127.0.0.1:4455");

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
      await client.ConnectAsync(new Uri("ws://127.0.0.1:4455"), MockServer.Password).ConfigureAwait(false);
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

        // Record
        new StartRecordFlow(),
        new PauseRecordFlow(),
        new ResumeRecordFlow(),
        new ToggleRecordPauseFlow(),
        new GetRecordStatusFlow(),
        new StopRecordFlow(),
        new ToggleRecordFlow(),

//new CreateInputFlow(),
//new CreateSceneCollectionFlow(),
//new CreateSceneItemFlow(),
//new CreateSceneFlow(),
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
//new GetInputListFlow(),
//new GetInputMuteFlow(),
//new GetInputPropertiesListPropertyItemsFlow(),
//new GetInputSettingsFlow(),
//new GetInputVolumeFlow(),
//new GetLastReplayBufferReplayFlow(),
//new GetMediaInputStatusFlow(),
//new GetMonitorListFlow(),
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
//new GetSourceActiveFlow(),
//new GetSourceFilterDefaultSettingsFlow(),
//new GetSourceFilterListFlow(),
//new GetSourceFilterFlow(),
//new GetSourceScreenshotFlow(),
//new GetSpecialInputsFlow(),
//new GetStreamServiceSettingsFlow(),
//new GetStreamStatusFlow(),
//new GetStudioModeEnabledFlow(),
//new GetTransitionKindListFlow(),
//new GetVideoSettingsFlow(),
//new GetVirtualCamStatusFlow(),
//new OffsetMediaInputCursorFlow(),
//new OpenInputFiltersDialogFlow(),
//new OpenInputInteractDialogFlow(),
//new OpenInputPropertiesDialogFlow(),
//new OpenSourceProjectorFlow(),
//new OpenVideoMixProjectorFlow(),
//new PressInputPropertiesButtonFlow(),
//new RemoveInputFlow(),
//new RemoveSceneItemFlow(),
//new RemoveSceneFlow(),
//new RemoveSourceFilterFlow(),
//new SaveReplayBufferFlow(),
//new SaveSourceScreenshotFlow(),
//new SendStreamCaptionFlow(),
//new SetCurrentPreviewSceneFlow(),
//new SetCurrentProgramSceneFlow(),
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
//new SetStudioModeEnabledFlow(),
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
        Debug.WriteLine(flow.GetType().Name);
        await flow.RequestAsync(client).ConfigureAwait(false);
      }
      return;
    }
  }
}
