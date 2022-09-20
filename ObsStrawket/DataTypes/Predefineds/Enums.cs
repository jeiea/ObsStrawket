using MessagePack.Formatters;
using MessagePack;
using System;
using System.Runtime.Serialization;

namespace ObsStrawket.DataTypes.Predefineds {
  /// <summary>
  /// Flag specifying which events to subscribe to.
  /// </summary>
  [Flags]
  public enum EventSubscription {
    /// <summary>
    /// Subcription value used to disable all events.
    /// </summary>
    [EnumMember(Value = "None")]
    None = 0,

    /// <summary>
    /// Subscription value to receive events in the <c>General</c> category.
    /// </summary>
    [EnumMember(Value = "General")]
    General = 1 << 0,

    /// <summary>
    /// Subscription value to receive events in the <c>Config</c> category.
    /// </summary>
    [EnumMember(Value = "Config")]
    Config = 1 << 1,

    /// <summary>
    /// Subscription value to receive events in the <c>Scenes</c> category.
    /// </summary>
    [EnumMember(Value = "Scenes")]
    Scenes = 1 << 2,

    /// <summary>
    /// Subscription value to receive events in the <c>Inputs</c> category.
    /// </summary>
    [EnumMember(Value = "Inputs")]
    Inputs = 1 << 3,

    /// <summary>
    /// Subscription value to receive events in the <c>Transitions</c> category.
    /// </summary>
    [EnumMember(Value = "Transitions")]
    Transitions = 1 << 4,

    /// <summary>
    /// Subscription value to receive events in the <c>Filters</c> category.
    /// </summary>
    [EnumMember(Value = "Filters")]
    Filters = 1 << 5,

    /// <summary>
    /// Subscription value to receive events in the <c>Outputs</c> category.
    /// </summary>
    [EnumMember(Value = "Outputs")]
    Outputs = 1 << 6,

    /// <summary>
    /// Subscription value to receive events in the <c>SceneItems</c> category.
    /// </summary>
    [EnumMember(Value = "SceneItems")]
    SceneItems = 1 << 7,

    /// <summary>
    /// Subscription value to receive events in the <c>MediaInputs</c> category.
    /// </summary>
    [EnumMember(Value = "MediaInputs")]
    MediaInputs = 1 << 8,

    /// <summary>
    /// Subscription value to receive the <c>VendorEvent</c> event.
    /// </summary>
    [EnumMember(Value = "Vendors")]
    Vendors = 1 << 9,

    /// <summary>
    /// Subscription value to receive events in the <c>Ui</c> category.
    /// </summary>
    [EnumMember(Value = "Ui")]
    Ui = 1 << 10,

    /// <summary>
    /// Helper to receive all non-high-volume events.
    /// </summary>
    [EnumMember(Value = "All")]
    All = General | Config | Scenes | Inputs | Transitions | Filters | Outputs | SceneItems | MediaInputs | Vendors | Ui,

    /// <summary>
    /// Subscription value to receive the <c>InputVolumeMeters</c> high-volume event.
    /// </summary>
    [EnumMember(Value = "InputVolumeMeters")]
    InputVolumeMeters = 1 << 16,

    /// <summary>
    /// Subscription value to receive the <c>InputActiveStateChanged</c> high-volume event.
    /// </summary>
    [EnumMember(Value = "InputActiveStateChanged")]
    InputActiveStateChanged = 1 << 17,

    /// <summary>
    /// Subscription value to receive the <c>InputShowStateChanged</c> high-volume event.
    /// </summary>
    [EnumMember(Value = "InputShowStateChanged")]
    InputShowStateChanged = 1 << 18,

    /// <summary>
    /// Subscription value to receive the <c>SceneItemTransformChanged</c> high-volume event.
    /// </summary>
    [EnumMember(Value = "SceneItemTransformChanged")]
    SceneItemTransformChanged = 1 << 19,
  }

  /// <summary>
  /// Type of method to process request
  /// </summary>
  public enum RequestBatchExecutionType {
    /// <summary>
    /// Not a request batch.
    /// </summary>
    [EnumMember(Value = "None")]
    None = -1,

    /// <summary>
    /// A request batch which processes all requests serially, as fast as possible.<br />
    /// <br />
    /// Note: To introduce artificial delay, use the <c>Sleep</c> request and the <c>sleepMillis</c> request field.
    /// </summary>
    [EnumMember(Value = "SerialRealtime")]
    SerialRealtime = 0,

