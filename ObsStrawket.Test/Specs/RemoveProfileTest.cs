using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class RemoveProfileTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new RemoveProfileFlow()).ConfigureAwait(false);
    }
  }

  class RemoveProfileFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.RemoveProfileAsync(profileName: CreateProfileFlow.NewProfileName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "profileName": "test profile name"
    },
    "requestId": "{guid}",
    "requestType": "RemoveProfile"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "profileName": "test profile name"
    },
    "eventIntent": 2,
    "eventType": "CurrentProfileChanging"
  },
  "op": 5
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "profiles": [
        "default",
      ]
    },
    "eventIntent": 2,
    "eventType": "ProfileListChanged"
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
    "requestType": "RemoveProfile"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "profileName": "default"
    },
    "eventIntent": 2,
    "eventType": "CurrentProfileChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
