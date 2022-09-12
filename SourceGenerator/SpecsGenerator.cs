using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal class SpecsGenerator {
    private readonly SourceFetcher _fetcher = new();

    public async Task GenerateAsync() {
      var requestFields = new Dictionary<string, string?> {
        { "realm", "\"test realm\"" },
        { "slotName", "\"test slot\"" },
        { "slotValue", "\"test slot value\"" },
        { "sceneCollectionName", "\"test scene collection\"" },
        { "profileName", "\"test profile name\"" },
        { "parameterCategory", "\"test parameter category\"" },
        { "parameterName", "\"test parameter name\"" },
        { "parameterValue", "\"test parameter value\"" },
        { "fpsNumerator", "60000" },
        { "fpsDenominator", "1001" },
        { "baseWidth", null },
        { "baseHeight", null },
        { "outputWidth", null },
        { "outputHeight", null },
        { "streamServiceType", "\"test stream service type\"" },
        { "streamServiceSettings", "new Dictionary<string, object?>()" },
        { "sourceName", "\"test source\"" },
        { "filterKind", "\"test filter kind\"" },
        { "filterName", "\"test filter name\"" },
        { "filterSettings", "new Dictionary<string, object?>()" },
        { "newFilterName", "\"test new filter name\"" },
        { "filterIndex", "1" },
        { "overlay", null },
        { "filterEnabled", "true" },
        { "eventData", "new Dictionary<string, object?>()" },
        { "vendorName", "\"test vendor name\"" },
        { "requestType", "\"test request type\"" },
        { "requestData", null },
        { "hotkeyName", "\"test hotkey\"" },
        { "keyId", null },
        { "shift", null },
        { "control", null },
        { "alt", null },
        { "command", null },
        { "sleepMillis", "17" },
        { "sleepFrames", "1" },
        { "inputKind", "\"test input kind\"" },
        { "unversioned", null },
        { "sceneName", "\"test scene\"" },
        { "inputName", "\"test input\"" },
        { "inputSettings", "new Dictionary<string, object?>()" },
        { "sceneItemEnabled", "true" },
        { "newInputName", "\"test new input\"" },
        { "inputMuted", "false" },
        { "inputVolumeMul", null },
        { "inputVolumeDb", null },
        { "inputAudioBalance", "0.5" },
        { "inputAudioSyncOffset", "100" },
        { "monitorType", "MonitoringType.Only" },
        { "inputAudioTracks", "new Dictionary<string, object?>()" },
        { "propertyName", "\"test property name\"" },
        { "mediaCursor", "5.5" },
        { "mediaCursorOffset", "10" },
        { "mediaAction", "\"test media action\"" },
        { "outputName", "\"test output\"" },
        { "outputSettings", "new Dictionary<string, object?>()" },
        { "searchOffset", null },
        { "sceneItemId", "1" },
        { "destinationSceneName", null },
        { "sceneItemTransform", "new Dictionary<string, object?>()" },
        { "sceneItemLocked", "true" },
        { "sceneItemIndex", "1" },
        { "sceneItemBlendMode", "\"OBS_BLEND_NORMAL\"" },
        { "newSceneName", "\"test new scene\"" },
        { "transitionName", "\"test transition\"" },
        { "transitionDuration", "0" },
        { "imageFormat", "\"webp\"" },
        { "imageWidth", "1920" },
        { "imageHeight", "1080" },
        { "imageCompressionQuality", null },
        { "imageFilePath", "$\"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/test-obs.webp\"" },
        { "captionText", "\"test caption text\"" },
        { "transitionSettings", "new Dictionary<string, object?>()" },
        { "position", "0" },
        { "release", null },
        { "studioModeEnabled", "true" },
        { "videoMixType", "\"test video mix type\"" },
        { "monitorIndex", null },
        { "projectorGeometry", null },
      };

      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);
      Directory.CreateDirectory("Specs");

      foreach (var request in json.Requests) {
        var fields = request.RequestFields!
          .Where(x => requestFields[x.ValueName] != null)
          .Select(x => $"{x.ValueName}: {requestFields[x.ValueName]}");
        string parameters = string.Join(", ", fields);
        var usings = new List<string>() {
          "using ObsStrawket.Test.Utilities;",
          "using System.Threading.Tasks;",
          "using Xunit;",
        };
        if (parameters.Contains("Dictionary<")) {
          usings.Add("using System.Collections.Generic;");
        }
        if (parameters.Contains("MonitoringType.")) {
          usings.Add("using ObsStrawket.DataTypes;");
        }
        if (parameters.Contains("Environment.SpecialFolder")) {
          usings.Add("using System;");
        }
        usings.Sort();
        await File.WriteAllTextAsync($"Specs/{request.RequestType}Test.cs", @"{usings}

namespace ObsStrawket.Test.Specs {
  public class {type}Test {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new {type}Flow()).ConfigureAwait(false);
    }
  }

  class {type}Flow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.{type}Async({requestParams}).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"""").ConfigureAwait(false);
      await session.SendAsync(@"""".Replace(""{guid}"", guid)).ConfigureAwait(false);
    }
  }
}
"
.Replace("{type}", request.RequestType)
.Replace("{usings}", string.Join("\r\n", usings))
.Replace("{requestParams}", parameters));
      }
    }
  }
}
