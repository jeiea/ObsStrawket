using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal class SourceFetcher {
    private static readonly string ProtocolJsonPath = "protocol.json";
    private static readonly string ObsHeaderPath = "Obs.h";
    private static readonly string _rawRoot = "https://raw.githubusercontent.com/obsproject/obs-websocket/master";

    private readonly HttpClient _http = new();

    public async Task<string> GetObsHeaderAsync() {
      if (File.Exists(ObsHeaderPath)) {
        return await File.ReadAllTextAsync(ObsHeaderPath).ConfigureAwait(false);
      }

      string obsHeader = await _http.GetStringAsync($"{_rawRoot}/src/utils/Obs.h").ConfigureAwait(false);
      await File.WriteAllTextAsync(ObsHeaderPath, obsHeader).ConfigureAwait(false);
      return obsHeader;
    }

    public async Task<ProtocolJson> GetModifiedProtocolJsonAsync() {
      var protocol = await GetProtocolJsonAsync().ConfigureAwait(false);
      PatchProtocolJson(protocol);
      return protocol;
    }

    public async Task<ProtocolJson> GetProtocolJsonAsync() {
      var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
      if (File.Exists(ProtocolJsonPath)) {
        return (await JsonSerializer.DeserializeAsync<ProtocolJson>(File.OpenRead(ProtocolJsonPath), options).ConfigureAwait(false))!;
      }

      string protocolJson = await _http.GetStringAsync($"{_rawRoot}/docs/generated/protocol.json").ConfigureAwait(false);
      await File.WriteAllTextAsync(ProtocolJsonPath, protocolJson).ConfigureAwait(false);
      return JsonSerializer.Deserialize<ProtocolJson>(protocolJson, options)!;
    }

    /// <summary>
    /// Fix various issues while preserving protocol json structure.
    /// </summary>
    private static void PatchProtocolJson(ProtocolJson protocol) {
      FixDeprecateEnums(protocol);

      var outputState = protocol.Enums.First(x => x.EnumType == "ObsOutputState");
      foreach (var identifier in outputState.EnumIdentifiers) {
        string essence = identifier.EnumIdentifier.Replace("OBS_WEBSOCKET_OUTPUT_", "");
        identifier.EnumIdentifier = TransformHelper.ToPascalCase(essence);
      }


      var categoryOrder = new List<string>() {
        "General", "Config", "Sources", "Scenes", "Inputs", "Transitions", "Filters",
        "Scene Items", "Outputs", "Stream", "Record", "Media Inputs", "Ui", "High-Volume",
      }.Select(x => x.ToLower()).ToList();

      protocol.Events = protocol.Events.OrderBy(x => categoryOrder.IndexOf(x.Category!)).ToList();
      foreach (var ev in protocol.Events) {
        var dataFields = ev.DataFields;
        if (dataFields == null) {
          continue;
        }

        foreach (var field in ev.DataFields!) {
          if (field.ValueName == "sceneItems") {
            field.ValueType = "Array<BasicSceneItem>";
          }
          else if (GetCustomType(field.ValueName!, out string? type)) {
            field.ValueType = type;
          }
        }
      }

      protocol.Requests = protocol.Requests.OrderBy(x => categoryOrder.IndexOf(x.Category!)).ToList();
      foreach (var request in protocol.Requests) {
        var requestFields = request.RequestFields;
        if (requestFields == null) {
          continue;
        }

        PatchOthers(request);
      }
    }

    private static void FixDeprecateEnums(ProtocolJson protocol) {
      // There is no alternative, it seems to be a mistake.
      var obsMediaInputAction = protocol.Enums.First((en) => en.EnumType == "ObsMediaInputAction").EnumIdentifiers;
      var obsOutputState = protocol.Enums.First((en) => en.EnumType == "ObsOutputState").EnumIdentifiers;
      foreach (var identifier in obsMediaInputAction.Concat(obsOutputState)) {
        identifier.Deprecated = false;
      }
    }

    private static void PatchOthers(ObsRequest request) {
      switch (request.RequestType) {
      case "ToggleRecord":
        request.ResponseFields!.Add(new ObsDataField { ValueName = "outputActive", ValueDescription = "Whether the output is active", ValueType = "Boolean" });
        break;
      case "ToggleRecordPause":
        request.ResponseFields!.Add(new ObsDataField { ValueName = "outputPaused", ValueDescription = "Whether the output is paused", ValueType = "Boolean" });
        break;
      case "SaveSourceScreenshot":
        request.ResponseFields!.Clear();
        break;
      }

      foreach (var field in request.RequestFields!) {
        if (GetCustomType(field.ValueName!, out string? type)) {
          field.ValueType = type;
        }
      }
      foreach (var field in request.ResponseFields!) {
        if (GetCustomType(field.ValueName!, out string? type)) {
          field.ValueType = type;
        }
      }
    }

    private static bool GetCustomType(string name, out string? type) {
      switch (name) {
      case "outputs":
        type = "Array<Output>";
        return true;
      case "filters":
        type = "Array<SourceFilter>";
        return true;
      case "inputs":
        type = "Array<Input>";
        return true;
      case "keyModifiers":
        type = "KeyModifiers";
        return true;
      case "mediaAction":
        type = "MediaInputAction";
        return true;
      case "mediaState":
        type = "MediaState";
        return true;
      case "monitorType":
        type = "MonitoringType";
        return true;
      case "outputState":
        type = "ObsOutputState";
        return true;
      case "scenes":
        type = "Array<Scene>";
        return true;
      case "sceneItems":
        type = "Array<SceneItem>";
        return true;
      case "sceneItemBlendMode":
        type = "BlendingType";
        return true;
      case "streamServiceType":
        type = "StreamServiceType";
        return true;
      case "transitions":
        type = "Array<AvailableTransition>";
        return true;
      case "realm":
        type = "DataRealm";
        return true;
      case "videoMixType":
        type = "VideoMixType";
        return true;
      default:
        type = null;
        return false;
      }
    }
  }
}
