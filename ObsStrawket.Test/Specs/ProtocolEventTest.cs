using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class ProtocolEventTest {
    [Fact]
    public async Task DispatchesNewEventsAsync() {
      var flow = new ProtocolEventFlow();
      await SpecTester.TestAsync(flow, useChannel: false);
      Assert.True(flow.CanvasGroupDispatched);
      Assert.True(flow.CanvasCreatedDispatched);
      Assert.True(flow.CanvasRemovedDispatched);
      Assert.True(flow.CanvasNameChangedDispatched);
      Assert.True(flow.OutputsGroupDispatched);
      Assert.True(flow.RecordFileChangedDispatched);
      Assert.True(flow.InputActiveStateChangedDispatched);
      Assert.True(flow.InputShowStateChangedDispatched);
      Assert.True(flow.InputVolumeMetersDispatched);
      Assert.True(flow.SceneItemTransformChangedDispatched);
      Assert.True(flow.VendorEventDispatched);
    }
  }

  internal class ProtocolEventFlow : ITestFlow {
    private readonly TaskCompletionSource _canvasGroup = NewCompletionSource();
    private readonly TaskCompletionSource _canvasCreated = NewCompletionSource();
    private readonly TaskCompletionSource _canvasRemoved = NewCompletionSource();
    private readonly TaskCompletionSource _canvasNameChanged = NewCompletionSource();
    private readonly TaskCompletionSource _outputsGroup = NewCompletionSource();
    private readonly TaskCompletionSource _recordFileChanged = NewCompletionSource();
    private readonly TaskCompletionSource _inputActiveStateChanged = NewCompletionSource();
    private readonly TaskCompletionSource _inputShowStateChanged = NewCompletionSource();
    private readonly TaskCompletionSource _inputVolumeMeters = NewCompletionSource();
    private readonly TaskCompletionSource _sceneItemTransformChanged = NewCompletionSource();
    private readonly TaskCompletionSource _vendorEvent = NewCompletionSource();

    public bool CanvasGroupDispatched { get; private set; }
    public bool CanvasCreatedDispatched { get; private set; }
    public bool CanvasRemovedDispatched { get; private set; }
    public bool CanvasNameChangedDispatched { get; private set; }
    public bool OutputsGroupDispatched { get; private set; }
    public bool RecordFileChangedDispatched { get; private set; }
    public bool InputActiveStateChangedDispatched { get; private set; }
    public bool InputShowStateChangedDispatched { get; private set; }
    public bool InputVolumeMetersDispatched { get; private set; }
    public bool SceneItemTransformChangedDispatched { get; private set; }
    public bool VendorEventDispatched { get; private set; }

    public async Task RequestAsync(ObsClientSocket client) {
      client.CanvasesEvent += _ => Complete(_canvasGroup, () => CanvasGroupDispatched = true);
      client.CanvasCreated += ev => Complete(_canvasCreated, () => {
        Assert.Equal("Canvas", ev.CanvasName);
        Assert.Equal("canvas-uuid", ev.CanvasUuid);
        CanvasCreatedDispatched = true;
      });
      client.CanvasRemoved += ev => Complete(_canvasRemoved, () => {
        Assert.Equal("Canvas", ev.CanvasName);
        Assert.Equal("canvas-uuid", ev.CanvasUuid);
        CanvasRemovedDispatched = true;
      });
      client.CanvasNameChanged += ev => Complete(_canvasNameChanged, () => {
        Assert.Equal("Canvas", ev.OldCanvasName);
        Assert.Equal("Renamed canvas", ev.CanvasName);
        Assert.Equal("canvas-uuid", ev.CanvasUuid);
        CanvasNameChangedDispatched = true;
      });
      client.OutputsEvent += _ => Complete(_outputsGroup, () => OutputsGroupDispatched = true);
      client.RecordFileChanged += ev => Complete(_recordFileChanged, () => {
        Assert.Equal("recording-2.mp4", ev.NewOutputPath);
        RecordFileChangedDispatched = true;
      });
      client.InputActiveStateChanged += ev => Complete(_inputActiveStateChanged, () => {
        Assert.Equal("Input", ev.InputName);
        Assert.Equal("input-uuid", ev.InputUuid);
        Assert.True(ev.VideoActive);
        InputActiveStateChangedDispatched = true;
      });
      client.InputShowStateChanged += ev => Complete(_inputShowStateChanged, () => {
        Assert.Equal("Input", ev.InputName);
        Assert.Equal("input-uuid", ev.InputUuid);
        Assert.True(ev.VideoShowing);
        InputShowStateChangedDispatched = true;
      });
      client.InputVolumeMeters += ev => Complete(_inputVolumeMeters, () => {
        var input = Assert.Single(ev.Inputs);
        Assert.Equal("Input", input.Name);
        Assert.Equal("input-uuid", input.Uuid);
        Assert.Equal([0.25, 0.5, 0.75], Assert.Single(input.LevelsMul));
        InputVolumeMetersDispatched = true;
      });
      client.SceneItemTransformChanged += ev => Complete(_sceneItemTransformChanged, () => {
        Assert.Equal("Scene", ev.SceneName);
        Assert.Equal("scene-uuid", ev.SceneUuid);
        Assert.Equal(42, ev.SceneItemId);
        Assert.Equal(1280, ev.SceneItemTransform["sourceWidth"]!.Value.GetInt32());
        SceneItemTransformChangedDispatched = true;
      });
      client.VendorEvent += ev => Complete(_vendorEvent, () => {
        Assert.Equal("test-vendor", ev.VendorName);
        Assert.Equal("test-event", ev.VendorEventType);
        Assert.Equal("value", ev.EventData["key"]!.Value.GetString());
        VendorEventDispatched = true;
      });

      _ = await client.GetCanvasListAsync().ConfigureAwait(false);
      await Task.WhenAll(
        _canvasGroup.Task,
        _canvasCreated.Task,
        _canvasRemoved.Task,
        _canvasNameChanged.Task,
        _outputsGroup.Task,
        _recordFileChanged.Task,
        _inputActiveStateChanged.Task,
        _inputShowStateChanged.Task,
        _inputVolumeMeters.Task,
        _sceneItemTransformChanged.Task,
        _vendorEvent.Task
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string guid = (await session.ReceiveRequestAsync("GetCanvasList").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("GetCanvasList", guid, /*lang=json,strict*/ """{ "canvases": [] }""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "CanvasCreated",
    "eventIntent": 2048,
    "eventData": {
      "canvasName": "Canvas",
      "canvasUuid": "canvas-uuid"
    }
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "InputActiveStateChanged",
    "eventIntent": 131072,
    "eventData": {
      "inputName": "Input",
      "inputUuid": "input-uuid",
      "videoActive": true
    }
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "InputShowStateChanged",
    "eventIntent": 262144,
    "eventData": {
      "inputName": "Input",
      "inputUuid": "input-uuid",
      "videoShowing": true
    }
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "InputVolumeMeters",
    "eventIntent": 65536,
    "eventData": {
      "inputs": [
        {
          "inputName": "Input",
          "inputUuid": "input-uuid",
          "inputLevelsMul": [[0.25, 0.5, 0.75]]
        }
      ]
    }
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "SceneItemTransformChanged",
    "eventIntent": 524288,
    "eventData": {
      "sceneName": "Scene",
      "sceneUuid": "scene-uuid",
      "sceneItemId": 42,
      "sceneItemTransform": {
        "sourceWidth": 1280
      }
    }
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "VendorEvent",
    "eventIntent": 512,
    "eventData": {
      "vendorName": "test-vendor",
      "eventType": "test-event",
      "eventData": {
        "key": "value"
      }
    }
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "CanvasRemoved",
    "eventIntent": 2048,
    "eventData": {
      "canvasName": "Canvas",
      "canvasUuid": "canvas-uuid"
    }
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "CanvasNameChanged",
    "eventIntent": 2048,
    "eventData": {
      "oldCanvasName": "Canvas",
      "canvasName": "Renamed canvas",
      "canvasUuid": "canvas-uuid"
    }
  }
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "RecordFileChanged",
    "eventIntent": 64,
    "eventData": {
      "newOutputPath": "recording-2.mp4"
    }
  }
}
""").ConfigureAwait(false);
    }

    private static TaskCompletionSource NewCompletionSource() {
      return new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private static void Complete(TaskCompletionSource source, System.Action setDispatched) {
      try {
        setDispatched();
        _ = source.TrySetResult();
      }
      catch (System.Exception exception) {
        _ = source.TrySetException(exception);
      }
    }
  }
}
