using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SourceGenerator.Test {

  public sealed class GeneratorOutputTest : IDisposable {
    private readonly string _directory = Path.Combine(
      Path.GetTempPath(),
      $"ObsStrawket.GeneratorOutputTest.{Guid.NewGuid():N}");
    private readonly FakeDirectoryHelper _directoryHelper;

    public GeneratorOutputTest() {
      _directoryHelper = new FakeDirectoryHelper(_directory);
      _ = Directory.CreateDirectory(Path.Combine(
        _directoryHelper.MainProjectDirectory,
        "DataTypes",
        "Predefineds"));
    }

    [Fact]
    public async Task RequestGeneratorSuppressesOnlyKnownProtocolNames() {
      var protocol = new ProtocolJson {
        Requests = [
          Request("StartStream"),
          Request("OrdinaryRequest"),
        ],
      };
      var generator = new RequestResponseGenerator(
        _directoryHelper,
        new FakeSourceFetcher(protocol));

      await generator.GenerateAsync();

      string output = File.ReadAllText(Path.Combine(
        _directoryHelper.MainProjectDirectory,
        "DataTypes",
        "Predefineds",
        "RequestsAndResponses.cs")).ReplaceLineEndings("\n");
      Assert.Contains(
        """
#pragma warning disable CA1711
  public class StartStream : Request { }
#pragma warning restore CA1711
""".ReplaceLineEndings("\n"),
        output);
      Assert.Contains("  public class OrdinaryRequest : Request { }", output);
      Assert.Equal(1, Count(output, "#pragma warning disable CA1711"));
    }

    [Fact]
    public async Task EnumGeneratorUsesSystemTextJsonEnumNames() {
      var protocol = new ProtocolJson {
        Enums = [
          StringEnum(
            "ObsOutputState",
            "OBS_WEBSOCKET_OUTPUT_STARTED",
            "Started"),
          StringEnum(
            "ObsMediaInputAction",
            "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY",
            "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY"),
        ],
      };
      var generator = new EnumGenerator(
        _directoryHelper,
        new FakeSourceFetcher(protocol));

      await generator.GenerateAsync();

      string output = File.ReadAllText(Path.Combine(
        _directoryHelper.MainProjectDirectory,
        "DataTypes",
        "Predefineds",
        "Enums.cs"));
      Assert.Contains(
        "[JsonConverter(typeof(JsonStringEnumConverter<ObsOutputState>))]",
        output);
      Assert.Contains(
        "[JsonStringEnumMemberName(\"OBS_WEBSOCKET_OUTPUT_STARTED\")]",
        output);
      Assert.DoesNotContain("JsonStringEnumMemberConverter", output);
    }

    [Fact]
    public async Task RequestInterfaceGeneratorDocumentsPublicFailureContract() {
      var protocol = new ProtocolJson {
        Requests = [
          new ObsRequest {
            RequestType = "TriggerHotkeyByKeySequence",
            Description = "Trigger a hotkey.",
            RpcVersion = "1",
            InitialVersion = "5.0.0",
            RequestFields = [
              new ObsRequestField {
                ValueName = "keyModifiers",
                ValueType = "Object",
                ValueDescription = "Key modifiers.",
              },
            ],
          },
          Request("OrdinaryRequest"),
          Request("Sleep"),
        ],
      };
      File.WriteAllText(
        _directoryHelper.ObsClientPath,
        """
namespace ObsStrawket {
  public class ObsClientSocket {
    #region Requests
    #endregion
  }
}
""".ReplaceLineEndings("\r\n"));
      var generator = new RequestInterfaceGenerator(
        _directoryHelper,
        new FakeSourceFetcher(protocol));

      await generator.GenerateAsync();

      string output = File.ReadAllText(_directoryHelper.ObsClientPath);
      Assert.Contains(
        """<exception cref="ObsRequestException">OBS rejects the request.</exception>""",
        output);
      Assert.Contains(
        """<exception cref="ObsConnectionException">The connection fails before a response is received.</exception>""",
        output);
      Assert.Contains(
        """<exception cref="ObsProtocolException">OBS sends an invalid response.</exception>""",
        output);
    }

    public void Dispose() {
      Directory.Delete(_directory, true);
    }

    private static ObsRequest Request(string requestType) {
      return new ObsRequest {
        RequestType = requestType,
        Description = requestType,
        RpcVersion = "1",
        InitialVersion = "5.0.0",
      };
    }

    private static ObsEnumDefinition StringEnum(
      string enumType,
      string enumValue,
      string enumIdentifier) {
      return new ObsEnumDefinition {
        EnumType = enumType,
        EnumIdentifiers = [
          new ObsEnumIdentifier {
            Description = enumIdentifier,
            EnumIdentifier = enumIdentifier,
            EnumValue = enumValue,
            RpcVersion = "1",
            InitialVersion = "5.0.0",
          },
        ],
      };
    }

    private static int Count(string value, string fragment) {
      int count = 0;
      int index = 0;
      while ((index = value.IndexOf(fragment, index, StringComparison.Ordinal)) >= 0) {
        count++;
        index += fragment.Length;
      }
      return count;
    }

    private sealed class FakeSourceFetcher(ProtocolJson protocol) : ISourceFetcher {
      public Task PrepareAsync(bool updateUpstream) {
        return Task.CompletedTask;
      }

      public Task<string> GetObsHeaderAsync() {
        return Task.FromResult("");
      }

      public Task<ProtocolJson> GetModifiedProtocolJsonAsync() {
        return Task.FromResult(protocol);
      }
    }

    private sealed class FakeDirectoryHelper(string solutionDirectory) : IDirectoryHelper {
      public string SolutionDirectory { get; } = solutionDirectory;
      public string UpstreamDirectory => Path.Combine(SolutionDirectory, "SourceGenerator", "Upstream");
      public string MainProjectDirectory => Path.Combine(SolutionDirectory, "ObsStrawket");
      public string ObsClientPath => Path.Combine(MainProjectDirectory, "ObsClientSocket.cs");
    }
  }
}
