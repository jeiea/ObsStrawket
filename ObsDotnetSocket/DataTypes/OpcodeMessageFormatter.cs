namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;
  using MessagePack.Resolvers;
  using System;

  class OpcodeMessageFormatter : IMessagePackFormatter<IOpcodeMessage> {
    private static readonly DynamicObjectResolver _resolver = DynamicObjectResolver.Instance;

    private readonly IMessagePackFormatter<Hello> _helloFormatter = _resolver.GetFormatter<Hello>();
    private readonly IMessagePackFormatter<Identify> _identifyFormatter = _resolver.GetFormatter<Identify>();
    private readonly IMessagePackFormatter<Identified> _identifiedFormatter = _resolver.GetFormatter<Identified>();
    private readonly IMessagePackFormatter<Reidentify> _reidentifyFormatter = _resolver.GetFormatter<Reidentify>();
    private readonly IMessagePackFormatter<Event> _eventFormatter = _resolver.GetFormatter<Event>();
    private readonly IMessagePackFormatter<Request> _requestFormatter = _resolver.GetFormatter<Request>();
    private readonly IMessagePackFormatter<RequestResponse> _requestResponseFormatter = _resolver.GetFormatter<RequestResponse>();

    public IOpcodeMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      int propertyCount = reader.ReadMapHeader();
      if (propertyCount != 2) {
        throw new Exception("It's not seem to be obs websocket message");
      }

      int opcode = PeekOp(ref reader, options);
      string key = reader.ReadString();
      if (key != "d") {
        throw new Exception("It's not seem to be obs websocket message");
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

    private static int PeekOp(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var peeker = reader.CreatePeekReader();
      while (true) {
        string key = peeker.ReadString();
        if (key == "op") {
          return peeker.ReadUInt16();
        }
        peeker.Skip();
      }
    }

    public void Serialize(ref MessagePackWriter writer, IOpcodeMessage value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      switch (value) {
      case Hello data:
        _helloFormatter.Serialize(ref writer, data, options);
        break;
      case Identify data:
        _identifyFormatter.Serialize(ref writer, data, options);
        break;
      case Identified data:
        _identifiedFormatter.Serialize(ref writer, data, options);
        break;
      case Reidentify data:
        _reidentifyFormatter.Serialize(ref writer, data, options);
        break;
      case Event data:
        _eventFormatter.Serialize(ref writer, data, options);
        break;
      case Request data:
        _requestFormatter.Serialize(ref writer, data, options);
        break;
      case RequestResponse data:
        _requestResponseFormatter.Serialize(ref writer, data, options);
        break;
      default:
        StandardResolver.Instance.GetFormatter<object>().Serialize(ref writer, value, options);
        break;
      };
    }
  }
}
