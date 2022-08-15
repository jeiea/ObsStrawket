namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using System.Collections.Generic;

  /// <summary>
  /// Client is making a request to obs-websocket. Eg get current scene, create source.<br />
  /// Identified client -> obs-websocket
  /// </summary>
  /// <example><code>{
  ///   "op": 6,
  ///   "d": {
  ///     "requestType": "SetCurrentProgramScene",
  ///     "requestId": "f819dcf0-89cc-11eb-8f0e-382c4ac93b9c",
  ///     "requestData": {
  ///       "sceneName": "Scene 12"
  ///     }
  ///   }
  /// }</code></example>
  public interface IRequest : IOpcodeMessage {
    public string RequestType { get; }

    public string RequestId { get; set; }
  }

  [MessagePackObject]
  public class Request : IRequest {
    [IgnoreMember]
    public OpCode Op => OpCode.Event;

    [Key("requestType")]
    public string RequestType { get => GetType().Name; }

    [Key("requestId")]
    public string RequestId { get; set; } = "";
  }

  [MessagePackObject]
  public class RawRequest : IRequest {
    [IgnoreMember]
    public OpCode Op => OpCode.Event;

    [Key("requestType")]
    public string RequestType { get => GetType().Name; }

    [Key("requestId")]
    public string RequestId { get; set; } = "";

    [Key("requestData")]
    public Dictionary<string, object?>? RequestData { get; set; }
  }
}
