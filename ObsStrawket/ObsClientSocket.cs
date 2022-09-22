using Microsoft.Extensions.Logging;
using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {
  /// <summary>
  /// High level client interface.
  /// </summary>
  public class ObsClientSocket : IDisposable {

    #region Events

    /// <summary>
    /// All events.
    /// </summary>
    public event Action<IObsEvent> Event = delegate { };

    /// <summary>
    /// General event group.
    /// </summary>
    public event Action<GeneralEvent> GeneralEvent = delegate { };
    /// <summary>
    /// OBS has begun the shutdown process.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<ExitStarted> ExitStarted = delegate { };
    /// <summary>
    /// An event has been emitted from a vendor.<br />
    /// <br />
    /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
    /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<VendorEvent> VendorEvent = delegate { };

    /// <summary>
    /// Config event group.
    /// </summary>
    public event Action<ConfigEvent> ConfigEvent = delegate { };
    /// <summary>
    /// The current scene collection has begun changing.<br />
    /// <br />
    /// Note: We recommend using this event to trigger a pause of all polling requests, as performing any requests during a<br />
    /// scene collection change is considered undefined behavior and can cause crashes!<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentSceneCollectionChanging> CurrentSceneCollectionChanging = delegate { };
    /// <summary>
    /// The current scene collection has changed.<br />
    /// <br />
    /// Note: If polling has been paused during <c>CurrentSceneCollectionChanging</c>, this is the que to restart polling.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentSceneCollectionChanged> CurrentSceneCollectionChanged = delegate { };
    /// <summary>
    /// The scene collection list has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneCollectionListChanged> SceneCollectionListChanged = delegate { };
    /// <summary>
    /// The current profile has begun changing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentProfileChanging> CurrentProfileChanging = delegate { };
    /// <summary>
    /// The current profile has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentProfileChanged> CurrentProfileChanged = delegate { };
    /// <summary>
    /// The profile list has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<ProfileListChanged> ProfileListChanged = delegate { };

    /// <summary>
    /// Scenes event group.
    /// </summary>
    public event Action<ScenesEvent> ScenesEvent = delegate { };
    /// <summary>
    /// A new scene has been created.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneCreated> SceneCreated = delegate { };
    /// <summary>
    /// A scene has been removed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneRemoved> SceneRemoved = delegate { };
    /// <summary>
    /// The name of a scene has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneNameChanged> SceneNameChanged = delegate { };
    /// <summary>
    /// The current program scene has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentProgramSceneChanged> CurrentProgramSceneChanged = delegate { };
    /// <summary>
    /// The current preview scene has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentPreviewSceneChanged> CurrentPreviewSceneChanged = delegate { };
    /// <summary>
    /// The list of scenes has changed.<br />
    /// <br />
    /// TODO: Make OBS fire this event when scenes are reordered.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneListChanged> SceneListChanged = delegate { };

    /// <summary>
    /// Inputs event group.
    /// </summary>
    public event Action<InputsEvent> InputsEvent = delegate { };
    /// <summary>
    /// An input has been created.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputCreated> InputCreated = delegate { };
    /// <summary>
    /// An input has been removed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputRemoved> InputRemoved = delegate { };
    /// <summary>
    /// The name of an input has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputNameChanged> InputNameChanged = delegate { };
    /// <summary>
    /// An input's active state has changed.<br />
    /// <br />
    /// When an input is active, it means it's being shown by the program feed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputActiveStateChanged> InputActiveStateChanged = delegate { };
    /// <summary>
    /// An input's show state has changed.<br />
    /// <br />
    /// When an input is showing, it means it's being shown by the preview or a dialog.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputShowStateChanged> InputShowStateChanged = delegate { };
    /// <summary>
    /// An input's mute state has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputMuteStateChanged> InputMuteStateChanged = delegate { };
    /// <summary>
    /// An input's volume level has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputVolumeChanged> InputVolumeChanged = delegate { };
    /// <summary>
    /// The audio balance value of an input has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputAudioBalanceChanged> InputAudioBalanceChanged = delegate { };
    /// <summary>
    /// The sync offset of an input has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputAudioSyncOffsetChanged> InputAudioSyncOffsetChanged = delegate { };
    /// <summary>
    /// The audio tracks of an input have changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputAudioTracksChanged> InputAudioTracksChanged = delegate { };
    /// <summary>
    /// The monitor type of an input has changed.<br />
    /// <br />
    /// Available types are:<br />
    /// <br />
    /// - <c>OBS_MONITORING_TYPE_NONE</c><br />
    /// - <c>OBS_MONITORING_TYPE_MONITOR_ONLY</c><br />
    /// - <c>OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT</c><br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputAudioMonitorTypeChanged> InputAudioMonitorTypeChanged = delegate { };
    /// <summary>
    /// A high-volume event providing volume levels of all active inputs every 50 milliseconds.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputVolumeMeters> InputVolumeMeters = delegate { };

    /// <summary>
    /// Transitions event group.
    /// </summary>
    public event Action<TransitionsEvent> TransitionsEvent = delegate { };
    /// <summary>
    /// The current scene transition has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentSceneTransitionChanged> CurrentSceneTransitionChanged = delegate { };
    /// <summary>
    /// The current scene transition duration has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentSceneTransitionDurationChanged> CurrentSceneTransitionDurationChanged = delegate { };
    /// <summary>
    /// A scene transition has started.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneTransitionStarted> SceneTransitionStarted = delegate { };
    /// <summary>
    /// A scene transition has completed fully.<br />
    /// <br />
    /// Note: Does not appear to trigger when the transition is interrupted by the user.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneTransitionEnded> SceneTransitionEnded = delegate { };
    /// <summary>
    /// A scene transition's video has completed fully.<br />
    /// <br />
    /// Useful for stinger transitions to tell when the video *actually* ends.<br />
    /// <c>SceneTransitionEnded</c> only signifies the cut point, not the completion of transition playback.<br />
    /// <br />
    /// Note: Appears to be called by every transition, regardless of relevance.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneTransitionVideoEnded> SceneTransitionVideoEnded = delegate { };

    /// <summary>
    /// Filters event group.
    /// </summary>
    public event Action<FiltersEvent> FiltersEvent = delegate { };
    /// <summary>
    /// A source's filter list has been reindexed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterListReindexed> SourceFilterListReindexed = delegate { };
    /// <summary>
    /// A filter has been added to a source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterCreated> SourceFilterCreated = delegate { };
    /// <summary>
    /// A filter has been removed from a source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterRemoved> SourceFilterRemoved = delegate { };
    /// <summary>
    /// The name of a source filter has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterNameChanged> SourceFilterNameChanged = delegate { };
    /// <summary>
    /// A source filter's enable state has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterEnableStateChanged> SourceFilterEnableStateChanged = delegate { };

    /// <summary>
    /// Scene items event group.
    /// </summary>
    public event Action<SceneItemsEvent> SceneItemsEvent = delegate { };
    /// <summary>
    /// A scene item has been created.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemCreated> SceneItemCreated = delegate { };
    /// <summary>
    /// A scene item has been removed.<br />
    /// <br />
    /// This event is not emitted when the scene the item is in is removed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemRemoved> SceneItemRemoved = delegate { };
    /// <summary>
    /// A scene's item list has been reindexed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemListReindexed> SceneItemListReindexed = delegate { };
    /// <summary>
    /// A scene item's enable state has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemEnableStateChanged> SceneItemEnableStateChanged = delegate { };
    /// <summary>
    /// A scene item's lock state has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemLockStateChanged> SceneItemLockStateChanged = delegate { };
    /// <summary>
    /// A scene item has been selected in the Ui.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemSelected> SceneItemSelected = delegate { };
    /// <summary>
    /// The transform/crop of a scene item has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemTransformChanged> SceneItemTransformChanged = delegate { };

    /// <summary>
    /// Outputs event group.
    /// </summary>
    public event Action<OutputsEvent> OutputsEvent = delegate { };
    /// <summary>
    /// The state of the stream output has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<StreamStateChanged> StreamStateChanged = delegate { };
    /// <summary>
    /// The state of the record output has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<RecordStateChanged> RecordStateChanged = delegate { };
    /// <summary>
    /// The state of the replay buffer output has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<ReplayBufferStateChanged> ReplayBufferStateChanged = delegate { };
    /// <summary>
    /// The state of the virtualcam output has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<VirtualcamStateChanged> VirtualcamStateChanged = delegate { };
    /// <summary>
    /// The replay buffer has been saved.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<ReplayBufferSaved> ReplayBufferSaved = delegate { };

    /// <summary>
    /// Media inputs event group.
    /// </summary>
    public event Action<MediaInputsEvent> MediaInputsEvent = delegate { };
    /// <summary>
    /// A media input has started playing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<MediaInputPlaybackStarted> MediaInputPlaybackStarted = delegate { };
    /// <summary>
    /// A media input has finished playing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<MediaInputPlaybackEnded> MediaInputPlaybackEnded = delegate { };
    /// <summary>
    /// An action has been performed on an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<MediaInputActionTriggered> MediaInputActionTriggered = delegate { };

    /// <summary>
    /// Ui event group.
    /// </summary>
    public event Action<UiEvent> UiEvent = delegate { };
    /// <summary>
    /// Studio mode has been enabled or disabled.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<StudioModeStateChanged> StudioModeStateChanged = delegate { };

    #endregion

    /// <summary>
    /// Fired when it is connected to OBS
    /// </summary>
    public event Action<Uri> Connected = delegate { };
    /// <summary>
    /// Fired when it is disconnected to OBS
    /// </summary>
    public event Action<object> Disconnected = delegate { };

    private readonly ClientSocket _clientSocket;
    private readonly ILogger? _logger;
    private readonly SemaphoreSlim _connectSemaphore = new(1);
    private Task? _dispatch;

    /// <summary>
    /// Whether it is connected to OBS
    /// </summary>
    public bool IsConnected { get => _clientSocket.IsConnected; }

    /// <summary>
    /// It emits all of received OBS events. It can be used only when <see cref="ObsClientSocket"/> is created with <c>useChannel</c>.
    /// </summary>
    public ChannelReader<IObsEvent> Events {
      get {
        if (_dispatch != null) {
          throw new InvalidOperationException("Create first ObsClientSocket with useChannel: true");
        }
        return _clientSocket.Events;
      }
    }

    /// <summary>
    /// Create OBS websocket client. It can be reused unless <see cref="Dispose"/> is called.
    /// </summary>
    /// <param name="logger">Logger for library debugging.</param>
    /// <param name="client">Lower level client for custom behavior.</param>
    /// <param name="useChannel">Use channel for event receive.<br />
    /// Caution: If <see cref="Events"/> is not consumed it will cause memory leak.
    /// </param>
    public ObsClientSocket(ILogger? logger = null, ClientSocket? client = null, bool useChannel = false) {
      _logger = logger;
      _clientSocket = client ?? new ClientSocket(logger);
      if (!useChannel) {
        _dispatch = Task.CompletedTask;
      }
    }

    /// <summary>
    /// Connect to OBS websocket server.
    /// </summary>
    /// <param name="uri">Destination uri. Use <c>ws://localhost:4455</c> if null.</param>
    /// <param name="password">Password for handshake.</param>
    /// <param name="events">Event categories to subscribe.</param>
    /// <param name="cancellation">Token for cancellation.</param>
    public async Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken cancellation = default
    ) {
      var target = uri ?? ClientSocket.DefaultUri;
      await _connectSemaphore.WaitAsync(cancellation).ConfigureAwait(false);
      // _dispatch may be same among multiple threads if restored at the same time.
      try {
        await _clientSocket.ConnectAsync(target, password, events, cancellation).ConfigureAwait(false);
        if (_dispatch != null) {
          _dispatch = _dispatch.ContinueWith(
            (_) => DispatchEventAsync(_clientSocket.Events, target), TaskScheduler.Default);
        }
      }
      finally {
        _connectSemaphore.Release();
      }
    }

    /// <summary>
    /// Close this connection. Pending requests will be cancelled.
    /// </summary>
    public Task CloseAsync() => _clientSocket.CloseAsync();

    /// <summary>
    /// Low level request method. It can send <see cref="RawRequest"/>.
    /// </summary>
    /// <param name="request">Request data.</param>
    /// <param name="cancellation">Token for cancellation</param>
    /// <returns>Response from websocket server.</returns>
    public Task<IRequestResponse> RequestAsync(IRequest request, CancellationToken cancellation = default)
      => _clientSocket.RequestAsync(request, cancellation);

    /// <summary>
    /// Dispose this forever.
    /// </summary>
    public void Dispose() {
      _clientSocket.Dispose();
      _connectSemaphore.Dispose();
      GC.SuppressFinalize(this);
    }

    private async Task DispatchEventAsync(ChannelReader<IObsEvent> events, Uri uri) {
      using var _1 = _logger?.BeginScope(nameof(DispatchEventAsync));
      _logger?.LogDebug("Start");

      try {
        Connected(uri);
      }
      catch (Exception exception) {
        _logger?.LogWarning(exception, "Connected event handler throws");
      }

      try {
        while (await events.WaitToReadAsync().ConfigureAwait(false)) {
          DispatchEvent(await events.ReadAsync().ConfigureAwait(false));
        }
        _logger?.LogDebug("Terminated");
        InvokeCloseSafe("Normal closure");
      }
      catch (Exception exception) {
        _logger?.LogDebug(exception, "Terminated with exception");
        InvokeCloseSafe(exception);
      }
    }

    private void InvokeCloseSafe(object info) {
      try {
        Disconnected(info);
      }
      catch (Exception ex) {
        _logger?.LogWarning(ex, "Disconnected event handler throws");
      }
    }

    #region Requests

    /// <summary>
    /// Gets data about the current plugin and RPC version.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetVersionResponse> GetVersionAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetVersion() { }, cancellation).ConfigureAwait(false) as GetVersionResponse)!;
    }

    /// <summary>
    /// Gets statistics about OBS, obs-websocket, and the current session.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetStatsResponse> GetStatsAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetStats() { }, cancellation).ConfigureAwait(false) as GetStatsResponse)!;
    }

    /// <summary>
    /// Broadcasts a <c>CustomEvent</c> to all WebSocket clients. Receivers are clients which are identified and subscribed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="eventData">Data payload to emit to all receivers</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> BroadcastCustomEventAsync(Dictionary<string, object?> eventData, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new BroadcastCustomEvent() { EventData = eventData }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Call a request registered to a vendor.<br />
    /// <br />
    /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
    /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="vendorName">Name of the vendor to use</param>
    /// <param name="requestType">The request type to call</param>
    /// <param name="requestData">Object containing appropriate request data<br />If null, {}</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<CallVendorRequestResponse> CallVendorRequestAsync(string vendorName, string requestType, Dictionary<string, object?>? requestData = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CallVendorRequest() { VendorName = vendorName, VendorRequestType = requestType, RequestData = requestData }, cancellation).ConfigureAwait(false) as CallVendorRequestResponse)!;
    }

    /// <summary>
    /// Gets an array of all hotkey names in OBS<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetHotkeyListResponse> GetHotkeyListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetHotkeyList() { }, cancellation).ConfigureAwait(false) as GetHotkeyListResponse)!;
    }

    /// <summary>
    /// Triggers a hotkey using its name. See <c>GetHotkeyList</c><br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="hotkeyName">Name of the hotkey to trigger</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> TriggerHotkeyByNameAsync(string hotkeyName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new TriggerHotkeyByName() { HotkeyName = hotkeyName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Triggers a hotkey using a sequence of keys.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="keyId">The OBS key ID to use. See https://github.com/obsproject/obs-studio/blob/master/libobs/obs-hotkeys.h<br />If null, Not pressed</param>
    /// <param name="shift">Press Shift<br />If null, Not pressed</param>
    /// <param name="control">Press CTRL<br />If null, Not pressed</param>
    /// <param name="alt">Press ALT<br />If null, Not pressed</param>
    /// <param name="command">Press CMD (Mac)<br />If null, Not pressed</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> TriggerHotkeyByKeySequenceAsync(string? keyId = default, bool? shift = default, bool? control = default, bool? alt = default, bool? command = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new TriggerHotkeyByKeySequence() { KeyId = keyId, KeyModifiers = new KeyModifiers() { Shift = shift, Control = control, Alt = alt, Command = command } }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sleeps for a time duration or number of frames. Only available in request batches with types <c>SERIAL_REALTIME</c> or <c>SERIAL_FRAME</c>.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sleepMillis">Number of milliseconds to sleep for (if <c>SERIAL_REALTIME</c> mode)</param>
    /// <param name="sleepFrames">Number of frames to sleep for (if <c>SERIAL_FRAME</c> mode)</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SleepAsync(int sleepMillis, int sleepFrames, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new Sleep() { SleepMillis = sleepMillis, SleepFrames = sleepFrames }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the value of a "slot" from the selected persistent data realm.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="realm">The data realm to select. <c>OBS_WEBSOCKET_DATA_REALM_GLOBAL</c> or <c>OBS_WEBSOCKET_DATA_REALM_PROFILE</c></param>
    /// <param name="slotName">The name of the slot to retrieve data from</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetPersistentDataResponse> GetPersistentDataAsync(DataRealm realm, string slotName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetPersistentData() { Realm = realm, SlotName = slotName }, cancellation).ConfigureAwait(false) as GetPersistentDataResponse)!;
    }

    /// <summary>
    /// Sets the value of a "slot" from the selected persistent data realm.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="realm">The data realm to select. <c>OBS_WEBSOCKET_DATA_REALM_GLOBAL</c> or <c>OBS_WEBSOCKET_DATA_REALM_PROFILE</c></param>
    /// <param name="slotName">The name of the slot to retrieve data from</param>
    /// <param name="slotValue">The value to apply to the slot</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetPersistentDataAsync(DataRealm realm, string slotName, object? slotValue, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetPersistentData() { Realm = realm, SlotName = slotName, SlotValue = slotValue }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets an array of all scene collections<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sceneCollectionName">Name of the scene collection to switch to</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetCurrentSceneCollectionAsync(string sceneCollectionName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetCurrentSceneCollection() { SceneCollectionName = sceneCollectionName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Creates a new scene collection, switching to it in the process.<br />
    /// <br />
    /// Note: This will block until the collection has finished changing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneCollectionName">Name for the new scene collection</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> CreateSceneCollectionAsync(string sceneCollectionName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CreateSceneCollection() { SceneCollectionName = sceneCollectionName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets an array of all profiles<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetProfileListResponse> GetProfileListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetProfileList() { }, cancellation).ConfigureAwait(false) as GetProfileListResponse)!;
    }

    /// <summary>
    /// Switches to a profile.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="profileName">Name of the profile to switch to</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetCurrentProfileAsync(string profileName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetCurrentProfile() { ProfileName = profileName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Creates a new profile, switching to it in the process<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="profileName">Name for the new profile</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> CreateProfileAsync(string profileName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CreateProfile() { ProfileName = profileName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Removes a profile. If the current profile is chosen, it will change to a different profile first.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="profileName">Name of the profile to remove</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> RemoveProfileAsync(string profileName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new RemoveProfile() { ProfileName = profileName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets a parameter from the current profile's configuration.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="parameterCategory">Category of the parameter to get</param>
    /// <param name="parameterName">Name of the parameter to get</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetProfileParameterResponse> GetProfileParameterAsync(string parameterCategory, string parameterName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetProfileParameter() { ParameterCategory = parameterCategory, ParameterName = parameterName }, cancellation).ConfigureAwait(false) as GetProfileParameterResponse)!;
    }

    /// <summary>
    /// Sets the value of a parameter in the current profile's configuration.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="parameterCategory">Category of the parameter to set</param>
    /// <param name="parameterName">Name of the parameter to set</param>
    /// <param name="parameterValue">Value of the parameter to set. Use <c>null</c> to delete</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetProfileParameterAsync(string parameterCategory, string parameterName, string parameterValue, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetProfileParameter() { ParameterCategory = parameterCategory, ParameterName = parameterName, ParameterValue = parameterValue }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the current video settings.<br />
    /// <br />
    /// Note: To get the true FPS value, divide the FPS numerator by the FPS denominator. Example: <c>60000/1001</c><br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetVideoSettingsResponse> GetVideoSettingsAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetVideoSettings() { }, cancellation).ConfigureAwait(false) as GetVideoSettingsResponse)!;
    }

    /// <summary>
    /// Sets the current video settings.<br />
    /// <br />
    /// Note: Fields must be specified in pairs. For example, you cannot set only <c>baseWidth</c> without needing to specify <c>baseHeight</c>.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="fpsNumerator">Numerator of the fractional FPS value<br />If null, Not changed</param>
    /// <param name="fpsDenominator">Denominator of the fractional FPS value<br />If null, Not changed</param>
    /// <param name="baseWidth">Width of the base (canvas) resolution in pixels<br />If null, Not changed</param>
    /// <param name="baseHeight">Height of the base (canvas) resolution in pixels<br />If null, Not changed</param>
    /// <param name="outputWidth">Width of the output resolution in pixels<br />If null, Not changed</param>
    /// <param name="outputHeight">Height of the output resolution in pixels<br />If null, Not changed</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetVideoSettingsAsync(int? fpsNumerator = default, int? fpsDenominator = default, int? baseWidth = default, int? baseHeight = default, int? outputWidth = default, int? outputHeight = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetVideoSettings() { FpsNumerator = fpsNumerator, FpsDenominator = fpsDenominator, BaseWidth = baseWidth, BaseHeight = baseHeight, OutputWidth = outputWidth, OutputHeight = outputHeight }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the current stream service settings (stream destination).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetStreamServiceSettingsResponse> GetStreamServiceSettingsAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetStreamServiceSettings() { }, cancellation).ConfigureAwait(false) as GetStreamServiceSettingsResponse)!;
    }

    /// <summary>
    /// Sets the current stream service settings (stream destination).<br />
    /// <br />
    /// Note: Simple RTMP settings can be set with type <c>rtmp_custom</c> and the settings fields <c>server</c> and <c>key</c>.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="streamServiceType">Type of stream service to apply. Example: <c>rtmp_common</c> or <c>rtmp_custom</c></param>
    /// <param name="streamServiceSettings">Settings to apply to the service</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetStreamServiceSettingsAsync(StreamServiceType streamServiceType, Dictionary<string, object?> streamServiceSettings, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetStreamServiceSettings() { StreamServiceType = streamServiceType, StreamServiceSettings = streamServiceSettings }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the current directory that the record output is set to.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sourceName">Name of the source to get the active state of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSourceActiveResponse> GetSourceActiveAsync(string sourceName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceActive() { SourceName = sourceName }, cancellation).ConfigureAwait(false) as GetSourceActiveResponse)!;
    }

    /// <summary>
    /// Gets a Base64-encoded screenshot of a source.<br />
    /// <br />
    /// The <c>imageWidth</c> and <c>imageHeight</c> parameters are treated as "scale to inner", meaning the smallest ratio will be used and the aspect ratio of the original resolution is kept.<br />
    /// If <c>imageWidth</c> and <c>imageHeight</c> are not specified, the compressed image will use the full resolution of the source.<br />
    /// <br />
    /// **Compatible with inputs and scenes.**<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source to take a screenshot of</param>
    /// <param name="imageFormat">Image compression format to use. Use <c>GetVersion</c> to get compatible image formats</param>
    /// <param name="imageWidth">Width to scale the screenshot to<br />If null, Source value is used</param>
    /// <param name="imageHeight">Height to scale the screenshot to<br />If null, Source value is used</param>
    /// <param name="imageCompressionQuality">Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk)<br />If null, -1</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSourceScreenshotResponse> GetSourceScreenshotAsync(string sourceName, string imageFormat, int? imageWidth = default, int? imageHeight = default, int? imageCompressionQuality = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceScreenshot() { SourceName = sourceName, ImageFormat = imageFormat, ImageWidth = imageWidth, ImageHeight = imageHeight, ImageCompressionQuality = imageCompressionQuality }, cancellation).ConfigureAwait(false) as GetSourceScreenshotResponse)!;
    }

    /// <summary>
    /// Saves a screenshot of a source to the filesystem.<br />
    /// <br />
    /// The <c>imageWidth</c> and <c>imageHeight</c> parameters are treated as "scale to inner", meaning the smallest ratio will be used and the aspect ratio of the original resolution is kept.<br />
    /// If <c>imageWidth</c> and <c>imageHeight</c> are not specified, the compressed image will use the full resolution of the source.<br />
    /// <br />
    /// **Compatible with inputs and scenes.**<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source to take a screenshot of</param>
    /// <param name="imageFormat">Image compression format to use. Use <c>GetVersion</c> to get compatible image formats</param>
    /// <param name="imageFilePath">Path to save the screenshot file to. Eg. <c>C:\Users\user\Desktop\screenshot.png</c></param>
    /// <param name="imageWidth">Width to scale the screenshot to<br />If null, Source value is used</param>
    /// <param name="imageHeight">Height to scale the screenshot to<br />If null, Source value is used</param>
    /// <param name="imageCompressionQuality">Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk)<br />If null, -1</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SaveSourceScreenshotAsync(string sourceName, string imageFormat, string imageFilePath, int? imageWidth = default, int? imageHeight = default, int? imageCompressionQuality = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SaveSourceScreenshot() { SourceName = sourceName, ImageFormat = imageFormat, ImageFilePath = imageFilePath, ImageWidth = imageWidth, ImageHeight = imageHeight, ImageCompressionQuality = imageCompressionQuality }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets an array of all scenes in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetGroupListResponse> GetGroupListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetGroupList() { }, cancellation).ConfigureAwait(false) as GetGroupListResponse)!;
    }

    /// <summary>
    /// Gets the current program scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetCurrentProgramSceneResponse> GetCurrentProgramSceneAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetCurrentProgramScene() { }, cancellation).ConfigureAwait(false) as GetCurrentProgramSceneResponse)!;
    }

    /// <summary>
    /// Sets the current program scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Scene to set as the current program scene</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetCurrentProgramSceneAsync(string sceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetCurrentProgramScene() { SceneName = sceneName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the current preview scene.<br />
    /// <br />
    /// Only available when studio mode is enabled.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sceneName">Scene to set as the current preview scene</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetCurrentPreviewSceneAsync(string sceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetCurrentPreviewScene() { SceneName = sceneName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Creates a new scene in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name for the new scene</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> CreateSceneAsync(string sceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CreateScene() { SceneName = sceneName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Removes a scene from OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene to remove</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> RemoveSceneAsync(string sceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new RemoveScene() { SceneName = sceneName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sets the name of a scene (rename).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene to be renamed</param>
    /// <param name="newSceneName">New name for the scene</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSceneNameAsync(string sceneName, string newSceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSceneName() { SceneName = sceneName, NewSceneName = newSceneName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the scene transition overridden for a scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSceneSceneTransitionOverrideResponse> GetSceneSceneTransitionOverrideAsync(string sceneName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneSceneTransitionOverride() { SceneName = sceneName }, cancellation).ConfigureAwait(false) as GetSceneSceneTransitionOverrideResponse)!;
    }

    /// <summary>
    /// Gets the scene transition overridden for a scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene</param>
    /// <param name="transitionName">Name of the scene transition to use as override. Specify <c>null</c> to remove<br />If null, Unchanged</param>
    /// <param name="transitionDuration">Duration to use for any overridden transition. Specify <c>null</c> to remove<br />If null, Unchanged</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSceneSceneTransitionOverrideAsync(string sceneName, string? transitionName = default, int? transitionDuration = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSceneSceneTransitionOverride() { SceneName = sceneName, TransitionName = transitionName, TransitionDuration = transitionDuration }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets an array of all inputs in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputKind">Restrict the array to only inputs of the specified kind<br />If null, All kinds included</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputListResponse> GetInputListAsync(string? inputKind = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputList() { InputKind = inputKind }, cancellation).ConfigureAwait(false) as GetInputListResponse)!;
    }

    /// <summary>
    /// Gets an array of all available input kinds in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="unversioned">True == Return all kinds as unversioned, False == Return with version suffixes (if available)<br />If null, false</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputKindListResponse> GetInputKindListAsync(bool? unversioned = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputKindList() { Unversioned = unversioned }, cancellation).ConfigureAwait(false) as GetInputKindListResponse)!;
    }

    /// <summary>
    /// Gets the names of all special inputs.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSpecialInputsResponse> GetSpecialInputsAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSpecialInputs() { }, cancellation).ConfigureAwait(false) as GetSpecialInputsResponse)!;
    }

    /// <summary>
    /// Creates a new input, adding it as a scene item to the specified scene.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene to add the input to as a scene item</param>
    /// <param name="inputName">Name of the new input to created</param>
    /// <param name="inputKind">The kind of input to be created</param>
    /// <param name="inputSettings">Settings object to initialize the input with<br />If null, Default settings used</param>
    /// <param name="sceneItemEnabled">Whether to set the created scene item to enabled or disabled<br />If null, True</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<CreateInputResponse> CreateInputAsync(string sceneName, string inputName, string inputKind, Dictionary<string, object?>? inputSettings = default, bool? sceneItemEnabled = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CreateInput() { SceneName = sceneName, InputName = inputName, InputKind = inputKind, InputSettings = inputSettings, SceneItemEnabled = sceneItemEnabled }, cancellation).ConfigureAwait(false) as CreateInputResponse)!;
    }

    /// <summary>
    /// Removes an existing input.<br />
    /// <br />
    /// Note: Will immediately remove all associated scene items.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to remove</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> RemoveInputAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new RemoveInput() { InputName = inputName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sets the name of an input (rename).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Current input name</param>
    /// <param name="newInputName">New name for the input</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetInputNameAsync(string inputName, string newInputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetInputName() { InputName = inputName, NewInputName = newInputName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the default settings for an input kind.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputKind">Input kind to get the default settings for</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputDefaultSettingsResponse> GetInputDefaultSettingsAsync(string inputKind, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputDefaultSettings() { InputKind = inputKind }, cancellation).ConfigureAwait(false) as GetInputDefaultSettingsResponse)!;
    }

    /// <summary>
    /// Gets the settings of an input.<br />
    /// <br />
    /// Note: Does not include defaults. To create the entire settings object, overlay <c>inputSettings</c> over the <c>defaultInputSettings</c> provided by <c>GetInputDefaultSettings</c>.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to get the settings of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputSettingsResponse> GetInputSettingsAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputSettings() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputSettingsResponse)!;
    }

    /// <summary>
    /// Sets the settings of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to set the settings of</param>
    /// <param name="inputSettings">Object of settings to apply</param>
    /// <param name="overlay">True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.<br />If null, true</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetInputSettingsAsync(string inputName, Dictionary<string, object?> inputSettings, bool? overlay = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetInputSettings() { InputName = inputName, InputSettings = inputSettings, Overlay = overlay }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the audio mute state of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of input to get the mute state of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputMuteResponse> GetInputMuteAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputMute() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputMuteResponse)!;
    }

    /// <summary>
    /// Sets the audio mute state of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to set the mute state of</param>
    /// <param name="inputMuted">Whether to mute the input or not</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetInputMuteAsync(string inputName, bool inputMuted, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetInputMute() { InputName = inputName, InputMuted = inputMuted }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Toggles the audio mute state of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to toggle the mute state of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<ToggleInputMuteResponse> ToggleInputMuteAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleInputMute() { InputName = inputName }, cancellation).ConfigureAwait(false) as ToggleInputMuteResponse)!;
    }

    /// <summary>
    /// Gets the current volume setting of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to get the volume of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputVolumeResponse> GetInputVolumeAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputVolume() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputVolumeResponse)!;
    }

    /// <summary>
    /// Sets the volume setting of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to set the volume of</param>
    /// <param name="inputVolumeMul">Volume setting in mul<br />If null, <c>inputVolumeDb</c> should be specified</param>
    /// <param name="inputVolumeDb">Volume setting in dB<br />If null, <c>inputVolumeMul</c> should be specified</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetInputVolumeAsync(string inputName, double? inputVolumeMul = default, double? inputVolumeDb = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetInputVolume() { InputName = inputName, InputVolumeMul = inputVolumeMul, InputVolumeDb = inputVolumeDb }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the audio balance of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to get the audio balance of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputAudioBalanceResponse> GetInputAudioBalanceAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputAudioBalance() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputAudioBalanceResponse)!;
    }

    /// <summary>
    /// Sets the audio balance of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to set the audio balance of</param>
    /// <param name="inputAudioBalance">New audio balance value</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetInputAudioBalanceAsync(string inputName, double inputAudioBalance, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetInputAudioBalance() { InputName = inputName, InputAudioBalance = inputAudioBalance }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the audio sync offset of an input.<br />
    /// <br />
    /// Note: The audio sync offset can be negative too!<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to get the audio sync offset of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputAudioSyncOffsetResponse> GetInputAudioSyncOffsetAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputAudioSyncOffset() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputAudioSyncOffsetResponse)!;
    }

    /// <summary>
    /// Sets the audio sync offset of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to set the audio sync offset of</param>
    /// <param name="inputAudioSyncOffset">New audio sync offset in milliseconds</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetInputAudioSyncOffsetAsync(string inputName, int inputAudioSyncOffset, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetInputAudioSyncOffset() { InputName = inputName, InputAudioSyncOffset = inputAudioSyncOffset }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the audio monitor type of an input.<br />
    /// <br />
    /// The available audio monitor types are:<br />
    /// <br />
    /// - <c>OBS_MONITORING_TYPE_NONE</c><br />
    /// - <c>OBS_MONITORING_TYPE_MONITOR_ONLY</c><br />
    /// - <c>OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT</c><br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to get the audio monitor type of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputAudioMonitorTypeResponse> GetInputAudioMonitorTypeAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputAudioMonitorType() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputAudioMonitorTypeResponse)!;
    }

    /// <summary>
    /// Sets the audio monitor type of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to set the audio monitor type of</param>
    /// <param name="monitorType">Audio monitor type</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetInputAudioMonitorTypeAsync(string inputName, MonitoringType monitorType, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetInputAudioMonitorType() { InputName = inputName, MonitorType = monitorType }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the enable state of all audio tracks of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputAudioTracksResponse> GetInputAudioTracksAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputAudioTracks() { InputName = inputName }, cancellation).ConfigureAwait(false) as GetInputAudioTracksResponse)!;
    }

    /// <summary>
    /// Sets the enable state of audio tracks of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input</param>
    /// <param name="inputAudioTracks">Track settings to apply</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetInputAudioTracksAsync(string inputName, Dictionary<string, object?> inputAudioTracks, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetInputAudioTracks() { InputName = inputName, InputAudioTracks = inputAudioTracks }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the items of a list property from an input's properties.<br />
    /// <br />
    /// Note: Use this in cases where an input provides a dynamic, selectable list of items. For example, display capture, where it provides a list of available displays.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input</param>
    /// <param name="propertyName">Name of the list property to get the items of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetInputPropertiesListPropertyItemsResponse> GetInputPropertiesListPropertyItemsAsync(string inputName, string propertyName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetInputPropertiesListPropertyItems() { InputName = inputName, PropertyName = propertyName }, cancellation).ConfigureAwait(false) as GetInputPropertiesListPropertyItemsResponse)!;
    }

    /// <summary>
    /// Presses a button in the properties of an input.<br />
    /// <br />
    /// Some known <c>propertyName</c> values are:<br />
    /// <br />
    /// - <c>refreshnocache</c> - Browser source reload button<br />
    /// <br />
    /// Note: Use this in cases where there is a button in the properties of an input that cannot be accessed in any other way. For example, browser sources, where there is a refresh button.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input</param>
    /// <param name="propertyName">Name of the button property to press</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> PressInputPropertiesButtonAsync(string inputName, string propertyName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new PressInputPropertiesButton() { InputName = inputName, PropertyName = propertyName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets an array of all available transition kinds.<br />
    /// <br />
    /// Similar to <c>GetInputKindList</c><br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetTransitionKindListResponse> GetTransitionKindListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetTransitionKindList() { }, cancellation).ConfigureAwait(false) as GetTransitionKindListResponse)!;
    }

    /// <summary>
    /// Gets an array of all scene transitions in OBS.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSceneTransitionListResponse> GetSceneTransitionListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneTransitionList() { }, cancellation).ConfigureAwait(false) as GetSceneTransitionListResponse)!;
    }

    /// <summary>
    /// Gets information about the current scene transition.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="transitionName">Name of the transition to make active</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetCurrentSceneTransitionAsync(string transitionName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetCurrentSceneTransition() { TransitionName = transitionName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sets the duration of the current scene transition, if it is not fixed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="transitionDuration">Duration in milliseconds</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetCurrentSceneTransitionDurationAsync(int transitionDuration, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetCurrentSceneTransitionDuration() { TransitionDuration = transitionDuration }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sets the settings of the current scene transition.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="transitionSettings">Settings object to apply to the transition. Can be <c>{}</c></param>
    /// <param name="overlay">Whether to overlay over the current settings or replace them<br />If null, true</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetCurrentSceneTransitionSettingsAsync(Dictionary<string, object?> transitionSettings, bool? overlay = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetCurrentSceneTransitionSettings() { TransitionSettings = transitionSettings, Overlay = overlay }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the cursor position of the current scene transition.<br />
    /// <br />
    /// Note: <c>transitionCursor</c> will return 1.0 when the transition is inactive.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetCurrentSceneTransitionCursorResponse> GetCurrentSceneTransitionCursorAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetCurrentSceneTransitionCursor() { }, cancellation).ConfigureAwait(false) as GetCurrentSceneTransitionCursorResponse)!;
    }

    /// <summary>
    /// Triggers the current scene transition. Same functionality as the <c>Transition</c> button in studio mode.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> TriggerStudioModeTransitionAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new TriggerStudioModeTransition() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sets the position of the TBar.<br />
    /// <br />
    /// **Very important note**: This will be deprecated and replaced in a future version of obs-websocket.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="position">New position</param>
    /// <param name="release">Whether to release the TBar. Only set <c>false</c> if you know that you will be sending another position update<br />If null, <c>true</c></param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetTBarPositionAsync(double position, bool? release = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetTBarPosition() { Position = position, Release = release }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets an array of all of a source's filters.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSourceFilterListResponse> GetSourceFilterListAsync(string sourceName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceFilterList() { SourceName = sourceName }, cancellation).ConfigureAwait(false) as GetSourceFilterListResponse)!;
    }

    /// <summary>
    /// Gets the default settings for a filter kind.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="filterKind">Filter kind to get the default settings for</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSourceFilterDefaultSettingsResponse> GetSourceFilterDefaultSettingsAsync(string filterKind, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceFilterDefaultSettings() { FilterKind = filterKind }, cancellation).ConfigureAwait(false) as GetSourceFilterDefaultSettingsResponse)!;
    }

    /// <summary>
    /// Creates a new filter, adding it to the specified source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source to add the filter to</param>
    /// <param name="filterName">Name of the new filter to be created</param>
    /// <param name="filterKind">The kind of filter to be created</param>
    /// <param name="filterSettings">Settings object to initialize the filter with<br />If null, Default settings used</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> CreateSourceFilterAsync(string sourceName, string filterName, string filterKind, Dictionary<string, object?>? filterSettings = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CreateSourceFilter() { SourceName = sourceName, FilterName = filterName, FilterKind = filterKind, FilterSettings = filterSettings }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Removes a filter from a source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source the filter is on</param>
    /// <param name="filterName">Name of the filter to remove</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> RemoveSourceFilterAsync(string sourceName, string filterName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new RemoveSourceFilter() { SourceName = sourceName, FilterName = filterName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sets the name of a source filter (rename).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source the filter is on</param>
    /// <param name="filterName">Current name of the filter</param>
    /// <param name="newFilterName">New name for the filter</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSourceFilterNameAsync(string sourceName, string filterName, string newFilterName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSourceFilterName() { SourceName = sourceName, FilterName = filterName, NewFilterName = newFilterName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the info for a specific source filter.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source</param>
    /// <param name="filterName">Name of the filter</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSourceFilterResponse> GetSourceFilterAsync(string sourceName, string filterName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSourceFilter() { SourceName = sourceName, FilterName = filterName }, cancellation).ConfigureAwait(false) as GetSourceFilterResponse)!;
    }

    /// <summary>
    /// Sets the index position of a filter on a source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source the filter is on</param>
    /// <param name="filterName">Name of the filter</param>
    /// <param name="filterIndex">New index position of the filter</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSourceFilterIndexAsync(string sourceName, string filterName, int filterIndex, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSourceFilterIndex() { SourceName = sourceName, FilterName = filterName, FilterIndex = filterIndex }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sets the settings of a source filter.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source the filter is on</param>
    /// <param name="filterName">Name of the filter to set the settings of</param>
    /// <param name="filterSettings">Object of settings to apply</param>
    /// <param name="overlay">True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.<br />If null, true</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSourceFilterSettingsAsync(string sourceName, string filterName, Dictionary<string, object?> filterSettings, bool? overlay = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSourceFilterSettings() { SourceName = sourceName, FilterName = filterName, FilterSettings = filterSettings, Overlay = overlay }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sets the enable state of a source filter.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source the filter is on</param>
    /// <param name="filterName">Name of the filter</param>
    /// <param name="filterEnabled">New enable state of the filter</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSourceFilterEnabledAsync(string sourceName, string filterName, bool filterEnabled, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSourceFilterEnabled() { SourceName = sourceName, FilterName = filterName, FilterEnabled = filterEnabled }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets a list of all scene items in a scene.<br />
    /// <br />
    /// Scenes only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene to get the items of</param>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sceneName">Name of the group to get the items of</param>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sceneName">Name of the scene or group to search in</param>
    /// <param name="sourceName">Name of the source to find</param>
    /// <param name="searchOffset">Number of matches to skip during search. &gt;= 0 means first forward. -1 means last (top) item<br />If null, 0</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSceneItemIdResponse> GetSceneItemIdAsync(string sceneName, string sourceName, int? searchOffset = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemId() { SceneName = sceneName, SourceName = sourceName, SearchOffset = searchOffset }, cancellation).ConfigureAwait(false) as GetSceneItemIdResponse)!;
    }

    /// <summary>
    /// Creates a new scene item using a source.<br />
    /// <br />
    /// Scenes only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene to create the new item in</param>
    /// <param name="sourceName">Name of the source to add to the scene</param>
    /// <param name="sceneItemEnabled">Enable state to apply to the scene item on creation<br />If null, True</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<CreateSceneItemResponse> CreateSceneItemAsync(string sceneName, string sourceName, bool? sceneItemEnabled = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new CreateSceneItem() { SceneName = sceneName, SourceName = sourceName, SceneItemEnabled = sceneItemEnabled }, cancellation).ConfigureAwait(false) as CreateSceneItemResponse)!;
    }

    /// <summary>
    /// Removes a scene item from a scene.<br />
    /// <br />
    /// Scenes only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> RemoveSceneItemAsync(string sceneName, int sceneItemId, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new RemoveSceneItem() { SceneName = sceneName, SceneItemId = sceneItemId }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Duplicates a scene item, copying all transform and crop info.<br />
    /// <br />
    /// Scenes only<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="destinationSceneName">Name of the scene to create the duplicated item in<br />If null, <c>sceneName</c> is assumed</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<DuplicateSceneItemResponse> DuplicateSceneItemAsync(string sceneName, int sceneItemId, string? destinationSceneName = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new DuplicateSceneItem() { SceneName = sceneName, SceneItemId = sceneItemId, DestinationSceneName = destinationSceneName }, cancellation).ConfigureAwait(false) as DuplicateSceneItemResponse)!;
    }

    /// <summary>
    /// Gets the transform and crop info of a scene item.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetSceneItemTransformResponse> GetSceneItemTransformAsync(string sceneName, int sceneItemId, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetSceneItemTransform() { SceneName = sceneName, SceneItemId = sceneItemId }, cancellation).ConfigureAwait(false) as GetSceneItemTransformResponse)!;
    }

    /// <summary>
    /// Sets the transform and crop info of a scene item.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="sceneItemTransform">Object containing scene item transform info to update</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSceneItemTransformAsync(string sceneName, int sceneItemId, Dictionary<string, object?> sceneItemTransform, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSceneItemTransform() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemTransform = sceneItemTransform }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the enable state of a scene item.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="sceneItemEnabled">New enable state of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSceneItemEnabledAsync(string sceneName, int sceneItemId, bool sceneItemEnabled, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSceneItemEnabled() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemEnabled = sceneItemEnabled }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the lock state of a scene item.<br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="sceneItemLocked">New lock state of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSceneItemLockedAsync(string sceneName, int sceneItemId, bool sceneItemLocked, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSceneItemLocked() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemLocked = sceneItemLocked }, cancellation).ConfigureAwait(false) as RequestResponse)!;
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
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="sceneItemIndex">New index position of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSceneItemIndexAsync(string sceneName, int sceneItemId, int sceneItemIndex, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSceneItemIndex() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemIndex = sceneItemIndex }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the blend mode of a scene item.<br />
    /// <br />
    /// Blend modes:<br />
    /// <br />
    /// - <c>OBS_BLEND_NORMAL</c><br />
    /// - <c>OBS_BLEND_ADDITIVE</c><br />
    /// - <c>OBS_BLEND_SUBTRACT</c><br />
    /// - <c>OBS_BLEND_SCREEN</c><br />
    /// - <c>OBS_BLEND_MULTIPLY</c><br />
    /// - <c>OBS_BLEND_LIGHTEN</c><br />
    /// - <c>OBS_BLEND_DARKEN</c><br />
    /// <br />
    /// Scenes and Groups<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="sceneName">Name of the scene the item is in</param>
    /// <param name="sceneItemId">Numeric ID of the scene item</param>
    /// <param name="sceneItemBlendMode">New blend mode</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetSceneItemBlendModeAsync(string sceneName, int sceneItemId, string sceneItemBlendMode, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetSceneItemBlendMode() { SceneName = sceneName, SceneItemId = sceneItemId, SceneItemBlendMode = sceneItemBlendMode }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the status of the virtualcam output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetVirtualCamStatusResponse> GetVirtualCamStatusAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetVirtualCamStatus() { }, cancellation).ConfigureAwait(false) as GetVirtualCamStatusResponse)!;
    }

    /// <summary>
    /// Toggles the state of the virtualcam output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<ToggleVirtualCamResponse> ToggleVirtualCamAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleVirtualCam() { }, cancellation).ConfigureAwait(false) as ToggleVirtualCamResponse)!;
    }

    /// <summary>
    /// Starts the virtualcam output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StartVirtualCamAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StartVirtualCam() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Stops the virtualcam output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StopVirtualCamAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StopVirtualCam() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the status of the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetReplayBufferStatusResponse> GetReplayBufferStatusAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetReplayBufferStatus() { }, cancellation).ConfigureAwait(false) as GetReplayBufferStatusResponse)!;
    }

    /// <summary>
    /// Toggles the state of the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<ToggleReplayBufferResponse> ToggleReplayBufferAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleReplayBuffer() { }, cancellation).ConfigureAwait(false) as ToggleReplayBufferResponse)!;
    }

    /// <summary>
    /// Starts the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StartReplayBufferAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StartReplayBuffer() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Stops the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StopReplayBufferAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StopReplayBuffer() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Saves the contents of the replay buffer output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SaveReplayBufferAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SaveReplayBuffer() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the filename of the last replay buffer save file.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetLastReplayBufferReplayResponse> GetLastReplayBufferReplayAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetLastReplayBufferReplay() { }, cancellation).ConfigureAwait(false) as GetLastReplayBufferReplayResponse)!;
    }

    /// <summary>
    /// Gets the list of available outputs.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetOutputListResponse> GetOutputListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetOutputList() { }, cancellation).ConfigureAwait(false) as GetOutputListResponse)!;
    }

    /// <summary>
    /// Gets the status of an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="outputName">Output name</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetOutputStatusResponse> GetOutputStatusAsync(string outputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetOutputStatus() { OutputName = outputName }, cancellation).ConfigureAwait(false) as GetOutputStatusResponse)!;
    }

    /// <summary>
    /// Toggles the status of an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="outputName">Output name</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<ToggleOutputResponse> ToggleOutputAsync(string outputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleOutput() { OutputName = outputName }, cancellation).ConfigureAwait(false) as ToggleOutputResponse)!;
    }

    /// <summary>
    /// Starts an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="outputName">Output name</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StartOutputAsync(string outputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StartOutput() { OutputName = outputName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Stops an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="outputName">Output name</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StopOutputAsync(string outputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StopOutput() { OutputName = outputName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the settings of an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="outputName">Output name</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetOutputSettingsResponse> GetOutputSettingsAsync(string outputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetOutputSettings() { OutputName = outputName }, cancellation).ConfigureAwait(false) as GetOutputSettingsResponse)!;
    }

    /// <summary>
    /// Sets the settings of an output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="outputName">Output name</param>
    /// <param name="outputSettings">Output settings</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetOutputSettingsAsync(string outputName, Dictionary<string, object?> outputSettings, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetOutputSettings() { OutputName = outputName, OutputSettings = outputSettings }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the status of the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetStreamStatusResponse> GetStreamStatusAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetStreamStatus() { }, cancellation).ConfigureAwait(false) as GetStreamStatusResponse)!;
    }

    /// <summary>
    /// Toggles the status of the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<ToggleStreamResponse> ToggleStreamAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleStream() { }, cancellation).ConfigureAwait(false) as ToggleStreamResponse)!;
    }

    /// <summary>
    /// Starts the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StartStreamAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StartStream() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Stops the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StopStreamAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StopStream() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Sends CEA-608 caption text over the stream output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="captionText">Caption text</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SendStreamCaptionAsync(string captionText, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SendStreamCaption() { CaptionText = captionText }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the status of the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetRecordStatusResponse> GetRecordStatusAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetRecordStatus() { }, cancellation).ConfigureAwait(false) as GetRecordStatusResponse)!;
    }

    /// <summary>
    /// Toggles the status of the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<ToggleRecordResponse> ToggleRecordAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleRecord() { }, cancellation).ConfigureAwait(false) as ToggleRecordResponse)!;
    }

    /// <summary>
    /// Starts the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> StartRecordAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StartRecord() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Stops the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<StopRecordResponse> StopRecordAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new StopRecord() { }, cancellation).ConfigureAwait(false) as StopRecordResponse)!;
    }

    /// <summary>
    /// Toggles pause on the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<ToggleRecordPauseResponse> ToggleRecordPauseAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ToggleRecordPause() { }, cancellation).ConfigureAwait(false) as ToggleRecordPauseResponse)!;
    }

    /// <summary>
    /// Pauses the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> PauseRecordAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new PauseRecord() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Resumes the record output.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> ResumeRecordAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new ResumeRecord() { }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets the status of a media input.<br />
    /// <br />
    /// Media States:<br />
    /// <br />
    /// - <c>OBS_MEDIA_STATE_NONE</c><br />
    /// - <c>OBS_MEDIA_STATE_PLAYING</c><br />
    /// - <c>OBS_MEDIA_STATE_OPENING</c><br />
    /// - <c>OBS_MEDIA_STATE_BUFFERING</c><br />
    /// - <c>OBS_MEDIA_STATE_PAUSED</c><br />
    /// - <c>OBS_MEDIA_STATE_STOPPED</c><br />
    /// - <c>OBS_MEDIA_STATE_ENDED</c><br />
    /// - <c>OBS_MEDIA_STATE_ERROR</c><br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the media input</param>
    /// <param name="cancellation">Token for cancellation</param>
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
    /// <param name="inputName">Name of the media input</param>
    /// <param name="mediaCursor">New cursor position to set</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetMediaInputCursorAsync(string inputName, double mediaCursor, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetMediaInputCursor() { InputName = inputName, MediaCursor = mediaCursor }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Offsets the current cursor position of a media input by the specified value.<br />
    /// <br />
    /// This request does not perform bounds checking of the cursor position.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the media input</param>
    /// <param name="mediaCursorOffset">Value to offset the current cursor position by</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> OffsetMediaInputCursorAsync(string inputName, int mediaCursorOffset, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new OffsetMediaInputCursor() { InputName = inputName, MediaCursorOffset = mediaCursorOffset }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Triggers an action on a media input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the media input</param>
    /// <param name="mediaAction">Identifier of the <c>ObsMediaInputAction</c> enum</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> TriggerMediaInputActionAsync(string inputName, string mediaAction, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new TriggerMediaInputAction() { InputName = inputName, MediaAction = mediaAction }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets whether studio is enabled.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetStudioModeEnabledResponse> GetStudioModeEnabledAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetStudioModeEnabled() { }, cancellation).ConfigureAwait(false) as GetStudioModeEnabledResponse)!;
    }

    /// <summary>
    /// Enables or disables studio mode<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="studioModeEnabled">True == Enabled, False == Disabled</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> SetStudioModeEnabledAsync(bool studioModeEnabled, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new SetStudioModeEnabled() { StudioModeEnabled = studioModeEnabled }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Opens the properties dialog of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to open the dialog of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> OpenInputPropertiesDialogAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new OpenInputPropertiesDialog() { InputName = inputName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Opens the filters dialog of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to open the dialog of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> OpenInputFiltersDialogAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new OpenInputFiltersDialog() { InputName = inputName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Opens the interact dialog of an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="inputName">Name of the input to open the dialog of</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> OpenInputInteractDialogAsync(string inputName, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new OpenInputInteractDialog() { InputName = inputName }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Gets a list of connected monitors and information about them.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<GetMonitorListResponse> GetMonitorListAsync(CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new GetMonitorList() { }, cancellation).ConfigureAwait(false) as GetMonitorListResponse)!;
    }

    /// <summary>
    /// Opens a projector for a specific output video mix.<br />
    /// <br />
    /// Mix types:<br />
    /// <br />
    /// - <c>OBS_WEBSOCKET_VIDEO_MIX_TYPE_PREVIEW</c><br />
    /// - <c>OBS_WEBSOCKET_VIDEO_MIX_TYPE_PROGRAM</c><br />
    /// - <c>OBS_WEBSOCKET_VIDEO_MIX_TYPE_MULTIVIEW</c><br />
    /// <br />
    /// Note: This request serves to provide feature parity with 4.x. It is very likely to be changed/deprecated in a future release.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="videoMixType">Type of mix to open</param>
    /// <param name="monitorIndex">Monitor index, use <c>GetMonitorList</c> to obtain index<br />If null, -1: Opens projector in windowed mode</param>
    /// <param name="projectorGeometry">Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with <c>monitorIndex</c><br />If null, N/A</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> OpenVideoMixProjectorAsync(VideoMixType videoMixType, int? monitorIndex = default, string? projectorGeometry = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new OpenVideoMixProjector() { VideoMixType = videoMixType, MonitorIndex = monitorIndex, ProjectorGeometry = projectorGeometry }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    /// <summary>
    /// Opens a projector for a source.<br />
    /// <br />
    /// Note: This request serves to provide feature parity with 4.x. It is very likely to be changed/deprecated in a future release.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    /// <param name="sourceName">Name of the source to open a projector for</param>
    /// <param name="monitorIndex">Monitor index, use <c>GetMonitorList</c> to obtain index<br />If null, -1: Opens projector in windowed mode</param>
    /// <param name="projectorGeometry">Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with <c>monitorIndex</c><br />If null, N/A</param>
    /// <param name="cancellation">Token for cancellation</param>
    public async Task<RequestResponse> OpenSourceProjectorAsync(string sourceName, int? monitorIndex = default, string? projectorGeometry = default, CancellationToken cancellation = default) {
      return (await _clientSocket.RequestAsync(new OpenSourceProjector() { SourceName = sourceName, MonitorIndex = monitorIndex, ProjectorGeometry = projectorGeometry }, cancellation).ConfigureAwait(false) as RequestResponse)!;
    }

    #endregion

    #region Event dispatch
    private void DispatchEvent(IObsEvent message) {
      try {
        Event(message);
      }
      catch (Exception ex) {
        _logger?.LogWarning(ex, "Event handler throws: {}", message.EventType);
      }
      try {
        DispatchSpecificEvent(message);
      }
      catch (Exception ex) {
        _logger?.LogWarning(ex, "{} event handler throws", message.EventType);
      }
    }

    private void DispatchSpecificEvent(IObsEvent message) {
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
        _logger?.LogWarning("Ignore unclassified event");
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
