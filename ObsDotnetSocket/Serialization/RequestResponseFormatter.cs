namespace ObsDotnetSocket.Serialization {
  using MessagePack;
  using MessagePack.Formatters;
  using ObsDotnetSocket.DataTypes;

  class RequestResponseFormatter : IMessagePackFormatter<IRequestResponse> {
    public static readonly RequestResponseFormatter Instance = new();

    public IRequestResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      string requestType = FormatterUtil.SeekByKey(reader, "requestType").ReadString();
      string requestId = FormatterUtil.SeekByKey(reader, "requestId").ReadString();
      var peeker = FormatterUtil.SeekByKey(reader, "requestStatus");
      var requestStatus = MessagePackSerializer.Deserialize<RequestStatus>(ref peeker, options);
      peeker = FormatterUtil.SeekByKey(reader, "responseData");

      var data = DataTypeMapping.RequestToTypes.TryGetValue(requestType, out var type)
        ? (MessagePackSerializer.Deserialize(type.Item2, ref peeker, options) as IRequestResponse)!
        : MessagePackSerializer.Deserialize<RequestResponse>(ref peeker, options);
      if (data is RequestResponse response) {
        response.RequestId = requestId;
        response.RequestStatus = requestStatus;
      }

      reader.Skip();
      return data;
    }

    public void Serialize(ref MessagePackWriter writer, IRequestResponse value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      if (value is not RawRequestResponse) {
        writer.WriteMapHeader(3);
        writer.Write("requestType");
        writer.Write(value.RequestType);
        writer.Write("requestId");
        writer.Write(value.RequestId);
        writer.Write("responseStatus");
        MessagePackSerializer.Serialize(typeof(RequestStatus), ref writer, value, options);
        writer.Write("responseData");
      }

      MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
    }
  }
}
