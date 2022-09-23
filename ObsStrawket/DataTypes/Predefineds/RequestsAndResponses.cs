using MessagePack;
using System.Collections.Generic;

namespace ObsStrawket.DataTypes.Predefineds {

  /// <summary>
  /// Gets data about the current plugin and RPC version.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetVersion : Request { }

  /// <summary>
  /// Response of GetVersion
  /// </summary>
  public class GetVersionResponse : RequestResponse {
    /// <summary>
    /// Current OBS Studio version
    /// </summary>
    [Key("obsVersion")]
    public string ObsVersion { get; set; } = "";

    /// <summary>
    /// Current obs-websocket version
    /// </summary>
    [Key("obsWebSocketVersion")]
    public string ObsWebSocketVersion { get; set; } = "";

    /// <summary>
    /// Current latest obs-websocket RPC version
    /// </summary>
    [Key("rpcVersion")]
    public int RpcVersion { get; set; }

    /// <summary>
    /// Array of available RPC requests for the currently negotiated RPC version
    /// </summary>
    [Key("availableRequests")]
    public List<string> AvailableRequests { get; set; } = new();

    /// <summary>
    /// Image formats available in <c>GetSourceScreenshot</c> and <c>SaveSourceScreenshot</c> requests.
    /// </summary>
    [Key("supportedImageFormats")]
    public List<string> SupportedImageFormats { get; set; } = new();

    /// <summary>
    /// Name of the platform. Usually <c>windows</c>, <c>macos</c>, or <c>ubuntu</c> (linux flavor). Not guaranteed to be any of those
    /// </summary>
    [Key("platform")]
    public string Platform { get; set; } = "";

    /// <summary>
    /// Description of the platform, like <c>Windows 10 (10.0)</c>
    /// </summary>
    [Key("platformDescription")]
    public string PlatformDescription { get; set; } = "";
  }

  /// <summary>
  /// Gets statistics about OBS, obs-websocket, and the current session.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetStats : Request { }

  /// <summary>
  /// Response of GetStats
  /// </summary>
  public class GetStatsResponse : RequestResponse {
    /// <summary>
    /// Current CPU usage in percent
    /// </summary>
    [Key("cpuUsage")]
    public double CpuUsage { get; set; }

    /// <summary>
    /// Amount of memory in MB currently being used by OBS
    /// </summary>
    [Key("memoryUsage")]
    public double MemoryUsage { get; set; }

    /// <summary>
    /// Available disk space on the device being used for recording storage
    /// </summary>
    [Key("availableDiskSpace")]
    public double AvailableDiskSpace { get; set; }

    /// <summary>
    /// Current FPS being rendered
    /// </summary>
    [Key("activeFps")]
    public double ActiveFps { get; set; }

    /// <summary>
    /// Average time in milliseconds that OBS is taking to render a frame
    /// </summary>
    [Key("averageFrameRenderTime")]
    public double AverageFrameRenderTime { get; set; }

    /// <summary>
    /// Number of frames skipped by OBS in the render thread
    /// </summary>
    [Key("renderSkippedFrames")]
    public int RenderSkippedFrames { get; set; }

    /// <summary>
    /// Total number of frames outputted by the render thread
    /// </summary>
    [Key("renderTotalFrames")]
    public int RenderTotalFrames { get; set; }

    /// <summary>
    /// Number of frames skipped by OBS in the output thread
    /// </summary>
    [Key("outputSkippedFrames")]
    public int OutputSkippedFrames { get; set; }

    /// <summary>
    /// Total number of frames outputted by the output thread
    /// </summary>
    [Key("outputTotalFrames")]
    public int OutputTotalFrames { get; set; }

    /// <summary>
    /// Total number of messages received by obs-websocket from the client
    /// </summary>
    [Key("webSocketSessionIncomingMessages")]
    public int WebSocketSessionIncomingMessages { get; set; }

    /// <summary>
    /// Total number of messages sent by obs-websocket to the client
    /// </summary>
    [Key("webSocketSessionOutgoingMessages")]
    public int WebSocketSessionOutgoingMessages { get; set; }
  }

