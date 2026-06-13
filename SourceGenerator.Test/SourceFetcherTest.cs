using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SourceGenerator.Test {

  public sealed class SourceFetcherTest : IDisposable {
    private const string Revision = "0123456789abcdef0123456789abcdef01234567";
    private static readonly byte[] _protocol = Encoding.UTF8.GetBytes(
      """
      {
        "enums": [{ "enumType": "ObsOutputState", "enumIdentifiers": [] }],
        "events": [{ "eventType": "Test", "category": "General" }],
        "requests": [{ "requestType": "Test", "category": "General" }]
      }
      """);
    private static readonly byte[] _obsHeader = Encoding.UTF8.GetBytes("namespace OBS { }\n");

    private readonly string _directory = Path.Combine(
      Path.GetTempPath(),
      $"ObsStrawket.SourceGenerator.Test.{Guid.NewGuid():N}");
    private readonly string _upstreamDirectory;

    public SourceFetcherTest() {
      Directory.CreateDirectory(_directory);
      _upstreamDirectory = new FakeDirectoryHelper(_directory).UpstreamDirectory;
    }

    [Fact]
    public async Task PrepareAsyncUsesTrackedSourcesWithoutHttpRequests() {
      WriteTrackedSources(_protocol, _obsHeader);
      var handler = new FakeHttpMessageHandler([]);
      var fetcher = CreateFetcher(handler);

      await fetcher.PrepareAsync(false);

      Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task PrepareAsyncUpdatesBothSourcesFromOneRevision() {
      var handler = CreateSuccessfulHandler();
      var fetcher = CreateFetcher(handler);

      await fetcher.PrepareAsync(true);

      Assert.Equal(_protocol, File.ReadAllBytes(Path.Combine(_upstreamDirectory, "protocol.json")));
      Assert.Equal(_obsHeader, File.ReadAllBytes(Path.Combine(_upstreamDirectory, "Obs.h")));
      Assert.Equal(3, handler.Requests.Count);
      Assert.Single(handler.Requests, uri => uri.Host == "api.github.com");
      Assert.Equal(2, handler.Requests.Count(uri => uri.AbsoluteUri.Contains(Revision)));
      using var document = JsonDocument.Parse(
        File.ReadAllBytes(Path.Combine(_upstreamDirectory, "upstream-revision.json")));
      Assert.Equal(Revision, document.RootElement.GetProperty("revision").GetString());
    }

    [Fact]
    public async Task PrepareAsyncPreservesTrackedSourcesWhenDownloadFails() {
      byte[] oldProtocol = [.. _protocol, (byte)0x20];
      byte[] oldObsHeader = [.. _obsHeader, (byte)0x20];
      WriteTrackedSources(oldProtocol, oldObsHeader);
      byte[] oldRevision = File.ReadAllBytes(Path.Combine(_upstreamDirectory, "upstream-revision.json"));
      var handler = new FakeHttpMessageHandler([
        Response(HttpStatusCode.OK, $"[{{\"sha\":\"{Revision}\"}}]"),
        Response(HttpStatusCode.OK, _protocol),
        Response(HttpStatusCode.InternalServerError, "failed"),
      ]);
      var fetcher = CreateFetcher(handler);

      await Assert.ThrowsAsync<HttpRequestException>(() => fetcher.PrepareAsync(true));

      Assert.Equal(oldProtocol, File.ReadAllBytes(Path.Combine(_upstreamDirectory, "protocol.json")));
      Assert.Equal(oldObsHeader, File.ReadAllBytes(Path.Combine(_upstreamDirectory, "Obs.h")));
      Assert.Equal(oldRevision, File.ReadAllBytes(Path.Combine(_upstreamDirectory, "upstream-revision.json")));
    }

    [Fact]
    public async Task PrepareAsyncPreservesTrackedSourcesWhenDownloadedProtocolIsInvalid() {
      WriteTrackedSources(_protocol, _obsHeader);
      byte[] oldRevision = File.ReadAllBytes(Path.Combine(_upstreamDirectory, "upstream-revision.json"));
      var handler = new FakeHttpMessageHandler([
        Response(HttpStatusCode.OK, $"[{{\"sha\":\"{Revision}\"}}]"),
        Response(HttpStatusCode.OK, "{}"),
        Response(HttpStatusCode.OK, _obsHeader),
      ]);
      var fetcher = CreateFetcher(handler);

      await Assert.ThrowsAsync<InvalidDataException>(() => fetcher.PrepareAsync(true));

      Assert.Equal(_protocol, File.ReadAllBytes(Path.Combine(_upstreamDirectory, "protocol.json")));
      Assert.Equal(_obsHeader, File.ReadAllBytes(Path.Combine(_upstreamDirectory, "Obs.h")));
      Assert.Equal(oldRevision, File.ReadAllBytes(Path.Combine(_upstreamDirectory, "upstream-revision.json")));
    }

    [Fact]
    public async Task PrepareAsyncRejectsInvalidTrackedProtocol() {
      WriteTrackedSources(_protocol, _obsHeader);
      File.WriteAllText(Path.Combine(_upstreamDirectory, "protocol.json"), "{}");
      var fetcher = CreateFetcher(new FakeHttpMessageHandler([]));

      var exception = await Assert.ThrowsAsync<InvalidDataException>(
        () => fetcher.PrepareAsync(false));

      Assert.Contains("complete protocol", exception.Message);
    }

    [Fact]
    public async Task PrepareAsyncRejectsMissingTrackedSource() {
      WriteTrackedSources(_protocol, _obsHeader);
      File.Delete(Path.Combine(_upstreamDirectory, "Obs.h"));
      var fetcher = CreateFetcher(new FakeHttpMessageHandler([]));

      var exception = await Assert.ThrowsAsync<FileNotFoundException>(
        () => fetcher.PrepareAsync(false));

      Assert.Contains("Obs.h is missing", exception.Message);
    }

    [Fact]
    public async Task PrepareAsyncRejectsMalformedRevision() {
      WriteTrackedSources(_protocol, _obsHeader);
      File.WriteAllText(Path.Combine(_upstreamDirectory, "upstream-revision.json"), "{");
      var fetcher = CreateFetcher(new FakeHttpMessageHandler([]));

      var exception = await Assert.ThrowsAsync<InvalidDataException>(
        () => fetcher.PrepareAsync(false));

      Assert.Contains("invalid JSON", exception.Message);
    }

    [Theory]
    [InlineData("--unknown")]
    [InlineData("--update-upstream", "--unknown")]
    public void ParseUpdateUpstreamRejectsUnknownArguments(params string[] args) {
      Assert.Throws<ArgumentException>(() => Program.ParseUpdateUpstream(args));
    }

    [Fact]
    public void ParseUpdateUpstreamAcceptsSupportedArguments() {
      Assert.False(Program.ParseUpdateUpstream([]));
      Assert.True(Program.ParseUpdateUpstream(["--update-upstream"]));
    }

    public void Dispose() {
      Directory.Delete(_directory, true);
    }

    private SourceFetcher CreateFetcher(FakeHttpMessageHandler handler) {
      return new SourceFetcher(new FakeDirectoryHelper(_directory), new HttpClient(handler));
    }

    private static FakeHttpMessageHandler CreateSuccessfulHandler() {
      return new FakeHttpMessageHandler([
        Response(HttpStatusCode.OK, $"[{{\"sha\":\"{Revision}\"}}]"),
        Response(HttpStatusCode.OK, _protocol),
        Response(HttpStatusCode.OK, _obsHeader),
      ]);
    }

    private void WriteTrackedSources(byte[] protocol, byte[] obsHeader) {
      Directory.CreateDirectory(_upstreamDirectory);
      File.WriteAllBytes(Path.Combine(_upstreamDirectory, "protocol.json"), protocol);
      File.WriteAllBytes(Path.Combine(_upstreamDirectory, "Obs.h"), obsHeader);
      var revision = new {
        repository = "https://github.com/obsproject/obs-websocket",
        revision = Revision,
        commitUrl = $"https://github.com/obsproject/obs-websocket/commit/{Revision}",
        files = new Dictionary<string, object> {
          ["protocol.json"] = new {
            sourcePath = "docs/generated/protocol.json",
          },
          ["Obs.h"] = new {
            sourcePath = "src/utils/Obs.h",
          },
        },
      };
      File.WriteAllText(
        Path.Combine(_upstreamDirectory, "upstream-revision.json"),
        JsonSerializer.Serialize(revision));
    }

    private static HttpResponseMessage Response(HttpStatusCode status, string contents) {
      return Response(status, Encoding.UTF8.GetBytes(contents));
    }

    private static HttpResponseMessage Response(HttpStatusCode status, byte[] contents) {
      return new HttpResponseMessage(status) {
        Content = new ByteArrayContent(contents),
      };
    }

    private sealed class FakeDirectoryHelper(string solutionDirectory) : IDirectoryHelper {
      public string SolutionDirectory { get; } = solutionDirectory;
      public string UpstreamDirectory => Path.Combine(SolutionDirectory, "SourceGenerator", "Upstream");
      public string MainProjectDirectory => Path.Combine(SolutionDirectory, "ObsStrawket");
      public string ObsClientPath => Path.Combine(MainProjectDirectory, "ObsClientSocket.cs");
    }

    private sealed class FakeHttpMessageHandler(IEnumerable<HttpResponseMessage> responses)
      : HttpMessageHandler {
      private readonly Queue<HttpResponseMessage> _responses = new(responses);

      public List<Uri> Requests { get; } = [];

      protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) {
        Requests.Add(request.RequestUri!);
        if (_responses.Count == 0) {
          throw new InvalidOperationException($"Unexpected HTTP request: {request.RequestUri}");
        }
        return Task.FromResult(_responses.Dequeue());
      }
    }
  }
}
