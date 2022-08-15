namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;

  class EventFormatter : IMessagePackFormatter<IEvent> {
    public static readonly EventFormatter Instance = new();

    public IEvent Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var peeker = reader.CreatePeekReader();
      if (!FormatterUtil.SeekByKey(ref peeker, "eventType")) {
        throw new UnexpectedProtocolException();
      }
      string eventType = peeker.ReadString();

      peeker = reader.CreatePeekReader();
      if (!FormatterUtil.SeekByKey(ref peeker, "eventData")) {
        throw new UnexpectedProtocolException();
      }

      var data = DataTypeMapping.EventToTypes.TryGetValue(eventType, out var type)
        ? (MessagePackSerializer.Deserialize(type, ref peeker, options) as IEvent)!
        : MessagePackSerializer.Deserialize<Event>(ref peeker, options);
      reader.Skip();
      return data;
    }

    public void Serialize(ref MessagePackWriter writer, IEvent value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
    }
  }
}