  /// <summary>
  /// Broadcasts a <c>CustomEvent</c> to all WebSocket clients. Receivers are clients which are identified and subscribed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class BroadcastCustomEvent : Request {
    /// <summary>
    /// Data payload to emit to all receivers
    /// </summary>
    [Key("eventData")]
    public Dictionary<string, object?> EventData { get; set; } = new();
  }

  /// <summary>
  /// Call a request registered to a vendor.<br />
  /// <br />
  /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
  /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CallVendorRequest : Request {
    /// <summary>
    /// Name of the vendor to use
    /// </summary>
    [Key("vendorName")]
    public string VendorName { get; set; } = "";

    /// <summary>
    /// The request type to call
    /// </summary>
    [Key("requestType")]
    public string VendorRequestType { get; set; } = "";

    /// <summary>
    /// Object containing appropriate request data<br />
    /// If null, {}
    /// </summary>
    [Key("requestData")]
    public Dictionary<string, object?>? RequestData { get; set; } = new();
  }

  /// <summary>
  /// Response of CallVendorRequest
  /// </summary>
  public class CallVendorRequestResponse : RequestResponse {
    /// <summary>
    /// Echoed of <c>vendorName</c>
    /// </summary>
    [Key("vendorName")]
    public string VendorName { get; set; } = "";

    /// <summary>
    /// Echoed of <c>requestType</c>
    /// </summary>
    [Key("requestType")]
    public string VendorRequestType { get; set; } = "";

    /// <summary>
    /// Object containing appropriate response data. {} if request does not provide any response data
    /// </summary>
    [Key("responseData")]
    public Dictionary<string, object?> ResponseData { get; set; } = new();
  }

  /// <summary>
  /// Gets an array of all hotkey names in OBS<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetHotkeyList : Request { }

  /// <summary>
  /// Response of GetHotkeyList
  /// </summary>
  public class GetHotkeyListResponse : RequestResponse {
    /// <summary>
    /// Array of hotkey names
    /// </summary>
    [Key("hotkeys")]
    public List<string> Hotkeys { get; set; } = new();
  }

  /// <summary>
  /// Triggers a hotkey using its name. See <c>GetHotkeyList</c><br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class TriggerHotkeyByName : Request {
    /// <summary>
    /// Name of the hotkey to trigger
    /// </summary>
    [Key("hotkeyName")]
    public string HotkeyName { get; set; } = "";
  }

  /// <summary>
  /// Triggers a hotkey using a sequence of keys.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class TriggerHotkeyByKeySequence : Request {
    /// <summary>
    /// The OBS key ID to use. See https://github.com/obsproject/obs-studio/blob/master/libobs/obs-hotkeys.h<br />
    /// If null, Not pressed
    /// </summary>
    [Key("keyId")]
    public string? KeyId { get; set; } = "";

    /// <summary>
    /// Object containing key modifiers to apply<br />
    /// If null, Ignored
    /// </summary>
    [Key("keyModifiers")]
    public KeyModifiers? KeyModifiers { get; set; } = new();
  }

  /// <summary>
  /// Sleeps for a time duration or number of frames. Only available in request batches with types <c>SERIAL_REALTIME</c> or <c>SERIAL_FRAME</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class Sleep : Request {
    /// <summary>
    /// Number of milliseconds to sleep for (if <c>SERIAL_REALTIME</c> mode)<br />
    /// It should be &gt;= 0, &lt;= 50000
    /// </summary>
    [Key("sleepMillis")]
    public int SleepMillis { get; set; }

    /// <summary>
    /// Number of frames to sleep for (if <c>SERIAL_FRAME</c> mode)<br />
    /// It should be &gt;= 0, &lt;= 10000
    /// </summary>
    [Key("sleepFrames")]
    public int SleepFrames { get; set; }
  }

  /// <summary>
  /// Gets the value of a "slot" from the selected persistent data realm.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetPersistentData : Request {
    /// <summary>
    /// The data realm to select. <c>OBS_WEBSOCKET_DATA_REALM_GLOBAL</c> or <c>OBS_WEBSOCKET_DATA_REALM_PROFILE</c>
    /// </summary>
    [Key("realm")]
    public DataRealm Realm { get; set; } = new();

    /// <summary>
    /// The name of the slot to retrieve data from
    /// </summary>
    [Key("slotName")]
    public string SlotName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetPersistentData
  /// </summary>
  public class GetPersistentDataResponse : RequestResponse {
    /// <summary>
    /// Value associated with the slot. <c>null</c> if not set
    /// </summary>
    [Key("slotValue")]
    public object? SlotValue { get; set; }
  }

  /// <summary>
  /// Sets the value of a "slot" from the selected persistent data realm.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetPersistentData : Request {
    /// <summary>
    /// The data realm to select. <c>OBS_WEBSOCKET_DATA_REALM_GLOBAL</c> or <c>OBS_WEBSOCKET_DATA_REALM_PROFILE</c>
    /// </summary>
    [Key("realm")]
    public DataRealm Realm { get; set; } = new();

    /// <summary>
    /// The name of the slot to retrieve data from
    /// </summary>
    [Key("slotName")]
    public string SlotName { get; set; } = "";

    /// <summary>
    /// The value to apply to the slot
    /// </summary>
    [Key("slotValue")]
    public object? SlotValue { get; set; }
  }

  /// <summary>
  /// Gets an array of all scene collections<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneCollectionList : Request { }

  /// <summary>
  /// Response of GetSceneCollectionList
  /// </summary>
  public class GetSceneCollectionListResponse : RequestResponse {
    /// <summary>
    /// The name of the current scene collection
    /// </summary>
    [Key("currentSceneCollectionName")]
    public string CurrentSceneCollectionName { get; set; } = "";

    /// <summary>
    /// Array of all available scene collections
    /// </summary>
    [Key("sceneCollections")]
    public List<string> SceneCollections { get; set; } = new();
  }

  /// <summary>
  /// Switches to a scene collection.<br />
  /// <br />
  /// Note: This will block until the collection has finished changing.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetCurrentSceneCollection : Request {
    /// <summary>
    /// Name of the scene collection to switch to
    /// </summary>
    [Key("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }

  /// <summary>
  /// Creates a new scene collection, switching to it in the process.<br />
  /// <br />
  /// Note: This will block until the collection has finished changing.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CreateSceneCollection : Request {
    /// <summary>
    /// Name for the new scene collection
    /// </summary>
    [Key("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }

  /// <summary>
  /// Gets an array of all profiles<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetProfileList : Request { }

  /// <summary>
  /// Response of GetProfileList
  /// </summary>
  public class GetProfileListResponse : RequestResponse {
    /// <summary>
    /// The name of the current profile
    /// </summary>
    [Key("currentProfileName")]
    public string CurrentProfileName { get; set; } = "";

    /// <summary>
    /// Array of all available profiles
    /// </summary>
    [Key("profiles")]
    public List<string> Profiles { get; set; } = new();
  }

  /// <summary>
  /// Switches to a profile.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetCurrentProfile : Request {
    /// <summary>
    /// Name of the profile to switch to
    /// </summary>
    [Key("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Creates a new profile, switching to it in the process<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CreateProfile : Request {
    /// <summary>
    /// Name for the new profile
    /// </summary>
    [Key("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Removes a profile. If the current profile is chosen, it will change to a different profile first.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class RemoveProfile : Request {
    /// <summary>
    /// Name of the profile to remove
    /// </summary>
    [Key("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Gets a parameter from the current profile's configuration.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetProfileParameter : Request {
    /// <summary>
    /// Category of the parameter to get
    /// </summary>
    [Key("parameterCategory")]
    public string ParameterCategory { get; set; } = "";

    /// <summary>
    /// Name of the parameter to get
    /// </summary>
    [Key("parameterName")]
    public string ParameterName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetProfileParameter
  /// </summary>
  public class GetProfileParameterResponse : RequestResponse {
    /// <summary>
    /// Value associated with the parameter. <c>null</c> if not set and no default
    /// </summary>
    [Key("parameterValue")]
    public string? ParameterValue { get; set; }

    /// <summary>
    /// Default value associated with the parameter. <c>null</c> if no default
    /// </summary>
    [Key("defaultParameterValue")]
    public string? DefaultParameterValue { get; set; }
  }

  /// <summary>
  /// Sets the value of a parameter in the current profile's configuration.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetProfileParameter : Request {
    /// <summary>
    /// Category of the parameter to set
    /// </summary>
    [Key("parameterCategory")]
    public string ParameterCategory { get; set; } = "";

    /// <summary>
    /// Name of the parameter to set
    /// </summary>
    [Key("parameterName")]
    public string ParameterName { get; set; } = "";

    /// <summary>
    /// Value of the parameter to set. Use <c>null</c> to delete
    /// </summary>
    [Key("parameterValue")]
    public string? ParameterValue { get; set; }
  }

  /// <summary>
  /// Gets the current video settings.<br />
  /// <br />
  /// Note: To get the true FPS value, divide the FPS numerator by the FPS denominator. Example: <c>60000/1001</c><br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetVideoSettings : Request { }

  /// <summary>
  /// Response of GetVideoSettings
  /// </summary>
  public class GetVideoSettingsResponse : RequestResponse {
    /// <summary>
    /// Numerator of the fractional FPS value
    /// </summary>
    [Key("fpsNumerator")]
    public int FpsNumerator { get; set; }

    /// <summary>
    /// Denominator of the fractional FPS value
    /// </summary>
    [Key("fpsDenominator")]
    public int FpsDenominator { get; set; }

    /// <summary>
    /// Width of the base (canvas) resolution in pixels
    /// </summary>
    [Key("baseWidth")]
    public int BaseWidth { get; set; }

    /// <summary>
    /// Height of the base (canvas) resolution in pixels
    /// </summary>
    [Key("baseHeight")]
    public int BaseHeight { get; set; }

    /// <summary>
    /// Width of the output resolution in pixels
    /// </summary>
    [Key("outputWidth")]
    public int OutputWidth { get; set; }

    /// <summary>
    /// Height of the output resolution in pixels
    /// </summary>
    [Key("outputHeight")]
    public int OutputHeight { get; set; }
  }

  /// <summary>
  /// Sets the current video settings.<br />
  /// <br />
  /// Note: Fields must be specified in pairs. For example, you cannot set only <c>baseWidth</c> without needing to specify <c>baseHeight</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetVideoSettings : Request {
    /// <summary>
    /// Numerator of the fractional FPS value<br />
    /// It should be &gt;= 1<br />
    /// If null, Not changed
    /// </summary>
    [Key("fpsNumerator")]
    public int? FpsNumerator { get; set; }

    /// <summary>
    /// Denominator of the fractional FPS value<br />
    /// It should be &gt;= 1<br />
    /// If null, Not changed
    /// </summary>
    [Key("fpsDenominator")]
    public int? FpsDenominator { get; set; }

    /// <summary>
    /// Width of the base (canvas) resolution in pixels<br />
    /// It should be &gt;= 1, &lt;= 4096<br />
    /// If null, Not changed
    /// </summary>
    [Key("baseWidth")]
    public int? BaseWidth { get; set; }

    /// <summary>
    /// Height of the base (canvas) resolution in pixels<br />
    /// It should be &gt;= 1, &lt;= 4096<br />
    /// If null, Not changed
    /// </summary>
    [Key("baseHeight")]
    public int? BaseHeight { get; set; }

    /// <summary>
    /// Width of the output resolution in pixels<br />
    /// It should be &gt;= 1, &lt;= 4096<br />
    /// If null, Not changed
    /// </summary>
    [Key("outputWidth")]
    public int? OutputWidth { get; set; }

    /// <summary>
    /// Height of the output resolution in pixels<br />
    /// It should be &gt;= 1, &lt;= 4096<br />
    /// If null, Not changed
    /// </summary>
    [Key("outputHeight")]
    public int? OutputHeight { get; set; }
  }

  /// <summary>
  /// Gets the current stream service settings (stream destination).<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetStreamServiceSettings : Request { }

  /// <summary>
  /// Response of GetStreamServiceSettings
  /// </summary>
  public class GetStreamServiceSettingsResponse : RequestResponse {
    /// <summary>
    /// Stream service type, like <c>rtmp_custom</c> or <c>rtmp_common</c>
    /// </summary>
    [Key("streamServiceType")]
    public StreamServiceType StreamServiceType { get; set; } = new();

    /// <summary>
    /// Stream service settings
    /// </summary>
    [Key("streamServiceSettings")]
    public Dictionary<string, object?> StreamServiceSettings { get; set; } = new();
  }

  /// <summary>
  /// Sets the current stream service settings (stream destination).<br />
  /// <br />
  /// Note: Simple RTMP settings can be set with type <c>rtmp_custom</c> and the settings fields <c>server</c> and <c>key</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetStreamServiceSettings : Request {
    /// <summary>
    /// Type of stream service to apply. Example: <c>rtmp_common</c> or <c>rtmp_custom</c>
    /// </summary>
    [Key("streamServiceType")]
    public StreamServiceType StreamServiceType { get; set; } = new();

    /// <summary>
    /// Settings to apply to the service
    /// </summary>
    [Key("streamServiceSettings")]
    public Dictionary<string, object?> StreamServiceSettings { get; set; } = new();
  }

  /// <summary>
  /// Gets the current directory that the record output is set to.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetRecordDirectory : Request { }

  /// <summary>
  /// Response of GetRecordDirectory
  /// </summary>
  public class GetRecordDirectoryResponse : RequestResponse {
    /// <summary>
    /// Output directory
    /// </summary>
    [Key("recordDirectory")]
    public string RecordDirectory { get; set; } = "";
  }

  /// <summary>
  /// Gets the active and show state of a source.<br />
  /// <br />
  /// **Compatible with inputs and scenes.**<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSourceActive : Request {
    /// <summary>
    /// Name of the source to get the active state of
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetSourceActive
  /// </summary>
  public class GetSourceActiveResponse : RequestResponse {
    /// <summary>
    /// Whether the source is showing in Program
    /// </summary>
    [Key("videoActive")]
    public bool VideoActive { get; set; }

    /// <summary>
    /// Whether the source is showing in the UI (Preview, Projector, Properties)
    /// </summary>
    [Key("videoShowing")]
    public bool VideoShowing { get; set; }
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
  [MessagePackObject]
  public class GetSourceScreenshot : Request {
    /// <summary>
    /// Name of the source to take a screenshot of
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Image compression format to use. Use <c>GetVersion</c> to get compatible image formats
    /// </summary>
    [Key("imageFormat")]
    public string ImageFormat { get; set; } = "";

    /// <summary>
    /// Width to scale the screenshot to<br />
    /// It should be &gt;= 8, &lt;= 4096<br />
    /// If null, Source value is used
    /// </summary>
    [Key("imageWidth")]
    public int? ImageWidth { get; set; }

    /// <summary>
    /// Height to scale the screenshot to<br />
    /// It should be &gt;= 8, &lt;= 4096<br />
    /// If null, Source value is used
    /// </summary>
    [Key("imageHeight")]
    public int? ImageHeight { get; set; }

    /// <summary>
    /// Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk)<br />
    /// It should be &gt;= -1, &lt;= 100<br />
    /// If null, -1
    /// </summary>
    [Key("imageCompressionQuality")]
    public int? ImageCompressionQuality { get; set; }
  }

  /// <summary>
  /// Response of GetSourceScreenshot
  /// </summary>
  public class GetSourceScreenshotResponse : RequestResponse {
    /// <summary>
    /// Base64-encoded screenshot
    /// </summary>
    [Key("imageData")]
    public string ImageData { get; set; } = "";
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
  [MessagePackObject]
  public class SaveSourceScreenshot : Request {
    /// <summary>
    /// Name of the source to take a screenshot of
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Image compression format to use. Use <c>GetVersion</c> to get compatible image formats
    /// </summary>
    [Key("imageFormat")]
    public string ImageFormat { get; set; } = "";

    /// <summary>
    /// Path to save the screenshot file to. Eg. <c>C:\Users\user\Desktop\screenshot.png</c>
    /// </summary>
    [Key("imageFilePath")]
    public string ImageFilePath { get; set; } = "";

    /// <summary>
    /// Width to scale the screenshot to<br />
    /// It should be &gt;= 8, &lt;= 4096<br />
    /// If null, Source value is used
    /// </summary>
    [Key("imageWidth")]
    public int? ImageWidth { get; set; }

    /// <summary>
    /// Height to scale the screenshot to<br />
    /// It should be &gt;= 8, &lt;= 4096<br />
    /// If null, Source value is used
    /// </summary>
    [Key("imageHeight")]
    public int? ImageHeight { get; set; }

    /// <summary>
    /// Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk)<br />
    /// It should be &gt;= -1, &lt;= 100<br />
    /// If null, -1
    /// </summary>
    [Key("imageCompressionQuality")]
    public int? ImageCompressionQuality { get; set; }
  }

  /// <summary>
  /// Gets an array of all scenes in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneList : Request { }

  /// <summary>
  /// Response of GetSceneList
  /// </summary>
  public class GetSceneListResponse : RequestResponse {
    /// <summary>
    /// Current program scene
    /// </summary>
    [Key("currentProgramSceneName")]
    public string CurrentProgramSceneName { get; set; } = "";

    /// <summary>
    /// Current preview scene. <c>null</c> if not in studio mode
    /// </summary>
    [Key("currentPreviewSceneName")]
    public string? CurrentPreviewSceneName { get; set; }

    /// <summary>
    /// Array of scenes
    /// </summary>
    [Key("scenes")]
    public List<Scene> Scenes { get; set; } = new();
  }

  /// <summary>
  /// Gets an array of all groups in OBS.<br />
  /// <br />
  /// Groups in OBS are actually scenes, but renamed and modified. In obs-websocket, we treat them as scenes where we can.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetGroupList : Request { }

  /// <summary>
  /// Response of GetGroupList
  /// </summary>
  public class GetGroupListResponse : RequestResponse {
    /// <summary>
    /// Array of group names
    /// </summary>
    [Key("groups")]
    public List<string> Groups { get; set; } = new();
  }

  /// <summary>
  /// Gets the current program scene.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetCurrentProgramScene : Request { }

  /// <summary>
  /// Response of GetCurrentProgramScene
  /// </summary>
  public class GetCurrentProgramSceneResponse : RequestResponse {
    /// <summary>
    /// Current program scene
    /// </summary>
    [Key("currentProgramSceneName")]
    public string CurrentProgramSceneName { get; set; } = "";
  }

  /// <summary>
  /// Sets the current program scene.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetCurrentProgramScene : Request {
    /// <summary>
    /// Scene to set as the current program scene
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Gets the current preview scene.<br />
  /// <br />
  /// Only available when studio mode is enabled.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetCurrentPreviewScene : Request { }

  /// <summary>
  /// Response of GetCurrentPreviewScene
  /// </summary>
  public class GetCurrentPreviewSceneResponse : RequestResponse {
    /// <summary>
    /// Current preview scene
    /// </summary>
    [Key("currentPreviewSceneName")]
    public string CurrentPreviewSceneName { get; set; } = "";
  }

  /// <summary>
  /// Sets the current preview scene.<br />
  /// <br />
  /// Only available when studio mode is enabled.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetCurrentPreviewScene : Request {
    /// <summary>
    /// Scene to set as the current preview scene
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Creates a new scene in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CreateScene : Request {
    /// <summary>
    /// Name for the new scene
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Removes a scene from OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class RemoveScene : Request {
    /// <summary>
    /// Name of the scene to remove
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Sets the name of a scene (rename).<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSceneName : Request {
    /// <summary>
    /// Name of the scene to be renamed
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// New name for the scene
    /// </summary>
    [Key("newSceneName")]
    public string NewSceneName { get; set; } = "";
  }

  /// <summary>
  /// Gets the scene transition overridden for a scene.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneSceneTransitionOverride : Request {
    /// <summary>
    /// Name of the scene
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetSceneSceneTransitionOverride
  /// </summary>
  public class GetSceneSceneTransitionOverrideResponse : RequestResponse {
    /// <summary>
    /// Name of the overridden scene transition, else <c>null</c>
    /// </summary>
    [Key("transitionName")]
    public string? TransitionName { get; set; }

    /// <summary>
    /// Duration of the overridden scene transition, else <c>null</c>
    /// </summary>
    [Key("transitionDuration")]
    public int? TransitionDuration { get; set; }
  }

  /// <summary>
  /// Gets the scene transition overridden for a scene.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSceneSceneTransitionOverride : Request {
    /// <summary>
    /// Name of the scene
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Name of the scene transition to use as override. Specify <c>null</c> to remove<br />
    /// If null, Unchanged
    /// </summary>
    [Key("transitionName")]
    public string? TransitionName { get; set; }

    /// <summary>
    /// Duration to use for any overridden transition. Specify <c>null</c> to remove<br />
    /// It should be &gt;= 50, &lt;= 20000<br />
    /// If null, Unchanged
    /// </summary>
    [Key("transitionDuration")]
    public int? TransitionDuration { get; set; }
  }

  /// <summary>
  /// Gets an array of all inputs in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputList : Request {
    /// <summary>
    /// Restrict the array to only inputs of the specified kind<br />
    /// If null, All kinds included
    /// </summary>
    [Key("inputKind")]
    public string? InputKind { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputList
  /// </summary>
  public class GetInputListResponse : RequestResponse {
    /// <summary>
    /// Array of inputs
    /// </summary>
    [Key("inputs")]
    public List<Input> Inputs { get; set; } = new();
  }

  /// <summary>
  /// Gets an array of all available input kinds in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputKindList : Request {
    /// <summary>
    /// True == Return all kinds as unversioned, False == Return with version suffixes (if available)<br />
    /// If null, false
    /// </summary>
    [Key("unversioned")]
    public bool? Unversioned { get; set; }
  }

  /// <summary>
  /// Response of GetInputKindList
  /// </summary>
  public class GetInputKindListResponse : RequestResponse {
    /// <summary>
    /// Array of input kinds
    /// </summary>
    [Key("inputKinds")]
    public List<string> InputKinds { get; set; } = new();
  }

  /// <summary>
  /// Gets the names of all special inputs.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSpecialInputs : Request { }

  /// <summary>
  /// Response of GetSpecialInputs
  /// </summary>
  public class GetSpecialInputsResponse : RequestResponse {
    /// <summary>
    /// Name of the Desktop Audio input
    /// </summary>
    [Key("desktop1")]
    public string Desktop1 { get; set; } = "";

    /// <summary>
    /// Name of the Desktop Audio 2 input
    /// </summary>
    [Key("desktop2")]
    public string Desktop2 { get; set; } = "";

    /// <summary>
    /// Name of the Mic/Auxiliary Audio input
    /// </summary>
    [Key("mic1")]
    public string Mic1 { get; set; } = "";

    /// <summary>
    /// Name of the Mic/Auxiliary Audio 2 input
    /// </summary>
    [Key("mic2")]
    public string Mic2 { get; set; } = "";

    /// <summary>
    /// Name of the Mic/Auxiliary Audio 3 input
    /// </summary>
    [Key("mic3")]
    public string Mic3 { get; set; } = "";

    /// <summary>
    /// Name of the Mic/Auxiliary Audio 4 input
    /// </summary>
    [Key("mic4")]
    public string Mic4 { get; set; } = "";
  }

  /// <summary>
  /// Creates a new input, adding it as a scene item to the specified scene.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CreateInput : Request {
    /// <summary>
    /// Name of the scene to add the input to as a scene item
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Name of the new input to created
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// The kind of input to be created
    /// </summary>
    [Key("inputKind")]
    public string InputKind { get; set; } = "";

    /// <summary>
    /// Settings object to initialize the input with<br />
    /// If null, Default settings used
    /// </summary>
    [Key("inputSettings")]
    public Dictionary<string, object?>? InputSettings { get; set; } = new();

    /// <summary>
    /// Whether to set the created scene item to enabled or disabled<br />
    /// If null, True
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool? SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// Response of CreateInput
  /// </summary>
  public class CreateInputResponse : RequestResponse {
    /// <summary>
    /// ID of the newly created scene item
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Removes an existing input.<br />
  /// <br />
  /// Note: Will immediately remove all associated scene items.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class RemoveInput : Request {
    /// <summary>
    /// Name of the input to remove
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Sets the name of an input (rename).<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetInputName : Request {
    /// <summary>
    /// Current input name
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New name for the input
    /// </summary>
    [Key("newInputName")]
    public string NewInputName { get; set; } = "";
  }

  /// <summary>
  /// Gets the default settings for an input kind.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputDefaultSettings : Request {
    /// <summary>
    /// Input kind to get the default settings for
    /// </summary>
    [Key("inputKind")]
    public string InputKind { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputDefaultSettings
  /// </summary>
  public class GetInputDefaultSettingsResponse : RequestResponse {
    /// <summary>
    /// Object of default settings for the input kind
    /// </summary>
    [Key("defaultInputSettings")]
    public Dictionary<string, object?> DefaultInputSettings { get; set; } = new();
  }

  /// <summary>
  /// Gets the settings of an input.<br />
  /// <br />
  /// Note: Does not include defaults. To create the entire settings object, overlay <c>inputSettings</c> over the <c>defaultInputSettings</c> provided by <c>GetInputDefaultSettings</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputSettings : Request {
    /// <summary>
    /// Name of the input to get the settings of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputSettings
  /// </summary>
  public class GetInputSettingsResponse : RequestResponse {
    /// <summary>
    /// Object of settings for the input
    /// </summary>
    [Key("inputSettings")]
    public Dictionary<string, object?> InputSettings { get; set; } = new();

    /// <summary>
    /// The kind of the input
    /// </summary>
    [Key("inputKind")]
    public string InputKind { get; set; } = "";
  }

  /// <summary>
  /// Sets the settings of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetInputSettings : Request {
    /// <summary>
    /// Name of the input to set the settings of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Object of settings to apply
    /// </summary>
    [Key("inputSettings")]
    public Dictionary<string, object?> InputSettings { get; set; } = new();

    /// <summary>
    /// True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.<br />
    /// If null, true
    /// </summary>
    [Key("overlay")]
    public bool? Overlay { get; set; }
  }

  /// <summary>
  /// Gets the audio mute state of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputMute : Request {
    /// <summary>
    /// Name of input to get the mute state of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputMute
  /// </summary>
  public class GetInputMuteResponse : RequestResponse {
    /// <summary>
    /// Whether the input is muted
    /// </summary>
    [Key("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// Sets the audio mute state of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetInputMute : Request {
    /// <summary>
    /// Name of the input to set the mute state of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Whether to mute the input or not
    /// </summary>
    [Key("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// Toggles the audio mute state of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ToggleInputMute : Request {
    /// <summary>
    /// Name of the input to toggle the mute state of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of ToggleInputMute
  /// </summary>
  public class ToggleInputMuteResponse : RequestResponse {
    /// <summary>
    /// Whether the input has been muted or unmuted
    /// </summary>
    [Key("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// Gets the current volume setting of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputVolume : Request {
    /// <summary>
    /// Name of the input to get the volume of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputVolume
  /// </summary>
  public class GetInputVolumeResponse : RequestResponse {
    /// <summary>
    /// Volume setting in mul
    /// </summary>
    [Key("inputVolumeMul")]
    public double InputVolumeMul { get; set; }

    /// <summary>
    /// Volume setting in dB
    /// </summary>
    [Key("inputVolumeDb")]
    public double InputVolumeDb { get; set; }
  }

  /// <summary>
  /// Sets the volume setting of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetInputVolume : Request {
    /// <summary>
    /// Name of the input to set the volume of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Volume setting in mul<br />
    /// It should be &gt;= 0, &lt;= 20<br />
    /// If null, <c>inputVolumeDb</c> should be specified
    /// </summary>
    [Key("inputVolumeMul")]
    public double? InputVolumeMul { get; set; }

    /// <summary>
    /// Volume setting in dB<br />
    /// It should be &gt;= -100, &lt;= 26<br />
    /// If null, <c>inputVolumeMul</c> should be specified
    /// </summary>
    [Key("inputVolumeDb")]
    public double? InputVolumeDb { get; set; }
  }

  /// <summary>
  /// Gets the audio balance of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputAudioBalance : Request {
    /// <summary>
    /// Name of the input to get the audio balance of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputAudioBalance
  /// </summary>
  public class GetInputAudioBalanceResponse : RequestResponse {
    /// <summary>
    /// Audio balance value from 0.0-1.0
    /// </summary>
    [Key("inputAudioBalance")]
    public double InputAudioBalance { get; set; }
  }

  /// <summary>
  /// Sets the audio balance of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetInputAudioBalance : Request {
    /// <summary>
    /// Name of the input to set the audio balance of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New audio balance value<br />
    /// It should be &gt;= 0.0, &lt;= 1.0
    /// </summary>
    [Key("inputAudioBalance")]
    public double InputAudioBalance { get; set; }
  }

  /// <summary>
  /// Gets the audio sync offset of an input.<br />
  /// <br />
  /// Note: The audio sync offset can be negative too!<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputAudioSyncOffset : Request {
    /// <summary>
    /// Name of the input to get the audio sync offset of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputAudioSyncOffset
  /// </summary>
  public class GetInputAudioSyncOffsetResponse : RequestResponse {
    /// <summary>
    /// Audio sync offset in milliseconds
    /// </summary>
    [Key("inputAudioSyncOffset")]
    public int InputAudioSyncOffset { get; set; }
  }

  /// <summary>
  /// Sets the audio sync offset of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetInputAudioSyncOffset : Request {
    /// <summary>
    /// Name of the input to set the audio sync offset of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New audio sync offset in milliseconds<br />
    /// It should be &gt;= -950, &lt;= 20000
    /// </summary>
    [Key("inputAudioSyncOffset")]
    public int InputAudioSyncOffset { get; set; }
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
  [MessagePackObject]
  public class GetInputAudioMonitorType : Request {
    /// <summary>
    /// Name of the input to get the audio monitor type of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputAudioMonitorType
  /// </summary>
  public class GetInputAudioMonitorTypeResponse : RequestResponse {
    /// <summary>
    /// Audio monitor type
    /// </summary>
    [Key("monitorType")]
    public MonitoringType MonitorType { get; set; } = new();
  }

  /// <summary>
  /// Sets the audio monitor type of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetInputAudioMonitorType : Request {
    /// <summary>
    /// Name of the input to set the audio monitor type of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Audio monitor type
    /// </summary>
    [Key("monitorType")]
    public MonitoringType MonitorType { get; set; } = new();
  }

  /// <summary>
  /// Gets the enable state of all audio tracks of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputAudioTracks : Request {
    /// <summary>
    /// Name of the input
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputAudioTracks
  /// </summary>
  public class GetInputAudioTracksResponse : RequestResponse {
    /// <summary>
    /// Object of audio tracks and associated enable states
    /// </summary>
    [Key("inputAudioTracks")]
    public Dictionary<string, object?> InputAudioTracks { get; set; } = new();
  }

  /// <summary>
  /// Sets the enable state of audio tracks of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetInputAudioTracks : Request {
    /// <summary>
    /// Name of the input
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Track settings to apply
    /// </summary>
    [Key("inputAudioTracks")]
    public Dictionary<string, object?> InputAudioTracks { get; set; } = new();
  }

  /// <summary>
  /// Gets the items of a list property from an input's properties.<br />
  /// <br />
  /// Note: Use this in cases where an input provides a dynamic, selectable list of items. For example, display capture, where it provides a list of available displays.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetInputPropertiesListPropertyItems : Request {
    /// <summary>
    /// Name of the input
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Name of the list property to get the items of
    /// </summary>
    [Key("propertyName")]
    public string PropertyName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputPropertiesListPropertyItems
  /// </summary>
  public class GetInputPropertiesListPropertyItemsResponse : RequestResponse {
    /// <summary>
    /// Array of items in the list property
    /// </summary>
    [Key("propertyItems")]
    public List<Dictionary<string, object?>> PropertyItems { get; set; } = new();
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
  [MessagePackObject]
  public class PressInputPropertiesButton : Request {
    /// <summary>
    /// Name of the input
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Name of the button property to press
    /// </summary>
    [Key("propertyName")]
    public string PropertyName { get; set; } = "";
  }

  /// <summary>
  /// Gets an array of all available transition kinds.<br />
  /// <br />
  /// Similar to <c>GetInputKindList</c><br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetTransitionKindList : Request { }

  /// <summary>
  /// Response of GetTransitionKindList
  /// </summary>
  public class GetTransitionKindListResponse : RequestResponse {
    /// <summary>
    /// Array of transition kinds
    /// </summary>
    [Key("transitionKinds")]
    public List<string> TransitionKinds { get; set; } = new();
  }

  /// <summary>
  /// Gets an array of all scene transitions in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneTransitionList : Request { }

  /// <summary>
  /// Response of GetSceneTransitionList
  /// </summary>
  public class GetSceneTransitionListResponse : RequestResponse {
    /// <summary>
    /// Name of the current scene transition. Can be null
    /// </summary>
    [Key("currentSceneTransitionName")]
    public string? CurrentSceneTransitionName { get; set; }

    /// <summary>
    /// Kind of the current scene transition. Can be null
    /// </summary>
    [Key("currentSceneTransitionKind")]
    public string? CurrentSceneTransitionKind { get; set; }

    /// <summary>
    /// Array of transitions
    /// </summary>
    [Key("transitions")]
    public List<Dictionary<string, object?>> Transitions { get; set; } = new();
  }

  /// <summary>
  /// Gets information about the current scene transition.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetCurrentSceneTransition : Request { }

  /// <summary>
  /// Response of GetCurrentSceneTransition
  /// </summary>
  public class GetCurrentSceneTransitionResponse : RequestResponse {
    /// <summary>
    /// Name of the transition
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";

    /// <summary>
    /// Kind of the transition
    /// </summary>
    [Key("transitionKind")]
    public string TransitionKind { get; set; } = "";

    /// <summary>
    /// Whether the transition uses a fixed (unconfigurable) duration
    /// </summary>
    [Key("transitionFixed")]
    public bool TransitionFixed { get; set; }

    /// <summary>
    /// Configured transition duration in milliseconds. <c>null</c> if transition is fixed
    /// </summary>
    [Key("transitionDuration")]
    public int? TransitionDuration { get; set; }

    /// <summary>
    /// Whether the transition supports being configured
    /// </summary>
    [Key("transitionConfigurable")]
    public bool TransitionConfigurable { get; set; }

    /// <summary>
    /// Object of settings for the transition. <c>null</c> if transition is not configurable
    /// </summary>
    [Key("transitionSettings")]
    public Dictionary<string, object?>? TransitionSettings { get; set; } = new();
  }

  /// <summary>
  /// Sets the current scene transition.<br />
  /// <br />
  /// Small note: While the namespace of scene transitions is generally unique, that uniqueness is not a guarantee as it is with other resources like inputs.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetCurrentSceneTransition : Request {
    /// <summary>
    /// Name of the transition to make active
    /// </summary>
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// Sets the duration of the current scene transition, if it is not fixed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetCurrentSceneTransitionDuration : Request {
    /// <summary>
    /// Duration in milliseconds<br />
    /// It should be &gt;= 50, &lt;= 20000
    /// </summary>
    [Key("transitionDuration")]
    public int TransitionDuration { get; set; }
  }

  /// <summary>
  /// Sets the settings of the current scene transition.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetCurrentSceneTransitionSettings : Request {
    /// <summary>
    /// Settings object to apply to the transition. Can be <c>{}</c>
    /// </summary>
    [Key("transitionSettings")]
    public Dictionary<string, object?> TransitionSettings { get; set; } = new();

    /// <summary>
    /// Whether to overlay over the current settings or replace them<br />
    /// If null, true
    /// </summary>
    [Key("overlay")]
    public bool? Overlay { get; set; }
  }

  /// <summary>
  /// Gets the cursor position of the current scene transition.<br />
  /// <br />
  /// Note: <c>transitionCursor</c> will return 1.0 when the transition is inactive.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetCurrentSceneTransitionCursor : Request { }

  /// <summary>
  /// Response of GetCurrentSceneTransitionCursor
  /// </summary>
  public class GetCurrentSceneTransitionCursorResponse : RequestResponse {
    /// <summary>
    /// Cursor position, between 0.0 and 1.0
    /// </summary>
    [Key("transitionCursor")]
    public double TransitionCursor { get; set; }
  }

  /// <summary>
  /// Triggers the current scene transition. Same functionality as the <c>Transition</c> button in studio mode.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class TriggerStudioModeTransition : Request { }

  /// <summary>
  /// Sets the position of the TBar.<br />
  /// <br />
  /// **Very important note**: This will be deprecated and replaced in a future version of obs-websocket.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetTBarPosition : Request {
    /// <summary>
    /// New position<br />
    /// It should be &gt;= 0.0, &lt;= 1.0
    /// </summary>
    [Key("position")]
    public double Position { get; set; }

    /// <summary>
    /// Whether to release the TBar. Only set <c>false</c> if you know that you will be sending another position update<br />
    /// If null, <c>true</c>
    /// </summary>
    [Key("release")]
    public bool? Release { get; set; }
  }

  /// <summary>
  /// Gets an array of all of a source's filters.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSourceFilterList : Request {
    /// <summary>
    /// Name of the source
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetSourceFilterList
  /// </summary>
  public class GetSourceFilterListResponse : RequestResponse {
    /// <summary>
    /// Array of filters
    /// </summary>
    [Key("filters")]
    public List<Dictionary<string, object?>> Filters { get; set; } = new();
  }

  /// <summary>
  /// Gets the default settings for a filter kind.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSourceFilterDefaultSettings : Request {
    /// <summary>
    /// Filter kind to get the default settings for
    /// </summary>
    [Key("filterKind")]
    public string FilterKind { get; set; } = "";
  }

  /// <summary>
  /// Response of GetSourceFilterDefaultSettings
  /// </summary>
  public class GetSourceFilterDefaultSettingsResponse : RequestResponse {
    /// <summary>
    /// Object of default settings for the filter kind
    /// </summary>
    [Key("defaultFilterSettings")]
    public Dictionary<string, object?> DefaultFilterSettings { get; set; } = new();
  }

  /// <summary>
  /// Creates a new filter, adding it to the specified source.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CreateSourceFilter : Request {
    /// <summary>
    /// Name of the source to add the filter to
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the new filter to be created
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// The kind of filter to be created
    /// </summary>
    [Key("filterKind")]
    public string FilterKind { get; set; } = "";

    /// <summary>
    /// Settings object to initialize the filter with<br />
    /// If null, Default settings used
    /// </summary>
    [Key("filterSettings")]
    public Dictionary<string, object?>? FilterSettings { get; set; } = new();
  }

  /// <summary>
  /// Removes a filter from a source.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class RemoveSourceFilter : Request {
    /// <summary>
    /// Name of the source the filter is on
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the filter to remove
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";
  }

  /// <summary>
  /// Sets the name of a source filter (rename).<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSourceFilterName : Request {
    /// <summary>
    /// Name of the source the filter is on
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Current name of the filter
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// New name for the filter
    /// </summary>
    [Key("newFilterName")]
    public string NewFilterName { get; set; } = "";
  }

  /// <summary>
  /// Gets the info for a specific source filter.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSourceFilter : Request {
    /// <summary>
    /// Name of the source
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
  /// Response of GetSourceFilter
  /// </summary>
  public class GetSourceFilterResponse : RequestResponse {
    /// <summary>
    /// Whether the filter is enabled
    /// </summary>
    [Key("filterEnabled")]
    public bool FilterEnabled { get; set; }

    /// <summary>
    /// Index of the filter in the list, beginning at 0
    /// </summary>
    [Key("filterIndex")]
    public int FilterIndex { get; set; }

    /// <summary>
    /// The kind of filter
    /// </summary>
    [Key("filterKind")]
    public string FilterKind { get; set; } = "";

    /// <summary>
    /// Settings object associated with the filter
    /// </summary>
    [Key("filterSettings")]
    public Dictionary<string, object?> FilterSettings { get; set; } = new();
  }

  /// <summary>
  /// Sets the index position of a filter on a source.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSourceFilterIndex : Request {
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
    /// New index position of the filter<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("filterIndex")]
    public int FilterIndex { get; set; }
  }

  /// <summary>
  /// Sets the settings of a source filter.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSourceFilterSettings : Request {
    /// <summary>
    /// Name of the source the filter is on
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the filter to set the settings of
    /// </summary>
    [Key("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// Object of settings to apply
    /// </summary>
    [Key("filterSettings")]
    public Dictionary<string, object?> FilterSettings { get; set; } = new();

    /// <summary>
    /// True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.<br />
    /// If null, true
    /// </summary>
    [Key("overlay")]
    public bool? Overlay { get; set; }
  }

  /// <summary>
  /// Sets the enable state of a source filter.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSourceFilterEnabled : Request {
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
    /// New enable state of the filter
    /// </summary>
    [Key("filterEnabled")]
    public bool FilterEnabled { get; set; }
  }

  /// <summary>
  /// Gets a list of all scene items in a scene.<br />
  /// <br />
  /// Scenes only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneItemList : Request {
    /// <summary>
    /// Name of the scene to get the items of
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetSceneItemList
  /// </summary>
  public class GetSceneItemListResponse : RequestResponse {
    /// <summary>
    /// Array of scene items in the scene
    /// </summary>
    [Key("sceneItems")]
    public List<Dictionary<string, object?>> SceneItems { get; set; } = new();
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
  [MessagePackObject]
  public class GetGroupSceneItemList : Request {
    /// <summary>
    /// Name of the group to get the items of
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetGroupSceneItemList
  /// </summary>
  public class GetGroupSceneItemListResponse : RequestResponse {
    /// <summary>
    /// Array of scene items in the group
    /// </summary>
    [Key("sceneItems")]
    public List<Dictionary<string, object?>> SceneItems { get; set; } = new();
  }

  /// <summary>
  /// Searches a scene for a source, and returns its id.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneItemId : Request {
    /// <summary>
    /// Name of the scene or group to search in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Name of the source to find
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Number of matches to skip during search. &gt;= 0 means first forward. -1 means last (top) item<br />
    /// It should be &gt;= -1<br />
    /// If null, 0
    /// </summary>
    [Key("searchOffset")]
    public int? SearchOffset { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemId
  /// </summary>
  public class GetSceneItemIdResponse : RequestResponse {
    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Creates a new scene item using a source.<br />
  /// <br />
  /// Scenes only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class CreateSceneItem : Request {
    /// <summary>
    /// Name of the scene to create the new item in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Name of the source to add to the scene
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Enable state to apply to the scene item on creation<br />
    /// If null, True
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool? SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// Response of CreateSceneItem
  /// </summary>
  public class CreateSceneItemResponse : RequestResponse {
    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Removes a scene item from a scene.<br />
  /// <br />
  /// Scenes only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class RemoveSceneItem : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Duplicates a scene item, copying all transform and crop info.<br />
  /// <br />
  /// Scenes only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class DuplicateSceneItem : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene to create the duplicated item in<br />
    /// If null, <c>sceneName</c> is assumed
    /// </summary>
    [Key("destinationSceneName")]
    public string? DestinationSceneName { get; set; } = "";
  }

  /// <summary>
  /// Response of DuplicateSceneItem
  /// </summary>
  public class DuplicateSceneItemResponse : RequestResponse {
    /// <summary>
    /// Numeric ID of the duplicated scene item
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Gets the transform and crop info of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneItemTransform : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemTransform
  /// </summary>
  public class GetSceneItemTransformResponse : RequestResponse {
    /// <summary>
    /// Object containing scene item transform info
    /// </summary>
    [Key("sceneItemTransform")]
    public Dictionary<string, object?> SceneItemTransform { get; set; } = new();
  }

  /// <summary>
  /// Sets the transform and crop info of a scene item.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSceneItemTransform : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Object containing scene item transform info to update
    /// </summary>
    [Key("sceneItemTransform")]
    public Dictionary<string, object?> SceneItemTransform { get; set; } = new();
  }

  /// <summary>
  /// Gets the enable state of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneItemEnabled : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemEnabled
  /// </summary>
  public class GetSceneItemEnabledResponse : RequestResponse {
    /// <summary>
    /// Whether the scene item is enabled. <c>true</c> for enabled, <c>false</c> for disabled
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// Sets the enable state of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSceneItemEnabled : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New enable state of the scene item
    /// </summary>
    [Key("sceneItemEnabled")]
    public bool SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// Gets the lock state of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetSceneItemLocked : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemLocked
  /// </summary>
  public class GetSceneItemLockedResponse : RequestResponse {
    /// <summary>
    /// Whether the scene item is locked. <c>true</c> for locked, <c>false</c> for unlocked
    /// </summary>
    [Key("sceneItemLocked")]
    public bool SceneItemLocked { get; set; }
  }

  /// <summary>
  /// Sets the lock state of a scene item.<br />
  /// <br />
  /// Scenes and Group<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSceneItemLocked : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New lock state of the scene item
    /// </summary>
    [Key("sceneItemLocked")]
    public bool SceneItemLocked { get; set; }
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
  [MessagePackObject]
  public class GetSceneItemIndex : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemIndex
  /// </summary>
  public class GetSceneItemIndexResponse : RequestResponse {
    /// <summary>
    /// Index position of the scene item
    /// </summary>
    [Key("sceneItemIndex")]
    public int SceneItemIndex { get; set; }
  }

  /// <summary>
  /// Sets the index position of a scene item in a scene.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSceneItemIndex : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New index position of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemIndex")]
    public int SceneItemIndex { get; set; }
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
  [MessagePackObject]
  public class GetSceneItemBlendMode : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemBlendMode
  /// </summary>
  public class GetSceneItemBlendModeResponse : RequestResponse {
    /// <summary>
    /// Current blend mode
    /// </summary>
    [Key("sceneItemBlendMode")]
    public string SceneItemBlendMode { get; set; } = "";
  }

  /// <summary>
  /// Sets the blend mode of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetSceneItemBlendMode : Request {
    /// <summary>
    /// Name of the scene the item is in
    /// </summary>
    [Key("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New blend mode
    /// </summary>
    [Key("sceneItemBlendMode")]
    public string SceneItemBlendMode { get; set; } = "";
  }

  /// <summary>
  /// Gets the status of the virtualcam output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetVirtualCamStatus : Request { }

  /// <summary>
  /// Response of GetVirtualCamStatus
  /// </summary>
  public class GetVirtualCamStatusResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Toggles the state of the virtualcam output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ToggleVirtualCam : Request { }

  /// <summary>
  /// Response of ToggleVirtualCam
  /// </summary>
  public class ToggleVirtualCamResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the virtualcam output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StartVirtualCam : Request { }

  /// <summary>
  /// Stops the virtualcam output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StopVirtualCam : Request { }

  /// <summary>
  /// Gets the status of the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetReplayBufferStatus : Request { }

  /// <summary>
  /// Response of GetReplayBufferStatus
  /// </summary>
  public class GetReplayBufferStatusResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Toggles the state of the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ToggleReplayBuffer : Request { }

  /// <summary>
  /// Response of ToggleReplayBuffer
  /// </summary>
  public class ToggleReplayBufferResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StartReplayBuffer : Request { }

  /// <summary>
  /// Stops the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StopReplayBuffer : Request { }

  /// <summary>
  /// Saves the contents of the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SaveReplayBuffer : Request { }

  /// <summary>
  /// Gets the filename of the last replay buffer save file.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetLastReplayBufferReplay : Request { }

  /// <summary>
  /// Response of GetLastReplayBufferReplay
  /// </summary>
  public class GetLastReplayBufferReplayResponse : RequestResponse {
    /// <summary>
    /// File path
    /// </summary>
    [Key("savedReplayPath")]
    public string SavedReplayPath { get; set; } = "";
  }

  /// <summary>
  /// Gets the list of available outputs.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetOutputList : Request { }

  /// <summary>
  /// Response of GetOutputList
  /// </summary>
  public class GetOutputListResponse : RequestResponse {
    /// <summary>
    /// List of outputs
    /// </summary>
    [Key("outputs")]
    public List<Output> Outputs { get; set; } = new();
  }

  /// <summary>
  /// Gets the status of an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetOutputStatus : Request {
    /// <summary>
    /// Output name
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetOutputStatus
  /// </summary>
  public class GetOutputStatusResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// Whether the output is reconnecting
    /// </summary>
    [Key("outputReconnecting")]
    public bool OutputReconnecting { get; set; }

    /// <summary>
    /// Current formatted timecode string for the output
    /// </summary>
    [Key("outputTimecode")]
    public string OutputTimecode { get; set; } = "";

    /// <summary>
    /// Current duration in milliseconds for the output
    /// </summary>
    [Key("outputDuration")]
    public int OutputDuration { get; set; }

    /// <summary>
    /// Congestion of the output
    /// </summary>
    [Key("outputCongestion")]
    public double OutputCongestion { get; set; }

    /// <summary>
    /// Number of bytes sent by the output
    /// </summary>
    [Key("outputBytes")]
    public int OutputBytes { get; set; }

    /// <summary>
    /// Number of frames skipped by the output's process
    /// </summary>
    [Key("outputSkippedFrames")]
    public int OutputSkippedFrames { get; set; }

    /// <summary>
    /// Total number of frames delivered by the output's process
    /// </summary>
    [Key("outputTotalFrames")]
    public int OutputTotalFrames { get; set; }
  }

  /// <summary>
  /// Toggles the status of an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ToggleOutput : Request {
    /// <summary>
    /// Output name
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Response of ToggleOutput
  /// </summary>
  public class ToggleOutputResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StartOutput : Request {
    /// <summary>
    /// Output name
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Stops an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StopOutput : Request {
    /// <summary>
    /// Output name
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Gets the settings of an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetOutputSettings : Request {
    /// <summary>
    /// Output name
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetOutputSettings
  /// </summary>
  public class GetOutputSettingsResponse : RequestResponse {
    /// <summary>
    /// Output settings
    /// </summary>
    [Key("outputSettings")]
    public Dictionary<string, object?> OutputSettings { get; set; } = new();
  }

  /// <summary>
  /// Sets the settings of an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetOutputSettings : Request {
    /// <summary>
    /// Output name
    /// </summary>
    [Key("outputName")]
    public string OutputName { get; set; } = "";

    /// <summary>
    /// Output settings
    /// </summary>
    [Key("outputSettings")]
    public Dictionary<string, object?> OutputSettings { get; set; } = new();
  }

  /// <summary>
  /// Gets the status of the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetStreamStatus : Request { }

  /// <summary>
  /// Response of GetStreamStatus
  /// </summary>
  public class GetStreamStatusResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// Whether the output is currently reconnecting
    /// </summary>
    [Key("outputReconnecting")]
    public bool OutputReconnecting { get; set; }

    /// <summary>
    /// Current formatted timecode string for the output
    /// </summary>
    [Key("outputTimecode")]
    public string OutputTimecode { get; set; } = "";

    /// <summary>
    /// Current duration in milliseconds for the output
    /// </summary>
    [Key("outputDuration")]
    public int OutputDuration { get; set; }

    /// <summary>
    /// Congestion of the output
    /// </summary>
    [Key("outputCongestion")]
    public double OutputCongestion { get; set; }

    /// <summary>
    /// Number of bytes sent by the output
    /// </summary>
    [Key("outputBytes")]
    public int OutputBytes { get; set; }

    /// <summary>
    /// Number of frames skipped by the output's process
    /// </summary>
    [Key("outputSkippedFrames")]
    public int OutputSkippedFrames { get; set; }

    /// <summary>
    /// Total number of frames delivered by the output's process
    /// </summary>
    [Key("outputTotalFrames")]
    public int OutputTotalFrames { get; set; }
  }

  /// <summary>
  /// Toggles the status of the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ToggleStream : Request { }

  /// <summary>
  /// Response of ToggleStream
  /// </summary>
  public class ToggleStreamResponse : RequestResponse {
    /// <summary>
    /// New state of the stream output
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StartStream : Request { }

  /// <summary>
  /// Stops the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StopStream : Request { }

  /// <summary>
  /// Sends CEA-608 caption text over the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SendStreamCaption : Request {
    /// <summary>
    /// Caption text
    /// </summary>
    [Key("captionText")]
    public string CaptionText { get; set; } = "";
  }

  /// <summary>
  /// Gets the status of the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetRecordStatus : Request { }

  /// <summary>
  /// Response of GetRecordStatus
  /// </summary>
  public class GetRecordStatusResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// Whether the output is paused
    /// </summary>
    [Key("ouputPaused")]
    public bool OuputPaused { get; set; }

    /// <summary>
    /// Current formatted timecode string for the output
    /// </summary>
    [Key("outputTimecode")]
    public string OutputTimecode { get; set; } = "";

    /// <summary>
    /// Current duration in milliseconds for the output
    /// </summary>
    [Key("outputDuration")]
    public int OutputDuration { get; set; }

    /// <summary>
    /// Number of bytes sent by the output
    /// </summary>
    [Key("outputBytes")]
    public int OutputBytes { get; set; }
  }

  /// <summary>
  /// Toggles the status of the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ToggleRecord : Request { }

  /// <summary>
  /// Response of ToggleRecord
  /// </summary>
  public class ToggleRecordResponse : RequestResponse {
    /// <summary>
    /// Whether the output is active
    /// </summary>
    [Key("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StartRecord : Request { }

  /// <summary>
  /// Stops the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class StopRecord : Request { }

  /// <summary>
  /// Response of StopRecord
  /// </summary>
  public class StopRecordResponse : RequestResponse {
    /// <summary>
    /// File name for the saved recording
    /// </summary>
    [Key("outputPath")]
    public string OutputPath { get; set; } = "";
  }

  /// <summary>
  /// Toggles pause on the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ToggleRecordPause : Request { }

  /// <summary>
  /// Response of ToggleRecordPause
  /// </summary>
  public class ToggleRecordPauseResponse : RequestResponse {
    /// <summary>
    /// Whether the output is paused
    /// </summary>
    [Key("outputPaused")]
    public bool OutputPaused { get; set; }
  }

  /// <summary>
  /// Pauses the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class PauseRecord : Request { }

  /// <summary>
  /// Resumes the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class ResumeRecord : Request { }

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
  [MessagePackObject]
  public class GetMediaInputStatus : Request {
    /// <summary>
    /// Name of the media input
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetMediaInputStatus
  /// </summary>
  public class GetMediaInputStatusResponse : RequestResponse {
    /// <summary>
    /// State of the media input
    /// </summary>
    [Key("mediaState")]
    public string MediaState { get; set; } = "";

    /// <summary>
    /// Total duration of the playing media in milliseconds. <c>null</c> if not playing
    /// </summary>
    [Key("mediaDuration")]
    public int? MediaDuration { get; set; }

    /// <summary>
    /// Position of the cursor in milliseconds. <c>null</c> if not playing
    /// </summary>
    [Key("mediaCursor")]
    public int? MediaCursor { get; set; }
  }

  /// <summary>
  /// Sets the cursor position of a media input.<br />
  /// <br />
  /// This request does not perform bounds checking of the cursor position.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetMediaInputCursor : Request {
    /// <summary>
    /// Name of the media input
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// New cursor position to set<br />
    /// It should be &gt;= 0
    /// </summary>
    [Key("mediaCursor")]
    public double MediaCursor { get; set; }
  }

  /// <summary>
  /// Offsets the current cursor position of a media input by the specified value.<br />
  /// <br />
  /// This request does not perform bounds checking of the cursor position.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class OffsetMediaInputCursor : Request {
    /// <summary>
    /// Name of the media input
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Value to offset the current cursor position by
    /// </summary>
    [Key("mediaCursorOffset")]
    public int MediaCursorOffset { get; set; }
  }

  /// <summary>
  /// Triggers an action on a media input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class TriggerMediaInputAction : Request {
    /// <summary>
    /// Name of the media input
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// Identifier of the <c>ObsMediaInputAction</c> enum
    /// </summary>
    [Key("mediaAction")]
    public string MediaAction { get; set; } = "";
  }

  /// <summary>
  /// Gets whether studio is enabled.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetStudioModeEnabled : Request { }

  /// <summary>
  /// Response of GetStudioModeEnabled
  /// </summary>
  public class GetStudioModeEnabledResponse : RequestResponse {
    /// <summary>
    /// Whether studio mode is enabled
    /// </summary>
    [Key("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }

  /// <summary>
  /// Enables or disables studio mode<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class SetStudioModeEnabled : Request {
    /// <summary>
    /// True == Enabled, False == Disabled
    /// </summary>
    [Key("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }

  /// <summary>
  /// Opens the properties dialog of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class OpenInputPropertiesDialog : Request {
    /// <summary>
    /// Name of the input to open the dialog of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Opens the filters dialog of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class OpenInputFiltersDialog : Request {
    /// <summary>
    /// Name of the input to open the dialog of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Opens the interact dialog of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class OpenInputInteractDialog : Request {
    /// <summary>
    /// Name of the input to open the dialog of
    /// </summary>
    [Key("inputName")]
    public string InputName { get; set; } = "";
  }

  /// <summary>
  /// Gets a list of connected monitors and information about them.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class GetMonitorList : Request { }

  /// <summary>
  /// Response of GetMonitorList
  /// </summary>
  public class GetMonitorListResponse : RequestResponse {
    /// <summary>
    /// a list of detected monitors with some information
    /// </summary>
    [Key("monitors")]
    public List<Dictionary<string, object?>> Monitors { get; set; } = new();
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
  [MessagePackObject]
  public class OpenVideoMixProjector : Request {
    /// <summary>
    /// Type of mix to open
    /// </summary>
    [Key("videoMixType")]
    public VideoMixType VideoMixType { get; set; } = new();

    /// <summary>
    /// Monitor index, use <c>GetMonitorList</c> to obtain index<br />
    /// If null, -1: Opens projector in windowed mode
    /// </summary>
    [Key("monitorIndex")]
    public int? MonitorIndex { get; set; }

    /// <summary>
    /// Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with <c>monitorIndex</c><br />
    /// If null, N/A
    /// </summary>
    [Key("projectorGeometry")]
    public string? ProjectorGeometry { get; set; } = "";
  }

  /// <summary>
  /// Opens a projector for a source.<br />
  /// <br />
  /// Note: This request serves to provide feature parity with 4.x. It is very likely to be changed/deprecated in a future release.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  [MessagePackObject]
  public class OpenSourceProjector : Request {
    /// <summary>
    /// Name of the source to open a projector for
    /// </summary>
    [Key("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Monitor index, use <c>GetMonitorList</c> to obtain index<br />
    /// If null, -1: Opens projector in windowed mode
    /// </summary>
    [Key("monitorIndex")]
    public int? MonitorIndex { get; set; }

    /// <summary>
    /// Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with <c>monitorIndex</c><br />
    /// If null, N/A
    /// </summary>
    [Key("projectorGeometry")]
    public string? ProjectorGeometry { get; set; } = "";
  }
}
