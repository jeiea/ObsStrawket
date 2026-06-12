## 0.15.0-alpha

- ! Replace MessagePack serialization with System.Text.Json.
  Generated data types now carry `System.Text.Json.Serialization` attributes
  and the MessagePack dependency is gone.
- ! Replace dynamic protocol dictionaries such as settings, transforms, and
  vendor data from `Dictionary<string, object?>` to
  `Dictionary<string, JsonElement?>`.
- ! Remove `RequestStatusCode` enum and rename `RequestStatus` class to
  `RequestStatusObject`. Status codes now use
  `ObsStrawket.DataTypes.Predefineds.RequestStatus`.
- ! Remove default value assignments from optional fields.
  Optional strings are `null` instead of `""` when absent.
- ! Change `Output.Width` and `Output.Height` from `int` to `uint`, matching
  the unsigned 32-bit values returned by OBS.
- ! Refresh generated request methods for the OBS 32 protocol. Many methods
  now accept UUIDs and canvas UUIDs, and required arguments precede optional
  name/UUID selectors. Positional call sites may need updating.
  ```csharp
  // Before
  SetInputMuteAsync("Mic", true);
  // After
  SetInputMuteAsync(true, inputName: "Mic");
  ```
- ! Remove the `ILogger` constructor parameter and
  Microsoft.Extensions.Logging dependency. Subscribe to `PipelineEvent` for
  structured diagnostics instead.
- Multi-target `netstandard2.0` and `net8.0`. On .NET 8+,
  System.Text.Json and System.Threading.Channels come from the framework
  instead of package downloads. The `netstandard2.0` target uses version
  10.0.9 of those packages.
- Remove System.IO.Pipelines and System.Net.WebSockets package dependencies.
- Fix pending requests hanging forever when the connection dies during close.
- Send outgoing JSON as WebSocket text frames instead of binary frames.
- Reflect upstream obs-websocket changes and regenerate protocol types.

## 0.14.0

- ! Change `ObsClientSocket.Disconnected` parameter type from `object` to `Exception?`.
- ! Calling `ConnectAsync` or `CloseAsync` during a request or event subscription
  now throws an `OperationCancelledException`, whereas previously
  it might have thrown an `ObsWebSocketException`.
- Prevent CloseAsync from emitting Disconnected event before it exits.
- Fix integer overflow with some response field.
- Fix throwing at second `Dispose()`.

## 0.13.0

- ! Change `ObsStrawket.DataTypes.OutputState` enum to
  `ObsStrawket.DataTypes.Predefineds.ObsOutputState`.
- Update dependencies.
- Reflect upstream changes.
  - Add `ScreenshotSaved`, `CustomEvent` event.
  - Add `SetRecordDirectory`.
    - **CAUTION: This is not usable in stable OBS yet!**
  - Fix `Sleep` request parameter nullability.

## 0.12.0

- ! Rename `UnexpectedProtocolException` to `UnexpectedResponseException`.
- Fix throwing other exception instead of `AuthenticationFailureException`
  when failed to authenticate.

## 0.11.0

- ! Promote boxed numeric type in `object` to int, long, ulong or double.
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

- Add `(ChannelReader<IEvent>)ObsClientSocket.Events`.It can be used when `new ObsClientSocket(useChannel: true)`.

# 0.1.0

- Initial release
