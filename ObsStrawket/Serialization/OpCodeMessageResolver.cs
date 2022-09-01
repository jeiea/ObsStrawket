namespace ObsStrawket.Serialization {
  using MessagePack;
  using MessagePack.Formatters;
  using MessagePack.Resolvers;
  using ObsStrawket.DataTypes;

  public class OpCodeMessageResolver : IFormatterResolver {
    public static OpCodeMessageResolver Instance = new();

    private OpCodeMessageResolver() { }

    public IMessagePackFormatter<T>? GetFormatter<T>() {
      return Cache<T>.Formatter ?? StandardResolver.Instance.GetFormatter<T>();
    }

    private static class Cache<T> {
      public static IMessagePackFormatter<T>? Formatter;

      static Cache() {
        if (typeof(T) == typeof(IOpCodeMessage)) {
          Formatter = (IMessagePackFormatter<T>)new OpCodeMessageFormatter();
        }
      }
    }
  }
}
