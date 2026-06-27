using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObsStrawket.DataTypes.Predefineds {

  /// <summary>
  /// Gets an array of canvases in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.7.0
  /// </summary>
  public class GetCanvasList : Request<GetCanvasListResponse> { }

  /// <summary>
  /// Response of GetCanvasList
  /// </summary>
  public class GetCanvasListResponse : RequestResponse {

    /// <summary>
    /// Array of canvases
    /// </summary>
    [JsonPropertyName("canvases")]
    public List<Dictionary<string, JsonElement?>> Canvases { get; set; } = [];
  }

  /// <summary>
  /// Gets data about the current plugin and RPC version.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetVersion : Request<GetVersionResponse> { }

  /// <summary>
  /// Response of GetVersion
  /// </summary>
  public class GetVersionResponse : RequestResponse {

    /// <summary>
    /// Current OBS Studio version
    /// </summary>
    [JsonPropertyName("obsVersion")]
    public string ObsVersion { get; set; } = "";

    /// <summary>
    /// Current obs-websocket version
    /// </summary>
    [JsonPropertyName("obsWebSocketVersion")]
    public string ObsWebSocketVersion { get; set; } = "";

    /// <summary>
    /// Current latest obs-websocket RPC version
    /// </summary>
    [JsonPropertyName("rpcVersion")]
    public int RpcVersion { get; set; }

    /// <summary>
    /// Array of available RPC requests for the currently negotiated RPC version
    /// </summary>
    [JsonPropertyName("availableRequests")]
    public List<string> AvailableRequests { get; set; } = [];

    /// <summary>
    /// Image formats available in <c>GetSourceScreenshot</c> and <c>SaveSourceScreenshot</c> requests.
    /// </summary>
    [JsonPropertyName("supportedImageFormats")]
    public List<string> SupportedImageFormats { get; set; } = [];

    /// <summary>
    /// Name of the platform. Usually <c>windows</c>, <c>macos</c>, or <c>ubuntu</c> (linux flavor). Not guaranteed to be any of those
    /// </summary>
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = "";

    /// <summary>
    /// Description of the platform, like <c>Windows 10 (10.0)</c>
    /// </summary>
    [JsonPropertyName("platformDescription")]
    public string PlatformDescription { get; set; } = "";
  }

  /// <summary>
  /// Gets statistics about OBS, obs-websocket, and the current session.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetStats : Request<GetStatsResponse> { }

  /// <summary>
  /// Response of GetStats
  /// </summary>
  public class GetStatsResponse : RequestResponse {

    /// <summary>
    /// Current CPU usage in percent
    /// </summary>
    [JsonPropertyName("cpuUsage")]
    public double CpuUsage { get; set; }

    /// <summary>
    /// Amount of memory in MB currently being used by OBS
    /// </summary>
    [JsonPropertyName("memoryUsage")]
    public double MemoryUsage { get; set; }

    /// <summary>
    /// Available disk space on the device being used for recording storage
    /// </summary>
    [JsonPropertyName("availableDiskSpace")]
    public double AvailableDiskSpace { get; set; }

    /// <summary>
    /// Current FPS being rendered
    /// </summary>
    [JsonPropertyName("activeFps")]
    public double ActiveFps { get; set; }

    /// <summary>
    /// Average time in milliseconds that OBS is taking to render a frame
    /// </summary>
    [JsonPropertyName("averageFrameRenderTime")]
    public double AverageFrameRenderTime { get; set; }

    /// <summary>
    /// Number of frames skipped by OBS in the render thread
    /// </summary>
    [JsonPropertyName("renderSkippedFrames")]
    public long RenderSkippedFrames { get; set; }

    /// <summary>
    /// Total number of frames outputted by the render thread
    /// </summary>
    [JsonPropertyName("renderTotalFrames")]
    public long RenderTotalFrames { get; set; }

    /// <summary>
    /// Number of frames skipped by OBS in the output thread
    /// </summary>
    [JsonPropertyName("outputSkippedFrames")]
    public long OutputSkippedFrames { get; set; }

    /// <summary>
    /// Total number of frames outputted by the output thread
    /// </summary>
    [JsonPropertyName("outputTotalFrames")]
    public long OutputTotalFrames { get; set; }

    /// <summary>
    /// Total number of messages received by obs-websocket from the client
    /// </summary>
    [JsonPropertyName("webSocketSessionIncomingMessages")]
    public long WebSocketSessionIncomingMessages { get; set; }

    /// <summary>
    /// Total number of messages sent by obs-websocket to the client
    /// </summary>
    [JsonPropertyName("webSocketSessionOutgoingMessages")]
    public long WebSocketSessionOutgoingMessages { get; set; }
  }

  /// <summary>
  /// Broadcasts a <c>CustomEvent</c> to all WebSocket clients. Receivers are clients which are identified and subscribed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class BroadcastCustomEvent : Request<RequestResponse> {

    /// <summary>
    /// Data payload to emit to all receivers
    /// </summary>
    [JsonPropertyName("eventData")]
    public Dictionary<string, JsonElement?> EventData { get; set; } = [];
  }

  /// <summary>
  /// Call a request registered to a vendor.<br />
  /// <br />
  /// A vendor is a unique name registered by a third-party plugin or script, which allows for custom requests and events to be added to obs-websocket.<br />
  /// If a plugin or script implements vendor requests or events, documentation is expected to be provided with them.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class CallVendorRequest : Request<CallVendorRequestResponse> {

    /// <summary>
    /// Name of the vendor to use
    /// </summary>
    [JsonPropertyName("vendorName")]
    public string VendorName { get; set; } = "";

    /// <summary>
    /// The request type to call
    /// </summary>
    [JsonPropertyName("requestType")]
    public string VendorRequestType { get; set; } = "";

    /// <summary>
    /// Object containing appropriate request data<br />
    /// If null, {}
    /// </summary>
    [JsonPropertyName("requestData")]
    public Dictionary<string, JsonElement?>? RequestData { get; set; }
  }

  /// <summary>
  /// Response of CallVendorRequest
  /// </summary>
  public class CallVendorRequestResponse : RequestResponse {

    /// <summary>
    /// Echoed of <c>vendorName</c>
    /// </summary>
    [JsonPropertyName("vendorName")]
    public string VendorName { get; set; } = "";

    /// <summary>
    /// Echoed of <c>requestType</c>
    /// </summary>
    [JsonPropertyName("requestType")]
    public string VendorRequestType { get; set; } = "";

    /// <summary>
    /// Object containing appropriate response data. {} if request does not provide any response data
    /// </summary>
    [JsonPropertyName("responseData")]
    public Dictionary<string, JsonElement?> ResponseData { get; set; } = [];
  }

  /// <summary>
  /// Gets an array of all hotkey names in OBS.<br />
  /// <br />
  /// Note: Hotkey functionality in obs-websocket comes as-is, and we do not guarantee support if things are broken. In 9/10 usages of hotkey requests, there exists a better, more reliable method via other requests.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetHotkeyList : Request<GetHotkeyListResponse> { }

  /// <summary>
  /// Response of GetHotkeyList
  /// </summary>
  public class GetHotkeyListResponse : RequestResponse {

    /// <summary>
    /// Array of hotkey names
    /// </summary>
    [JsonPropertyName("hotkeys")]
    public List<string> Hotkeys { get; set; } = [];
  }

  /// <summary>
  /// Triggers a hotkey using its name. See <c>GetHotkeyList</c>.<br />
  /// <br />
  /// Note: Hotkey functionality in obs-websocket comes as-is, and we do not guarantee support if things are broken. In 9/10 usages of hotkey requests, there exists a better, more reliable method via other requests.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class TriggerHotkeyByName : Request<RequestResponse> {

    /// <summary>
    /// Name of the hotkey to trigger
    /// </summary>
    [JsonPropertyName("hotkeyName")]
    public string HotkeyName { get; set; } = "";

    /// <summary>
    /// Name of context of the hotkey to trigger<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("contextName")]
    public string? ContextName { get; set; }
  }

  /// <summary>
  /// Triggers a hotkey using a sequence of keys.<br />
  /// <br />
  /// Note: Hotkey functionality in obs-websocket comes as-is, and we do not guarantee support if things are broken. In 9/10 usages of hotkey requests, there exists a better, more reliable method via other requests.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class TriggerHotkeyByKeySequence : Request<RequestResponse> {

    /// <summary>
    /// The OBS key ID to use. See https://github.com/obsproject/obs-studio/blob/master/libobs/obs-hotkeys.h<br />
    /// If null, Not pressed
    /// </summary>
    [JsonPropertyName("keyId")]
    public string? KeyId { get; set; }

    /// <summary>
    /// Object containing key modifiers to apply<br />
    /// If null, Ignored
    /// </summary>
    [JsonPropertyName("keyModifiers")]
    public KeyModifiers? KeyModifiers { get; set; }
  }

  /// <summary>
  /// Sleeps for a time duration or number of frames. Only available in request batches with types <c>SERIAL_REALTIME</c> or <c>SERIAL_FRAME</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class Sleep : Request<RequestResponse> {

    /// <summary>
    /// Number of milliseconds to sleep for (if <c>SERIAL_REALTIME</c> mode)<br />
    /// It should be &gt;= 0, &lt;= 50000<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sleepMillis")]
    public int? SleepMillis { get; set; }

    /// <summary>
    /// Number of frames to sleep for (if <c>SERIAL_FRAME</c> mode)<br />
    /// It should be &gt;= 0, &lt;= 10000<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sleepFrames")]
    public long? SleepFrames { get; set; }
  }

  /// <summary>
  /// Gets the value of a "slot" from the selected persistent data realm.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetPersistentData : Request<GetPersistentDataResponse> {

    /// <summary>
    /// The data realm to select. <c>OBS_WEBSOCKET_DATA_REALM_GLOBAL</c> or <c>OBS_WEBSOCKET_DATA_REALM_PROFILE</c>
    /// </summary>
    [JsonPropertyName("realm")]
    public DataRealm Realm { get; set; } = new();

    /// <summary>
    /// The name of the slot to retrieve data from
    /// </summary>
    [JsonPropertyName("slotName")]
    public string SlotName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetPersistentData
  /// </summary>
  public class GetPersistentDataResponse : RequestResponse {

    /// <summary>
    /// Value associated with the slot. <c>null</c> if not set
    /// </summary>
    [JsonPropertyName("slotValue")]
    public object? SlotValue { get; set; }
  }

  /// <summary>
  /// Sets the value of a "slot" from the selected persistent data realm.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetPersistentData : Request<RequestResponse> {

    /// <summary>
    /// The data realm to select. <c>OBS_WEBSOCKET_DATA_REALM_GLOBAL</c> or <c>OBS_WEBSOCKET_DATA_REALM_PROFILE</c>
    /// </summary>
    [JsonPropertyName("realm")]
    public DataRealm Realm { get; set; } = new();

    /// <summary>
    /// The name of the slot to retrieve data from
    /// </summary>
    [JsonPropertyName("slotName")]
    public string SlotName { get; set; } = "";

    /// <summary>
    /// The value to apply to the slot
    /// </summary>
    [JsonPropertyName("slotValue")]
    public object? SlotValue { get; set; }
  }

  /// <summary>
  /// Gets an array of all scene collections<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneCollectionList : Request<GetSceneCollectionListResponse> { }

  /// <summary>
  /// Response of GetSceneCollectionList
  /// </summary>
  public class GetSceneCollectionListResponse : RequestResponse {

    /// <summary>
    /// The name of the current scene collection
    /// </summary>
    [JsonPropertyName("currentSceneCollectionName")]
    public string CurrentSceneCollectionName { get; set; } = "";

    /// <summary>
    /// Array of all available scene collections
    /// </summary>
    [JsonPropertyName("sceneCollections")]
    public List<string> SceneCollections { get; set; } = [];
  }

  /// <summary>
  /// Switches to a scene collection.<br />
  /// <br />
  /// Note: This will block until the collection has finished changing.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
