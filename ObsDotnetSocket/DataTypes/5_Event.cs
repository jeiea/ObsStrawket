namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class EventMessage : OpcodeMessage<Event> {
    public EventMessage(Event data) : base(5, data) { }
  }


  [MessagePackObject]
  public class Event : IOpcodeMessage {
    [IgnoreMember]
    public int Op => 5;

    [Key("eventType")]
    public string EventType { get; set; } = "";

    [Key("eventIntent")]
    public int EventIntent { get; set; }

    [Key("eventData")]
    public object? EventData { get; set; }
  }
}
