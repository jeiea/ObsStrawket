using MessagePack;
using ObsStrawket.DataTypes.Predefineds;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Sent from: Freshly connected websocket client<br />
  /// Sent to: obs-websocket<br />
  /// Description: Response to <c>Hello</c> message, should contain authentication string
  /// if authentication is required, along with PubSub subscriptions and other session parameters.
  /// </summary>
  [MessagePackObject]
  public class Identify : IOpCodeMessage {
    /// <summary>
    /// Identify (OpCode 1)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.Identify;

    /// <summary>
    /// The version number that the client would like the obs-websocket server to use.
    /// </summary>
    [Key("rpcVersion")]
    public int RpcVersion { get; set; }

    /// <summary>
    /// Authentication string (optional)
    /// </summary>
    [Key("authentication")]
    public string? Authentication { get; set; }

    /// <summary>
    /// A bitmask of <c>EventSubscriptions</c> items to subscribe to events and event categories at will.
    /// By default, all event categories are subscribed, except for events marked as high volume. High volume events must be explicitly subscribed to.
    /// </summary>
    [Key("eventSubscriptions")]
    public EventSubscription EventSubscriptions { get; set; }
  }
}
