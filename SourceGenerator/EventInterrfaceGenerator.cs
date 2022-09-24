using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal class EventInterfaceGenerator {
    public static string ObsClientPath => "../../../../ObsStrawket/ObsClientSocket.cs";

    private readonly SourceFetcher _fetcher = new();

    public async Task GenerateAsync() {
      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);

      using var part = new StringWriter();
      part.WriteLine(@"    #region Events");

      part.WriteLine();
      part.WriteLine("    /// <summary>");
      part.WriteLine("    /// All events.");
      part.WriteLine("    /// </summary>");
      part.WriteLine("    public event Action<IObsEvent> Event = delegate { };");

      string previousCategory = "";
      foreach (var ev in json.Events) {
        if (previousCategory != ev.Category) {
          previousCategory = ev.Category;
          part.WriteLine();
          part.WriteLine("    /// <summary>");
          part.WriteLine("    /// {0}{1} event group.", char.ToUpper(ev.Category[0]), ev.Category[1..]);
          part.WriteLine("    /// </summary>");
          part.WriteLine("    public event Action<{0}Event> {0}Event = delegate {{ }};", TransformHelper.ToPascalCase(ev.Category));
        }
        part.WriteLine("    /// <summary>");
        part.WriteLine("    /// {0}<br />", TransformHelper.EscapeForXml(ev.Description!).Replace(Environment.NewLine, $"<br />{Environment.NewLine}    /// "));
        part.WriteLine("    /// Latest supported RPC version: {0}<br />", ev.RpcVersion);
        part.WriteLine("    /// Added in: {0}", ev.InitialVersion);
        part.WriteLine("    /// </summary>");
        part.WriteLine("    public event Action<{0}> {0} = delegate {{ }};", ev.EventType);
      }

      part.WriteLine();
      part.Write(@"    #endregion");

      string previous = await File.ReadAllTextAsync(ObsClientPath).ConfigureAwait(false);

      bool isReplaced = false;
      string result = Regex.Replace(previous, @"    #region Events\r\n.*?#endregion", (match) => {
        isReplaced = true;
        return part.ToString();
      }, RegexOptions.Singleline);

      if (!isReplaced) {
        throw new Exception("Unexpected file");
      }

      await File.WriteAllTextAsync(ObsClientPath, result).ConfigureAwait(false);
    }
  }
}
