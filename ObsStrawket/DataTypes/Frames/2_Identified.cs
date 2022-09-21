using MessagePack;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: Freshly identified client<br />
  /// Description: The identify request was received and validated,
  /// and the connection is now ready for normal operation.
  /// </summary>
  [MessagePackObject]
  public class Identified : IOpCodeMessage {
    /// <summary>
    /// Identified (OpCode 2)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.Identified;

    /// <summary>
    /// If rpc version negotiation succeeds, the server determines the RPC version
    /// to be used and gives it to the client as this.
    /// </summary>
    [Key("negotiatedRpcVersion")]
    public int NegotiatedRpcVersion { get; set; }
  }
}
