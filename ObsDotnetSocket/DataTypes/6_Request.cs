namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

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
