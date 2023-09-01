using ObsStrawket.Test.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SaveSourceScreenshotTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SaveSourceScreenshotFlow()).ConfigureAwait(false);
    }
  }

  class SaveSourceScreenshotFlow : ITestFlow {
    private static readonly string _fileName = "screenshot.png";

    public async Task RequestAsync(ObsClientSocket client) {
      var list = await client.GetSceneListAsync().ConfigureAwait(false);
      string path = Path.Combine(Directory.GetCurrentDirectory(), _fileName);
      File.Delete(path);
      await client.SaveSourceScreenshotAsync(list.CurrentProgramSceneName, "png", path, imageWidth: 1920, imageHeight: 1080).ConfigureAwait(false);
      Assert.InRange(new FileInfo(path).Length, 1, 600000);
    }

    public async Task RespondAsync(MockServerSession session) {
      await new GetSceneListFlow().RespondAsync(session).ConfigureAwait(false);

      string path = Path.Combine(Directory.GetCurrentDirectory(), _fileName);
      string? guid = await session.ReceiveAsync($$"""
{
  "d": {
    "requestData": {
      "imageCompressionQuality": null,
      "imageFilePath": "{{path.Replace("\\", "\\\\")}}",
      "imageFormat": "png",
      "imageHeight": 1080,
      "imageWidth": 1920,
      "sourceName": "Scene"
    },
    "requestId": "{guid}",
    "requestType": "SaveSourceScreenshot"
  },
  "op": 6
}
""").ConfigureAwait(false);

      byte[] png = Convert.FromBase64String(GetSourceScreenshotFlow.Base64SmallestPng);
      await File.WriteAllBytesAsync(path, png).ConfigureAwait(false);

      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "SaveSourceScreenshot"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
