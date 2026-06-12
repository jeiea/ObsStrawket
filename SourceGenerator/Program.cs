using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SourceGenerator {
  internal class Program {
    public async static Task Main() {
      var builder = Host.CreateApplicationBuilder();

      var services = builder.Services;
      services.AddSingleton<IDirectoryHelper, DirectoryHelper>();
      services.AddSingleton<ISourceFetcher, SourceFetcher>();
      services.AddSingleton<EnumGenerator>();
      services.AddSingleton<EventGenerator>();
      services.AddSingleton<EventInterfaceGenerator>();
      services.AddSingleton<RequestInterfaceGenerator>();
      services.AddSingleton<RequestResponseGenerator>();

      using var host = builder.Build();
      var provider = host.Services;

      await provider.GetRequiredService<EnumGenerator>().GenerateAsync().ConfigureAwait(false);
      await provider.GetRequiredService<EventGenerator>().GenerateAsync().ConfigureAwait(false);
      await provider.GetRequiredService<EventInterfaceGenerator>().GenerateAsync().ConfigureAwait(false);
      await provider.GetRequiredService<RequestInterfaceGenerator>().GenerateAsync().ConfigureAwait(false);
      await provider.GetRequiredService<RequestResponseGenerator>().GenerateAsync().ConfigureAwait(false);
    }
  }
}
