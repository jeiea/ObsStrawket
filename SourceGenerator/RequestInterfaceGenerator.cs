using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SourceGenerator {
  internal class RequestInterfaceGenerator {
    private readonly SourceFetcher _fetcher = new();

    public async Task GenerateAsync() {
      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);

      using var file = File.CreateText("RequestApi.cs");
      foreach (var request in json.Requests) {
        file.WriteLine("/// <summary>");
        file.WriteLine("/// {0}<br />", EscapeForXml(request.Description!).Replace(Environment.NewLine, $"<br />{Environment.NewLine}/// "));
        file.WriteLine("/// Latest supported RPC version: {0}<br />", request.RpcVersion);
        file.WriteLine("/// Added in: {0}", request.InitialVersion);
        file.WriteLine("/// </summary>");

        foreach (var parameter in request.RequestFields!) {
          file.Write("/// <param name=\"{0}\">{1}", parameter.ValueName, EscapeForXml(parameter.ValueDescription!));
          if (parameter.ValueOptionalBehavior != null) {
            file.Write("<br />If null, {0}", EscapeForXml(parameter.ValueOptionalBehavior));
          }
          file.WriteLine("</param>");
        }
        file.WriteLine("/// <param name=\"cancellation\">Token for cancellation</param>");

        string returnType = request.ResponseFields!.Count > 0 ? $"{request.RequestType}Response" : "RequestResponse";
        string parameters = GetParameters(request.RequestFields);
        file.WriteLine("public async Task<{0}> {1}Async({2}) {{", returnType, request.RequestType, parameters);
        string assignments = GetAssignments(request.RequestFields);
        file.WriteLine("  return (await _clientSocket.RequestAsync(new {0}() {{ {1}}}, cancellation).ConfigureAwait(false) as {2})!;", request.RequestType, assignments, returnType);
        file.WriteLine("}");
        file.WriteLine();
      }
    }

    private static string GetAssignments(IEnumerable<ObsRequestField> parameters) {
      if (parameters.Any(x => x.ValueName == "shift")) {
        return "KeyId = keyId, KeyModifiers = new KeyModifiers() { Shift = shift, Control = control, Alt = alt, Command = command }";
      }

      var stringifieds = new List<string>();
      var builder = new StringBuilder();
      foreach (var parameter in parameters) {
        builder.Clear();
        if (parameter.ValueName == "requestType") {
          builder.Append("VendorRequestType");
        } else {
          builder.Append(char.ToUpper(parameter.ValueName![0]));
          builder.Append(parameter.ValueName[1..]);
        }
        builder.Append(" = ");

        builder.Append(parameter.ValueName);
        stringifieds.Add(builder.ToString());
      }

      return string.Join(", ", stringifieds);
    }

    private static string GetParameters(IEnumerable<ObsRequestField> parameters) {
      var stringifieds = new List<string>();
      var builder = new StringBuilder();

      foreach (var parameter in parameters) {
        builder.Clear();

        string type = ToCSharpType(parameter.ValueType!, parameter.ValueDescription!);
        builder.Append(type);
        if (parameter.ValueOptional) {
          builder.Append('?');
        }
        builder.Append(' ');
        builder.Append(parameter.ValueName);
        if (parameter.ValueOptional) {
          builder.Append(" = default");
        }
        stringifieds.Add(builder.ToString());
      }
      stringifieds.Add("CancellationToken cancellation = default");

      return string.Join(", ", stringifieds);
    }

    private static string ToCSharpType(string type, string description) {
      if (description == "Audio monitor type") {
        return "MonitoringType";
      }

      string s1 = type
        .Replace("Array", "List")
        .Replace("String", "string")
        .Replace("Any", "object?")
        .Replace("Boolean", "bool")
        .Replace("Object", "Dictionary<string, object?>");
      bool seemsInteger = Regex.IsMatch(description, @"index|offset|milli|frame|numerator|denominator|pixel|width|height|quality|\bid\b|number of|version|duration", RegexOptions.IgnoreCase);
      string s2 = s1.Replace("Number", seemsInteger ? "int" : "double");
      return s2;
    }

    private static string EscapeForXml(string unescaped) {
      var doc = new XmlDocument();
      var node = doc.CreateElement("root");
      node.InnerText = Regex.Replace(unescaped, @"\r\n?|\n", Environment.NewLine);
      string xml = node.InnerXml;
      xml = Regex.Replace(xml, "`(.*?)`", "<c>$1</c>");
      return xml;
    }
  }
}
