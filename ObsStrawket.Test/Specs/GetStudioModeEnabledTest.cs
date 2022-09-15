using ObsStrawket.Test.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetStudioModeEnabledTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetStudioModeEnabledFlow()).ConfigureAwait(false);
    }
  }

  class GetStudioModeEnabledFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var modeEnabled = await client.GetStudioModeEnabledAsync().ConfigureAwait(false);
      Assert.True(modeEnabled.StudioModeEnabled);
      await client.SetStudioModeEnabledAsync(false).ConfigureAwait(false);
      ClientFlow.DrainEvents(client);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetStudioModeEnabled""
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
    ""requestType"": ""GetStudioModeEnabled"",
    ""responseData"": {
      ""studioModeEnabled"": true
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);


      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""studioModeEnabled"": false
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetStudioModeEnabled""
  },
  ""op"": 6
}").ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneName"": ""test scene""
    },
    ""eventIntent"": 4,
    ""eventType"": ""CurrentProgramSceneChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""studioModeEnabled"": false
    },
    ""eventIntent"": 1024,
    ""eventType"": ""StudioModeStateChanged""
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
    ""requestType"": ""SetStudioModeEnabled""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
