using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SourceGenerator {

  internal class Program {

    public static async Task Main() {
      var builder = Host.CreateApplicationBuilder();

      var services = builder.Services;
      services.AddSingleton<IDirectoryHelper, DirectoryHelper>();
      services.AddSingleton<ISourceFetcher, SourceFetcher>();
      services.AddSingleton<EnumGenerator>();
      services.AddSingleton<EventGenerator>();
      services.AddSingleton<EventInterfaceGenerator>();
      services.AddSingleton<RequestInterfaceGenerator>();
      services.AddSingleton<RequestResponseGenerator>();
      services.AddSingleton<DataTypeMappingGenerator>();

      using var host = builder.Build();
      var provider = host.Services;

      await Task.WhenAll([
        provider.GetRequiredService<EnumGenerator>().GenerateAsync(),
        provider.GetRequiredService<EventGenerator>().GenerateAsync(),
        provider.GetRequiredService<EventInterfaceGenerator>().GenerateAsync(),
        provider.GetRequiredService<RequestInterfaceGenerator>().GenerateAsync(),
        provider.GetRequiredService<RequestResponseGenerator>().GenerateAsync(),
        provider.GetRequiredService<DataTypeMappingGenerator>().GenerateAsync(),
      ]).ConfigureAwait(false);
    }
  }
}
