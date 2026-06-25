using ObsStrawket.Test.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class BadRequestTest {
    [Fact(Timeout = 1000)]
    public async Task TestAsync() {
      try {
        var client = ClientFlow.GetDebugClient();

        _ = await client.GetRecordDirectoryAsync(TestContext.Current.CancellationToken);
        Assert.Fail("Exception not fired");
      }
      catch (InvalidOperationException) { }
    }
  }
}
