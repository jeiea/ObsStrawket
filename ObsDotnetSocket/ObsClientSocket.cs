namespace ObsDotnetSocket {
  using ObsDotnetSocket.DataTypes;
  using ObsDotnetSocket.DataTypes.Predefineds;
  using System;
  using System.Collections.Generic;
  using System.Net.WebSockets;
  using System.Threading;
  using System.Threading.Tasks;

  public class ObsClientSocket : IDisposable {
    #region Events
    public event Action<IEvent> Event = delegate { };

    public event Action<GeneralEvent> GeneralEvent = delegate { };
    public event Action<ExitStarted> ExitStarted = delegate { };
    public event Action<VendorEvent> VendorEvent = delegate { };

    public event Action<ConfigEvent> ConfigEvent = delegate { };
    public event Action<CurrentSceneCollectionChanging> CurrentSceneCollectionChanging = delegate { };
    public event Action<CurrentSceneCollectionChanged> CurrentSceneCollectionChanged = delegate { };
    public event Action<SceneCollectionListChanged> SceneCollectionListChanged = delegate { };
    public event Action<CurrentProfileChanging> CurrentProfileChanging = delegate { };
    public event Action<CurrentProfileChanged> CurrentProfileChanged = delegate { };
    public event Action<ProfileListChanged> ProfileListChanged = delegate { };

    public event Action<ScenesEvent> ScenesEvent = delegate { };
    public event Action<SceneCreated> SceneCreated = delegate { };
    public event Action<SceneRemoved> SceneRemoved = delegate { };
    public event Action<SceneNameChanged> SceneNameChanged = delegate { };
    public event Action<CurrentProgramSceneChanged> CurrentProgramSceneChanged = delegate { };
    public event Action<CurrentPreviewSceneChanged> CurrentPreviewSceneChanged = delegate { };
    public event Action<SceneListChanged> SceneListChanged = delegate { };

    public event Action<InputsEvent> InputsEvent = delegate { };
    public event Action<InputCreated> InputCreated = delegate { };
    public event Action<InputRemoved> InputRemoved = delegate { };
    public event Action<InputNameChanged> InputNameChanged = delegate { };
    public event Action<InputActiveStateChanged> InputActiveStateChanged = delegate { };
    public event Action<InputShowStateChanged> InputShowStateChanged = delegate { };
    public event Action<InputMuteStateChanged> InputMuteStateChanged = delegate { };
    public event Action<InputVolumeChanged> InputVolumeChanged = delegate { };
    public event Action<InputAudioBalanceChanged> InputAudioBalanceChanged = delegate { };
    public event Action<InputAudioSyncOffsetChanged> InputAudioSyncOffsetChanged = delegate { };
    public event Action<InputAudioTracksChanged> InputAudioTracksChanged = delegate { };
    public event Action<InputAudioMonitorTypeChanged> InputAudioMonitorTypeChanged = delegate { };
    public event Action<InputVolumeMeters> InputVolumeMeters = delegate { };

    public event Action<TransitionsEvent> TransitionsEvent = delegate { };
    public event Action<CurrentSceneTransitionChanged> CurrentSceneTransitionChanged = delegate { };
    public event Action<CurrentSceneTransitionDurationChanged> CurrentSceneTransitionDurationChanged = delegate { };
    public event Action<SceneTransitionStarted> SceneTransitionStarted = delegate { };
    public event Action<SceneTransitionEnded> SceneTransitionEnded = delegate { };
    public event Action<SceneTransitionVideoEnded> SceneTransitionVideoEnded = delegate { };

    public event Action<FiltersEvent> FiltersEvent = delegate { };
    public event Action<SourceFilterListReindexed> SourceFilterListReindexed = delegate { };
    public event Action<SourceFilterCreated> SourceFilterCreated = delegate { };
    public event Action<SourceFilterRemoved> SourceFilterRemoved = delegate { };
    public event Action<SourceFilterNameChanged> SourceFilterNameChanged = delegate { };
    public event Action<SourceFilterEnableStateChanged> SourceFilterEnableStateChanged = delegate { };

    public event Action<SceneItemsEvent> SceneItemsEvent = delegate { };
    public event Action<SceneItemCreated> SceneItemCreated = delegate { };
    public event Action<SceneItemRemoved> SceneItemRemoved = delegate { };
    public event Action<SceneItemListReindexed> SceneItemListReindexed = delegate { };
    public event Action<SceneItemEnableStateChanged> SceneItemEnableStateChanged = delegate { };
    public event Action<SceneItemLockStateChanged> SceneItemLockStateChanged = delegate { };
    public event Action<SceneItemSelected> SceneItemSelected = delegate { };
    public event Action<SceneItemTransformChanged> SceneItemTransformChanged = delegate { };

    public event Action<OutputsEvent> OutputsEvent = delegate { };
    public event Action<StreamStateChanged> StreamStateChanged = delegate { };
    public event Action<RecordStateChanged> RecordStateChanged = delegate { };
    public event Action<ReplayBufferStateChanged> ReplayBufferStateChanged = delegate { };
    public event Action<VirtualcamStateChanged> VirtualcamStateChanged = delegate { };
    public event Action<ReplayBufferSaved> ReplayBufferSaved = delegate { };

    public event Action<MediaInputsEvent> MediaInputsEvent = delegate { };
    public event Action<MediaInputPlaybackStarted> MediaInputPlaybackStarted = delegate { };
    public event Action<MediaInputPlaybackEnded> MediaInputPlaybackEnded = delegate { };
    public event Action<MediaInputActionTriggered> MediaInputActionTriggered = delegate { };

    public event Action<UiEvent> UiEvent = delegate { };
    public event Action<StudioModeStateChanged> StudioModeStateChanged = delegate { };
    #endregion

    public event Action<object> Closed {
      add => _clientSocket.Closed += value;
      remove => _clientSocket.Closed -= value;
    }

    private readonly ClientSocket _clientSocket;

    public string? CloseDescription { get => _clientSocket.CloseDescription; }

    public bool IsConnected { get => _clientSocket.IsConnected; }

    public ObsClientSocket() {
      _clientSocket = new ClientSocket();
      _clientSocket.Event += Dispatch;
    }

    public Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken cancellation = default
    ) => _clientSocket.ConnectAsync(uri, password, events, cancellation);

    public Task CloseAsync() => _clientSocket.CloseAsync();

    public Task<RequestResponse?> RequestAsync(IRequest request, CancellationToken cancellation = default)
      => _clientSocket.RequestAsync(request, cancellation);

    public void Dispose() {
      _clientSocket.Dispose();
    }

    #region Requests

    /// <summary>
    /// Gets data about the current plugin and RPC version.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetVersionResponse> GetVersionAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetVersion() { }, cancellation).ConfigureAwait(false) as GetVersionResponse)!;
    }

    /// <summary>
    /// Gets statistics about OBS, obs-websocket, and the current session.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetStatsResponse> GetStatsAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetStats() { }, cancellation).ConfigureAwait(false) as GetStatsResponse)!;
    }

    /// <summary>
    /// Broadcasts a `CustomEvent` to all WebSocket clients. Receivers are clients which are identified and subscribed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=eventData>Data payload to emit to all receivers</param>
    public Task BroadcastCustomEventAsync(Dictionary<string, object?> eventData, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new BroadcastCustomEvent() { EventData = eventData }, cancellation);
    }

    /// <summary>
    /// Call a request registered to a vendor.<br />
    /// <br />
    /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
    /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=vendorName>Name of the vendor to use</param>
    /// <param name=requestType>The request type to call</param>
    /// <param name=requestData>Object containing appropriate request data</param>
    public async Task<CallVendorRequestResponse> CallVendorRequestAsync(string vendorName, string requestType, Dictionary<string, object?>? requestData = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CallVendorRequest() { VendorName = vendorName, VendorRequestType = requestType, RequestData = requestData }, cancellation).ConfigureAwait(false) as CallVendorRequestResponse)!;
    }

    /// <summary>
    /// Gets an array of all hotkey names in OBS<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetHotkeyListResponse> GetHotkeyListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetHotkeyList() { }, cancellation).ConfigureAwait(false) as GetHotkeyListResponse)!;
    }

    /// <summary>
    /// Triggers a hotkey using its name. See `GetHotkeyList`<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=hotkeyName>Name of the hotkey to trigger</param>
    public Task TriggerHotkeyByNameAsync(string hotkeyName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new TriggerHotkeyByName() { HotkeyName = hotkeyName }, cancellation);
    }

    /// <summary>
    /// Triggers a hotkey using a sequence of keys.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=keyId>The OBS key ID to use. See https://github.com/obsproject/obs-studio/blob/master/libobs/obs-hotkeys.h</param>
    /// <param name=keyModifiers>Object containing key modifiers to apply</param>
    /// <param name=keyModifiers.shift>Press Shift</param>
    /// <param name=keyModifiers.control>Press CTRL</param>
    /// <param name=keyModifiers.alt>Press ALT</param>
    /// <param name=keyModifiers.command>Press CMD (Mac)</param>
    public Task TriggerHotkeyByKeySequenceAsync(string? keyId = null, bool? shift = null, bool? control = null, bool? alt = null, bool? command = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new TriggerHotkeyByKeySequence() { KeyId = keyId, KeyModifiers = new KeyModifiers() { Shift = shift, Control = control, Alt = alt, Command = command } }, cancellation);
    }

    /// <summary>
    /// Sleeps for a time duration or number of frames. Only available in request batches with types `SERIAL_REALTIME` or `SERIAL_FRAME`.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sleepMillis>Number of milliseconds to sleep for (if `SERIAL_REALTIME` mode)</param>
    /// <param name=sleepFrames>Number of frames to sleep for (if `SERIAL_FRAME` mode)</param>
    public Task SleepAsync(int sleepMillis, int sleepFrames, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new Sleep() { SleepMillis = sleepMillis, SleepFrames = sleepFrames }, cancellation);
    }

    /// <summary>
    /// Gets the value of a "slot" from the selected persistent data realm.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=realm>The data realm to select. `OBS_WEBSOCKET_DATA_REALM_GLOBAL` or `OBS_WEBSOCKET_DATA_REALM_PROFILE`</param>
    /// <param name=slotName>The name of the slot to retrieve data from</param>
    public async Task<GetPersistentDataResponse> GetPersistentDataAsync(string realm, string slotName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetPersistentData() { Realm = realm, SlotName = slotName }, cancellation).ConfigureAwait(false) as GetPersistentDataResponse)!;
    }

    /// <summary>
    /// Sets the value of a "slot" from the selected persistent data realm.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=realm>The data realm to select. `OBS_WEBSOCKET_DATA_REALM_GLOBAL` or `OBS_WEBSOCKET_DATA_REALM_PROFILE`</param>
    /// <param name=slotName>The name of the slot to retrieve data from</param>
    /// <param name=slotValue>The value to apply to the slot</param>
    public Task SetPersistentDataAsync(string realm, string slotName, object? slotValue, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetPersistentData() { Realm = realm, SlotName = slotName, SlotValue = slotValue }, cancellation);
    }

    /// <summary>
    /// Gets an array of all scene collections<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetSceneCollectionListResponse> GetSceneCollectionListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneCollectionList() { }, cancellation).ConfigureAwait(false) as GetSceneCollectionListResponse)!;
    }

    /// <summary>
    /// Switches to a scene collection.<br />
    /// <br />
    /// Note: This will block until the collection has finished changing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneCollectionName>Name of the scene collection to switch to</param>
    public Task SetCurrentSceneCollectionAsync(string sceneCollectionName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetCurrentSceneCollection() { SceneCollectionName = sceneCollectionName }, cancellation);
    }

    /// <summary>
    /// Creates a new scene collection, switching to it in the process.<br />
    /// <br />
    /// Note: This will block until the collection has finished changing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneCollectionName>Name for the new scene collection</param>
    public Task CreateSceneCollectionAsync(string sceneCollectionName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new CreateSceneCollection() { SceneCollectionName = sceneCollectionName }, cancellation);
    }

    /// <summary>
    /// Gets an array of all profiles<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetProfileListResponse> GetProfileListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetProfileList() { }, cancellation).ConfigureAwait(false) as GetProfileListResponse)!;
    }

    /// <summary>
    /// Switches to a profile.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=profileName>Name of the profile to switch to</param>
    public Task SetCurrentProfileAsync(string profileName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetCurrentProfile() { ProfileName = profileName }, cancellation);
    }

    /// <summary>
    /// Creates a new profile, switching to it in the process<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=profileName>Name for the new profile</param>
    public Task CreateProfileAsync(string profileName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new CreateProfile() { ProfileName = profileName }, cancellation);
    }

    /// <summary>
    /// Removes a profile. If the current profile is chosen, it will change to a different profile first.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=profileName>Name of the profile to remove</param>
    public Task RemoveProfileAsync(string profileName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new RemoveProfile() { ProfileName = profileName }, cancellation);
    }

    /// <summary>
    /// Gets a parameter from the current profile's configuration.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=parameterCategory>Category of the parameter to get</param>
    /// <param name=parameterName>Name of the parameter to get</param>
    public async Task<GetProfileParameterResponse> GetProfileParameterAsync(string parameterCategory, string parameterName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetProfileParameter() { ParameterCategory = parameterCategory, ParameterName = parameterName }, cancellation).ConfigureAwait(false) as GetProfileParameterResponse)!;
    }

    /// <summary>
    /// Sets the value of a parameter in the current profile's configuration.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=parameterCategory>Category of the parameter to set</param>
    /// <param name=parameterName>Name of the parameter to set</param>
    /// <param name=parameterValue>Value of the parameter to set. Use `null` to delete</param>
    public Task SetProfileParameterAsync(string parameterCategory, string parameterName, string parameterValue, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetProfileParameter() { ParameterCategory = parameterCategory, ParameterName = parameterName, ParameterValue = parameterValue }, cancellation);
    }

    /// <summary>
    /// Gets the current video settings.<br />
    /// <br />
    /// Note: To get the true FPS value, divide the FPS numerator by the FPS denominator. Example: `60000/1001`<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetVideoSettingsResponse> GetVideoSettingsAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetVideoSettings() { }, cancellation).ConfigureAwait(false) as GetVideoSettingsResponse)!;
    }

    /// <summary>
    /// Sets the current video settings.<br />
    /// <br />
    /// Note: Fields must be specified in pairs. For example, you cannot set only `baseWidth` without needing to specify `baseHeight`.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=fpsNumerator>Numerator of the fractional FPS value</param>
    /// <param name=fpsDenominator>Denominator of the fractional FPS value</param>
    /// <param name=baseWidth>Width of the base (canvas) resolution in pixels</param>
    /// <param name=baseHeight>Height of the base (canvas) resolution in pixels</param>
    /// <param name=outputWidth>Width of the output resolution in pixels</param>
    /// <param name=outputHeight>Height of the output resolution in pixels</param>
    public Task SetVideoSettingsAsync(int? fpsNumerator = null, int? fpsDenominator = null, int? baseWidth = null, int? baseHeight = null, int? outputWidth = null, int? outputHeight = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetVideoSettings() { FpsNumerator = fpsNumerator, FpsDenominator = fpsDenominator, BaseWidth = baseWidth, BaseHeight = baseHeight, OutputWidth = outputWidth, OutputHeight = outputHeight }, cancellation);
    }

    /// <summary>
    /// Gets the current stream service settings (stream destination).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetStreamServiceSettingsResponse> GetStreamServiceSettingsAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetStreamServiceSettings() { }, cancellation).ConfigureAwait(false) as GetStreamServiceSettingsResponse)!;
    }

    /// <summary>
    /// Sets the current stream service settings (stream destination).<br />
    /// <br />
    /// Note: Simple RTMP settings can be set with type `rtmp_custom` and the settings fields `server` and `key`.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=streamServiceType>Type of stream service to apply. Example: `rtmp_common` or `rtmp_custom`</param>
    /// <param name=streamServiceSettings>Settings to apply to the service</param>
    public Task SetStreamServiceSettingsAsync(string streamServiceType, Dictionary<string, object?> streamServiceSettings, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetStreamServiceSettings() { StreamServiceType = streamServiceType, StreamServiceSettings = streamServiceSettings }, cancellation);
    }

    /// <summary>
    /// Gets the current directory that the record output is set to.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetRecordDirectoryResponse> GetRecordDirectoryAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetRecordDirectory() { }, cancellation).ConfigureAwait(false) as GetRecordDirectoryResponse)!;
    }

    /// <summary>
    /// Gets the active and show state of a source.<br />
    /// <br />
    /// **Compatible with inputs and scenes.**<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source to get the active state of</param>
    public async Task<GetSourceActiveResponse> GetSourceActiveAsync(string sourceName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceActive() { SourceName = sourceName }, cancellation).ConfigureAwait(false) as GetSourceActiveResponse)!;
    }

    /// <summary>
    /// Gets a Base64-encoded screenshot of a source.<br />
    /// <br />
    /// The `imageWidth` and `imageHeight` parameters are treated as "scale to inner", meaning the smallest ratio will be used and the aspect ratio of the original resolution is kept.<br />
    /// If `imageWidth` and `imageHeight` are not specified, the compressed image will use the full resolution of the source.<br />
    /// <br />
    /// **Compatible with inputs and scenes.**<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source to take a screenshot of</param>
    /// <param name=imageFormat>Image compression format to use. Use `GetVersion` to get compatible image formats</param>
    /// <param name=imageWidth>Width to scale the screenshot to</param>
    /// <param name=imageHeight>Height to scale the screenshot to</param>
    /// <param name=imageCompressionQuality>Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk)</param>
    public async Task<GetSourceScreenshotResponse> GetSourceScreenshotAsync(string sourceName, string imageFormat, int? imageWidth = null, int? imageHeight = null, int? imageCompressionQuality = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceScreenshot() { SourceName = sourceName, ImageFormat = imageFormat, ImageWidth = imageWidth, ImageHeight = imageHeight, ImageCompressionQuality = imageCompressionQuality }, cancellation).ConfigureAwait(false) as GetSourceScreenshotResponse)!;
    }

    /// <summary>
    /// Saves a screenshot of a source to the filesystem.<br />
    /// <br />
    /// The `imageWidth` and `imageHeight` parameters are treated as "scale to inner", meaning the smallest ratio will be used and the aspect ratio of the original resolution is kept.<br />
    /// If `imageWidth` and `imageHeight` are not specified, the compressed image will use the full resolution of the source.<br />
    /// <br />
    /// **Compatible with inputs and scenes.**<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source to take a screenshot of</param>
    /// <param name=imageFormat>Image compression format to use. Use `GetVersion` to get compatible image formats</param>
    /// <param name=imageFilePath>Path to save the screenshot file to. Eg. `C:\Users\user\Desktop\screenshot.png`</param>
    /// <param name=imageWidth>Width to scale the screenshot to</param>
    /// <param name=imageHeight>Height to scale the screenshot to</param>
    /// <param name=imageCompressionQuality>Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk)</param>
    public async Task<SaveSourceScreenshotResponse> SaveSourceScreenshotAsync(string sourceName, string imageFormat, string imageFilePath, int? imageWidth = null, int? imageHeight = null, int? imageCompressionQuality = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SaveSourceScreenshot() { SourceName = sourceName, ImageFormat = imageFormat, ImageFilePath = imageFilePath, ImageWidth = imageWidth, ImageHeight = imageHeight, ImageCompressionQuality = imageCompressionQuality }, cancellation).ConfigureAwait(false) as SaveSourceScreenshotResponse)!;
    }

    /// <summary>
    /// Gets an array of all scenes in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetSceneListResponse> GetSceneListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneList() { }, cancellation).ConfigureAwait(false) as GetSceneListResponse)!;
    }

    /// <summary>
    /// Gets an array of all groups in OBS.<br />
    /// <br />
    /// Groups in OBS are actually scenes, but renamed and modified. In obs-websocket, we treat them as scenes where we can.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetGroupListResponse> GetGroupListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetGroupList() { }, cancellation).ConfigureAwait(false) as GetGroupListResponse)!;
    }

    /// <summary>
    /// Gets the current program scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetCurrentProgramSceneResponse> GetCurrentProgramSceneAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetCurrentProgramScene() { }, cancellation).ConfigureAwait(false) as GetCurrentProgramSceneResponse)!;
    }

    /// <summary>
    /// Sets the current program scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Scene to set as the current program scene</param>
    public Task SetCurrentProgramSceneAsync(string sceneName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetCurrentProgramScene() { SceneName = sceneName }, cancellation);
    }

    /// <summary>
    /// Gets the current preview scene.<br />
    /// <br />
    /// Only available when studio mode is enabled.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetCurrentPreviewSceneResponse> GetCurrentPreviewSceneAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetCurrentPreviewScene() { }, cancellation).ConfigureAwait(false) as GetCurrentPreviewSceneResponse)!;
    }

    /// <summary>
    /// Sets the current preview scene.<br />
    /// <br />
    /// Only available when studio mode is enabled.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Scene to set as the current preview scene</param>
    public Task SetCurrentPreviewSceneAsync(string sceneName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetCurrentPreviewScene() { SceneName = sceneName }, cancellation);
    }

    /// <summary>
    /// Creates a new scene in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name for the new scene</param>
    public Task CreateSceneAsync(string sceneName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new CreateScene() { SceneName = sceneName }, cancellation);
    }

    /// <summary>
    /// Removes a scene from OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene to remove</param>
    public Task RemoveSceneAsync(string sceneName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new RemoveScene() { SceneName = sceneName }, cancellation);
    }

    /// <summary>
    /// Sets the name of a scene (rename).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene to be renamed</param>
    /// <param name=newSceneName>New name for the scene</param>
    public Task SetSceneNameAsync(string sceneName, string newSceneName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSceneName() { SceneName = sceneName, NewSceneName = newSceneName }, cancellation);
    }

    /// <summary>
    /// Gets the scene transition overridden for a scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene</param>
    public async Task<GetSceneSceneTransitionOverrideResponse> GetSceneSceneTransitionOverrideAsync(string sceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneSceneTransitionOverride() { SceneName = sceneName }, cancellation).ConfigureAwait(false) as GetSceneSceneTransitionOverrideResponse)!;
    }

    /// <summary>
    /// Gets the scene transition overridden for a scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene</param>
    /// <param name=transitionName>Name of the scene transition to use as override. Specify `null` to remove</param>
    /// <param name=transitionDuration>Duration to use for any overridden transition. Specify `null` to remove</param>
    public Task SetSceneSceneTransitionOverrideAsync(string sceneName, string? transitionName = null, int? transitionDuration = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSceneSceneTransitionOverride() { SceneName = sceneName, TransitionName = transitionName, TransitionDuration = transitionDuration }, cancellation);
    }

    /// <summary>
    /// Gets an array of all inputs in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputKind>Restrict the array to only inputs of the specified kind</param>
    public async Task<GetInputListResponse> GetInputListAsync(string? inputKind = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputList() { InputKind = inputKind }, cancellation).ConfigureAwait(false) as GetInputListResponse)!;
    }

    /// <summary>
    /// Gets an array of all available input kinds in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=unversioned>True == Return all kinds as unversioned, False == Return with version suffixes (if available)</param>
    public async Task<GetInputKindListResponse> GetInputKindListAsync(bool? unversioned = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputKindList() { Unversioned = unversioned }, cancellation).ConfigureAwait(false) as GetInputKindListResponse)!;
    }

    /// <summary>
    /// Gets the names of all special inputs.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetSpecialInputsResponse> GetSpecialInputsAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSpecialInputs() { }, cancellation).ConfigureAwait(false) as GetSpecialInputsResponse)!;
    }

    /// <summary>
    /// Creates a new input, adding it as a scene item to the specified scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene to add the input to as a scene item</param>
    /// <param name=inputName>Name of the new input to created</param>
    /// <param name=inputKind>The kind of input to be created</param>
    /// <param name=inputSettings>Settings object to initialize the input with</param>
    /// <param name=sceneItemEnabled>Whether to set the created scene item to enabled or disabled</param>
    public async Task<CreateInputResponse> CreateInputAsync(string sceneName, string inputName, string inputKind, Dictionary<string, object?>? inputSettings = null, bool? sceneItemEnabled = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CreateInput() { SceneName = sceneName, InputName = inputName, InputKind = inputKind, InputSettings = inputSettings, SceneItemEnabled = sceneItemEnabled }, cancellation).ConfigureAwait(false) as CreateInputResponse)!;
    }

    /// <summary>
    /// Removes an existing input.<br />
    /// <br />
    /// Note: Will immediately remove all associated scene items.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to remove</param>
    public Task RemoveInputAsync(string inputName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new RemoveInput() { InputName = inputName }, cancellation);
    }

    /// <summary>
    /// Sets the name of an input (rename).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Current input name</param>
    /// <param name=newInputName>New name for the input</param>
    public Task SetInputNameAsync(string inputName, string newInputName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetInputName() { InputName = inputName, NewInputName = newInputName }, cancellation);
    }

    /// <summary>
    /// Gets the default settings for an input kind.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputKind>Input kind to get the default settings for</param>
    public async Task<GetInputDefaultSettingsResponse> GetInputDefaultSettingsAsync(string inputKind, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputDefaultSettings() { InputKind = inputKind }, cancellation).ConfigureAwait(false) as GetInputDefaultSettingsResponse)!;
    }

    /// <summary>
    /// Gets the settings of an input.<br />
    /// <br />
    /// Note: Does not include defaults. To create the entire settings object, overlay `inputSettings` over the `defaultInputSettings` provided by `GetInputDefaultSettings`.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to get the settings of</param>
    public async Task<GetInputSettingsResponse> GetInputSettingsAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputSettings() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputSettingsResponse)!;
    }

    /// <summary>
    /// Sets the settings of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to set the settings of</param>
    /// <param name=inputSettings>Object of settings to apply</param>
    /// <param name=overlay>True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.</param>
    public Task SetInputSettingsAsync(string inputName, Dictionary<string, object?> inputSettings, bool? overlay = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetInputSettings() { InputName = inputName, InputSettings = inputSettings, Overlay = overlay }, cancellation);
    }

    /// <summary>
    /// Gets the audio mute state of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of input to get the mute state of</param>
    public async Task<GetInputMuteResponse> GetInputMuteAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputMute() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputMuteResponse)!;
    }

    /// <summary>
    /// Sets the audio mute state of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to set the mute state of</param>
    /// <param name=inputMuted>Whether to mute the input or not</param>
    public Task SetInputMuteAsync(string inputName, bool inputMuted, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetInputMute() { InputName = inputName, InputMuted = inputMuted }, cancellation);
    }

    /// <summary>
    /// Toggles the audio mute state of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to toggle the mute state of</param>
    public async Task<ToggleInputMuteResponse> ToggleInputMuteAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleInputMute() { InputName = inputName }, cancellation).ConfigureAwait(false) as ToggleInputMuteResponse)!;
    }

    /// <summary>
    /// Gets the current volume setting of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to get the volume of</param>
    public async Task<GetInputVolumeResponse> GetInputVolumeAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputVolume() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputVolumeResponse)!;
    }

    /// <summary>
    /// Sets the volume setting of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to set the volume of</param>
    /// <param name=inputVolumeMul>Volume setting in mul</param>
    /// <param name=inputVolumeDb>Volume setting in dB</param>
    public Task SetInputVolumeAsync(string inputName, double? inputVolumeMul = null, double? inputVolumeDb = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetInputVolume() { InputName = inputName, InputVolumeMul = inputVolumeMul, InputVolumeDb = inputVolumeDb }, cancellation);
    }

    /// <summary>
    /// Gets the audio balance of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to get the audio balance of</param>
    public async Task<GetInputAudioBalanceResponse> GetInputAudioBalanceAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputAudioBalance() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputAudioBalanceResponse)!;
    }

    /// <summary>
    /// Sets the audio balance of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to set the audio balance of</param>
    /// <param name=inputAudioBalance>New audio balance value</param>
    public Task SetInputAudioBalanceAsync(string inputName, double inputAudioBalance, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetInputAudioBalance() { InputName = inputName, InputAudioBalance = inputAudioBalance }, cancellation);
    }

    /// <summary>
    /// Gets the audio sync offset of an input.<br />
    /// <br />
    /// Note: The audio sync offset can be negative too!<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to get the audio sync offset of</param>
    public async Task<GetInputAudioSyncOffsetResponse> GetInputAudioSyncOffsetAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputAudioSyncOffset() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputAudioSyncOffsetResponse)!;
    }

    /// <summary>
    /// Sets the audio sync offset of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to set the audio sync offset of</param>
    /// <param name=inputAudioSyncOffset>New audio sync offset in milliseconds</param>
    public Task SetInputAudioSyncOffsetAsync(string inputName, int inputAudioSyncOffset, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetInputAudioSyncOffset() { InputName = inputName, InputAudioSyncOffset = inputAudioSyncOffset }, cancellation);
    }

    /// <summary>
    /// Gets the audio monitor type of an input.<br />
    /// <br />
    /// The available audio monitor types are:<br />
    /// <br />
    /// - `OBS_MONITORING_TYPE_NONE`<br />
    /// - `OBS_MONITORING_TYPE_MONITOR_ONLY`<br />
    /// - `OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT`<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to get the audio monitor type of</param>
    public async Task<GetInputAudioMonitorTypeResponse> GetInputAudioMonitorTypeAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputAudioMonitorType() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputAudioMonitorTypeResponse)!;
    }

    /// <summary>
    /// Sets the audio monitor type of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to set the audio monitor type of</param>
    /// <param name=monitorType>Audio monitor type</param>
    public Task SetInputAudioMonitorTypeAsync(string inputName, MonitoringType monitorType, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetInputAudioMonitorType() { InputName = inputName, MonitorType = monitorType }, cancellation);
    }

    /// <summary>
    /// Gets the enable state of all audio tracks of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input</param>
    public async Task<GetInputAudioTracksResponse> GetInputAudioTracksAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputAudioTracks() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputAudioTracksResponse)!;
    }

    /// <summary>
    /// Sets the enable state of audio tracks of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input</param>
    /// <param name=inputAudioTracks>Track settings to apply</param>
    public Task SetInputAudioTracksAsync(string inputName, Dictionary<string, object?> inputAudioTracks, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetInputAudioTracks() { InputName = inputName, InputAudioTracks = inputAudioTracks }, cancellation);
    }

    /// <summary>
    /// Gets the items of a list property from an input's properties.<br />
    /// <br />
    /// Note: Use this in cases where an input provides a dynamic, selectable list of items. For example, display capture, where it provides a list of available displays.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input</param>
    /// <param name=propertyName>Name of the list property to get the items of</param>
    public async Task<GetInputPropertiesListPropertyItemsResponse> GetInputPropertiesListPropertyItemsAsync(string inputName, string propertyName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputPropertiesListPropertyItems() { InputName = inputName, PropertyName = propertyName }, cancellation).ConfigureAwait(false) as GetInputPropertiesListPropertyItemsResponse)!;
    }

    /// <summary>
    /// Presses a button in the properties of an input.<br />
    /// <br />
    /// Note: Use this in cases where there is a button in the properties of an input that cannot be accessed in any other way. For example, browser sources, where there is a refresh button.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input</param>
    /// <param name=propertyName>Name of the button property to press</param>
    public Task PressInputPropertiesButtonAsync(string inputName, string propertyName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new PressInputPropertiesButton() { InputName = inputName, PropertyName = propertyName }, cancellation);
    }

    /// <summary>
    /// Gets an array of all available transition kinds.<br />
    /// <br />
    /// Similar to `GetInputKindList`<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetTransitionKindListResponse> GetTransitionKindListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetTransitionKindList() { }, cancellation).ConfigureAwait(false) as GetTransitionKindListResponse)!;
    }

    /// <summary>
    /// Gets an array of all scene transitions in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetSceneTransitionListResponse> GetSceneTransitionListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneTransitionList() { }, cancellation).ConfigureAwait(false) as GetSceneTransitionListResponse)!;
    }

    /// <summary>
    /// Gets information about the current scene transition.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetCurrentSceneTransitionResponse> GetCurrentSceneTransitionAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetCurrentSceneTransition() { }, cancellation).ConfigureAwait(false) as GetCurrentSceneTransitionResponse)!;
    }

    /// <summary>
    /// Sets the current scene transition.<br />
    /// <br />
    /// Small note: While the namespace of scene transitions is generally unique, that uniqueness is not a guarantee as it is with other resources like inputs.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=transitionName>Name of the transition to make active</param>
    public Task SetCurrentSceneTransitionAsync(string transitionName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetCurrentSceneTransition() { TransitionName = transitionName }, cancellation);
    }

    /// <summary>
    /// Sets the duration of the current scene transition, if it is not fixed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=transitionDuration>Duration in milliseconds</param>
    public Task SetCurrentSceneTransitionDurationAsync(int transitionDuration, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetCurrentSceneTransitionDuration() { TransitionDuration = transitionDuration }, cancellation);
    }

    /// <summary>
    /// Sets the settings of the current scene transition.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=transitionSettings>Settings object to apply to the transition. Can be `{}`</param>
    /// <param name=overlay>Whether to overlay over the current settings or replace them</param>
    public Task SetCurrentSceneTransitionSettingsAsync(Dictionary<string, object?> transitionSettings, bool? overlay = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetCurrentSceneTransitionSettings() { TransitionSettings = transitionSettings, Overlay = overlay }, cancellation);
    }

    /// <summary>
    /// Gets the cursor position of the current scene transition.<br />
    /// <br />
    /// Note: `transitionCursor` will return 1.0 when the transition is inactive.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetCurrentSceneTransitionCursorResponse> GetCurrentSceneTransitionCursorAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetCurrentSceneTransitionCursor() { }, cancellation).ConfigureAwait(false) as GetCurrentSceneTransitionCursorResponse)!;
    }

    /// <summary>
    /// Triggers the current scene transition. Same functionality as the `Transition` button in studio mode.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task TriggerStudioModeTransitionAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new TriggerStudioModeTransition() { }, cancellation);
    }

    /// <summary>
    /// Sets the position of the TBar.<br />
    /// <br />
    /// **Very important note**: This will be deprecated and replaced in a future version of obs-websocket.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=position>New position</param>
    /// <param name=release>Whether to release the TBar. Only set `false` if you know that you will be sending another position update</param>
    public Task SetTBarPositionAsync(double position, bool? release = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetTBarPosition() { Position = position, Release = release }, cancellation);
    }

    /// <summary>
    /// Gets an array of all of a source's filters.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source</param>
    public async Task<GetSourceFilterListResponse> GetSourceFilterListAsync(string sourceName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceFilterList() { SourceName = sourceName }, cancellation).ConfigureAwait(false) as GetSourceFilterListResponse)!;
    }

    /// <summary>
    /// Gets the default settings for a filter kind.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=filterKind>Filter kind to get the default settings for</param>
    public async Task<GetSourceFilterDefaultSettingsResponse> GetSourceFilterDefaultSettingsAsync(string filterKind, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceFilterDefaultSettings() { FilterKind = filterKind }, cancellation).ConfigureAwait(false) as GetSourceFilterDefaultSettingsResponse)!;
    }

    /// <summary>
    /// Creates a new filter, adding it to the specified source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source to add the filter to</param>
    /// <param name=filterName>Name of the new filter to be created</param>
    /// <param name=filterKind>The kind of filter to be created</param>
    /// <param name=filterSettings>Settings object to initialize the filter with</param>
    public Task CreateSourceFilterAsync(string sourceName, string filterName, string filterKind, Dictionary<string, object?>? filterSettings = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new CreateSourceFilter() { SourceName = sourceName, FilterName = filterName, FilterKind = filterKind, FilterSettings = filterSettings }, cancellation);
    }

    /// <summary>
    /// Removes a filter from a source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source the filter is on</param>
    /// <param name=filterName>Name of the filter to remove</param>
    public Task RemoveSourceFilterAsync(string sourceName, string filterName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new RemoveSourceFilter() { SourceName = sourceName, FilterName = filterName }, cancellation);
    }

    /// <summary>
    /// Sets the name of a source filter (rename).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source the filter is on</param>
    /// <param name=filterName>Current name of the filter</param>
    /// <param name=newFilterName>New name for the filter</param>
    public Task SetSourceFilterNameAsync(string sourceName, string filterName, string newFilterName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSourceFilterName() { SourceName = sourceName, FilterName = filterName, NewFilterName = newFilterName }, cancellation);
    }

    /// <summary>
    /// Gets the info for a specific source filter.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source</param>
    /// <param name=filterName>Name of the filter</param>
    public async Task<GetSourceFilterResponse> GetSourceFilterAsync(string sourceName, string filterName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceFilter() { SourceName = sourceName, FilterName = filterName }, cancellation).ConfigureAwait(false) as GetSourceFilterResponse)!;
    }

    /// <summary>
    /// Sets the index position of a filter on a source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source the filter is on</param>
    /// <param name=filterName>Name of the filter</param>
    /// <param name=filterIndex>New index position of the filter</param>
    public Task SetSourceFilterIndexAsync(string sourceName, string filterName, int filterIndex, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSourceFilterIndex() { SourceName = sourceName, FilterName = filterName, FilterIndex = filterIndex }, cancellation);
    }

    /// <summary>
    /// Sets the settings of a source filter.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source the filter is on</param>
    /// <param name=filterName>Name of the filter to set the settings of</param>
    /// <param name=filterSettings>Object of settings to apply</param>
    /// <param name=overlay>True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.</param>
    public Task SetSourceFilterSettingsAsync(string sourceName, string filterName, Dictionary<string, object?> filterSettings, bool? overlay = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSourceFilterSettings() { SourceName = sourceName, FilterName = filterName, FilterSettings = filterSettings, Overlay = overlay }, cancellation);
    }

    /// <summary>
    /// Sets the enable state of a source filter.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source the filter is on</param>
    /// <param name=filterName>Name of the filter</param>
    /// <param name=filterEnabled>New enable state of the filter</param>
    public Task SetSourceFilterEnabledAsync(string sourceName, string filterName, bool filterEnabled, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSourceFilterEnabled() { SourceName = sourceName, FilterName = filterName, FilterEnabled = filterEnabled }, cancellation);
    }

    /// <summary>
    /// Gets a list of all scene items in a scene.<br />
    /// <br />
    /// Scenes only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene to get the items of</param>
    public async Task<GetSceneItemListResponse> GetSceneItemListAsync(string sceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemList() { SceneName = sceneName }, cancellation).ConfigureAwait(false) as GetSceneItemListResponse)!;
    }

    /// <summary>
    /// Basically GetSceneItemList, but for groups.<br />
    /// <br />
    /// Using groups at all in OBS is discouraged, as they are very broken under the hood.<br />
    /// <br />
    /// Groups only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the group to get the items of</param>
    public async Task<GetGroupSceneItemListResponse> GetGroupSceneItemListAsync(string sceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetGroupSceneItemList() { SceneName = sceneName }, cancellation).ConfigureAwait(false) as GetGroupSceneItemListResponse)!;
    }

    /// <summary>
    /// Searches a scene for a source, and returns its id.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene or group to search in</param>
    /// <param name=sourceName>Name of the source to find</param>
    /// <param name=searchOffset>Number of matches to skip during search. >= 0 means first forward. -1 means last (top) item</param>
    public async Task<GetSceneItemIdResponse> GetSceneItemIdAsync(string sceneName, string sourceName, int? searchOffset = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemId() { SceneName = sceneName, SourceName = sourceName, SearchOffset = searchOffset }, cancellation).ConfigureAwait(false) as GetSceneItemIdResponse)!;
    }

    /// <summary>
    /// Creates a new scene item using a source.<br />
    /// <br />
    /// Scenes only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene to create the new item in</param>
    /// <param name=sourceName>Name of the source to add to the scene</param>
    /// <param name=sceneItemEnabled>Enable state to apply to the scene item on creation</param>
    public async Task<CreateSceneItemResponse> CreateSceneItemAsync(string sceneName, string sourceName, bool? sceneItemEnabled = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CreateSceneItem() { SceneName = sceneName, SourceName = sourceName, SceneItemEnabled = sceneItemEnabled }, cancellation).ConfigureAwait(false) as CreateSceneItemResponse)!;
    }

    /// <summary>
    /// Removes a scene item from a scene.<br />
    /// <br />
    /// Scenes only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    public Task RemoveSceneItemAsync(string sceneName, int sceneItemId, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new RemoveSceneItem() { SceneName = sceneName, SceneItemId = sceneItemId }, cancellation);
    }

    /// <summary>
    /// Duplicates a scene item, copying all transform and crop info.<br />
    /// <br />
    /// Scenes only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    /// <param name=destinationSceneName>Name of the scene to create the duplicated item in</param>
    public async Task<DuplicateSceneItemResponse> DuplicateSceneItemAsync(string sceneName, int sceneItemId, string? destinationSceneName = null, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new DuplicateSceneItem() { SceneName = sceneName, SceneItemId = sceneItemId, DestinationSceneName = destinationSceneName }, cancellation).ConfigureAwait(false) as DuplicateSceneItemResponse)!;
    }

    /// <summary>
    /// Gets the transform and crop info of a scene item.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    public async Task<GetSceneItemTransformResponse> GetSceneItemTransformAsync(string sceneName, int sceneItemId, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemTransform() { SceneName = sceneName, SceneItemId = sceneItemId }, cancellation).ConfigureAwait(false) as GetSceneItemTransformResponse)!;
    }

    /// <summary>
    /// Sets the transform and crop info of a scene item.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    /// <param name=sceneItemTransform>Object containing scene item transform info to update</param>
    public Task SetSceneItemTransformAsync(string sceneName, int sceneItemId, Dictionary<string, object?> sceneItemTransform, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSceneItemTransform() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemTransform = sceneItemTransform }, cancellation);
    }

    /// <summary>
    /// Gets the enable state of a scene item.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    public async Task<GetSceneItemEnabledResponse> GetSceneItemEnabledAsync(string sceneName, int sceneItemId, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemEnabled() { SceneName = sceneName, SceneItemId = sceneItemId }, cancellation).ConfigureAwait(false) as GetSceneItemEnabledResponse)!;
    }

    /// <summary>
    /// Sets the enable state of a scene item.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    /// <param name=sceneItemEnabled>New enable state of the scene item</param>
    public Task SetSceneItemEnabledAsync(string sceneName, int sceneItemId, bool sceneItemEnabled, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSceneItemEnabled() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemEnabled = sceneItemEnabled }, cancellation);
    }

    /// <summary>
    /// Gets the lock state of a scene item.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    public async Task<GetSceneItemLockedResponse> GetSceneItemLockedAsync(string sceneName, int sceneItemId, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemLocked() { SceneName = sceneName, SceneItemId = sceneItemId }, cancellation).ConfigureAwait(false) as GetSceneItemLockedResponse)!;
    }

    /// <summary>
    /// Sets the lock state of a scene item.<br />
    /// <br />
    /// Scenes and Group<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    /// <param name=sceneItemLocked>New lock state of the scene item</param>
    public Task SetSceneItemLockedAsync(string sceneName, int sceneItemId, bool sceneItemLocked, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSceneItemLocked() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemLocked = sceneItemLocked }, cancellation);
    }

    /// <summary>
    /// Gets the index position of a scene item in a scene.<br />
    /// <br />
    /// An index of 0 is at the bottom of the source list in the UI.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    public async Task<GetSceneItemIndexResponse> GetSceneItemIndexAsync(string sceneName, int sceneItemId, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemIndex() { SceneName = sceneName, SceneItemId = sceneItemId }, cancellation).ConfigureAwait(false) as GetSceneItemIndexResponse)!;
    }

    /// <summary>
    /// Sets the index position of a scene item in a scene.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    /// <param name=sceneItemIndex>New index position of the scene item</param>
    public Task SetSceneItemIndexAsync(string sceneName, int sceneItemId, int sceneItemIndex, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSceneItemIndex() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemIndex = sceneItemIndex }, cancellation);
    }

    /// <summary>
    /// Gets the blend mode of a scene item.<br />
    /// <br />
    /// Blend modes:<br />
    /// <br />
    /// - `OBS_BLEND_NORMAL`<br />
    /// - `OBS_BLEND_ADDITIVE`<br />
    /// - `OBS_BLEND_SUBTRACT`<br />
    /// - `OBS_BLEND_SCREEN`<br />
    /// - `OBS_BLEND_MULTIPLY`<br />
    /// - `OBS_BLEND_LIGHTEN`<br />
    /// - `OBS_BLEND_DARKEN`<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    public async Task<GetSceneItemBlendModeResponse> GetSceneItemBlendModeAsync(string sceneName, int sceneItemId, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemBlendMode() { SceneName = sceneName, SceneItemId = sceneItemId }, cancellation).ConfigureAwait(false) as GetSceneItemBlendModeResponse)!;
    }

    /// <summary>
    /// Sets the blend mode of a scene item.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sceneName>Name of the scene the item is in</param>
    /// <param name=sceneItemId>Numeric ID of the scene item</param>
    /// <param name=sceneItemBlendMode>New blend mode</param>
    public Task SetSceneItemBlendModeAsync(string sceneName, int sceneItemId, string sceneItemBlendMode, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetSceneItemBlendMode() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemBlendMode = sceneItemBlendMode }, cancellation);
    }

    /// <summary>
    /// Gets the status of the virtualcam output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetVirtualCamStatusResponse> GetVirtualCamStatusAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetVirtualCamStatus() { }, cancellation).ConfigureAwait(false) as GetVirtualCamStatusResponse)!;
    }

    /// <summary>
    /// Toggles the state of the virtualcam output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<ToggleVirtualCamResponse> ToggleVirtualCamAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleVirtualCam() { }, cancellation).ConfigureAwait(false) as ToggleVirtualCamResponse)!;
    }

    /// <summary>
    /// Starts the virtualcam output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task StartVirtualCamAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StartVirtualCam() { }, cancellation);
    }

    /// <summary>
    /// Stops the virtualcam output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task StopVirtualCamAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StopVirtualCam() { }, cancellation);
    }

    /// <summary>
    /// Gets the status of the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetReplayBufferStatusResponse> GetReplayBufferStatusAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetReplayBufferStatus() { }, cancellation).ConfigureAwait(false) as GetReplayBufferStatusResponse)!;
    }

    /// <summary>
    /// Toggles the state of the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<ToggleReplayBufferResponse> ToggleReplayBufferAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleReplayBuffer() { }, cancellation).ConfigureAwait(false) as ToggleReplayBufferResponse)!;
    }

    /// <summary>
    /// Starts the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task StartReplayBufferAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StartReplayBuffer() { }, cancellation);
    }

    /// <summary>
    /// Stops the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task StopReplayBufferAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StopReplayBuffer() { }, cancellation);
    }

    /// <summary>
    /// Saves the contents of the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task SaveReplayBufferAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SaveReplayBuffer() { }, cancellation);
    }

    /// <summary>
    /// Gets the filename of the last replay buffer save file.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetLastReplayBufferReplayResponse> GetLastReplayBufferReplayAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetLastReplayBufferReplay() { }, cancellation).ConfigureAwait(false) as GetLastReplayBufferReplayResponse)!;
    }

    /// <summary>
    /// Gets the list of available outputs.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetOutputListResponse> GetOutputListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetOutputList() { }, cancellation).ConfigureAwait(false) as GetOutputListResponse)!;
    }

    /// <summary>
    /// Gets the status of an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=outputName>Output name</param>
    public async Task<GetOutputStatusResponse> GetOutputStatusAsync(string outputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetOutputStatus() { OutputName = outputName }, cancellation).ConfigureAwait(false) as GetOutputStatusResponse)!;
    }

    /// <summary>
    /// Toggles the status of an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=outputName>Output name</param>
    public async Task<ToggleOutputResponse> ToggleOutputAsync(string outputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleOutput() { OutputName = outputName }, cancellation).ConfigureAwait(false) as ToggleOutputResponse)!;
    }

    /// <summary>
    /// Starts an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=outputName>Output name</param>
    public Task StartOutputAsync(string outputName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StartOutput() { OutputName = outputName }, cancellation);
    }

    /// <summary>
    /// Stops an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=outputName>Output name</param>
    public Task StopOutputAsync(string outputName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StopOutput() { OutputName = outputName }, cancellation);
    }

    /// <summary>
    /// Gets the settings of an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=outputName>Output name</param>
    public async Task<GetOutputSettingsResponse> GetOutputSettingsAsync(string outputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetOutputSettings() { OutputName = outputName }, cancellation).ConfigureAwait(false) as GetOutputSettingsResponse)!;
    }

    /// <summary>
    /// Sets the settings of an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=outputName>Output name</param>
    /// <param name=outputSettings>Output settings</param>
    public Task SetOutputSettingsAsync(string outputName, Dictionary<string, object?> outputSettings, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetOutputSettings() { OutputName = outputName, OutputSettings = outputSettings }, cancellation);
    }

    /// <summary>
    /// Gets the status of the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetStreamStatusResponse> GetStreamStatusAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetStreamStatus() { }, cancellation).ConfigureAwait(false) as GetStreamStatusResponse)!;
    }

    /// <summary>
    /// Toggles the status of the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<ToggleStreamResponse> ToggleStreamAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleStream() { }, cancellation).ConfigureAwait(false) as ToggleStreamResponse)!;
    }

    /// <summary>
    /// Starts the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task StartStreamAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StartStream() { }, cancellation);
    }

    /// <summary>
    /// Stops the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task StopStreamAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StopStream() { }, cancellation);
    }

    /// <summary>
    /// Sends CEA-608 caption text over the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=captionText>Caption text</param>
    public Task SendStreamCaptionAsync(string captionText, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SendStreamCaption() { CaptionText = captionText }, cancellation);
    }

    /// <summary>
    /// Gets the status of the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetRecordStatusResponse> GetRecordStatusAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetRecordStatus() { }, cancellation).ConfigureAwait(false) as GetRecordStatusResponse)!;
    }

    /// <summary>
    /// Toggles the status of the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task ToggleRecordAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new ToggleRecord() { }, cancellation);
    }

    /// <summary>
    /// Starts the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task StartRecordAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new StartRecord() { }, cancellation);
    }

    /// <summary>
    /// Stops the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<StopRecordResponse> StopRecordAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StopRecord() { }, cancellation).ConfigureAwait(false) as StopRecordResponse)!;
    }

    /// <summary>
    /// Toggles pause on the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task ToggleRecordPauseAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new ToggleRecordPause() { }, cancellation);
    }

    /// <summary>
    /// Pauses the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task PauseRecordAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new PauseRecord() { }, cancellation);
    }

    /// <summary>
    /// Resumes the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public Task ResumeRecordAsync(CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new ResumeRecord() { }, cancellation);
    }

    /// <summary>
    /// Gets the status of a media input.<br />
    /// <br />
    /// Media States:<br />
    /// <br />
    /// - `OBS_MEDIA_STATE_NONE`<br />
    /// - `OBS_MEDIA_STATE_PLAYING`<br />
    /// - `OBS_MEDIA_STATE_OPENING`<br />
    /// - `OBS_MEDIA_STATE_BUFFERING`<br />
    /// - `OBS_MEDIA_STATE_PAUSED`<br />
    /// - `OBS_MEDIA_STATE_STOPPED`<br />
    /// - `OBS_MEDIA_STATE_ENDED`<br />
    /// - `OBS_MEDIA_STATE_ERROR`<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the media input</param>
    public async Task<GetMediaInputStatusResponse> GetMediaInputStatusAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetMediaInputStatus() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetMediaInputStatusResponse)!;
    }

    /// <summary>
    /// Sets the cursor position of a media input.<br />
    /// <br />
    /// This request does not perform bounds checking of the cursor position.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the media input</param>
    /// <param name=mediaCursor>New cursor position to set</param>
    public Task SetMediaInputCursorAsync(string inputName, double mediaCursor, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetMediaInputCursor() { InputName = inputName, MediaCursor = mediaCursor }, cancellation);
    }

    /// <summary>
    /// Offsets the current cursor position of a media input by the specified value.<br />
    /// <br />
    /// This request does not perform bounds checking of the cursor position.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the media input</param>
    /// <param name=mediaCursorOffset>Value to offset the current cursor position by</param>
    public Task OffsetMediaInputCursorAsync(string inputName, int mediaCursorOffset, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new OffsetMediaInputCursor() { InputName = inputName, MediaCursorOffset = mediaCursorOffset }, cancellation);
    }

    /// <summary>
    /// Triggers an action on a media input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the media input</param>
    /// <param name=mediaAction>Identifier of the `ObsMediaInputAction` enum</param>
    public Task TriggerMediaInputActionAsync(string inputName, string mediaAction, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new TriggerMediaInputAction() { InputName = inputName, MediaAction = mediaAction }, cancellation);
    }

    /// <summary>
    /// Gets whether studio is enabled.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetStudioModeEnabledResponse> GetStudioModeEnabledAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetStudioModeEnabled() { }, cancellation).ConfigureAwait(false) as GetStudioModeEnabledResponse)!;
    }

    /// <summary>
    /// Enables or disables studio mode<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=studioModeEnabled>True == Enabled, False == Disabled</param>
    public Task SetStudioModeEnabledAsync(bool studioModeEnabled, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new SetStudioModeEnabled() { StudioModeEnabled = studioModeEnabled }, cancellation);
    }

    /// <summary>
    /// Opens the properties dialog of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to open the dialog of</param>
    public Task OpenInputPropertiesDialogAsync(string inputName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new OpenInputPropertiesDialog() { InputName = inputName }, cancellation);
    }

    /// <summary>
    /// Opens the filters dialog of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to open the dialog of</param>
    public Task OpenInputFiltersDialogAsync(string inputName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new OpenInputFiltersDialog() { InputName = inputName }, cancellation);
    }

    /// <summary>
    /// Opens the interact dialog of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=inputName>Name of the input to open the dialog of</param>
    public Task OpenInputInteractDialogAsync(string inputName, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new OpenInputInteractDialog() { InputName = inputName }, cancellation);
    }

    /// <summary>
    /// Gets a list of connected monitors and information about them.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public async Task<GetMonitorListResponse> GetMonitorListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetMonitorList() { }, cancellation).ConfigureAwait(false) as GetMonitorListResponse)!;
    }

    /// <summary>
    /// Opens a projector for a specific output video mix.<br />
    /// <br />
    /// Mix types:<br />
    /// <br />
    /// - `OBS_WEBSOCKET_VIDEO_MIX_TYPE_PREVIEW`<br />
    /// - `OBS_WEBSOCKET_VIDEO_MIX_TYPE_PROGRAM`<br />
    /// - `OBS_WEBSOCKET_VIDEO_MIX_TYPE_MULTIVIEW`<br />
    /// <br />
    /// Note: This request serves to provide feature parity with 4.x. It is very likely to be changed/deprecated in a future release.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=videoMixType>Type of mix to open</param>
    /// <param name=monitorIndex>Monitor index, use `GetMonitorList` to obtain index</param>
    /// <param name=projectorGeometry>Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with `monitorIndex`</param>
    public Task OpenVideoMixProjectorAsync(string videoMixType, int? monitorIndex = null, string? projectorGeometry = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new OpenVideoMixProjector() { VideoMixType = videoMixType, MonitorIndex = monitorIndex, ProjectorGeometry = projectorGeometry }, cancellation);
    }

    /// <summary>
    /// Opens a projector for a source.<br />
    /// <br />
    /// Note: This request serves to provide feature parity with 4.x. It is very likely to be changed/deprecated in a future release.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name=sourceName>Name of the source to open a projector for</param>
    /// <param name=monitorIndex>Monitor index, use `GetMonitorList` to obtain index</param>
    /// <param name=projectorGeometry>Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with `monitorIndex`</param>
    public Task OpenSourceProjectorAsync(string sourceName, int? monitorIndex = null, string? projectorGeometry = null, CancellationToken cancellation = default) {
      return _clientSocket.RequestAsync(new OpenSourceProjector() { SourceName = sourceName, MonitorIndex = monitorIndex, ProjectorGeometry = projectorGeometry }, cancellation);
    }

    #endregion

    #region Event dispatch
    private void Dispatch(IEvent message) {
      Event(message);
      switch (message) {
      case GeneralEvent general:
        DispatchGeneralEvent(general);
        break;
      case ConfigEvent config:
        DispatchConfigEvent(config);
        break;
      case ScenesEvent scenes:
        DispatchScenesEvent(scenes);
        break;
      case InputsEvent input:
        DispatchInputsEvent(input);
        break;
      case TransitionsEvent transitions:
        DispatchTransitionsEvent(transitions);
        break;
      case FiltersEvent filters:
        DispatchFiltersEvent(filters);
        break;
      case SceneItemsEvent sceneItems:
        DispatchSceneItemsEvent(sceneItems);
        break;
      case OutputsEvent outputs:
        DispatchOutputsEvent(outputs);
        break;
      case MediaInputsEvent mediaInputs:
        DispatchMediaInputsEvent(mediaInputs);
        break;
      case UiEvent ui:
        DispatchUiEvent(ui);
        break;
      default:
        // TODO: Log
        break;
      }
    }

    private void DispatchGeneralEvent(GeneralEvent general) {
      GeneralEvent(general);
      switch (general) {
      case ExitStarted exit:
        ExitStarted(exit);
        break;
      case VendorEvent vendor:
        VendorEvent(vendor);
        break;
      }
    }

    private void DispatchConfigEvent(ConfigEvent config) {
      ConfigEvent(config);
      switch (config) {
      case CurrentSceneCollectionChanging ev:
        CurrentSceneCollectionChanging(ev);
        break;
      case CurrentSceneCollectionChanged ev:
        CurrentSceneCollectionChanged(ev);
        break;
      case SceneCollectionListChanged ev:
        SceneCollectionListChanged(ev);
        break;
      case CurrentProfileChanging ev:
        CurrentProfileChanging(ev);
        break;
      case CurrentProfileChanged ev:
        CurrentProfileChanged(ev);
        break;
      case ProfileListChanged ev:
        ProfileListChanged(ev);
        break;
      }
    }

    private void DispatchScenesEvent(ScenesEvent scene) {
      ScenesEvent(scene);
      switch (scene) {
      case SceneCreated ev:
        SceneCreated(ev);
        break;
      case SceneRemoved ev:
        SceneRemoved(ev);
        break;
      case SceneNameChanged ev:
        SceneNameChanged(ev);
        break;
      case CurrentProgramSceneChanged ev:
        CurrentProgramSceneChanged(ev);
        break;
      case CurrentPreviewSceneChanged ev:
        CurrentPreviewSceneChanged(ev);
        break;
      case SceneListChanged ev:
        SceneListChanged(ev);
        break;
      }
    }

    private void DispatchInputsEvent(InputsEvent input) {
      InputsEvent(input);
      switch (input) {
      case InputCreated ev:
        InputCreated(ev);
        break;
      case InputRemoved ev:
        InputRemoved(ev);
        break;
      case InputNameChanged ev:
        InputNameChanged(ev);
        break;
      case InputActiveStateChanged ev:
        InputActiveStateChanged(ev);
        break;
      case InputShowStateChanged ev:
        InputShowStateChanged(ev);
        break;
      case InputMuteStateChanged ev:
        InputMuteStateChanged(ev);
        break;
      case InputVolumeChanged ev:
        InputVolumeChanged(ev);
        break;
      case InputAudioBalanceChanged ev:
        InputAudioBalanceChanged(ev);
        break;
      case InputAudioSyncOffsetChanged ev:
        InputAudioSyncOffsetChanged(ev);
        break;
      case InputAudioTracksChanged ev:
        InputAudioTracksChanged(ev);
        break;
      case InputAudioMonitorTypeChanged ev:
        InputAudioMonitorTypeChanged(ev);
        break;
      case InputVolumeMeters ev:
        InputVolumeMeters(ev);
        break;
      }
    }

    private void DispatchTransitionsEvent(TransitionsEvent transitions) {
      TransitionsEvent(transitions);
      switch (transitions) {
      case CurrentSceneTransitionChanged ev:
        CurrentSceneTransitionChanged(ev);
        break;
      case CurrentSceneTransitionDurationChanged ev:
        CurrentSceneTransitionDurationChanged(ev);
        break;
      case SceneTransitionStarted ev:
        SceneTransitionStarted(ev);
        break;
      case SceneTransitionEnded ev:
        SceneTransitionEnded(ev);
        break;
      case SceneTransitionVideoEnded ev:
        SceneTransitionVideoEnded(ev);
        break;
      }
    }

    private void DispatchFiltersEvent(FiltersEvent filters) {
      FiltersEvent(filters);
      switch (filters) {
      case SourceFilterListReindexed ev:
        SourceFilterListReindexed(ev);
        break;
      case SourceFilterCreated ev:
        SourceFilterCreated(ev);
        break;
      case SourceFilterRemoved ev:
        SourceFilterRemoved(ev);
        break;
      case SourceFilterNameChanged ev:
        SourceFilterNameChanged(ev);
        break;
      case SourceFilterEnableStateChanged ev:
        SourceFilterEnableStateChanged(ev);
        break;
      }
    }

    private void DispatchSceneItemsEvent(SceneItemsEvent sceneItems) {
      SceneItemsEvent(sceneItems);
      switch (sceneItems) {
      case SceneItemCreated ev:
        SceneItemCreated(ev);
        break;
      case SceneItemRemoved ev:
        SceneItemRemoved(ev);
        break;
      case SceneItemListReindexed ev:
        SceneItemListReindexed(ev);
        break;
      case SceneItemEnableStateChanged ev:
        SceneItemEnableStateChanged(ev);
        break;
      case SceneItemLockStateChanged ev:
        SceneItemLockStateChanged(ev);
        break;
      case SceneItemSelected ev:
        SceneItemSelected(ev);
        break;
      case SceneItemTransformChanged ev:
        SceneItemTransformChanged(ev);
        break;
      }
    }
    private void DispatchOutputsEvent(OutputsEvent outputs) {
      OutputsEvent(outputs);
      switch (outputs) {
      case StreamStateChanged ev:
        StreamStateChanged(ev);
        break;
      case RecordStateChanged ev:
        RecordStateChanged(ev);
        break;
      case ReplayBufferStateChanged ev:
        ReplayBufferStateChanged(ev);
        break;
      case VirtualcamStateChanged ev:
        VirtualcamStateChanged(ev);
        break;
      case ReplayBufferSaved ev:
        ReplayBufferSaved(ev);
        break;
      }
    }

    private void DispatchMediaInputsEvent(MediaInputsEvent mediaInputs) {
      MediaInputsEvent(mediaInputs);
      switch (mediaInputs) {
      case MediaInputPlaybackStarted ev:
        MediaInputPlaybackStarted(ev);
        break;
      case MediaInputPlaybackEnded ev:
        MediaInputPlaybackEnded(ev);
        break;
      case MediaInputActionTriggered ev:
        MediaInputActionTriggered(ev);
        break;
      }
    }

    private void DispatchUiEvent(UiEvent ui) {
      UiEvent(ui);
      switch (ui) {
      case StudioModeStateChanged ev:
        StudioModeStateChanged(ev);
        break;
      }
    }

    #endregion
  }
}
