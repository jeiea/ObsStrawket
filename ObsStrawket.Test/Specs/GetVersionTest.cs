using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetVersionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetVersionFlow()).ConfigureAwait(false);
    }
  }

  class GetVersionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetVersionAsync().ConfigureAwait(false);
      Assert.Contains("GetVersion", response.AvailableRequests);
      Assert.Contains("bmp", response.SupportedImageFormats);
      Assert.True(response.ObsVersion.Length > 0);
      Assert.True(response.ObsWebSocketVersion.Length > 0);
      Assert.True(response.Platform.Length > 0);
      Assert.True(response.PlatformDescription.Length > 0);
      Assert.True(response.RpcVersion >= 0);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetVersion""
  },
  ""op"": 6
}").ConfigureAwait(false);

      await session.SendGetVersionResponseAsync(guid!).ConfigureAwait(false);
    }
  }
}
