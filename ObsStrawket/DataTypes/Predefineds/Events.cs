using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ObsStrawket.DataTypes.Predefineds {
  /// <summary>
  /// General event.
  /// </summary>
  [MessagePackObject]
  public class GeneralEvent : ObsEvent { }

  /// <summary>
  /// Config event.
  /// </summary>
  [MessagePackObject]
  public class ConfigEvent : ObsEvent { }

  /// <summary>
  /// Scenes event.
  /// </summary>
  [MessagePackObject]
  public class ScenesEvent : ObsEvent { }

  /// <summary>
  /// Inputs event.
  /// </summary>
  [MessagePackObject]
  public class InputsEvent : ObsEvent { }

  /// <summary>
  /// Transitions event.
  /// </summary>
  [MessagePackObject]
  public class TransitionsEvent : ObsEvent { }

  /// <summary>
  /// Filters event.
  /// </summary>
  [MessagePackObject]
  public class FiltersEvent : ObsEvent { }

  /// <summary>
  /// Scene items event.
  /// </summary>
  [MessagePackObject]
  public class SceneItemsEvent : ObsEvent { }

  /// <summary>
  /// Outputs event.
  /// </summary>
  [MessagePackObject]
  public class OutputsEvent : ObsEvent { }

  /// <summary>
  /// Media inputs event.
  /// </summary>
  [MessagePackObject]
  public class MediaInputsEvent : ObsEvent { }

  /// <summary>
  /// Ui event.
  /// </summary>
  [MessagePackObject]
  public class UiEvent : ObsEvent { }

  /// <summary>
  /// OBS has begun the shutdown process.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ExitStarted : GeneralEvent { }

  /// <summary>
  /// An event has been emitted from a vendor.<br />
  /// <br />
  /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
  /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class VendorEvent : GeneralEvent {
    /// <summary>
    /// Name of the vendor emitting the event
    /// </summary>
    [JsonPropertyName("vendorName")]
    public string VendorName { get; set; } = "";

    /// <summary>
    /// Vendor-provided event typedef
    /// </summary>
    [JsonPropertyName("eventType")]
    public string VendorEventType { get; set; } = "";

    /// <summary>
    /// Vendor-provided event data. {} if event does not provide any data
    /// </summary>
    [JsonPropertyName("eventData")]
    public Dictionary<string, object?> EventData { get; set; } = new();
  }

  /// <summary>
  /// Custom event emitted by <c>BroadcastCustomEvent</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CustomEvent : GeneralEvent {
    /// <summary>
    /// Custom event data
    /// </summary>
    [JsonPropertyName("eventData")]
    public Dictionary<string, object?> EventData { get; set; } = new();
  }

  /// <summary>
  /// The current scene collection has begun changing.<br />
  /// <br />
  /// Note: We recommend using this event to trigger a pause of all polling requests, as performing any requests during a<br />
  /// scene collection change is considered undefined behavior and can cause crashes!<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CurrentSceneCollectionChanging : ConfigEvent {
    /// <summary>
    /// Name of the current scene collection
    /// </summary>
    [JsonPropertyName("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }

  /// <summary>
  /// The current scene collection has changed.<br />
  /// <br />
  /// Note: If polling has been paused during <c>CurrentSceneCollectionChanging</c>, this is the que to restart polling.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CurrentSceneCollectionChanged : ConfigEvent {
    /// <summary>
    /// Name of the new scene collection
    /// </summary>
    [JsonPropertyName("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }

  /// <summary>
  /// The scene collection list has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneCollectionListChanged : ConfigEvent {
    /// <summary>
    /// Updated list of scene collections
    /// </summary>
    [JsonPropertyName("sceneCollections")]
    public List<string> SceneCollections { get; set; } = new();
  }

  /// <summary>
  /// The current profile has begun changing.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CurrentProfileChanging : ConfigEvent {
    /// <summary>
    /// Name of the current profile
    /// </summary>
    [JsonPropertyName("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// The current profile has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CurrentProfileChanged : ConfigEvent {
    /// <summary>
    /// Name of the new profile
    /// </summary>
    [JsonPropertyName("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// The profile list has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ProfileListChanged : ConfigEvent {
    /// <summary>
    /// Updated list of profiles
    /// </summary>
    [JsonPropertyName("profiles")]
    public List<string> Profiles { get; set; } = new();
  }

  /// <summary>
  /// A new scene has been created.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneCreated : ScenesEvent {
    /// <summary>
    /// Name of the new scene
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Whether the new scene is a group
    /// </summary>
    [JsonPropertyName("isGroup")]
    public bool IsGroup { get; set; }
  }

  /// <summary>
  /// A scene has been removed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneRemoved : ScenesEvent {
    /// <summary>
    /// Name of the removed scene
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Whether the scene was a group
    /// </summary>
    [JsonPropertyName("isGroup")]
    public bool IsGroup { get; set; }
  }

  /// <summary>
  /// The name of a scene has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneNameChanged : ScenesEvent {
    /// <summary>
    /// Old name of the scene
    /// </summary>
    [JsonPropertyName("oldSceneName")]
    public string OldSceneName { get; set; } = "";

    /// <summary>
    /// New name of the scene
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// The current program scene has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CurrentProgramSceneChanged : ScenesEvent {
    /// <summary>
    /// Name of the scene that was switched to
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// The current preview scene has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CurrentPreviewSceneChanged : ScenesEvent {
    /// <summary>
    /// Name of the scene that was switched to
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// The list of scenes has changed.<br />
  /// <br />
  /// TODO: Make OBS fire this event when scenes are reordered.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneListChanged : ScenesEvent {
    /// <summary>
    /// Updated array of scenes
    /// </summary>
    [JsonPropertyName("scenes")]
    public List<Scene> Scenes { get; set; } = new();
  }

  /// <summary>
  /// An input has been created.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputCreated : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// The kind of the input
    /// </summary>
    [JsonPropertyName("inputKind")]
    public string InputKind { get; set; } = "";

    /// <summary>
    /// The unversioned kind of input (aka no <c>_v2</c> stuff)
    /// </summary>
    [JsonPropertyName("unversionedInputKind")]
    public string UnversionedInputKind { get; set; } = "";

    /// <summary>
    /// The settings configured to the input when it was created
    /// </summary>
    [JsonPropertyName("inputSettings")]
    public Dictionary<string, object?> InputSettings { get; set; } = new();

    /// <summary>
    /// The default settings for the input
    /// </summary>
    [JsonPropertyName("defaultInputSettings")]
    public Dictionary<string, object?> DefaultInputSettings { get; set; } = new();
  }

  /// <summary>
  /// An input has been removed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputRemoved : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// The name of an input has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputNameChanged : InputsEvent {
    /// <summary>
    /// Old name of the input
    /// </summary>
    [JsonPropertyName("oldInputName")]
    public string OldInputName { get; set; } = "";

    /// <summary>
    /// New name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// An input's active state has changed.<br />
  /// <br />
  /// When an input is active, it means it's being shown by the program feed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputActiveStateChanged : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Whether the input is active
    /// </summary>
    [JsonPropertyName("videoActive")]
    public bool VideoActive { get; set; }
  }

  /// <summary>
  /// An input's show state has changed.<br />
  /// <br />
  /// When an input is showing, it means it's being shown by the preview or a dialog.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputShowStateChanged : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Whether the input is showing
    /// </summary>
    [JsonPropertyName("videoShowing")]
    public bool VideoShowing { get; set; }
  }

  /// <summary>
  /// An input's mute state has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputMuteStateChanged : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Whether the input is muted
    /// </summary>
    [JsonPropertyName("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// An input's volume level has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputVolumeChanged : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New volume level multiplier
    /// </summary>
    [JsonPropertyName("inputVolumeMul")]
    public double InputVolumeMul { get; set; }

    /// <summary>
    /// New volume level in dB
    /// </summary>
    [JsonPropertyName("inputVolumeDb")]
    public double InputVolumeDb { get; set; }
  }

  /// <summary>
  /// The audio balance value of an input has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputAudioBalanceChanged : InputsEvent {
    /// <summary>
    /// Name of the affected input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New audio balance value of the input
    /// </summary>
    [JsonPropertyName("inputAudioBalance")]
    public double InputAudioBalance { get; set; }
  }

  /// <summary>
  /// The sync offset of an input has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputAudioSyncOffsetChanged : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New sync offset in milliseconds
    /// </summary>
    [JsonPropertyName("inputAudioSyncOffset")]
    public int InputAudioSyncOffset { get; set; }
  }

  /// <summary>
  /// The audio tracks of an input have changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputAudioTracksChanged : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Object of audio tracks along with their associated enable states
    /// </summary>
    [JsonPropertyName("inputAudioTracks")]
    public Dictionary<string, object?> InputAudioTracks { get; set; } = new();
  }

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
  [MessagePackObject]
  public class InputAudioMonitorTypeChanged : InputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New monitor type of the input
    /// </summary>
    [JsonPropertyName("monitorType")]
    public MonitoringType MonitorType { get; set; } = new();
  }

  /// <summary>
  /// A high-volume event providing volume levels of all active inputs every 50 milliseconds.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class InputVolumeMeters : InputsEvent {
    /// <summary>
    /// Array of active inputs with their associated volume levels
    /// </summary>
    [JsonPropertyName("inputs")]
    public List<Input> Inputs { get; set; } = new();
  }

  /// <summary>
  /// The current scene transition has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CurrentSceneTransitionChanged : TransitionsEvent {
    /// <summary>
    /// Name of the new transition
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// The current scene transition duration has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CurrentSceneTransitionDurationChanged : TransitionsEvent {
    /// <summary>
    /// Transition duration in milliseconds
    /// </summary>
    [JsonPropertyName("transitionDuration")]
    public long TransitionDuration { get; set; }
  }

  /// <summary>
  /// A scene transition has started.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneTransitionStarted : TransitionsEvent {
    /// <summary>
    /// Scene transition name
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// A scene transition has completed fully.<br />
  /// <br />
  /// Note: Does not appear to trigger when the transition is interrupted by the user.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneTransitionEnded : TransitionsEvent {
    /// <summary>
    /// Scene transition name
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string TransitionName { get; set; } = "";
  }

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
  [MessagePackObject]
  public class SceneTransitionVideoEnded : TransitionsEvent {
    /// <summary>
    /// Scene transition name
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// A source's filter list has been reindexed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SourceFilterListReindexed : FiltersEvent {
    /// <summary>
    /// Name of the source
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Array of filter objects
    /// </summary>
    [JsonPropertyName("filters")]
    public List<SourceFilter> Filters { get; set; } = new();
  }

  /// <summary>
  /// A filter has been added to a source.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SourceFilterCreated : FiltersEvent {
    /// <summary>
    /// Name of the source the filter was added to
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the filter
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// The kind of the filter
    /// </summary>
    [JsonPropertyName("filterKind")]
    public string FilterKind { get; set; } = "";

    /// <summary>
    /// Index position of the filter
    /// </summary>
    [JsonPropertyName("filterIndex")]
    public int FilterIndex { get; set; }

    /// <summary>
    /// The settings configured to the filter when it was created
    /// </summary>
    [JsonPropertyName("filterSettings")]
    public Dictionary<string, object?> FilterSettings { get; set; } = new();

    /// <summary>
    /// The default settings for the filter
    /// </summary>
    [JsonPropertyName("defaultFilterSettings")]
    public Dictionary<string, object?> DefaultFilterSettings { get; set; } = new();
  }

  /// <summary>
  /// A filter has been removed from a source.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SourceFilterRemoved : FiltersEvent {
    /// <summary>
    /// Name of the source the filter was on
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the filter
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";
  }

  /// <summary>
  /// The name of a source filter has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SourceFilterNameChanged : FiltersEvent {
    /// <summary>
    /// The source the filter is on
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Old name of the filter
    /// </summary>
    [JsonPropertyName("oldFilterName")]
    public string OldFilterName { get; set; } = "";

    /// <summary>
    /// New name of the filter
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";
  }

  /// <summary>
  /// A source filter's enable state has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SourceFilterEnableStateChanged : FiltersEvent {
    /// <summary>
    /// Name of the source the filter is on
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the filter
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// Whether the filter is enabled
    /// </summary>
    [JsonPropertyName("filterEnabled")]
    public bool FilterEnabled { get; set; }
  }

  /// <summary>
  /// A scene item has been created.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneItemCreated : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item was added to
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Name of the underlying source (input/scene)
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Index position of the item
    /// </summary>
    [JsonPropertyName("sceneItemIndex")]
    public int SceneItemIndex { get; set; }
  }

  /// <summary>
  /// A scene item has been removed.<br />
  /// <br />
  /// This event is not emitted when the scene the item is in is removed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneItemRemoved : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item was removed from
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Name of the underlying source (input/scene)
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// A scene's item list has been reindexed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneItemListReindexed : SceneItemsEvent {
    /// <summary>
    /// Name of the scene
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Array of scene item objects
    /// </summary>
    [JsonPropertyName("sceneItems")]
    public List<BasicSceneItem> SceneItems { get; set; } = new();
  }

  /// <summary>
  /// A scene item's enable state has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneItemEnableStateChanged : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Whether the scene item is enabled (visible)
    /// </summary>
    [JsonPropertyName("sceneItemEnabled")]
    public bool SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// A scene item's lock state has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneItemLockStateChanged : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Whether the scene item is locked
    /// </summary>
    [JsonPropertyName("sceneItemLocked")]
    public bool SceneItemLocked { get; set; }
  }

  /// <summary>
  /// A scene item has been selected in the Ui.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneItemSelected : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// The transform/crop of a scene item has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SceneItemTransformChanged : SceneItemsEvent {
    /// <summary>
    /// The name of the scene the item is in
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New transform/crop info of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemTransform")]
    public Dictionary<string, object?> SceneItemTransform { get; set; } = new();
  }

  /// <summary>
  /// The state of the stream output has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StreamStateChanged : OutputsEvent {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// The specific state of the output
    /// </summary>
    [JsonPropertyName("outputState")]
    public ObsOutputState OutputState { get; set; } = new();
  }

  /// <summary>
  /// The state of the record output has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class RecordStateChanged : OutputsEvent {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// The specific state of the output
    /// </summary>
    [JsonPropertyName("outputState")]
    public ObsOutputState OutputState { get; set; } = new();

    /// <summary>
    /// File name for the saved recording, if record stopped. <c>null</c> otherwise
    /// </summary>
    [JsonPropertyName("outputPath")]
    public string? OutputPath { get; set; }
  }

  /// <summary>
  /// The state of the replay buffer output has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ReplayBufferStateChanged : OutputsEvent {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// The specific state of the output
    /// </summary>
    [JsonPropertyName("outputState")]
    public ObsOutputState OutputState { get; set; } = new();
  }

  /// <summary>
  /// The state of the virtualcam output has changed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class VirtualcamStateChanged : OutputsEvent {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// The specific state of the output
    /// </summary>
    [JsonPropertyName("outputState")]
    public ObsOutputState OutputState { get; set; } = new();
  }

  /// <summary>
  /// The replay buffer has been saved.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ReplayBufferSaved : OutputsEvent {
    /// <summary>
    /// Path of the saved replay file
    /// </summary>
    [JsonPropertyName("savedReplayPath")]
    public string SavedReplayPath { get; set; } = "";
  }

  /// <summary>
  /// A media input has started playing.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class MediaInputPlaybackStarted : MediaInputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// A media input has finished playing.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class MediaInputPlaybackEnded : MediaInputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// An action has been performed on an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class MediaInputActionTriggered : MediaInputsEvent {
    /// <summary>
    /// Name of the input
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Action performed on the input. See <c>ObsMediaInputAction</c> enum
    /// </summary>
    [JsonPropertyName("mediaAction")]
    public MediaInputAction MediaAction { get; set; } = new();
  }

  /// <summary>
  /// Studio mode has been enabled or disabled.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StudioModeStateChanged : UiEvent {
    /// <summary>
    /// True == Enabled, False == Disabled
    /// </summary>
    [JsonPropertyName("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }

  /// <summary>
  /// A screenshot has been saved.<br />
  /// <br />
  /// Note: Triggered for the screenshot feature available in <c>Settings -&gt; Hotkeys -&gt; Screenshot Output</c> ONLY.<br />
  /// Applications using <c>Get/SaveSourceScreenshot</c> should implement a <c>CustomEvent</c> if this kind of inter-client<br />
  /// communication is desired.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.1.0
  /// </summary>
  [MessagePackObject]
  public class ScreenshotSaved : UiEvent {
    /// <summary>
    /// Path of the saved image file
    /// </summary>
    [JsonPropertyName("savedScreenshotPath")]
    public string SavedScreenshotPath { get; set; } = "";
  }
}
