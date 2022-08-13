namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;
  using MessagePack.Resolvers;
  using System;
  using System.Linq;

  class UnexpectedProtocolException : Exception {
    public UnexpectedProtocolException(string? message = null) : base(message ?? "It's not seem to be obs websocket message") { }
  }

  class OpcodeMessageFormatter : IMessagePackFormatter<IOpcodeMessage> {
    private static readonly DynamicObjectResolver _resolver = DynamicObjectResolver.Instance;

    private readonly IMessagePackFormatter<Hello> _helloFormatter = _resolver.GetFormatter<Hello>();
    private readonly IMessagePackFormatter<Identify> _identifyFormatter = _resolver.GetFormatter<Identify>();
    private readonly IMessagePackFormatter<Identified> _identifiedFormatter = _resolver.GetFormatter<Identified>();
    private readonly IMessagePackFormatter<Reidentify> _reidentifyFormatter = _resolver.GetFormatter<Reidentify>();
    private readonly IMessagePackFormatter<Request> _requestFormatter = _resolver.GetFormatter<Request>();
    private readonly IMessagePackFormatter<RequestResponse> _requestResponseFormatter = _resolver.GetFormatter<RequestResponse>();

    public IOpcodeMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var picker = reader.CreatePeekReader();
      picker.ReadMapHeader();
      int opcode = FormatterUtil.SeekByKey(picker, "op").ReadInt32();
      FormatterUtil.SeekByKey(ref picker, "d");

      var data = opcode switch {
        (int)WebSocketOpCode.Hello => _helloFormatter.Deserialize(ref picker, options),
        (int)WebSocketOpCode.Identify => _identifyFormatter.Deserialize(ref picker, options),
        (int)WebSocketOpCode.Identified => _identifiedFormatter.Deserialize(ref picker, options),
        (int)WebSocketOpCode.Reidentify => _reidentifyFormatter.Deserialize(ref picker, options),
        (int)WebSocketOpCode.Event => EventMessageFormatter.Instance.Deserialize(ref picker, options),
        (int)WebSocketOpCode.Request => _requestFormatter.Deserialize(ref picker, options),
        (int)WebSocketOpCode.RequestResponse => _requestResponseFormatter.Deserialize(ref picker, options),
        // TODO: not implemented
        _ => new OpcodeMessage<object>((WebSocketOpCode)opcode, StandardResolver.Instance.GetFormatter<object>().Deserialize(ref picker, options)),
      };

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

  class EventMessageFormatter : IMessagePackFormatter<IOpcodeMessage> {
    public static readonly EventMessageFormatter Instance = new();

    public IOpcodeMessage Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var peeker = reader.CreatePeekReader();
      peeker.ReadMapHeader();

      string eventType = FormatterUtil.SeekByKey(peeker, "eventType").ReadString();
      var types = new Type[] {
        typeof(StreamStateChanged),
        typeof(RecordStateChanged),
      };
      var dict = types.ToDictionary(t => t.Name, t => t);
      if (dict.TryGetValue(eventType, out var type)) {
        return (MessagePackSerializer.Deserialize(type, ref reader, options) as IOpcodeMessage)!;
      }
      else {
        return MessagePackSerializer.Deserialize<Event>(ref reader, options);
      }
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
    public static MessagePackReader SeekByKey(ref MessagePackReader reader, string key) {
      while (true) {
        string cursor = reader.ReadString();
        if (cursor == key) {
          return reader;
        }
        reader.Skip();
      }
    }

    public static MessagePackReader SeekByKey(MessagePackReader reader, string key) {
      var peeker = reader.CreatePeekReader();
      return SeekByKey(ref peeker, key);
    }
  }
}
