using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentSceneTransitionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentSceneTransitionFlow()).ConfigureAwait(false);
    }
  }

  class SetCurrentSceneTransitionFlow : ITestFlow {
    public static string TransitionName => "Swipe";

    private static readonly string _spareTransition = "Fade";

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetCurrentSceneTransitionAsync(transitionName: _spareTransition).ConfigureAwait(false);
      ClientFlow.DrainEvents(client);

      await client.SetCurrentSceneTransitionAsync(transitionName: TransitionName).ConfigureAwait(false);
      using var cts = new CancellationTokenSource();
      cts.CancelAfter(2000);
      await foreach (var changed in client.Events.ReadAllAsync(cts.Token).OfType<CurrentSceneTransitionChanged>().ConfigureAwait(false)) {
        if (changed.TransitionName == _spareTransition) {
          continue;
        }
        Assert.Equal(TransitionName, changed.TransitionName);
        break;
      }
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "transitionName": "Fade"
    },
    "requestId": "{guid}",
    "requestType": "SetCurrentSceneTransition"
  },
  "op": 6
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
    "requestType": "SetCurrentSceneTransition"
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "transitionName": "Swipe"
    },
    "requestId": "{guid}",
    "requestType": "SetCurrentSceneTransition"
  },
  "op": 6
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
    "requestType": "SetCurrentSceneTransition"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "transitionName": "Swipe"
    },
    "eventIntent": 16,
    "eventType": "CurrentSceneTransitionChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
