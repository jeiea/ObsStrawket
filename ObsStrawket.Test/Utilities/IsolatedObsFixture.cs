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
  public sealed partial class IsolatedObsFixture : IAsyncLifetime {
    private static readonly TimeSpan _bootTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan _windowTimeout = TimeSpan.FromSeconds(5);
    private static readonly string[] _junctionNames = ["bin", "data", "obs-plugins"];

    public bool IsAvailable => Uri != null;
    public bool HasExited => _process?.HasExited == true;
    public int? ExitCode => _process is { HasExited: true } process
      ? process.ExitCode
      : null;
    public static bool HasVendorPlugin => GetVendorPluginRoot() != null;
    public Uri? Uri { get; private set; }
    public string Password { get; } = MockServer.Password;

    private string? _root;
    private Process? _process;
    private string? _termination;
    private readonly Lock _operationLogLock = new();

    public async ValueTask InitializeAsync() {
      if (FindInstalledObs() is not string installation) {
        return;
      }

      int port = GetFreeTcpPort();
      _root = Path.Combine(Path.GetTempPath(), "obsstrawket-test", $"{Guid.NewGuid():N}");
      _termination = null;
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
      _process = Process.Start(startInfo);

      var deadline = DateTime.UtcNow + _bootTimeout;
      await WaitUntilListeningAsync(port, deadline).ConfigureAwait(false);
      await WaitUntilReadyAsync(port, deadline).ConfigureAwait(false);
      Uri = new Uri($"ws://127.0.0.1:{port}");
    }

    public async Task ObserveWindowAsync(string requestName, Func<Task> request) {
      RecordOperation($"Starting {requestName}");
      var existing = GetWindows(visibleOnly: false)
        .Select(static window => window.Handle)
        .ToHashSet();
      await request().ConfigureAwait(false);
      var opened = await WaitForNewWindowsAsync(requestName, existing).ConfigureAwait(false);
      RecordOperation($"Observed {requestName}: {FormatWindows(opened)}");
      await CloseWindowsAsync(requestName, opened).ConfigureAwait(false);
      RecordOperation($"Completed {requestName}");
    }

    public async Task RestartAsync() {
      await StopAsync(force: true).ConfigureAwait(false);
      await InitializeAsync().ConfigureAwait(false);
    }

    public void RecordOperation(string message) {
      string line = $"{DateTimeOffset.UtcNow:O} {message}";
      lock (_operationLogLock) {
        if (_root != null) {
          File.AppendAllText(
            Path.Combine(_root, "test-operations.log"),
            line + Environment.NewLine
          );
        }
        TestContext.Current.TestOutputHelper?.WriteLine(line);
      }
    }

    public Exception AddProcessDiagnostics(Exception exception) {
      return _process is not { HasExited: true } process
        ? exception
        : new InvalidOperationException(
        $"OBS exited with code {FormatExitCode(process.ExitCode)}.{GetLogTail()}",
        exception
      );
    }

    public async ValueTask DisposeAsync() {
      await StopAsync(force: false).ConfigureAwait(false);
    }

    private async Task StopAsync(bool force) {
      int? exitCode = null;
      bool preserveLocal = false;
      if (_process is { } process) {
        preserveLocal = process.HasExited;
        if (!process.HasExited) {
          if (force) {
            if (ReadLatestCrashReport() != null) {
              if (await DismissCrashDialogAndWaitForExitAsync(
                TimeSpan.FromSeconds(2)
              ).ConfigureAwait(false)) {
                _termination = "crash-dialog-dismissed";
              }
            }
            if (!process.HasExited) {
              process.Kill(entireProcessTree: true);
              _ = process.WaitForExit(10_000);
              _termination = "forced-kill";
            }
          }
          else {
            _ = process.CloseMainWindow();
            if (!process.WaitForExit(10_000)) {
              process.Kill(entireProcessTree: true);
              _ = process.WaitForExit(10_000);
              _termination = "forced-kill";
            }
            else {
              _termination = "normal-exit";
            }
          }
        }
        _termination ??= ReadLatestCrashReport() == null
          ? "normal-exit"
          : "crash-exit";
        exitCode = process.ExitCode;
        preserveLocal |= ReadLatestCrashReport() != null;
        preserveLocal |= _termination == "normal-exit" && exitCode != 0;
        process.Dispose();
        _process = null;
      }
      if (_root != null) {
        try {
          string? artifacts = ResolveArtifactsDirectory(
            Environment.GetEnvironmentVariable("OBSSTRAWKET_TEST_ARTIFACTS"),
            AppContext.BaseDirectory,
            preserveLocal
          );
          if (artifacts != null) {
            ArchiveDiagnostics(
              _root,
              artifacts,
              _termination ?? "not-started",
              exitCode
            );
          }
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
      _termination = null;
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
      _ = Directory.CreateDirectory(root);
      foreach (string name in _junctionNames) {
        CreateJunction(Path.Combine(root, name), Path.Combine(installation, name));
      }
      SeedPortableConfig(root, port);
    }

    private static void SeedPortableConfig(string root, int port) {
      _ = Directory.CreateDirectory(root);
      File.WriteAllText(Path.Combine(root, "portable_mode.txt"), "");

      string config = Path.Combine(root, "config", "obs-studio");
      // FirstRun suppresses the auto-configuration wizard on a fresh config.
      string generalIni = """
        [General]
        FirstRun=true
        Pre31Migrated=true

        """;
      _ = Directory.CreateDirectory(config);
      File.WriteAllText(Path.Combine(config, "global.ini"), generalIni);
      File.WriteAllText(Path.Combine(config, "user.ini"), generalIni);

      // Keep recordings from tests inside the disposable root.
      string recordings = Path.Combine(root, "recordings");
      _ = Directory.CreateDirectory(recordings);
      string profile = Path.Combine(config, "basic", "profiles", "Untitled");
      _ = Directory.CreateDirectory(profile);
      _ = Directory.CreateDirectory(Path.Combine(config, "basic", "scenes"));
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
      _ = Directory.CreateDirectory(websocket);
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
        _ = await client.ConnectAsync(uri, Password, cancellation: token).ConfigureAwait(false);
        while (true) {
          if (_process!.HasExited) {
            throw new InvalidOperationException($"OBS exited with code {_process.ExitCode}.{GetLogTail()}");
          }
          try {
            _ = await client.GetSceneListAsync(cancellation: token).ConfigureAwait(false);
            return;
          }
          catch (ObsRequestException ex)
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

    public async Task<string?> WaitForCrashAsync(
      TimeSpan timeout,
      CancellationToken cancellation = default
    ) {
      var deadline = DateTime.UtcNow + timeout;
      while (DateTime.UtcNow < deadline) {
        if (ReadLatestCrashReport() is string report) {
          return !await DismissCrashDialogAndWaitForExitAsync(
            deadline - DateTime.UtcNow,
            cancellation
          ).ConfigureAwait(false)
            ? throw new TimeoutException(
              "OBS wrote a crash report but did not exit after dismissing its crash dialog."
            )
            : SetCrashTermination(report);
        }
        await Task.Delay(100, cancellation).ConfigureAwait(false);
      }
      return null;
    }

    private string SetCrashTermination(string report) {
      _termination = "crash-dialog-dismissed";
      return report;
    }

    private async Task<IReadOnlyList<ObsWindow>> WaitForNewWindowsAsync(
      string requestName,
      HashSet<nint> existing
    ) {
      var deadline = DateTime.UtcNow + _windowTimeout;
      while (DateTime.UtcNow < deadline) {
        var opened = GetWindows(visibleOnly: true);
        _ = opened.RemoveAll(window => existing.Contains(window.Handle));
        if (opened.Count > 0) {
          return opened;
        }
        await Task.Delay(100).ConfigureAwait(false);
      }
      throw new TimeoutException(
        $"{requestName} did not open a new OBS window within {_windowTimeout}."
          + $"{Environment.NewLine}Current OBS windows: {FormatWindows(GetWindows(visibleOnly: false))}");
    }

    private async Task CloseWindowsAsync(string requestName, IReadOnlyList<ObsWindow> windows) {
      foreach (var window in windows) {
        _ = PostMessage(window.Handle, WmClose, 0, 0);
      }

      var handles = windows.Select(static window => window.Handle).ToHashSet();
      var deadline = DateTime.UtcNow + _windowTimeout;
      List<ObsWindow> remaining = [];
      while (DateTime.UtcNow < deadline) {
        remaining = [
          .. GetWindows(visibleOnly: false)
            .Where(window => handles.Contains(window.Handle)),
        ];
        if (remaining.Count == 0) {
          return;
        }
        await Task.Delay(100).ConfigureAwait(false);
      }

      throw new TimeoutException(
        $"{requestName} did not close observed OBS windows within {_windowTimeout}."
          + $"{Environment.NewLine}Remaining OBS windows: {FormatWindows(remaining)}");
    }

    private List<ObsWindow> GetWindows(bool visibleOnly) {
      var windows = new List<ObsWindow>();
      uint processId = checked((uint)_process!.Id);
      _ = EnumWindows((window, lParam) => {
        _ = GetWindowThreadProcessId(window, out uint windowProcessId);
        if (windowProcessId == processId && (!visibleOnly || IsWindowVisible(window))) {
          windows.Add(new ObsWindow(
            window,
            GetWindowTitle(window),
            IsWindowVisible(window),
            windowProcessId
          ));
        }
        return true;
      }, 0);
      return windows;
    }

    private static string FormatWindows(IReadOnlyList<ObsWindow> windows) {
      return windows.Count == 0
        ? "<none>"
        : string.Join("; ", windows.Select(static window =>
        $"hwnd=0x{window.Handle.ToInt64():X}, pid={window.ProcessId}, visible={window.Visible}, title=\"{NormalizeWindowTitle(window.Title)}\""
      ));
    }

    private static string NormalizeWindowTitle(string title) {
      return title.Length == 0
        ? "<empty>"
        : title.Replace('\r', ' ').Replace('\n', ' ');
    }

    private string GetLogTail() {
      if (_root == null) {
        return "";
      }
      try {
        string logs = Path.Combine(_root, "config", "obs-studio", "logs");
        var newest = new DirectoryInfo(logs).GetFiles().MaxBy(static file => file.LastWriteTimeUtc);
        if (newest == null) {
          return "";
        }
        using var stream = new FileStream(
          newest.FullName,
          FileMode.Open,
          FileAccess.Read,
          FileShare.ReadWrite | FileShare.Delete
        );
        using var reader = new StreamReader(stream);
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

    private string? ReadLatestCrashReport() {
      if (_root == null) {
        return null;
      }
      try {
        string crashes = Path.Combine(_root, "config", "obs-studio", "crashes");
        var newest = new DirectoryInfo(crashes)
          .GetFiles("Crash *.txt")
          .MaxBy(static file => file.LastWriteTimeUtc);
        if (newest == null) {
          return null;
        }
        using var stream = new FileStream(
          newest.FullName,
          FileMode.Open,
          FileAccess.Read,
          FileShare.ReadWrite | FileShare.Delete
        );
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
      }
      catch (DirectoryNotFoundException) {
        return null;
      }
      catch (IOException) {
        return null;
      }
      catch (UnauthorizedAccessException) {
        return null;
      }
    }

    private async Task<bool> DismissCrashDialogAndWaitForExitAsync(
      TimeSpan timeout,
      CancellationToken cancellation = default
    ) {
      if (_process is not { } process) {
        return false;
      }
      if (process.HasExited) {
        return true;
      }
      var deadline = DateTime.UtcNow + timeout;
      while (DateTime.UtcNow < deadline) {
        _ = DismissCrashDialog();
        if (process.HasExited) {
          return true;
        }
        await Task.Delay(100, cancellation).ConfigureAwait(false);
      }
      return process.HasExited;
    }

    private bool DismissCrashDialog() {
      if (_process is not { HasExited: false } process) {
        return false;
      }
      bool dismissed = false;
      uint processId = checked((uint)process.Id);
      _ = EnumWindows((window, lParam) => {
        _ = GetWindowThreadProcessId(window, out uint windowProcessId);
        if (windowProcessId != processId || GetWindowTitle(window) != "OBS has crashed!") {
          return true;
        }
        nint noButton = GetDlgItem(window, IdNo);
        dismissed |= noButton != 0 && PostMessage(noButton, BmClick, 0, 0);
        return true;
      }, 0);
      return dismissed;
    }

    private static string GetWindowTitle(nint window) {
      int length = GetWindowTextLength(window);
      if (length == 0) {
        return "";
      }
      Span<char> title = stackalloc char[length + 1];
      int copied = GetWindowText(window, title, title.Length);
      return new string(title[..copied]);
    }

    internal static void ArchiveDiagnostics(
      string root,
      string artifacts,
      string termination,
      int? exitCode
    ) {
      string destination = Path.Combine(artifacts, Path.GetFileName(root));
      _ = Directory.CreateDirectory(destination);
      File.WriteAllText(
        Path.Combine(destination, "process.txt"),
        $"timestamp={DateTimeOffset.UtcNow:O}{Environment.NewLine}"
          + $"termination={termination}{Environment.NewLine}"
          + $"exitCode={FormatExitCode(exitCode)}{Environment.NewLine}"
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

    private static string? ResolveArtifactsDirectory(
      string? configured,
      string baseDirectory,
      bool preserveLocal
    ) {
      return !string.IsNullOrWhiteSpace(configured)
        ? configured
        : preserveLocal
          ? Path.Combine(baseDirectory, "obsstrawket-diagnostics")
          : null;
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
      _ = Directory.CreateDirectory(destination);
      foreach (string file in Directory.EnumerateFiles(source)) {
        File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
      }
      foreach (string directory in Directory.EnumerateDirectories(source)) {
        CopyDirectoryIfExists(directory, Path.Combine(destination, Path.GetFileName(directory)));
      }
    }

    private static string FormatExitCode(int? exitCode) {
      return exitCode is int value
        ? $"{value} (0x{unchecked((uint)value):X8})"
        : "n/a";
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
    private const uint BmClick = 0x00F5;
    private const uint WmClose = 0x0010;
    private const int IdNo = 7;

    private readonly record struct ObsWindow(
      nint Handle,
      string Title,
      bool Visible,
      uint ProcessId
    );

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EnumWindows(EnumWindowsCallback callback, nint parameter);

    [LibraryImport("user32.dll")]
    private static partial uint GetWindowThreadProcessId(nint window, out uint processId);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsWindowVisible(nint window);

    [LibraryImport("user32.dll")]
    private static partial nint GetDlgItem(nint dialog, int itemId);

    [LibraryImport("user32.dll", EntryPoint = "GetWindowTextLengthW")]
    private static partial int GetWindowTextLength(nint window);

    [LibraryImport(
      "user32.dll",
      EntryPoint = "GetWindowTextW",
      StringMarshalling = StringMarshalling.Utf16
    )]
    private static partial int GetWindowText(
      nint window,
      Span<char> text,
      int maxCount
    );

    [LibraryImport("user32.dll", EntryPoint = "PostMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PostMessage(
      nint window,
      uint message,
      nuint wParam,
      nint lParam
    );

  }

  [CollectionDefinition("IsolatedObs")]
  public class IsolatedObsCollectionDefinition : ICollectionFixture<IsolatedObsFixture> { }
}
