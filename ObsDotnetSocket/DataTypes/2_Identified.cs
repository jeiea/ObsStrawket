namespace ObsDotnetSocket.DataTypes {
  using MessagePack;


  [MessagePackObject]
  public class IdentifiedMessage : OpcodeMessage<Identified> {
    public IdentifiedMessage(Identified data) : base(2, data) { }
  }


  [MessagePackObject]
  public class Identified : IOpcodeMessage {
    [IgnoreMember]
    public int Op => 2;

    [Key("negotiatedRpcVersion")]
    public int NegotiatedRpcVersion { get; set; }
  }
}
