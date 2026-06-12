using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SourceGenerator {

  internal interface ISourceFetcher {

    Task PrepareAsync(bool updateUpstream);

    Task<string> GetObsHeaderAsync();

    Task<ProtocolJson> GetModifiedProtocolJsonAsync();
  }

  internal class SourceFetcher : ISourceFetcher {
    private const string Repository = "https://github.com/obsproject/obs-websocket";
    private const string ProtocolSourcePath = "docs/generated/protocol.json";
    private const string ObsHeaderSourcePath = "src/utils/Obs.h";
    private const string RevisionFileName = "upstream-revision.json";
    private const string ProtocolFileName = "protocol.json";
    private const string ObsHeaderFileName = "Obs.h";

    private static readonly JsonSerializerOptions _jsonOptions = new() {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    private static readonly JsonSerializerOptions _revisionJsonOptions = new() {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      WriteIndented = true,
    };

    private readonly HttpClient _http;
    private readonly string _upstreamDirectory;

    public SourceFetcher(IDirectoryHelper directoryHelper, HttpClient http) {
      _upstreamDirectory = directoryHelper.UpstreamDirectory;
      _http = http;
      if (!_http.DefaultRequestHeaders.UserAgent.Any()) {
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("ObsStrawket-SourceGenerator");
      }
    }

    public async Task PrepareAsync(bool updateUpstream) {
      if (updateUpstream) {
        await UpdateUpstreamAsync().ConfigureAwait(false);
      }

      await ValidateLocalSourcesAsync().ConfigureAwait(false);
    }

    public Task<string> GetObsHeaderAsync() {
      return File.ReadAllTextAsync(GetPath(ObsHeaderFileName));
    }

    public async Task<ProtocolJson> GetModifiedProtocolJsonAsync() {
      var protocol = await ReadProtocolAsync(GetPath(ProtocolFileName)).ConfigureAwait(false);
      PatchProtocolJson(protocol);
      return protocol;
    }

    private async Task UpdateUpstreamAsync() {
      string revision = await GetLatestRevisionAsync().ConfigureAwait(false);
      var protocolBytesTask = _http.GetByteArrayAsync(GetRawUrl(revision, ProtocolSourcePath));
      var obsHeaderBytesTask = _http.GetByteArrayAsync(GetRawUrl(revision, ObsHeaderSourcePath));
      await Task.WhenAll(protocolBytesTask, obsHeaderBytesTask).ConfigureAwait(false);

      byte[] protocolBytes = await protocolBytesTask.ConfigureAwait(false);
      byte[] obsHeaderBytes = await obsHeaderBytesTask.ConfigureAwait(false);
      ValidateProtocol(protocolBytes);
      ValidateObsHeader(obsHeaderBytes);

      var revisionInfo = new UpstreamRevision {
        Repository = Repository,
        Revision = revision,
        CommitUrl = $"{Repository}/commit/{revision}",
        Files = new Dictionary<string, UpstreamFile> {
          [ProtocolFileName] = new() {
            SourcePath = ProtocolSourcePath,
          },
          [ObsHeaderFileName] = new() {
            SourcePath = ObsHeaderSourcePath,
          },
        },
      };
      byte[] revisionBytes = JsonSerializer.SerializeToUtf8Bytes(revisionInfo, _revisionJsonOptions);

      Directory.CreateDirectory(_upstreamDirectory);
      ReplaceFiles([
        new PendingFile(GetPath(ProtocolFileName), protocolBytes),
        new PendingFile(GetPath(ObsHeaderFileName), obsHeaderBytes),
        new PendingFile(GetPath(RevisionFileName), revisionBytes),
      ]);
    }

    private async Task ValidateLocalSourcesAsync() {
      string revisionPath = GetPath(RevisionFileName);
      if (!File.Exists(revisionPath)) {
        throw new FileNotFoundException(
          $"{RevisionFileName} is missing. Run the source generator with --update-upstream.",
          revisionPath);
      }

      UpstreamRevision revision;
      try {
        await using var stream = File.OpenRead(revisionPath);
        revision = (await JsonSerializer.DeserializeAsync<UpstreamRevision>(
          stream,
          _revisionJsonOptions).ConfigureAwait(false))
          ?? throw new InvalidDataException($"{RevisionFileName} is empty.");
      }
      catch (JsonException exception) {
        throw new InvalidDataException($"{RevisionFileName} contains invalid JSON.", exception);
      }

      ValidateRevision(revision);
      foreach (string fileName in revision.Files.Keys) {
        string path = GetPath(fileName);
        if (!File.Exists(path)) {
          throw new FileNotFoundException(
            $"{fileName} is missing. Run the source generator with --update-upstream.",
            path);
        }
      }

      ValidateProtocol(await File.ReadAllBytesAsync(GetPath(ProtocolFileName)).ConfigureAwait(false));
      ValidateObsHeader(await File.ReadAllBytesAsync(GetPath(ObsHeaderFileName)).ConfigureAwait(false));
    }

    private async Task<string> GetLatestRevisionAsync() {
      byte[] response = await _http.GetByteArrayAsync(
        "https://api.github.com/repos/obsproject/obs-websocket/commits?per_page=1").ConfigureAwait(false);
      var commits = JsonSerializer.Deserialize<List<GitHubCommit>>(response, _jsonOptions);
      string? revision = commits?.FirstOrDefault()?.Sha;
      if (revision == null || revision.Length != 40 || !revision.All(Uri.IsHexDigit)) {
        throw new InvalidDataException("GitHub returned an invalid obs-websocket revision.");
      }
      return revision.ToLowerInvariant();
    }

    private static void ValidateRevision(UpstreamRevision revision) {
      if (revision.Repository != Repository
          || !IsRevision(revision.Revision)
          || revision.CommitUrl != $"{Repository}/commit/{revision.Revision}"
          || revision.Files is not { Count: 2 } files
          || !files.TryGetValue(ProtocolFileName, out var protocol)
          || protocol?.SourcePath != ProtocolSourcePath
          || !files.TryGetValue(ObsHeaderFileName, out var obsHeader)
          || obsHeader?.SourcePath != ObsHeaderSourcePath) {
        throw new InvalidDataException($"{RevisionFileName} has an invalid structure.");
      }
    }

    private static void ValidateProtocol(byte[] bytes) {
      try {
        var protocol = JsonSerializer.Deserialize<ProtocolJson>(bytes, _jsonOptions);
        if (protocol == null
            || protocol.Enums.Count == 0
            || protocol.Events.Count == 0
            || protocol.Requests.Count == 0) {
          throw new InvalidDataException($"{ProtocolFileName} does not contain a complete protocol.");
        }
      }
      catch (JsonException exception) {
        throw new InvalidDataException($"{ProtocolFileName} contains invalid JSON.", exception);
      }
    }

    private static void ValidateObsHeader(byte[] bytes) {
      string header = Encoding.UTF8.GetString(bytes);
      if (!header.Contains("namespace OBS", StringComparison.Ordinal)
          && !header.Contains("namespace obs", StringComparison.OrdinalIgnoreCase)) {
        throw new InvalidDataException($"{ObsHeaderFileName} does not look like an OBS header.");
      }
    }

    private static void ReplaceFiles(IReadOnlyList<PendingFile> files) {
      string transactionId = Guid.NewGuid().ToString("N");
      var replacements = new List<FileReplacement>();
      try {
        foreach (var file in files) {
          string temporaryPath = $"{file.Path}.{transactionId}.tmp";
          File.WriteAllBytes(temporaryPath, file.Contents);
          replacements.Add(new FileReplacement(file.Path, temporaryPath, $"{file.Path}.{transactionId}.bak"));
        }

        foreach (var replacement in replacements) {
          if (File.Exists(replacement.DestinationPath)) {
            File.Move(replacement.DestinationPath, replacement.BackupPath);
          }
          File.Move(replacement.TemporaryPath, replacement.DestinationPath);
          replacement.Applied = true;
        }
      }
      catch {
        RollBack(replacements);
        throw;
      }
      finally {
        foreach (var replacement in replacements) {
          DeleteIfExists(replacement.TemporaryPath);
        }
      }

      foreach (var replacement in replacements) {
        DeleteIfExists(replacement.BackupPath);
      }
    }

    private static void RollBack(IEnumerable<FileReplacement> replacements) {
      foreach (var replacement in replacements.Reverse()) {
        if (replacement.Applied) {
          DeleteIfExists(replacement.DestinationPath);
        }
        if (File.Exists(replacement.BackupPath)) {
          File.Move(replacement.BackupPath, replacement.DestinationPath);
        }
      }
    }

    private static void DeleteIfExists(string path) {
      if (File.Exists(path)) {
        File.Delete(path);
      }
    }

    private static bool IsRevision(string? value) {
      return value?.Length == 40 && value.All(Uri.IsHexDigit);
    }

    private static string GetRawUrl(string revision, string sourcePath) {
      return $"https://raw.githubusercontent.com/obsproject/obs-websocket/{revision}/{sourcePath}";
    }

    private string GetPath(string fileName) {
      return Path.Combine(_upstreamDirectory, fileName);
    }

    private static async Task<ProtocolJson> ReadProtocolAsync(string path) {
      await using var stream = File.OpenRead(path);
      return (await JsonSerializer.DeserializeAsync<ProtocolJson>(
        stream,
        _jsonOptions).ConfigureAwait(false))!;
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
        "Canvases", "General", "Config", "Sources", "Scenes", "Inputs", "Transitions", "Filters",
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

        PatchRequest(request);
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

    private static void PatchRequest(ObsRequest request) {
      switch (request.RequestType) {
      case "ToggleRecordPause":
        request.ResponseFields.Add(new ObsDataField { ValueName = "outputPaused", ValueDescription = "Whether the output is paused", ValueType = "Boolean" });
        break;

      case "SaveSourceScreenshot":
        request.ResponseFields.Clear();
        break;
      }

      request.RequestFields = [
        .. request.RequestFields
          .OrderBy(field => field.ValueOptional)
          .ThenBy(field => field.ValueName == "canvasUuid"),
      ];
      foreach (var field in request.RequestFields) {
        if (GetCustomType(field.ValueName!, out string? type)) {
          field.ValueType = type;
        }
      }
      foreach (var field in request.ResponseFields) {
        if (GetCustomType(field.ValueName!, out string? type)) {
          field.ValueType = type;
        }
      }
    }

    private static bool GetCustomType(string name, out string? type) {
      switch (name) {
      case "filters":
        type = "Array<SourceFilter>";
        return true;

      case "inputAudioTracks":
        type = "Dictionary<string, bool>";
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

      case "outputs":
        type = "Array<Output>";
        return true;

      case "realm":
        type = "DataRealm";
        return true;

      case "sceneItemBlendMode":
        type = "BlendingType";
        return true;

      case "sceneItems":
        type = "Array<SceneItem>";
        return true;

      case "scenes":
        type = "Array<Scene>";
        return true;

      case "streamServiceType":
        type = "StreamServiceType";
        return true;

      case "transitions":
        type = "Array<AvailableTransition>";
        return true;

      case "videoMixType":
        type = "VideoMixType";
        return true;

      default:
        type = null;
        return false;
      }
    }

    private sealed record GitHubCommit {
      public string Sha { get; init; } = "";
    }

    private sealed record UpstreamRevision {
      public string Repository { get; init; } = "";
      public string Revision { get; init; } = "";
      public string CommitUrl { get; init; } = "";
      public Dictionary<string, UpstreamFile> Files { get; init; } = [];
    }

    private sealed record UpstreamFile {
      public string SourcePath { get; init; } = "";
    }

    private sealed record PendingFile(string Path, byte[] Contents);

    private sealed class FileReplacement(
      string destinationPath,
      string temporaryPath,
      string backupPath) {
      public string DestinationPath { get; } = destinationPath;
      public string TemporaryPath { get; } = temporaryPath;
      public string BackupPath { get; } = backupPath;
      public bool Applied { get; set; }
    }
  }
}
