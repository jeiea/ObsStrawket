using ObsStrawket.DataTypes.Predefineds;
using System.Collections.Generic;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: All subscribed and identified clients<br />
  /// Description: An event coming from OBS has occurred. E.g. scene switched, source muted.
  /// </summary>
  public interface IObsEvent : IOpCodeMessage {
    /// <summary>
    /// Event (OpCode 5)
    /// </summary>
    public string EventType { get; }

    /// <summary>
    /// The original intent required to be subscribed to in order to receive the event.
    /// </summary>
    public EventSubscription EventIntent { get; set; }
  }

  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: All subscribed and identified clients<br />
  /// Description: An event coming from OBS has occurred. E.g. scene switched, source muted.
  /// </summary>
  public class ObsEvent : IObsEvent {
    /// <summary>
    /// Event (OpCode 5)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.Event;

    /// <summary>
    /// The string identifying event type.
    /// </summary>
    [IgnoreMember]
    public string EventType { get => GetType().Name; }

    /// <summary>
    /// The original intent required to be subscribed to in order to receive the event.
    /// </summary>
    [IgnoreMember]
    public EventSubscription EventIntent { get; set; }
  }

  /// <summary>
  /// Sent from: obs-websocket<br />
  /// Sent to: All subscribed and identified clients<br />
  /// Description: An event coming from OBS has occurred. E.g. scene switched, source muted.<br />
  /// Unidentifiable event will be this.
  /// </summary>
  public sealed class RawEvent : IObsEvent {
    /// <summary>
    /// Event (OpCode 5)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.Event;

    /// <summary>
    /// The string identifying event type.
    /// </summary>
    [Key("eventType")]
    public string EventType { get; set; } = "";

    /// <summary>
    /// The original intent required to be subscribed to in order to receive the event.
    /// </summary>
    [Key("eventIntent")]
    public EventSubscription EventIntent { get; set; }

    /// <summary>
    /// Raw event data from server.
    /// </summary>
    [Key("eventData")]
    public Dictionary<string, object?>? EventData { get; set; }
  }
}
