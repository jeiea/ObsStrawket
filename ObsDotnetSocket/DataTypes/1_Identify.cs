namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class Identify : IOpcodeMessage {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.Identify;

    [Key("rpcVersion")]
    public int RpcVersion { get; set; }

    [Key("authentication")]
    public string? Authentication { get; set; }

    [Key("eventSubscriptions")]
    public EventSubscription EventSubscriptions { get; set; }
  }
}
