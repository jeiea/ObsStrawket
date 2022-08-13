namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class IdentifyMessage : OpcodeMessage<Identify> {
    public IdentifyMessage(Identify data) : base(1, data) { }
  }

  [MessagePackObject]
  public class Identify : IOpcodeMessage {
    [IgnoreMember]
    public int Op => 1;

    [Key("rpcVersion")]
    public int RpcVersion { get; set; }

    [Key("authentication")]
    public string? Authentication { get; set; }

    [Key("eventSubscriptions")]
    public EventSubscriptions EventSubscriptions { get; set; }
  }
}
