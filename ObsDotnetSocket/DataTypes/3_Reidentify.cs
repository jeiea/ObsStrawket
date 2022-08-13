namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class Reidentify : IOpcodeMessage {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.Reidentify;

    [Key("eventSubscriptions")]
    public int EventSubscriptions { get; set; }
  }
}
