using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetCurrentProfileTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetCurrentProfileFlow()).ConfigureAwait(false);
    }
  }

  class SetCurrentProfileFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      // SetProfileParameter enables replay buffer but it doesn't take effect immediately.
      // So switch to other profile then testing profile here.
      var profile = await GetProfileListFlow.GetProfileList(client).ConfigureAwait(false);
      string anotherProfile = profile.Profiles.Find(x => x != CreateProfileFlow.NewProfileName)!;
      var response = await client.SetCurrentProfileAsync(profileName: anotherProfile).ConfigureAwait(false);
      response = await client.SetCurrentProfileAsync(profileName: CreateProfileFlow.NewProfileName).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      await new GetProfileListFlow().RespondAsync(session).ConfigureAwait(false);

      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""profileName"": ""default""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetCurrentProfile""
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
    ""requestType"": ""SetCurrentProfile""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""profileName"": ""test profile name""
    },
    ""eventIntent"": 2,
    ""eventType"": ""CurrentProfileChanging""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""profileName"": ""default""
    },
    ""eventIntent"": 2,
    ""eventType"": ""CurrentProfileChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""profileName"": ""test profile name""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetCurrentProfile""
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
    ""requestType"": ""SetCurrentProfile""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""profileName"": ""default""
    },
    ""eventIntent"": 2,
    ""eventType"": ""CurrentProfileChanging""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""profileName"": ""test profile name""
    },
    ""eventIntent"": 2,
    ""eventType"": ""CurrentProfileChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
