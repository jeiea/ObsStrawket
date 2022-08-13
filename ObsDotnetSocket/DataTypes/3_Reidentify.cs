namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class Reidentify : IOpcodeMessage {
    [IgnoreMember]
    public int Op => 3;

    [Key("eventSubscriptions")]
    public int EventSubscriptions { get; set; }
  }
}
