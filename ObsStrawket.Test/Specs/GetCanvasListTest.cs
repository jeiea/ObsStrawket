using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetCanvasListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetCanvasListFlow());
    }
  }

  internal class GetCanvasListFlow : ITestFlow {
    public const string CanvasName = "Main";
    public const string CanvasUuid = "6f9619ff-8b86-d011-b42d-00cf4fc964ff";

    public async Task RequestAsync(ObsClientSocket client) {
      var response = await GetCanvasListAsync(client).ConfigureAwait(false);
      var canvas = Assert.Single(response.Canvases);
      Assert.NotEmpty(canvas["canvasName"]!.Value.GetString()!);
      Assert.NotEmpty(canvas["canvasUuid"]!.Value.GetString()!);
    }

    public static Task<GetCanvasListResponse> GetCanvasListAsync(ObsClientSocket client) {
      return client.GetCanvasListAsync();
    }

    public async Task RespondAsync(MockServerSession session) {
      string guid = (await session.ReceiveRequestAsync("GetCanvasList").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("GetCanvasList", guid, $$"""
{
  "canvases": [
    {
      "canvasName": "{{CanvasName}}",
      "canvasUuid": "{{CanvasUuid}}"
    }
  ]
}
""").ConfigureAwait(false);
    }
  }

}
