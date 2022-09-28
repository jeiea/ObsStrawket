using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSceneItemTransformTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSceneItemTransformFlow()).ConfigureAwait(false);
    }
  }

  class SetSceneItemTransformFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetSceneItemTransformAsync(
        sceneName: CreateSceneFlow.NewScene,
        sceneItemId: CreateSceneItemFlow.CreatedItemId,
        sceneItemTransform: new Dictionary<string, object?> {
          { "boundsType", "OBS_BOUNDS_SCALE_TO_WIDTH" }
        }
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneItemId"": 3,
      ""sceneItemTransform"": {
        ""boundsType"": ""OBS_BOUNDS_SCALE_TO_WIDTH""
      },
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetSceneItemTransform""
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
    ""requestType"": ""SetSceneItemTransform""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
