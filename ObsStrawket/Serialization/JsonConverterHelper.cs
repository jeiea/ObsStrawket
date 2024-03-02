using System;
using System.Text.Json;

namespace ObsStrawket.Serialization {
  internal static class JsonConverterHelper {

    public static void CheckObjectStart(ref Utf8JsonReader reader) {
      if (reader.TokenType != JsonTokenType.StartObject) {
        throw new UnexpectedResponseException($"Expected object start, but found {reader.TokenType}.");
      }
    }

    public static T CreateInstance<T>(Type? type) where T : class {
      return (Activator.CreateInstance(type) as T)!;
    }

    public static Utf8JsonReader SeekByKey(Utf8JsonReader reader, string key) {
      if (!SeekByKey(ref reader, key)) {
        throw new UnexpectedResponseException($"Cannot find {key} key.");
      }
      return reader;
    }

    public static bool SeekByKey(ref Utf8JsonReader reader, string key) {
      while (true) {
        reader.Read();
        if (reader.TokenType == JsonTokenType.EndObject) {
          return false;
        }

        string? cursor = reader.GetString();
        reader.Read();

        if (cursor == key) {
          return true;
        }

        reader.Skip();
      }
    }
  }
}
