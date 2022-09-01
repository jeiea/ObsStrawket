namespace ObsStrawket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class Reidentify : IOpCodeMessage {
    [IgnoreMember]
    public OpCode Op => OpCode.Reidentify;

    [Key("eventSubscriptions")]
    public int EventSubscriptions { get; set; }
  }
}
