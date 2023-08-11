using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceGenerator {
  internal class EnumGenerator {
    private readonly SourceFetcher _fetcher = new();

    public async Task GenerateAsync() {
      var json = await _fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);

      var obsMediaInputAction = json.Enums.First((en) => en.EnumType == "ObsMediaInputAction");
      obsMediaInputAction.EnumType = "MediaInputAction";
      foreach (var identifier in obsMediaInputAction.EnumIdentifiers) {
        identifier.EnumIdentifier = $"{identifier.EnumIdentifier[33]}{identifier.EnumIdentifier[34..].ToLower()}";
      }

      var descriptions = new Dictionary<string, string> {
        { "EventSubscription", "Flag specifying which events to subscribe to." },
        { "RequestBatchExecutionType", "Type of method to process request" },
        { "RequestStatus", "Request result code" },
        { "MediaInputAction", "Actions used with media source and `TriggerMediaInputAction`." },
        { "WebSocketCloseCode", "Reason that OBS closed the connection." },
        { "WebSocketOpCode", "Type of OBS websocket protocol message." },
        { "ObsOutputState", "Represents output sending state." },
      };

      using var file = File.CreateText("../../../../ObsStrawket/DataTypes/Predefineds/Enums.cs");
      file.Write(@"using MessagePack;
using MessagePack.Formatters;
using System;
using System.Runtime.Serialization;

namespace ObsStrawket.DataTypes.Predefineds {");

      foreach (var en in json.Enums) {
        file.WriteLine();
        file.WriteLine("  /// <summary>");
        file.WriteLine("  /// {0}", TransformHelper.EscapeForXml(descriptions[en.EnumType]).Replace(Environment.NewLine, $"<br />{Environment.NewLine}  /// "));
        file.WriteLine("  /// </summary>");
        if (en.EnumType == "EventSubscription") {
          file.WriteLine("  [Flags]");
        }
        string sample = $"{en.EnumIdentifiers.Last().EnumValue}";
        bool isStringEnum = Regex.IsMatch(sample, @"[A-Z]");
        if (isStringEnum) {
          file.WriteLine("  [MessagePackFormatter(typeof(EnumAsStringFormatter<{0}>))]", en.EnumType);
        }
        file.Write("  public enum {0} {{", en.EnumType);
        foreach (var identifier in en.EnumIdentifiers!) {
          file.WriteLine();
          file.WriteLine("    /// <summary>");
          file.WriteLine("    /// {0}", TransformHelper.EscapeForXml(identifier.Description).Replace(Environment.NewLine, $"<br />{Environment.NewLine}    /// "));
          file.WriteLine("    /// </summary>");
          if (identifier.Deprecated) {
            file.WriteLine("    [Obsolete]");
          }

          if (isStringEnum) {
            file.WriteLine("    [EnumMember(Value = \"{0}\")]", identifier.EnumValue);
          }
          else {
            file.WriteLine("    [EnumMember]");
          }
          file.Write("    {0}", identifier.EnumIdentifier);
          if (isStringEnum) {
            file.WriteLine(',');
          }
          else {
            string str = $"{identifier.EnumValue}";
            if (str[0] == '(') {
              str = str[1..(str.Length - 1)];
            }
            file.WriteLine(" = {0},", str);
          }
        }
        file.WriteLine("  }");
      }

      file.WriteLine("}");
    }
  }
}
