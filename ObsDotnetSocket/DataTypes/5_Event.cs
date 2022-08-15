namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using System.Collections.Generic;

  public interface IEvent : IOpcodeMessage {
    public string EventType { get; }

    public EventSubscription EventIntent { get; set; }
  }

  [MessagePackObject]
  public class Event : IEvent {
    [IgnoreMember]
    public OpCode Op => OpCode.Event;

    [IgnoreMember]
    public string EventType { get => GetType().Name; }

    [IgnoreMember]
    public EventSubscription EventIntent { get; set; }
  }

  [MessagePackObject]
  public class RawEvent : IEvent {
    [IgnoreMember]
    public OpCode Op => OpCode.Event;

    [IgnoreMember]
    public string EventType { get; set; } = "";

    [IgnoreMember]
    public EventSubscription EventIntent { get; set; }

    [IgnoreMember]
    public Dictionary<string, object?>? EventData { get; set; }
  }
}
