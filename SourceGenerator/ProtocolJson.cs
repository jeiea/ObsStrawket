using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SourceGenerator {
  public record class ProtocolJson {
    public List<ObsEnum> Enums { get; set; } = new();

    public List<ObsEvent> Events { get; set; } = new();

    public List<ObsRequest> Requests { get; set; } = new();
  }

  public record class ObsEnum {
    public string EnumType { get; set; } = "";

    public List<ObsEnumIdentifier> EnumIdentifiers { get; set; } = new();
  }

  public record class ObsEnumIdentifier {
    public string Description { get; set; } = "";

    public string EnumIdentifier { get; set; } = "";

    [JsonConverter(typeof(StringToNumberConverter))]
    public string RpcVersion { get; set; } = "";

    public bool Deprecated { get; set; }

    public string InitialVersion { get; set; } = "";

    public object EnumValue { get; set; } = 0;
  }

  public record class ObsEvent {
    public string EventType { get; set; } = "";

    public string Description { get; set; } = "";

    public string EventSubscription { get; set; } = "";

    public int Complexity { get; set; }

    public string RpcVersion { get; set; } = "";

    public bool Deprecated { get; set; }

    public string InitialVersion { get; set; } = "";

    public string Category { get; set; } = "";

    public List<ObsDataField>? DataFields { get; set; }
  }

  public record class ObsDataField {
    public string? ValueName { get; set; }

    public string? ValueType { get; set; }

    public string? ValueDescription { get; set; }
  }

  public record class ObsRequestField {
    public string? ValueName { get; set; }

    public string? ValueType { get; set; }

    public string? ValueDescription { get; set; }

    public string? ValueRestrictions { get; set; }

    public bool ValueOptional { get; set; }

    public string? ValueOptionalBehavior { get; set; }
  };

  public record class ObsRequest {
    public string? RequestType { get; set; }

    public string? Description { get; set; }

    public int Complexity { get; set; }

    public string? RpcVersion { get; set; }

    public bool Deprecated { get; set; }

    public string? InitialVersion { get; set; }

    public string? Category { get; set; }

    public List<ObsRequestField> RequestFields { get; set; } = [];

    public List<ObsDataField> ResponseFields { get; set; } = [];
  }

  internal class StringToNumberConverter : JsonConverter<string> {

    public StringToNumberConverter() : base() {
    }

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      if (reader.TokenType == JsonTokenType.Number) {
        return reader.GetInt32().ToString();
      }
      return reader.GetString()!;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) {
      throw new NotImplementedException();
    }
  }
}
