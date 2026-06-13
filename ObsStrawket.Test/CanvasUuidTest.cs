using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class CanvasUuidTest {
    [Fact]
    public async Task SendsCanvasUuidForEverySupportedRequestAsync() {
      await SpecTester.TestAsync(new CanvasUuidFlow());
    }
  }

  class CanvasUuidFlow : ITestFlow {
    private const string _canvasUuid = "6f9619ff-8b86-d011-b42d-00cf4fc964ff";
    private const string _scene = "Scene";
    private const string _source = "Source";
    private const string _filter = "Filter";

    private static readonly string[] _requestTypes = [
      "GetSourceActive",
      "GetSourceScreenshot",
      "SaveSourceScreenshot",
      "GetSceneList",
      "CreateScene",
      "RemoveScene",
      "SetSceneName",
      "GetSceneSceneTransitionOverride",
      "SetSceneSceneTransitionOverride",
      "CreateInput",
      "GetSourceFilterList",
      "CreateSourceFilter",
      "RemoveSourceFilter",
      "SetSourceFilterName",
      "GetSourceFilter",
      "SetSourceFilterIndex",
      "SetSourceFilterSettings",
      "SetSourceFilterEnabled",
      "GetSceneItemList",
      "GetGroupSceneItemList",
      "GetSceneItemId",
      "GetSceneItemSource",
      "CreateSceneItem",
      "RemoveSceneItem",
      "DuplicateSceneItem",
      "GetSceneItemTransform",
      "SetSceneItemTransform",
      "GetSceneItemEnabled",
      "SetSceneItemEnabled",
      "GetSceneItemLocked",
      "SetSceneItemLocked",
      "GetSceneItemIndex",
      "SetSceneItemIndex",
      "GetSceneItemBlendMode",
      "SetSceneItemBlendMode",
      "OpenSourceProjector",
    ];

    public async Task RequestAsync(ObsClientSocket client) {
      var settings = new Dictionary<string, JsonElement?>();
      const int sceneItemId = 1;

      await client.GetSourceActiveAsync(sourceName: _source, canvasUuid: _canvasUuid).ConfigureAwait(false);
      await client.GetSourceScreenshotAsync("png", sourceName: _source, canvasUuid: _canvasUuid).ConfigureAwait(false);
      await client.SaveSourceScreenshotAsync(
        "png",
        "screenshot.png",
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.GetSceneListAsync(_canvasUuid).ConfigureAwait(false);
      await client.CreateSceneAsync(_scene, _canvasUuid).ConfigureAwait(false);
      await client.RemoveSceneAsync(sceneName: _scene, canvasUuid: _canvasUuid).ConfigureAwait(false);
      await client.SetSceneNameAsync("Renamed", sceneName: _scene, canvasUuid: _canvasUuid).ConfigureAwait(false);
      await client.GetSceneSceneTransitionOverrideAsync(
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSceneSceneTransitionOverrideAsync(
        sceneName: _scene,
        transitionName: "Fade",
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.CreateInputAsync(
        "Input",
        "browser_source",
        sceneName: _scene,
        inputSettings: settings,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);

      await client.GetSourceFilterListAsync(sourceName: _source, canvasUuid: _canvasUuid).ConfigureAwait(false);
      await client.CreateSourceFilterAsync(
        _filter,
        "color_filter",
        sourceName: _source,
        filterSettings: settings,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.RemoveSourceFilterAsync(
        _filter,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSourceFilterNameAsync(
        _filter,
        "Renamed filter",
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.GetSourceFilterAsync(
        _filter,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSourceFilterIndexAsync(
        _filter,
        0,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSourceFilterSettingsAsync(
        _filter,
        settings,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSourceFilterEnabledAsync(
        _filter,
        true,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);

      await client.GetSceneItemListAsync(sceneName: _scene, canvasUuid: _canvasUuid).ConfigureAwait(false);
      await client.GetGroupSceneItemListAsync(sceneName: _scene, canvasUuid: _canvasUuid).ConfigureAwait(false);
      await client.GetSceneItemIdAsync(
        _source,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.GetSceneItemSourceAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.CreateSceneItemAsync(
        sceneName: _scene,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.RemoveSceneItemAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.DuplicateSceneItemAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.GetSceneItemTransformAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSceneItemTransformAsync(
        sceneItemId,
        settings,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.GetSceneItemEnabledAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSceneItemEnabledAsync(
        sceneItemId,
        true,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.GetSceneItemLockedAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSceneItemLockedAsync(
        sceneItemId,
        true,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.GetSceneItemIndexAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSceneItemIndexAsync(
        sceneItemId,
        0,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.GetSceneItemBlendModeAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.SetSceneItemBlendModeAsync(
        sceneItemId,
        BlendingType.Normal,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      await client.OpenSourceProjectorAsync(sourceName: _source, canvasUuid: _canvasUuid).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      foreach (string requestType in _requestTypes) {
        await session.ReceiveRequestFieldAndSendSuccessAsync(
          requestType,
          "canvasUuid",
          _canvasUuid
        ).ConfigureAwait(false);
      }
    }
  }
}
