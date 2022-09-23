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
- [ ] ProfileListChanged
- [x] RecordStateChanged
- [ ] ReplayBufferSaved
- [ ] ReplayBufferStateChanged
- [x] SceneCollectionListChanged
- [x] SceneCreated
- [x] SceneItemCreated
- [ ] SceneItemEnableStateChanged
- [ ] SceneItemListReindexed
- [ ] SceneItemLockStateChanged
- [x] SceneItemRemoved
- [x] SceneItemSelected
- [ ] SceneItemTransformChanged
- [x] SceneListChanged
- [x] SceneNameChanged
- [x] SceneRemoved
- [x] SceneTransitionEnded
- [x] SceneTransitionStarted
- [x] SceneTransitionVideoEnded
- [ ] SourceFilterCreated
- [ ] SourceFilterEnableStateChanged
- [ ] SourceFilterListReindexed
- [ ] SourceFilterNameChanged
- [ ] SourceFilterRemoved
- [x] StreamStateChanged
- [x] StudioModeStateChanged
- [ ] VendorEvent
- [ ] VirtualcamStateChanged

## Requests

- [x] BroadcastCustomEvent
- [ ] CallVendorRequest
- [x] CreateInput
- [x] CreateProfile
- [x] CreateScene
- [x] CreateSceneCollection
- [ ] CreateSceneItem
- [ ] CreateSourceFilter
- [ ] DuplicateSceneItem
- [x] GetCurrentPreviewScene
- [x] GetCurrentProgramScene
- [ ] GetCurrentSceneTransition
- [ ] GetCurrentSceneTransitionCursor
- [x] GetGroupList
- [ ] GetGroupSceneItemList
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
- [ ] GetLastReplayBufferReplay
- [ ] GetMediaInputStatus
- [x] GetMonitorList
- [x] GetOutputList
- [ ] GetOutputSettings
- [ ] GetOutputStatus
- [x] GetPersistentData
- [x] GetProfileList
- [x] GetProfileParameter
- [x] GetRecordDirectory
- [x] GetRecordStatus
- [ ] GetReplayBufferStatus
- [x] GetSceneCollectionList
- [ ] GetSceneItemBlendMode
- [ ] GetSceneItemEnabled
- [ ] GetSceneItemId
- [ ] GetSceneItemIndex
- [ ] GetSceneItemList
- [ ] GetSceneItemLocked
- [ ] GetSceneItemTransform
- [x] GetSceneList
- [x] GetSceneSceneTransitionOverride
- [ ] GetSceneTransitionList
- [x] GetSourceActive
- [ ] GetSourceFilter
- [ ] GetSourceFilterDefaultSettings
- [ ] GetSourceFilterList
- [x] GetSourceScreenshot
- [x] GetSpecialInputs
- [x] GetStats
- [x] GetStreamServiceSettings
- [x] GetStreamStatus
- [x] GetStudioModeEnabled
- [ ] GetTransitionKindList
- [x] GetVersion
- [x] GetVideoSettings
- [ ] GetVirtualCamStatus
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
- [ ] RemoveSceneItem
- [ ] RemoveSourceFilter
- [x] ResumeRecord
- [ ] SaveReplayBuffer
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
- [ ] SetOutputSettings
- [x] SetPersistentData
- [x] SetProfileParameter
- [ ] SetSceneItemBlendMode
- [ ] SetSceneItemEnabled
- [ ] SetSceneItemIndex
- [ ] SetSceneItemLocked
- [ ] SetSceneItemTransform
- [x] SetSceneName
- [x] SetSceneSceneTransitionOverride
- [ ] SetSourceFilterEnabled
- [ ] SetSourceFilterIndex
- [ ] SetSourceFilterName
- [ ] SetSourceFilterSettings
- [x] SetStreamServiceSettings
- [x] SetStudioModeEnabled
- [ ] SetTBarPosition
- [x] SetVideoSettings
- [ ] Sleep
- [ ] StartOutput
- [x] StartRecord
- [ ] StartReplayBuffer
- [x] StartStream
- [ ] StartVirtualCam
- [ ] StopOutput
- [x] StopRecord
- [ ] StopReplayBuffer
- [x] StopStream
- [ ] StopVirtualCam
- [x] ToggleInputMute
- [ ] ToggleOutput
- [x] ToggleRecord
- [x] ToggleRecordPause
- [ ] ToggleReplayBuffer
- [x] ToggleStream
- [ ] ToggleVirtualCam
- [x] TriggerHotkeyByKeySequence
- [x] TriggerHotkeyByName
- [ ] TriggerMediaInputAction
- [ ] TriggerStudioModeTransition
