namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;

  class RequestFormatter : IMessagePackFormatter<IRequest> {
    public static readonly RequestFormatter Instance = new();

    public IRequest Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      string requestType = FormatterUtil.SeekByKey(reader, "requestType").ReadString();
      string requestId = FormatterUtil.SeekByKey(reader, "requestId").ReadString();
      var peeker = FormatterUtil.SeekByKey(reader, "responseData");

      var data = DataTypeMapping.RequestToTypes.TryGetValue(requestType, out var type)
        ? (MessagePackSerializer.Deserialize(type.Item1, ref peeker, options) as IRequest)!
        : MessagePackSerializer.Deserialize<Request>(ref peeker, options);
      data.RequestId = requestId;

      reader.Skip();
      return data;
    }

    public void Serialize(ref MessagePackWriter writer, IRequest value, MessagePackSerializerOptions options) {
      if (value is not RawRequest) {
        writer.WriteMapHeader(3);
        writer.Write("requestType");
        writer.Write(value.RequestType);
        writer.Write("requestId");
        writer.Write(value.RequestId);
        writer.Write("requestData");
      }

      MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
    }
  }
}
