using MessagePack;
using ObsStrawket.DataTypes.Predefineds;

namespace ObsStrawket.DataTypes {
  [MessagePackObject]
  public class Identify : IOpCodeMessage {
    [IgnoreMember]
    public OpCode Op => OpCode.Identify;

    [Key("rpcVersion")]
    public int RpcVersion { get; set; }

    [Key("authentication")]
    public string? Authentication { get; set; }

    [Key("eventSubscriptions")]
    public EventSubscription EventSubscriptions { get; set; }
  }
}
