using MessagePack;
using MessagePack.Formatters;
using ObsStrawket.DataTypes;

namespace ObsStrawket.Serialization {
  internal class RequestFormatter : IMessagePackFormatter<IRequest> {
    public static readonly RequestFormatter Instance = new();

    public IRequest Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      string? requestType = FormatterUtil.SeekByKey(reader, "requestType").ReadString();
      string? requestId = FormatterUtil.SeekByKey(reader, "requestId").ReadString();
      var peeker = FormatterUtil.SeekByKey(reader, "responseData");

      var data = DataTypeMapping.RequestToTypes.TryGetValue(requestType ?? "", out var type)
        ? (MessagePackSerializer.Deserialize(type.Request, ref peeker, options) as IRequest)!
        : MessagePackSerializer.Deserialize<Request>(ref peeker, options);
      data.RequestId = requestId ?? "null";

      reader.Skip();
      return data;
    }

    public void Serialize(ref MessagePackWriter writer, IRequest value, MessagePackSerializerOptions options) {
      var type = value.GetType();
      if (value is not RawRequest) {
        bool isEmpty = DataTypeMapping.RequestToTypes.TryGetValue(type.Name, out var mapping) && mapping.IsRequestEmpty;
        writer.WriteMapHeader(isEmpty ? 2 : 3);
        writer.Write("requestType");
        writer.Write(value.RequestType);
        writer.Write("requestId");
        writer.Write(value.RequestId);
        if (isEmpty) {
          return;
        }

        writer.Write("requestData");
      }

      MessagePackSerializer.Serialize(type, ref writer, value, options);
    }
  }
}
