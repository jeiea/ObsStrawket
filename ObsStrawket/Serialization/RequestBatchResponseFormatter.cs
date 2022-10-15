using MessagePack;
using MessagePack.Formatters;
using ObsStrawket.DataTypes;
using System;

namespace ObsStrawket.Serialization {
  internal class RequestBatchResponseFormatter : IMessagePackFormatter<IRequestBatchResponse> {
    public static readonly RequestResponseFormatter Instance = new();

    public IRequestBatchResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      throw new NotImplementedException();
    }

    public void Serialize(ref MessagePackWriter writer, IRequestBatchResponse value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      throw new NotImplementedException();
    }
  }
}
