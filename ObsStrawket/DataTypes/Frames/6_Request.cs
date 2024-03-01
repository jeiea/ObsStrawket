using System.Collections.Generic;
using System.Text.Json.Serialization;

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
  public class Request : IRequest {

    /// <summary>
    /// Request (OpCode 6)
    /// </summary>
    [JsonIgnore]
    public OpCode Op => OpCode.Request;

    /// <summary>
    /// The string identifying request type.
    /// </summary>
    [JsonIgnore]
    public string RequestType { get => GetType().Name; }

    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    [JsonIgnore]
    public string RequestId { get; set; } = "";
  }

  /// <summary>
  /// Sent from: Identified client<br />
  /// Sent to: obs-websocket<br />
  /// Description: Client is making a request to obs-websocket.Eg get current scene, create source.
  /// </summary>
  public class RawRequest : IRequest {

    /// <summary>
    /// Request (OpCode 6)
    /// </summary>
    [JsonIgnore]
    public OpCode Op => OpCode.Request;

    /// <summary>
    /// The string identifying request type.
    /// </summary>
    [JsonPropertyName("requestType")]
    public string RequestType { get; set; } = "";

    /// <summary>
    /// Unique identifier for response matching.
    /// </summary>
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = "";

    /// <summary>
    /// Raw request data.
    /// </summary>
    [JsonPropertyName("requestData")]
    public Dictionary<string, object?>? RequestData { get; set; }
  }
}
