namespace ObsStrawket.DataTypes {
  using MessagePack;

  public interface IOpCodeMessage {
    public OpCode Op { get; }
  }

  [MessagePackObject]
  public class OpCodeMessage<T> : IOpCodeMessage where T : new() {
    public OpCodeMessage(OpCode op, T d = default) {
      Op = op;
      Data = d ?? new();
    }

    [Key("op")]
    public OpCode Op { get; set; }

    [Key("d")]
    public T Data { get; set; }
  }
}
