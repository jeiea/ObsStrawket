using ObsDotnetSocket.Test.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ObsDotnetSocket.Test {
  public class SimpleTest {
    [Fact(Timeout = 1000)]
    public async Task TestDisconnectedRequestAsync() {
      try {
        var client = ClientFlow.GetDebugClient();

        await client.GetRecordDirectoryAsync().ConfigureAwait(false);
        Assert.Fail("Exception not fired");
      }
      catch (InvalidOperationException) { }
    }
  }
}
