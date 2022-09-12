using System.Threading.Tasks;

namespace SourceGenerator {
  internal class Program {
    public async static Task Main() {
      //await new SpecsGenerator().GenerateAsync().ConfigureAwait(false);
      await new RequestInterfaceGenerator().GenerateAsync().ConfigureAwait(false);
    }
  }
}
