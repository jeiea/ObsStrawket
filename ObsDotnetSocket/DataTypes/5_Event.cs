namespace ObsDotnetSocket.DataTypes {
  using MessagePack;

  [MessagePackObject]
  public class Event<T> : IOpcodeMessage {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.Event;

    [Key("eventType")]
    public virtual string EventType { get => GetType().Name; set { } }

    [Key("eventIntent")]
    public EventSubscription EventIntent { get; set; }
  }

  [MessagePackObject()]
  public class Event : Event<object?> {
    [IgnoreMember]
    public override string EventType { get; set; } = "";

    [Key("eventData")]
    public object? EventData { get; set; }
  }

  public class GeneralEvent<T> : Event<T> {
    public GeneralEvent() {
      EventIntent = EventSubscription.General;
    }
  }

  [MessagePackObject]
  public class ExitStarted : OutputEvent<ExitStarted> { }


  public class OutputEvent<T> : Event<T> {
    public OutputEvent() {
      EventIntent = EventSubscription.Outputs;
    }
  }

  [MessagePackObject]
  public class StreamStateChanged : OutputEvent<StreamStateChanged> {
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    [Key("outputState")]
    public string OutputState { get; set; } = "";
  }

  [MessagePackObject]
  public class RecordStateChanged : OutputEvent<RecordStateChanged> {
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    [Key("outputState")]
    public string OutputState { get; set; } = "";

    [Key("outputPath")]
    public string? OutputPath { get; set; }
  }
}
