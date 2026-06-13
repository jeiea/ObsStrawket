using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CallVendorRequestTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CallVendorRequestFlow());
    }
  }

  class CallVendorRequestFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.CallVendorRequestAsync(
        vendorName: "test-vendor",
        requestType: "echo",
        requestData: new Dictionary<string, JsonElement?> {
          ["message"] = "hello".ToJsonElement(),
        }
      ).ConfigureAwait(false);
      Assert.Equal("test-vendor", response.VendorName);
      Assert.Equal("echo", response.VendorRequestType);
      Assert.Equal("hello", response.ResponseData["message"]!.Value.GetString());
    }

    public async Task RespondAsync(MockServerSession session) {
      string guid = (await session.ReceiveRequestAsync("CallVendorRequest", """
{
  "vendorName": "test-vendor",
  "requestType": "echo",
  "requestData": {
    "message": "hello"
  }
}
""").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("CallVendorRequest", guid, """
{
  "vendorName": "test-vendor",
  "requestType": "echo",
  "responseData": {
    "message": "hello"
  }
}
""").ConfigureAwait(false);
    }
  }
}
