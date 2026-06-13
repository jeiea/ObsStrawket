using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
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
    }
  }

  class ProtocolEventFlow : ITestFlow {
    private readonly TaskCompletionSource _canvasGroup = NewCompletionSource();
    private readonly TaskCompletionSource _canvasCreated = NewCompletionSource();
    private readonly TaskCompletionSource _canvasRemoved = NewCompletionSource();
    private readonly TaskCompletionSource _canvasNameChanged = NewCompletionSource();
    private readonly TaskCompletionSource _outputsGroup = NewCompletionSource();
    private readonly TaskCompletionSource _recordFileChanged = NewCompletionSource();

    public bool CanvasGroupDispatched { get; private set; }
    public bool CanvasCreatedDispatched { get; private set; }
    public bool CanvasRemovedDispatched { get; private set; }
    public bool CanvasNameChangedDispatched { get; private set; }
    public bool OutputsGroupDispatched { get; private set; }
    public bool RecordFileChangedDispatched { get; private set; }

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

      await client.GetCanvasListAsync().ConfigureAwait(false);
      await Task.WhenAll(
        _canvasGroup.Task,
        _canvasCreated.Task,
        _canvasRemoved.Task,
        _canvasNameChanged.Task,
        _outputsGroup.Task,
        _recordFileChanged.Task
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string guid = (await session.ReceiveRequestAsync("GetCanvasList").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("GetCanvasList", guid, """{ "canvases": [] }""").ConfigureAwait(false);
      await session.SendAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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
        source.TrySetResult();
      }
      catch (System.Exception exception) {
        source.TrySetException(exception);
      }
    }
  }
}
