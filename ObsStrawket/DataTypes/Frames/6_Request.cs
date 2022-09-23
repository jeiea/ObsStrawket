using MessagePack;
using System.Collections.Generic;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Sent from: Identified client<br />
  /// Sent to: obs-websocket<br />
  /// Description: Client is making a request to obs-websocket.Eg get current scene, create source.
  /// </summary>
  public interface IRequest : IOpCodeMessage {
    /// <summary>
    /// The string identifying request type.
    /// </summary>
    public string RequestType { get; }

    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    public string RequestId { get; set; }
  }

  /// <summary>
  /// Sent from: Identified client<br />
  /// Sent to: obs-websocket<br />
  /// Description: Client is making a request to obs-websocket.Eg get current scene, create source.
  /// </summary>
  [MessagePackObject]
  public class Request : IRequest {
    /// <summary>
    /// Request (OpCode 6)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.Request;

    /// <summary>
    /// The string identifying request type.
    /// </summary>
    [IgnoreMember]
    public string RequestType { get => GetType().Name; }

    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    [IgnoreMember]
    public string RequestId { get; set; } = "";
  }

  /// <summary>
  /// Sent from: Identified client<br />
  /// Sent to: obs-websocket<br />
  /// Description: Client is making a request to obs-websocket.Eg get current scene, create source.
  /// </summary>
  [MessagePackObject]
  public class RawRequest : IRequest {
    /// <summary>
    /// Request (OpCode 6)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.Request;

    /// <summary>
    /// The string identifying request type.
    /// </summary>
    [Key("requestType")]
    public string RequestType { get; set; } = "";

    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    [Key("requestId")]
    public string RequestId { get; set; } = "";

    /// <summary>
    /// Raw request data.
    /// </summary>
    [Key("requestData")]
    public Dictionary<string, object?>? RequestData { get; set; }
  }
}
