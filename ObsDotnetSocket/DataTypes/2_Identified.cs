namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class Identified : IOpcodeMessage {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.Identified;

    [Key("negotiatedRpcVersion")]
    public int NegotiatedRpcVersion { get; set; }
  }
}
