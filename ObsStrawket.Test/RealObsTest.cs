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

namespace ObsStrawket.Test.Real {
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

        //new SetProfileParameterFlow(),
        //new GetProfileParameterFlow(),
        //new GetProfileListFlow(),
        //new SetCurrentProfileFlow(),
        //new SetVideoSettingsFlow(),
        //new GetVideoSettingsFlow(),
        //new SetPersistentDataFlow(),
        //new GetPersistentDataFlow(),

        //new CreateSceneCollectionFlow(), // setup sandbox scene collection
        //new GetSceneCollectionListFlow(),
        //new SetCurrentSceneCollectionFlow(),

        new CreateSceneFlow(), // setup sandbox scene
        //new GetSceneListFlow(),

        //new GetGroupListFlow(),
        //new SetSceneNameFlow(),
        //new SetSceneSceneTransitionOverrideFlow(),
        //new GetSceneSceneTransitionOverrideFlow(),


        //new SetStudioModeEnabledFlow(), // setup studio mode
        //new SetCurrentProgramSceneFlow(),
        //new GetCurrentProgramSceneFlow(),
        //new SetCurrentPreviewSceneFlow(),
        //new GetCurrentPreviewSceneFlow(),
        //new GetStudioModeEnabledFlow(), // reset studio mode to false

        //new GetMonitorListFlow(), // UI
        //new OpenInputPropertiesDialogFlow(),
        //new OpenInputFiltersDialogFlow(),
        //new OpenInputInteractDialogFlow(),
        //new OpenSourceProjectorFlow(),
        //new OpenVideoMixProjectorFlow(),

        new CreateInputFlow(), // Inputs
        //new GetInputListFlow(),
        //new GetInputKindListFlow(),
        //new GetSpecialInputsFlow(),
        //new SetInputNameFlow(),
        //new GetInputDefaultSettingsFlow(),
        //new SetInputSettingsFlow(),
        //new GetInputSettingsFlow(),
        //new SetInputMuteFlow(),
        //new GetInputMuteFlow(),
        //new ToggleInputMuteFlow(),
        //new SetInputVolumeFlow(),
        //new GetInputVolumeFlow(),
        //new SetInputAudioBalanceFlow(),
        //new GetInputAudioBalanceFlow(),
        //new SetInputAudioSyncOffsetFlow(),
        //new GetInputAudioSyncOffsetFlow(),
        //new SetInputAudioMonitorTypeFlow(),
        //new GetInputAudioMonitorTypeFlow(),
        //new SetInputAudioTracksFlow(),
        //new GetInputAudioTracksFlow(),
        //new GetInputPropertiesListPropertyItemsFlow(),
        //new PressInputPropertiesButtonFlow(),

        new CreateSourceFilterFlow(), // Filters
        new GetSourceFilterDefaultSettingsFlow(),
        new SetSourceFilterIndexFlow(),
        new GetSourceFilterListFlow(),
        new SetSourceFilterSettingsFlow(),
        new SetSourceFilterEnabledFlow(),
        new SetSourceFilterNameFlow(),
        new GetSourceFilterFlow(),
        new RemoveSourceFilterFlow(),

        //new CreateSceneItemFlow(), // Setup scene item
        //new DuplicateSceneItemFlow(),
        //new GetSceneItemIdFlow(),
        //new GetSceneItemListFlow(),
        //new GetGroupSceneItemListFlow(),
        //new SetSceneItemEnabledFlow(),
        //new GetSceneItemEnabledFlow(),
        //new SetSceneItemLockedFlow(),
        //new GetSceneItemLockedFlow(),
        //new SetSceneItemTransformFlow(),
        //new GetSceneItemTransformFlow(),
        //new SetSceneItemBlendModeFlow(),
        //new GetSceneItemBlendModeFlow(),
        //new SetSceneItemIndexFlow(),
        //new GetSceneItemIndexFlow(),
        //new RemoveSceneItemFlow(),

        //new GetOutputListFlow(), // Outputs
        //new SetOutputSettingsFlow(),
        //new GetOutputSettingsFlow(),

        //new StartVirtualCamFlow(),
        //new GetVirtualCamStatusFlow(),
        //new StopVirtualCamFlow(),
        //new ToggleVirtualCamFlow(),
        //new StartReplayBufferFlow(),
        //new GetReplayBufferStatusFlow(),
        //new SaveReplayBufferFlow(),
        //new GetLastReplayBufferReplayFlow(),
        //new StopReplayBufferFlow(),
        //new ToggleReplayBufferFlow(),
        //new StartOutputFlow(),
        //new GetOutputStatusFlow(),
        //new StopOutputFlow(),
        //new ToggleOutputFlow(),

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

        new RemoveInputFlow(), // cleanup input
        //new RemoveSceneFlow(),
        //new RemoveProfileFlow(),


//new GetCurrentSceneTransitionCursorFlow(),
//new GetCurrentSceneTransitionFlow(),
//new GetMediaInputStatusFlow(),
//new GetSceneTransitionListFlow(),
//new GetTransitionKindListFlow(),
//new OffsetMediaInputCursorFlow(),
//new SetCurrentSceneTransitionDurationFlow(),
//new SetCurrentSceneTransitionFlow(),
//new SetCurrentSceneTransitionSettingsFlow(),
//new SetMediaInputCursorFlow(),
//new SetTBarPositionFlow(),
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
