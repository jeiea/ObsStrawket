using ObsStrawket.Test.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSourceScreenshotTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSourceScreenshotFlow()).ConfigureAwait(false);
    }
  }

  class GetSourceScreenshotFlow : ITestFlow {
    public static readonly string Base64SmallestPng = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAACklEQVR4nGMAAQAABQABDQottAAAAABJRU5ErkJggg==";
    public async Task RequestAsync(ObsClientSocket client) {
      var list = await client.GetSceneListAsync().ConfigureAwait(false);
      var response = await client.GetSourceScreenshotAsync(list.CurrentProgramSceneName, imageFormat: "png", imageWidth: 1920, imageHeight: 1080).ConfigureAwait(false);
      byte[] buffer = Convert.FromBase64String(response.ImageData.Replace("data:image/png;base64,", ""));
      Assert.InRange(buffer.Length, 1, 300000);
    }

    public async Task RespondAsync(MockServerSession session) {
      await new GetSceneListFlow().RespondAsync(session).ConfigureAwait(false);

      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""imageCompressionQuality"": null,
      ""imageFormat"": ""png"",
      ""imageHeight"": 1080,
      ""imageWidth"": 1920,
      ""sourceName"": ""Scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetSourceScreenshot""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""GetSourceScreenshot"",
    ""responseData"": {
      ""imageData"": ""data:image/png;base64,{png}""
    }
  },
  ""op"": 7
}".Replace("{png}", Base64SmallestPng).Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
