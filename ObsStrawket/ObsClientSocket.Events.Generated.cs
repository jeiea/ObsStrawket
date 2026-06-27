#nullable enable

using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using System;

namespace ObsStrawket {

  public partial class ObsClientSocket {

    #region Events

    /// <summary>
    /// All events.
    /// </summary>
    public event Action<IObsEvent> Event = static delegate { };

    /// <summary>
    /// Canvases event group.
    /// </summary>
    public event Action<CanvasesEvent> CanvasesEvent = static delegate { };
    /// <summary>
    /// A new canvas has been created.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.7.0
    /// </summary>
    public event Action<CanvasCreated> CanvasCreated = static delegate { };
    /// <summary>
    /// A canvas has been removed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.7.0
    /// </summary>
    public event Action<CanvasRemoved> CanvasRemoved = static delegate { };
    /// <summary>
    /// The name of a canvas has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.7.0
    /// </summary>
    public event Action<CanvasNameChanged> CanvasNameChanged = static delegate { };

    /// <summary>
    /// General event group.
    /// </summary>
    public event Action<GeneralEvent> GeneralEvent = static delegate { };
    /// <summary>
    /// OBS has begun the shutdown process.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<ExitStarted> ExitStarted = static delegate { };
    /// <summary>
    /// An event has been emitted from a vendor.<br />
    /// <br />
    /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
    /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<VendorEvent> VendorEvent = static delegate { };
    /// <summary>
    /// Custom event emitted by <c>BroadcastCustomEvent</c>.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CustomEvent> CustomEvent = static delegate { };

    /// <summary>
    /// Config event group.
    /// </summary>
    public event Action<ConfigEvent> ConfigEvent = static delegate { };
    /// <summary>
    /// The current scene collection has begun changing.<br />
    /// <br />
    /// Note: We recommend using this event to trigger a pause of all polling requests, as performing any requests during a<br />
    /// scene collection change is considered undefined behavior and can cause crashes!<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentSceneCollectionChanging> CurrentSceneCollectionChanging = static delegate { };
    /// <summary>
    /// The current scene collection has changed.<br />
    /// <br />
    /// Note: If polling has been paused during <c>CurrentSceneCollectionChanging</c>, this is the que to restart polling.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentSceneCollectionChanged> CurrentSceneCollectionChanged = static delegate { };
    /// <summary>
    /// The scene collection list has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneCollectionListChanged> SceneCollectionListChanged = static delegate { };
    /// <summary>
    /// The current profile has begun changing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentProfileChanging> CurrentProfileChanging = static delegate { };
    /// <summary>
    /// The current profile has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentProfileChanged> CurrentProfileChanged = static delegate { };
    /// <summary>
    /// The profile list has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<ProfileListChanged> ProfileListChanged = static delegate { };

    /// <summary>
    /// Scenes event group.
    /// </summary>
    public event Action<ScenesEvent> ScenesEvent = static delegate { };
    /// <summary>
    /// A new scene has been created.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneCreated> SceneCreated = static delegate { };
    /// <summary>
    /// A scene has been removed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneRemoved> SceneRemoved = static delegate { };
    /// <summary>
    /// The name of a scene has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneNameChanged> SceneNameChanged = static delegate { };
    /// <summary>
    /// The current program scene has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentProgramSceneChanged> CurrentProgramSceneChanged = static delegate { };
    /// <summary>
    /// The current preview scene has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentPreviewSceneChanged> CurrentPreviewSceneChanged = static delegate { };
    /// <summary>
    /// The list of scenes has changed.<br />
    /// <br />
    /// TODO: Make OBS fire this event when scenes are reordered.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneListChanged> SceneListChanged = static delegate { };

    /// <summary>
    /// Inputs event group.
    /// </summary>
    public event Action<InputsEvent> InputsEvent = static delegate { };
    /// <summary>
    /// An input has been created.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputCreated> InputCreated = static delegate { };
    /// <summary>
    /// An input has been removed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputRemoved> InputRemoved = static delegate { };
    /// <summary>
    /// The name of an input has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputNameChanged> InputNameChanged = static delegate { };
    /// <summary>
    /// An input's settings have changed (been updated).<br />
    /// <br />
    /// Note: On some inputs, changing values in the properties dialog will cause an immediate update. Pressing the "Cancel" button will revert the settings, resulting in another event being fired.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.4.0
    /// </summary>
    public event Action<InputSettingsChanged> InputSettingsChanged = static delegate { };
    /// <summary>
    /// An input's active state has changed.<br />
    /// <br />
    /// When an input is active, it means it's being shown by the program feed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputActiveStateChanged> InputActiveStateChanged = static delegate { };
    /// <summary>
    /// An input's show state has changed.<br />
    /// <br />
    /// When an input is showing, it means it's being shown by the preview or a dialog.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputShowStateChanged> InputShowStateChanged = static delegate { };
    /// <summary>
    /// An input's mute state has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputMuteStateChanged> InputMuteStateChanged = static delegate { };
    /// <summary>
    /// An input's volume level has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputVolumeChanged> InputVolumeChanged = static delegate { };
    /// <summary>
    /// The audio balance value of an input has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputAudioBalanceChanged> InputAudioBalanceChanged = static delegate { };
    /// <summary>
    /// The sync offset of an input has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputAudioSyncOffsetChanged> InputAudioSyncOffsetChanged = static delegate { };
    /// <summary>
    /// The audio tracks of an input have changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputAudioTracksChanged> InputAudioTracksChanged = static delegate { };
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
    public event Action<InputAudioMonitorTypeChanged> InputAudioMonitorTypeChanged = static delegate { };
    /// <summary>
    /// A high-volume event providing volume levels of all active inputs every 50 milliseconds.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<InputVolumeMeters> InputVolumeMeters = static delegate { };

