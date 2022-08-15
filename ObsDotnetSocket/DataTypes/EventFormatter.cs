namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;
  using ObsDotnetSocket.DataTypes.Predefineds;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  class EventFormatter : IMessagePackFormatter<IOpcodeMessage> {
    public static readonly EventFormatter Instance = new();

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
      if (!FormatterUtil.SeekByKey(ref peeker, "eventType")) {
        throw new UnexpectedProtocolException();
      }
      string eventType = peeker.ReadString();

      peeker = reader.CreatePeekReader();
      if (!FormatterUtil.SeekByKey(ref peeker, "eventData")) {
        throw new UnexpectedProtocolException();
      }

      var data = _eventNameToTypes.TryGetValue(eventType, out var type)
        ? (MessagePackSerializer.Deserialize(type, ref peeker, options) as IOpcodeMessage)!
        : MessagePackSerializer.Deserialize<Event>(ref peeker, options);
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
}
