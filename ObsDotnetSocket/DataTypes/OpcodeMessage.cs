namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  public interface IOpcodeMessage {
    public int Op { get; }
  }

  [MessagePackObject]
  public class OpcodeMessage<T> : IOpcodeMessage where T : new() {
    public OpcodeMessage(int op = -1, T? data = default) {
      Op = op;
      Data = data ?? new();
    }

    [Key("op")]
    public int Op { get; set; }

    [Key("d")]
    public T Data { get; set; }
  }
}