    /// <summary>
    /// A request batch type which processes all requests serially, in sync with the graphics thread. Designed to provide high accuracy for animations.<br />
    /// <br />
    /// Note: To introduce artificial delay, use the <c>Sleep</c> request and the <c>sleepFrames</c> request field.
    /// </summary>
    [EnumMember(Value = "SerialFrame")]
    SerialFrame = 1,

    /// <summary>
    /// A request batch type which processes all requests using all available threads in the thread pool.<br />
    /// <br />
    /// Note: This is mainly experimental, and only really shows its colors during requests which require lots of<br />
    /// active processing, like <c>GetSourceScreenshot</c>.
    /// </summary>
    [EnumMember(Value = "Parallel")]
    Parallel = 2,
  }

  /// <summary>
  /// Request result code
  /// </summary>
  public enum RequestStatus {
    /// <summary>
    /// Unknown status, should never be used.
    /// </summary>
    [EnumMember(Value = "Unknown")]
    Unknown = 0,

    /// <summary>
    /// For internal use to signify a successful field check.
    /// </summary>
    [EnumMember(Value = "NoError")]
    NoError = 10,

    /// <summary>
    /// The request has succeeded.
    /// </summary>
    [EnumMember(Value = "Success")]
    Success = 100,

    /// <summary>
    /// The <c>requestType</c> field is missing from the request data.
    /// </summary>
    [EnumMember(Value = "MissingRequestType")]
    MissingRequestType = 203,

    /// <summary>
    /// The request type is invalid or does not exist.
    /// </summary>
    [EnumMember(Value = "UnknownRequestType")]
    UnknownRequestType = 204,

    /// <summary>
    /// Generic error code.<br />
    /// <br />
    /// Note: A comment is required to be provided by obs-websocket.
    /// </summary>
    [EnumMember(Value = "GenericError")]
    GenericError = 205,

    /// <summary>
    /// The request batch execution type is not supported.
    /// </summary>
    [EnumMember(Value = "UnsupportedRequestBatchExecutionType")]
    UnsupportedRequestBatchExecutionType = 206,

    /// <summary>
    /// A required request field is missing.
    /// </summary>
    [EnumMember(Value = "MissingRequestField")]
    MissingRequestField = 300,

    /// <summary>
    /// The request does not have a valid requestData object.
    /// </summary>
    [EnumMember(Value = "MissingRequestData")]
    MissingRequestData = 301,

    /// <summary>
    /// Generic invalid request field message.<br />
    /// <br />
    /// Note: A comment is required to be provided by obs-websocket.
    /// </summary>
    [EnumMember(Value = "InvalidRequestField")]
    InvalidRequestField = 400,

    /// <summary>
    /// A request field has the wrong data type.
    /// </summary>
    [EnumMember(Value = "InvalidRequestFieldType")]
    InvalidRequestFieldType = 401,

    /// <summary>
    /// A request field (number) is outside of the allowed range.
    /// </summary>
    [EnumMember(Value = "RequestFieldOutOfRange")]
    RequestFieldOutOfRange = 402,

    /// <summary>
    /// A request field (string or array) is empty and cannot be.
    /// </summary>
    [EnumMember(Value = "RequestFieldEmpty")]
    RequestFieldEmpty = 403,

    /// <summary>
    /// There are too many request fields (eg. a request takes two optionals, where only one is allowed at a time).
    /// </summary>
    [EnumMember(Value = "TooManyRequestFields")]
    TooManyRequestFields = 404,

    /// <summary>
    /// An output is running and cannot be in order to perform the request.
    /// </summary>
    [EnumMember(Value = "OutputRunning")]
    OutputRunning = 500,

    /// <summary>
    /// An output is not running and should be.
    /// </summary>
    [EnumMember(Value = "OutputNotRunning")]
    OutputNotRunning = 501,

    /// <summary>
    /// An output is paused and should not be.
    /// </summary>
    [EnumMember(Value = "OutputPaused")]
    OutputPaused = 502,

    /// <summary>
    /// An output is not paused and should be.
    /// </summary>
    [EnumMember(Value = "OutputNotPaused")]
    OutputNotPaused = 503,

    /// <summary>
    /// An output is disabled and should not be.
    /// </summary>
    [EnumMember(Value = "OutputDisabled")]
    OutputDisabled = 504,

    /// <summary>
    /// Studio mode is active and cannot be.
    /// </summary>
    [EnumMember(Value = "StudioModeActive")]
    StudioModeActive = 505,

