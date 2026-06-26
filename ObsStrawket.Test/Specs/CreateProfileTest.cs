using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateProfileTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateProfileFlow());
    }
  }

  internal class CreateProfileFlow : ITestFlow {
    private readonly Func<Func<Task>, Task> _confirmRemoveProfileAsync;

    public CreateProfileFlow(Func<Func<Task>, Task>? confirmRemoveProfileAsync = null) {
      _confirmRemoveProfileAsync = confirmRemoveProfileAsync ?? (static request => request());
    }

    public static string NewProfileName => "test profile name";

    public async Task RequestAsync(ObsClientSocket client) {
      try {
        await CreateProfileAsync(client).ConfigureAwait(false);
      }
      catch (ObsRequestException failure)
      when (failure.Response.RequestStatus.Code == RequestStatus.ResourceAlreadyExists) {
        await RemoveProfileFlow.RemoveProfileAsync(
          client,
          NewProfileName,
          _confirmRemoveProfileAsync
        ).ConfigureAwait(false);
        await CreateProfileAsync(client).ConfigureAwait(false);
      }
    }

    private static async Task CreateProfileAsync(ObsClientSocket client) {
      _ = await client.CreateProfileAsync(profileName: NewProfileName).ConfigureAwait(false);

      // Event makeup and order differ across OBS versions; read until the
      // guaranteed CurrentProfileChanged and verify whatever arrived with it.
      var events = new List<IObsEvent>();
      do {
        events.Add(await client.Events.ReadAsync().ConfigureAwait(false));
      } while (events[^1] is not CurrentProfileChanged);

      Assert.Equal(NewProfileName, ((CurrentProfileChanged)events[^1]).ProfileName);
      if (events.OfType<ProfileListChanged>().SingleOrDefault() is { } listChanged) {
        Assert.Contains(NewProfileName, listChanged.Profiles);
      }
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "profileName": "test profile name"
    },
    "requestId": "{guid}",
    "requestType": "CreateProfile"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "profileName": "default"
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
      "profiles": [
        "default",
        "test profile name"
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
    "requestType": "CreateProfile"
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
    "eventType": "CurrentProfileChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
