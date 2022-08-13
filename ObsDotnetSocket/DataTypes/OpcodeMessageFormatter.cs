namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;
  using MessagePack.Resolvers;
  using System;

  class UnexpectedProtocolException : Exception {
    public UnexpectedProtocolException(string? message = null) : base(message ?? "It's not seem to be obs websocket message") { }
  }

  class OpcodeMessageFormatter : IMessagePackFormatter<IOpcodeMessage> {
    private static readonly DynamicObjectResolver _resolver = DynamicObjectResolver.Instance;

    private readonly IMessagePackFormatter<Hello> _helloFormatter = _resolver.GetFormatter<Hello>();
    private readonly IMessagePackFormatter<Identify> _identifyFormatter = _resolver.GetFormatter<Identify>();
    private readonly IMessagePackFormatter<Identified> _identifiedFormatter = _resolver.GetFormatter<Identified>();
    private readonly IMessagePackFormatter<Reidentify> _reidentifyFormatter = _resolver.GetFormatter<Reidentify>();
    private readonly IMessagePackFormatter<Event<object?>> _eventFormatter = _resolver.GetFormatter<Event<object?>>();
    private readonly IMessagePackFormatter<Request> _requestFormatter = _resolver.GetFormatter<Request>();
    private readonly IMessagePackFormatter<RequestResponse> _requestResponseFormatter = _resolver.GetFormatter<RequestResponse>();

    public IOpcodeMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      int propertyCount = reader.ReadMapHeader();
      if (propertyCount != 2) {
        throw new UnexpectedProtocolException();
      }

      int opcode = FormatterUtil.SeekByKey(reader, "op").ReadInt32();
      string key = reader.ReadString();
      if (key != "d") {
        throw new UnexpectedProtocolException();
      }

      IOpcodeMessage data = opcode switch {
        (int)WebSocketOpCode.Hello => _helloFormatter.Deserialize(ref reader, options),
        (int)WebSocketOpCode.Identify => _identifyFormatter.Deserialize(ref reader, options),
        (int)WebSocketOpCode.Identified => _identifiedFormatter.Deserialize(ref reader, options),
        (int)WebSocketOpCode.Reidentify => _reidentifyFormatter.Deserialize(ref reader, options),
        (int)WebSocketOpCode.Event => _eventFormatter.Deserialize(ref reader, options),
        (int)WebSocketOpCode.Request => _requestFormatter.Deserialize(ref reader, options),
        (int)WebSocketOpCode.RequestResponse => _requestResponseFormatter.Deserialize(ref reader, options),
        // TODO: not implemented
        _ => new OpcodeMessage<object>((WebSocketOpCode)opcode, StandardResolver.Instance.GetFormatter<object>().Deserialize(ref reader, options)),
      };

      reader.Skip();
      reader.Skip();

      return data;
    }

    public void Serialize(ref MessagePackWriter writer, IOpcodeMessage value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
    }
  }

  internal static class FormatterUtil {
    public static MessagePackReader SeekByKey(MessagePackReader reader, string key) {
      var peeker = reader.CreatePeekReader();
      while (true) {
        string cursor = peeker.ReadString();
        if (cursor == key) {
          return peeker;
        }
        peeker.Skip();
      }
    }
  }
}