    /// <summary>
    /// Transitions event group.
    /// </summary>
    public event Action<TransitionsEvent> TransitionsEvent = static delegate { };
    /// <summary>
    /// The current scene transition has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentSceneTransitionChanged> CurrentSceneTransitionChanged = static delegate { };
    /// <summary>
    /// The current scene transition duration has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<CurrentSceneTransitionDurationChanged> CurrentSceneTransitionDurationChanged = static delegate { };
    /// <summary>
    /// A scene transition has started.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneTransitionStarted> SceneTransitionStarted = static delegate { };
    /// <summary>
    /// A scene transition has completed fully.<br />
    /// <br />
    /// Note: Does not appear to trigger when the transition is interrupted by the user.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneTransitionEnded> SceneTransitionEnded = static delegate { };
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
    public event Action<SceneTransitionVideoEnded> SceneTransitionVideoEnded = static delegate { };

    /// <summary>
    /// Filters event group.
    /// </summary>
    public event Action<FiltersEvent> FiltersEvent = static delegate { };
    /// <summary>
    /// A source's filter list has been reindexed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterListReindexed> SourceFilterListReindexed = static delegate { };
    /// <summary>
    /// A filter has been added to a source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterCreated> SourceFilterCreated = static delegate { };
    /// <summary>
    /// A filter has been removed from a source.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterRemoved> SourceFilterRemoved = static delegate { };
    /// <summary>
    /// The name of a source filter has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterNameChanged> SourceFilterNameChanged = static delegate { };
    /// <summary>
    /// An source filter's settings have changed (been updated).<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.4.0
    /// </summary>
    public event Action<SourceFilterSettingsChanged> SourceFilterSettingsChanged = static delegate { };
    /// <summary>
    /// A source filter's enable state has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SourceFilterEnableStateChanged> SourceFilterEnableStateChanged = static delegate { };

    /// <summary>
    /// Scene items event group.
    /// </summary>
    public event Action<SceneItemsEvent> SceneItemsEvent = static delegate { };
    /// <summary>
    /// A scene item has been created.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemCreated> SceneItemCreated = static delegate { };
    /// <summary>
    /// A scene item has been removed.<br />
    /// <br />
    /// This event is not emitted when the scene the item is in is removed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemRemoved> SceneItemRemoved = static delegate { };
    /// <summary>
    /// A scene's item list has been reindexed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemListReindexed> SceneItemListReindexed = static delegate { };
    /// <summary>
    /// A scene item's enable state has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemEnableStateChanged> SceneItemEnableStateChanged = static delegate { };
    /// <summary>
    /// A scene item's lock state has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemLockStateChanged> SceneItemLockStateChanged = static delegate { };
    /// <summary>
    /// A scene item has been selected in the Ui.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemSelected> SceneItemSelected = static delegate { };
    /// <summary>
    /// The transform/crop of a scene item has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<SceneItemTransformChanged> SceneItemTransformChanged = static delegate { };

    /// <summary>
    /// Outputs event group.
    /// </summary>
    public event Action<OutputsEvent> OutputsEvent = static delegate { };
    /// <summary>
    /// The state of the stream output has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<StreamStateChanged> StreamStateChanged = static delegate { };
    /// <summary>
    /// The state of the record output has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<RecordStateChanged> RecordStateChanged = static delegate { };
    /// <summary>
    /// The record output has started writing to a new file. For example, when a file split happens.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.5.0
    /// </summary>
    public event Action<RecordFileChanged> RecordFileChanged = static delegate { };
    /// <summary>
    /// The state of the replay buffer output has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<ReplayBufferStateChanged> ReplayBufferStateChanged = static delegate { };
    /// <summary>
    /// The state of the virtualcam output has changed.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<VirtualcamStateChanged> VirtualcamStateChanged = static delegate { };
    /// <summary>
    /// The replay buffer has been saved.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<ReplayBufferSaved> ReplayBufferSaved = static delegate { };

    /// <summary>
    /// Media inputs event group.
    /// </summary>
    public event Action<MediaInputsEvent> MediaInputsEvent = static delegate { };
    /// <summary>
    /// A media input has started playing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<MediaInputPlaybackStarted> MediaInputPlaybackStarted = static delegate { };
    /// <summary>
    /// A media input has finished playing.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<MediaInputPlaybackEnded> MediaInputPlaybackEnded = static delegate { };
    /// <summary>
    /// An action has been performed on an input.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<MediaInputActionTriggered> MediaInputActionTriggered = static delegate { };

    /// <summary>
    /// Ui event group.
    /// </summary>
    public event Action<UiEvent> UiEvent = static delegate { };
    /// <summary>
    /// Studio mode has been enabled or disabled.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.0.0
    /// </summary>
    public event Action<StudioModeStateChanged> StudioModeStateChanged = static delegate { };
    /// <summary>
    /// A screenshot has been saved.<br />
    /// <br />
    /// Note: Triggered for the screenshot feature available in <c>Settings -&gt; Hotkeys -&gt; Screenshot Output</c> ONLY.<br />
    /// Applications using <c>Get/SaveSourceScreenshot</c> should implement a <c>CustomEvent</c> if this kind of inter-client<br />
    /// communication is desired.<br />
    /// Latest supported RPC version: 1<br />
    /// Added in: 5.1.0
    /// </summary>
    public event Action<ScreenshotSaved> ScreenshotSaved = static delegate { };

    #endregion
  }
}