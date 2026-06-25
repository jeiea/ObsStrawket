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
      await SpecTester.TestAsync(new SetCurrentSceneTransitionFlow());
    }
  }

  internal class SetCurrentSceneTransitionFlow : ITestFlow {
    // Default transitions only; a fresh OBS has no user-added ones like Swipe.
    public static string TransitionName => "Fade";

    private static readonly string _spareTransition = "Cut";

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetCurrentSceneTransitionAsync(transitionName: _spareTransition).ConfigureAwait(false);
      _ = ClientFlow.DrainEvents(client);

      _ = await client.SetCurrentSceneTransitionAsync(transitionName: TransitionName).ConfigureAwait(false);
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
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "transitionName": "Cut"
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

      guid = await session.ReceiveAsync(/*lang=json,strict*/ """
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
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "transitionName": "Fade"
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
