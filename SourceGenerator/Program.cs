using System.Threading.Tasks;

namespace SourceGenerator {
  internal class Program {
    public async static Task Main() {
      await new EnumGenerator().GenerateAsync().ConfigureAwait(false);
      await new EventGenerator().GenerateAsync().ConfigureAwait(false);
      await new EventInterfaceGenerator().GenerateAsync().ConfigureAwait(false);
      await new RequestInterfaceGenerator().GenerateAsync().ConfigureAwait(false);
      await new RequestResponseGenerator().GenerateAsync().ConfigureAwait(false);
    }
  }
}
