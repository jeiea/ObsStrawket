using System.IO;

namespace SourceGenerator {
  internal interface IDirectoryHelper {
    string SolutionDirectory { get; }
    string UpstreamDirectory { get; }
    string MainProjectDirectory { get; }
    string ObsClientEventsPath { get; }
    string ObsClientRequestsPath { get; }
  }

  internal class DirectoryHelper : IDirectoryHelper {
    public string SolutionDirectory { get; } = GetSolutionDirectory();

    public string UpstreamDirectory => Path.Combine(SolutionDirectory, "SourceGenerator", "Upstream");
    public string MainProjectDirectory => Path.Combine(SolutionDirectory, "ObsStrawket");
    public string ObsClientEventsPath => $"{MainProjectDirectory}/ObsClientSocket.Events.Generated.cs";
    public string ObsClientRequestsPath => $"{MainProjectDirectory}/ObsClientSocket.Requests.Generated.cs";

    private static string GetSolutionDirectory() {
      string? current = Directory.GetCurrentDirectory();
      while (current != null) {
        if (File.Exists(Path.Combine(current, "ObsStrawket.sln"))) {
          return current;
        }

        current = Path.GetDirectoryName(current);
      }
      throw new FileNotFoundException("ObsStrawket.sln is not found. Please check cwd.");
    }
  }
}
