namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class Hello : IOpcodeMessage {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.Hello;

    [Key("obsWebSocketVersion")]
    public string ObsWebSocketVersion { get; set; } = "";

    [Key("rpcVersion")]
    public int RpcVersion { get; set; }

    [Key("authentication")]
    public HelloAuthentication? Authentication { get; set; }
  }

  [MessagePackObject]
  public class HelloAuthentication {
    [Key("challenge")]
    public string Challenge { get; set; } = "";

    [Key("salt")]
    public string Salt { get; set; } = "";
  }
}
