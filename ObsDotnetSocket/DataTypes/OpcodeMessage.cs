namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  public interface IOpcodeMessage {
    public OpCode Op { get; }
  }

  [MessagePackObject]
  public class OpcodeMessage<T> : IOpcodeMessage where T : new() {
    public OpcodeMessage(OpCode op, T? data = default) {
      Op = op;
      Data = data ?? new();
    }

    [Key("op")]
    public OpCode Op { get; set; }

    [Key("d")]
    public T Data { get; set; }
  }
}
