using ObsStrawket.DataTypes;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObsStrawket.Serialization {

  internal sealed class OutputFlagsMapConverter : JsonConverter<OutputFlags> {

    public override OutputFlags Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      if (reader.TokenType != JsonTokenType.StartObject) {
        throw new JsonException();
      }

      OutputFlags result = 0;
      while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
        string name = reader.GetString() ?? throw new JsonException();
        reader.Read();
        if (reader.GetBoolean()) {
          result |= name switch {
            "OBS_OUTPUT_VIDEO" => OutputFlags.Video,
            "OBS_OUTPUT_AUDIO" => OutputFlags.Audio,
            "OBS_OUTPUT_ENCODED" => OutputFlags.Encoded,
            "OBS_OUTPUT_SERVICE" => OutputFlags.Service,
            "OBS_OUTPUT_MULTI_TRACK" => OutputFlags.MultiTrack,
            "OBS_OUTPUT_CAN_PAUSE" => OutputFlags.CanPause,
            _ => 0,
          };
        }
      }

      return result;
    }

    public override void Write(Utf8JsonWriter writer, OutputFlags value, JsonSerializerOptions options) {
      writer.WriteStartObject();
      writer.WriteBoolean("OBS_OUTPUT_VIDEO", value.HasFlag(OutputFlags.Video));
      writer.WriteBoolean("OBS_OUTPUT_AUDIO", value.HasFlag(OutputFlags.Audio));
      writer.WriteBoolean("OBS_OUTPUT_ENCODED", value.HasFlag(OutputFlags.Encoded));
      writer.WriteBoolean("OBS_OUTPUT_SERVICE", value.HasFlag(OutputFlags.Service));
      writer.WriteBoolean("OBS_OUTPUT_MULTI_TRACK", value.HasFlag(OutputFlags.MultiTrack));
      writer.WriteBoolean("OBS_OUTPUT_CAN_PAUSE", value.HasFlag(OutputFlags.CanPause));
      writer.WriteEndObject();
    }
  }
}
