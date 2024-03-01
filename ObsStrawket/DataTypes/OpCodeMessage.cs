namespace ObsStrawket.DataTypes {
  /// <summary>
  /// The interface for representing low-level message types which may be sent to and from obs-websocket.
  /// </summary>
  public interface IOpCodeMessage {
    /// <summary>
    /// <c>WebSocketOpCode</c> OpCode.
    /// </summary>
    public OpCode Op { get; }
  }

  /// <summary>
  /// The low-level message types which may be sent to and from obs-websocket.
  /// </summary>
  /// <typeparam name="T">Type of the data fields associated with the operation.</typeparam>
  public class OpCodeMessage<T> : IOpCodeMessage where T : new() {
    /// <summary>
    /// Create websocket protocol message.
    /// </summary>
    public OpCodeMessage(OpCode op, T d) {
      Op = op;
      Data = d ?? new();
    }

    /// <summary>
    /// <c>WebSocketOpCode</c> OpCode.
    /// </summary>
    [Key("op")]
    public OpCode Op { get; set; }

    /// <summary>
    /// An object of the data fields associated with the operation.
    /// </summary>
    [Key("d")]
    public T Data { get; set; }
  }
}
