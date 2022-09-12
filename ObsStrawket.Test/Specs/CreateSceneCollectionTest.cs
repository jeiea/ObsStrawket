using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class CreateSceneCollectionTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new CreateSceneCollectionFlow()).ConfigureAwait(false);
    }
  }

  class CreateSceneCollectionFlow : ITestFlow {
    public static string NewSceneCollection = "test scene collection";
    public async Task RequestAsync(ObsClientSocket client) {
      await client.CreateSceneCollectionAsync(sceneCollectionName: NewSceneCollection).ConfigureAwait(false);
      var list = new List<IEvent>();
      while (client.Events.TryPeek(out _)) {
        list.Add(await client.Events.ReadAsync().ConfigureAwait(false));
      }
      Assert.Contains(list, (x) => x is CurrentSceneCollectionChanging);
      Assert.Contains(list, (x) => x is SceneCreated);
      Assert.Contains(list, (x) => x is SceneCollectionListChanged);
      Assert.Contains(list, (x) => x is CurrentSceneCollectionChanged);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""sceneCollectionName"": ""test scene collection""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""CreateSceneCollection""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneCollectionName"": ""default""
    },
    ""eventIntent"": 2,
    ""eventType"": ""CurrentSceneCollectionChanging""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""inputName"": ""Video Source""
    },
    ""eventIntent"": 8,
    ""eventType"": ""InputRemoved""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""isGroup"": false,
      ""sceneName"": ""Scene""
    },
    ""eventIntent"": 4,
    ""eventType"": ""SceneCreated""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneCollections"": [
        ""default"",
        ""test scene collection""
      ]
    },
    ""eventIntent"": 2,
    ""eventType"": ""SceneCollectionListChanged""
  },
  ""op"": 5
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""eventData"": {
      ""sceneCollectionName"": ""test scene collection""
    },
    ""eventIntent"": 2,
    ""eventType"": ""CurrentSceneCollectionChanged""
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
    ""requestType"": ""CreateSceneCollection""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
