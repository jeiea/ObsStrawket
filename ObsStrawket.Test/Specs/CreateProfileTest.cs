using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateProfileTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateProfileFlow()).ConfigureAwait(false);
    }
  }

  class CreateProfileFlow : ITestFlow {
    public static string NewProfileName { get => "test profile name"; }

    public async Task RequestAsync(ObsClientSocket client) {
      try {
        await CreateProfileAsync(client).ConfigureAwait(false);
      }
      catch (FailureResponseException failure)
      when (failure.Response.RequestStatus.Code == RequestStatus.ResourceAlreadyExists) {
        await client.RemoveProfileAsync(NewProfileName).ConfigureAwait(false);
        await client.Events.ReadAllAsync()
          .Where((x) => x is CurrentProfileChanging || x is ProfileListChanged || x is CurrentProfileChanged)
          .Take(3).ToListAsync().ConfigureAwait(false);
        await CreateProfileAsync(client).ConfigureAwait(false);
      }
    }

    private static async Task CreateProfileAsync(ObsClientSocket client) {
      await client.CreateProfileAsync(profileName: NewProfileName).ConfigureAwait(false);

      var currentProfileChanging = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.NotEqual("", (currentProfileChanging as CurrentProfileChanging)!.ProfileName);
      var profileListChanged = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Contains(NewProfileName, (profileListChanged as ProfileListChanged)!.Profiles);
      var currentProfileChanged = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(NewProfileName, (currentProfileChanged as CurrentProfileChanged)!.ProfileName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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
      await session.SendAsync("""
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
