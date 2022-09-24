# ObsStrawket

[![prerelease shield](https://img.shields.io/nuget/vpre/ObsStrawket)](https://www.nuget.org/packages/ObsStrawket) ![Downloads count](https://img.shields.io/nuget/dt/ObsStrawket)

Personal .NET implementation of
[obs-websocket](https://github.com/obsproject/obs-websocket) protocol v5.

## Can I use this in production?

No. I verified few events and requests. And I can change interfaces later.

But I won't stop you from using this if this works for you.

## Difference to [obs-websocket-dotnet](https://github.com/BarRaider/obs-websocket-dotnet)

|              |             ObsStrawket              |        obs-websocket-dotnet         |
| :----------: | :----------------------------------: | :---------------------------------: |
| Dependencies | System.Net.WebSockets<br>MessagePack | WebSocket.Client<br>Newtonsoft.Json |
| Method type  |             Asynchronous             |             Synchronous             |

## Example

```csharp
var client = new ObsClientSocket();
await client.ConnectAsync(new Uri("ws://localhost:4455"), "ahrEYXzXKytCIlpI");

var version = await client.GetVersionAsync();
Assert.Contains("bmp", version.SupportedImageFormats);

// Listen specific event.
client.RecordStateChanged += (changed) => {
  switch (changed.OutputState) {
  case OutputState.Unknown:
  case OutputState.Starting:
  case OutputState.Started:
  case OutputState.Stopping:
  case OutputState.Stopped:
  case OutputState.Paused:
  case OutputState.Resumed:
    break;
  }
};
// Listen all events and filter.
client.Event += (ev) => {
  switch (ev) {
  case RecordStateChanged changed:
    break;
  }
};
await client.StartRecordAsync();

await client.CloseAsync();
```

# To do

- Complete the support list
- Batch request

# Support list

Checkmark means it has test.

## Events

- [x] CurrentPreviewSceneChanged
- [x] CurrentProfileChanged
- [x] CurrentProfileChanging
- [x] CurrentProgramSceneChanged
- [x] CurrentSceneCollectionChanged
- [x] CurrentSceneCollectionChanging
- [ ] CurrentSceneTransitionChanged
- [ ] CurrentSceneTransitionDurationChanged
- [x] ExitStarted
- [ ] InputActiveStateChanged
- [x] InputAudioBalanceChanged
- [x] InputAudioMonitorTypeChanged
- [x] InputAudioSyncOffsetChanged
- [x] InputAudioTracksChanged
- [x] InputCreated
- [x] InputMuteStateChanged
- [x] InputNameChanged
- [x] InputRemoved
- [ ] InputShowStateChanged
- [x] InputVolumeChanged
- [ ] InputVolumeMeters
- [ ] MediaInputActionTriggered
- [ ] MediaInputPlaybackEnded
- [ ] MediaInputPlaybackStarted
- [x] ProfileListChanged
- [x] RecordStateChanged
- [x] ReplayBufferSaved
- [x] ReplayBufferStateChanged
- [x] SceneCollectionListChanged
- [x] SceneCreated
- [x] SceneItemCreated
- [x] SceneItemEnableStateChanged
- [x] SceneItemListReindexed
- [x] SceneItemLockStateChanged
- [x] SceneItemRemoved
- [x] SceneItemSelected
- [ ] SceneItemTransformChanged
- [x] SceneListChanged
- [x] SceneNameChanged
- [x] SceneRemoved
- [x] SceneTransitionEnded
- [x] SceneTransitionStarted
- [x] SceneTransitionVideoEnded
- [x] SourceFilterCreated
- [x] SourceFilterEnableStateChanged
- [x] SourceFilterListReindexed
- [x] SourceFilterNameChanged
- [x] SourceFilterRemoved
- [x] StreamStateChanged
- [x] StudioModeStateChanged
- [ ] VendorEvent
- [x] VirtualcamStateChanged

## Requests

- [x] BroadcastCustomEvent
- [ ] CallVendorRequest
- [x] CreateInput
- [x] CreateProfile
- [x] CreateScene
- [x] CreateSceneCollection
- [x] CreateSceneItem
- [x] CreateSourceFilter
- [x] DuplicateSceneItem
- [x] GetCurrentPreviewScene
- [x] GetCurrentProgramScene
- [ ] GetCurrentSceneTransition
- [ ] GetCurrentSceneTransitionCursor
- [x] GetGroupList
- [x] GetGroupSceneItemList
- [x] GetHotkeyList
- [x] GetInputAudioBalance
- [x] GetInputAudioMonitorType
- [x] GetInputAudioSyncOffset
- [x] GetInputAudioTracks
- [x] GetInputDefaultSettings
- [x] GetInputKindList
- [x] GetInputList
- [x] GetInputMute
- [x] GetInputPropertiesListPropertyItems
- [x] GetInputSettings
- [x] GetInputVolume
- [x] GetLastReplayBufferReplay
- [ ] GetMediaInputStatus
- [x] GetMonitorList
- [x] GetOutputList
- [x] GetOutputSettings
- [x] GetOutputStatus
- [x] GetPersistentData
- [x] GetProfileList
- [x] GetProfileParameter
- [x] GetRecordDirectory
- [x] GetRecordStatus
- [x] GetReplayBufferStatus
- [x] GetSceneCollectionList
- [x] GetSceneItemBlendMode
- [x] GetSceneItemEnabled
- [x] GetSceneItemId
- [x] GetSceneItemIndex
- [x] GetSceneItemList
- [x] GetSceneItemLocked
- [x] GetSceneItemTransform
- [x] GetSceneList
- [x] GetSceneSceneTransitionOverride
- [ ] GetSceneTransitionList
- [x] GetSourceActive
- [x] GetSourceFilter
- [x] GetSourceFilterDefaultSettings
- [x] GetSourceFilterList
- [x] GetSourceScreenshot
- [x] GetSpecialInputs
- [x] GetStats
- [x] GetStreamServiceSettings
- [x] GetStreamStatus
- [x] GetStudioModeEnabled
- [ ] GetTransitionKindList
- [x] GetVersion
- [x] GetVideoSettings
- [x] GetVirtualCamStatus
- [ ] OffsetMediaInputCursor
- [x] OpenInputFiltersDialog
- [x] OpenInputInteractDialog
- [x] OpenInputPropertiesDialog
- [x] OpenSourceProjector
- [x] OpenVideoMixProjector
- [x] PauseRecord
- [x] PressInputPropertiesButton
- [x] RemoveInput
- [x] RemoveProfile
- [x] RemoveScene
- [x] RemoveSceneItem
- [ ] RemoveSourceFilter
- [x] ResumeRecord
- [x] SaveReplayBuffer
- [x] SaveSourceScreenshot
- [x] SendStreamCaption
- [x] SetCurrentPreviewScene
- [x] SetCurrentProfile
- [x] SetCurrentProgramScene
- [x] SetCurrentSceneCollection
- [ ] SetCurrentSceneTransition
- [ ] SetCurrentSceneTransitionDuration
- [ ] SetCurrentSceneTransitionSettings
- [x] SetInputAudioBalance
- [x] SetInputAudioMonitorType
- [x] SetInputAudioSyncOffset
- [x] SetInputAudioTracks
- [x] SetInputMute
- [x] SetInputName
- [x] SetInputSettings
- [x] SetInputVolume
- [ ] SetMediaInputCursor
- [x] SetOutputSettings
- [x] SetPersistentData
- [x] SetProfileParameter
- [x] SetSceneItemBlendMode
- [x] SetSceneItemEnabled
- [x] SetSceneItemIndex
- [x] SetSceneItemLocked
- [x] SetSceneItemTransform
- [x] SetSceneName
- [x] SetSceneSceneTransitionOverride
- [x] SetSourceFilterEnabled
- [x] SetSourceFilterIndex
- [x] SetSourceFilterName
- [x] SetSourceFilterSettings
- [x] SetStreamServiceSettings
- [x] SetStudioModeEnabled
- [ ] SetTBarPosition
- [x] SetVideoSettings
- [ ] Sleep
- [x] StartOutput
- [x] StartRecord
- [x] StartReplayBuffer
- [x] StartStream
- [x] StartVirtualCam
- [x] StopOutput
- [x] StopRecord
- [x] StopReplayBuffer
- [x] StopStream
- [x] StopVirtualCam
- [x] ToggleInputMute
- [x] ToggleOutput
- [x] ToggleRecord
- [x] ToggleRecordPause
- [x] ToggleReplayBuffer
- [x] ToggleStream
- [x] ToggleVirtualCam
- [x] TriggerHotkeyByKeySequence
- [x] TriggerHotkeyByName
- [ ] TriggerMediaInputAction
- [ ] TriggerStudioModeTransition
