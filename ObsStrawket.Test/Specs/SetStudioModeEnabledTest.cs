using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetStudioModeEnabledTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetStudioModeEnabledFlow());
    }
  }

  internal class SetStudioModeEnabledFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetStudioModeEnabledAsync(studioModeEnabled: false).ConfigureAwait(false);
      _ = ClientFlow.DrainEvents(client);
      _ = await client.SetStudioModeEnabledAsync(studioModeEnabled: true).ConfigureAwait(false);
      _ = await ClientFlow.WaitEventAsync<StudioModeStateChanged>(
        client,
        static ev => ev.StudioModeEnabled
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "studioModeEnabled": false
    },
    "requestId": "{guid}",
    "requestType": "SetStudioModeEnabled"
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
    "requestType": "SetStudioModeEnabled"
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "studioModeEnabled": true
    },
    "requestId": "{guid}",
    "requestType": "SetStudioModeEnabled"
  },
  "op": 6
}
""").ConfigureAwait(false);

      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "studioModeEnabled": false
    },
    "eventIntent": 1024,
    "eventType": "StudioModeStateChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "studioModeEnabled": true
    },
    "eventIntent": 1024,
    "eventType": "StudioModeStateChanged"
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
    "requestType": "SetStudioModeEnabled"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
