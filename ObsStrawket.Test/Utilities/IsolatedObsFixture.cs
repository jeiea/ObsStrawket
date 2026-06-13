using ObsStrawket;
using ObsStrawket.DataTypes.Predefineds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
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
    private static readonly TimeSpan _windowTimeout = TimeSpan.FromSeconds(5);
    private static readonly string[] _junctionNames = ["bin", "data", "obs-plugins"];

    public bool IsAvailable => Uri != null;
    public bool HasExited => _process?.HasExited == true;
    public bool HasVendorPlugin => GetVendorPluginRoot() != null;
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
      string? pluginRoot = GetVendorPluginRoot();

      string binDirectory = Path.Combine(_root, "bin", "64bit");
      var startInfo = new ProcessStartInfo {
        FileName = Path.Combine(binDirectory, "obs64.exe"),
        WorkingDirectory = binDirectory,
        Arguments = "--portable --multi --minimize-to-tray --disable-updater"
          + " --disable-shutdown-check --disable-missing-files-check",
        UseShellExecute = false,
      };
      if (pluginRoot != null) {
        startInfo.Environment["OBS_PLUGINS_PATH"] = Path.Combine(pluginRoot, "obs-plugins", "64bit");
        startInfo.Environment["OBS_PLUGINS_DATA_PATH"] = Path.Combine(pluginRoot, "data", "obs-plugins");
      }
      _process = Process.Start(startInfo)!;

      var deadline = DateTime.UtcNow + _bootTimeout;
      await WaitUntilListeningAsync(port, deadline).ConfigureAwait(false);
      await WaitUntilReadyAsync(port, deadline).ConfigureAwait(false);
      Uri = new Uri($"ws://127.0.0.1:{port}");
    }

    public async Task ObserveWindowAsync(string requestName, Func<Task> request) {
      var existing = GetWindows(visibleOnly: false);
      await request().ConfigureAwait(false);
      await WaitForNewWindowsAsync(requestName, existing).ConfigureAwait(false);
    }

    public async Task RestartAsync() {
      await StopAsync(force: true).ConfigureAwait(false);
      await InitializeAsync().ConfigureAwait(false);
    }

    public void RecordOperation(string message) {
      string line = $"{DateTimeOffset.UtcNow:O} {message}";
      TestContext.Current.TestOutputHelper?.WriteLine(line);
      if (_root != null) {
        File.AppendAllText(Path.Combine(_root, "test-operations.log"), line + Environment.NewLine);
      }
    }

    public Exception AddProcessDiagnostics(Exception exception) {
      if (_process is not { HasExited: true } process) {
        return exception;
      }
      return new InvalidOperationException(
        $"OBS exited with code {FormatExitCode(process.ExitCode)}.{GetLogTail()}",
        exception
      );
    }

    public async ValueTask DisposeAsync() {
      await StopAsync(force: false).ConfigureAwait(false);
    }

    private async Task StopAsync(bool force) {
      string processState = "not started";
      if (_process is { } process) {
        if (!process.HasExited) {
          if (force) {
            process.Kill(entireProcessTree: true);
            process.WaitForExit(10_000);
          }
          else {
            process.CloseMainWindow();
            if (!process.WaitForExit(10_000)) {
              process.Kill(entireProcessTree: true);
              process.WaitForExit(10_000);
            }
          }
        }
        processState = $"exited with code {FormatExitCode(process.ExitCode)}";
        process.Dispose();
        _process = null;
      }
      if (_root != null) {
        try {
          ArchiveDiagnostics(_root, processState);
        }
        catch (IOException ex) {
          TestContext.Current.TestOutputHelper?.WriteLine($"Failed to archive OBS diagnostics: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex) {
          TestContext.Current.TestOutputHelper?.WriteLine($"Failed to archive OBS diagnostics: {ex.Message}");
        }
        await DeletePortableRootAsync(_root).ConfigureAwait(false);
        _root = null;
      }
      Uri = null;
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

    private static string? GetVendorPluginRoot() {
      string? root = Environment.GetEnvironmentVariable("OBSSTRAWKET_TEST_PLUGIN_ROOT");
      if (root == null) {
        return null;
      }
      string plugin = Path.Combine(root, "obs-plugins", "64bit", "advanced-scene-switcher.dll");
      return File.Exists(plugin) ? root : null;
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
        [Output]
        Mode=Advanced
        FilenameFormatting=%CCYY-%MM-%DD %hh-%mm-%ss

        [AdvOut]
        RecType=Standard
        RecEncoder=obs_x264
        RecAudioEncoder=ffmpeg_aac
        RecTracks=1
        RecFilePath={recordings.Replace('\\', '/')}
        RecFormat2=hybrid_mp4
        RecSplitFile=true
        RecSplitFileType=Manual

        [SimpleOutput]
        FilePath={recordings.Replace('\\', '/')}
        RecFormat2=hybrid_mp4

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

    private async Task WaitUntilListeningAsync(int port, DateTime deadline) {
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

    // obs-websocket starts listening before the OBS frontend finishes loading, so
    // frontend requests (CreateScene, etc.) return NotReady during that gap. Connect
    // once and poll a read-only frontend request until it stops reporting NotReady.
    private async Task WaitUntilReadyAsync(int port, DateTime deadline) {
      var uri = new Uri($"ws://127.0.0.1:{port}");
      var remaining = deadline - DateTime.UtcNow;
      using var timeout = new CancellationTokenSource(remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero);
      var token = timeout.Token;
      using var client = new ObsClientSocket();
      try {
        await client.ConnectAsync(uri, Password, cancellation: token).ConfigureAwait(false);
        while (true) {
          if (_process!.HasExited) {
            throw new InvalidOperationException($"OBS exited with code {_process.ExitCode}.{GetLogTail()}");
          }
          try {
            await client.GetSceneListAsync(cancellation: token).ConfigureAwait(false);
            return;
          }
          catch (FailureResponseException ex)
              when (ex.Response.RequestStatus.Code == RequestStatus.NotReady) {
            await Task.Delay(500, token).ConfigureAwait(false);
          }
        }
      }
      catch (OperationCanceledException) {
        throw new TimeoutException($"OBS frontend was not ready within {_bootTimeout}.{GetLogTail()}");
      }
      finally {
        await client.CloseAsync().ConfigureAwait(false);
      }
    }

    private async Task<HashSet<nint>> WaitForNewWindowsAsync(string requestName, HashSet<nint> existing) {
      var deadline = DateTime.UtcNow + _windowTimeout;
      while (DateTime.UtcNow < deadline) {
        var opened = GetWindows(visibleOnly: true);
        opened.ExceptWith(existing);
        if (opened.Count > 0) {
          return opened;
        }
        await Task.Delay(100).ConfigureAwait(false);
      }
      throw new TimeoutException($"{requestName} did not open an OBS window within {_windowTimeout}.");
    }

    private HashSet<nint> GetWindows(bool visibleOnly) {
      var windows = new HashSet<nint>();
      uint processId = checked((uint)_process!.Id);
      EnumWindows((window, _) => {
        GetWindowThreadProcessId(window, out uint windowProcessId);
        if (windowProcessId == processId && (!visibleOnly || IsWindowVisible(window))) {
          windows.Add(window);
        }
        return true;
      }, 0);
      return windows;
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

    private static void ArchiveDiagnostics(string root, string processState) {
      string? artifacts = Environment.GetEnvironmentVariable("OBSSTRAWKET_TEST_ARTIFACTS");
      if (artifacts == null) {
        return;
      }

      string destination = Path.Combine(artifacts, Path.GetFileName(root));
      Directory.CreateDirectory(destination);
      File.WriteAllText(
        Path.Combine(destination, "process.txt"),
        $"{DateTimeOffset.UtcNow:O} {processState}{Environment.NewLine}"
      );
      CopyFileIfExists(
        Path.Combine(root, "test-operations.log"),
        Path.Combine(destination, "test-operations.log")
      );
      CopyDirectoryIfExists(
        Path.Combine(root, "config", "obs-studio", "logs"),
        Path.Combine(destination, "logs")
      );
      CopyDirectoryIfExists(
        Path.Combine(root, "config", "obs-studio", "crashes"),
        Path.Combine(destination, "crashes")
      );
    }

    private static void CopyFileIfExists(string source, string destination) {
      if (File.Exists(source)) {
        File.Copy(source, destination, overwrite: true);
      }
    }

    private static void CopyDirectoryIfExists(string source, string destination) {
      if (!Directory.Exists(source)) {
        return;
      }
      Directory.CreateDirectory(destination);
      foreach (string file in Directory.EnumerateFiles(source)) {
        File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
      }
      foreach (string directory in Directory.EnumerateDirectories(source)) {
        CopyDirectoryIfExists(directory, Path.Combine(destination, Path.GetFileName(directory)));
      }
    }

    private static string FormatExitCode(int exitCode) {
      return $"{exitCode} (0x{unchecked((uint)exitCode):X8})";
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

    private delegate bool EnumWindowsCallback(nint window, nint parameter);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumWindows(EnumWindowsCallback callback, nint parameter);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint window, out uint processId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(nint window);
  }

  [CollectionDefinition("IsolatedObs")]
  public class IsolatedObsCollection : ICollectionFixture<IsolatedObsFixture> { }
}