    /// <summary>
    /// Studio mode is not active and should be.
    /// </summary>
    [EnumMember(Value = "StudioModeNotActive")]
    StudioModeNotActive = 506,

    /// <summary>
    /// The resource was not found.<br />
    /// <br />
    /// Note: Resources are any kind of object in obs-websocket, like inputs, profiles, outputs, etc.
    /// </summary>
    [EnumMember(Value = "ResourceNotFound")]
    ResourceNotFound = 600,

    /// <summary>
    /// The resource already exists.
    /// </summary>
    [EnumMember(Value = "ResourceAlreadyExists")]
    ResourceAlreadyExists = 601,

    /// <summary>
    /// The type of resource found is invalid.
    /// </summary>
    [EnumMember(Value = "InvalidResourceType")]
    InvalidResourceType = 602,

    /// <summary>
    /// There are not enough instances of the resource in order to perform the request.
    /// </summary>
    [EnumMember(Value = "NotEnoughResources")]
    NotEnoughResources = 603,

    /// <summary>
    /// The state of the resource is invalid. For example, if the resource is blocked from being accessed.
    /// </summary>
    [EnumMember(Value = "InvalidResourceState")]
    InvalidResourceState = 604,

    /// <summary>
    /// The specified input (obs_source_t-OBS_SOURCE_TYPE_INPUT) had the wrong kind.
    /// </summary>
    [EnumMember(Value = "InvalidInputKind")]
    InvalidInputKind = 605,

    /// <summary>
    /// The resource does not support being configured.<br />
    /// <br />
    /// This is particularly relevant to transitions, where they do not always have changeable settings.
    /// </summary>
    [EnumMember(Value = "ResourceNotConfigurable")]
    ResourceNotConfigurable = 606,

    /// <summary>
    /// The specified filter (obs_source_t-OBS_SOURCE_TYPE_FILTER) had the wrong kind.
    /// </summary>
    [EnumMember(Value = "InvalidFilterKind")]
    InvalidFilterKind = 607,

    /// <summary>
    /// Creating the resource failed.
    /// </summary>
    [EnumMember(Value = "ResourceCreationFailed")]
    ResourceCreationFailed = 700,

    /// <summary>
    /// Performing an action on the resource failed.
    /// </summary>
    [EnumMember(Value = "ResourceActionFailed")]
    ResourceActionFailed = 701,

    /// <summary>
    /// Processing the request failed unexpectedly.<br />
    /// <br />
    /// Note: A comment is required to be provided by obs-websocket.
    /// </summary>
    [EnumMember(Value = "RequestProcessingFailed")]
    RequestProcessingFailed = 702,

    /// <summary>
    /// The combination of request fields cannot be used to perform an action.
    /// </summary>
    [EnumMember(Value = "CannotAct")]
    CannotAct = 703,
  }

