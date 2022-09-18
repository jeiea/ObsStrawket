using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal class RequestResponseGenerator {
    private readonly SourceFetcher _fetcher = new();

    public async Task GenerateAsync() {
      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);

      using var file = File.CreateText("RequestsAndResponses.cs");
      file.WriteLine(@"using MessagePack;
using ObsStrawket.DataTypes;
using System.Collections.Generic;

namespace ObsStrawket.DataTypes.Predefineds {");
      foreach (var request in json.Requests) {
        file.WriteLine();
        file.WriteLine("  /// <summary>");
        file.WriteLine("  /// {0}<br />", TransformHelper.EscapeForXml(request.Description!).Replace(Environment.NewLine, $"<br />{Environment.NewLine}  /// "));
        file.WriteLine("  /// Latest supported RPC version: {0}<br />", request.RpcVersion);
        file.WriteLine("  /// Added in: {0}", request.InitialVersion);
        file.WriteLine("  /// </summary>");

        file.WriteLine("  [MessagePackObject]");
        file.Write("  public class {0} : Request {{", request.RequestType);
        if (request.RequestFields!.Count == 0) {
          file.WriteLine(" }");
        }
        else {
          foreach (var field in request.RequestFields!) {
            // keyModifiers.shift
            if (field.ValueName!.Contains(".")) {
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
            file.WriteLine("    [Key(\"{0}\")]", field.ValueName);
            file.WriteLine("    {0}", MakeFieldDeclaration(
              field.ValueName!, field.ValueType!, field.ValueDescription!, field.ValueOptional
            ));
          }
          file.WriteLine("  }");
        }

        if (request.ResponseFields!.Count > 0) {
          file.WriteLine();
          file.WriteLine("  /// <summary>");
          file.WriteLine("  /// Response of {0}", request.RequestType);
          file.WriteLine("  /// </summary>");
          file.Write("  public class {0}Response : RequestResponse {{", request.RequestType);
          foreach (var field in request.ResponseFields) {
            file.WriteLine();
            file.WriteLine("    /// <summary>");
            file.WriteLine("    /// {0}", TransformHelper.EscapeForXml(field.ValueDescription!));
            file.WriteLine("    /// </summary>");
            file.WriteLine("    [Key(\"{0}\")]", field.ValueName);
            file.WriteLine("    {0}", MakeFieldDeclaration(
              field.ValueName!, field.ValueType!, field.ValueDescription!, false
            ));
          }
          file.WriteLine("  }");
        }
      }

      file.WriteLine("}");
    }

    private static string MakeFieldDeclaration(string name, string type, string description, bool isOptional) {
      var builder = new StringBuilder();
      builder.Append("public ");

      string valueType = TransformHelper.ToCSharpType(type, description);
      builder.Append(valueType);
      if (isOptional && !valueType.EndsWith("?")) {
        builder.Append('?');
      }
      builder.Append(' ');

      if (name == "requestType") {
        builder.Append("VendorRequestType");
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
