namespace ObsStrawket.Serialization {
  using MessagePack;
  using MessagePack.Formatters;
  using ObsStrawket.DataTypes;
  using System;

  public class OutputFlagsMapFormatter : IMessagePackFormatter<OutputFlags> {
    internal static readonly OutputFlagsMapFormatter Instance = new();
    private static readonly EnumAsStringFormatter<OutputFlags> _enumFormatter = new();

    public OutputFlags Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      OutputFlags value = 0;

      int count = reader.ReadMapHeader();
      for (int i = 0; i < count; i++) {
        var key = _enumFormatter.Deserialize(ref reader, options);
        if (reader.ReadBoolean()) {
          value |= key;
        }
      }

      return value;
    }

    public void Serialize(ref MessagePackWriter writer, OutputFlags value, MessagePackSerializerOptions options) {
      throw new NotImplementedException();
    }
  }
}
