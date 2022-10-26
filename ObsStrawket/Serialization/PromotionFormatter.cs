using MessagePack.Formatters;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Buffers;

namespace ObsStrawket.Serialization {
  internal class PromotionFormatter : IMessagePackFormatter<object> {
    public static readonly IMessagePackFormatter<object> Instance = new PromotionFormatter();

    private static readonly Dictionary<Type, int> TypeToJumpCode = new()
    {
            // When adding types whose size exceeds 32-bits, add support in MessagePackSecurity.GetHashCollisionResistantEqualityComparer<T>()
            { typeof(bool), 0 },
            { typeof(char), 1 },
            { typeof(sbyte), 2 },
            { typeof(byte), 3 },
            { typeof(short), 4 },
            { typeof(ushort), 5 },
            { typeof(int), 6 },
            { typeof(uint), 7 },
            { typeof(long), 8 },
            { typeof(ulong), 9 },
            { typeof(float), 10 },
            { typeof(double), 11 },
            { typeof(DateTime), 12 },
            { typeof(string), 13 },
            { typeof(byte[]), 14 },
        };

    protected PromotionFormatter() {
    }

    public static bool IsSupportedType(Type type, TypeInfo typeInfo, object value) {
      if (value == null) {
        return true;
      }

      if (TypeToJumpCode.ContainsKey(type)) {
        return true;
      }

      if (typeInfo.IsEnum) {
        return true;
      }

      if (value is System.Collections.IDictionary) {
        return true;
      }

      if (value is System.Collections.ICollection) {
        return true;
      }

      return false;
    }

    public void Serialize(ref MessagePackWriter writer, object value, MessagePackSerializerOptions options) {
      DynamicObjectTypeFallbackFormatter.Instance.Serialize(ref writer, value, options);
    }

    public object Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      var type = reader.NextMessagePackType;
      var resolver = options.Resolver;
      switch (type) {
      case MessagePackType.Integer:
        byte code = reader.NextCode;
        if (code >= MessagePackCode.MinNegativeFixInt && code <= MessagePackCode.MaxNegativeFixInt) {
          return (int)reader.ReadSByte();
        }
        else if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt) {
          return (int)reader.ReadByte();
        }
        else if (code == MessagePackCode.Int8) {
          return (int)reader.ReadSByte();
        }
        else if (code == MessagePackCode.Int16) {
          return (int)reader.ReadInt16();
        }
        else if (code == MessagePackCode.Int32) {
          return (long)reader.ReadInt32();
        }
        else if (code == MessagePackCode.Int64) {
          return reader.ReadInt64();
        }
        else if (code == MessagePackCode.UInt8) {
          return (int)reader.ReadByte();
        }
        else if (code == MessagePackCode.UInt16) {
          return (int)reader.ReadUInt16();
        }
        else if (code == MessagePackCode.UInt32) {
          return (long)reader.ReadUInt32();
        }
        else if (code == MessagePackCode.UInt64) {
          return reader.ReadUInt64();
        }

        throw new MessagePackSerializationException("Invalid primitive bytes.");
      case MessagePackType.Boolean:
        return reader.ReadBoolean();
      case MessagePackType.Float:
        if (reader.NextCode == MessagePackCode.Float32) {
          return (double)reader.ReadSingle();
        }
        else {
          return reader.ReadDouble();
        }

      case MessagePackType.String:
        return reader.ReadString();
      case MessagePackType.Binary:
        // We must copy the sequence returned by ReadBytes since the reader's sequence is only valid during deserialization.
        return reader.ReadBytes()?.ToArray()!;
      case MessagePackType.Extension:
        var ext = reader.ReadExtensionFormatHeader();
        if (ext.TypeCode == ReservedMessagePackExtensionTypeCode.DateTime) {
          return reader.ReadDateTime(ext);
        }

        throw new MessagePackSerializationException("Invalid primitive bytes.");
      case MessagePackType.Array: {
          int length = reader.ReadArrayHeader();
          if (length == 0) {
            return Array.Empty<object>();
          }

          var objectFormatter = resolver.GetFormatter<object>();
          object[] array = new object[length];
          options.Security.DepthStep(ref reader);
          try {
            for (int i = 0; i < length; i++) {
              array[i] = objectFormatter.Deserialize(ref reader, options);
            }
          }
          finally {
            reader.Depth--;
          }

          return array;
        }

      case MessagePackType.Map: {
          int length = reader.ReadMapHeader();

          options.Security.DepthStep(ref reader);
          try {
            return DeserializeMap(ref reader, length, options);
          }
          finally {
            reader.Depth--;
          }
        }

      case MessagePackType.Nil:
        reader.ReadNil();
#pragma warning disable CS8603 // Possible null reference return.
        return null;
#pragma warning restore CS8603 // Possible null reference return.
      default:
        throw new MessagePackSerializationException("Invalid primitive bytes.");
      }
    }

    protected virtual object DeserializeMap(ref MessagePackReader reader, int length, MessagePackSerializerOptions options) {
      var objectFormatter = options.Resolver.GetFormatter<object>();
      var dictionary = new Dictionary<object, object>(length, options.Security.GetEqualityComparer<object>());
      for (int i = 0; i < length; i++) {
        object key = objectFormatter.Deserialize(ref reader, options);
        object value = objectFormatter.Deserialize(ref reader, options);
        dictionary.Add(key, value);
      }

      return dictionary;
    }
  }
}
