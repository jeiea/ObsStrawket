using System.IO;

namespace SourceGenerator {
  interface IDirectoryHelper {
    string MainProjectDirectory { get; }
    string ObsClientPath { get; }
  }

  internal class DirectoryHelper : IDirectoryHelper {
    private readonly string _solutionDirectory = GetSolutionDirectory();

    public string MainProjectDirectory => Path.Combine(_solutionDirectory, "ObsStrawket");
    public string ObsClientPath => $"{MainProjectDirectory}/ObsClientSocket.cs";

    static string GetSolutionDirectory() {
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
