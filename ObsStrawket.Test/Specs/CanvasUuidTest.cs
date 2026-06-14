using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CanvasUuidTest {
    [Fact]
    public async Task SendsCanvasUuidForEverySupportedRequestAsync() {
      await SpecTester.TestAsync(new CanvasUuidFlow());
    }
  }

  internal class CanvasUuidFlow : ITestFlow {
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

    private readonly bool _useExistingCanvas;

    public CanvasUuidFlow(bool useExistingCanvas = false) {
      _useExistingCanvas = useExistingCanvas;
    }

    public async Task RequestAsync(ObsClientSocket client) {
      if (_useExistingCanvas) {
        var canvases = await client.GetCanvasListAsync().ConfigureAwait(false);
        var canvas = Assert.Single(canvases.Canvases);
        string canvasUuid = canvas["canvasUuid"]!.Value.GetString()!;
        var scenes = await client.GetSceneListAsync(canvasUuid).ConfigureAwait(false);
        Assert.NotEmpty(scenes.Scenes);
        return;
      }

      var settings = new Dictionary<string, JsonElement?>();
      const int sceneItemId = 1;

      _ = await client.GetSourceActiveAsync(sourceName: _source, canvasUuid: _canvasUuid).ConfigureAwait(false);
      _ = await client.GetSourceScreenshotAsync("png", sourceName: _source, canvasUuid: _canvasUuid).ConfigureAwait(false);
      _ = await client.SaveSourceScreenshotAsync(
        "png",
        "screenshot.png",
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.GetSceneListAsync(_canvasUuid).ConfigureAwait(false);
      _ = await client.CreateSceneAsync(_scene, _canvasUuid).ConfigureAwait(false);
      _ = await client.RemoveSceneAsync(sceneName: _scene, canvasUuid: _canvasUuid).ConfigureAwait(false);
      _ = await client.SetSceneNameAsync("Renamed", sceneName: _scene, canvasUuid: _canvasUuid).ConfigureAwait(false);
      _ = await client.GetSceneSceneTransitionOverrideAsync(
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSceneSceneTransitionOverrideAsync(
        sceneName: _scene,
        transitionName: "Fade",
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.CreateInputAsync(
        "Input",
        "browser_source",
        sceneName: _scene,
        inputSettings: settings,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);

      _ = await client.GetSourceFilterListAsync(sourceName: _source, canvasUuid: _canvasUuid).ConfigureAwait(false);
      _ = await client.CreateSourceFilterAsync(
        _filter,
        "color_filter",
        sourceName: _source,
        filterSettings: settings,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.RemoveSourceFilterAsync(
        _filter,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSourceFilterNameAsync(
        _filter,
        "Renamed filter",
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.GetSourceFilterAsync(
        _filter,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSourceFilterIndexAsync(
        _filter,
        0,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSourceFilterSettingsAsync(
        _filter,
        settings,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSourceFilterEnabledAsync(
        _filter,
        true,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);

      _ = await client.GetSceneItemListAsync(sceneName: _scene, canvasUuid: _canvasUuid).ConfigureAwait(false);
      _ = await client.GetGroupSceneItemListAsync(sceneName: _scene, canvasUuid: _canvasUuid).ConfigureAwait(false);
      _ = await client.GetSceneItemIdAsync(
        _source,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.GetSceneItemSourceAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.CreateSceneItemAsync(
        sceneName: _scene,
        sourceName: _source,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.RemoveSceneItemAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.DuplicateSceneItemAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.GetSceneItemTransformAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSceneItemTransformAsync(
        sceneItemId,
        settings,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.GetSceneItemEnabledAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSceneItemEnabledAsync(
        sceneItemId,
        true,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.GetSceneItemLockedAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSceneItemLockedAsync(
        sceneItemId,
        true,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.GetSceneItemIndexAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSceneItemIndexAsync(
        sceneItemId,
        0,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.GetSceneItemBlendModeAsync(
        sceneItemId,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.SetSceneItemBlendModeAsync(
        sceneItemId,
        BlendingType.Normal,
        sceneName: _scene,
        canvasUuid: _canvasUuid
      ).ConfigureAwait(false);
      _ = await client.OpenSourceProjectorAsync(sourceName: _source, canvasUuid: _canvasUuid).ConfigureAwait(false);
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
