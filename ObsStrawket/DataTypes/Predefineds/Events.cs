namespace ObsStrawket.DataTypes.Predefineds {
  using MessagePack;
  using ObsStrawket.DataTypes;
  using System.Collections.Generic;
  /*
  Generated file
  input: https://raw.githubusercontent.com/obsproject/obs-websocket/master/docs/generated/protocol.md
  process: https://gchq.github.io/CyberChef/#recipe=Find_/_Replace(%7B'option':'Regex','string':'%5E.*?(?%3D%5C%5Cn%23%23%20General%20Events)%7C%5C%5Cn%23%20Requests.*$'%7D,'',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5E%23%23%20(%5C%5Cw%2B)%20(%5C%5Cw%2B)?%20?Events'%7D,'public%20class%20$1$2Event%20:%20Event%20%7B%5C%5Cn%7D%5C%5Cn',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'(?:%5E%7C%5C%5Cn)%23%23%23%20(.*?)%5C%5Cn%5C%5Cn(.*?)%5C%5Cn-%20Complexity.*?%5C%5Cn(.*?)%5C%5Cn%5C%5Cn(.*?)(?:---%5C%5Cn%7C(?%3Dpublic)%7C$)'%7D,'///%20%3Csummary%3E%5C%5Cn///%20$2%5C%5Cn$3%5C%5Cn///%20%3C/summary%3E%5C%5Cnpublic%20class%20$1%20:%20Event%20%7B%5C%5Cn$4%5C%5Cn%7D%5C%5Cn',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cn%5C%5C%7C%20Name.*?%5C%5Cn.*?%5C%5Cn'%7D,'',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Array%26lt;(.*?)%26gt;%20%5C%5C%7C'%7D,'%7C%20$1%5B%5D%20%7C',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20String%5C%5Cb'%7D,'%7C%20string',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Any%5C%5Cb'%7D,'%7C%20object?',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Boolean%5C%5Cb'%7D,'%7C%20bool',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Object(%5C%5C%5B%5C%5C%5D)?%20%5C%5C%7C'%7D,'%7C%20Dictionary%3Cstring,%20object?%3E$1%20%7C',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20(%5B%5E%7C%5D*?)%5C%5C%5B%5C%5C%5D%20%5C%5C%7C'%7D,'%7C%20List%3C$1%3E%20%7C',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Number%20%5C%5C%7C(?%3D.*?(?:index%7Coffset%7Cmilli%7Cframe%7Cnumerator%7Cdenominator%7Cpixel%7Cwidth%7Cheight%7Cquality%7C%5C%5Cbid%5C%5Cb%7Cnumber%20of%7Cversion%7Cduration))'%7D,'%7C%20int%20%7C',true,true,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Number%20%5C%5C%7C'%7D,'%7C%20double%20%7C',true,true,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cn%5C%5C*%5C%5C*Data%20Fields:%5C%5C*%5C%5C*'%7D,'',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'(?%3C%3Dpublic%20class%20(%5C%5Cw%2B)%20:%20Event%20%5C%5C%7B%5C%5Cn%5C%5C%7D.*?%3E%5C%5Cn)(public%20class%20%5C%5Cw%2B?%20:%20)%5C%5Cw%2B'%7D,'$2$1',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cn%5C%5C%7C%20(%5C%5C?)?(%5B%5C%5Cw.%5D%2B)%20%5C%5C%7C%20(.*?)%20%5C%5C%7C%20(.*?)%20%5C%5C%7C(?%3D%5C%5Cn)'%7D,'%5C%5Cn///%20%3Csummary%3E%5C%5Cn///%20$4%5C%5Cn///%20%3C/summary%3E%5C%5Cn%5BKey(%22$2%22)%5D%5C%5Cnpublic%20$3$1%20$2%20%7B%20get;%20set;%20%7D',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5E(public%20(?:List%7CDictionary)%3C.*?%3E%20%5C%5Cw%2B%20.*?%5C%5C%7D)'%7D,'$1%20%3D%20new();',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5E(public%20string%20%5C%5Cw%2B%20.*?%5C%5C%7D)'%7D,'$1%20%3D%20%22%22;',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5E(?!///%20)(%5B%5E%5C%5Cn%5D%2B)(?%3D(?:(?!%3Csummary%3E).)*?%3C/summary%3E)'%7D,'///%20$1',true,false,true,true)Find_/_Replace(%7B'option':'Regex','string':'%5E(///%20(?!%5C%5C%3C).*?)%5C%5Cn'%7D,'$1%3Cbr%20/%3E%5C%5Cn',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7B%5C%5Cn%5C%5C%7D'%7D,'%7B%20%7D',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cn%2B'%7D,'%5C%5Cn',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5E%5C%5C%7D%5C%5Cn'%7D,'%7D%5C%5Cn%5C%5Cn',true,false,true,false)Subsection('%5C%5Cw%2B%20%5C%5C%7B',true,true,false)To_Upper_case('Word')Regular_expression('User%20defined','',false,false,true,false,false,false,'Highlight%20matches')
  */


  [MessagePackObject]
  public class GeneralEvent : Event { }
  /// <summary>
  /// OBS has begun the shutdown process.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class ExitStarted : GeneralEvent {
  }

  /// <summary>
  /// An event has been emitted from a vendor.<br />
  /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
  /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class VendorEvent : GeneralEvent {
    /// <summary>
    /// Name of the vendor emitting the event<br />
    /// </summary>
    [Key("vendorName")]
    public string VendorName { get; set; } = "";
    /// <summary>
    /// Vendor-provided event typedef<br />
    /// </summary>
    [Key("eventType")]
    public string VendorEventType { get; set; } = "";
    /// <summary>
    /// Vendor-provided event data. {} if event does not provide any data<br />
    /// </summary>
    [Key("eventData")]
    public Dictionary<string, object?> EventData { get; set; } = new();
  }

  [MessagePackObject]
  public class ConfigEvent : Event { }
  /// <summary>
  /// The current scene collection has begun changing.<br />
  /// Note: We recommend using this event to trigger a pause of all polling requests, as performing any requests during a<br />
  /// scene collection change is considered undefined behavior and can cause crashes!<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class CurrentSceneCollectionChanging : ConfigEvent {
    /// <summary>
    /// Name of the current scene collection<br />
    /// </summary>
    [Key("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }

  /// <summary>
  /// The current scene collection has changed.<br />
  /// Note: If polling has been paused during `CurrentSceneCollectionChanging`, this is the que to restart polling.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class CurrentSceneCollectionChanged : ConfigEvent {
    /// <summary>
    /// Name of the new scene collection<br />
    /// </summary>
    [Key("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }

  /// <summary>
  /// The scene collection list has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneCollectionListChanged : ConfigEvent {
    /// <summary>
    /// Updated list of scene collections<br />
    /// </summary>
    [Key("sceneCollections")]
    public List<string> SceneCollections { get; set; } = new();
  }

  /// <summary>
  /// The current profile has begun changing.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class CurrentProfileChanging : ConfigEvent {
    /// <summary>
    /// Name of the current profile<br />
    /// </summary>
    [Key("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// The current profile has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class CurrentProfileChanged : ConfigEvent {
    /// <summary>
    /// Name of the new profile<br />
    /// </summary>
    [Key("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// The profile list has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class ProfileListChanged : ConfigEvent {
    /// <summary>
    /// Updated list of profiles<br />
    /// </summary>
    [Key("profiles")]
    public List<string> Profiles { get; set; } = new();
  }

  [MessagePackObject]
  public class ScenesEvent : Event { }
  /// <summary>
  /// A new scene has been created.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneCreated : ScenesEvent {
    /// <summary>
    /// Name of the new scene<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Whether the new scene is a group<br />
    /// </summary>
    [Key("isGroup")]
    public bool IsGroup { get; set; }
  }

  /// <summary>
  /// A scene has been removed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneRemoved : ScenesEvent {
    /// <summary>
    /// Name of the removed scene<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Whether the scene was a group<br />
    /// </summary>
    [Key("isGroup")]
    public bool IsGroup { get; set; }
  }

  /// <summary>
  /// The name of a scene has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneNameChanged : ScenesEvent {
    /// <summary>
    /// Old name of the scene<br />
    /// </summary>
    [Key("oldSceneName")]
    public string OldSceneName { get; set; } = "";
    /// <summary>
    /// New name of the scene<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// The current program scene has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class CurrentProgramSceneChanged : ScenesEvent {
    /// <summary>
    /// Name of the scene that was switched to<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// The current preview scene has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class CurrentPreviewSceneChanged : ScenesEvent {
    /// <summary>
    /// Name of the scene that was switched to<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// The list of scenes has changed.<br />
  /// TODO: Make OBS fire this event when scenes are reordered.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneListChanged : ScenesEvent {
    /// <summary>
    /// Updated array of scenes<br />
    /// </summary>
    [Key("scenes")]
    public List<Dictionary<string, object?>> Scenes { get; set; } = new();
  }

  [MessagePackObject]
  public class InputsEvent : Event { }
  /// <summary>
  /// An input has been created.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputCreated : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// The kind of the input<br />
    /// </summary>
    [Key("inputKind")]
    public string InputKind { get; set; } = "";
    /// <summary>
    /// The unversioned kind of input (aka no `_v2` stuff)<br />
    /// </summary>
    [Key("unversionedInputKind")]
    public string UnversionedInputKind { get; set; } = "";
    /// <summary>
    /// The settings configured to the input when it was created<br />
    /// </summary>
    [Key("inputSettings")]
    public Dictionary<string, object?> InputSettings { get; set; } = new();
    /// <summary>
    /// The default settings for the input<br />
    /// </summary>
    [Key("defaultInputSettings")]
    public Dictionary<string, object?> DefaultInputSettings { get; set; } = new();
  }

  /// <summary>
  /// An input has been removed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputRemoved : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// The name of an input has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputNameChanged : InputsEvent {
    /// <summary>
    /// Old name of the input<br />
    /// </summary>
    [Key("oldInputName")]
    public string OldInputName { get; set; } = "";
    /// <summary>
    /// New name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// An input's active state has changed.<br />
  /// When an input is active, it means it's being shown by the program feed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputActiveStateChanged : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Whether the input is active<br />
    /// </summary>
    [Key("videoActive")]
    public bool VideoActive { get; set; }
  }

  /// <summary>
  /// An input's show state has changed.<br />
  /// When an input is showing, it means it's being shown by the preview or a dialog.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputShowStateChanged : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Whether the input is showing<br />
    /// </summary>
    [Key("videoShowing")]
    public bool VideoShowing { get; set; }
  }

  /// <summary>
  /// An input's mute state has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputMuteStateChanged : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Whether the input is muted<br />
    /// </summary>
    [Key("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// An input's volume level has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputVolumeChanged : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// New volume level in multimap<br />
    /// </summary>
    [Key("inputVolumeMul")]
    public double InputVolumeMul { get; set; }
    /// <summary>
    /// New volume level in dB<br />
    /// </summary>
    [Key("inputVolumeDb")]
    public double InputVolumeDb { get; set; }
  }

  /// <summary>
  /// The audio balance value of an input has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputAudioBalanceChanged : InputsEvent {
    /// <summary>
    /// Name of the affected input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// New audio balance value of the input<br />
    /// </summary>
    [Key("inputAudioBalance")]
    public double InputAudioBalance { get; set; }
  }

  /// <summary>
  /// The sync offset of an input has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputAudioSyncOffsetChanged : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// New sync offset in milliseconds<br />
    /// </summary>
    [Key("inputAudioSyncOffset")]
    public int InputAudioSyncOffset { get; set; }
  }

  /// <summary>
  /// The audio tracks of an input have changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputAudioTracksChanged : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Object of audio tracks along with their associated enable states<br />
    /// </summary>
    [Key("inputAudioTracks")]
    public Dictionary<string, object?> InputAudioTracks { get; set; } = new();
  }

  /// <summary>
  /// The monitor type of an input has changed.<br />
  /// Available types are:<br />
  /// - `OBS_MONITORING_TYPE_NONE`<br />
  /// - `OBS_MONITORING_TYPE_MONITOR_ONLY`<br />
  /// - `OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT`<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputAudioMonitorTypeChanged : InputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// New monitor type of the input<br />
    /// </summary>
    [Key("monitorType")]
    public string MonitorType { get; set; } = "";
  }

  /// <summary>
  /// A high-volume event providing volume levels of all active inputs every 50 milliseconds.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class InputVolumeMeters : InputsEvent {
    /// <summary>
    /// Array of active inputs with their associated volume levels<br />
    /// </summary>
    [Key("inputs")]
    public List<Dictionary<string, object?>> Inputs { get; set; } = new();
  }

  [MessagePackObject]
  public class TransitionsEvent : Event { }
  /// <summary>
  /// The current scene transition has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class CurrentSceneTransitionChanged : TransitionsEvent {
    /// <summary>
    /// Name of the new transition<br />
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// The current scene transition duration has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class CurrentSceneTransitionDurationChanged : TransitionsEvent {
    /// <summary>
    /// Transition duration in milliseconds<br />
    /// </summary>
    [Key("transitionDuration")]
    public int TransitionDuration { get; set; }
  }

  /// <summary>
  /// A scene transition has started.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneTransitionStarted : TransitionsEvent {
    /// <summary>
    /// Scene transition name<br />
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// A scene transition has completed fully.<br />
  /// Note: Does not appear to trigger when the transition is interrupted by the user.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneTransitionEnded : TransitionsEvent {
    /// <summary>
    /// Scene transition name<br />
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// A scene transition's video has completed fully.<br />
  /// Useful for stinger transitions to tell when the video *actually* ends.<br />
  /// `SceneTransitionEnded` only signifies the cut point, not the completion of transition playback.<br />
  /// Note: Appears to be called by every transition, regardless of relevance.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneTransitionVideoEnded : TransitionsEvent {
    /// <summary>
    /// Scene transition name<br />
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  [MessagePackObject]
  public class FiltersEvent : Event { }
  /// <summary>
  /// A source's filter list has been reindexed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SourceFilterListReindexed : FiltersEvent {
    /// <summary>
    /// Name of the source<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Array of filter objects<br />
    /// </summary>
    [Key("filters")]
    public List<Dictionary<string, object?>> Filters { get; set; } = new();
  }

  /// <summary>
  /// A filter has been added to a source.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SourceFilterCreated : FiltersEvent {
    /// <summary>
    /// Name of the source the filter was added to<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the filter<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
    /// <summary>
    /// The kind of the filter<br />
    /// </summary>
    [Key("filterKind")]
    public string FilterKind { get; set; } = "";
    /// <summary>
    /// Index position of the filter<br />
    /// </summary>
    [Key("filterIndex")]
    public int FilterIndex { get; set; }
    /// <summary>
    /// The settings configured to the filter when it was created<br />
    /// </summary>
    [Key("filterSettings")]
    public Dictionary<string, object?> FilterSettings { get; set; } = new();
    /// <summary>
    /// The default settings for the filter<br />
    /// </summary>
    [Key("defaultFilterSettings")]
    public Dictionary<string, object?> DefaultFilterSettings { get; set; } = new();
  }

  /// <summary>
  /// A filter has been removed from a source.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SourceFilterRemoved : FiltersEvent {
    /// <summary>
    /// Name of the source the filter was on<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the filter<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
  }

  /// <summary>
  /// The name of a source filter has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SourceFilterNameChanged : FiltersEvent {
    /// <summary>
    /// The source the filter is on<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Old name of the filter<br />
    /// </summary>
    [Key("oldFilterName")]
    public string OldFilterName { get; set; } = "";
    /// <summary>
    /// New name of the filter<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
  }

  /// <summary>
  /// A source filter's enable state has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SourceFilterEnableStateChanged : FiltersEvent {
    /// <summary>
    /// Name of the source the filter is on<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the filter<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
    /// <summary>
    /// Whether the filter is enabled<br />
    /// </summary>
    [Key("filterEnabled")]
    public bool FilterEnabled { get; set; }
  }

  [MessagePackObject]
  public class SceneItemsEvent : Event { }
  /// <summary>
  /// A scene item has been created.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneItemCreated : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item was added to<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Name of the underlying source (input/scene)<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// Index position of the item<br />
    /// </summary>
    [Key("sceneItemIndex")]
    public int SceneItemIndex { get; set; }
  }

  /// <summary>
  /// A scene item has been removed.<br />
  /// This event is not emitted when the scene the item is in is removed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneItemRemoved : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item was removed from<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Name of the underlying source (input/scene)<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// A scene's item list has been reindexed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneItemListReindexed : SceneItemsEvent {
    /// <summary>
    /// Name of the scene<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Array of scene item objects<br />
    /// </summary>
    [Key("sceneItems")]
    public List<Dictionary<string, object?>> SceneItems { get; set; } = new();
  }

  /// <summary>
  /// A scene item's enable state has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneItemEnableStateChanged : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item is in<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// Whether the scene item is enabled (visible)<br />
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// A scene item's lock state has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneItemLockStateChanged : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item is in<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// Whether the scene item is locked<br />
    /// </summary>
    [Key("sceneItemLocked")]
    public bool SceneItemLocked { get; set; }
  }

  /// <summary>
  /// A scene item has been selected in the Ui.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneItemSelected : SceneItemsEvent {
    /// <summary>
    /// Name of the scene the item is in<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// The transform/crop of a scene item has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class SceneItemTransformChanged : SceneItemsEvent {
    /// <summary>
    /// The name of the scene the item is in<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// New transform/crop info of the scene item<br />
    /// </summary>
    [Key("sceneItemTransform")]
    public Dictionary<string, object?> SceneItemTransform { get; set; } = new();
  }

  [MessagePackObject]
  public class OutputsEvent : Event { }
  /// <summary>
  /// The state of the stream output has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class StreamStateChanged : OutputsEvent {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
    /// <summary>
    /// The specific state of the output<br />
    /// </summary>
    [Key("outputState")]
    public OutputState OutputState { get; set; }
  }

  /// <summary>
  /// The state of the record output has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class RecordStateChanged : OutputsEvent {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
    /// <summary>
    /// The specific state of the output<br />
    /// </summary>
    [Key("outputState")]
    public OutputState OutputState { get; set; }
    /// <summary>
    /// File name for the saved recording, if record stopped. `null` otherwise<br />
    /// </summary>
    [Key("outputPath")]
    public string OutputPath { get; set; } = "";
  }

  /// <summary>
  /// The state of the replay buffer output has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class ReplayBufferStateChanged : OutputsEvent {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
    /// <summary>
    /// The specific state of the output<br />
    /// </summary>
    [Key("outputState")]
    public OutputState OutputState { get; set; }
  }

  /// <summary>
  /// The state of the virtualcam output has changed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class VirtualcamStateChanged : OutputsEvent {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
    /// <summary>
    /// The specific state of the output<br />
    /// </summary>
    [Key("outputState")]
    public OutputState OutputState { get; set; }
  }

  /// <summary>
  /// The replay buffer has been saved.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class ReplayBufferSaved : OutputsEvent {
    /// <summary>
    /// Path of the saved replay file<br />
    /// </summary>
    [Key("savedReplayPath")]
    public string SavedReplayPath { get; set; } = "";
  }

  [MessagePackObject]
  public class MediaInputsEvent : Event { }
  /// <summary>
  /// A media input has started playing.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class MediaInputPlaybackStarted : MediaInputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// A media input has finished playing.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class MediaInputPlaybackEnded : MediaInputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// An action has been performed on an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class MediaInputActionTriggered : MediaInputsEvent {
    /// <summary>
    /// Name of the input<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Action performed on the input. See `ObsMediaInputAction` enum<br />
    /// </summary>
    [Key("mediaAction")]
    public MediaInputAction MediaAction { get; set; }
  }

  [MessagePackObject]
  public class UiEvent : Event { }
  /// <summary>
  /// Studio mode has been enabled or disabled.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  [MessagePackObject]
  public class StudioModeStateChanged : UiEvent {
    /// <summary>
    /// True == Enabled, False == Disabled<br />
    /// </summary>
    [Key("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }
}
