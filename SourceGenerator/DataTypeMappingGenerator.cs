using System.IO;
using System.Threading.Tasks;

namespace SourceGenerator {

  internal class DataTypeMappingGenerator(IDirectoryHelper directoryHelper, ISourceFetcher fetcher) {

    public static string GetDataTypeMapping(ProtocolJson protocol) {
      using var writer = GeneratedText.CreateStringWriter();

      writer.WriteLine(GeneratedText.NormalizeNewLine("""
using ObsStrawket.DataTypes.Predefineds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObsStrawket.DataTypes {
  internal static class DataTypeMapping {
    internal static readonly Dictionary<string, Type> EventToTypes = new Type[] {
"""));

      string? previousCategory = null;
      foreach (var @event in protocol.Events) {
        if (@event.Category != previousCategory) {
          writer.WriteLine();
          writer.WriteLine($"      // {TransformHelper.ToPascalCase(@event.Category)} Events");
          previousCategory = @event.Category;
        }
        writer.WriteLine($"      typeof({@event.EventType}),");
      }

      writer.WriteLine(GeneratedText.NormalizeNewLine("""
    }.ToDictionary(static x => x.Name, static x => x);

    internal record RequestMapping(Type Request, Type Response, bool IsRequestEmpty = false);

    internal static readonly Dictionary<string, RequestMapping> RequestToTypes = new RequestMapping[] {
"""));

      previousCategory = null;
      foreach (var request in protocol.Requests) {
        if (request.Category != previousCategory) {
          writer.WriteLine();
          writer.WriteLine($"      // {TransformHelper.ToPascalCase(request.Category!)} Requests");
          previousCategory = request.Category;
        }
        string response = TransformHelper.ToResponseType(request);
        string isRequestEmpty = request.RequestFields.Count == 0 ? ", true" : "";
        writer.WriteLine($"      new (typeof({request.RequestType}), typeof({response}){isRequestEmpty}),");
      }
      writer.WriteLine(GeneratedText.NormalizeNewLine("""
    }.ToDictionary(static x => x.Request.Name, static x => x);
  }
}
"""));

      return writer.ToString();
    }

    public async Task GenerateAsync() {
      var json = await fetcher.GetModifiedProtocolJsonAsync().ConfigureAwait(false);
      string path = $"{directoryHelper.MainProjectDirectory}/DataTypes/DataTypeMapping.cs";
      await File.WriteAllTextAsync(path, GetDataTypeMapping(json)).ConfigureAwait(false);
    }
  }
}
