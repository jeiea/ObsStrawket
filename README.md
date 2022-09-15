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
- [x] CurrentProgramSceneChanged
- [x] CurrentSceneCollectionChanged
- [x] CurrentSceneCollectionChanging
- [ ] CurrentSceneTransitionChanged
- [ ] CurrentSceneTransitionDurationChanged
- [x] ExitStarted
- [ ] InputActiveStateChanged
- [ ] InputAudioBalanceChanged
- [ ] InputAudioMonitorTypeChanged
- [ ] InputAudioSyncOffsetChanged
- [ ] InputAudioTracksChanged
- [x] InputCreated
- [ ] InputMuteStateChanged
- [ ] InputNameChanged
- [x] InputRemoved
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
- [ ] SceneNameChanged
- [x] SceneRemoved
- [x] SceneTransitionEnded
- [x] SceneTransitionStarted
- [x] SceneTransitionVideoEnded
- [ ] SourceFilterCreated
- [ ] SourceFilterEnableStateChanged
- [ ] SourceFilterListReindexed
- [ ] SourceFilterNameChanged
- [ ] SourceFilterRemoved
- [ ] StreamStateChanged
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
- [ ] GetCurrentPreviewScene
- [ ] GetCurrentProgramScene
- [ ] GetCurrentSceneTransition
- [ ] GetCurrentSceneTransitionCursor
- [ ] GetGroupList
- [ ] GetGroupSceneItemList
- [x] GetHotkeyList
- [ ] GetInputAudioBalance
- [ ] GetInputAudioMonitorType
- [ ] GetInputAudioSyncOffset
- [ ] GetInputAudioTracks
- [ ] GetInputDefaultSettings
- [ ] GetInputKindList
- [x] GetInputList
- [ ] GetInputMute
- [ ] GetInputPropertiesListPropertyItems
- [ ] GetInputSettings
- [ ] GetInputVolume
- [ ] GetLastReplayBufferReplay
- [ ] GetMediaInputStatus
- [x] GetMonitorList
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
- [x] GetSceneList
- [ ] GetSceneSceneTransitionOverride
- [ ] GetSceneTransitionList
- [x] GetSourceActive
- [ ] GetSourceFilter
- [ ] GetSourceFilterDefaultSettings
- [ ] GetSourceFilterList
- [x] GetSourceScreenshot
- [ ] GetSpecialInputs
- [x] GetStats
- [ ] GetStreamServiceSettings
- [ ] GetStreamStatus
- [x] GetStudioModeEnabled
- [ ] GetTransitionKindList
- [x] GetVersion
- [ ] GetVideoSettings
- [ ] GetVirtualCamStatus
- [ ] OffsetMediaInputCursor
- [x] OpenInputFiltersDialog
- [x] OpenInputInteractDialog
- [x] OpenInputPropertiesDialog
- [x] OpenSourceProjector
- [x] OpenVideoMixProjector
- [x] PauseRecord
- [ ] PressInputPropertiesButton
- [x] RemoveInput
- [x] RemoveProfile
- [x] RemoveScene
- [ ] RemoveSceneItem
- [ ] RemoveSourceFilter
- [x] ResumeRecord
- [ ] SaveReplayBuffer
- [x] SaveSourceScreenshot
- [ ] SendStreamCaption
- [ ] SetCurrentPreviewScene
- [x] SetCurrentProfile
- [x] SetCurrentProgramScene
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
- [x] SetStudioModeEnabled
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
- [x] ToggleRecordPause
- [ ] ToggleReplayBuffer
- [ ] ToggleStream
- [ ] ToggleVirtualCam
- [x] TriggerHotkeyByKeySequence
- [x] TriggerHotkeyByName
- [ ] TriggerMediaInputAction
- [ ] TriggerStudioModeTransition