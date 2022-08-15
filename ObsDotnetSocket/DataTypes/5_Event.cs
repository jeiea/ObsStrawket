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
    public WebSocketOpCode Op => WebSocketOpCode.Event;

    [Key("eventType")]
    public string EventType { get => GetType().Name; }

    [Key("eventIntent")]
    public EventSubscription EventIntent { get; set; }
  }

  [MessagePackObject]
  public class RawEvent : IEvent {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.Event;

    [Key("eventType")]
    public string EventType { get; set; } = "";

    [Key("eventIntent")]
    public EventSubscription EventIntent { get; set; }

    [Key("eventData")]
    public Dictionary<string, object?>? EventData { get; set; }
  }

  public class GeneralEvent : Event {
    public GeneralEvent() {
      EventIntent = EventSubscription.General;
    }
  }

  [MessagePackObject]
  public class ExitStarted : GeneralEvent { }


  public class TransitionsEvent<T> : Event {
    public TransitionsEvent() {
      EventIntent = EventSubscription.Transitions;
    }
  }

  [MessagePackObject]
  public class SceneTransitionStarted : OutputsEvent {
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  [MessagePackObject]
  public class SceneTransitionEnded : OutputsEvent {
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  public class OutputsEvent : Event {
    public OutputsEvent() {
      EventIntent = EventSubscription.Outputs;
    }
  }

  [MessagePackObject]
  public class StreamStateChanged : OutputsEvent {
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    [Key("outputState")]
    public string OutputState { get; set; } = "";
  }

  [MessagePackObject]
  public class RecordStateChanged : OutputsEvent {
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    [Key("outputState")]
    public string OutputState { get; set; } = "";

    [Key("outputPath")]
    public string? OutputPath { get; set; }
  }
}
