namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class Reidentify : IOpcodeMessage {
    [IgnoreMember]
    public OpCode Op => OpCode.Reidentify;

    [Key("eventSubscriptions")]
    public int EventSubscriptions { get; set; }
  }
}
