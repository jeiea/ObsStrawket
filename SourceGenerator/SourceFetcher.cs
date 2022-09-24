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

      var categoryOrder = new List<string>() {
        "General", "Config", "Sources", "Scenes", "Inputs", "Transitions", "Filters",
        "Scene Items", "Outputs", "Stream", "Record", "Media Inputs", "Ui", "High-Volume",
      }.Select(x => x.ToLower()).ToList();
      protocol.Requests = protocol.Requests.OrderBy(x => categoryOrder.IndexOf(x.Category!)).ToList();
      protocol.Events = protocol.Events.OrderBy(x => categoryOrder.IndexOf(x.Category!)).ToList();

      foreach (var request in protocol.Requests) {
        var requestFields = request.RequestFields;
        if (requestFields == null) {
          continue;
        }

        PatchOthers(request);
      }
      foreach (var request in protocol.Events) {
        var requestFields = request.DataFields;
        if (requestFields == null) {
          continue;
        }

        foreach (var field in request.DataFields!) {
          if (field.ValueName == "sceneItems") {
            field.ValueType = "Array<BasicSceneItem>";
          }
          else if (GetCustomType(field.ValueName!, out string? type)) {
            field.ValueType = type;
          }
        }
      }


      return protocol;
    }

    public async Task<ProtocolJson> GetProtocolJsonAsync() {
      var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  };
      if (File.Exists(ProtocolJsonPath)) {
        return (await JsonSerializer.DeserializeAsync<ProtocolJson>(File.OpenRead(ProtocolJsonPath), options).ConfigureAwait(false))!;
      }

      string protocolJson = await _http.GetStringAsync($"{_rawRoot}/docs/generated/protocol.json").ConfigureAwait(false);
      await File.WriteAllTextAsync(ProtocolJsonPath, protocolJson).ConfigureAwait(false);
      return JsonSerializer.Deserialize<ProtocolJson>(protocolJson, options)!;
    }

    private static void PatchOthers(ObsRequest request) {
      switch (request.RequestType) {
      case "GetRecordStatus":
        var typoField = request.ResponseFields![1];
        if (typoField.ValueName == "ouputPaused") {
          typoField.ValueName = "outputPaused";
        }
        break;
      case "GetOutputList":
        request.ResponseFields!.Add(new ObsDataField { ValueName = "outputs", ValueDescription = "List of outputs", ValueType = "Array<Output>" });
        break;
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
      case "outputState":
        type = "OutputState";
        return true;
      case "monitorType":
        type = "MonitoringType";
        return true;
      case "videoMixType":
        type = "VideoMixType";
        return true;
      case "keyModifiers":
        type = "KeyModifiers";
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
      case "inputs":
        type = "Array<Input>";
        return true;
      case "streamServiceType":
        type = "StreamServiceType";
        return true;
      case "realm":
        type = "DataRealm";
        return true;
      default:
        type = null;
        return false;
      }
    }
  }
}
