using MessagePack;
using ObsStrawket.DataTypes.Predefineds;
using System.Collections.Generic;

namespace ObsStrawket.DataTypes {
  public interface IObsEvent : IOpCodeMessage {
    public string EventType { get; }

    public EventSubscription EventIntent { get; set; }
  }

  [MessagePackObject]
  public class ObsEvent : IObsEvent {
    [IgnoreMember]
    public OpCode Op => OpCode.Event;

    [IgnoreMember]
    public string EventType { get => GetType().Name; }

    [IgnoreMember]
    public EventSubscription EventIntent { get; set; }
  }

  [MessagePackObject]
  public sealed class RawEvent : IObsEvent {
    [IgnoreMember]
    public OpCode Op => OpCode.Event;

    [Key("eventType")]
    public string EventType { get; set; } = "";

    [Key("eventIntent")]
    public EventSubscription EventIntent { get; set; }

    [Key("eventData")]
    public Dictionary<string, object?>? EventData { get; set; }
  }
}
