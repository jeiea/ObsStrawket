using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal partial class RequestInterfaceGenerator {
    private readonly IDirectoryHelper _directoryHelper;
    private readonly ISourceFetcher _fetcher;

    public RequestInterfaceGenerator(IDirectoryHelper directoryHelper, ISourceFetcher fetcher) {
      _directoryHelper = directoryHelper;
      _fetcher = fetcher;
    }


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
        part.WriteLine("    /// <exception cref=\"InvalidOperationException\">The client is not connected.</exception>");
        part.WriteLine("    /// <exception cref=\"OperationCanceledException\">The caller cancels the operation.</exception>");
        part.WriteLine("    /// <exception cref=\"ObsRequestException\">OBS rejects the request.</exception>");
        part.WriteLine("    /// <exception cref=\"ObsConnectionException\">The connection fails before a response is received.</exception>");
        part.WriteLine("    /// <exception cref=\"ObsProtocolException\">OBS sends an invalid response.</exception>");

        string returnType = TransformHelper.ToResponseType(request);
        string parameters = GetParameters(request.RequestFields);
        part.WriteLine("    public async Task<{0}> {1}Async({2}) {{", returnType, request.RequestType, parameters);
        string assignments = GetAssignments(request.RequestFields);
        part.WriteLine("      return await _clientSocket.RequestAsync(new {0}() {1}, cancellation).ConfigureAwait(false);", request.RequestType, assignments);
        part.WriteLine("    }");
        part.WriteLine();
      }

      part.Write("    #endregion");

      string previous = await File.ReadAllTextAsync(_directoryHelper.ObsClientPath).ConfigureAwait(false);

      bool isReplaced = false;
      string result = RequestsRegionPattern().Replace(previous, (match) => {
        isReplaced = true;
        return part.ToString();
      });
      if (!isReplaced) {
        throw new InvalidOperationException("Unexpected file");
      }

      await File.WriteAllTextAsync(_directoryHelper.ObsClientPath, result).ConfigureAwait(false);
    }

    [GeneratedRegex(@"    #region Requests\r?\n.*?#endregion", RegexOptions.Singleline)]
    private static partial Regex RequestsRegionPattern();

    private static void PatchTriggerHotkeyByKeySequence(List<ObsRequestField> requestFields) {
      int index = requestFields.FindIndex(static x => x.ValueName == "keyModifiers");
      requestFields.RemoveAt(index);
      foreach (var field in requestFields) {
        if (field.ValueName!.StartsWith("keyModifiers.", StringComparison.Ordinal)) {
          field.ValueName = field.ValueName.Replace("keyModifiers.", "");
        }
      }
    }

    private static string GetAssignments(IEnumerable<ObsRequestField> parameters) {
      if (parameters.Any(static x => x.ValueName == "shift")) {
        return "{ KeyId = keyId, KeyModifiers = new KeyModifiers() { Shift = shift, Control = control, Alt = alt, Command = command } }";
      }

      var stringifieds = new List<string>();
      var builder = new StringBuilder();
      foreach (var parameter in parameters) {
        _ = builder.Clear();
        if (parameter.ValueName == "requestType") {
          _ = builder.Append("VendorRequestType");
        }
        else {
          _ = builder.Append(char.ToUpper(parameter.ValueName![0], CultureInfo.InvariantCulture));
          _ = builder.Append(parameter.ValueName[1..]);
        }
        _ = builder.Append(" = ");

        _ = builder.Append(parameter.ValueName);
        stringifieds.Add(builder.ToString());
      }

      return stringifieds.Count > 0 ? $"{{ {string.Join(", ", stringifieds)} }}" : "{ }";
    }

    private static string GetParameters(IEnumerable<ObsRequestField> parameters) {
      var stringifieds = new List<string>();
      var builder = new StringBuilder();

      foreach (var parameter in parameters) {
        _ = builder.Clear();

        string type = TransformHelper.ToCSharpType(parameter.ValueType!, parameter.ValueDescription!);
        _ = builder.Append(type);
        if (parameter.ValueOptional && !type.EndsWith('?')) {
          _ = builder.Append('?');
        }
        _ = builder.Append(' ');
        _ = builder.Append(parameter.ValueName);
        if (parameter.ValueOptional) {
          _ = builder.Append(" = default");
        }
        stringifieds.Add(builder.ToString());
      }
      stringifieds.Add("CancellationToken cancellation = default");

      return string.Join(", ", stringifieds);
    }
  }
}
