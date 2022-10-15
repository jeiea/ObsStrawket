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
    public async Task TestbedAsync() {
      if (_shouldSkip) {
        return;
      }

      var client = ClientFlow.GetDebugClient(useChannel: true);
      await client.ConnectAsync(_uri, MockServer.Password).ConfigureAwait(false);

      var response = await client.RequestAsync(new RequestBatch() {
        Requests = {
          new Sleep() { SleepMillis = 100 },
          new GetStudioModeEnabled(),
        }
      }).ConfigureAwait(false);

      Debug.WriteLine(response.Results);
    }

    [Fact]
    public async Task TestSequenceAsync() {
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
        //new CallVendorRequestFlow(), // test how?
        //new SleepFlow(), // Not implemented

        //new GetVersionFlow(), // General
        //new GetStatsFlow(),
        //new BroadcastCustomEventFlow(),
        //new GetHotkeyListFlow(),
        //new TriggerHotkeyByNameFlow(),

        //new TriggerHotkeyByKeySequenceFlow(), // Requires manual setup.
        //new GetSpecialInputsFlow(), // Requires manual setup.
        //new GetGroupSceneItemListFlow(), // Requires manual setup.

        //new CreateProfileFlow(), // setup sandbox profile

        //new SetProfileParameterFlow(), // Config
        //new GetProfileParameterFlow(),
        //new GetProfileListFlow(),
        //new SetCurrentProfileFlow(),
        //new SetVideoSettingsFlow(),
        //new GetVideoSettingsFlow(),
        //new SetPersistentDataFlow(),
        //new GetPersistentDataFlow(),
        //new GetRecordDirectoryFlow(),

        //new GetTransitionKindListFlow(),
        //new SetCurrentSceneTransitionFlow(), // Requires manual scene collection setup
        //new SetCurrentSceneTransitionDurationFlow(), // Requires manual scene collection setup
        //new SetCurrentSceneTransitionSettingsFlow(), // Requires manual scene collection setup
        //new GetSceneTransitionListFlow(),
        //new GetCurrentSceneTransitionFlow(),
        //new GetCurrentSceneTransitionCursorFlow(),

        //new CreateSceneCollectionFlow(), // setup sandbox scene collection
        //new GetSceneCollectionListFlow(),
        //new SetCurrentSceneCollectionFlow(),

        //new CreateSceneFlow(), // setup sandbox scene
        //new GetSceneListFlow(),

        //new CreateInputFlow(), // Setup inputs
        //new GetInputListFlow(),
        //new GetInputKindListFlow(),
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

        //new StartRecordFlow(), // Record
        //new GetRecordStatusFlow(),
        //new PauseRecordFlow(),
        //new ResumeRecordFlow(),
        //new ToggleRecordPauseFlow(),
        //new StopRecordFlow(),
        //new ToggleRecordFlow(),

        //new SetStreamServiceSettingsFlow(), // Stream
        //new GetStreamServiceSettingsFlow(),
        //new StartStreamFlow(),
        //new SendStreamCaptionFlow(),
        //new GetStreamStatusFlow(),
        //new StopStreamFlow(),
        //new ToggleStreamFlow(),

        //new SetStudioModeEnabledFlow(), // setup studio mode
        //new SetCurrentProgramSceneFlow(),
        //new GetCurrentProgramSceneFlow(),
        //new SetCurrentPreviewSceneFlow(),
        //new GetCurrentPreviewSceneFlow(),
        //new TriggerStudioModeTransitionFlow(),
        //new SetTBarPositionFlow(),
        //new GetStudioModeEnabledFlow(), // reset studio mode to false

        //new SetMediaInputCursorFlow(), // setup
        //new OffsetMediaInputCursorFlow(),
        //new GetMediaInputStatusFlow(),
        //new TriggerMediaInputActionFlow(), // cleanup

        //new CreateSourceFilterFlow(), // Filters
        //new GetSourceFilterDefaultSettingsFlow(),
        //new SetSourceFilterIndexFlow(),
        //new GetSourceFilterListFlow(),
        //new SetSourceFilterSettingsFlow(),
        //new SetSourceFilterEnabledFlow(),
        //new SetSourceFilterNameFlow(),
        //new GetSourceFilterFlow(),
        //new RemoveSourceFilterFlow(),

        //new CreateSceneItemFlow(), // Setup scene item
        //new DuplicateSceneItemFlow(),
        //new GetSceneItemIdFlow(),
        //new GetSceneItemListFlow(),
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
        new StartReplayBufferFlow(),
        new GetReplayBufferStatusFlow(),
        new SaveReplayBufferFlow(),
        new GetLastReplayBufferReplayFlow(),
        new StopReplayBufferFlow(),
        new ToggleReplayBufferFlow(),
        //new StartOutputFlow(),
        //new GetOutputStatusFlow(),
        //new StopOutputFlow(),
        //new ToggleOutputFlow(),


        //new GetSourceActiveFlow(), // Sources
        //new GetSourceScreenshotFlow(),
        //new SaveSourceScreenshotFlow(),

        //new GetGroupListFlow(),
        //new SetSceneNameFlow(),
        //new SetSceneSceneTransitionOverrideFlow(),
        //new GetSceneSceneTransitionOverrideFlow(),

        //new GetMonitorListFlow(), // UI
        //new OpenInputPropertiesDialogFlow(),
        //new OpenInputFiltersDialogFlow(),
        //new OpenInputInteractDialogFlow(),
        //new OpenSourceProjectorFlow(),
        //new OpenVideoMixProjectorFlow(),

        //new RemoveInputFlow(), // cleanup input
        //new RemoveSceneFlow(),
        //new RemoveProfileFlow(),
      };
      foreach (var flow in flows) {
        Debug.WriteLine($"Test {flow.GetType().Name}");
        while (client.Events.TryPeek(out var ev)) {
          ClientFlow.DrainEvents(client);
          await Task.Delay(100).ConfigureAwait(false);
        }
        await flow.RequestAsync(client).ConfigureAwait(false);
      }
      return;
    }
  }
}
