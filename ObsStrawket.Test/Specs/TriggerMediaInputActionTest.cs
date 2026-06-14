using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class TriggerMediaInputActionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new TriggerMediaInputActionFlow());
    }
  }

  internal class TriggerMediaInputActionFlow : ITestFlow {
    private readonly bool _expectPlaybackEvents;

    public TriggerMediaInputActionFlow(bool expectPlaybackEvents = true) {
      _expectPlaybackEvents = expectPlaybackEvents;
    }

    public async Task RequestAsync(ObsClientSocket client) {
#pragma warning disable CS0612 // Type or member is obsolete
      _ = await client.TriggerMediaInputActionAsync(
        inputName: CreateInputFlow.MediaInputName,
        mediaAction: MediaInputAction.Stop
      ).ConfigureAwait(false);

      var triggered = await ClientFlow.WaitEventAsync<MediaInputActionTriggered>(client).ConfigureAwait(false);
      Assert.Equal(MediaInputAction.Stop, triggered.MediaAction);
      if (_expectPlaybackEvents) {
        var playbackEnded = await ClientFlow.WaitEventAsync<MediaInputPlaybackEnded>(client).ConfigureAwait(false);
        Assert.Equal(CreateInputFlow.MediaInputName, playbackEnded.InputName);
      }

      _ = await client.TriggerMediaInputActionAsync(
        inputName: CreateInputFlow.MediaInputName,
        mediaAction: MediaInputAction.Play
      ).ConfigureAwait(false);

      triggered = await ClientFlow.WaitEventAsync<MediaInputActionTriggered>(client).ConfigureAwait(false);
      Assert.Equal(MediaInputAction.Play, triggered.MediaAction);
      if (_expectPlaybackEvents) {
        var playbackStarted = await ClientFlow.WaitEventAsync<MediaInputPlaybackStarted>(client).ConfigureAwait(false);
        Assert.Equal(CreateInputFlow.MediaInputName, playbackStarted.InputName);
      }
#pragma warning restore CS0612 // Type or member is obsolete

      _ = await client.RemoveInputAsync(CreateInputFlow.MediaInputName).ConfigureAwait(false);
      _ = await ClientFlow.WaitEventAsync<InputRemoved>(client).ConfigureAwait(false);
      await Task.Delay(100).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "inputName": "Media source",
      "mediaAction": "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_STOP"
    },
    "requestId": "{guid}",
    "requestType": "TriggerMediaInputAction"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "inputName": "Media source",
      "mediaAction": "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_STOP"
    },
    "eventIntent": 256,
    "eventType": "MediaInputActionTriggered"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "TriggerMediaInputAction"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "inputName": "Media source"
    },
    "eventIntent": 256,
    "eventType": "MediaInputPlaybackEnded"
  },
  "op": 5
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "inputName": "Media source",
      "mediaAction": "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY"
    },
    "requestId": "{guid}",
    "requestType": "TriggerMediaInputAction"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "inputName": "Media source",
      "mediaAction": "OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY"
    },
    "eventIntent": 256,
    "eventType": "MediaInputActionTriggered"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "inputName": "Media source"
    },
    "eventIntent": 256,
    "eventType": "MediaInputPlaybackStarted"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "TriggerMediaInputAction"
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "inputName": "Media source"
    },
    "requestId": "{guid}",
    "requestType": "RemoveInput"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "inputName": "Media source"
    },
    "eventIntent": 8,
    "eventType": "InputRemoved"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "RemoveInput"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
