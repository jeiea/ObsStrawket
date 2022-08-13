namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;
  using MessagePack.Resolvers;

  public class OpcodeMessageResolver : IFormatterResolver {
    public static OpcodeMessageResolver Instance = new();

    private OpcodeMessageResolver() { }

    public IMessagePackFormatter<T>? GetFormatter<T>() {
      return Cache<T>.Formatter ?? StandardResolver.Instance.GetFormatter<T>();
    }

    private static class Cache<T> {
      public static IMessagePackFormatter<T>? Formatter;

      static Cache() {
        if (typeof(T) == typeof(IOpcodeMessage)) {
          Formatter = (IMessagePackFormatter<T>)new OpcodeMessageFormatter();
        }
      }
    }
  }
}
