using MessagePack;
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
    [Key("vendorName")]
    public string VendorName { get; set; } = "";

    /// <summary>
    /// Vendor-provided event typedef
    /// </summary>
    [Key("eventType")]
    public string VendorEventType { get; set; } = "";

    /// <summary>
    /// Vendor-provided event data. {} if event does not provide any data
    /// </summary>
    [Key("eventData")]
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
    [Key("sceneCollectionName")]
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
    [Key("sceneCollectionName")]
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
    [Key("sceneCollections")]
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
    [Key("profileName")]
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
    [Key("profileName")]
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
    [Key("profiles")]
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Whether the new scene is a group
    /// </summary>
    [Key("isGroup")]
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Whether the scene was a group
    /// </summary>
    [Key("isGroup")]
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
    [Key("oldSceneName")]
    public string OldSceneName { get; set; } = "";

    /// <summary>
    /// New name of the scene
    /// </summary>
    [Key("sceneName")]
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
    [Key("sceneName")]
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
    [Key("sceneName")]
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
    [Key("scenes")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// The kind of the input
    /// </summary>
    [Key("inputKind")]
    public string InputKind { get; set; } = "";

    /// <summary>
    /// The unversioned kind of input (aka no <c>_v2</c> stuff)
    /// </summary>
    [Key("unversionedInputKind")]
    public string UnversionedInputKind { get; set; } = "";

    /// <summary>
    /// The settings configured to the input when it was created
    /// </summary>
    [Key("inputSettings")]
    public Dictionary<string, object?> InputSettings { get; set; } = new();

    /// <summary>
    /// The default settings for the input
    /// </summary>
    [Key("defaultInputSettings")]
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
    [Key("inputName")]
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
    [Key("oldInputName")]
    public string OldInputName { get; set; } = "";

    /// <summary>
    /// New name of the input
    /// </summary>
    [Key("inputName")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Whether the input is active
    /// </summary>
    [Key("videoActive")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Whether the input is showing
    /// </summary>
    [Key("videoShowing")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Whether the input is muted
    /// </summary>
    [Key("inputMuted")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New volume level in multimap
    /// </summary>
    [Key("inputVolumeMul")]
    public double InputVolumeMul { get; set; }

    /// <summary>
    /// New volume level in dB
    /// </summary>
    [Key("inputVolumeDb")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New audio balance value of the input
    /// </summary>
    [Key("inputAudioBalance")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New sync offset in milliseconds
    /// </summary>
    [Key("inputAudioSyncOffset")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Object of audio tracks along with their associated enable states
    /// </summary>
    [Key("inputAudioTracks")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New monitor type of the input
    /// </summary>
    [Key("monitorType")]
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
    [Key("inputs")]
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
    [Key("transitionName")]
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
    [Key("transitionDuration")]
    public int TransitionDuration { get; set; }
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
    [Key("transitionName")]
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
    [Key("transitionName")]
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
    [Key("transitionName")]
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
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Array of filter objects
    /// </summary>
    [Key("filters")]
    public List<Dictionary<string, object?>> Filters { get; set; } = new();
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
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the filter
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// The kind of the filter
    /// </summary>
    [Key("filterKind")]
    public string FilterKind { get; set; } = "";

    /// <summary>
    /// Index position of the filter
    /// </summary>
    [Key("filterIndex")]
    public int FilterIndex { get; set; }

    /// <summary>
    /// The settings configured to the filter when it was created
    /// </summary>
    [Key("filterSettings")]
    public Dictionary<string, object?> FilterSettings { get; set; } = new();

    /// <summary>
    /// The default settings for the filter
    /// </summary>
    [Key("defaultFilterSettings")]
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
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the filter
    /// </summary>
    [Key("filterName")]
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
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Old name of the filter
    /// </summary>
    [Key("oldFilterName")]
    public string OldFilterName { get; set; } = "";

    /// <summary>
    /// New name of the filter
    /// </summary>
    [Key("filterName")]
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
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the filter
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// Whether the filter is enabled
    /// </summary>
    [Key("filterEnabled")]
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Name of the underlying source (input/scene)
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Index position of the item
    /// </summary>
    [Key("sceneItemIndex")]
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Name of the underlying source (input/scene)
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [Key("sceneItemId")]
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Array of scene item objects
    /// </summary>
    [Key("sceneItems")]
    public List<Dictionary<string, object?>> SceneItems { get; set; } = new();
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Whether the scene item is enabled (visible)
    /// </summary>
    [Key("sceneItemEnabled")]
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Whether the scene item is locked
    /// </summary>
    [Key("sceneItemLocked")]
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [Key("sceneItemId")]
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
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New transform/crop info of the scene item
    /// </summary>
    [Key("sceneItemTransform")]
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
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// The specific state of the output
    /// </summary>
    [Key("outputState")]
    public OutputState OutputState { get; set; } = new();
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
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// The specific state of the output
    /// </summary>
    [Key("outputState")]
    public OutputState OutputState { get; set; } = new();

    /// <summary>
    /// File name for the saved recording, if record stopped. <c>null</c> otherwise
    /// </summary>
    [Key("outputPath")]
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
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// The specific state of the output
    /// </summary>
    [Key("outputState")]
    public OutputState OutputState { get; set; } = new();
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
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// The specific state of the output
    /// </summary>
    [Key("outputState")]
    public OutputState OutputState { get; set; } = new();
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
    [Key("savedReplayPath")]
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
    [Key("inputName")]
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
    [Key("inputName")]
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
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Action performed on the input. See <c>ObsMediaInputAction</c> enum
    /// </summary>
    [Key("mediaAction")]
    public string MediaAction { get; set; } = "";
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
    [Key("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }
}
