namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using System.Collections.Generic;

  [MessagePackObject]
  public class Event<T> : IOpcodeMessage {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.Event;

    [Key("eventType")]
    public virtual string EventType { get => GetType().Name; set { } }

    [Key("eventIntent")]
    public EventSubscription EventIntent { get; set; }
  }

  [MessagePackObject]
  public class Event : Event<Dictionary<string, object?>?> {
    [IgnoreMember]
    public override string EventType { get; set; } = "";

    [Key("eventData")]
    public Dictionary<string, object?>? EventData { get; set; }
  }

  public class GeneralEvent<T> : Event<T> {
    public GeneralEvent() {
      EventIntent = EventSubscription.General;
    }
  }

  [MessagePackObject]
  public class ExitStarted : OutputsEvent<ExitStarted> { }


  public class TransitionsEvent<T> : Event<T> {
    public TransitionsEvent() {
      EventIntent = EventSubscription.Transitions;
    }
  }

  [MessagePackObject]
  public class SceneTransitionStarted : OutputsEvent<SceneTransitionStarted> {
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  [MessagePackObject]
  public class SceneTransitionEnded : OutputsEvent<SceneTransitionEnded> {
    [Key("transitionName")]
    public string TransitionName { get; set; } = "";
  }

  public class OutputsEvent<T> : Event<T> {
    public OutputsEvent() {
      EventIntent = EventSubscription.Outputs;
    }
  }

  [MessagePackObject]
  public class StreamStateChanged : OutputsEvent<StreamStateChanged> {
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    [Key("outputState")]
    public string OutputState { get; set; } = "";
  }

  [MessagePackObject]
  public class RecordStateChanged : OutputsEvent<RecordStateChanged> {
    [Key("outputActive")]
    public bool OutputActive { get; set; }

    [Key("outputState")]
    public string OutputState { get; set; } = "";

    [Key("outputPath")]
    public string? OutputPath { get; set; }
  }
}
