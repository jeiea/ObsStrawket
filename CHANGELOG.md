## 0.11.0
- Support RequestBatch.

  You can use it by passing `RequestBatch` to `RequestAsync`.

- Don't abort if received message is malformed.

## 0.10.0

- ! Stronly typed some requests and events.
  - `GetMediaInputStatus`
  - `MediaInputActionTriggered`
  - `TriggerMediaInputAction`
- Fix some incorrect enum value annotations.

## 0.9.0

- ! Strongly typed some requests and events.
  - `SourceFilterListReindexed`
  - `GetSourceFilterList`
  - `GetSceneTransitionList`
- Make `SetProfileParameterAsync` accept null value.
- Change DebugType to embedded

## 0.8.0

- ! Fix typo in `GetRecordStatus` response.
- Try integrating SourceLink.

## 0.7.0

- ! Strongly typed some requests and events.
  - `SceneItemListReindexed`
  - `GetSceneItemList`
  - `GetGroupSceneItemList`
  - `GetSceneItemBlendMode`
  - `SetSceneItemBlendMode`
- Fix out of order connect / disconnect events.

## 0.6.0

- Fix some doc comment issues.
- Support more requests (including `GetSceneSceneTransitionOverride`).

## 0.5.0

- Support `GetOutputList`.

## 0.4.0

- (**BREAKING**) Add `VideoMixType`, `StreamServiceType` enum.
- (**BREAKING**) `SceneListChanged.Scenes` is typed.
- Fix invalid xml doc comments.
- Less convincing fix for out of order connect / disconnect events.

## 0.3.0

- Remove SaveSourceScreenshot response's `ImageData` (upstream document is invalid).
- Verify more requests.

## 0.2.0

- Add `ObsClientSocket.Connected` event.
- Rename `ObsClientSocket.Closed` event to `Disconnected`.
- Catch event handler's exception.

## 0.1.2

- Add `(ChannelReader<IEvent>)ObsClientSocket.Events`. It can be used when `new ObsClientSocket(useChannel: true)`.

# 0.1.0

- Initial release
