using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SourceGenerator {
  public record class ProtocolJson {
    [JsonPropertyName("events")]
    public List<ObsEvent> Events { get; set; } = new();

    [JsonPropertyName("requests")]
    public List<ObsRequest> Requests { get; set; } = new();
  }

  public record class ObsEvent {
    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("eventSubscription")]
    public string EventSubscription { get; set; } = "";

    [JsonPropertyName("complexity")]
    public int Complexity { get; set; }

    [JsonPropertyName("rpcVersion")]
    public string RpcVersion { get; set; } = "";

    [JsonPropertyName("deprecated")]
    public bool Deprecated { get; set; }

    [JsonPropertyName("initialVersion")]
    public string InitialVersion { get; set; } = "";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("dataFields")]
    public List<ObsDataFields>? DataFields { get; set; }
  }

  public record class ObsDataFields {
    [JsonPropertyName("valueName")]
    public string? ValueName { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("valueDescription")]
    public string? ValueDescription { get; set; }
  }

  public record class ObsRequestField {
    [JsonPropertyName("valueName")]
    public string? ValueName { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("valueDescription")]
    public string? ValueDescription { get; set; }

    [JsonPropertyName("valueRestrictions")]
    public string? ValueRestrictions { get; set; }

    [JsonPropertyName("valueOptional")]
    public bool ValueOptional { get; set; }

    [JsonPropertyName("valueOptionalBehavior")]
    public string? ValueOptionalBehavior { get; set; }
  };

  public record class ObsRequest {
    [JsonPropertyName("requestType")]
    public string? RequestType { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("complexity")]
    public int Complexity { get; set; }

    [JsonPropertyName("rpcVersion")]
    public string? RpcVersion { get; set; }

    [JsonPropertyName("deprecated")]
    public bool Deprecated { get; set; }

    [JsonPropertyName("initialVersion")]
    public string? InitialVersion { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("requestFields")]
    public List<ObsRequestField>? RequestFields { get; set; }

    [JsonPropertyName("responseFields")]
    public List<ObsDataFields>? ResponseFields { get; set; }
  }
}
