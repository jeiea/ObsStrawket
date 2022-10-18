using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using ObsStrawket.DataTypes;

namespace ObsStrawket.Serialization {
  /// <summary>
  /// Msgpack resolver for IOpcodeMessage deserialization.
  /// It is public due to library restriction and not intended for general use.
  /// </summary>
  public class OpCodeMessageResolver : IFormatterResolver {
    internal static OpCodeMessageResolver Instance = new();

    private OpCodeMessageResolver() { }

    /// <summary>
    /// Gets an <see cref="IMessagePackFormatter{T}"/> instance that can serialize or deserialize some type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value to be serialized or deserialized.</typeparam>
    /// <returns>A formatter, if this resolver supplies one for type <typeparamref name="T"/>; otherwise <c>null</c>.</returns>
    public IMessagePackFormatter<T>? GetFormatter<T>() {
      return Cache<T>.Formatter ?? StandardResolver.Instance.GetFormatter<T>();
    }

    private static class Cache<T> {
      public static IMessagePackFormatter<T>? Formatter;

      static Cache() {
        if (typeof(T) == typeof(IOpCodeMessage)) {
          Formatter = (IMessagePackFormatter<T>)OpCodeMessageFormatter.Instance;
        }
        else if (typeof(T) == typeof(IRequest)) {
          Formatter = (IMessagePackFormatter<T>)RequestFormatter.Instance;
        }
        else if (typeof(T) == typeof(IRequestResponse)) {
          Formatter = (IMessagePackFormatter<T>)RequestResponseFormatter.Instance;
        }
        else if (typeof(T) == typeof(object)) {
          Formatter = (IMessagePackFormatter<T>)PromotionFormatter.Instance;
        }
      }
    }
  }
}
