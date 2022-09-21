namespace ObsStrawket.DataTypes {
  using MessagePack;
  using System.Net.WebSockets;

  /// <summary>
  /// Sent from: Identified client<br />
  /// Sent to: obs-websocket<br />
  /// Description: Sent at any time after initial identification to update the provided session parameters.
  /// </summary>
  [MessagePackObject]
  public class Reidentify : IOpCodeMessage {
    /// <summary>
    /// Reidentify (OpCode 3)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.Reidentify;

    /// <summary>
    /// Only the listed parameters may be changed after initial identification.
    /// To change a parameter not listed, you must reconnect to the obs-websocket server.
    /// </summary>
    [Key("eventSubscriptions")]
    public int EventSubscriptions { get; set; }
  }
}
