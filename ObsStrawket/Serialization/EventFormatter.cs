using MessagePack;
using MessagePack.Formatters;
using ObsStrawket.DataTypes;
using System;

namespace ObsStrawket.Serialization {
  internal class EventFormatter : IMessagePackFormatter<IObsEvent> {
    public static readonly EventFormatter Instance = new();

    public IObsEvent Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var peeker = reader.CreatePeekReader();
      if (!FormatterUtil.SeekByKey(ref peeker, "eventType")) {
        throw new UnexpectedProtocolException();
      }

      string eventType = peeker.ReadString();
      if (!DataTypeMapping.EventToTypes.TryGetValue(eventType, out var type)) {
        peeker = reader.CreatePeekReader();
        reader.Skip();
        return MessagePackSerializer.Deserialize<RawEvent>(ref peeker, options);
      }

      peeker = reader.CreatePeekReader();
      reader.Skip();
      return FormatterUtil.SeekByKey(ref peeker, "eventData")
        ? (MessagePackSerializer.Deserialize(type, ref peeker, options) as IObsEvent)!
        : (Activator.CreateInstance(type) as IObsEvent)!;
    }

    public void Serialize(ref MessagePackWriter writer, IObsEvent value, MessagePackSerializerOptions options) {
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
