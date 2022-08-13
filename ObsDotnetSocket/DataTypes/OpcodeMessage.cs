namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  public interface IOpcodeMessage {
    public WebSocketOpCode Op { get; }
  }

  [MessagePackObject]
  public class OpcodeMessage<T> : IOpcodeMessage where T : new() {
    public OpcodeMessage(WebSocketOpCode op, T? data = default) {
      Op = op;
      Data = data ?? new();
    }

    [Key("op")]
    public WebSocketOpCode Op { get; set; }

    [Key("d")]
    public T Data { get; set; }
  }
}
