# ObsStrawket

[![prerelease shield](https://img.shields.io/nuget/vpre/ObsStrawket)](https://www.nuget.org/packages/ObsStrawket) ![Downloads count](https://img.shields.io/nuget/dt/ObsStrawket)

Personal .NET implementation of
[obs-websocket](https://github.com/obsproject/obs-websocket) protocol v5.

## Can I use this in production?

No. I verified few events and requests. And I can change interfaces later.

But I won't stop you from using this if this works for you.

## Difference to [obs-websocket-dotnet](https://github.com/BarRaider/obs-websocket-dotnet)

|              |                        ObsStrawket                       |         obs-websocket-dotnet        |
|:------------:|:--------------------------------------------------------:|:-----------------------------------:|
| Dependencies | System.Net.WebSockets<br>MessagePack<br>Nerdbank.Streams | WebSocket.Client<br>Newtonsoft.Json |
| Method type  | Asynchronous                                             | Synchronous                         |

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

- [ ] CurrentPreviewSceneChanged
- [x] CurrentProfileChanged
- [x] CurrentProfileChanging
- [ ] CurrentProgramSceneChanged
- [ ] CurrentSceneCollectionChanged
- [ ] CurrentSceneCollectionChanging
- [ ] CurrentSceneTransitionChanged
- [ ] CurrentSceneTransitionDurationChanged
- [x] ExitStarted
- [ ] InputActiveStateChanged
- [ ] InputAudioBalanceChanged
- [ ] InputAudioMonitorTypeChanged
- [ ] InputAudioSyncOffsetChanged
- [ ] InputAudioTracksChanged
- [ ] InputCreated
- [ ] InputMuteStateChanged
- [ ] InputNameChanged
- [ ] InputRemoved
- [ ] InputShowStateChanged
- [ ] InputVolumeChanged
- [ ] InputVolumeMeters
- [ ] MediaInputActionTriggered
- [ ] MediaInputPlaybackEnded
- [ ] MediaInputPlaybackStarted
- [ ] ProfileListChanged
- [x] RecordStateChanged
- [ ] ReplayBufferSaved
- [ ] ReplayBufferStateChanged
- [ ] SceneCollectionListChanged
- [ ] SceneCreated
- [ ] SceneItemCreated
- [ ] SceneItemEnableStateChanged
- [ ] SceneItemListReindexed
- [ ] SceneItemLockStateChanged
- [ ] SceneItemRemoved
- [ ] SceneItemSelected
- [ ] SceneItemTransformChanged
- [ ] SceneListChanged
- [ ] SceneNameChanged
- [ ] SceneRemoved
- [ ] SceneTransitionEnded
- [ ] SceneTransitionStarted
- [ ] SceneTransitionVideoEnded
- [ ] SourceFilterCreated
- [ ] SourceFilterEnableStateChanged
- [ ] SourceFilterListReindexed
- [ ] SourceFilterNameChanged
- [ ] SourceFilterRemoved
- [ ] StreamStateChanged
- [ ] StudioModeStateChanged
- [ ] VendorEvent
- [ ] VirtualcamStateChanged

## Requests

- [ ] BroadcastCustomEvent
- [ ] CallVendorRequest
- [ ] CreateInput
- [x] CreateProfile
- [ ] CreateScene
- [ ] CreateSceneCollection
- [ ] CreateSceneItem
- [ ] CreateSourceFilter
- [ ] DuplicateSceneItem
- [ ] GetCurrentPreviewScene
- [ ] GetCurrentProgramScene
- [ ] GetCurrentSceneTransition
- [ ] GetCurrentSceneTransitionCursor
- [ ] GetGroupList
- [ ] GetGroupSceneItemList
- [ ] GetHotkeyList
- [ ] GetInputAudioBalance
- [ ] GetInputAudioMonitorType
- [ ] GetInputAudioSyncOffset
- [ ] GetInputAudioTracks
- [ ] GetInputDefaultSettings
- [ ] GetInputKindList
- [ ] GetInputList
- [ ] GetInputMute
- [ ] GetInputPropertiesListPropertyItems
- [ ] GetInputSettings
- [ ] GetInputVolume
- [ ] GetLastReplayBufferReplay
- [ ] GetMediaInputStatus
- [ ] GetMonitorList
- [ ] GetOutputList
- [ ] GetOutputSettings
- [ ] GetOutputStatus
- [x] GetPersistentData
- [ ] GetProfileList
- [ ] GetProfileParameter
- [x] GetRecordDirectory
- [x] GetRecordStatus
- [ ] GetReplayBufferStatus
- [ ] GetSceneCollectionList
- [ ] GetSceneItemBlendMode
- [ ] GetSceneItemEnabled
- [ ] GetSceneItemId
- [ ] GetSceneItemIndex
- [ ] GetSceneItemList
- [ ] GetSceneItemLocked
- [ ] GetSceneItemTransform
- [ ] GetSceneList
- [ ] GetSceneSceneTransitionOverride
- [ ] GetSceneTransitionList
- [ ] GetSourceActive
- [ ] GetSourceFilter
- [ ] GetSourceFilterDefaultSettings
- [ ] GetSourceFilterList
- [ ] GetSourceScreenshot
- [ ] GetSpecialInputs
- [x] GetStats
- [ ] GetStreamServiceSettings
- [ ] GetStreamStatus
- [ ] GetStudioModeEnabled
- [ ] GetTransitionKindList
- [x] GetVersion
- [ ] GetVideoSettings
- [ ] GetVirtualCamStatus
- [ ] OffsetMediaInputCursor
- [ ] OpenInputFiltersDialog
- [ ] OpenInputInteractDialog
- [ ] OpenInputPropertiesDialog
- [ ] OpenSourceProjector
- [ ] OpenVideoMixProjector
- [ ] PauseRecord
- [ ] PressInputPropertiesButton
- [ ] RemoveInput
- [x] RemoveProfile
- [ ] RemoveScene
- [ ] RemoveSceneItem
- [ ] RemoveSourceFilter
- [ ] ResumeRecord
- [ ] SaveReplayBuffer
- [ ] SaveSourceScreenshot
- [ ] SendStreamCaption
- [ ] SetCurrentPreviewScene
- [x] SetCurrentProfile
- [ ] SetCurrentProgramScene
- [ ] SetCurrentSceneCollection
- [ ] SetCurrentSceneTransition
- [ ] SetCurrentSceneTransitionDuration
- [ ] SetCurrentSceneTransitionSettings
- [ ] SetInputAudioBalance
- [ ] SetInputAudioMonitorType
- [ ] SetInputAudioSyncOffset
- [ ] SetInputAudioTracks
- [ ] SetInputMute
- [ ] SetInputName
- [ ] SetInputSettings
- [ ] SetInputVolume
- [ ] SetMediaInputCursor
- [ ] SetOutputSettings
- [x] SetPersistentData
- [ ] SetProfileParameter
- [ ] SetSceneItemBlendMode
- [ ] SetSceneItemEnabled
- [ ] SetSceneItemIndex
- [ ] SetSceneItemLocked
- [ ] SetSceneItemTransform
- [ ] SetSceneName
- [ ] SetSceneSceneTransitionOverride
- [ ] SetSourceFilterEnabled
- [ ] SetSourceFilterIndex
- [ ] SetSourceFilterName
- [ ] SetSourceFilterSettings
- [ ] SetStreamServiceSettings
- [ ] SetStudioModeEnabled
- [ ] SetTBarPosition
- [ ] SetVideoSettings
- [ ] Sleep
- [ ] StartOutput
- [x] StartRecord
- [ ] StartReplayBuffer
- [ ] StartStream
- [ ] StartVirtualCam
- [ ] StopOutput
- [x] StopRecord
- [ ] StopReplayBuffer
- [ ] StopStream
- [ ] StopVirtualCam
- [ ] ToggleInputMute
- [ ] ToggleOutput
- [x] ToggleRecord
- [ ] ToggleRecordPause
- [ ] ToggleReplayBuffer
- [ ] ToggleStream
- [ ] ToggleVirtualCam
- [x] TriggerHotkeyByKeySequence
- [x] TriggerHotkeyByName
- [ ] TriggerMediaInputAction
- [ ] TriggerStudioModeTransition