using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneItemBlendModeTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneItemBlendModeFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneItemBlendModeFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneItemBlendModeAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: CreateSceneItemFlow.CreatedItemId
      ).ConfigureAwait(false);
      Assert.Equal(SetSceneItemBlendModeFlow.NewBlendingMode, response.SceneItemBlendMode);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneItemId"": 2,
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetSceneItemBlendMode""
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
    ""requestType"": ""GetSceneItemBlendMode"",
    ""responseData"": {
      ""sceneItemBlendMode"": ""OBS_BLEND_SCREEN""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
