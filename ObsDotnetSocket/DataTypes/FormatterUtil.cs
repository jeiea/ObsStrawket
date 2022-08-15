namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using System;

  class UnexpectedProtocolException : Exception {
    public UnexpectedProtocolException(string? message = null) : base(message ?? "It's not seem to be obs websocket message") { }
  }

  internal static class FormatterUtil {
    public static MessagePackReader SeekByKey(MessagePackReader reader, string key) {
      if (!SeekByKey(ref reader, key)) {
        throw new UnexpectedProtocolException();
      }
      return reader;
    }

    public static bool SeekByKey(ref MessagePackReader reader, string key) {
      int count = reader.ReadMapHeader();
      for (int i = 0; i < count; i++) {
        string cursor = reader.ReadString();
        if (cursor == key) {
          return true;
        }
        reader.Skip();
      }
      return false;
    }
  }
}