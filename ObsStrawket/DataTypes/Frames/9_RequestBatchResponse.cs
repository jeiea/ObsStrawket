using System.Collections.Generic;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: Identified client which made the request<br />
  /// Description: obs-websocket is responding to a request batch coming from the client.
  /// </summary>
  public interface IRequestBatchResponse : IOpCodeMessage {
    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// Responses in same order.
    /// </summary>
    public List<IRequestResponse> Results { get; }
  }

  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: Identified client which made the request<br />
  /// Description: obs-websocket is responding to a request batch coming from the client.
  /// </summary>
  public class RequestBatchResponse : IRequestBatchResponse {
    /// <summary>
    /// <see cref="OpCode.RequestBatchResponse"/> (OpCode 9)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.RequestBatchResponse;

    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    [Key("requestId")]
    public string RequestId { get; set; } = "";

    /// <summary>
    /// Responses in same order.
    /// </summary>
    [Key("results")]
    public List<IRequestResponse> Results { get; set; } = new();
  }
}