  /// <summary>
  /// 
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<ObsMediaInputAction>))]
  public enum ObsMediaInputAction {
    /// <summary>
    /// No action.
    /// </summary>
    [Obsolete]
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_NONE")]
    OBS_WEBSOCKET_MEDIA_INPUT_ACTION_NONE,

    /// <summary>
    /// Play the media input.
    /// </summary>
    [Obsolete]
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY")]
    OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY,

    /// <summary>
    /// Pause the media input.
    /// </summary>
    [Obsolete]
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PAUSE")]
    OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PAUSE,

    /// <summary>
    /// Stop the media input.
    /// </summary>
    [Obsolete]
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_STOP")]
    OBS_WEBSOCKET_MEDIA_INPUT_ACTION_STOP,

    /// <summary>
    /// Restart the media input.
    /// </summary>
    [Obsolete]
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_RESTART")]
    OBS_WEBSOCKET_MEDIA_INPUT_ACTION_RESTART,

    /// <summary>
    /// Go to the next playlist item.
    /// </summary>
    [Obsolete]
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_NEXT")]
    OBS_WEBSOCKET_MEDIA_INPUT_ACTION_NEXT,

    /// <summary>
    /// Go to the previous playlist item.
    /// </summary>
    [Obsolete]
    [EnumMember(Value = "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PREVIOUS")]
    OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PREVIOUS,
  }

  /// <summary>
  /// Reason that OBS closed the connection.
  /// </summary>
  public enum WebSocketCloseCode {
    /// <summary>
    /// For internal use only to tell the request handler not to perform any close action.
    /// </summary>
    [EnumMember(Value = "DontClose")]
    DontClose = 0,

    /// <summary>
    /// Unknown reason, should never be used.
    /// </summary>
    [EnumMember(Value = "UnknownReason")]
    UnknownReason = 4000,

    /// <summary>
    /// The server was unable to decode the incoming websocket message.
    /// </summary>
    [EnumMember(Value = "MessageDecodeError")]
    MessageDecodeError = 4002,

    /// <summary>
    /// A data field is required but missing from the payload.
    /// </summary>
    [EnumMember(Value = "MissingDataField")]
    MissingDataField = 4003,

    /// <summary>
    /// A data field's value type is invalid.
    /// </summary>
    [EnumMember(Value = "InvalidDataFieldType")]
    InvalidDataFieldType = 4004,

    /// <summary>
    /// A data field's value is invalid.
    /// </summary>
    [EnumMember(Value = "InvalidDataFieldValue")]
    InvalidDataFieldValue = 4005,

    /// <summary>
    /// The specified <c>op</c> was invalid or missing.
    /// </summary>
    [EnumMember(Value = "UnknownOpCode")]
    UnknownOpCode = 4006,

    /// <summary>
    /// The client sent a websocket message without first sending <c>Identify</c> message.
    /// </summary>
    [EnumMember(Value = "NotIdentified")]
    NotIdentified = 4007,

    /// <summary>
    /// The client sent an <c>Identify</c> message while already identified.<br />
    /// <br />
    /// Note: Once a client has identified, only <c>Reidentify</c> may be used to change session parameters.
    /// </summary>
    [EnumMember(Value = "AlreadyIdentified")]
    AlreadyIdentified = 4008,

    /// <summary>
    /// The authentication attempt (via <c>Identify</c>) failed.
    /// </summary>
    [EnumMember(Value = "AuthenticationFailed")]
    AuthenticationFailed = 4009,

    /// <summary>
    /// The server detected the usage of an old version of the obs-websocket RPC protocol.
    /// </summary>
    [EnumMember(Value = "UnsupportedRpcVersion")]
    UnsupportedRpcVersion = 4010,

    /// <summary>
    /// The websocket session has been invalidated by the obs-websocket server.<br />
    /// <br />
    /// Note: This is the code used by the <c>Kick</c> button in the UI Session List. If you receive this code, you must not automatically reconnect.
    /// </summary>
    [EnumMember(Value = "SessionInvalidated")]
    SessionInvalidated = 4011,

    /// <summary>
    /// A requested feature is not supported due to hardware/software limitations.
    /// </summary>
    [EnumMember(Value = "UnsupportedFeature")]
    UnsupportedFeature = 4012,
  }

  /// <summary>
  /// Type of OBS websocket protocol message.
  /// </summary>
  public enum WebSocketOpCode {
    /// <summary>
    /// The initial message sent by obs-websocket to newly connected clients.
    /// </summary>
    [EnumMember(Value = "Hello")]
    Hello = 0,

    /// <summary>
    /// The message sent by a newly connected client to obs-websocket in response to a <c>Hello</c>.
    /// </summary>
    [EnumMember(Value = "Identify")]
    Identify = 1,

    /// <summary>
    /// The response sent by obs-websocket to a client after it has successfully identified with obs-websocket.
    /// </summary>
    [EnumMember(Value = "Identified")]
    Identified = 2,

    /// <summary>
    /// The message sent by an already-identified client to update identification parameters.
    /// </summary>
    [EnumMember(Value = "Reidentify")]
    Reidentify = 3,

    /// <summary>
    /// The message sent by obs-websocket containing an event payload.
    /// </summary>
    [EnumMember(Value = "Event")]
    Event = 5,

    /// <summary>
    /// The message sent by a client to obs-websocket to perform a request.
    /// </summary>
    [EnumMember(Value = "Request")]
    Request = 6,

    /// <summary>
    /// The message sent by obs-websocket in response to a particular request from a client.
    /// </summary>
    [EnumMember(Value = "RequestResponse")]
    RequestResponse = 7,

    /// <summary>
    /// The message sent by a client to obs-websocket to perform a batch of requests.
    /// </summary>
    [EnumMember(Value = "RequestBatch")]
    RequestBatch = 8,

    /// <summary>
    /// The message sent by obs-websocket in response to a particular batch of requests from a client.
    /// </summary>
    [EnumMember(Value = "RequestBatchResponse")]
    RequestBatchResponse = 9,
  }
}
