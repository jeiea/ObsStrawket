using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SourceGenerator {

  internal class Program {

    public static async Task Main(string[] args) {
      bool updateUpstream = ParseUpdateUpstream(args);
      var builder = Host.CreateApplicationBuilder();

      var services = builder.Services;
      _ = services.AddSingleton<IDirectoryHelper, DirectoryHelper>();
      _ = services.AddSingleton<HttpClient>();
      _ = services.AddSingleton<ISourceFetcher, SourceFetcher>();
      _ = services.AddSingleton<EnumGenerator>();
      _ = services.AddSingleton<EventGenerator>();
      _ = services.AddSingleton<EventInterfaceGenerator>();
      _ = services.AddSingleton<RequestInterfaceGenerator>();
      _ = services.AddSingleton<RequestResponseGenerator>();
      _ = services.AddSingleton<DataTypeMappingGenerator>();

      using var host = builder.Build();
      var provider = host.Services;

      await provider.GetRequiredService<ISourceFetcher>().PrepareAsync(updateUpstream).ConfigureAwait(false);

      await Task.WhenAll([
        provider.GetRequiredService<EnumGenerator>().GenerateAsync(),
        provider.GetRequiredService<EventGenerator>().GenerateAsync(),
        provider.GetRequiredService<EventInterfaceGenerator>().GenerateAsync(),
        provider.GetRequiredService<RequestInterfaceGenerator>().GenerateAsync(),
        provider.GetRequiredService<RequestResponseGenerator>().GenerateAsync(),
        provider.GetRequiredService<DataTypeMappingGenerator>().GenerateAsync(),
      ]).ConfigureAwait(false);
    }

    internal static bool ParseUpdateUpstream(string[] args) {
      return args.Length != 0 && (args.Length == 1 && args[0] == "--update-upstream"
        ? true
        : throw new ArgumentException(
        $"Unknown arguments: {string.Join(" ", args)}. Only --update-upstream is supported."));
    }
  }
}
