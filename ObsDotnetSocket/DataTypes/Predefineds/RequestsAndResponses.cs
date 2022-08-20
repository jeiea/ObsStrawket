namespace ObsDotnetSocket.DataTypes.Predefineds {
  using MessagePack;
  using ObsDotnetSocket.DataTypes;
  using System.Collections.Generic;
  /*
  Generated file
  input: https://raw.githubusercontent.com/obsproject/obs-websocket/master/docs/generated/protocol.md
  process: https://gchq.github.io/CyberChef/#recipe=Find_/_Replace(%7B'option':'Regex','string':'%5E.*?(?%3D%5C%5Cn%23%23%20General%20Requests)'%7D,'',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5E%23%23%20(%5C%5Cw%2B)%20(%5C%5Cw%2B)?%20?Requests'%7D,'public%20class%20$1$2Request%20:%20Request%20%7B%5C%5Cn%7D%5C%5Cnpublic%20class%20$1$2RequestResponse%20:%20RequestResponse%20%7B%5C%5Cn%7D%5C%5Cn',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'(?:%5E%7C%5C%5Cn)%23%23%23%20(.*?)%5C%5Cn%5C%5Cn(.*?)%5C%5Cn-%20Complexity.*?%5C%5Cn(.*?)%5C%5Cn%5C%5Cn(.*?)(?:---%5C%5Cn%7C(?%3Dpublic)%7C$)'%7D,'///%20%3Csummary%3E%5C%5Cn///%20$2%5C%5Cn$3%5C%5Cn///%20%3C/summary%3E%5C%5Cnpublic%20class%20$1%20%7B%5C%5Cn$4%5C%5Cn%7D%5C%5Cn',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cn%5C%5C%7C%20Name.*?%5C%5Cn.*?%5C%5Cn'%7D,'',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Array%26lt;(.*?)%26gt;%20%5C%5C%7C'%7D,'%7C%20$1%5B%5D%20%7C',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20String%5C%5Cb'%7D,'%7C%20string',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Any%5C%5Cb'%7D,'%7C%20object?',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Boolean%5C%5Cb'%7D,'%7C%20bool',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Object(%5C%5C%5B%5C%5C%5D)?%20%5C%5C%7C'%7D,'%7C%20Dictionary%3Cstring,%20object?%3E$1%20%7C',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20(%5B%5E%7C%5D*?)%5C%5C%5B%5C%5C%5D%20%5C%5C%7C'%7D,'%7C%20List%3C$1%3E%20%7C',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Number%20%5C%5C%7C(?%3D.*?(?:index%7Coffset%7Cmilli%7Cframe%7Cnumerator%7Cdenominator%7Cpixel%7Cwidth%7Cheight%7Cquality%7C%5C%5Cbid%5C%5Cb%7Cnumber%20of%7Cversion%7Cduration))'%7D,'%7C%20int%20%7C',true,true,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7C%20Number%20%5C%5C%7C'%7D,'%7C%20double%20%7C',true,true,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cn%5C%5C*%5C%5C*Request%20Fields:%5C%5C*%5C%5C*'%7D,'',true,false,false,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cnpublic%20class%20(%5C%5Cw%2B)%20((?:(?!%5C%5Cn%5C%5C%7D).)*?%5C%5Cn)%5C%5C*%5C%5C*Response%20Fields:%5C%5C*%5C%5C*%5C%5Cn(.*?)%5C%5Cn%5C%5C%7D'%7D,'%5C%5Cnpublic%20class%20$1%20$2%7D%5C%5Cnpublic%20class%20$1Response%20%7B%5C%5Cn$3%7D%5C%5Cn',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'(?%3C%3Dpublic%20class%20(%5C%5Cw%2B)%20:%20Request%20%5C%5C%7B%5C%5Cn%5C%5C%7D.*?)(public%20class%20%5C%5Cw%2B?(Response)?)%20%5C%5C%7B'%7D,'$2%20:%20$1$3%20%7B',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cn%5C%5C%7C%20(%5C%5C?)?(%5B%5C%5Cw.%5D%2B)%20%5C%5C%7C%20(.*?)%20%5C%5C%7C%20(.*?)%20%5C%5C%7C(?%3D%5C%5Cn)'%7D,'%5C%5Cn///%20%3Csummary%3E%5C%5Cn///%20$4%5C%5Cn///%20%3C/summary%3E%5C%5Cn%5BKey(%22$2%22)%5D%5C%5Cnpublic%20$3$1%20$2%20%7B%20get;%20set;%20%7D',true,false,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5E(public%20(?:List%7CDictionary)%3C.*?%3E%20%5C%5Cw%2B%20.*?%5C%5C%7D)'%7D,'$1%20%3D%20new();',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5E(public%20string%20%5C%5Cw%2B%20.*?%5C%5C%7D)'%7D,'$1%20%3D%20%22%22;',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5E(?!///%20)(%5B%5E%5C%5Cn%5D%2B)(?%3D(?:(?!%3Csummary%3E).)*?%3C/summary%3E)'%7D,'///%20$1',true,false,true,true)Find_/_Replace(%7B'option':'Regex','string':'%5E(///%20(?!%5C%5C%3C).*?)%5C%5Cn'%7D,'$1%3Cbr%20/%3E%5C%5Cn',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7B%5C%5Cn%5C%5C%7D'%7D,'%7B%20%7D',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5C%5Cn%2B'%7D,'%5C%5Cn',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'%5E%5C%5C%7D%5C%5Cn'%7D,'%7D%5C%5Cn%5C%5Cn',true,false,true,false)Subsection('%5C%5Cw%2B%20%5C%5C%7B',true,true,false)To_Upper_case('Word')Regular_expression('User%20defined','',false,false,true,false,false,false,'Highlight%20matches')
  */

  public class GeneralRequest : Request { }
  public class GeneralRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets data about the current plugin and RPC version.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetVersion : GeneralRequest { }
  public class GetVersionResponse : GeneralRequestResponse {
    /// <summary>
    /// Current OBS Studio version<br />
    /// </summary>
    [Key("obsVersion")]
    public string ObsVersion { get; set; } = "";
    /// <summary>
    /// Current obs-websocket version<br />
    /// </summary>
    [Key("obsWebSocketVersion")]
    public string ObsWebSocketVersion { get; set; } = "";
    /// <summary>
    /// Current latest obs-websocket RPC version<br />
    /// </summary>
    [Key("rpcVersion")]
    public int RpcVersion { get; set; }
    /// <summary>
    /// Array of available RPC requests for the currently negotiated RPC version<br />
    /// </summary>
    [Key("availableRequests")]
    public List<string> AvailableRequests { get; set; } = new();
    /// <summary>
    /// Image formats available in `GetSourceScreenshot` and `SaveSourceScreenshot` requests.<br />
    /// </summary>
    [Key("supportedImageFormats")]
    public List<string> SupportedImageFormats { get; set; } = new();
    /// <summary>
    /// Name of the platform. Usually `windows`, `macos`, or `ubuntu` (linux flavor). Not guaranteed to be any of those<br />
    /// </summary>
    [Key("platform")]
    public string Platform { get; set; } = "";
    /// <summary>
    /// Description of the platform, like `Windows 10 (10.0)`<br />
    /// </summary>
    [Key("platformDescription")]
    public string PlatformDescription { get; set; } = "";
  }

  /// <summary>
  /// Gets statistics about OBS, obs-websocket, and the current session.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetStats : GeneralRequest { }
  public class GetStatsResponse : GeneralRequestResponse {
    /// <summary>
    /// Current CPU usage in percent<br />
    /// </summary>
    [Key("cpuUsage")]
    public double CpuUsage { get; set; }
    /// <summary>
    /// Amount of memory in MB currently being used by OBS<br />
    /// </summary>
    [Key("memoryUsage")]
    public double MemoryUsage { get; set; }
    /// <summary>
    /// Available disk space on the device being used for recording storage<br />
    /// </summary>
    [Key("availableDiskSpace")]
    public double AvailableDiskSpace { get; set; }
    /// <summary>
    /// Current FPS being rendered<br />
    /// </summary>
    [Key("activeFps")]
    public double ActiveFps { get; set; }
    /// <summary>
    /// Average time in milliseconds that OBS is taking to render a frame<br />
    /// </summary>
    [Key("averageFrameRenderTime")]
    public int AverageFrameRenderTime { get; set; }
    /// <summary>
    /// Number of frames skipped by OBS in the render thread<br />
    /// </summary>
    [Key("renderSkippedFrames")]
    public int RenderSkippedFrames { get; set; }
    /// <summary>
    /// Total number of frames outputted by the render thread<br />
    /// </summary>
    [Key("renderTotalFrames")]
    public int RenderTotalFrames { get; set; }
    /// <summary>
    /// Number of frames skipped by OBS in the output thread<br />
    /// </summary>
    [Key("outputSkippedFrames")]
    public int OutputSkippedFrames { get; set; }
    /// <summary>
    /// Total number of frames outputted by the output thread<br />
    /// </summary>
    [Key("outputTotalFrames")]
    public int OutputTotalFrames { get; set; }
    /// <summary>
    /// Total number of messages received by obs-websocket from the client<br />
    /// </summary>
    [Key("webSocketSessionIncomingMessages")]
    public int WebSocketSessionIncomingMessages { get; set; }
    /// <summary>
    /// Total number of messages sent by obs-websocket to the client<br />
    /// </summary>
    [Key("webSocketSessionOutgoingMessages")]
    public int WebSocketSessionOutgoingMessages { get; set; }
  }

  /// <summary>
  /// Broadcasts a `CustomEvent` to all WebSocket clients. Receivers are clients which are identified and subscribed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class BroadcastCustomEvent : GeneralRequest {
    /// <summary>
    /// Data payload to emit to all receivers | None | N/A<br />
    /// </summary>
    [Key("eventData")]
    public Dictionary<string, object?> EventData { get; set; } = new();
  }

  /// <summary>
  /// Call a request registered to a vendor.<br />
  /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
  /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class CallVendorRequest : GeneralRequest {
    /// <summary>
    /// Name of the vendor to use | None | N/A<br />
    /// </summary>
    [Key("vendorName")]
    public string VendorName { get; set; } = "";
    /// <summary>
    /// The request type to call | None | N/A<br />
    /// </summary>
    [Key("requestType")]
    public string VendorRequestType { get; set; } = "";
    /// <summary>
    /// Object containing appropriate request data | None | {}<br />
    /// </summary>
    [Key("requestData")]
    public Dictionary<string, object?>? RequestData { get; set; }
  }

  public class CallVendorRequestResponse : GeneralRequestResponse {
    /// <summary>
    /// Echoed of `vendorName`<br />
    /// </summary>
    [Key("vendorName")]
    public string VendorName { get; set; } = "";
    /// <summary>
    /// Echoed of `requestType`<br />
    /// </summary>
    [Key("requestType")]
    public string VendorRequestType { get; set; } = "";
    /// <summary>
    /// Object containing appropriate response data. {} if request does not provide any response data<br />
    /// </summary>
    [Key("responseData")]
    public Dictionary<string, object?> ResponseData { get; set; } = new();
  }

  /// <summary>
  /// Gets an array of all hotkey names in OBS<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetHotkeyList : GeneralRequest { }
  public class GetHotkeyListResponse : GeneralRequestResponse {
    /// <summary>
    /// Array of hotkey names<br />
    /// </summary>
    [Key("hotkeys")]
    public List<string> Hotkeys { get; set; } = new();
  }

  /// <summary>
  /// Triggers a hotkey using its name. See `GetHotkeyList`<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class TriggerHotkeyByName : GeneralRequest {
    /// <summary>
    /// Name of the hotkey to trigger | None | N/A<br />
    /// </summary>
    [Key("hotkeyName")]
    public string HotkeyName { get; set; } = "";
  }

  /// <summary>
  /// Triggers a hotkey using a sequence of keys.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class TriggerHotkeyByKeySequence : GeneralRequest {
    /// <summary>
    /// The OBS key ID to use. See https://github.com/obsproject/obs-studio/blob/master/libobs/obs-hotkeys.h | None | Not pressed<br />
    /// </summary>
    [Key("keyId")]
    public string? KeyId { get; set; }
    /// <summary>
    /// Object containing key modifiers to apply | None | Ignored<br />
    /// </summary>
    [Key("keyModifiers")]
    public KeyModifiers KeyModifiers { get; set; } = new();
  }

  /// <summary>
  /// Sleeps for a time duration or number of frames. Only available in request batches with types `SERIAL_REALTIME` or `SERIAL_FRAME`.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class Sleep : GeneralRequest {
    /// <summary>
    /// Number of milliseconds to sleep for (if `SERIAL_REALTIME` mode) | >= 0, <= 50000 | N/A<br />
    /// </summary>
    [Key("sleepMillis")]
    public int SleepMillis { get; set; }
    /// <summary>
    /// Number of frames to sleep for (if `SERIAL_FRAME` mode) | >= 0, <= 10000 | N/A<br />
    /// </summary>
    [Key("sleepFrames")]
    public int SleepFrames { get; set; }
  }

  public class ConfigRequest : Request { }
  public class ConfigRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets the current directory that the record output is set to.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetRecordDirectory : ConfigRequest {
  }
  public class GetRecordDirectoryResponse : ConfigRequestResponse {
    /// <summary>
    /// Output directory
    /// </summary>
    [Key("recordDirectory")]
    public string RecordDirectory { get; set; } = "";
  }

  /// <summary>
  /// Gets the value of a "slot" from the selected persistent data realm.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetPersistentData : ConfigRequest {
    /// <summary>
    /// The data realm to select. `OBS_WEBSOCKET_DATA_REALM_GLOBAL` or `OBS_WEBSOCKET_DATA_REALM_PROFILE` | None | N/A<br />
    /// </summary>
    [Key("realm")]
    public string Realm { get; set; } = "";
    /// <summary>
    /// The name of the slot to retrieve data from | None | N/A<br />
    /// </summary>
    [Key("slotName")]
    public string SlotName { get; set; } = "";
  }

  public class GetPersistentDataResponse : ConfigRequestResponse {
    /// <summary>
    /// Value associated with the slot. `null` if not set<br />
    /// </summary>
    [Key("slotValue")]
    public object? SlotValue { get; set; }
  }

  /// <summary>
  /// Sets the value of a "slot" from the selected persistent data realm.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetPersistentData : ConfigRequest {
    /// <summary>
    /// The data realm to select. `OBS_WEBSOCKET_DATA_REALM_GLOBAL` or `OBS_WEBSOCKET_DATA_REALM_PROFILE` | None | N/A<br />
    /// </summary>
    [Key("realm")]
    public string Realm { get; set; } = "";
    /// <summary>
    /// The name of the slot to retrieve data from | None | N/A<br />
    /// </summary>
    [Key("slotName")]
    public string SlotName { get; set; } = "";
    /// <summary>
    /// The value to apply to the slot | None | N/A<br />
    /// </summary>
    [Key("slotValue")]
    public object? SlotValue { get; set; }
  }

  /// <summary>
  /// Gets an array of all scene collections<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneCollectionList : ConfigRequest { }
  public class GetSceneCollectionListResponse : ConfigRequestResponse {
    /// <summary>
    /// The name of the current scene collection<br />
    /// </summary>
    [Key("currentSceneCollectionName")]
    public string CurrentSceneCollectionName { get; set; } = "";
    /// <summary>
    /// Array of all available scene collections<br />
    /// </summary>
    [Key("sceneCollections")]
    public List<string> SceneCollections { get; set; } = new();
  }

  /// <summary>
  /// Switches to a scene collection.<br />
  /// Note: This will block until the collection has finished changing.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetCurrentSceneCollection : ConfigRequest {
    /// <summary>
    /// Name of the scene collection to switch to | None | N/A<br />
    /// </summary>
    [Key("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }

  /// <summary>
  /// Creates a new scene collection, switching to it in the process.<br />
  /// Note: This will block until the collection has finished changing.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class CreateSceneCollection : ConfigRequest {
    /// <summary>
    /// Name for the new scene collection | None | N/A<br />
    /// </summary>
    [Key("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }

  /// <summary>
  /// Gets an array of all profiles<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetProfileList : ConfigRequest { }
  public class GetProfileListResponse : ConfigRequestResponse {
    /// <summary>
    /// The name of the current profile<br />
    /// </summary>
    [Key("currentProfileName")]
    public string CurrentProfileName { get; set; } = "";
    /// <summary>
    /// Array of all available profiles<br />
    /// </summary>
    [Key("profiles")]
    public List<string> Profiles { get; set; } = new();
  }

  /// <summary>
  /// Switches to a profile.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetCurrentProfile : ConfigRequest {
    /// <summary>
    /// Name of the profile to switch to | None | N/A<br />
    /// </summary>
    [Key("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Creates a new profile, switching to it in the process<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class CreateProfile : ConfigRequest {
    /// <summary>
    /// Name for the new profile | None | N/A<br />
    /// </summary>
    [Key("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Removes a profile. If the current profile is chosen, it will change to a different profile first.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class RemoveProfile : ConfigRequest {
    /// <summary>
    /// Name of the profile to remove | None | N/A<br />
    /// </summary>
    [Key("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Gets a parameter from the current profile's configuration.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetProfileParameter : ConfigRequest {
    /// <summary>
    /// Category of the parameter to get | None | N/A<br />
    /// </summary>
    [Key("parameterCategory")]
    public string ParameterCategory { get; set; } = "";
    /// <summary>
    /// Name of the parameter to get | None | N/A<br />
    /// </summary>
    [Key("parameterName")]
    public string ParameterName { get; set; } = "";
  }

  public class GetProfileParameterResponse : ConfigRequestResponse {
    /// <summary>
    /// Value associated with the parameter. `null` if not set and no default<br />
    /// </summary>
    [Key("parameterValue")]
    public string ParameterValue { get; set; } = "";
    /// <summary>
    /// Default value associated with the parameter. `null` if no default<br />
    /// </summary>
    [Key("defaultParameterValue")]
    public string DefaultParameterValue { get; set; } = "";
  }

  /// <summary>
  /// Sets the value of a parameter in the current profile's configuration.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetProfileParameter : ConfigRequest {
    /// <summary>
    /// Category of the parameter to set | None | N/A<br />
    /// </summary>
    [Key("parameterCategory")]
    public string ParameterCategory { get; set; } = "";
    /// <summary>
    /// Name of the parameter to set | None | N/A<br />
    /// </summary>
    [Key("parameterName")]
    public string ParameterName { get; set; } = "";
    /// <summary>
    /// Value of the parameter to set. Use `null` to delete | None | N/A<br />
    /// </summary>
    [Key("parameterValue")]
    public string ParameterValue { get; set; } = "";
  }

  /// <summary>
  /// Gets the current video settings.<br />
  /// Note: To get the true FPS value, divide the FPS numerator by the FPS denominator. Example: `60000/1001`<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetVideoSettings : ConfigRequest { }
  public class GetVideoSettingsResponse : ConfigRequestResponse {
    /// <summary>
    /// Numerator of the fractional FPS value<br />
    /// </summary>
    [Key("fpsNumerator")]
    public int FpsNumerator { get; set; }
    /// <summary>
    /// Denominator of the fractional FPS value<br />
    /// </summary>
    [Key("fpsDenominator")]
    public int FpsDenominator { get; set; }
    /// <summary>
    /// Width of the base (canvas) resolution in pixels<br />
    /// </summary>
    [Key("baseWidth")]
    public int BaseWidth { get; set; }
    /// <summary>
    /// Height of the base (canvas) resolution in pixels<br />
    /// </summary>
    [Key("baseHeight")]
    public int BaseHeight { get; set; }
    /// <summary>
    /// Width of the output resolution in pixels<br />
    /// </summary>
    [Key("outputWidth")]
    public int OutputWidth { get; set; }
    /// <summary>
    /// Height of the output resolution in pixels<br />
    /// </summary>
    [Key("outputHeight")]
    public int OutputHeight { get; set; }
  }

  /// <summary>
  /// Sets the current video settings.<br />
  /// Note: Fields must be specified in pairs. For example, you cannot set only `baseWidth` without needing to specify `baseHeight`.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetVideoSettings : ConfigRequest {
    /// <summary>
    /// Numerator of the fractional FPS value | >= 1 | Not changed<br />
    /// </summary>
    [Key("fpsNumerator")]
    public int? FpsNumerator { get; set; }
    /// <summary>
    /// Denominator of the fractional FPS value | >= 1 | Not changed<br />
    /// </summary>
    [Key("fpsDenominator")]
    public int? FpsDenominator { get; set; }
    /// <summary>
    /// Width of the base (canvas) resolution in pixels | >= 1, <= 4096 | Not changed<br />
    /// </summary>
    [Key("baseWidth")]
    public int? BaseWidth { get; set; }
    /// <summary>
    /// Height of the base (canvas) resolution in pixels | >= 1, <= 4096 | Not changed<br />
    /// </summary>
    [Key("baseHeight")]
    public int? BaseHeight { get; set; }
    /// <summary>
    /// Width of the output resolution in pixels | >= 1, <= 4096 | Not changed<br />
    /// </summary>
    [Key("outputWidth")]
    public int? OutputWidth { get; set; }
    /// <summary>
    /// Height of the output resolution in pixels | >= 1, <= 4096 | Not changed<br />
    /// </summary>
    [Key("outputHeight")]
    public int? OutputHeight { get; set; }
  }

  /// <summary>
  /// Gets the current stream service settings (stream destination).<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetStreamServiceSettings : ConfigRequest { }
  public class GetStreamServiceSettingsResponse : ConfigRequestResponse {
    /// <summary>
    /// Stream service type, like `rtmp_custom` or `rtmp_common`<br />
    /// </summary>
    [Key("streamServiceType")]
    public string StreamServiceType { get; set; } = "";
    /// <summary>
    /// Stream service settings<br />
    /// </summary>
    [Key("streamServiceSettings")]
    public Dictionary<string, object?> StreamServiceSettings { get; set; } = new();
  }

  /// <summary>
  /// Sets the current stream service settings (stream destination).<br />
  /// Note: Simple RTMP settings can be set with type `rtmp_custom` and the settings fields `server` and `key`.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetStreamServiceSettings : ConfigRequest {
    /// <summary>
    /// Type of stream service to apply. Example: `rtmp_common` or `rtmp_custom` | None | N/A<br />
    /// </summary>
    [Key("streamServiceType")]
    public string StreamServiceType { get; set; } = "";
    /// <summary>
    /// Settings to apply to the service | None | N/A<br />
    /// </summary>
    [Key("streamServiceSettings")]
    public Dictionary<string, object?> StreamServiceSettings { get; set; } = new();
  }

  public class SourcesRequest : Request { }
  public class SourcesRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets the active and show state of a source.<br />
  /// **Compatible with inputs and scenes.**<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSourceActive : SourcesRequest {
    /// <summary>
    /// Name of the source to get the active state of | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
  }

  public class GetSourceActiveResponse : SourcesRequestResponse {
    /// <summary>
    /// Whether the source is showing in Program<br />
    /// </summary>
    [Key("videoActive")]
    public bool VideoActive { get; set; }
    /// <summary>
    /// Whether the source is showing in the UI (Preview, Projector, Properties)<br />
    /// </summary>
    [Key("videoShowing")]
    public bool VideoShowing { get; set; }
  }

  /// <summary>
  /// Gets a Base64-encoded screenshot of a source.<br />
  /// The `imageWidth` and `imageHeight` parameters are treated as "scale to inner", meaning the smallest ratio will be used and the aspect ratio of the original resolution is kept.<br />
  /// If `imageWidth` and `imageHeight` are not specified, the compressed image will use the full resolution of the source.<br />
  /// **Compatible with inputs and scenes.**<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSourceScreenshot : SourcesRequest {
    /// <summary>
    /// Name of the source to take a screenshot of | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Image compression format to use. Use `GetVersion` to get compatible image formats | None | N/A<br />
    /// </summary>
    [Key("imageFormat")]
    public string ImageFormat { get; set; } = "";
    /// <summary>
    /// Width to scale the screenshot to | >= 8, <= 4096 | Source value is used<br />
    /// </summary>
    [Key("imageWidth")]
    public int? ImageWidth { get; set; }
    /// <summary>
    /// Height to scale the screenshot to | >= 8, <= 4096 | Source value is used<br />
    /// </summary>
    [Key("imageHeight")]
    public int? ImageHeight { get; set; }
    /// <summary>
    /// Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk) | >= -1, <= 100 | -1<br />
    /// </summary>
    [Key("imageCompressionQuality")]
    public int? ImageCompressionQuality { get; set; }
  }

  public class GetSourceScreenshotResponse : SourcesRequestResponse {
    /// <summary>
    /// Base64-encoded screenshot<br />
    /// </summary>
    [Key("imageData")]
    public string ImageData { get; set; } = "";
  }

  /// <summary>
  /// Saves a screenshot of a source to the filesystem.<br />
  /// The `imageWidth` and `imageHeight` parameters are treated as "scale to inner", meaning the smallest ratio will be used and the aspect ratio of the original resolution is kept.<br />
  /// If `imageWidth` and `imageHeight` are not specified, the compressed image will use the full resolution of the source.<br />
  /// **Compatible with inputs and scenes.**<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SaveSourceScreenshot : SourcesRequest {
    /// <summary>
    /// Name of the source to take a screenshot of | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Image compression format to use. Use `GetVersion` to get compatible image formats | None | N/A<br />
    /// </summary>
    [Key("imageFormat")]
    public string ImageFormat { get; set; } = "";
    /// <summary>
    /// Path to save the screenshot file to. Eg. `C:\Users\user\Desktop\screenshot.png` | None | N/A<br />
    /// </summary>
    [Key("imageFilePath")]
    public string ImageFilePath { get; set; } = "";
    /// <summary>
    /// Width to scale the screenshot to | >= 8, <= 4096 | Source value is used<br />
    /// </summary>
    [Key("imageWidth")]
    public int? ImageWidth { get; set; }
    /// <summary>
    /// Height to scale the screenshot to | >= 8, <= 4096 | Source value is used<br />
    /// </summary>
    [Key("imageHeight")]
    public int? ImageHeight { get; set; }
    /// <summary>
    /// Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk) | >= -1, <= 100 | -1<br />
    /// </summary>
    [Key("imageCompressionQuality")]
    public int? ImageCompressionQuality { get; set; }
  }

  public class SaveSourceScreenshotResponse : SourcesRequestResponse {
    /// <summary>
    /// Base64-encoded screenshot<br />
    /// </summary>
    [Key("imageData")]
    public string ImageData { get; set; } = "";
  }

  public class ScenesRequest : Request { }
  public class ScenesRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets an array of all scenes in OBS.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneList : ScenesRequest { }
  public class GetSceneListResponse : ScenesRequestResponse {
    /// <summary>
    /// Current program scene<br />
    /// </summary>
    [Key("currentProgramSceneName")]
    public string CurrentProgramSceneName { get; set; } = "";
    /// <summary>
    /// Current preview scene. `null` if not in studio mode<br />
    /// </summary>
    [Key("currentPreviewSceneName")]
    public string CurrentPreviewSceneName { get; set; } = "";
    /// <summary>
    /// Array of scenes<br />
    /// </summary>
    [Key("scenes")]
    public List<Scene> Scenes { get; set; } = new();
  }

  /// <summary>
  /// Gets an array of all groups in OBS.<br />
  /// Groups in OBS are actually scenes, but renamed and modified. In obs-websocket, we treat them as scenes where we can.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetGroupList : ScenesRequest { }
  public class GetGroupListResponse : ScenesRequestResponse {
    /// <summary>
    /// Array of group names<br />
    /// </summary>
    [Key("groups")]
    public List<string> Groups { get; set; } = new();
  }

  /// <summary>
  /// Gets the current program scene.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetCurrentProgramScene : ScenesRequest { }
  public class GetCurrentProgramSceneResponse : ScenesRequestResponse {
    /// <summary>
    /// Current program scene<br />
    /// </summary>
    [Key("currentProgramSceneName")]
    public string CurrentProgramSceneName { get; set; } = "";
  }

  /// <summary>
  /// Sets the current program scene.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetCurrentProgramScene : ScenesRequest {
    /// <summary>
    /// Scene to set as the current program scene | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Gets the current preview scene.<br />
  /// Only available when studio mode is enabled.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetCurrentPreviewScene : ScenesRequest { }
  public class GetCurrentPreviewSceneResponse : ScenesRequestResponse {
    /// <summary>
    /// Current preview scene<br />
    /// </summary>
    [Key("currentPreviewSceneName")]
    public string CurrentPreviewSceneName { get; set; } = "";
  }

  /// <summary>
  /// Sets the current preview scene.<br />
  /// Only available when studio mode is enabled.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetCurrentPreviewScene : ScenesRequest {
    /// <summary>
    /// Scene to set as the current preview scene | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Creates a new scene in OBS.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class CreateScene : ScenesRequest {
    /// <summary>
    /// Name for the new scene | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Removes a scene from OBS.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class RemoveScene : ScenesRequest {
    /// <summary>
    /// Name of the scene to remove | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Sets the name of a scene (rename).<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSceneName : ScenesRequest {
    /// <summary>
    /// Name of the scene to be renamed | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// New name for the scene | None | N/A<br />
    /// </summary>
    [Key("newSceneName")]
    public string NewSceneName { get; set; } = "";
  }

  /// <summary>
  /// Gets the scene transition overridden for a scene.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneSceneTransitionOverride : ScenesRequest {
    /// <summary>
    /// Name of the scene | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  public class GetSceneSceneTransitionOverrideResponse : ScenesRequestResponse {
    /// <summary>
    /// Name of the overridden scene transition, else `null`<br />
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
    /// <summary>
    /// Duration of the overridden scene transition, else `null`<br />
    /// </summary>
    [Key("transitionDuration")]
    public int TransitionDuration { get; set; }
  }

  /// <summary>
  /// Gets the scene transition overridden for a scene.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSceneSceneTransitionOverride : ScenesRequest {
    /// <summary>
    /// Name of the scene | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Name of the scene transition to use as override. Specify `null` to remove | None | Unchanged<br />
    /// </summary>
    [Key("transitionName")]
    public string? TransitionName { get; set; }
    /// <summary>
    /// Duration to use for any overridden transition. Specify `null` to remove | >= 50, <= 20000 | Unchanged<br />
    /// </summary>
    [Key("transitionDuration")]
    public int? TransitionDuration { get; set; }
  }

  public class InputsRequest : Request { }
  public class InputsRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets an array of all inputs in OBS.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputList : InputsRequest {
    /// <summary>
    /// Restrict the array to only inputs of the specified kind | None | All kinds included<br />
    /// </summary>
    [Key("inputKind")]
    public string? InputKind { get; set; }
  }

  public class GetInputListResponse : InputsRequestResponse {
    /// <summary>
    /// Array of inputs<br />
    /// </summary>
    [Key("inputs")]
    public List<Input> Inputs { get; set; } = new();
  }

  /// <summary>
  /// Gets an array of all available input kinds in OBS.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputKindList : InputsRequest {
    /// <summary>
    /// True == Return all kinds as unversioned, False == Return with version suffixes (if available) | None | false<br />
    /// </summary>
    [Key("unversioned")]
    public bool? Unversioned { get; set; }
  }

  public class GetInputKindListResponse : InputsRequestResponse {
    /// <summary>
    /// Array of input kinds<br />
    /// </summary>
    [Key("inputKinds")]
    public List<string> InputKinds { get; set; } = new();
  }

  /// <summary>
  /// Gets the names of all special inputs.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSpecialInputs : InputsRequest { }
  public class GetSpecialInputsResponse : InputsRequestResponse {
    /// <summary>
    /// Name of the Desktop Audio input<br />
    /// </summary>
    [Key("desktop1")]
    public string Desktop1 { get; set; } = "";
    /// <summary>
    /// Name of the Desktop Audio 2 input<br />
    /// </summary>
    [Key("desktop2")]
    public string Desktop2 { get; set; } = "";
    /// <summary>
    /// Name of the Mic/Auxiliary Audio input<br />
    /// </summary>
    [Key("mic1")]
    public string Mic1 { get; set; } = "";
    /// <summary>
    /// Name of the Mic/Auxiliary Audio 2 input<br />
    /// </summary>
    [Key("mic2")]
    public string Mic2 { get; set; } = "";
    /// <summary>
    /// Name of the Mic/Auxiliary Audio 3 input<br />
    /// </summary>
    [Key("mic3")]
    public string Mic3 { get; set; } = "";
    /// <summary>
    /// Name of the Mic/Auxiliary Audio 4 input<br />
    /// </summary>
    [Key("mic4")]
    public string Mic4 { get; set; } = "";
  }

  /// <summary>
  /// Creates a new input, adding it as a scene item to the specified scene.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class CreateInput : InputsRequest {
    /// <summary>
    /// Name of the scene to add the input to as a scene item | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Name of the new input to created | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// The kind of input to be created | None | N/A<br />
    /// </summary>
    [Key("inputKind")]
    public string InputKind { get; set; } = "";
    /// <summary>
    /// Settings object to initialize the input with | None | Default settings used<br />
    /// </summary>
    [Key("inputSettings")]
    public Dictionary<string, object?>? InputSettings { get; set; }
    /// <summary>
    /// Whether to set the created scene item to enabled or disabled | None | True<br />
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool? SceneItemEnabled { get; set; }
  }

  public class CreateInputResponse : InputsRequestResponse {
    /// <summary>
    /// ID of the newly created scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Removes an existing input.<br />
  /// Note: Will immediately remove all associated scene items.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class RemoveInput : InputsRequest {
    /// <summary>
    /// Name of the input to remove | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Sets the name of an input (rename).<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetInputName : InputsRequest {
    /// <summary>
    /// Current input name | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// New name for the input | None | N/A<br />
    /// </summary>
    [Key("newInputName")]
    public string NewInputName { get; set; } = "";
  }

  /// <summary>
  /// Gets the default settings for an input kind.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputDefaultSettings : InputsRequest {
    /// <summary>
    /// Input kind to get the default settings for | None | N/A<br />
    /// </summary>
    [Key("inputKind")]
    public string InputKind { get; set; } = "";
  }

  public class GetInputDefaultSettingsResponse : InputsRequestResponse {
    /// <summary>
    /// Object of default settings for the input kind<br />
    /// </summary>
    [Key("defaultInputSettings")]
    public Dictionary<string, object?> DefaultInputSettings { get; set; } = new();
  }

  /// <summary>
  /// Gets the settings of an input.<br />
  /// Note: Does not include defaults. To create the entire settings object, overlay `inputSettings` over the `defaultInputSettings` provided by `GetInputDefaultSettings`.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputSettings : InputsRequest {
    /// <summary>
    /// Name of the input to get the settings of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class GetInputSettingsResponse : InputsRequestResponse {
    /// <summary>
    /// Object of settings for the input<br />
    /// </summary>
    [Key("inputSettings")]
    public Dictionary<string, object?> InputSettings { get; set; } = new();
    /// <summary>
    /// The kind of the input<br />
    /// </summary>
    [Key("inputKind")]
    public string InputKind { get; set; } = "";
  }

  /// <summary>
  /// Sets the settings of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetInputSettings : InputsRequest {
    /// <summary>
    /// Name of the input to set the settings of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Object of settings to apply | None | N/A<br />
    /// </summary>
    [Key("inputSettings")]
    public Dictionary<string, object?> InputSettings { get; set; } = new();
    /// <summary>
    /// True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings. | None | true<br />
    /// </summary>
    [Key("overlay")]
    public bool? Overlay { get; set; }
  }

  /// <summary>
  /// Gets the audio mute state of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputMute : InputsRequest {
    /// <summary>
    /// Name of input to get the mute state of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class GetInputMuteResponse : InputsRequestResponse {
    /// <summary>
    /// Whether the input is muted<br />
    /// </summary>
    [Key("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// Sets the audio mute state of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetInputMute : InputsRequest {
    /// <summary>
    /// Name of the input to set the mute state of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Whether to mute the input or not | None | N/A<br />
    /// </summary>
    [Key("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// Toggles the audio mute state of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class ToggleInputMute : InputsRequest {
    /// <summary>
    /// Name of the input to toggle the mute state of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class ToggleInputMuteResponse : InputsRequestResponse {
    /// <summary>
    /// Whether the input has been muted or unmuted<br />
    /// </summary>
    [Key("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// Gets the current volume setting of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputVolume : InputsRequest {
    /// <summary>
    /// Name of the input to get the volume of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class GetInputVolumeResponse : InputsRequestResponse {
    /// <summary>
    /// Volume setting in mul<br />
    /// </summary>
    [Key("inputVolumeMul")]
    public double InputVolumeMul { get; set; }
    /// <summary>
    /// Volume setting in dB<br />
    /// </summary>
    [Key("inputVolumeDb")]
    public double InputVolumeDb { get; set; }
  }

  /// <summary>
  /// Sets the volume setting of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetInputVolume : InputsRequest {
    /// <summary>
    /// Name of the input to set the volume of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Volume setting in mul | >= 0, <= 20 | `inputVolumeDb` should be specified<br />
    /// </summary>
    [Key("inputVolumeMul")]
    public double? InputVolumeMul { get; set; }
    /// <summary>
    /// Volume setting in dB | >= -100, <= 26 | `inputVolumeMul` should be specified<br />
    /// </summary>
    [Key("inputVolumeDb")]
    public double? InputVolumeDb { get; set; }
  }

  /// <summary>
  /// Gets the audio balance of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputAudioBalance : InputsRequest {
    /// <summary>
    /// Name of the input to get the audio balance of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class GetInputAudioBalanceResponse : InputsRequestResponse {
    /// <summary>
    /// Audio balance value from 0.0-1.0<br />
    /// </summary>
    [Key("inputAudioBalance")]
    public double InputAudioBalance { get; set; }
  }

  /// <summary>
  /// Sets the audio balance of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetInputAudioBalance : InputsRequest {
    /// <summary>
    /// Name of the input to set the audio balance of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// New audio balance value | >= 0.0, <= 1.0 | N/A<br />
    /// </summary>
    [Key("inputAudioBalance")]
    public double InputAudioBalance { get; set; }
  }

  /// <summary>
  /// Gets the audio sync offset of an input.<br />
  /// Note: The audio sync offset can be negative too!<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputAudioSyncOffset : InputsRequest {
    /// <summary>
    /// Name of the input to get the audio sync offset of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class GetInputAudioSyncOffsetResponse : InputsRequestResponse {
    /// <summary>
    /// Audio sync offset in milliseconds<br />
    /// </summary>
    [Key("inputAudioSyncOffset")]
    public int InputAudioSyncOffset { get; set; }
  }

  /// <summary>
  /// Sets the audio sync offset of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetInputAudioSyncOffset : InputsRequest {
    /// <summary>
    /// Name of the input to set the audio sync offset of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// New audio sync offset in milliseconds | >= -950, <= 20000 | N/A<br />
    /// </summary>
    [Key("inputAudioSyncOffset")]
    public int InputAudioSyncOffset { get; set; }
  }

  /// <summary>
  /// Gets the audio monitor type of an input.<br />
  /// The available audio monitor types are:<br />
  /// - `OBS_MONITORING_TYPE_NONE`<br />
  /// - `OBS_MONITORING_TYPE_MONITOR_ONLY`<br />
  /// - `OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT`<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputAudioMonitorType : InputsRequest {
    /// <summary>
    /// Name of the input to get the audio monitor type of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class GetInputAudioMonitorTypeResponse : InputsRequestResponse {
    /// <summary>
    /// Audio monitor type<br />
    /// </summary>
    [Key("monitorType")]
    public MonitoringType MonitorType { get; set; }
  }

  /// <summary>
  /// Sets the audio monitor type of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetInputAudioMonitorType : InputsRequest {
    /// <summary>
    /// Name of the input to set the audio monitor type of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Audio monitor type | None | N/A<br />
    /// </summary>
    [Key("monitorType")]
    public MonitoringType MonitorType { get; set; }
  }

  /// <summary>
  /// Gets the enable state of all audio tracks of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputAudioTracks : InputsRequest {
    /// <summary>
    /// Name of the input | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class GetInputAudioTracksResponse : InputsRequestResponse {
    /// <summary>
    /// Object of audio tracks and associated enable states<br />
    /// </summary>
    [Key("inputAudioTracks")]
    public Dictionary<string, object?> InputAudioTracks { get; set; } = new();
  }

  /// <summary>
  /// Sets the enable state of audio tracks of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetInputAudioTracks : InputsRequest {
    /// <summary>
    /// Name of the input | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Track settings to apply | None | N/A<br />
    /// </summary>
    [Key("inputAudioTracks")]
    public Dictionary<string, object?> InputAudioTracks { get; set; } = new();
  }

  /// <summary>
  /// Gets the items of a list property from an input's properties.<br />
  /// Note: Use this in cases where an input provides a dynamic, selectable list of items. For example, display capture, where it provides a list of available displays.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetInputPropertiesListPropertyItems : InputsRequest {
    /// <summary>
    /// Name of the input | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Name of the list property to get the items of | None | N/A<br />
    /// </summary>
    [Key("propertyName")]
    public string PropertyName { get; set; } = "";
  }

  public class GetInputPropertiesListPropertyItemsResponse : InputsRequestResponse {
    /// <summary>
    /// Array of items in the list property<br />
    /// </summary>
    [Key("propertyItems")]
    public List<Dictionary<string, object?>> PropertyItems { get; set; } = new();
  }

  /// <summary>
  /// Presses a button in the properties of an input.<br />
  /// Note: Use this in cases where there is a button in the properties of an input that cannot be accessed in any other way. For example, browser sources, where there is a refresh button.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class PressInputPropertiesButton : InputsRequest {
    /// <summary>
    /// Name of the input | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Name of the button property to press | None | N/A<br />
    /// </summary>
    [Key("propertyName")]
    public string PropertyName { get; set; } = "";
  }

  public class TransitionsRequest : Request { }
  public class TransitionsRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets an array of all available transition kinds.<br />
  /// Similar to `GetInputKindList`<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetTransitionKindList : TransitionsRequest { }
  public class GetTransitionKindListResponse : TransitionsRequestResponse {
    /// <summary>
    /// Array of transition kinds<br />
    /// </summary>
    [Key("transitionKinds")]
    public List<string> TransitionKinds { get; set; } = new();
  }

  /// <summary>
  /// Gets an array of all scene transitions in OBS.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneTransitionList : TransitionsRequest { }
  public class GetSceneTransitionListResponse : TransitionsRequestResponse {
    /// <summary>
    /// Name of the current scene transition. Can be null<br />
    /// </summary>
    [Key("currentSceneTransitionName")]
    public string CurrentSceneTransitionName { get; set; } = "";
    /// <summary>
    /// Kind of the current scene transition. Can be null<br />
    /// </summary>
    [Key("currentSceneTransitionKind")]
    public string CurrentSceneTransitionKind { get; set; } = "";
    /// <summary>
    /// Array of transitions<br />
    /// </summary>
    [Key("transitions")]
    public List<Dictionary<string, object?>> Transitions { get; set; } = new();
  }

  /// <summary>
  /// Gets information about the current scene transition.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetCurrentSceneTransition : TransitionsRequest { }
  public class GetCurrentSceneTransitionResponse : TransitionsRequestResponse {
    /// <summary>
    /// Name of the transition<br />
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
    /// <summary>
    /// Kind of the transition<br />
    /// </summary>
    [Key("transitionKind")]
    public string TransitionKind { get; set; } = "";
    /// <summary>
    /// Whether the transition uses a fixed (unconfigurable) duration<br />
    /// </summary>
    [Key("transitionFixed")]
    public bool TransitionFixed { get; set; }
    /// <summary>
    /// Configured transition duration in milliseconds. `null` if transition is fixed<br />
    /// </summary>
    [Key("transitionDuration")]
    public int TransitionDuration { get; set; }
    /// <summary>
    /// Whether the transition supports being configured<br />
    /// </summary>
    [Key("transitionConfigurable")]
    public bool TransitionConfigurable { get; set; }
    /// <summary>
    /// Object of settings for the transition. `null` if transition is not configurable<br />
    /// </summary>
    [Key("transitionSettings")]
    public Dictionary<string, object?> TransitionSettings { get; set; } = new();
  }

  /// <summary>
  /// Sets the current scene transition.<br />
  /// Small note: While the namespace of scene transitions is generally unique, that uniqueness is not a guarantee as it is with other resources like inputs.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetCurrentSceneTransition : TransitionsRequest {
    /// <summary>
    /// Name of the transition to make active | None | N/A<br />
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// Sets the duration of the current scene transition, if it is not fixed.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetCurrentSceneTransitionDuration : TransitionsRequest {
    /// <summary>
    /// Duration in milliseconds | >= 50, <= 20000 | N/A<br />
    /// </summary>
    [Key("transitionDuration")]
    public int TransitionDuration { get; set; }
  }

  /// <summary>
  /// Sets the settings of the current scene transition.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetCurrentSceneTransitionSettings : TransitionsRequest {
    /// <summary>
    /// Settings object to apply to the transition. Can be `{}` | None | N/A<br />
    /// </summary>
    [Key("transitionSettings")]
    public Dictionary<string, object?> TransitionSettings { get; set; } = new();
    /// <summary>
    /// Whether to overlay over the current settings or replace them | None | true<br />
    /// </summary>
    [Key("overlay")]
    public bool? Overlay { get; set; }
  }

  /// <summary>
  /// Gets the cursor position of the current scene transition.<br />
  /// Note: `transitionCursor` will return 1.0 when the transition is inactive.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetCurrentSceneTransitionCursor : TransitionsRequest { }
  public class GetCurrentSceneTransitionCursorResponse : TransitionsRequestResponse {
    /// <summary>
    /// Cursor position, between 0.0 and 1.0<br />
    /// </summary>
    [Key("transitionCursor")]
    public double TransitionCursor { get; set; }
  }

  /// <summary>
  /// Triggers the current scene transition. Same functionality as the `Transition` button in studio mode.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class TriggerStudioModeTransition : TransitionsRequest {
  }

  /// <summary>
  /// Sets the position of the TBar.<br />
  /// **Very important note**: This will be deprecated and replaced in a future version of obs-websocket.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetTBarPosition : TransitionsRequest {
    /// <summary>
    /// New position | >= 0.0, <= 1.0 | N/A<br />
    /// </summary>
    [Key("position")]
    public double Position { get; set; }
    /// <summary>
    /// Whether to release the TBar. Only set `false` if you know that you will be sending another position update | None | `true`<br />
    /// </summary>
    [Key("release")]
    public bool? Release { get; set; }
  }

  public class FiltersRequest : Request { }
  public class FiltersRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets an array of all of a source's filters.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSourceFilterList : FiltersRequest {
    /// <summary>
    /// Name of the source | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
  }

  public class GetSourceFilterListResponse : FiltersRequestResponse {
    /// <summary>
    /// Array of filters<br />
    /// </summary>
    [Key("filters")]
    public List<Dictionary<string, object?>> Filters { get; set; } = new();
  }

  /// <summary>
  /// Gets the default settings for a filter kind.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSourceFilterDefaultSettings : FiltersRequest {
    /// <summary>
    /// Filter kind to get the default settings for | None | N/A<br />
    /// </summary>
    [Key("filterKind")]
    public string FilterKind { get; set; } = "";
  }

  public class GetSourceFilterDefaultSettingsResponse : FiltersRequestResponse {
    /// <summary>
    /// Object of default settings for the filter kind<br />
    /// </summary>
    [Key("defaultFilterSettings")]
    public Dictionary<string, object?> DefaultFilterSettings { get; set; } = new();
  }

  /// <summary>
  /// Creates a new filter, adding it to the specified source.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class CreateSourceFilter : FiltersRequest {
    /// <summary>
    /// Name of the source to add the filter to | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the new filter to be created | None | N/A<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
    /// <summary>
    /// The kind of filter to be created | None | N/A<br />
    /// </summary>
    [Key("filterKind")]
    public string FilterKind { get; set; } = "";
    /// <summary>
    /// Settings object to initialize the filter with | None | Default settings used<br />
    /// </summary>
    [Key("filterSettings")]
    public Dictionary<string, object?>? FilterSettings { get; set; }
  }

  /// <summary>
  /// Removes a filter from a source.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class RemoveSourceFilter : FiltersRequest {
    /// <summary>
    /// Name of the source the filter is on | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the filter to remove | None | N/A<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
  }

  /// <summary>
  /// Sets the name of a source filter (rename).<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSourceFilterName : FiltersRequest {
    /// <summary>
    /// Name of the source the filter is on | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Current name of the filter | None | N/A<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
    /// <summary>
    /// New name for the filter | None | N/A<br />
    /// </summary>
    [Key("newFilterName")]
    public string NewFilterName { get; set; } = "";
  }

  /// <summary>
  /// Gets the info for a specific source filter.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSourceFilter : FiltersRequest {
    /// <summary>
    /// Name of the source | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the filter | None | N/A<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
  }

  public class GetSourceFilterResponse : FiltersRequestResponse {
    /// <summary>
    /// Whether the filter is enabled<br />
    /// </summary>
    [Key("filterEnabled")]
    public bool FilterEnabled { get; set; }
    /// <summary>
    /// Index of the filter in the list, beginning at 0<br />
    /// </summary>
    [Key("filterIndex")]
    public int FilterIndex { get; set; }
    /// <summary>
    /// The kind of filter<br />
    /// </summary>
    [Key("filterKind")]
    public string FilterKind { get; set; } = "";
    /// <summary>
    /// Settings object associated with the filter<br />
    /// </summary>
    [Key("filterSettings")]
    public Dictionary<string, object?> FilterSettings { get; set; } = new();
  }

  /// <summary>
  /// Sets the index position of a filter on a source.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSourceFilterIndex : FiltersRequest {
    /// <summary>
    /// Name of the source the filter is on | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the filter | None | N/A<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
    /// <summary>
    /// New index position of the filter | >= 0 | N/A<br />
    /// </summary>
    [Key("filterIndex")]
    public int FilterIndex { get; set; }
  }

  /// <summary>
  /// Sets the settings of a source filter.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSourceFilterSettings : FiltersRequest {
    /// <summary>
    /// Name of the source the filter is on | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the filter to set the settings of | None | N/A<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
    /// <summary>
    /// Object of settings to apply | None | N/A<br />
    /// </summary>
    [Key("filterSettings")]
    public Dictionary<string, object?> FilterSettings { get; set; } = new();
    /// <summary>
    /// True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings. | None | true<br />
    /// </summary>
    [Key("overlay")]
    public bool? Overlay { get; set; }
  }

  /// <summary>
  /// Sets the enable state of a source filter.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSourceFilterEnabled : FiltersRequest {
    /// <summary>
    /// Name of the source the filter is on | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Name of the filter | None | N/A<br />
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
    /// <summary>
    /// New enable state of the filter | None | N/A<br />
    /// </summary>
    [Key("filterEnabled")]
    public bool FilterEnabled { get; set; }
  }

  public class SceneItemsRequest : Request { }
  public class SceneItemsRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets a list of all scene items in a scene.<br />
  /// Scenes only<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneItemList : SceneItemsRequest {
    /// <summary>
    /// Name of the scene to get the items of | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  public class GetSceneItemListResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Array of scene items in the scene<br />
    /// </summary>
    [Key("sceneItems")]
    public List<Dictionary<string, object?>> SceneItems { get; set; } = new();
  }

  /// <summary>
  /// Basically GetSceneItemList, but for groups.<br />
  /// Using groups at all in OBS is discouraged, as they are very broken under the hood.<br />
  /// Groups only<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetGroupSceneItemList : SceneItemsRequest {
    /// <summary>
    /// Name of the group to get the items of | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  public class GetGroupSceneItemListResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Array of scene items in the group<br />
    /// </summary>
    [Key("sceneItems")]
    public List<Dictionary<string, object?>> SceneItems { get; set; } = new();
  }

  /// <summary>
  /// Searches a scene for a source, and returns its id.<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneItemId : SceneItemsRequest {
    /// <summary>
    /// Name of the scene or group to search in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Name of the source to find | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Number of matches to skip during search. >= 0 means first forward. -1 means last (top) item | >= -1 | 0<br />
    /// </summary>
    [Key("searchOffset")]
    public int? SearchOffset { get; set; }
  }

  public class GetSceneItemIdResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Numeric ID of the scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Creates a new scene item using a source.<br />
  /// Scenes only<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class CreateSceneItem : SceneItemsRequest {
    /// <summary>
    /// Name of the scene to create the new item in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Name of the source to add to the scene | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Enable state to apply to the scene item on creation | None | True<br />
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool? SceneItemEnabled { get; set; }
  }

  public class CreateSceneItemResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Numeric ID of the scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Removes a scene item from a scene.<br />
  /// Scenes only<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class RemoveSceneItem : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Duplicates a scene item, copying all transform and crop info.<br />
  /// Scenes only<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class DuplicateSceneItem : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// Name of the scene to create the duplicated item in | None | `sceneName` is assumed<br />
    /// </summary>
    [Key("destinationSceneName")]
    public string? DestinationSceneName { get; set; }
  }

  public class DuplicateSceneItemResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Numeric ID of the duplicated scene item<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Gets the transform and crop info of a scene item.<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneItemTransform : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  public class GetSceneItemTransformResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Object containing scene item transform info<br />
    /// </summary>
    [Key("sceneItemTransform")]
    public Dictionary<string, object?> SceneItemTransform { get; set; } = new();
  }

  /// <summary>
  /// Sets the transform and crop info of a scene item.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSceneItemTransform : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// Object containing scene item transform info to update | None | N/A<br />
    /// </summary>
    [Key("sceneItemTransform")]
    public Dictionary<string, object?> SceneItemTransform { get; set; } = new();
  }

  /// <summary>
  /// Gets the enable state of a scene item.<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneItemEnabled : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  public class GetSceneItemEnabledResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Whether the scene item is enabled. `true` for enabled, `false` for disabled<br />
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// Sets the enable state of a scene item.<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSceneItemEnabled : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// New enable state of the scene item | None | N/A<br />
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// Gets the lock state of a scene item.<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneItemLocked : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  public class GetSceneItemLockedResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Whether the scene item is locked. `true` for locked, `false` for unlocked<br />
    /// </summary>
    [Key("sceneItemLocked")]
    public bool SceneItemLocked { get; set; }
  }

  /// <summary>
  /// Sets the lock state of a scene item.<br />
  /// Scenes and Group<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSceneItemLocked : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// New lock state of the scene item | None | N/A<br />
    /// </summary>
    [Key("sceneItemLocked")]
    public bool SceneItemLocked { get; set; }
  }

  /// <summary>
  /// Gets the index position of a scene item in a scene.<br />
  /// An index of 0 is at the bottom of the source list in the UI.<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneItemIndex : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  public class GetSceneItemIndexResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Index position of the scene item<br />
    /// </summary>
    [Key("sceneItemIndex")]
    public int SceneItemIndex { get; set; }
  }

  /// <summary>
  /// Sets the index position of a scene item in a scene.<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSceneItemIndex : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// New index position of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemIndex")]
    public int SceneItemIndex { get; set; }
  }

  /// <summary>
  /// Gets the blend mode of a scene item.<br />
  /// Blend modes:<br />
  /// - `OBS_BLEND_NORMAL`<br />
  /// - `OBS_BLEND_ADDITIVE`<br />
  /// - `OBS_BLEND_SUBTRACT`<br />
  /// - `OBS_BLEND_SCREEN`<br />
  /// - `OBS_BLEND_MULTIPLY`<br />
  /// - `OBS_BLEND_LIGHTEN`<br />
  /// - `OBS_BLEND_DARKEN`<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetSceneItemBlendMode : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  public class GetSceneItemBlendModeResponse : SceneItemsRequestResponse {
    /// <summary>
    /// Current blend mode<br />
    /// </summary>
    [Key("sceneItemBlendMode")]
    public string SceneItemBlendMode { get; set; } = "";
  }

  /// <summary>
  /// Sets the blend mode of a scene item.<br />
  /// Scenes and Groups<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetSceneItemBlendMode : SceneItemsRequest {
    /// <summary>
    /// Name of the scene the item is in | None | N/A<br />
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
    /// <summary>
    /// Numeric ID of the scene item | >= 0 | N/A<br />
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
    /// <summary>
    /// New blend mode | None | N/A<br />
    /// </summary>
    [Key("sceneItemBlendMode")]
    public string SceneItemBlendMode { get; set; } = "";
  }

  public class OutputsRequest : Request { }
  public class OutputsRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets the status of the virtualcam output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetVirtualCamStatus : OutputsRequest { }
  public class GetVirtualCamStatusResponse : OutputsRequestResponse {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Toggles the state of the virtualcam output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class ToggleVirtualCam : OutputsRequest { }
  public class ToggleVirtualCamResponse : OutputsRequestResponse {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the virtualcam output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StartVirtualCam : OutputsRequest {
  }

  /// <summary>
  /// Stops the virtualcam output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StopVirtualCam : OutputsRequest {
  }

  /// <summary>
  /// Gets the status of the replay buffer output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetReplayBufferStatus : OutputsRequest { }
  public class GetReplayBufferStatusResponse : OutputsRequestResponse {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Toggles the state of the replay buffer output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class ToggleReplayBuffer : OutputsRequest { }
  public class ToggleReplayBufferResponse : OutputsRequestResponse {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the replay buffer output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StartReplayBuffer : OutputsRequest {
  }

  /// <summary>
  /// Stops the replay buffer output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StopReplayBuffer : OutputsRequest {
  }

  /// <summary>
  /// Saves the contents of the replay buffer output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SaveReplayBuffer : OutputsRequest {
  }

  /// <summary>
  /// Gets the filename of the last replay buffer save file.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetLastReplayBufferReplay : OutputsRequest { }
  public class GetLastReplayBufferReplayResponse : OutputsRequestResponse {
    /// <summary>
    /// File path<br />
    /// </summary>
    [Key("savedReplayPath")]
    public string SavedReplayPath { get; set; } = "";
  }

  /// <summary>
  /// Gets the list of available outputs.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetOutputList : OutputsRequest {
  }

  public class GetOutputListResponse : OutputsRequestResponse {
    [Key("outputs")]
    public List<Output> Outputs { get; set; } = new();
  }

  /// <summary>
  /// Gets the status of an output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetOutputStatus : OutputsRequest {
    /// <summary>
    /// Output name | None | N/A<br />
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  public class GetOutputStatusResponse : OutputsRequestResponse {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
    /// <summary>
    /// Whether the output is reconnecting<br />
    /// </summary>
    [Key("outputReconnecting")]
    public bool OutputReconnecting { get; set; }
    /// <summary>
    /// Current formatted timecode string for the output<br />
    /// </summary>
    [Key("outputTimecode")]
    public string OutputTimecode { get; set; } = "";
    /// <summary>
    /// Current duration in milliseconds for the output<br />
    /// </summary>
    [Key("outputDuration")]
    public int OutputDuration { get; set; }
    /// <summary>
    /// Congestion of the output<br />
    /// </summary>
    [Key("outputCongestion")]
    public double OutputCongestion { get; set; }
    /// <summary>
    /// Number of bytes sent by the output<br />
    /// </summary>
    [Key("outputBytes")]
    public int OutputBytes { get; set; }
    /// <summary>
    /// Number of frames skipped by the output's process<br />
    /// </summary>
    [Key("outputSkippedFrames")]
    public int OutputSkippedFrames { get; set; }
    /// <summary>
    /// Total number of frames delivered by the output's process<br />
    /// </summary>
    [Key("outputTotalFrames")]
    public int OutputTotalFrames { get; set; }
  }

  /// <summary>
  /// Toggles the status of an output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class ToggleOutput : OutputsRequest {
    /// <summary>
    /// Output name | None | N/A<br />
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  public class ToggleOutputResponse : OutputsRequestResponse {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts an output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StartOutput : OutputsRequest {
    /// <summary>
    /// Output name | None | N/A<br />
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Stops an output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StopOutput : OutputsRequest {
    /// <summary>
    /// Output name | None | N/A<br />
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Gets the settings of an output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetOutputSettings : OutputsRequest {
    /// <summary>
    /// Output name | None | N/A<br />
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  public class GetOutputSettingsResponse : OutputsRequestResponse {
    /// <summary>
    /// Output settings<br />
    /// </summary>
    [Key("outputSettings")]
    public Dictionary<string, object?> OutputSettings { get; set; } = new();
  }

  /// <summary>
  /// Sets the settings of an output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetOutputSettings : OutputsRequest {
    /// <summary>
    /// Output name | None | N/A<br />
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
    /// <summary>
    /// Output settings | None | N/A<br />
    /// </summary>
    [Key("outputSettings")]
    public Dictionary<string, object?> OutputSettings { get; set; } = new();
  }

  public class StreamRequest : Request { }
  public class StreamRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets the status of the stream output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetStreamStatus : StreamRequest { }
  public class GetStreamStatusResponse : StreamRequestResponse {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
    /// <summary>
    /// Whether the output is currently reconnecting<br />
    /// </summary>
    [Key("outputReconnecting")]
    public bool OutputReconnecting { get; set; }
    /// <summary>
    /// Current formatted timecode string for the output<br />
    /// </summary>
    [Key("outputTimecode")]
    public string OutputTimecode { get; set; } = "";
    /// <summary>
    /// Current duration in milliseconds for the output<br />
    /// </summary>
    [Key("outputDuration")]
    public int OutputDuration { get; set; }
    /// <summary>
    /// Congestion of the output<br />
    /// </summary>
    [Key("outputCongestion")]
    public double OutputCongestion { get; set; }
    /// <summary>
    /// Number of bytes sent by the output<br />
    /// </summary>
    [Key("outputBytes")]
    public int OutputBytes { get; set; }
    /// <summary>
    /// Number of frames skipped by the output's process<br />
    /// </summary>
    [Key("outputSkippedFrames")]
    public int OutputSkippedFrames { get; set; }
    /// <summary>
    /// Total number of frames delivered by the output's process<br />
    /// </summary>
    [Key("outputTotalFrames")]
    public int OutputTotalFrames { get; set; }
  }

  /// <summary>
  /// Toggles the status of the stream output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class ToggleStream : StreamRequest { }
  public class ToggleStreamResponse : StreamRequestResponse {
    /// <summary>
    /// New state of the stream output<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the stream output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StartStream : StreamRequest {
  }

  /// <summary>
  /// Stops the stream output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StopStream : StreamRequest {
  }

  /// <summary>
  /// Sends CEA-608 caption text over the stream output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SendStreamCaption : StreamRequest {
    /// <summary>
    /// Caption text | None | N/A<br />
    /// </summary>
    [Key("captionText")]
    public string CaptionText { get; set; } = "";
  }

  public class RecordRequest : Request { }
  public class RecordRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets the status of the record output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetRecordStatus : RecordRequest { }
  public class GetRecordStatusResponse : RecordRequestResponse {
    /// <summary>
    /// Whether the output is active<br />
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
    /// <summary>
    /// Whether the output is paused<br />
    /// </summary>
    [Key("outputPaused")]
    public bool OutputPaused { get; set; }
    /// <summary>
    /// Current formatted timecode string for the output<br />
    /// </summary>
    [Key("outputTimecode")]
    public string OutputTimecode { get; set; } = "";
    /// <summary>
    /// Current duration in milliseconds for the output<br />
    /// </summary>
    [Key("outputDuration")]
    public int OutputDuration { get; set; }
    /// <summary>
    /// Number of bytes sent by the output<br />
    /// </summary>
    [Key("outputBytes")]
    public int OutputBytes { get; set; }
  }

  /// <summary>
  /// Toggles the status of the record output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class ToggleRecord : RecordRequest {
  }

  /// <summary>
  /// Starts the record output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StartRecord : RecordRequest {
  }

  /// <summary>
  /// Stops the record output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class StopRecord : RecordRequest { }
  public class StopRecordResponse : RecordRequestResponse {
    /// <summary>
    /// File name for the saved recording<br />
    /// </summary>
    [Key("outputPath")]
    public string OutputPath { get; set; } = "";
  }

  /// <summary>
  /// Toggles pause on the record output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class ToggleRecordPause : RecordRequest {
  }

  /// <summary>
  /// Pauses the record output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class PauseRecord : RecordRequest {
  }

  /// <summary>
  /// Resumes the record output.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class ResumeRecord : RecordRequest {
  }

  public class MediaInputsRequest : Request { }
  public class MediaInputsRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets the status of a media input.<br />
  /// Media States:<br />
  /// - `OBS_MEDIA_STATE_NONE`<br />
  /// - `OBS_MEDIA_STATE_PLAYING`<br />
  /// - `OBS_MEDIA_STATE_OPENING`<br />
  /// - `OBS_MEDIA_STATE_BUFFERING`<br />
  /// - `OBS_MEDIA_STATE_PAUSED`<br />
  /// - `OBS_MEDIA_STATE_STOPPED`<br />
  /// - `OBS_MEDIA_STATE_ENDED`<br />
  /// - `OBS_MEDIA_STATE_ERROR`<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetMediaInputStatus : MediaInputsRequest {
    /// <summary>
    /// Name of the media input | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  public class GetMediaInputStatusResponse : MediaInputsRequestResponse {
    /// <summary>
    /// State of the media input<br />
    /// </summary>
    [Key("mediaState")]
    public MediaState MediaState { get; set; }
    /// <summary>
    /// Total duration of the playing media in milliseconds. `null` if not playing<br />
    /// </summary>
    [Key("mediaDuration")]
    public int MediaDuration { get; set; }
    /// <summary>
    /// Position of the cursor in milliseconds. `null` if not playing<br />
    /// </summary>
    [Key("mediaCursor")]
    public int MediaCursor { get; set; }
  }

  /// <summary>
  /// Sets the cursor position of a media input.<br />
  /// This request does not perform bounds checking of the cursor position.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetMediaInputCursor : MediaInputsRequest {
    /// <summary>
    /// Name of the media input | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// New cursor position to set | >= 0 | N/A<br />
    /// </summary>
    [Key("mediaCursor")]
    public double MediaCursor { get; set; }
  }

  /// <summary>
  /// Offsets the current cursor position of a media input by the specified value.<br />
  /// This request does not perform bounds checking of the cursor position.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class OffsetMediaInputCursor : MediaInputsRequest {
    /// <summary>
    /// Name of the media input | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Value to offset the current cursor position by | None | N/A<br />
    /// </summary>
    [Key("mediaCursorOffset")]
    public int MediaCursorOffset { get; set; }
  }

  /// <summary>
  /// Triggers an action on a media input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class TriggerMediaInputAction : MediaInputsRequest {
    /// <summary>
    /// Name of the media input | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
    /// <summary>
    /// Identifier of the `ObsMediaInputAction` enum | None | N/A<br />
    /// </summary>
    [Key("mediaAction")]
    public string MediaAction { get; set; } = "";
  }

  public class UiRequest : Request { }
  public class UiRequestResponse : RequestResponse { }
  /// <summary>
  /// Gets whether studio is enabled.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetStudioModeEnabled : UiRequest { }
  public class GetStudioModeEnabledResponse : UiRequestResponse {
    /// <summary>
    /// Whether studio mode is enabled<br />
    /// </summary>
    [Key("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }

  /// <summary>
  /// Enables or disables studio mode<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class SetStudioModeEnabled : UiRequest {
    /// <summary>
    /// True == Enabled, False == Disabled | None | N/A<br />
    /// </summary>
    [Key("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }

  /// <summary>
  /// Opens the properties dialog of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class OpenInputPropertiesDialog : UiRequest {
    /// <summary>
    /// Name of the input to open the dialog of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Opens the filters dialog of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class OpenInputFiltersDialog : UiRequest {
    /// <summary>
    /// Name of the input to open the dialog of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Opens the interact dialog of an input.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class OpenInputInteractDialog : UiRequest {
    /// <summary>
    /// Name of the input to open the dialog of | None | N/A<br />
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Gets a list of connected monitors and information about them.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class GetMonitorList : UiRequest { }
  public class GetMonitorListResponse : UiRequestResponse {
    /// <summary>
    /// a list of detected monitors with some information<br />
    /// </summary>
    [Key("monitors")]
    public List<Dictionary<string, object?>> Monitors { get; set; } = new();
  }

  /// <summary>
  /// Opens a projector for a specific output video mix.<br />
  /// Mix types:<br />
  /// - `OBS_WEBSOCKET_VIDEO_MIX_TYPE_PREVIEW`<br />
  /// - `OBS_WEBSOCKET_VIDEO_MIX_TYPE_PROGRAM`<br />
  /// - `OBS_WEBSOCKET_VIDEO_MIX_TYPE_MULTIVIEW`<br />
  /// Note: This request serves to provide feature parity with 4.x. It is very likely to be changed/deprecated in a future release.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class OpenVideoMixProjector : UiRequest {
    /// <summary>
    /// Type of mix to open | None | N/A<br />
    /// </summary>
    [Key("videoMixType")]
    public string VideoMixType { get; set; } = "";
    /// <summary>
    /// Monitor index, use `GetMonitorList` to obtain index | None | -1: Opens projector in windowed mode<br />
    /// </summary>
    [Key("monitorIndex")]
    public int? MonitorIndex { get; set; }
    /// <summary>
    /// Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with `monitorIndex` | None | N/A<br />
    /// </summary>
    [Key("projectorGeometry")]
    public string? ProjectorGeometry { get; set; }
  }

  /// <summary>
  /// Opens a projector for a source.<br />
  /// Note: This request serves to provide feature parity with 4.x. It is very likely to be changed/deprecated in a future release.<br />
  /// - Latest Supported RPC Version: `1`<br />
  /// - Added in v5.0.0<br />
  /// </summary>
  public class OpenSourceProjector : UiRequest {
    /// <summary>
    /// Name of the source to open a projector for | None | N/A<br />
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
    /// <summary>
    /// Monitor index, use `GetMonitorList` to obtain index | None | -1: Opens projector in windowed mode<br />
    /// </summary>
    [Key("monitorIndex")]
    public int? MonitorIndex { get; set; }
    /// <summary>
    /// Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with `monitorIndex` | None | N/A<br />
    /// </summary>
    [Key("projectorGeometry")]
    public string? ProjectorGeometry { get; set; }
  }

}
