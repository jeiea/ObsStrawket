using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal class EventInterfaceGenerator {
    private readonly SourceFetcher _fetcher = new();

    public async Task GenerateAsync() {
      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);

      using var file = File.CreateText("EventInterface.cs");
      file.WriteLine(@"    #region Events");

      file.WriteLine();
      file.WriteLine("    /// <summary>");
      file.WriteLine("    /// All events.");
      file.WriteLine("    /// </summary>");
      file.WriteLine("    public event Action<IEvent> Event = delegate {{ }};");

      string previousCategory = "";
      foreach (var ev in json.Events) {
        if (previousCategory != ev.Category) {
          previousCategory = ev.Category;
          file.WriteLine();
          file.WriteLine("    /// <summary>");
          file.WriteLine("    /// {0}{1} event group.", char.ToUpper(ev.Category[0]), ev.Category[1..]);
          file.WriteLine("    /// </summary>");
          file.WriteLine("    public event Action<{0}Event> {0}Event = delegate {{ }};", TransformHelper.ToPascalCase(ev.Category));
        }
        file.WriteLine("    /// <summary>");
        file.WriteLine("    /// {0}<br />", TransformHelper.EscapeForXml(ev.Description!).Replace(Environment.NewLine, $"<br />{Environment.NewLine}    /// "));
        file.WriteLine("    /// Latest supported RPC version: {0}<br />", ev.RpcVersion);
        file.WriteLine("    /// Added in: {0}", ev.InitialVersion);
        file.WriteLine("    /// </summary>");
        file.WriteLine("    public event Action<{0}> {0} = delegate {{ }};", ev.EventType);
      }

      file.WriteLine();
      file.WriteLine(@"    #endregion");
    }
  }
}
