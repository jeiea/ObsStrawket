using ObsStrawket.DataTypes.Predefineds;
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

  internal class CallVendorRequestFlow : ITestFlow {
    private readonly string _vendorName;
    private readonly string _requestType;
    private readonly Dictionary<string, JsonElement?>? _requestData;

    public CallVendorRequestFlow(
      string vendorName = "test-vendor",
      string requestType = "echo",
      Dictionary<string, JsonElement?>? requestData = null
    ) {
      _vendorName = vendorName;
      _requestType = requestType;
      _requestData = requestData ?? new Dictionary<string, JsonElement?> {
        ["message"] = "hello".ToJsonElement(),
      };
    }

    public async Task RequestAsync(ObsClientSocket client) {
      var response = await RequestForResponseAsync(client).ConfigureAwait(false);
      Assert.Equal("hello", response.ResponseData["message"]!.Value.GetString());
    }

    public async Task<CallVendorRequestResponse> RequestForResponseAsync(ObsClientSocket client) {
      var response = await client.CallVendorRequestAsync(
        vendorName: _vendorName,
        requestType: _requestType,
        requestData: _requestData
      ).ConfigureAwait(false);
      Assert.Equal(_vendorName, response.VendorName);
      Assert.Equal(_requestType, response.VendorRequestType);
      return response;
    }

    public async Task RespondAsync(MockServerSession session) {
      string guid = (await session.ReceiveRequestAsync("CallVendorRequest", /*lang=json,strict*/ """
{
  "vendorName": "test-vendor",
  "requestType": "echo",
  "requestData": {
    "message": "hello"
  }
}
""").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("CallVendorRequest", guid, /*lang=json,strict*/ """
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
