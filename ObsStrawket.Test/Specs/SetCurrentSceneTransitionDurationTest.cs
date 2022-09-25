using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentSceneTransitionDurationTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentSceneTransitionDurationFlow()).ConfigureAwait(false);
    }
  }

  class SetCurrentSceneTransitionDurationFlow : ITestFlow {
    public static int TransitionDuration => 300;

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetCurrentSceneTransitionDurationAsync(transitionDuration: 200).ConfigureAwait(false);

      await client.SetCurrentSceneTransitionDurationAsync(transitionDuration: TransitionDuration).ConfigureAwait(false);

      using var cts = new CancellationTokenSource();
      cts.CancelAfter(2000);
      await foreach (var changed in client.Events.ReadAllAsync(cts.Token).OfType<CurrentSceneTransitionDurationChanged>().ConfigureAwait(false)) {
        if (changed.TransitionDuration == 200) {
          continue;
        }
        Assert.Equal(TransitionDuration, changed.TransitionDuration);
        break;
      }
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""transitionDuration"": 200
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetCurrentSceneTransitionDuration""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""transitionDuration"": 200
    },
    ""eventIntent"": 16,
    ""eventType"": ""CurrentSceneTransitionDurationChanged""
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
    ""requestType"": ""SetCurrentSceneTransitionDuration""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""transitionDuration"": 300
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetCurrentSceneTransitionDuration""
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
    ""requestType"": ""SetCurrentSceneTransitionDuration""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""transitionDuration"": 300
    },
    ""eventIntent"": 16,
    ""eventType"": ""CurrentSceneTransitionDurationChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
