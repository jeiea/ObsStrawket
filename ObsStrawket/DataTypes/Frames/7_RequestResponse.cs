using System.Collections.Generic;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: Identified client which made the request<br />
  /// Description: obs-websocket is responding to a request coming from a client.
  /// </summary>
  public interface IRequestResponse : IOpCodeMessage {
    /// <summary>
    /// Same as the request's one.
    /// </summary>
    public string RequestType { get; }

    /// <summary>
    /// Same as the request's one.
    /// </summary>
    public string RequestId { get; }

    /// <summary>
    /// Result of the request processing.
    /// </summary>
    public RequestStatus RequestStatus { get; }
  }

  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: Identified client which made the request<br />
  /// Description: obs-websocket is responding to a request coming from a client.
  /// </summary>
  public class RequestResponse : IRequestResponse {
    /// <summary>
    /// RequestResponse (OpCode 7)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.RequestResponse;

    /// <summary>
    /// Same as the request's one.
    /// </summary>
    [IgnoreMember]
    public string RequestType { get => GetType().Name; }

    /// <summary>
    /// Same as the request's one.
    /// </summary>
    [IgnoreMember]
    public string RequestId { get; set; } = "";

    /// <summary>
    /// Result of the request processing.
    /// </summary>
    [IgnoreMember]
    public RequestStatus RequestStatus { get; set; } = new();
  }

  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: Identified client which made the request<br />
  /// Description: obs-websocket is responding to a request coming from a client.
  /// </summary>
  public class RawRequestResponse : IRequestResponse {
    /// <summary>
    /// RequestResponse (OpCode 7)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.RequestResponse;

    /// <summary>
    /// Same as the request's one.
    /// </summary>
    [Key("requestType")]
    public string RequestType { get; set; } = "";

    /// <summary>
    /// Same as the request's one.
    /// </summary>
    [Key("requestId")]
    public string RequestId { get; set; } = "";

    /// <summary>
    /// Result of the request processing.
    /// </summary>
    [Key("requestStatus")]
    public RequestStatus RequestStatus { get; set; } = new();

    /// <summary>
    /// Response data.
    /// </summary>
    [Key("responseData")]
    public Dictionary<string, object?>? ResponseData { get; set; }
  }

  /// <summary>
  /// Success example: 
  /// <code>"requestStatus": {
  ///   "result": true,
  ///   "code": 100
  /// }</code>
  /// </summary>
  public class RequestStatus {
    /// <summary>
    /// <c>true</c> if the request resulted in <see cref="RequestStatusCode.Success"/>. False if otherwise.
    /// </summary>
    [Key("result")]
    public bool Result { get; set; }

    /// <summary>
    /// RequestStatus code.
    /// </summary>
    [Key("code")]
    public RequestStatusCode Code { get; set; }

    /// <summary>
    /// This may be provided by the server on errors to offer further details on why a request failed.
    /// </summary>
    [Key("comment")]
    public string? Comment { get; set; }
  }
}
