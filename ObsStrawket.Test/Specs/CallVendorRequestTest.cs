using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CallVendorRequestTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CallVendorRequestFlow()).ConfigureAwait(false);
    }
  }

  class CallVendorRequestFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Yield();
      // TBD
      //var response = await client.CallVendorRequestAsync(vendorName: "test vendor name", requestType: "test request type").ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      await Task.Yield();
      //string? guid = await session.ReceiveAsync(@"").ConfigureAwait(false);
      //await session.SendAsync(@"".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
