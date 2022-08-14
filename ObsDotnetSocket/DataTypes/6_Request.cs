namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

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
  [MessagePackObject]
  public class Request : IOpcodeMessage {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.Request;

    [Key("requestType")]
    public string RequestType { get; set; } = "";

    [Key("requestId")]
    public string RequestId { get; set; } = "";

    [Key("requestData")]
    public object? RequestData { get; set; }
  }
}
