using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class RemoveProfileTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new RemoveProfileFlow());
    }
  }

  internal class RemoveProfileFlow : ITestFlow {
    private readonly Func<Func<Task>, Task> _confirmRemoveProfileAsync;

    public RemoveProfileFlow(Func<Func<Task>, Task>? confirmRemoveProfileAsync = null) {
      _confirmRemoveProfileAsync = confirmRemoveProfileAsync ?? (static request => request());
    }

    public async Task RequestAsync(ObsClientSocket client) {
      await RemoveProfileAsync(
        client,
        CreateProfileFlow.NewProfileName,
        _confirmRemoveProfileAsync
      ).ConfigureAwait(false);
    }

    public static async Task RemoveProfileAsync(
        ObsClientSocket client,
        string profileName,
        Func<Func<Task>, Task>? confirmRemoveProfileAsync = null) {
      await SwitchAwayFromProfileAsync(client, profileName).ConfigureAwait(false);

      await (confirmRemoveProfileAsync ?? (static request => request()))(async () => {
        _ = await client.RemoveProfileAsync(profileName: profileName).ConfigureAwait(false);
      }).ConfigureAwait(false);

      await WaitUntilRemovedAsync(client, profileName).ConfigureAwait(false);
    }

    private static async Task SwitchAwayFromProfileAsync(ObsClientSocket client, string profileName) {
      var profile = await client.GetProfileListAsync().ConfigureAwait(false);
      if (profile.CurrentProfileName != profileName) {
        return;
      }

      if (profile.Profiles.Find(name => name != profileName) is not string nextProfileName) {
        throw new InvalidOperationException($"Cannot remove the only OBS profile: {profileName}");
      }

      _ = await client.SetCurrentProfileAsync(profileName: nextProfileName).ConfigureAwait(false);
      _ = await ClientFlow.WaitEventsAsync(
        client,
        ev => ev is CurrentProfileChanged changed && changed.ProfileName == nextProfileName
      ).ConfigureAwait(false);
    }

    private static async Task WaitUntilRemovedAsync(ObsClientSocket client, string profileName) {
      _ = await ClientFlow.WaitEventsAsync(
        client,
        ev => ev is ProfileListChanged changed && !changed.Profiles.Contains(profileName)
      ).ConfigureAwait(false);

      var profile = await client.GetProfileListAsync().ConfigureAwait(false);
      Assert.DoesNotContain(profileName, profile.Profiles);
    }

    public async Task RespondAsync(MockServerSession session) {
      await new GetProfileListFlow().RespondAsync(session).ConfigureAwait(false);

      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "profileName": "default"
    },
    "requestId": "{guid}",
    "requestType": "SetCurrentProfile"
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
    "requestType": "SetCurrentProfile"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
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
      await session.SendAsync(/*lang=json,strict*/ """
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

      guid = await session.ReceiveAsync(/*lang=json,strict*/ """
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
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "profiles": [
        "default"
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

      guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetProfileList"
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
    "requestType": "GetProfileList",
    "responseData": {
      "currentProfileName": "default",
      "profiles": [
        "default"
      ]
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
