using System.Threading.Tasks;

namespace SourceGenerator {
  internal class Program {
    public async static Task Main() {
      //await new SpecsGenerator().GenerateAsync().ConfigureAwait(false);
      await new RequestResponseGenerator().GenerateAsync().ConfigureAwait(false);
    }
  }
}
