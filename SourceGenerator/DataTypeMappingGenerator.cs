using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenerator {

  internal class DataTypeMappingGenerator(IDirectoryHelper directoryHelper, ISourceFetcher fetcher) {

    public static string GetDataTypeMapping(ProtocolJson protocol) {
      var builder = new StringBuilder();

      _ = builder.AppendLine("""
using ObsStrawket.DataTypes.Predefineds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObsStrawket.DataTypes {
  internal static class DataTypeMapping {
    internal static readonly Dictionary<string, Type> EventToTypes = new Type[] {
""");

      string? previousCategory = null;
      foreach (var @event in protocol.Events) {
        if (@event.Category != previousCategory) {
          _ = builder.AppendLine();
          _ = builder.AppendLine(CultureInfo.InvariantCulture, $"      // {TransformHelper.ToPascalCase(@event.Category)} Events");
          previousCategory = @event.Category;
        }
        _ = builder.AppendLine(CultureInfo.InvariantCulture, $"      typeof({@event.EventType}),");
      }

      _ = builder.AppendLine("""
    }.ToDictionary(x => x.Name, x => x);

    internal record RequestMapping(Type Request, Type Response, bool IsRequestEmpty = false);

    internal static readonly Dictionary<string, RequestMapping> RequestToTypes = new RequestMapping[] {
""");

      previousCategory = null;
      foreach (var request in protocol.Requests) {
        if (request.Category != previousCategory) {
          _ = builder.AppendLine();
          _ = builder.AppendLine(CultureInfo.InvariantCulture, $"      // {TransformHelper.ToPascalCase(request.Category!)} Requests");
          previousCategory = request.Category;
        }
        string response = request.ResponseFields.Count > 0 ? $"{request.RequestType}Response" : "RequestResponse";
        string isRequestEmpty = request.RequestFields.Count == 0 ? ", true" : "";
        _ = builder.AppendLine(CultureInfo.InvariantCulture, $"      new (typeof({request.RequestType}), typeof({response}){isRequestEmpty}),");
      }
      _ = builder.AppendLine("""
    }.ToDictionary(x => x.Request.Name, x => x);
  }
}
""");

      return builder.ToString();
    }

    public async Task GenerateAsync() {
      var json = await fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);
      string path = $"{directoryHelper.MainProjectDirectory}/DataTypes/DataTypeMapping.cs";
      await File.WriteAllTextAsync(path, GetDataTypeMapping(json)).ConfigureAwait(false);
    }
  }
}