#pragma warning disable CA1711
  public class SetCurrentSceneCollection : Request<RequestResponse> {

    /// <summary>
    /// Name of the scene collection to switch to
    /// </summary>
    [JsonPropertyName("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }
#pragma warning restore CA1711

  /// <summary>
  /// Creates a new scene collection, switching to it in the process.<br />
  /// <br />
  /// Note: This will block until the collection has finished changing.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
#pragma warning disable CA1711
  public class CreateSceneCollection : Request<RequestResponse> {

    /// <summary>
    /// Name for the new scene collection
    /// </summary>
    [JsonPropertyName("sceneCollectionName")]
    public string SceneCollectionName { get; set; } = "";
  }
#pragma warning restore CA1711

  /// <summary>
  /// Gets an array of all profiles<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetProfileList : Request<GetProfileListResponse> { }

  /// <summary>
  /// Response of GetProfileList
  /// </summary>
  public class GetProfileListResponse : RequestResponse {

    /// <summary>
    /// The name of the current profile
    /// </summary>
    [JsonPropertyName("currentProfileName")]
    public string CurrentProfileName { get; set; } = "";

    /// <summary>
    /// Array of all available profiles
    /// </summary>
    [JsonPropertyName("profiles")]
    public List<string> Profiles { get; set; } = [];
  }

  /// <summary>
  /// Switches to a profile.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetCurrentProfile : Request<RequestResponse> {

    /// <summary>
    /// Name of the profile to switch to
    /// </summary>
    [JsonPropertyName("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Creates a new profile, switching to it in the process<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class CreateProfile : Request<RequestResponse> {

    /// <summary>
    /// Name for the new profile
    /// </summary>
    [JsonPropertyName("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Removes a profile. If the current profile is chosen, it will change to a different profile first.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class RemoveProfile : Request<RequestResponse> {

    /// <summary>
    /// Name of the profile to remove
    /// </summary>
    [JsonPropertyName("profileName")]
    public string ProfileName { get; set; } = "";
  }

  /// <summary>
  /// Gets a parameter from the current profile's configuration.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetProfileParameter : Request<GetProfileParameterResponse> {

    /// <summary>
    /// Category of the parameter to get
    /// </summary>
    [JsonPropertyName("parameterCategory")]
    public string ParameterCategory { get; set; } = "";

    /// <summary>
    /// Name of the parameter to get
    /// </summary>
    [JsonPropertyName("parameterName")]
    public string ParameterName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetProfileParameter
  /// </summary>
  public class GetProfileParameterResponse : RequestResponse {

    /// <summary>
    /// Value associated with the parameter. <c>null</c> if not set and no default
    /// </summary>
    [JsonPropertyName("parameterValue")]
    public string? ParameterValue { get; set; }

    /// <summary>
    /// Default value associated with the parameter. <c>null</c> if no default
    /// </summary>
    [JsonPropertyName("defaultParameterValue")]
    public string? DefaultParameterValue { get; set; }
  }

  /// <summary>
  /// Sets the value of a parameter in the current profile's configuration.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetProfileParameter : Request<RequestResponse> {

    /// <summary>
    /// Category of the parameter to set
    /// </summary>
    [JsonPropertyName("parameterCategory")]
    public string ParameterCategory { get; set; } = "";

    /// <summary>
    /// Name of the parameter to set
    /// </summary>
    [JsonPropertyName("parameterName")]
    public string ParameterName { get; set; } = "";

    /// <summary>
    /// Value of the parameter to set. Use <c>null</c> to delete
    /// </summary>
    [JsonPropertyName("parameterValue")]
    public string? ParameterValue { get; set; }
  }

  /// <summary>
  /// Gets the current video settings.<br />
  /// <br />
  /// Note: To get the true FPS value, divide the FPS numerator by the FPS denominator. Example: <c>60000/1001</c><br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetVideoSettings : Request<GetVideoSettingsResponse> { }

  /// <summary>
  /// Response of GetVideoSettings
  /// </summary>
  public class GetVideoSettingsResponse : RequestResponse {

    /// <summary>
    /// Numerator of the fractional FPS value
    /// </summary>
    [JsonPropertyName("fpsNumerator")]
    public int FpsNumerator { get; set; }

    /// <summary>
    /// Denominator of the fractional FPS value
    /// </summary>
    [JsonPropertyName("fpsDenominator")]
    public int FpsDenominator { get; set; }

    /// <summary>
    /// Width of the base (canvas) resolution in pixels
    /// </summary>
    [JsonPropertyName("baseWidth")]
    public int BaseWidth { get; set; }

    /// <summary>
    /// Height of the base (canvas) resolution in pixels
    /// </summary>
    [JsonPropertyName("baseHeight")]
    public int BaseHeight { get; set; }

    /// <summary>
    /// Width of the output resolution in pixels
    /// </summary>
    [JsonPropertyName("outputWidth")]
    public int OutputWidth { get; set; }

    /// <summary>
    /// Height of the output resolution in pixels
    /// </summary>
    [JsonPropertyName("outputHeight")]
    public int OutputHeight { get; set; }
  }

  /// <summary>
  /// Sets the current video settings.<br />
  /// <br />
  /// Note: Fields must be specified in pairs. For example, you cannot set only <c>baseWidth</c> without needing to specify <c>baseHeight</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetVideoSettings : Request<RequestResponse> {

    /// <summary>
    /// Numerator of the fractional FPS value<br />
    /// It should be &gt;= 1<br />
    /// If null, Not changed
    /// </summary>
    [JsonPropertyName("fpsNumerator")]
    public int? FpsNumerator { get; set; }

    /// <summary>
    /// Denominator of the fractional FPS value<br />
    /// It should be &gt;= 1<br />
    /// If null, Not changed
    /// </summary>
    [JsonPropertyName("fpsDenominator")]
    public int? FpsDenominator { get; set; }

    /// <summary>
    /// Width of the base (canvas) resolution in pixels<br />
    /// It should be &gt;= 1, &lt;= 4096<br />
    /// If null, Not changed
    /// </summary>
    [JsonPropertyName("baseWidth")]
    public int? BaseWidth { get; set; }

    /// <summary>
    /// Height of the base (canvas) resolution in pixels<br />
    /// It should be &gt;= 1, &lt;= 4096<br />
    /// If null, Not changed
    /// </summary>
    [JsonPropertyName("baseHeight")]
    public int? BaseHeight { get; set; }

    /// <summary>
    /// Width of the output resolution in pixels<br />
    /// It should be &gt;= 1, &lt;= 4096<br />
    /// If null, Not changed
    /// </summary>
    [JsonPropertyName("outputWidth")]
    public int? OutputWidth { get; set; }

    /// <summary>
    /// Height of the output resolution in pixels<br />
    /// It should be &gt;= 1, &lt;= 4096<br />
    /// If null, Not changed
    /// </summary>
    [JsonPropertyName("outputHeight")]
    public int? OutputHeight { get; set; }
  }

  /// <summary>
  /// Gets the current stream service settings (stream destination).<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetStreamServiceSettings : Request<GetStreamServiceSettingsResponse> { }

  /// <summary>
  /// Response of GetStreamServiceSettings
  /// </summary>
  public class GetStreamServiceSettingsResponse : RequestResponse {

    /// <summary>
    /// Stream service type, like <c>rtmp_custom</c> or <c>rtmp_common</c>
    /// </summary>
    [JsonPropertyName("streamServiceType")]
    public StreamServiceType StreamServiceType { get; set; } = new();

    /// <summary>
    /// Stream service settings
    /// </summary>
    [JsonPropertyName("streamServiceSettings")]
    public Dictionary<string, JsonElement?> StreamServiceSettings { get; set; } = [];
  }

  /// <summary>
  /// Sets the current stream service settings (stream destination).<br />
  /// <br />
  /// Note: Simple RTMP settings can be set with type <c>rtmp_custom</c> and the settings fields <c>server</c> and <c>key</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetStreamServiceSettings : Request<RequestResponse> {

    /// <summary>
    /// Type of stream service to apply. Example: <c>rtmp_common</c> or <c>rtmp_custom</c>
    /// </summary>
    [JsonPropertyName("streamServiceType")]
    public StreamServiceType StreamServiceType { get; set; } = new();

    /// <summary>
    /// Settings to apply to the service
    /// </summary>
    [JsonPropertyName("streamServiceSettings")]
    public Dictionary<string, JsonElement?> StreamServiceSettings { get; set; } = [];
  }

  /// <summary>
  /// Gets the current directory that the record output is set to.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetRecordDirectory : Request<GetRecordDirectoryResponse> { }

  /// <summary>
  /// Response of GetRecordDirectory
  /// </summary>
  public class GetRecordDirectoryResponse : RequestResponse {

    /// <summary>
    /// Output directory
    /// </summary>
    [JsonPropertyName("recordDirectory")]
    public string RecordDirectory { get; set; } = "";
  }

  /// <summary>
  /// Sets the current directory that the record output writes files to.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.3.0
  /// </summary>
  public class SetRecordDirectory : Request<RequestResponse> {

    /// <summary>
    /// Output directory
    /// </summary>
    [JsonPropertyName("recordDirectory")]
    public string RecordDirectory { get; set; } = "";
  }

  /// <summary>
  /// Gets the active and show state of a source.<br />
  /// <br />
  /// **Compatible with inputs and scenes.**<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSourceActive : Request<GetSourceActiveResponse> {

    /// <summary>
    /// Name of the source to get the active state of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source to get the active state of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSourceActive
  /// </summary>
  public class GetSourceActiveResponse : RequestResponse {

    /// <summary>
    /// Whether the source is showing in Program
    /// </summary>
    [JsonPropertyName("videoActive")]
    public bool VideoActive { get; set; }

    /// <summary>
    /// Whether the source is showing in the UI (Preview, Projector, Properties)
    /// </summary>
    [JsonPropertyName("videoShowing")]
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
  public class GetSourceScreenshot : Request<GetSourceScreenshotResponse> {

    /// <summary>
    /// Image compression format to use. Use <c>GetVersion</c> to get compatible image formats
    /// </summary>
    [JsonPropertyName("imageFormat")]
    public string ImageFormat { get; set; } = "";

    /// <summary>
    /// Name of the source to take a screenshot of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source to take a screenshot of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// Width to scale the screenshot to<br />
    /// It should be &gt;= 8, &lt;= 4096<br />
    /// If null, Source value is used
    /// </summary>
    [JsonPropertyName("imageWidth")]
    public int? ImageWidth { get; set; }

    /// <summary>
    /// Height to scale the screenshot to<br />
    /// It should be &gt;= 8, &lt;= 4096<br />
    /// If null, Source value is used
    /// </summary>
    [JsonPropertyName("imageHeight")]
    public int? ImageHeight { get; set; }

    /// <summary>
    /// Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk)<br />
    /// It should be &gt;= -1, &lt;= 100<br />
    /// If null, -1
    /// </summary>
    [JsonPropertyName("imageCompressionQuality")]
    public int? ImageCompressionQuality { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSourceScreenshot
  /// </summary>
  public class GetSourceScreenshotResponse : RequestResponse {

    /// <summary>
    /// Base64-encoded screenshot
    /// </summary>
    [JsonPropertyName("imageData")]
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
  public class SaveSourceScreenshot : Request<RequestResponse> {

    /// <summary>
    /// Image compression format to use. Use <c>GetVersion</c> to get compatible image formats
    /// </summary>
    [JsonPropertyName("imageFormat")]
    public string ImageFormat { get; set; } = "";

    /// <summary>
    /// Path to save the screenshot file to. Eg. <c>C:\Users\user\Desktop\screenshot.png</c>
    /// </summary>
    [JsonPropertyName("imageFilePath")]
    public string ImageFilePath { get; set; } = "";

    /// <summary>
    /// Name of the source to take a screenshot of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source to take a screenshot of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// Width to scale the screenshot to<br />
    /// It should be &gt;= 8, &lt;= 4096<br />
    /// If null, Source value is used
    /// </summary>
    [JsonPropertyName("imageWidth")]
    public int? ImageWidth { get; set; }

    /// <summary>
    /// Height to scale the screenshot to<br />
    /// It should be &gt;= 8, &lt;= 4096<br />
    /// If null, Source value is used
    /// </summary>
    [JsonPropertyName("imageHeight")]
    public int? ImageHeight { get; set; }

    /// <summary>
    /// Compression quality to use. 0 for high compression, 100 for uncompressed. -1 to use "default" (whatever that means, idk)<br />
    /// It should be &gt;= -1, &lt;= 100<br />
    /// If null, -1
    /// </summary>
    [JsonPropertyName("imageCompressionQuality")]
    public int? ImageCompressionQuality { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Gets an array of scenes in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneList : Request<GetSceneListResponse> {

    /// <summary>
    /// UUID of the canvas the scenes are in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneList
  /// </summary>
  public class GetSceneListResponse : RequestResponse {

    /// <summary>
    /// Current program scene name. Can be <c>null</c> if non-main canvas or internal state desync
    /// </summary>
    [JsonPropertyName("currentProgramSceneName")]
    public string? CurrentProgramSceneName { get; set; }

    /// <summary>
    /// Current program scene UUID. Can be <c>null</c> if non-main canvas or internal state desync
    /// </summary>
    [JsonPropertyName("currentProgramSceneUuid")]
    public string? CurrentProgramSceneUuid { get; set; }

    /// <summary>
    /// Current preview scene name. <c>null</c> if not in studio mode or non-main canvas
    /// </summary>
    [JsonPropertyName("currentPreviewSceneName")]
    public string? CurrentPreviewSceneName { get; set; }

    /// <summary>
    /// Current preview scene UUID. <c>null</c> if not in studio mode or non-main canvas
    /// </summary>
    [JsonPropertyName("currentPreviewSceneUuid")]
    public string? CurrentPreviewSceneUuid { get; set; }

    /// <summary>
    /// Array of scenes
    /// </summary>
    [JsonPropertyName("scenes")]
    public List<Scene> Scenes { get; set; } = [];
  }

  /// <summary>
  /// Gets an array of all groups in OBS.<br />
  /// <br />
  /// Groups in OBS are actually scenes, but renamed and modified. In obs-websocket, we treat them as scenes where we can.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetGroupList : Request<GetGroupListResponse> { }

  /// <summary>
  /// Response of GetGroupList
  /// </summary>
  public class GetGroupListResponse : RequestResponse {

    /// <summary>
    /// Array of group names
    /// </summary>
    [JsonPropertyName("groups")]
    public List<string> Groups { get; set; } = [];
  }

  /// <summary>
  /// Gets the current program scene.<br />
  /// <br />
  /// Note 1: This request is slated to have the <c>currentProgram</c>-prefixed fields removed from in an upcoming RPC version.<br />
  /// <br />
  /// Note 2: Canvases do not have any concept of a program or preview scene, so this request does not support canvases.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetCurrentProgramScene : Request<GetCurrentProgramSceneResponse> { }

  /// <summary>
  /// Response of GetCurrentProgramScene
  /// </summary>
  public class GetCurrentProgramSceneResponse : RequestResponse {

    /// <summary>
    /// Current program scene name
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Current program scene UUID
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string SceneUuid { get; set; } = "";

    /// <summary>
    /// Current program scene name (Deprecated)
    /// </summary>
    [JsonPropertyName("currentProgramSceneName")]
    public string CurrentProgramSceneName { get; set; } = "";

    /// <summary>
    /// Current program scene UUID (Deprecated)
    /// </summary>
    [JsonPropertyName("currentProgramSceneUuid")]
    public string CurrentProgramSceneUuid { get; set; } = "";
  }

  /// <summary>
  /// Sets the current program scene.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetCurrentProgramScene : Request<RequestResponse> {

    /// <summary>
    /// Scene name to set as the current program scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// Scene UUID to set as the current program scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }
  }

  /// <summary>
  /// Gets the current preview scene.<br />
  /// <br />
  /// Only available when studio mode is enabled.<br />
  /// <br />
  /// Note: This request is slated to have the <c>currentPreview</c>-prefixed fields removed from in an upcoming RPC version.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetCurrentPreviewScene : Request<GetCurrentPreviewSceneResponse> { }

  /// <summary>
  /// Response of GetCurrentPreviewScene
  /// </summary>
  public class GetCurrentPreviewSceneResponse : RequestResponse {

    /// <summary>
    /// Current preview scene name
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// Current preview scene UUID
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string SceneUuid { get; set; } = "";

    /// <summary>
    /// Current preview scene name
    /// </summary>
    [JsonPropertyName("currentPreviewSceneName")]
    public string CurrentPreviewSceneName { get; set; } = "";

    /// <summary>
    /// Current preview scene UUID
    /// </summary>
    [JsonPropertyName("currentPreviewSceneUuid")]
    public string CurrentPreviewSceneUuid { get; set; } = "";
  }

  /// <summary>
  /// Sets the current preview scene.<br />
  /// <br />
  /// Only available when studio mode is enabled.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetCurrentPreviewScene : Request<RequestResponse> {

    /// <summary>
    /// Scene name to set as the current preview scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// Scene UUID to set as the current preview scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }
  }

  /// <summary>
  /// Creates a new scene in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class CreateScene : Request<CreateSceneResponse> {

    /// <summary>
    /// Name for the new scene
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = "";

    /// <summary>
    /// UUID of the canvas to create the new scene in. Leave default to assume main canvas<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of CreateScene
  /// </summary>
  public class CreateSceneResponse : RequestResponse {

    /// <summary>
    /// UUID of the created scene
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string SceneUuid { get; set; } = "";
  }

  /// <summary>
  /// Removes a scene from OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class RemoveScene : Request<RequestResponse> {

    /// <summary>
    /// Name of the scene to remove<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene to remove<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Sets the name of a scene (rename).<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSceneName : Request<RequestResponse> {

    /// <summary>
    /// New name for the scene
    /// </summary>
    [JsonPropertyName("newSceneName")]
    public string NewSceneName { get; set; } = "";

    /// <summary>
    /// Name of the scene to be renamed<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene to be renamed<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Gets the scene transition overridden for a scene.<br />
  /// <br />
  /// Note: A transition UUID response field is not currently able to be implemented as of 2024-1-18.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneSceneTransitionOverride : Request<GetSceneSceneTransitionOverrideResponse> {

    /// <summary>
    /// Name of the scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneSceneTransitionOverride
  /// </summary>
  public class GetSceneSceneTransitionOverrideResponse : RequestResponse {

    /// <summary>
    /// Name of the overridden scene transition, else <c>null</c>
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string? TransitionName { get; set; }

    /// <summary>
    /// Duration of the overridden scene transition, else <c>null</c>
    /// </summary>
    [JsonPropertyName("transitionDuration")]
    public long? TransitionDuration { get; set; }
  }

  /// <summary>
  /// Sets the scene transition overridden for a scene.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSceneSceneTransitionOverride : Request<RequestResponse> {

    /// <summary>
    /// Name of the scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// Name of the scene transition to use as override. Specify <c>null</c> to remove<br />
    /// If null, Unchanged
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string? TransitionName { get; set; }

    /// <summary>
    /// Duration to use for any overridden transition. Specify <c>null</c> to remove<br />
    /// It should be &gt;= 50, &lt;= 20000<br />
    /// If null, Unchanged
    /// </summary>
    [JsonPropertyName("transitionDuration")]
    public long? TransitionDuration { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Gets an array of all inputs in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputList : Request<GetInputListResponse> {

    /// <summary>
    /// Restrict the array to only inputs of the specified kind<br />
    /// If null, All kinds included
    /// </summary>
    [JsonPropertyName("inputKind")]
    public string? InputKind { get; set; }
  }

  /// <summary>
  /// Response of GetInputList
  /// </summary>
  public class GetInputListResponse : RequestResponse {

    /// <summary>
    /// Array of inputs
    /// </summary>
    [JsonPropertyName("inputs")]
    public List<Input> Inputs { get; set; } = [];
  }

  /// <summary>
  /// Gets an array of all available input kinds in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputKindList : Request<GetInputKindListResponse> {

    /// <summary>
    /// True == Return all kinds as unversioned, False == Return with version suffixes (if available)<br />
    /// If null, false
    /// </summary>
    [JsonPropertyName("unversioned")]
    public bool? Unversioned { get; set; }
  }

  /// <summary>
  /// Response of GetInputKindList
  /// </summary>
  public class GetInputKindListResponse : RequestResponse {

    /// <summary>
    /// Array of input kinds
    /// </summary>
    [JsonPropertyName("inputKinds")]
    public List<string> InputKinds { get; set; } = [];
  }

  /// <summary>
  /// Gets the names of all special inputs.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSpecialInputs : Request<GetSpecialInputsResponse> { }

  /// <summary>
  /// Response of GetSpecialInputs
  /// </summary>
  public class GetSpecialInputsResponse : RequestResponse {

    /// <summary>
    /// Name of the Desktop Audio input
    /// </summary>
    [JsonPropertyName("desktop1")]
    public string Desktop1 { get; set; } = "";

    /// <summary>
    /// Name of the Desktop Audio 2 input
    /// </summary>
    [JsonPropertyName("desktop2")]
    public string Desktop2 { get; set; } = "";

    /// <summary>
    /// Name of the Mic/Auxiliary Audio input
    /// </summary>
    [JsonPropertyName("mic1")]
    public string Mic1 { get; set; } = "";

    /// <summary>
    /// Name of the Mic/Auxiliary Audio 2 input
    /// </summary>
    [JsonPropertyName("mic2")]
    public string Mic2 { get; set; } = "";

    /// <summary>
    /// Name of the Mic/Auxiliary Audio 3 input
    /// </summary>
    [JsonPropertyName("mic3")]
    public string Mic3 { get; set; } = "";

    /// <summary>
    /// Name of the Mic/Auxiliary Audio 4 input
    /// </summary>
    [JsonPropertyName("mic4")]
    public string Mic4 { get; set; } = "";
  }

  /// <summary>
  /// Creates a new input, adding it as a scene item to the specified scene.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class CreateInput : Request<CreateInputResponse> {

    /// <summary>
    /// Name of the new input to created
    /// </summary>
    [JsonPropertyName("inputName")]
    public string InputName { get; set; } = "";

    /// <summary>
    /// The kind of input to be created
    /// </summary>
    [JsonPropertyName("inputKind")]
    public string InputKind { get; set; } = "";

    /// <summary>
    /// Name of the scene to add the input to as a scene item<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene to add the input to as a scene item<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// Settings object to initialize the input with<br />
    /// If null, Default settings used
    /// </summary>
    [JsonPropertyName("inputSettings")]
    public Dictionary<string, JsonElement?>? InputSettings { get; set; }

    /// <summary>
    /// Whether to set the created scene item to enabled or disabled<br />
    /// If null, True
    /// </summary>
    [JsonPropertyName("sceneItemEnabled")]
    public bool? SceneItemEnabled { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of CreateInput
  /// </summary>
  public class CreateInputResponse : RequestResponse {

    /// <summary>
    /// UUID of the newly created input
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string InputUuid { get; set; } = "";

    /// <summary>
    /// ID of the newly created scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Removes an existing input.<br />
  /// <br />
  /// Note: Will immediately remove all associated scene items.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class RemoveInput : Request<RequestResponse> {

    /// <summary>
    /// Name of the input to remove<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to remove<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Sets the name of an input (rename).<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetInputName : Request<RequestResponse> {

    /// <summary>
    /// New name for the input
    /// </summary>
    [JsonPropertyName("newInputName")]
    public string NewInputName { get; set; } = "";

    /// <summary>
    /// Current input name<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// Current input UUID<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets the default settings for an input kind.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputDefaultSettings : Request<GetInputDefaultSettingsResponse> {

    /// <summary>
    /// Input kind to get the default settings for
    /// </summary>
    [JsonPropertyName("inputKind")]
    public string InputKind { get; set; } = "";
  }

  /// <summary>
  /// Response of GetInputDefaultSettings
  /// </summary>
  public class GetInputDefaultSettingsResponse : RequestResponse {

    /// <summary>
    /// Object of default settings for the input kind
    /// </summary>
    [JsonPropertyName("defaultInputSettings")]
    public Dictionary<string, JsonElement?> DefaultInputSettings { get; set; } = [];
  }

  /// <summary>
  /// Gets the settings of an input.<br />
  /// <br />
  /// Note: Does not include defaults. To create the entire settings object, overlay <c>inputSettings</c> over the <c>defaultInputSettings</c> provided by <c>GetInputDefaultSettings</c>.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputSettings : Request<GetInputSettingsResponse> {

    /// <summary>
    /// Name of the input to get the settings of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to get the settings of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputSettings
  /// </summary>
  public class GetInputSettingsResponse : RequestResponse {

    /// <summary>
    /// Object of settings for the input
    /// </summary>
    [JsonPropertyName("inputSettings")]
    public Dictionary<string, JsonElement?> InputSettings { get; set; } = [];

    /// <summary>
    /// The kind of the input
    /// </summary>
    [JsonPropertyName("inputKind")]
    public string InputKind { get; set; } = "";
  }

  /// <summary>
  /// Sets the settings of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetInputSettings : Request<RequestResponse> {

    /// <summary>
    /// Object of settings to apply
    /// </summary>
    [JsonPropertyName("inputSettings")]
    public Dictionary<string, JsonElement?> InputSettings { get; set; } = [];

    /// <summary>
    /// Name of the input to set the settings of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to set the settings of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }

    /// <summary>
    /// True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.<br />
    /// If null, true
    /// </summary>
    [JsonPropertyName("overlay")]
    public bool? Overlay { get; set; }
  }

  /// <summary>
  /// Gets the audio mute state of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputMute : Request<GetInputMuteResponse> {

    /// <summary>
    /// Name of input to get the mute state of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of input to get the mute state of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputMute
  /// </summary>
  public class GetInputMuteResponse : RequestResponse {

    /// <summary>
    /// Whether the input is muted
    /// </summary>
    [JsonPropertyName("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// Sets the audio mute state of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetInputMute : Request<RequestResponse> {

    /// <summary>
    /// Whether to mute the input or not
    /// </summary>
    [JsonPropertyName("inputMuted")]
    public bool InputMuted { get; set; }

    /// <summary>
    /// Name of the input to set the mute state of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to set the mute state of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Toggles the audio mute state of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class ToggleInputMute : Request<ToggleInputMuteResponse> {

    /// <summary>
    /// Name of the input to toggle the mute state of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to toggle the mute state of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of ToggleInputMute
  /// </summary>
  public class ToggleInputMuteResponse : RequestResponse {

    /// <summary>
    /// Whether the input has been muted or unmuted
    /// </summary>
    [JsonPropertyName("inputMuted")]
    public bool InputMuted { get; set; }
  }

  /// <summary>
  /// Gets the current volume setting of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputVolume : Request<GetInputVolumeResponse> {

    /// <summary>
    /// Name of the input to get the volume of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to get the volume of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputVolume
  /// </summary>
  public class GetInputVolumeResponse : RequestResponse {

    /// <summary>
    /// Volume setting in mul
    /// </summary>
    [JsonPropertyName("inputVolumeMul")]
    public double InputVolumeMul { get; set; }

    /// <summary>
    /// Volume setting in dB
    /// </summary>
    [JsonPropertyName("inputVolumeDb")]
    public double InputVolumeDb { get; set; }
  }

  /// <summary>
  /// Sets the volume setting of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetInputVolume : Request<RequestResponse> {

    /// <summary>
    /// Name of the input to set the volume of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to set the volume of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }

    /// <summary>
    /// Volume setting in mul<br />
    /// It should be &gt;= 0, &lt;= 20<br />
    /// If null, <c>inputVolumeDb</c> should be specified
    /// </summary>
    [JsonPropertyName("inputVolumeMul")]
    public double? InputVolumeMul { get; set; }

    /// <summary>
    /// Volume setting in dB<br />
    /// It should be &gt;= -100, &lt;= 26<br />
    /// If null, <c>inputVolumeMul</c> should be specified
    /// </summary>
    [JsonPropertyName("inputVolumeDb")]
    public double? InputVolumeDb { get; set; }
  }

  /// <summary>
  /// Gets the audio balance of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputAudioBalance : Request<GetInputAudioBalanceResponse> {

    /// <summary>
    /// Name of the input to get the audio balance of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to get the audio balance of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputAudioBalance
  /// </summary>
  public class GetInputAudioBalanceResponse : RequestResponse {

    /// <summary>
    /// Audio balance value from 0.0-1.0
    /// </summary>
    [JsonPropertyName("inputAudioBalance")]
    public double InputAudioBalance { get; set; }
  }

  /// <summary>
  /// Sets the audio balance of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetInputAudioBalance : Request<RequestResponse> {

    /// <summary>
    /// New audio balance value<br />
    /// It should be &gt;= 0.0, &lt;= 1.0
    /// </summary>
    [JsonPropertyName("inputAudioBalance")]
    public double InputAudioBalance { get; set; }

    /// <summary>
    /// Name of the input to set the audio balance of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to set the audio balance of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets the audio sync offset of an input.<br />
  /// <br />
  /// Note: The audio sync offset can be negative too!<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputAudioSyncOffset : Request<GetInputAudioSyncOffsetResponse> {

    /// <summary>
    /// Name of the input to get the audio sync offset of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to get the audio sync offset of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputAudioSyncOffset
  /// </summary>
  public class GetInputAudioSyncOffsetResponse : RequestResponse {

    /// <summary>
    /// Audio sync offset in milliseconds
    /// </summary>
    [JsonPropertyName("inputAudioSyncOffset")]
    public int InputAudioSyncOffset { get; set; }
  }

  /// <summary>
  /// Sets the audio sync offset of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetInputAudioSyncOffset : Request<RequestResponse> {

    /// <summary>
    /// New audio sync offset in milliseconds<br />
    /// It should be &gt;= -950, &lt;= 20000
    /// </summary>
    [JsonPropertyName("inputAudioSyncOffset")]
    public int InputAudioSyncOffset { get; set; }

    /// <summary>
    /// Name of the input to set the audio sync offset of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to set the audio sync offset of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
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
  public class GetInputAudioMonitorType : Request<GetInputAudioMonitorTypeResponse> {

    /// <summary>
    /// Name of the input to get the audio monitor type of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to get the audio monitor type of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputAudioMonitorType
  /// </summary>
  public class GetInputAudioMonitorTypeResponse : RequestResponse {

    /// <summary>
    /// Audio monitor type
    /// </summary>
    [JsonPropertyName("monitorType")]
    public MonitoringType MonitorType { get; set; } = new();
  }

  /// <summary>
  /// Sets the audio monitor type of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetInputAudioMonitorType : Request<RequestResponse> {

    /// <summary>
    /// Audio monitor type
    /// </summary>
    [JsonPropertyName("monitorType")]
    public MonitoringType MonitorType { get; set; } = new();

    /// <summary>
    /// Name of the input to set the audio monitor type of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to set the audio monitor type of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets the enable state of all audio tracks of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputAudioTracks : Request<GetInputAudioTracksResponse> {

    /// <summary>
    /// Name of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputAudioTracks
  /// </summary>
  public class GetInputAudioTracksResponse : RequestResponse {

    /// <summary>
    /// Object of audio tracks and associated enable states
    /// </summary>
    [JsonPropertyName("inputAudioTracks")]
    public Dictionary<string, bool> InputAudioTracks { get; set; } = [];
  }

  /// <summary>
  /// Sets the enable state of audio tracks of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetInputAudioTracks : Request<RequestResponse> {

    /// <summary>
    /// Track settings to apply
    /// </summary>
    [JsonPropertyName("inputAudioTracks")]
    public Dictionary<string, bool> InputAudioTracks { get; set; } = [];

    /// <summary>
    /// Name of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets the deinterlace mode of an input.<br />
  /// <br />
  /// Deinterlace Modes:<br />
  /// <br />
  /// - <c>OBS_DEINTERLACE_MODE_DISABLE</c><br />
  /// - <c>OBS_DEINTERLACE_MODE_DISCARD</c><br />
  /// - <c>OBS_DEINTERLACE_MODE_RETRO</c><br />
  /// - <c>OBS_DEINTERLACE_MODE_BLEND</c><br />
  /// - <c>OBS_DEINTERLACE_MODE_BLEND_2X</c><br />
  /// - <c>OBS_DEINTERLACE_MODE_LINEAR</c><br />
  /// - <c>OBS_DEINTERLACE_MODE_LINEAR_2X</c><br />
  /// - <c>OBS_DEINTERLACE_MODE_YADIF</c><br />
  /// - <c>OBS_DEINTERLACE_MODE_YADIF_2X</c><br />
  /// <br />
  /// Note: Deinterlacing functionality is restricted to async inputs only.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.6.0
  /// </summary>
  public class GetInputDeinterlaceMode : Request<GetInputDeinterlaceModeResponse> {

    /// <summary>
    /// Name of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputDeinterlaceMode
  /// </summary>
  public class GetInputDeinterlaceModeResponse : RequestResponse {

    /// <summary>
    /// Deinterlace mode of the input
    /// </summary>
    [JsonPropertyName("inputDeinterlaceMode")]
    public string InputDeinterlaceMode { get; set; } = "";
  }

  /// <summary>
  /// Sets the deinterlace mode of an input.<br />
  /// <br />
  /// Note: Deinterlacing functionality is restricted to async inputs only.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.6.0
  /// </summary>
  public class SetInputDeinterlaceMode : Request<RequestResponse> {

    /// <summary>
    /// Deinterlace mode for the input
    /// </summary>
    [JsonPropertyName("inputDeinterlaceMode")]
    public string InputDeinterlaceMode { get; set; } = "";

    /// <summary>
    /// Name of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets the deinterlace field order of an input.<br />
  /// <br />
  /// Deinterlace Field Orders:<br />
  /// <br />
  /// - <c>OBS_DEINTERLACE_FIELD_ORDER_TOP</c><br />
  /// - <c>OBS_DEINTERLACE_FIELD_ORDER_BOTTOM</c><br />
  /// <br />
  /// Note: Deinterlacing functionality is restricted to async inputs only.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.6.0
  /// </summary>
  public class GetInputDeinterlaceFieldOrder : Request<GetInputDeinterlaceFieldOrderResponse> {

    /// <summary>
    /// Name of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputDeinterlaceFieldOrder
  /// </summary>
  public class GetInputDeinterlaceFieldOrderResponse : RequestResponse {

    /// <summary>
    /// Deinterlace field order of the input
    /// </summary>
    [JsonPropertyName("inputDeinterlaceFieldOrder")]
    public string InputDeinterlaceFieldOrder { get; set; } = "";
  }

  /// <summary>
  /// Sets the deinterlace field order of an input.<br />
  /// <br />
  /// Note: Deinterlacing functionality is restricted to async inputs only.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.6.0
  /// </summary>
  public class SetInputDeinterlaceFieldOrder : Request<RequestResponse> {

    /// <summary>
    /// Deinterlace field order for the input
    /// </summary>
    [JsonPropertyName("inputDeinterlaceFieldOrder")]
    public string InputDeinterlaceFieldOrder { get; set; } = "";

    /// <summary>
    /// Name of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets the items of a list property from an input's properties.<br />
  /// <br />
  /// Note: Use this in cases where an input provides a dynamic, selectable list of items. For example, display capture, where it provides a list of available displays.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetInputPropertiesListPropertyItems : Request<GetInputPropertiesListPropertyItemsResponse> {

    /// <summary>
    /// Name of the list property to get the items of
    /// </summary>
    [JsonPropertyName("propertyName")]
    public string PropertyName { get; set; } = "";

    /// <summary>
    /// Name of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetInputPropertiesListPropertyItems
  /// </summary>
  public class GetInputPropertiesListPropertyItemsResponse : RequestResponse {

    /// <summary>
    /// Array of items in the list property
    /// </summary>
    [JsonPropertyName("propertyItems")]
    public List<Dictionary<string, JsonElement?>> PropertyItems { get; set; } = [];
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
  public class PressInputPropertiesButton : Request<RequestResponse> {

    /// <summary>
    /// Name of the button property to press
    /// </summary>
    [JsonPropertyName("propertyName")]
    public string PropertyName { get; set; } = "";

    /// <summary>
    /// Name of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets an array of all available transition kinds.<br />
  /// <br />
  /// Similar to <c>GetInputKindList</c><br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetTransitionKindList : Request<GetTransitionKindListResponse> { }

  /// <summary>
  /// Response of GetTransitionKindList
  /// </summary>
  public class GetTransitionKindListResponse : RequestResponse {

    /// <summary>
    /// Array of transition kinds
    /// </summary>
    [JsonPropertyName("transitionKinds")]
    public List<string> TransitionKinds { get; set; } = [];
  }

  /// <summary>
  /// Gets an array of all scene transitions in OBS.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneTransitionList : Request<GetSceneTransitionListResponse> { }

  /// <summary>
  /// Response of GetSceneTransitionList
  /// </summary>
  public class GetSceneTransitionListResponse : RequestResponse {

    /// <summary>
    /// Name of the current scene transition. Can be null
    /// </summary>
    [JsonPropertyName("currentSceneTransitionName")]
    public string? CurrentSceneTransitionName { get; set; }

    /// <summary>
    /// UUID of the current scene transition. Can be null
    /// </summary>
    [JsonPropertyName("currentSceneTransitionUuid")]
    public string? CurrentSceneTransitionUuid { get; set; }

    /// <summary>
    /// Kind of the current scene transition. Can be null
    /// </summary>
    [JsonPropertyName("currentSceneTransitionKind")]
    public string? CurrentSceneTransitionKind { get; set; }

    /// <summary>
    /// Array of transitions
    /// </summary>
    [JsonPropertyName("transitions")]
    public List<AvailableTransition> Transitions { get; set; } = [];
  }

  /// <summary>
  /// Gets information about the current scene transition.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetCurrentSceneTransition : Request<GetCurrentSceneTransitionResponse> { }

  /// <summary>
  /// Response of GetCurrentSceneTransition
  /// </summary>
  public class GetCurrentSceneTransitionResponse : RequestResponse {

    /// <summary>
    /// Name of the transition
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string TransitionName { get; set; } = "";

    /// <summary>
    /// UUID of the transition
    /// </summary>
    [JsonPropertyName("transitionUuid")]
    public string TransitionUuid { get; set; } = "";

    /// <summary>
    /// Kind of the transition
    /// </summary>
    [JsonPropertyName("transitionKind")]
    public string TransitionKind { get; set; } = "";

    /// <summary>
    /// Whether the transition uses a fixed (unconfigurable) duration
    /// </summary>
    [JsonPropertyName("transitionFixed")]
    public bool TransitionFixed { get; set; }

    /// <summary>
    /// Configured transition duration in milliseconds. <c>null</c> if transition is fixed
    /// </summary>
    [JsonPropertyName("transitionDuration")]
    public long? TransitionDuration { get; set; }

    /// <summary>
    /// Whether the transition supports being configured
    /// </summary>
    [JsonPropertyName("transitionConfigurable")]
    public bool TransitionConfigurable { get; set; }

    /// <summary>
    /// Object of settings for the transition. <c>null</c> if transition is not configurable
    /// </summary>
    [JsonPropertyName("transitionSettings")]
    public Dictionary<string, JsonElement?>? TransitionSettings { get; set; } = [];
  }

  /// <summary>
  /// Sets the current scene transition.<br />
  /// <br />
  /// Small note: While the namespace of scene transitions is generally unique, that uniqueness is not a guarantee as it is with other resources like inputs.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetCurrentSceneTransition : Request<RequestResponse> {

    /// <summary>
    /// Name of the transition to make active
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  /// <summary>
  /// Sets the duration of the current scene transition, if it is not fixed.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetCurrentSceneTransitionDuration : Request<RequestResponse> {

    /// <summary>
    /// Duration in milliseconds<br />
    /// It should be &gt;= 50, &lt;= 20000
    /// </summary>
    [JsonPropertyName("transitionDuration")]
    public long TransitionDuration { get; set; }
  }

  /// <summary>
  /// Sets the settings of the current scene transition.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetCurrentSceneTransitionSettings : Request<RequestResponse> {

    /// <summary>
    /// Settings object to apply to the transition. Can be <c>{}</c>
    /// </summary>
    [JsonPropertyName("transitionSettings")]
    public Dictionary<string, JsonElement?> TransitionSettings { get; set; } = [];

    /// <summary>
    /// Whether to overlay over the current settings or replace them<br />
    /// If null, true
    /// </summary>
    [JsonPropertyName("overlay")]
    public bool? Overlay { get; set; }
  }

  /// <summary>
  /// Gets the cursor position of the current scene transition.<br />
  /// <br />
  /// Note: <c>transitionCursor</c> will return 1.0 when the transition is inactive.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetCurrentSceneTransitionCursor : Request<GetCurrentSceneTransitionCursorResponse> { }

  /// <summary>
  /// Response of GetCurrentSceneTransitionCursor
  /// </summary>
  public class GetCurrentSceneTransitionCursorResponse : RequestResponse {

    /// <summary>
    /// Cursor position, between 0.0 and 1.0
    /// </summary>
    [JsonPropertyName("transitionCursor")]
    public double TransitionCursor { get; set; }
  }

  /// <summary>
  /// Triggers the current scene transition. Same functionality as the <c>Transition</c> button in studio mode.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class TriggerStudioModeTransition : Request<RequestResponse> { }

  /// <summary>
  /// Sets the position of the TBar.<br />
  /// <br />
  /// **Very important note**: This will be deprecated and replaced in a future version of obs-websocket.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetTBarPosition : Request<RequestResponse> {

    /// <summary>
    /// New position<br />
    /// It should be &gt;= 0.0, &lt;= 1.0
    /// </summary>
    [JsonPropertyName("position")]
    public double Position { get; set; }

    /// <summary>
    /// Whether to release the TBar. Only set <c>false</c> if you know that you will be sending another position update<br />
    /// If null, <c>true</c>
    /// </summary>
    [JsonPropertyName("release")]
    public bool? Release { get; set; }
  }

  /// <summary>
  /// Gets an array of all available source filter kinds.<br />
  /// <br />
  /// Similar to <c>GetInputKindList</c><br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.4.0
  /// </summary>
  public class GetSourceFilterKindList : Request<GetSourceFilterKindListResponse> { }

  /// <summary>
  /// Response of GetSourceFilterKindList
  /// </summary>
  public class GetSourceFilterKindListResponse : RequestResponse {

    /// <summary>
    /// Array of source filter kinds
    /// </summary>
    [JsonPropertyName("sourceFilterKinds")]
    public List<string> SourceFilterKinds { get; set; } = [];
  }

  /// <summary>
  /// Gets an array of all of a source's filters.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSourceFilterList : Request<GetSourceFilterListResponse> {

    /// <summary>
    /// Name of the source<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSourceFilterList
  /// </summary>
  public class GetSourceFilterListResponse : RequestResponse {

    /// <summary>
    /// Array of filters
    /// </summary>
    [JsonPropertyName("filters")]
    public List<SourceFilter> Filters { get; set; } = [];
  }

  /// <summary>
  /// Gets the default settings for a filter kind.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSourceFilterDefaultSettings : Request<GetSourceFilterDefaultSettingsResponse> {

    /// <summary>
    /// Filter kind to get the default settings for
    /// </summary>
    [JsonPropertyName("filterKind")]
    public string FilterKind { get; set; } = "";
  }

  /// <summary>
  /// Response of GetSourceFilterDefaultSettings
  /// </summary>
  public class GetSourceFilterDefaultSettingsResponse : RequestResponse {

    /// <summary>
    /// Object of default settings for the filter kind
    /// </summary>
    [JsonPropertyName("defaultFilterSettings")]
    public Dictionary<string, JsonElement?> DefaultFilterSettings { get; set; } = [];
  }

  /// <summary>
  /// Creates a new filter, adding it to the specified source.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class CreateSourceFilter : Request<RequestResponse> {

    /// <summary>
    /// Name of the new filter to be created
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// The kind of filter to be created
    /// </summary>
    [JsonPropertyName("filterKind")]
    public string FilterKind { get; set; } = "";

    /// <summary>
    /// Name of the source to add the filter to<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source to add the filter to<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// Settings object to initialize the filter with<br />
    /// If null, Default settings used
    /// </summary>
    [JsonPropertyName("filterSettings")]
    public Dictionary<string, JsonElement?>? FilterSettings { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Removes a filter from a source.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class RemoveSourceFilter : Request<RequestResponse> {

    /// <summary>
    /// Name of the filter to remove
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// Name of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Sets the name of a source filter (rename).<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSourceFilterName : Request<RequestResponse> {

    /// <summary>
    /// Current name of the filter
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// New name for the filter
    /// </summary>
    [JsonPropertyName("newFilterName")]
    public string NewFilterName { get; set; } = "";

    /// <summary>
    /// Name of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Gets the info for a specific source filter.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSourceFilter : Request<GetSourceFilterResponse> {

    /// <summary>
    /// Name of the filter
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// Name of the source<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSourceFilter
  /// </summary>
  public class GetSourceFilterResponse : RequestResponse {

    /// <summary>
    /// Whether the filter is enabled
    /// </summary>
    [JsonPropertyName("filterEnabled")]
    public bool FilterEnabled { get; set; }

    /// <summary>
    /// Index of the filter in the list, beginning at 0
    /// </summary>
    [JsonPropertyName("filterIndex")]
    public int FilterIndex { get; set; }

    /// <summary>
    /// The kind of filter
    /// </summary>
    [JsonPropertyName("filterKind")]
    public string FilterKind { get; set; } = "";

    /// <summary>
    /// Settings object associated with the filter
    /// </summary>
    [JsonPropertyName("filterSettings")]
    public Dictionary<string, JsonElement?> FilterSettings { get; set; } = [];
  }

  /// <summary>
  /// Sets the index position of a filter on a source.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSourceFilterIndex : Request<RequestResponse> {

    /// <summary>
    /// Name of the filter
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// New index position of the filter<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("filterIndex")]
    public int FilterIndex { get; set; }

    /// <summary>
    /// Name of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Sets the settings of a source filter.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSourceFilterSettings : Request<RequestResponse> {

    /// <summary>
    /// Name of the filter to set the settings of
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// Object of settings to apply
    /// </summary>
    [JsonPropertyName("filterSettings")]
    public Dictionary<string, JsonElement?> FilterSettings { get; set; } = [];

    /// <summary>
    /// Name of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// True == apply the settings on top of existing ones, False == reset the input to its defaults, then apply settings.<br />
    /// If null, true
    /// </summary>
    [JsonPropertyName("overlay")]
    public bool? Overlay { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Sets the enable state of a source filter.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSourceFilterEnabled : Request<RequestResponse> {

    /// <summary>
    /// Name of the filter
    /// </summary>
    [JsonPropertyName("filterName")]
    public string FilterName { get; set; } = "";

    /// <summary>
    /// New enable state of the filter
    /// </summary>
    [JsonPropertyName("filterEnabled")]
    public bool FilterEnabled { get; set; }

    /// <summary>
    /// Name of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source the filter is on<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Gets a list of all scene items in a scene.<br />
  /// <br />
  /// Scenes only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneItemList : Request<GetSceneItemListResponse> {

    /// <summary>
    /// Name of the scene to get the items of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene to get the items of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemList
  /// </summary>
  public class GetSceneItemListResponse : RequestResponse {

    /// <summary>
    /// Array of scene items in the scene
    /// </summary>
    [JsonPropertyName("sceneItems")]
    public List<SceneItem> SceneItems { get; set; } = [];
  }

  /// <summary>
  /// Basically GetSceneItemList, but for groups.<br />
  /// <br />
  /// Using groups at all in OBS is discouraged, as they are very broken under the hood. Please use nested scenes instead.<br />
  /// <br />
  /// Groups only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetGroupSceneItemList : Request<GetGroupSceneItemListResponse> {

    /// <summary>
    /// Name of the group to get the items of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the group to get the items of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the group is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetGroupSceneItemList
  /// </summary>
  public class GetGroupSceneItemListResponse : RequestResponse {

    /// <summary>
    /// Array of scene items in the group
    /// </summary>
    [JsonPropertyName("sceneItems")]
    public List<SceneItem> SceneItems { get; set; } = [];
  }

  /// <summary>
  /// Searches a scene for a source, and returns its id.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneItemId : Request<GetSceneItemIdResponse> {

    /// <summary>
    /// Name of the source to find
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// Name of the scene or group to search in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene or group to search in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// Number of matches to skip during search. &gt;= 0 means first forward. -1 means last (top) item<br />
    /// It should be &gt;= -1<br />
    /// If null, 0
    /// </summary>
    [JsonPropertyName("searchOffset")]
    public int? SearchOffset { get; set; }

    /// <summary>
    /// UUID of the canvas the scene or group is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemId
  /// </summary>
  public class GetSceneItemIdResponse : RequestResponse {

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Gets the source associated with a scene item.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.4.0
  /// </summary>
  public class GetSceneItemSource : Request<GetSceneItemSourceResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemSource
  /// </summary>
  public class GetSceneItemSourceResponse : RequestResponse {

    /// <summary>
    /// Name of the source associated with the scene item
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = "";

    /// <summary>
    /// UUID of the source associated with the scene item
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string SourceUuid { get; set; } = "";
  }

  /// <summary>
  /// Creates a new scene item using a source.<br />
  /// <br />
  /// Scenes only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class CreateSceneItem : Request<CreateSceneItemResponse> {

    /// <summary>
    /// Name of the scene to create the new item in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene to create the new item in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// Name of the source to add to the scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source to add to the scene<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// Enable state to apply to the scene item on creation<br />
    /// If null, True
    /// </summary>
    [JsonPropertyName("sceneItemEnabled")]
    public bool? SceneItemEnabled { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of CreateSceneItem
  /// </summary>
  public class CreateSceneItemResponse : RequestResponse {

    /// <summary>
    /// Numeric ID of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Removes a scene item from a scene.<br />
  /// <br />
  /// Scenes only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class RemoveSceneItem : Request<RequestResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Duplicates a scene item, copying all transform and crop info.<br />
  /// <br />
  /// Scenes only<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class DuplicateSceneItem : Request<DuplicateSceneItemResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// Name of the scene to create the duplicated item in<br />
    /// If null, From scene is assumed
    /// </summary>
    [JsonPropertyName("destinationSceneName")]
    public string? DestinationSceneName { get; set; }

    /// <summary>
    /// UUID of the scene to create the duplicated item in<br />
    /// If null, From scene is assumed
    /// </summary>
    [JsonPropertyName("destinationSceneUuid")]
    public string? DestinationSceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of DuplicateSceneItem
  /// </summary>
  public class DuplicateSceneItemResponse : RequestResponse {

    /// <summary>
    /// Numeric ID of the duplicated scene item
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }
  }

  /// <summary>
  /// Gets the transform and crop info of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneItemTransform : Request<GetSceneItemTransformResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemTransform
  /// </summary>
  public class GetSceneItemTransformResponse : RequestResponse {

    /// <summary>
    /// Object containing scene item transform info
    /// </summary>
    [JsonPropertyName("sceneItemTransform")]
    public Dictionary<string, JsonElement?> SceneItemTransform { get; set; } = [];
  }

  /// <summary>
  /// Sets the transform and crop info of a scene item.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSceneItemTransform : Request<RequestResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Object containing scene item transform info to update
    /// </summary>
    [JsonPropertyName("sceneItemTransform")]
    public Dictionary<string, JsonElement?> SceneItemTransform { get; set; } = [];

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Gets the enable state of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneItemEnabled : Request<GetSceneItemEnabledResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemEnabled
  /// </summary>
  public class GetSceneItemEnabledResponse : RequestResponse {

    /// <summary>
    /// Whether the scene item is enabled. <c>true</c> for enabled, <c>false</c> for disabled
    /// </summary>
    [JsonPropertyName("sceneItemEnabled")]
    public bool SceneItemEnabled { get; set; }
  }

  /// <summary>
  /// Sets the enable state of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSceneItemEnabled : Request<RequestResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New enable state of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemEnabled")]
    public bool SceneItemEnabled { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Gets the lock state of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetSceneItemLocked : Request<GetSceneItemLockedResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemLocked
  /// </summary>
  public class GetSceneItemLockedResponse : RequestResponse {

    /// <summary>
    /// Whether the scene item is locked. <c>true</c> for locked, <c>false</c> for unlocked
    /// </summary>
    [JsonPropertyName("sceneItemLocked")]
    public bool SceneItemLocked { get; set; }
  }

  /// <summary>
  /// Sets the lock state of a scene item.<br />
  /// <br />
  /// Scenes and Group<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSceneItemLocked : Request<RequestResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New lock state of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemLocked")]
    public bool SceneItemLocked { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
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
  public class GetSceneItemIndex : Request<GetSceneItemIndexResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemIndex
  /// </summary>
  public class GetSceneItemIndexResponse : RequestResponse {

    /// <summary>
    /// Index position of the scene item
    /// </summary>
    [JsonPropertyName("sceneItemIndex")]
    public int SceneItemIndex { get; set; }
  }

  /// <summary>
  /// Sets the index position of a scene item in a scene.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSceneItemIndex : Request<RequestResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New index position of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemIndex")]
    public int SceneItemIndex { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
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
  public class GetSceneItemBlendMode : Request<GetSceneItemBlendModeResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Response of GetSceneItemBlendMode
  /// </summary>
  public class GetSceneItemBlendModeResponse : RequestResponse {

    /// <summary>
    /// Current blend mode
    /// </summary>
    [JsonPropertyName("sceneItemBlendMode")]
    public BlendingType SceneItemBlendMode { get; set; } = new();
  }

  /// <summary>
  /// Sets the blend mode of a scene item.<br />
  /// <br />
  /// Scenes and Groups<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetSceneItemBlendMode : Request<RequestResponse> {

    /// <summary>
    /// Numeric ID of the scene item<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("sceneItemId")]
    public int SceneItemId { get; set; }

    /// <summary>
    /// New blend mode
    /// </summary>
    [JsonPropertyName("sceneItemBlendMode")]
    public BlendingType SceneItemBlendMode { get; set; } = new();

    /// <summary>
    /// Name of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    /// <summary>
    /// UUID of the scene the item is in<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sceneUuid")]
    public string? SceneUuid { get; set; }

    /// <summary>
    /// UUID of the canvas the scene is in, if using the sceneName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }

  /// <summary>
  /// Gets the status of the virtualcam output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetVirtualCamStatus : Request<GetVirtualCamStatusResponse> { }

  /// <summary>
  /// Response of GetVirtualCamStatus
  /// </summary>
  public class GetVirtualCamStatusResponse : RequestResponse {

    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Toggles the state of the virtualcam output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class ToggleVirtualCam : Request<ToggleVirtualCamResponse> { }

  /// <summary>
  /// Response of ToggleVirtualCam
  /// </summary>
  public class ToggleVirtualCamResponse : RequestResponse {

    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the virtualcam output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class StartVirtualCam : Request<RequestResponse> { }

  /// <summary>
  /// Stops the virtualcam output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class StopVirtualCam : Request<RequestResponse> { }

  /// <summary>
  /// Gets the status of the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetReplayBufferStatus : Request<GetReplayBufferStatusResponse> { }

  /// <summary>
  /// Response of GetReplayBufferStatus
  /// </summary>
  public class GetReplayBufferStatusResponse : RequestResponse {

    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Toggles the state of the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class ToggleReplayBuffer : Request<ToggleReplayBufferResponse> { }

  /// <summary>
  /// Response of ToggleReplayBuffer
  /// </summary>
  public class ToggleReplayBufferResponse : RequestResponse {

    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class StartReplayBuffer : Request<RequestResponse> { }

  /// <summary>
  /// Stops the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class StopReplayBuffer : Request<RequestResponse> { }

  /// <summary>
  /// Saves the contents of the replay buffer output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SaveReplayBuffer : Request<RequestResponse> { }

  /// <summary>
  /// Gets the filename of the last replay buffer save file.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetLastReplayBufferReplay : Request<GetLastReplayBufferReplayResponse> { }

  /// <summary>
  /// Response of GetLastReplayBufferReplay
  /// </summary>
  public class GetLastReplayBufferReplayResponse : RequestResponse {

    /// <summary>
    /// File path
    /// </summary>
    [JsonPropertyName("savedReplayPath")]
    public string SavedReplayPath { get; set; } = "";
  }

  /// <summary>
  /// Gets the list of available outputs.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetOutputList : Request<GetOutputListResponse> { }

  /// <summary>
  /// Response of GetOutputList
  /// </summary>
  public class GetOutputListResponse : RequestResponse {

    /// <summary>
    /// Array of outputs
    /// </summary>
    [JsonPropertyName("outputs")]
    public List<Output> Outputs { get; set; } = [];
  }

  /// <summary>
  /// Gets the status of an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetOutputStatus : Request<GetOutputStatusResponse> {

    /// <summary>
    /// Output name
    /// </summary>
    [JsonPropertyName("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetOutputStatus
  /// </summary>
  public class GetOutputStatusResponse : RequestResponse {

    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// Whether the output is reconnecting
    /// </summary>
    [JsonPropertyName("outputReconnecting")]
    public bool OutputReconnecting { get; set; }

    /// <summary>
    /// Current formatted timecode string for the output
    /// </summary>
    [JsonPropertyName("outputTimecode")]
    public string OutputTimecode { get; set; } = "";

    /// <summary>
    /// Current duration in milliseconds for the output
    /// </summary>
    [JsonPropertyName("outputDuration")]
    public long OutputDuration { get; set; }

    /// <summary>
    /// Congestion of the output
    /// </summary>
    [JsonPropertyName("outputCongestion")]
    public double OutputCongestion { get; set; }

    /// <summary>
    /// Number of bytes sent by the output
    /// </summary>
    [JsonPropertyName("outputBytes")]
    public long OutputBytes { get; set; }

    /// <summary>
    /// Number of frames skipped by the output's process
    /// </summary>
    [JsonPropertyName("outputSkippedFrames")]
    public long OutputSkippedFrames { get; set; }

    /// <summary>
    /// Total number of frames delivered by the output's process
    /// </summary>
    [JsonPropertyName("outputTotalFrames")]
    public long OutputTotalFrames { get; set; }
  }

  /// <summary>
  /// Toggles the status of an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class ToggleOutput : Request<ToggleOutputResponse> {

    /// <summary>
    /// Output name
    /// </summary>
    [JsonPropertyName("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Response of ToggleOutput
  /// </summary>
  public class ToggleOutputResponse : RequestResponse {

    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class StartOutput : Request<RequestResponse> {

    /// <summary>
    /// Output name
    /// </summary>
    [JsonPropertyName("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Stops an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class StopOutput : Request<RequestResponse> {

    /// <summary>
    /// Output name
    /// </summary>
    [JsonPropertyName("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Gets the settings of an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetOutputSettings : Request<GetOutputSettingsResponse> {

    /// <summary>
    /// Output name
    /// </summary>
    [JsonPropertyName("outputName")]
    public string OutputName { get; set; } = "";
  }

  /// <summary>
  /// Response of GetOutputSettings
  /// </summary>
  public class GetOutputSettingsResponse : RequestResponse {

    /// <summary>
    /// Output settings
    /// </summary>
    [JsonPropertyName("outputSettings")]
    public Dictionary<string, JsonElement?> OutputSettings { get; set; } = [];
  }

  /// <summary>
  /// Sets the settings of an output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetOutputSettings : Request<RequestResponse> {

    /// <summary>
    /// Output name
    /// </summary>
    [JsonPropertyName("outputName")]
    public string OutputName { get; set; } = "";

    /// <summary>
    /// Output settings
    /// </summary>
    [JsonPropertyName("outputSettings")]
    public Dictionary<string, JsonElement?> OutputSettings { get; set; } = [];
  }

  /// <summary>
  /// Gets the status of the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetStreamStatus : Request<GetStreamStatusResponse> { }

  /// <summary>
  /// Response of GetStreamStatus
  /// </summary>
  public class GetStreamStatusResponse : RequestResponse {

    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// Whether the output is currently reconnecting
    /// </summary>
    [JsonPropertyName("outputReconnecting")]
    public bool OutputReconnecting { get; set; }

    /// <summary>
    /// Current formatted timecode string for the output
    /// </summary>
    [JsonPropertyName("outputTimecode")]
    public string OutputTimecode { get; set; } = "";

    /// <summary>
    /// Current duration in milliseconds for the output
    /// </summary>
    [JsonPropertyName("outputDuration")]
    public long OutputDuration { get; set; }

    /// <summary>
    /// Congestion of the output
    /// </summary>
    [JsonPropertyName("outputCongestion")]
    public double OutputCongestion { get; set; }

    /// <summary>
    /// Number of bytes sent by the output
    /// </summary>
    [JsonPropertyName("outputBytes")]
    public long OutputBytes { get; set; }

    /// <summary>
    /// Number of frames skipped by the output's process
    /// </summary>
    [JsonPropertyName("outputSkippedFrames")]
    public long OutputSkippedFrames { get; set; }

    /// <summary>
    /// Total number of frames delivered by the output's process
    /// </summary>
    [JsonPropertyName("outputTotalFrames")]
    public long OutputTotalFrames { get; set; }
  }

  /// <summary>
  /// Toggles the status of the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
#pragma warning disable CA1711
  public class ToggleStream : Request<ToggleStreamResponse> { }
#pragma warning restore CA1711

  /// <summary>
  /// Response of ToggleStream
  /// </summary>
  public class ToggleStreamResponse : RequestResponse {

    /// <summary>
    /// New state of the stream output
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
#pragma warning disable CA1711
  public class StartStream : Request<RequestResponse> { }
#pragma warning restore CA1711

  /// <summary>
  /// Stops the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
#pragma warning disable CA1711
  public class StopStream : Request<RequestResponse> { }
#pragma warning restore CA1711

  /// <summary>
  /// Sends CEA-608 caption text over the stream output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SendStreamCaption : Request<RequestResponse> {

    /// <summary>
    /// Caption text
    /// </summary>
    [JsonPropertyName("captionText")]
    public string CaptionText { get; set; } = "";
  }

  /// <summary>
  /// Gets the status of the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetRecordStatus : Request<GetRecordStatusResponse> { }

  /// <summary>
  /// Response of GetRecordStatus
  /// </summary>
  public class GetRecordStatusResponse : RequestResponse {

    /// <summary>
    /// Whether the output is active
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }

    /// <summary>
    /// Whether the output is paused
    /// </summary>
    [JsonPropertyName("outputPaused")]
    public bool OutputPaused { get; set; }

    /// <summary>
    /// Current formatted timecode string for the output
    /// </summary>
    [JsonPropertyName("outputTimecode")]
    public string OutputTimecode { get; set; } = "";

    /// <summary>
    /// Current duration in milliseconds for the output
    /// </summary>
    [JsonPropertyName("outputDuration")]
    public long OutputDuration { get; set; }

    /// <summary>
    /// Number of bytes sent by the output
    /// </summary>
    [JsonPropertyName("outputBytes")]
    public long OutputBytes { get; set; }
  }

  /// <summary>
  /// Toggles the status of the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class ToggleRecord : Request<ToggleRecordResponse> { }

  /// <summary>
  /// Response of ToggleRecord
  /// </summary>
  public class ToggleRecordResponse : RequestResponse {

    /// <summary>
    /// The new active state of the output
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool OutputActive { get; set; }
  }

  /// <summary>
  /// Starts the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class StartRecord : Request<RequestResponse> { }

  /// <summary>
  /// Stops the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class StopRecord : Request<StopRecordResponse> { }

  /// <summary>
  /// Response of StopRecord
  /// </summary>
  public class StopRecordResponse : RequestResponse {

    /// <summary>
    /// File name for the saved recording
    /// </summary>
    [JsonPropertyName("outputPath")]
    public string OutputPath { get; set; } = "";
  }

  /// <summary>
  /// Toggles pause on the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class ToggleRecordPause : Request<ToggleRecordPauseResponse> { }

  /// <summary>
  /// Response of ToggleRecordPause
  /// </summary>
  public class ToggleRecordPauseResponse : RequestResponse {

    /// <summary>
    /// Whether the output is paused
    /// </summary>
    [JsonPropertyName("outputPaused")]
    public bool OutputPaused { get; set; }
  }

  /// <summary>
  /// Pauses the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class PauseRecord : Request<RequestResponse> { }

  /// <summary>
  /// Resumes the record output.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class ResumeRecord : Request<RequestResponse> { }

  /// <summary>
  /// Splits the current file being recorded into a new file.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.5.0
  /// </summary>
  public class SplitRecordFile : Request<RequestResponse> { }

  /// <summary>
  /// Adds a new chapter marker to the file currently being recorded.<br />
  /// <br />
  /// Note: As of OBS 30.2.0, the only file format supporting this feature is Hybrid MP4.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.5.0
  /// </summary>
  public class CreateRecordChapter : Request<RequestResponse> {

    /// <summary>
    /// Name of the new chapter<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("chapterName")]
    public string? ChapterName { get; set; }
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
  public class GetMediaInputStatus : Request<GetMediaInputStatusResponse> {

    /// <summary>
    /// Name of the media input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the media input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Response of GetMediaInputStatus
  /// </summary>
  public class GetMediaInputStatusResponse : RequestResponse {

    /// <summary>
    /// State of the media input
    /// </summary>
    [JsonPropertyName("mediaState")]
    public MediaState MediaState { get; set; } = new();

    /// <summary>
    /// Total duration of the playing media in milliseconds. <c>null</c> if not playing
    /// </summary>
    [JsonPropertyName("mediaDuration")]
    public long? MediaDuration { get; set; }

    /// <summary>
    /// Position of the cursor in milliseconds. <c>null</c> if not playing
    /// </summary>
    [JsonPropertyName("mediaCursor")]
    public int? MediaCursor { get; set; }
  }

  /// <summary>
  /// Sets the cursor position of a media input.<br />
  /// <br />
  /// This request does not perform bounds checking of the cursor position.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetMediaInputCursor : Request<RequestResponse> {

    /// <summary>
    /// New cursor position to set<br />
    /// It should be &gt;= 0
    /// </summary>
    [JsonPropertyName("mediaCursor")]
    public double MediaCursor { get; set; }

    /// <summary>
    /// Name of the media input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the media input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Offsets the current cursor position of a media input by the specified value.<br />
  /// <br />
  /// This request does not perform bounds checking of the cursor position.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class OffsetMediaInputCursor : Request<RequestResponse> {

    /// <summary>
    /// Value to offset the current cursor position by
    /// </summary>
    [JsonPropertyName("mediaCursorOffset")]
    public int MediaCursorOffset { get; set; }

    /// <summary>
    /// Name of the media input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the media input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Triggers an action on a media input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class TriggerMediaInputAction : Request<RequestResponse> {

    /// <summary>
    /// Identifier of the <c>ObsMediaInputAction</c> enum
    /// </summary>
    [JsonPropertyName("mediaAction")]
    public MediaInputAction MediaAction { get; set; } = new();

    /// <summary>
    /// Name of the media input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the media input<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets whether studio is enabled.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetStudioModeEnabled : Request<GetStudioModeEnabledResponse> { }

  /// <summary>
  /// Response of GetStudioModeEnabled
  /// </summary>
  public class GetStudioModeEnabledResponse : RequestResponse {

    /// <summary>
    /// Whether studio mode is enabled
    /// </summary>
    [JsonPropertyName("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }

  /// <summary>
  /// Enables or disables studio mode<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class SetStudioModeEnabled : Request<RequestResponse> {

    /// <summary>
    /// True == Enabled, False == Disabled
    /// </summary>
    [JsonPropertyName("studioModeEnabled")]
    public bool StudioModeEnabled { get; set; }
  }

  /// <summary>
  /// Opens the properties dialog of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class OpenInputPropertiesDialog : Request<RequestResponse> {

    /// <summary>
    /// Name of the input to open the dialog of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to open the dialog of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Opens the filters dialog of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class OpenInputFiltersDialog : Request<RequestResponse> {

    /// <summary>
    /// Name of the input to open the dialog of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to open the dialog of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Opens the interact dialog of an input.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class OpenInputInteractDialog : Request<RequestResponse> {

    /// <summary>
    /// Name of the input to open the dialog of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputName")]
    public string? InputName { get; set; }

    /// <summary>
    /// UUID of the input to open the dialog of<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("inputUuid")]
    public string? InputUuid { get; set; }
  }

  /// <summary>
  /// Gets a list of connected monitors and information about them.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class GetMonitorList : Request<GetMonitorListResponse> { }

  /// <summary>
  /// Response of GetMonitorList
  /// </summary>
  public class GetMonitorListResponse : RequestResponse {

    /// <summary>
    /// a list of detected monitors with some information
    /// </summary>
    [JsonPropertyName("monitors")]
    public List<Dictionary<string, JsonElement?>> Monitors { get; set; } = [];
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
  public class OpenVideoMixProjector : Request<RequestResponse> {

    /// <summary>
    /// Type of mix to open
    /// </summary>
    [JsonPropertyName("videoMixType")]
    public VideoMixType VideoMixType { get; set; } = new();

    /// <summary>
    /// Monitor index, use <c>GetMonitorList</c> to obtain index<br />
    /// If null, -1: Opens projector in windowed mode
    /// </summary>
    [JsonPropertyName("monitorIndex")]
    public int? MonitorIndex { get; set; }

    /// <summary>
    /// Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with <c>monitorIndex</c><br />
    /// If null, N/A
    /// </summary>
    [JsonPropertyName("projectorGeometry")]
    public string? ProjectorGeometry { get; set; }
  }

  /// <summary>
  /// Opens a projector for a source.<br />
  /// <br />
  /// Note: This request serves to provide feature parity with 4.x. It is very likely to be changed/deprecated in a future release.<br />
  /// Latest supported RPC version: 1<br />
  /// Added in: 5.0.0
  /// </summary>
  public class OpenSourceProjector : Request<RequestResponse> {

    /// <summary>
    /// Name of the source to open a projector for<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    /// <summary>
    /// UUID of the source to open a projector for<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("sourceUuid")]
    public string? SourceUuid { get; set; }

    /// <summary>
    /// Monitor index, use <c>GetMonitorList</c> to obtain index<br />
    /// If null, -1: Opens projector in windowed mode
    /// </summary>
    [JsonPropertyName("monitorIndex")]
    public int? MonitorIndex { get; set; }

    /// <summary>
    /// Size/Position data for a windowed projector, in Qt Base64 encoded format. Mutually exclusive with <c>monitorIndex</c><br />
    /// If null, N/A
    /// </summary>
    [JsonPropertyName("projectorGeometry")]
    public string? ProjectorGeometry { get; set; }

    /// <summary>
    /// UUID of the canvas the source is in, if using the sourceName field<br />
    /// If null, Unknown
    /// </summary>
    [JsonPropertyName("canvasUuid")]
    public string? CanvasUuid { get; set; }
  }
}
