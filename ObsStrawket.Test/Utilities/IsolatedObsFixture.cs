using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Utilities {

  /// <summary>
  /// Launches the locally installed OBS as an isolated portable instance.
  /// Junctions reuse the read-only installation while every write goes to a
  /// temporary config directory, so the user's OBS settings and any running
  /// OBS instance are never touched.
  /// </summary>
  public sealed class IsolatedObsFixture : IAsyncLifetime {
    private static readonly TimeSpan _bootTimeout = TimeSpan.FromSeconds(60);
    private static readonly string[] _junctionNames = ["bin", "data", "obs-plugins"];

    public bool IsAvailable => Uri != null;
    public Uri? Uri { get; private set; }
    public string Password { get; } = MockServer.Password;

    private string? _root;
    private Process? _process;

    public async ValueTask InitializeAsync() {
      if (FindInstalledObs() is not string installation) {
        return;
      }

      int port = GetFreeTcpPort();
      _root = Path.Combine(Path.GetTempPath(), "obsstrawket-test", $"{Guid.NewGuid():N}");
      SeedPortableRoot(_root, installation, port);

      string binDirectory = Path.Combine(_root, "bin", "64bit");
      _process = Process.Start(new ProcessStartInfo {
        FileName = Path.Combine(binDirectory, "obs64.exe"),
        WorkingDirectory = binDirectory,
        Arguments = "--portable --multi --minimize-to-tray --disable-updater"
          + " --disable-shutdown-check --disable-missing-files-check",
        UseShellExecute = false,
      })!;

      await WaitUntilListeningAsync(port).ConfigureAwait(false);
      Uri = new Uri($"ws://127.0.0.1:{port}");
    }

    public async ValueTask DisposeAsync() {
      if (_process is { } process) {
        if (!process.HasExited) {
          process.CloseMainWindow();
          if (!process.WaitForExit(10_000)) {
            process.Kill(entireProcessTree: true);
            process.WaitForExit(10_000);
          }
        }
        process.Dispose();
      }
      if (_root != null) {
        await DeletePortableRootAsync(_root).ConfigureAwait(false);
      }
    }

    private static string? FindInstalledObs() {
      if (!OperatingSystem.IsWindows()) {
        return null;
      }

      string? registered =
        Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\OBS Studio", null, null) as string;
      string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
      foreach (string? candidate in new[] { registered, Path.Combine(programFiles, "obs-studio") }) {
        if (candidate != null && File.Exists(Path.Combine(candidate, "bin", "64bit", "obs64.exe"))) {
          return candidate;
        }
      }
      return null;
    }

    private static void SeedPortableRoot(string root, string installation, int port) {
      Directory.CreateDirectory(root);
      foreach (string name in _junctionNames) {
        CreateJunction(Path.Combine(root, name), Path.Combine(installation, name));
      }
      File.WriteAllText(Path.Combine(root, "portable_mode.txt"), "");

      string config = Path.Combine(root, "config", "obs-studio");
      // FirstRun suppresses the auto-configuration wizard on a fresh config.
      string generalIni = """
        [General]
        FirstRun=true
        Pre31Migrated=true

        """;
      Directory.CreateDirectory(config);
      File.WriteAllText(Path.Combine(config, "global.ini"), generalIni);
      File.WriteAllText(Path.Combine(config, "user.ini"), generalIni);

      // Keep recordings from tests inside the disposable root.
      string recordings = Path.Combine(root, "recordings");
      Directory.CreateDirectory(recordings);
      string profile = Path.Combine(config, "basic", "profiles", "Untitled");
      Directory.CreateDirectory(profile);
      // OBS ini values treat backslashes as escapes; use forward slashes.
      File.WriteAllText(Path.Combine(profile, "basic.ini"), $"""
        [SimpleOutput]
        FilePath={recordings.Replace('\\', '/')}

        """);

      // CLI --websocket_port cannot enable the server, only the config can.
      string websocket = Path.Combine(config, "plugin_config", "obs-websocket");
      Directory.CreateDirectory(websocket);
      File.WriteAllText(Path.Combine(websocket, "config.json"), $$"""
        {
          "alerts_enabled": false,
          "auth_required": true,
          "first_load": false,
          "server_enabled": true,
          "server_password": "{{MockServer.Password}}",
          "server_port": {{port}}
        }
        """);
    }

    private static void CreateJunction(string link, string target) {
      // Directory junctions need no privilege, unlike symbolic links.
      var mklink = Process.Start(new ProcessStartInfo {
        FileName = "cmd.exe",
        Arguments = $"/c mklink /J \"{link}\" \"{target}\"",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
      })!;
      mklink.WaitForExit();
      if (mklink.ExitCode != 0) {
        throw new IOException($"mklink /J failed: {mklink.StandardError.ReadToEnd()}");
      }
    }

    private async Task WaitUntilListeningAsync(int port) {
      var deadline = DateTime.UtcNow + _bootTimeout;
      while (DateTime.UtcNow < deadline) {
        if (_process!.HasExited) {
          throw new InvalidOperationException($"OBS exited with code {_process.ExitCode}.{GetLogTail()}");
        }
        try {
          using var probe = new TcpClient();
          await probe.ConnectAsync(IPAddress.Loopback, port).ConfigureAwait(false);
          return;
        }
        catch (SocketException) {
          await Task.Delay(500).ConfigureAwait(false);
        }
      }
      throw new TimeoutException($"obs-websocket did not listen within {_bootTimeout}.{GetLogTail()}");
    }

    private string GetLogTail() {
      try {
        string logs = Path.Combine(_root!, "config", "obs-studio", "logs");
        var newest = new DirectoryInfo(logs).GetFiles().MaxBy(file => file.LastWriteTimeUtc);
        if (newest == null) {
          return "";
        }
        using var reader = new StreamReader(newest.OpenRead());
        string[] lines = reader.ReadToEnd().Split('\n');
        return $"\nOBS log tail:\n{string.Join('\n', lines[Math.Max(0, lines.Length - 20)..])}";
      }
      catch (IOException) {
        return "";
      }
      catch (UnauthorizedAccessException) {
        return "";
      }
    }

    private static async Task DeletePortableRootAsync(string root) {
      foreach (string name in _junctionNames) {
        string junction = Path.Combine(root, name);
        if (Directory.Exists(junction)) {
          // Deleting a junction non-recursively removes the link only.
          Directory.Delete(junction, recursive: false);
        }
      }
      for (int attempt = 0; ; attempt++) {
        try {
          Directory.Delete(root, recursive: true);
          return;
        }
        catch (IOException) when (attempt < 5) {
          await Task.Delay(500).ConfigureAwait(false);
        }
        catch (IOException) {
          return; // Leftovers under %TEMP% are harmless.
        }
      }
    }

    private static int GetFreeTcpPort() {
      var listener = new TcpListener(IPAddress.Loopback, 0);
      listener.Start();
      int port = ((IPEndPoint)listener.LocalEndpoint).Port;
      listener.Stop();
      return port;
    }
  }

  [CollectionDefinition("IsolatedObs")]
  public class IsolatedObsCollection : ICollectionFixture<IsolatedObsFixture> { }
}
