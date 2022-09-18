using System;
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

      foreach (var request in protocol.Requests) {
        var requestFields = request.RequestFields;
        if (requestFields == null) {
          continue;
        }

        PatchTriggerHotkeyByKeySequence(requestFields);
        PatchOthers(request);
      }

      return protocol;
    }

    private static void PatchOthers(ObsRequest request) {
      switch (request.RequestType) {
      case "GetOutputList":
        request.ResponseFields!.Add(new ObsDataFields { ValueName = "outputs", ValueDescription = "List of outputs", ValueType = "Output[]" });
        break;
      case "ToggleRecord":
        request.ResponseFields!.Add(new ObsDataFields { ValueName = "outputActive", ValueDescription = "Whether the output is active", ValueType = "Boolean" });
        break;
      case "ToggleRecordPause":
        request.ResponseFields!.Add(new ObsDataFields { ValueName = "outputPaused", ValueDescription = "Whether the output is paused", ValueType = "Boolean" });
        break;
      case "SaveSourceScreenshot":
        request.ResponseFields!.Clear();
        break;
      }
      foreach (var field in request.RequestFields!) {
        if (field.ValueName == "videoMixType") {
          field.ValueType = "VideoMixType";
        }
        else if (field.ValueName == "streamServiceType") {
          field.ValueType = "StreamServiceType";
        }
        else if (field.ValueDescription!.Contains("OBS_WEBSOCKET_DATA_REALM_GLOBAL")) {
          field.ValueType = "DataRealm";
        }
      }
    }

    private static void PatchTriggerHotkeyByKeySequence(List<ObsRequestField> requestFields) {
      int index = requestFields.FindIndex(x => x.ValueName == "keyModifiers");
      if (index == -1) {
        return;
      }

      requestFields.RemoveAt(index);
      foreach (var field in requestFields) {
        if (field.ValueName!.StartsWith("keyModifiers.")) {
          field.ValueName = field.ValueName.Replace("keyModifiers.", "");
        }
      }
    }

    public async Task<ProtocolJson> GetProtocolJsonAsync() {
      if (File.Exists(ProtocolJsonPath)) {
        return (await JsonSerializer.DeserializeAsync<ProtocolJson>(File.OpenRead(ProtocolJsonPath)).ConfigureAwait(false))!;
      }

      string protocolJson = await _http.GetStringAsync($"{_rawRoot}/docs/generated/protocol.json").ConfigureAwait(false);
      await File.WriteAllTextAsync(ProtocolJsonPath, protocolJson).ConfigureAwait(false);
      return JsonSerializer.Deserialize<ProtocolJson>(protocolJson)!;
    }
  }
}
