## 0.6.0

- Fix some doc comment issues.
- Support more requests (including `GetSceneSceneTransitionOverride`).

## 0.5.0

- Support `GetOutputList`.

## 0.4.0

- Add `VideoMixType`, `StreamServiceType` enum.
- `SceneListChanged.Scenes` is typed.
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