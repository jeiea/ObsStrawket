namespace ObsStrawket.DataTypes {
  /*
# RequestStatusCode generation
input: https://github.com/obsproject/obs-websocket/blob/master/docs/generated/protocol.md#enumerations-table-of-contents
regex:
RequestStatus::(\w+)\n([\s\S]*?)\nIdentifier Value: (\d+)\n([\s\S]*?)(?=RequestStatus::)
///<summary>
///$2
///$4
///</summary>
$1 = $3,
   */
  public enum RequestStatusCode {
    /// <summary>
    /// Unknown status, should never be used.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// For internal use to signify a successful field check.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    NoError = 10,
    /// <summary>
    /// The request has succeeded.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    Success = 100,
    /// <summary>
    /// The requestType field is missing from the request data.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    MissingRequestType = 203,
    /// <summary>
    /// The request type is invalid or does not exist.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    UnknownRequestType = 204,
    /// <summary>
    /// Generic error code.<br />
    /// Note:A comment is required to be provided by obs-websocket.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    GenericError = 205,
    /// <summary>
    /// The request batch execution type is not supported.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    UnsupportedRequestBatchExecutionType = 206,
    /// <summary>
    /// A required request field is missing.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    MissingRequestField = 300,
    /// <summary>
    /// The request does not have a valid requestData object.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    MissingRequestData = 301,
    /// <summary>
    /// Generic invalid request field message.<br />
    /// Note:A comment is required to be provided by obs-websocket.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    InvalidRequestField = 400,
    /// <summary>
    /// A request field has the wrong data type.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    InvalidRequestFieldType = 401,
    /// <summary>
    /// A request field (number) is outside of the allowed range.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    RequestFieldOutOfRange = 402,
    /// <summary>
    /// A request field (string or array) is empty and cannot be.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    RequestFieldEmpty = 403,
    /// <summary>
    /// There are too many request fields (eg. a request takes two optionals, where only one is allowed at a time).<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    TooManyRequestFields = 404,
    /// <summary>
    /// An output is running and cannot be in order to perform the request.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    OutputRunning = 500,
    /// <summary>
    /// An output is not running and should be.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    OutputNotRunning = 501,
    /// <summary>
    /// An output is paused and should not be.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    OutputPaused = 502,
    /// <summary>
    /// An output is not paused and should be.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    OutputNotPaused = 503,
    /// <summary>
    /// An output is disabled and should not be.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    OutputDisabled = 504,
    /// <summary>
    /// Studio mode is active and cannot be.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    StudioModeActive = 505,
    /// <summary>
    /// Studio mode is not active and should be.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    StudioModeNotActive = 506,
    /// <summary>
    /// The resource was not found.<br />
    /// Note:Resources are any kind of object in obs-websocket, like inputs, profiles, outputs, etc.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    ResourceNotFound = 600,
    /// <summary>
    /// The resource already exists.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    ResourceAlreadyExists = 601,
    /// <summary>
    /// The type of resource found is invalid.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    InvalidResourceType = 602,
    /// <summary>
    /// There are not enough instances of the resource in order to perform the request.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    NotEnoughResources = 603,
    /// <summary>
    /// The state of the resource is invalid. For example, if the resource is blocked from being accessed.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    InvalidResourceState = 604,
    /// <summary>
    /// The specified input (obs_source_t-OBS_SOURCE_TYPE_INPUT) had the wrong kind.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    InvalidInputKind = 605,
    /// <summary>
    /// The resource does not support being configured.<br />
    /// This is particularly relevant to transitions, where they do not always have changeable settings.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    ResourceNotConfigurable = 606,
    /// <summary>
    /// The specified filter (obs_source_t-OBS_SOURCE_TYPE_FILTER) had the wrong kind.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    InvalidFilterKind = 607,
    /// <summary>
    /// Creating the resource failed.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    ResourceCreationFailed = 700,
    /// <summary>
    /// Performing an action on the resource failed.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    ResourceActionFailed = 701,
    /// <summary>
    /// Processing the request failed unexpectedly.<br />
    /// Note:A comment is required to be provided by obs-websocket.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    RequestProcessingFailed = 702,
    /// <summary>
    /// The combination of request fields cannot be used to perform an action.<br />
    /// Latest Supported RPC Version: 1<br />
    /// Added in v5.0.0
    /// </summary>
    CannotAct = 703,
  }
}
