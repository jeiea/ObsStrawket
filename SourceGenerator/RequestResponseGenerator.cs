using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceGenerator {

  internal partial class RequestResponseGenerator(IDirectoryHelper directoryHelper, ISourceFetcher fetcher) {
    private static readonly HashSet<string> _ca1711AllowedRequestTypes = [
      "CreateSceneCollection",
      "SetCurrentSceneCollection",
      "StartStream",
      "StopStream",
      "ToggleStream",
    ];

    public async Task GenerateAsync() {
      var json = await fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);
      using var file = File.CreateText($"{directoryHelper.MainProjectDirectory}/DataTypes/Predefineds/RequestsAndResponses.cs");

      WriteRequestsAndResponses(json, file);
    }

    private static void WriteRequestsAndResponses(ProtocolJson json, TextWriter file) {
      file.WriteLine(@"using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObsStrawket.DataTypes.Predefineds {");

      foreach (var request in json.Requests) {
        WriteRequestAndResponse(file, request);
      }

      file.WriteLine("}");
    }

    private static void WriteRequestAndResponse(TextWriter file, ObsRequest request) {
      file.WriteLine();
      file.WriteLine("  /// <summary>");
      file.WriteLine("  /// {0}<br />", TransformHelper.EscapeForXml(request.Description!).Replace(Environment.NewLine, $"<br />{Environment.NewLine}  /// "));
      file.WriteLine("  /// Latest supported RPC version: {0}<br />", request.RpcVersion);
      file.WriteLine("  /// Added in: {0}", request.InitialVersion);
      file.WriteLine("  /// </summary>");

      bool suppressCa1711 = _ca1711AllowedRequestTypes.Contains(request.RequestType!);
      if (suppressCa1711) {
        file.WriteLine("#pragma warning disable CA1711");
      }
      file.Write("  public class {0} : Request {{", request.RequestType);
      if (request.RequestFields.Count == 0) {
        file.WriteLine(" }");
      }
      else {
        file.WriteLine();

        foreach (var field in request.RequestFields) {
          // keyModifiers.shift
          if (field.ValueName!.Contains('.')) {
            continue;
          }

          file.WriteLine();
          file.WriteLine("    /// <summary>");
          file.Write("    /// {0}", TransformHelper.EscapeForXml(field.ValueDescription!));
          if (field.ValueRestrictions != null) {
            file.WriteLine("<br />");
            file.Write("    /// It should be {0}", TransformHelper.EscapeForXml(field.ValueRestrictions!));
          }
          if (field.ValueOptionalBehavior != null) {
            file.WriteLine("<br />");
            file.Write("    /// If null, {0}", TransformHelper.EscapeForXml(field.ValueOptionalBehavior));
          }
          file.WriteLine();
          file.WriteLine("    /// </summary>");
          file.WriteLine("    [JsonPropertyName(\"{0}\")]", field.ValueName);
          file.WriteLine("    {0}", MakeFieldDeclaration(
            field.ValueName!, field.ValueType!, field.ValueDescription!, field.ValueOptional
          ));
        }
        file.WriteLine("  }");
      }
      if (suppressCa1711) {
        file.WriteLine("#pragma warning restore CA1711");
      }

      if (request.ResponseFields!.Count > 0) {
        file.WriteLine();
        file.WriteLine("  /// <summary>");
        file.WriteLine("  /// Response of {0}", request.RequestType);
        file.WriteLine("  /// </summary>");
        file.WriteLine("  public class {0}Response : RequestResponse {{", request.RequestType);
        foreach (var field in request.ResponseFields) {
          file.WriteLine();
          file.WriteLine("    /// <summary>");
          file.WriteLine("    /// {0}", TransformHelper.EscapeForXml(field.ValueDescription!));
          file.WriteLine("    /// </summary>");
          file.WriteLine("    [JsonPropertyName(\"{0}\")]", field.ValueName);
          file.WriteLine("    {0}", MakeFieldDeclaration(
            field.ValueName!, field.ValueType!, field.ValueDescription!, false
          ));
        }
        file.WriteLine("  }");
      }
    }

    private static string MakeFieldDeclaration(string name, string type, string description, bool isOptional) {
      var builder = new StringBuilder();
      _ = builder.Append("public ");

      string valueType = TransformHelper.ToCSharpType(type, description);
      _ = builder.Append(valueType);
      if (isOptional && !valueType.EndsWith('?')) {
        _ = builder.Append('?');
      }
      _ = builder.Append(' ');

      if (name == "requestType") {
        _ = builder.Append("VendorRequestType");
      }
      else {
        _ = builder.Append(char.ToUpper(name[0], CultureInfo.InvariantCulture));
        _ = builder.Append(name[1..]);
      }
      _ = builder.Append(" { get; set; }");
      if (!isOptional) {
        if (valueType == "string") {
          _ = builder.Append(" = \"\";");
        }
        else if (IsListOrDictionary().IsMatch(valueType)) {
          _ = builder.Append(" = [];");
        }
        else if (char.IsUpper(valueType[0])) {
          _ = builder.Append(" = new();");
        }
      }

      return builder.ToString();
    }

    [GeneratedRegex(@"^(List|Dictionary)<")]
    private static partial Regex IsListOrDictionary();
  }
}
