using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ObsStrawket.DataTypes.Predefineds {

  /// <summary>
  /// Flag specifying which events to subscribe to.
  /// </summary>
  [Flags]
  public enum EventSubscription {

    /// <summary>
    /// <para>Subcription value used to disable all events.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    None = 0,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>General</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    General = 1 << 0,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>Config</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Config = 1 << 1,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>Scenes</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Scenes = 1 << 2,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>Inputs</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Inputs = 1 << 3,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>Transitions</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Transitions = 1 << 4,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>Filters</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Filters = 1 << 5,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>Outputs</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Outputs = 1 << 6,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>SceneItems</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    SceneItems = 1 << 7,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>MediaInputs</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    MediaInputs = 1 << 8,

    /// <summary>
    /// <para>Subscription value to receive the <c>VendorEvent</c> event.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Vendors = 1 << 9,

    /// <summary>
    /// <para>Subscription value to receive events in the <c>Ui</c> category.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Ui = 1 << 10,

    /// <summary>
    /// <para>Helper to receive all non-high-volume events.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    All = General | Config | Scenes | Inputs | Transitions | Filters | Outputs | SceneItems | MediaInputs | Vendors | Ui,

    /// <summary>
    /// <para>Subscription value to receive the <c>InputVolumeMeters</c> high-volume event.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InputVolumeMeters = 1 << 16,

    /// <summary>
    /// <para>Subscription value to receive the <c>InputActiveStateChanged</c> high-volume event.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InputActiveStateChanged = 1 << 17,

    /// <summary>
    /// <para>Subscription value to receive the <c>InputShowStateChanged</c> high-volume event.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InputShowStateChanged = 1 << 18,

    /// <summary>
    /// <para>Subscription value to receive the <c>SceneItemTransformChanged</c> high-volume event.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    SceneItemTransformChanged = 1 << 19,
  }

  /// <summary>
  /// Type of method to process request
  /// </summary>
  public enum RequestBatchExecutionType {

    /// <summary>
    /// <para>Not a request batch.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    None = -1,

    /// <summary>
    /// <para>A request batch which processes all requests serially, as fast as possible.<br />
    /// <br />
    /// Note: To introduce artificial delay, use the <c>Sleep</c> request and the <c>sleepMillis</c> request field.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    SerialRealtime = 0,

    /// <summary>
    /// <para>A request batch type which processes all requests serially, in sync with the graphics thread. Designed to provide high accuracy for animations.<br />
    /// <br />
    /// Note: To introduce artificial delay, use the <c>Sleep</c> request and the <c>sleepFrames</c> request field.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    SerialFrame = 1,

    /// <summary>
    /// <para>A request batch type which processes all requests using all available threads in the thread pool.<br />
    /// <br />
    /// Note: This is mainly experimental, and only really shows its colors during requests which require lots of<br />
    /// active processing, like <c>GetSourceScreenshot</c>.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Parallel = 2,
  }

  /// <summary>
  /// Request result code
  /// </summary>
  public enum RequestStatus {

    /// <summary>
    /// <para>Unknown status, should never be used.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Unknown = 0,

    /// <summary>
    /// <para>For internal use to signify a successful field check.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    NoError = 10,

    /// <summary>
    /// <para>The request has succeeded.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Success = 100,

    /// <summary>
    /// <para>The <c>requestType</c> field is missing from the request data.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    MissingRequestType = 203,

    /// <summary>
    /// <para>The request type is invalid or does not exist.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    UnknownRequestType = 204,

    /// <summary>
    /// <para>Generic error code.<br />
    /// <br />
    /// Note: A comment is required to be provided by obs-websocket.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    GenericError = 205,

    /// <summary>
    /// <para>The request batch execution type is not supported.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    UnsupportedRequestBatchExecutionType = 206,

    /// <summary>
    /// <para>The server is not ready to handle the request.<br />
    /// <br />
    /// Note: This usually occurs during OBS scene collection change or exit. Requests may be tried again after a delay if this code is given.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.3.0</para>
    /// </summary>
    [EnumMember]
    NotReady = 207,

    /// <summary>
    /// <para>A required request field is missing.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    MissingRequestField = 300,

    /// <summary>
    /// <para>The request does not have a valid requestData object.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    MissingRequestData = 301,

    /// <summary>
    /// <para>Generic invalid request field message.<br />
    /// <br />
    /// Note: A comment is required to be provided by obs-websocket.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InvalidRequestField = 400,

    /// <summary>
    /// <para>A request field has the wrong data type.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InvalidRequestFieldType = 401,

    /// <summary>
    /// <para>A request field (number) is outside of the allowed range.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    RequestFieldOutOfRange = 402,

    /// <summary>
    /// <para>A request field (string or array) is empty and cannot be.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    RequestFieldEmpty = 403,

    /// <summary>
    /// <para>There are too many request fields (eg. a request takes two optionals, where only one is allowed at a time).</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    TooManyRequestFields = 404,

    /// <summary>
    /// <para>An output is running and cannot be in order to perform the request.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    OutputRunning = 500,

    /// <summary>
    /// <para>An output is not running and should be.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    OutputNotRunning = 501,

    /// <summary>
    /// <para>An output is paused and should not be.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    OutputPaused = 502,

    /// <summary>
    /// <para>An output is not paused and should be.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    OutputNotPaused = 503,

    /// <summary>
    /// <para>An output is disabled and should not be.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    OutputDisabled = 504,

    /// <summary>
    /// <para>Studio mode is active and cannot be.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    StudioModeActive = 505,

    /// <summary>
    /// <para>Studio mode is not active and should be.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    StudioModeNotActive = 506,

    /// <summary>
    /// <para>The resource was not found.<br />
    /// <br />
    /// Note: Resources are any kind of object in obs-websocket, like inputs, profiles, outputs, etc.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    ResourceNotFound = 600,

    /// <summary>
    /// <para>The resource already exists.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    ResourceAlreadyExists = 601,

    /// <summary>
    /// <para>The type of resource found is invalid.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InvalidResourceType = 602,

    /// <summary>
    /// <para>There are not enough instances of the resource in order to perform the request.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    NotEnoughResources = 603,

    /// <summary>
    /// <para>The state of the resource is invalid. For example, if the resource is blocked from being accessed.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InvalidResourceState = 604,

    /// <summary>
    /// <para>The specified input (obs_source_t-OBS_SOURCE_TYPE_INPUT) had the wrong kind.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InvalidInputKind = 605,

    /// <summary>
    /// <para>The resource does not support being configured.<br />
    /// <br />
    /// This is particularly relevant to transitions, where they do not always have changeable settings.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    ResourceNotConfigurable = 606,

    /// <summary>
    /// <para>The specified filter (obs_source_t-OBS_SOURCE_TYPE_FILTER) had the wrong kind.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InvalidFilterKind = 607,

    /// <summary>
    /// <para>Creating the resource failed.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    ResourceCreationFailed = 700,

    /// <summary>
    /// <para>Performing an action on the resource failed.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    ResourceActionFailed = 701,

    /// <summary>
    /// <para>Processing the request failed unexpectedly.<br />
    /// <br />
    /// Note: A comment is required to be provided by obs-websocket.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    RequestProcessingFailed = 702,

    /// <summary>
    /// <para>The combination of request fields cannot be used to perform an action.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    CannotAct = 703,
  }

  /// <summary>
  /// Represents output sending state.
  /// </summary>
  [JsonConverter(typeof(JsonStringEnumMemberConverter))]
  public enum ObsOutputState {

    /// <summary>
    /// <para>Unknown state.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_UNKNOWN")]
    Unknown,

    /// <summary>
    /// <para>The output is starting.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_STARTING")]
    Starting,

    /// <summary>
    /// <para>The input has started.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_STARTED")]
    Started,

    /// <summary>
    /// <para>The output is stopping.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_STOPPING")]
    Stopping,

    /// <summary>
    /// <para>The output has stopped.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_STOPPED")]
    Stopped,

    /// <summary>
    /// <para>The output has disconnected and is reconnecting.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_RECONNECTING")]
    Reconnecting,

    /// <summary>
    /// <para>The output has reconnected successfully.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.1.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_RECONNECTED")]
    Reconnected,

    /// <summary>
    /// <para>The output is now paused.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.1.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_PAUSED")]
    Paused,

    /// <summary>
    /// <para>The output has been resumed (unpaused).</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_OUTPUT_RESUMED")]
    Resumed,
  }

  /// <summary>
  /// Actions used with media source and <c>TriggerMediaInputAction</c>.
  /// </summary>
  [JsonConverter(typeof(JsonStringEnumMemberConverter))]
  public enum MediaInputAction {

    /// <summary>
    /// <para>No action.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_NONE")]
    None,

    /// <summary>
    /// <para>Play the media input.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY")]
    Play,

    /// <summary>
    /// <para>Pause the media input.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PAUSE")]
    Pause,

    /// <summary>
    /// <para>Stop the media input.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_STOP")]
    Stop,

    /// <summary>
    /// <para>Restart the media input.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_RESTART")]
    Restart,

    /// <summary>
    /// <para>Go to the next playlist item.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_NEXT")]
    Next,

    /// <summary>
    /// <para>Go to the previous playlist item.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PREVIOUS")]
    Previous,
  }

  /// <summary>
  /// Reason that OBS closed the connection.
  /// </summary>
  public enum WebSocketCloseCode {

    /// <summary>
    /// <para>For internal use only to tell the request handler not to perform any close action.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    DontClose = 0,

    /// <summary>
    /// <para>Unknown reason, should never be used.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    UnknownReason = 4000,

    /// <summary>
    /// <para>The server was unable to decode the incoming websocket message.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    MessageDecodeError = 4002,

    /// <summary>
    /// <para>A data field is required but missing from the payload.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    MissingDataField = 4003,

    /// <summary>
    /// <para>A data field's value type is invalid.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InvalidDataFieldType = 4004,

    /// <summary>
    /// <para>A data field's value is invalid.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    InvalidDataFieldValue = 4005,

    /// <summary>
    /// <para>The specified <c>op</c> was invalid or missing.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    UnknownOpCode = 4006,

    /// <summary>
    /// <para>The client sent a websocket message without first sending <c>Identify</c> message.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    NotIdentified = 4007,

    /// <summary>
    /// <para>The client sent an <c>Identify</c> message while already identified.<br />
    /// <br />
    /// Note: Once a client has identified, only <c>Reidentify</c> may be used to change session parameters.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    AlreadyIdentified = 4008,

    /// <summary>
    /// <para>The authentication attempt (via <c>Identify</c>) failed.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    AuthenticationFailed = 4009,

    /// <summary>
    /// <para>The server detected the usage of an old version of the obs-websocket RPC protocol.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    UnsupportedRpcVersion = 4010,

    /// <summary>
    /// <para>The websocket session has been invalidated by the obs-websocket server.<br />
    /// <br />
    /// Note: This is the code used by the <c>Kick</c> button in the UI Session List. If you receive this code, you must not automatically reconnect.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    SessionInvalidated = 4011,

    /// <summary>
    /// <para>A requested feature is not supported due to hardware/software limitations.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    UnsupportedFeature = 4012,
  }

  /// <summary>
  /// Type of OBS websocket protocol message.
  /// </summary>
  public enum WebSocketOpCode {

    /// <summary>
    /// <para>The initial message sent by obs-websocket to newly connected clients.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Hello = 0,

    /// <summary>
    /// <para>The message sent by a newly connected client to obs-websocket in response to a <c>Hello</c>.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Identify = 1,

    /// <summary>
    /// <para>The response sent by obs-websocket to a client after it has successfully identified with obs-websocket.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Identified = 2,

    /// <summary>
    /// <para>The message sent by an already-identified client to update identification parameters.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Reidentify = 3,

    /// <summary>
    /// <para>The message sent by obs-websocket containing an event payload.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Event = 5,

    /// <summary>
    /// <para>The message sent by a client to obs-websocket to perform a request.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    Request = 6,

    /// <summary>
    /// <para>The message sent by obs-websocket in response to a particular request from a client.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    RequestResponse = 7,

    /// <summary>
    /// <para>The message sent by a client to obs-websocket to perform a batch of requests.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    RequestBatch = 8,

    /// <summary>
    /// <para>The message sent by obs-websocket in response to a particular batch of requests from a client.</para>
    /// <para>Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0</para>
    /// </summary>
    [EnumMember]
    RequestBatchResponse = 9,
  }
}
