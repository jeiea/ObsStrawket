namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;
  using MessagePack.Resolvers;
  using ObsDotnetSocket.DataTypes.Predefineds;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  class UnexpectedProtocolException : Exception {
    public UnexpectedProtocolException(string? message = null) : base(message ?? "It's not seem to be obs websocket message") { }
  }

  class OpcodeMessageFormatter : IMessagePackFormatter<IOpcodeMessage> {
    private static readonly DynamicObjectResolver _resolver = DynamicObjectResolver.Instance;

    private readonly IMessagePackFormatter<Hello> _helloFormatter = _resolver.GetFormatter<Hello>();
    private readonly IMessagePackFormatter<Identify> _identifyFormatter = _resolver.GetFormatter<Identify>();
    private readonly IMessagePackFormatter<Identified> _identifiedFormatter = _resolver.GetFormatter<Identified>();
    private readonly IMessagePackFormatter<Reidentify> _reidentifyFormatter = _resolver.GetFormatter<Reidentify>();
    private readonly IMessagePackFormatter<Request> _requestFormatter = _resolver.GetFormatter<Request>();
    private readonly IMessagePackFormatter<RequestResponse> _requestResponseFormatter = _resolver.GetFormatter<RequestResponse>();

    public IOpcodeMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var picker = reader.CreatePeekReader();
      picker.ReadMapHeader();
      int opcode = FormatterUtil.SeekByKey(picker, "op").ReadInt32();
      FormatterUtil.SeekByKey(ref picker, "d");

      var data = opcode switch {
        (int)OpCode.Hello => _helloFormatter.Deserialize(ref picker, options),
        (int)OpCode.Identify => _identifyFormatter.Deserialize(ref picker, options),
        (int)OpCode.Identified => _identifiedFormatter.Deserialize(ref picker, options),
        (int)OpCode.Reidentify => _reidentifyFormatter.Deserialize(ref picker, options),
        (int)OpCode.Event => EventMessageFormatter.Instance.Deserialize(ref picker, options),
        (int)OpCode.Request => _requestFormatter.Deserialize(ref picker, options),
        (int)OpCode.RequestResponse => _requestResponseFormatter.Deserialize(ref picker, options),
        // TODO: not implemented
        _ => new OpcodeMessage<object>((OpCode)opcode, StandardResolver.Instance.GetFormatter<object>().Deserialize(ref picker, options)),
      };

      reader.Skip();
      return data;
    }

    public void Serialize(ref MessagePackWriter writer, IOpcodeMessage value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
    }
  }

  class EventMessageFormatter : IMessagePackFormatter<IOpcodeMessage> {
    public static readonly EventMessageFormatter Instance = new();

    private static readonly Dictionary<string, Type> _eventNameToTypes = new Type[] {
     // General Events,
    typeof(ExitStarted),
    typeof(VendorEvent),

    // Config Events,
    typeof(CurrentSceneCollectionChanging),
    typeof(CurrentSceneCollectionChanged),
    typeof(SceneCollectionListChanged),
    typeof(CurrentProfileChanging),
    typeof(CurrentProfileChanged),
    typeof(ProfileListChanged),

    // Scenes Events,
    typeof(SceneCreated),
    typeof(SceneRemoved),
    typeof(SceneNameChanged),
    typeof(CurrentProgramSceneChanged),
    typeof(CurrentPreviewSceneChanged),
    typeof(SceneListChanged),

    // Inputs Events,
    typeof(InputCreated),
    typeof(InputRemoved),
    typeof(InputNameChanged),
    typeof(InputActiveStateChanged),
    typeof(InputShowStateChanged),
    typeof(InputMuteStateChanged),
    typeof(InputVolumeChanged),
    typeof(InputAudioBalanceChanged),
    typeof(InputAudioSyncOffsetChanged),
    typeof(InputAudioTracksChanged),
    typeof(InputAudioMonitorTypeChanged),
    typeof(InputVolumeMeters),

    // Transitions Events,
    typeof(CurrentSceneTransitionChanged),
    typeof(CurrentSceneTransitionDurationChanged),
    typeof(SceneTransitionStarted),
    typeof(SceneTransitionEnded),
    typeof(SceneTransitionVideoEnded),

    // Filters Events,
    typeof(SourceFilterListReindexed),
    typeof(SourceFilterCreated),
    typeof(SourceFilterRemoved),
    typeof(SourceFilterNameChanged),
    typeof(SourceFilterEnableStateChanged),

    // Scene Items Events,
    typeof(SceneItemCreated),
    typeof(SceneItemRemoved),
    typeof(SceneItemListReindexed),
    typeof(SceneItemEnableStateChanged),
    typeof(SceneItemLockStateChanged),
    typeof(SceneItemSelected),
    typeof(SceneItemTransformChanged),

    // Outputs Events,
    typeof(StreamStateChanged),
    typeof(RecordStateChanged),
    typeof(ReplayBufferStateChanged),
    typeof(VirtualcamStateChanged),
    typeof(ReplayBufferSaved),

    // Media Inputs Events,
    typeof(MediaInputPlaybackStarted),
    typeof(MediaInputPlaybackEnded),
    typeof(MediaInputActionTriggered),

    // Ui Events,
    typeof(StudioModeStateChanged),
    }.ToDictionary(t => t.Name, t => t);

    public IOpcodeMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var peeker = reader.CreatePeekReader();
      reader.Skip();
      peeker.ReadMapHeader();

      string eventType = FormatterUtil.SeekByKey(peeker, "eventType").ReadString();
      FormatterUtil.SeekByKey(ref peeker, "eventData");
      if (_eventNameToTypes.TryGetValue(eventType, out var type)) {
        return (MessagePackSerializer.Deserialize(type, ref peeker, options) as IOpcodeMessage)!;
      }
      else {
        return MessagePackSerializer.Deserialize<Event>(ref peeker, options);
      }
    }

    public void Serialize(ref MessagePackWriter writer, IOpcodeMessage value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
    }
  }

  internal static class FormatterUtil {
    public static MessagePackReader SeekByKey(ref MessagePackReader reader, string key) {
      while (true) {
        string cursor = reader.ReadString();
        if (cursor == key) {
          return reader;
        }
        reader.Skip();
      }
    }

    public static MessagePackReader SeekByKey(MessagePackReader reader, string key) {
      var peeker = reader.CreatePeekReader();
      return SeekByKey(ref peeker, key);
    }
  }
}
