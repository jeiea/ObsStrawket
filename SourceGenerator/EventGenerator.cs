using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal class EventGenerator {
    private readonly IDirectoryHelper _directoryHelper;
    private readonly ISourceFetcher _fetcher;

    public EventGenerator(IDirectoryHelper directoryHelper, ISourceFetcher fetcher) {
      _directoryHelper = directoryHelper;
      _fetcher = fetcher;
    }

    public async Task GenerateAsync() {
      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);

      using var file = File.CreateText($"{_directoryHelper.MainProjectDirectory}/DataTypes/Predefineds/Events.cs");
      file.Write(@"using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ObsStrawket.DataTypes.Predefineds {");

      foreach (string category in json.Events.Select(x => x.Category).Distinct()) {
        file.WriteLine();
        file.WriteLine("  /// <summary>");
        file.WriteLine("  /// {0}{1} event.", char.ToUpper(category[0]), category[1..]);
        file.WriteLine("  /// </summary>");
        file.WriteLine("  [MessagePackObject]");
        string pascalCategory = TransformHelper.ToPascalCase(category);
        file.WriteLine("  public class {0}Event : ObsEvent {{ }}", pascalCategory);
      }

      foreach (var ev in json.Events) {
        file.WriteLine();
        file.WriteLine("  /// <summary>");
        file.WriteLine("  /// {0}<br />", TransformHelper.EscapeForXml(ev.Description!).Replace(Environment.NewLine, $"<br />{Environment.NewLine}  /// "));
        file.WriteLine("  /// Latest supported RPC version: {0}<br />", ev.RpcVersion);
        file.WriteLine("  /// Added in: {0}", ev.InitialVersion);
        file.WriteLine("  /// </summary>");
        if (ev.Deprecated) {
          file.WriteLine("  [Obsolete]");
        }
        file.WriteLine("  [MessagePackObject]");
        string pascalCategory = TransformHelper.ToPascalCase(ev.Category);
        file.Write("  public class {0} : {1}Event {{", ev.EventType, pascalCategory);
        if (ev.DataFields!.Count == 0) {
          file.WriteLine(" }");
        }
        else {
          foreach (var field in ev.DataFields!) {
            file.WriteLine();
            file.WriteLine("    /// <summary>");
            file.WriteLine("    /// {0}", TransformHelper.EscapeForXml(field.ValueDescription!));
            file.WriteLine("    /// </summary>");
            file.WriteLine("    [JsonPropertyName(\"{0}\")]", field.ValueName);
            file.WriteLine("    {0}", MakeFieldDeclaration(
              field.ValueName!, field.ValueType!, field.ValueDescription!
            ));
          }
          file.WriteLine("  }");
        }
      }

      file.WriteLine("}");
    }

    private static string MakeFieldDeclaration(string name, string type, string description) {
      var builder = new StringBuilder();
      builder.Append("public ");

      string valueType = TransformHelper.ToCSharpType(type, description);
      builder.Append(valueType);
      builder.Append(' ');

      if (name == "eventType") {
        builder.Append("VendorEventType");
      }
      else {
        builder.Append(char.ToUpper(name[0]));
        builder.Append(name[1..]);
      }
      builder.Append(" { get; set; }");
      if (valueType == "string") {
        builder.Append(" = \"\";");
      }
      else if (char.IsUpper(valueType[0])) {
        builder.Append(" = new();");
      }

      return builder.ToString();
    }
  }
}
