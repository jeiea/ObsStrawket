using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal class RequestInterfaceGenerator {
    private readonly SourceFetcher _fetcher = new();

    public async Task GenerateAsync() {
      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);
      PatchTriggerHotkeyByKeySequence(json.Requests.Find(x => x.RequestType == "TriggerHotkeyByKeySequence")!.RequestFields!);
      json.Requests.RemoveAt(json.Requests.FindIndex(x => x.RequestType == "Sleep"));

      using var part = new StringWriter();
      part.WriteLine("    #region Requests");
      part.WriteLine();

      foreach (var request in json.Requests) {
        part.WriteLine("    /// <summary>");
        part.WriteLine("    /// {0}<br />", TransformHelper.EscapeForXml(request.Description!).Replace(Environment.NewLine, $"<br />{Environment.NewLine}    /// "));
        part.WriteLine("    /// Latest supported RPC version: {0}<br />", request.RpcVersion);
        part.WriteLine("    /// Added in: {0}", request.InitialVersion);
        part.WriteLine("    /// </summary>");

        foreach (var parameter in request.RequestFields!) {
          part.Write("    /// <param name=\"{0}\">{1}", parameter.ValueName, TransformHelper.EscapeForXml(parameter.ValueDescription!));
          if (parameter.ValueOptionalBehavior != null) {
            part.Write("<br />If null, {0}", TransformHelper.EscapeForXml(parameter.ValueOptionalBehavior));
          }
          part.WriteLine("</param>");
        }
        part.WriteLine("    /// <param name=\"cancellation\">Token for cancellation</param>");

        string returnType = request.ResponseFields!.Count > 0 ? $"{request.RequestType}Response" : "RequestResponse";
        string parameters = GetParameters(request.RequestFields);
        part.WriteLine("    public async Task<{0}> {1}Async({2}) {{", returnType, request.RequestType, parameters);
        string assignments = GetAssignments(request.RequestFields);
        part.WriteLine("      return (await _clientSocket.RequestAsync(new {0}() {1}, cancellation).ConfigureAwait(false) as {2})!;", request.RequestType, assignments, returnType);
        part.WriteLine("    }");
        part.WriteLine();
      }

      part.Write("    #endregion");

      string previous = await File.ReadAllTextAsync(EventInterfaceGenerator.ObsClientPath).ConfigureAwait(false);

      bool isReplaced = false;
      string result = Regex.Replace(previous, @"    #region Requests\r\n.*?#endregion", (match) => {
        isReplaced = true;
        return part.ToString();
      }, RegexOptions.Singleline);
      if (!isReplaced) {
        throw new Exception("Unexpected file");
      }

      await File.WriteAllTextAsync(EventInterfaceGenerator.ObsClientPath, result).ConfigureAwait(false);
    }

    private static void PatchTriggerHotkeyByKeySequence(List<ObsRequestField> requestFields) {
      int index = requestFields.FindIndex(x => x.ValueName == "keyModifiers");
      requestFields.RemoveAt(index);
      foreach (var field in requestFields) {
        if (field.ValueName!.StartsWith("keyModifiers.")) {
          field.ValueName = field.ValueName.Replace("keyModifiers.", "");
        }
      }
    }

    private static string GetAssignments(IEnumerable<ObsRequestField> parameters) {
      if (parameters.Any(x => x.ValueName == "shift")) {
        return "{ KeyId = keyId, KeyModifiers = new KeyModifiers() { Shift = shift, Control = control, Alt = alt, Command = command } }";
      }

      var stringifieds = new List<string>();
      var builder = new StringBuilder();
      foreach (var parameter in parameters) {
        builder.Clear();
        if (parameter.ValueName == "requestType") {
          builder.Append("VendorRequestType");
        }
        else {
          builder.Append(char.ToUpper(parameter.ValueName![0]));
          builder.Append(parameter.ValueName[1..]);
        }
        builder.Append(" = ");

        builder.Append(parameter.ValueName);
        stringifieds.Add(builder.ToString());
      }

      return stringifieds.Count > 0 ? $"{{ {string.Join(", ", stringifieds)} }}" : "{ }";
    }

    private static string GetParameters(IEnumerable<ObsRequestField> parameters) {
      var stringifieds = new List<string>();
      var builder = new StringBuilder();

      foreach (var parameter in parameters) {
        builder.Clear();

        string type = TransformHelper.ToCSharpType(parameter.ValueType!, parameter.ValueDescription!);
        builder.Append(type);
        if (parameter.ValueOptional && !type.EndsWith('?')) {
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
  }
}
