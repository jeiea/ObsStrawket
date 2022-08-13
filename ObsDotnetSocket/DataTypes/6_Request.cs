namespace ObsDotnetSocket.DataTypes {
  using MessagePack;


  [MessagePackObject]
  public class RequestMessage : OpcodeMessage<Request> {
    public RequestMessage(Request data) : base(6, data) { }
  }


  [MessagePackObject]
  public class Request : IOpcodeMessage {
    [IgnoreMember]
    public int Op => 6;

    [Key("requestType")]
    public string RequestType { get; set; } = "";

    [Key("requestId")]
    public string RequestId { get; set; } = "";

    [Key("requestData")]
    public object? RequestData { get; set; }
  }
}
