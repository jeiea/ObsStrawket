using ObsDotnetSocket.DataTypes;

namespace ObsDotnetSocket.Serialization {
  using MessagePack;
  using MessagePack.Formatters;
  using MessagePack.Resolvers;

  class OpCodeMessageFormatter : IMessagePackFormatter<IOpCodeMessage> {
    private static readonly DynamicObjectResolver _resolver = DynamicObjectResolver.Instance;

    private readonly IMessagePackFormatter<Hello> _helloFormatter = _resolver.GetFormatter<Hello>();
    private readonly IMessagePackFormatter<Identify> _identifyFormatter = _resolver.GetFormatter<Identify>();
    private readonly IMessagePackFormatter<Identified> _identifiedFormatter = _resolver.GetFormatter<Identified>();
    private readonly IMessagePackFormatter<Reidentify> _reidentifyFormatter = _resolver.GetFormatter<Reidentify>();

    public IOpCodeMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var peeker = reader.CreatePeekReader();
      if (!FormatterUtil.SeekByKey(ref peeker, "op")) {
        throw new UnexpectedProtocolException();
      }
      int opcode = peeker.ReadInt32();

      peeker = reader.CreatePeekReader();
      if (!FormatterUtil.SeekByKey(ref peeker, "d")) {
        throw new UnexpectedProtocolException();
      }

      var data = opcode switch {
        (int)OpCode.Hello => _helloFormatter.Deserialize(ref peeker, options),
        (int)OpCode.Identify => _identifyFormatter.Deserialize(ref peeker, options),
        (int)OpCode.Identified => _identifiedFormatter.Deserialize(ref peeker, options),
        (int)OpCode.Reidentify => _reidentifyFormatter.Deserialize(ref peeker, options),
        (int)OpCode.Event => EventFormatter.Instance.Deserialize(ref peeker, options) as IOpCodeMessage,
        (int)OpCode.Request => RequestFormatter.Instance.Deserialize(ref peeker, options),
        (int)OpCode.RequestResponse => RequestResponseFormatter.Instance.Deserialize(ref peeker, options),
        // TODO: not implemented
        _ => new OpCodeMessage<object>((OpCode)opcode, StandardResolver.Instance.GetFormatter<object>().Deserialize(ref peeker, options)),
      };

      reader.Skip();
      return data;
    }

    public void Serialize(ref MessagePackWriter writer, IOpCodeMessage value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      switch (value) {
      case IEvent @event:
        EventFormatter.Instance.Serialize(ref writer, @event, options);
        break;
      case IRequest request:
        RequestFormatter.Instance.Serialize(ref writer, request, options);
        break;
      case IRequestResponse response:
        RequestResponseFormatter.Instance.Serialize(ref writer, response, options);
        break;
      default:
        MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
        break;
      }
    }
  }
}
