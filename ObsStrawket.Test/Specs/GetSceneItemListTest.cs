using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetSceneItemListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetSceneItemListFlow()).ConfigureAwait(false);
    }
  }

  class GetSceneItemListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetSceneItemListAsync(sceneName: CreateSceneFlow.NewScene).ConfigureAwait(false);
      Assert.NotEmpty(response.SceneItems);
      Assert.Equal(CreateInputFlow.InputKind, response.SceneItems[0].InputKind);
      Assert.Equal(CreateInputFlow.InputName, response.SceneItems[0].SourceName);
      Assert.Equal(SourceType.Input, response.SceneItems[0].SourceType);
      Assert.Equal(BlendingType.Normal, response.SceneItems[0].BlendMode);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneName"": ""test scene""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetSceneItemList""
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
    ""requestType"": ""GetSceneItemList"",
    ""responseData"": {
      ""sceneItems"": [
        {
          ""inputKind"": ""browser_source"",
          ""isGroup"": null,
          ""sceneItemBlendMode"": ""OBS_BLEND_NORMAL"",
          ""sceneItemEnabled"": true,
          ""sceneItemId"": 1,
          ""sceneItemIndex"": 0,
          ""sceneItemLocked"": false,
          ""sceneItemTransform"": {
            ""alignment"": 5,
            ""boundsAlignment"": 0,
            ""boundsHeight"": 0.0,
            ""boundsType"": ""OBS_BOUNDS_NONE"",
            ""boundsWidth"": 0.0,
            ""cropBottom"": 0,
            ""cropLeft"": 0,
            ""cropRight"": 0,
            ""cropTop"": 0,
            ""height"": 600.0,
            ""positionX"": 0.0,
            ""positionY"": 0.0,
            ""rotation"": 0.0,
            ""scaleX"": 1.0,
            ""scaleY"": 1.0,
            ""sourceHeight"": 600.0,
            ""sourceWidth"": 800.0,
            ""width"": 800.0
          },
          ""sourceName"": ""Browser source"",
          ""sourceType"": ""OBS_SOURCE_TYPE_INPUT""
        },
        {
          ""inputKind"": ""browser_source"",
          ""isGroup"": null,
          ""sceneItemBlendMode"": ""OBS_BLEND_NORMAL"",
          ""sceneItemEnabled"": true,
          ""sceneItemId"": 2,
          ""sceneItemIndex"": 1,
          ""sceneItemLocked"": false,
          ""sceneItemTransform"": {
            ""alignment"": 5,
            ""boundsAlignment"": 0,
            ""boundsHeight"": 0.0,
            ""boundsType"": ""OBS_BOUNDS_NONE"",
            ""boundsWidth"": 0.0,
            ""cropBottom"": 0,
            ""cropLeft"": 0,
            ""cropRight"": 0,
            ""cropTop"": 0,
            ""height"": 600.0,
            ""positionX"": 0.0,
            ""positionY"": 0.0,
            ""rotation"": 0.0,
            ""scaleX"": 1.0,
            ""scaleY"": 1.0,
            ""sourceHeight"": 600.0,
            ""sourceWidth"": 800.0,
            ""width"": 800.0
          },
          ""sourceName"": ""Browser source"",
          ""sourceType"": ""OBS_SOURCE_TYPE_INPUT""
        },
        {
          ""inputKind"": ""browser_source"",
          ""isGroup"": null,
          ""sceneItemBlendMode"": ""OBS_BLEND_NORMAL"",
          ""sceneItemEnabled"": true,
          ""sceneItemId"": 3,
          ""sceneItemIndex"": 2,
          ""sceneItemLocked"": false,
          ""sceneItemTransform"": {
            ""alignment"": 5,
            ""boundsAlignment"": 0,
            ""boundsHeight"": 0.0,
            ""boundsType"": ""OBS_BOUNDS_NONE"",
            ""boundsWidth"": 0.0,
            ""cropBottom"": 0,
            ""cropLeft"": 0,
            ""cropRight"": 0,
            ""cropTop"": 0,
            ""height"": 600.0,
            ""positionX"": 0.0,
            ""positionY"": 0.0,
            ""rotation"": 0.0,
            ""scaleX"": 1.0,
            ""scaleY"": 1.0,
            ""sourceHeight"": 600.0,
            ""sourceWidth"": 800.0,
            ""width"": 800.0
          },
          ""sourceName"": ""Browser source"",
          ""sourceType"": ""OBS_SOURCE_TYPE_INPUT""
        }
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
