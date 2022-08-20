namespace ObsDotnetSocket.Serialization {
  using MessagePack;
  using MessagePack.Formatters;
  using ObsDotnetSocket.DataTypes;

  class RequestResponseFormatter : IMessagePackFormatter<IRequestResponse> {
    public static readonly RequestResponseFormatter Instance = new();

    public IRequestResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      string requestType = FormatterUtil.SeekByKey(reader, "requestType").ReadString();
      if (!DataTypeMapping.RequestToTypes.TryGetValue(requestType, out var type)) {
        return MessagePackSerializer.Deserialize<RawRequestResponse>(ref reader, options);
      }

      var peeker = reader.CreatePeekReader();
      if (!FormatterUtil.SeekByKey(ref peeker, "responseData")) {
        return MessagePackSerializer.Deserialize<RawRequestResponse>(ref reader, options);
      }

      var response = (MessagePackSerializer.Deserialize(type.Response, ref peeker, options) as RequestResponse)!;
      response.RequestId = FormatterUtil.SeekByKey(reader, "requestId").ReadString();
      peeker = FormatterUtil.SeekByKey(reader, "requestStatus");
      response.RequestStatus = MessagePackSerializer.Deserialize<RequestStatus>(ref peeker, options);

      reader.Skip();

      return response;
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
