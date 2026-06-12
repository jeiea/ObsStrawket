using ObsStrawket.Test.Utilities;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class SequentialTest {
    [Fact]
    public async Task TestAsync() {
      var cancellation = new CancellationTokenSource();
      using var server = new MockServer().Run(cancellation.Token);
      var client = ClientFlow.GetDebugClient();
      await new ClientFlow().RunClientAsync(server.Uri, client, cancellation: cancellation.Token);
      await new ClientFlow().RunClientAsync(server.Uri, client, cancellation: cancellation.Token);
      client.Dispose();
      // Duplicate disposal should be no problem.
      client.Dispose();
    }
  }
}
