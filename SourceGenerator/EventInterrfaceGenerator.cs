using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal partial class EventInterfaceGenerator {
    private readonly IDirectoryHelper _directoryHelper;
    private readonly ISourceFetcher _fetcher;

    public EventInterfaceGenerator(IDirectoryHelper directoryHelper, ISourceFetcher fetcher) {
      _directoryHelper = directoryHelper;
      _fetcher = fetcher;
    }

    public async Task GenerateAsync() {
      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);

      using var part = new StringWriter();
      part.WriteLine(@"    #region Events");

      part.WriteLine();
      part.WriteLine("    /// <summary>");
      part.WriteLine("    /// All events.");
      part.WriteLine("    /// </summary>");
      part.WriteLine("    public event Action<IObsEvent> Event = static delegate { };");

      string previousCategory = "";
      foreach (var ev in json.Events) {
        if (previousCategory != ev.Category) {
          previousCategory = ev.Category;
          part.WriteLine();
          part.WriteLine("    /// <summary>");
          part.WriteLine("    /// {0}{1} event group.", char.ToUpper(ev.Category[0], CultureInfo.InvariantCulture), ev.Category[1..]);
          part.WriteLine("    /// </summary>");
          part.WriteLine("    public event Action<{0}Event> {0}Event = static delegate {{ }};", TransformHelper.ToPascalCase(ev.Category));
        }
        part.WriteLine("    /// <summary>");
        part.WriteLine("    /// {0}<br />", TransformHelper.EscapeForXml(ev.Description).Replace(Environment.NewLine, $"<br />{Environment.NewLine}    /// "));
        part.WriteLine("    /// Latest supported RPC version: {0}<br />", ev.RpcVersion);
        part.WriteLine("    /// Added in: {0}", ev.InitialVersion);
        part.WriteLine("    /// </summary>");
        part.WriteLine("    public event Action<{0}> {0} = static delegate {{ }};", ev.EventType);
      }

      part.WriteLine();
      part.Write(@"    #endregion");

      string previous = await File.ReadAllTextAsync(_directoryHelper.ObsClientPath).ConfigureAwait(false);

      bool isReplaced = false;
      string result = EventsRegionPattern().Replace(previous, (match) => {
        isReplaced = true;
        return part.ToString();
      });

      if (!isReplaced) {
        throw new InvalidOperationException("Unexpected file");
      }

      await File.WriteAllTextAsync(_directoryHelper.ObsClientPath, result).ConfigureAwait(false);
    }

    [GeneratedRegex(@"    #region Events\r\n.*?#endregion", RegexOptions.Singleline)]
    private static partial Regex EventsRegionPattern();
  }
}
