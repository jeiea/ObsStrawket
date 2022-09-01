namespace ObsStrawket.Serialization {
  using MessagePack;
  using MessagePack.Formatters;
  using ObsStrawket.DataTypes;
  using System;

  class EventFormatter : IMessagePackFormatter<IEvent> {
    public static readonly EventFormatter Instance = new();

    public IEvent Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var peeker = reader.CreatePeekReader();
      if (!FormatterUtil.SeekByKey(ref peeker, "eventType")) {
        throw new UnexpectedProtocolException();
      }

      string eventType = peeker.ReadString();
      if (!DataTypeMapping.EventToTypes.TryGetValue(eventType, out var type)) {
        reader.Skip();
        return MessagePackSerializer.Deserialize<RawEvent>(ref reader, options);
      }

      peeker = reader.CreatePeekReader();
      reader.Skip();
      return FormatterUtil.SeekByKey(ref peeker, "eventData")
        ? (MessagePackSerializer.Deserialize(type, ref peeker, options) as IEvent)!
        : (Activator.CreateInstance(type) as IEvent)!;
    }

    public void Serialize(ref MessagePackWriter writer, IEvent value, MessagePackSerializerOptions options) {
      if (value is RawEvent raw) {
        MessagePackSerializer.Serialize(ref writer, raw, options);
      }
      else {
        writer.WriteMapHeader(2);
        writer.Write("op");
        writer.Write((int)value.Op);
        writer.Write("d");

        MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
      }
    }
  }
}
