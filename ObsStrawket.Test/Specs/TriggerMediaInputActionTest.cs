using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class TriggerMediaInputActionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new TriggerMediaInputActionFlow()).ConfigureAwait(false);
    }
  }

  class TriggerMediaInputActionFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
#pragma warning disable CS0612 // Type or member is obsolete
      await client.TriggerMediaInputActionAsync(
        inputName: CreateInputFlow.MediaInputName,
        mediaAction: MediaInputAction.Stop
      ).ConfigureAwait(false);

      var triggered = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(MediaInputAction.Stop, (triggered as MediaInputActionTriggered)!.MediaAction);
      var ended = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateInputFlow.MediaInputName, (ended as MediaInputPlaybackEnded)!.InputName);

      await client.TriggerMediaInputActionAsync(
        inputName: CreateInputFlow.MediaInputName,
        mediaAction: MediaInputAction.Play
      ).ConfigureAwait(false);

      var events = await client.Events.ReadAllAsync().Take(2).ToListAsync().ConfigureAwait(false);
      Assert.Contains(events, x => x is MediaInputActionTriggered triggered && triggered.MediaAction == MediaInputAction.Play);
      Assert.Contains(events, x => x is MediaInputPlaybackStarted started && started.InputName == CreateInputFlow.MediaInputName);
#pragma warning restore CS0612 // Type or member is obsolete

      await client.RemoveInputAsync(CreateInputFlow.MediaInputName).ConfigureAwait(false);
      await client.Events.ReadAllAsync().OfType<InputRemoved>().FirstAsync().ConfigureAwait(false);
      await Task.Delay(100).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Media source"",
      ""mediaAction"": ""OBS_WEBSOCKET_MEDIA_INPUT_ACTION_STOP""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""TriggerMediaInputAction""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Media source"",
      ""mediaAction"": ""OBS_WEBSOCKET_MEDIA_INPUT_ACTION_STOP""
    },
    ""eventIntent"": 256,
    ""eventType"": ""MediaInputActionTriggered""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""TriggerMediaInputAction""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Media source""
    },
    ""eventIntent"": 256,
    ""eventType"": ""MediaInputPlaybackEnded""
  },
  ""op"": 5
}").ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Media source"",
      ""mediaAction"": ""OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""TriggerMediaInputAction""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Media source"",
      ""mediaAction"": ""OBS_WEBSOCKET_MEDIA_INPUT_ACTION_PLAY""
    },
    ""eventIntent"": 256,
    ""eventType"": ""MediaInputActionTriggered""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Media source""
    },
    ""eventIntent"": 256,
    ""eventType"": ""MediaInputPlaybackStarted""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""TriggerMediaInputAction""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Media source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""RemoveInput""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Media source""
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputRemoved""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""RemoveInput""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
