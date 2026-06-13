using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class TriggerMediaInputActionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new TriggerMediaInputActionFlow());
    }
  }

  class TriggerMediaInputActionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
#pragma warning disable CS0612 // Type or member is obsolete
      await client.TriggerMediaInputActionAsync(
        inputName: CreateInputFlow.MediaInputName,
        mediaAction: MediaInputAction.Stop
      ).ConfigureAwait(false);

      var (triggered, playbackEnded) = await ClientFlow.WaitEventsAsync<MediaInputActionTriggered, MediaInputPlaybackEnded>(
        client,
        e => e.MediaAction == MediaInputAction.Stop,
        e => e.InputName == CreateInputFlow.MediaInputName).ConfigureAwait(false);
      Assert.Equal(MediaInputAction.Stop, triggered.MediaAction);
      Assert.Equal(CreateInputFlow.MediaInputName, playbackEnded.InputName);

      await client.TriggerMediaInputActionAsync(
        inputName: CreateInputFlow.MediaInputName,
        mediaAction: MediaInputAction.Play
      ).ConfigureAwait(false);

      await ClientFlow.WaitEventsAsync<MediaInputActionTriggered, MediaInputPlaybackStarted>(
        client,
        e => e.MediaAction == MediaInputAction.Play,
        e => e.InputName == CreateInputFlow.MediaInputName).ConfigureAwait(false);
#pragma warning restore CS0612 // Type or member is obsolete

      await client.RemoveInputAsync(CreateInputFlow.MediaInputName).ConfigureAwait(false);
      await ClientFlow.WaitEventAsync<InputRemoved>(client).ConfigureAwait(false);
      await Task.Delay(100).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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

      guid = await session.ReceiveAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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

      guid = await session.ReceiveAsync("""
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
      await session.SendAsync("""
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
