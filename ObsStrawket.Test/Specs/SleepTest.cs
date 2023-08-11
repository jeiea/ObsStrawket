using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SleepTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SleepFlow()).ConfigureAwait(false);
    }
  }

  class SleepFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.RequestAsync(new RequestBatch() {
        Requests = {
          new Sleep() { SleepMillis = 100 },
          new GetStudioModeEnabled(),
        }
      }).ConfigureAwait(false);

      Assert.True((response.Results[1] as GetStudioModeEnabledResponse)!.StudioModeEnabled);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""executionType"": 0,
    ""haltOnFailure"": null,
    ""requestId"": ""{guid}"",
    ""requests"": [
      {
        ""requestData"": {
          ""sleepFrames"": null,
          ""sleepMillis"": 100
        },
        ""requestId"": """",
        ""requestType"": ""Sleep""
      },
      {
        ""requestId"": """",
        ""requestType"": ""GetStudioModeEnabled""
      }
    ]
  },
  ""op"": 8
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""results"": [
      {
        ""requestId"": """",
        ""requestStatus"": {
          ""code"": 100,
          ""result"": true
        },
        ""requestType"": ""Sleep""
      },
      {
        ""requestId"": """",
        ""requestStatus"": {
          ""code"": 100,
          ""result"": true
        },
        ""requestType"": ""GetStudioModeEnabled"",
        ""responseData"": {
          ""studioModeEnabled"": true
        }
      }
    ]
  },
  ""op"": 9
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
