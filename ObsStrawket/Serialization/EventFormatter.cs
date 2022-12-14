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
        throw new UnexpectedResponseException();
      }

      string eventType = peeker.ReadString();
      if (DataTypeMapping.EventToTypes.TryGetValue(eventType, out var type)) {
        try {
          peeker = reader.CreatePeekReader();
          var ev = FormatterUtil.SeekByKey(ref peeker, "eventData")
            ? (MessagePackSerializer.Deserialize(type, ref peeker, options) as IObsEvent)!
            : (Activator.CreateInstance(type) as IObsEvent)!;
          reader.Skip();
          return ev;
        }
        catch (MessagePackSerializationException) { }
      }

      peeker = reader.CreatePeekReader();
      reader.Skip();
      return MessagePackSerializer.Deserialize<RawEvent>(ref peeker, options);
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
