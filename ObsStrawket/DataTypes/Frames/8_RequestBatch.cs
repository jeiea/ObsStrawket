using ObsStrawket.DataTypes.Predefineds;
using System.Collections.Generic;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Sent from: Identified client<br />
  /// Sent to: obs-websocket<br />
  /// Description: Client is making a batch of requests for obs-websocket.
  /// Requests are processed serially (in order) by the server.
  /// </summary>
  public interface IRequestBatch : IOpCodeMessage {
    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// When <c>haltOnFailure</c> is true, the processing of requests will be halted on first failure.
    /// Returns only the processed requests in RequestBatchResponse
    /// </summary>
    public bool? HaltOnFailure { get; }

    /// <summary>
    /// Default: <see cref="RequestBatchExecutionType.SerialRealtime"/>
    /// </summary>
    public RequestBatchExecutionType ExecutionType { get; }

    /// <summary>
    /// Requests in the requests array follow the same structure as the Request payload data format,
    /// however requestId is an optional field.
    /// </summary>
    public List<IRequest> Requests { get; }
  }

  /// <summary>
  /// Sent from: Identified client<br />
  /// Sent to: obs-websocket<br />
  /// Description: Client is making a batch of requests for obs-websocket.
  /// Requests are processed serially (in order) by the server.
  /// </summary>
  public class RequestBatch : IRequestBatch {
    /// <summary>
    /// <see cref="OpCode.RequestBatch"/> (OpCode 8)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.RequestBatch;

    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    [Key("requestId")]
    public string RequestId { get; set; } = "";

    /// <summary>
    /// When <c>haltOnFailure</c> is true, the processing of requests will be halted on first failure.
    /// Returns only the processed requests in RequestBatchResponse
    /// </summary>
    [Key("haltOnFailure")]
    public bool? HaltOnFailure { get; set; } = null;

    /// <summary>
    /// Default: <see cref="RequestBatchExecutionType.SerialRealtime"/>
    /// </summary>
    [Key("executionType")]
    public RequestBatchExecutionType ExecutionType { get; set; } = RequestBatchExecutionType.SerialRealtime;

    /// <summary>
    /// Requests in the requests array follow the same structure as the Request payload data format,
    /// however requestId is an optional field.
    /// </summary>
    [Key("requests")]
    public List<IRequest> Requests { get; set; } = new();
  }
}

