namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class RequestResponseMessage : OpcodeMessage<RequestResponse> {
    public RequestResponseMessage(RequestResponse data) : base(0, data) { }
  }

  [MessagePackObject]
  public class RequestResponse : IOpcodeMessage {
    [IgnoreMember]
    public int Op => 7;

    [Key("requestType")]
    public string RequestType { get; set; } = "";

    [Key("requestId")]
    public string RequestId { get; set; } = "";

    [Key("requestData")]
    public object? RequestData { get; set; }
  }
}
