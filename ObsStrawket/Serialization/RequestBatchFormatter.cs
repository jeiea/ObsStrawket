using MessagePack;
using MessagePack.Formatters;
using ObsStrawket.DataTypes;
using System;

namespace ObsStrawket.Serialization {
  internal class RequestBatchFormatter : IMessagePackFormatter<IRequestBatch> {
    public static readonly RequestFormatter Instance = new();

    public IRequestBatch Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      throw new NotImplementedException();
    }

    public void Serialize(ref MessagePackWriter writer, IRequestBatch value, MessagePackSerializerOptions options) {
      var type = value.GetType();

      MessagePackSerializer.Serialize(type, ref writer, value, options);
    }
  }
}
