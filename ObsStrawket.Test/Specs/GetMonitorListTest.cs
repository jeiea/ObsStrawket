using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetMonitorListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetMonitorListFlow()).ConfigureAwait(false);
    }
  }

  class GetMonitorListFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetMonitorListAsync().ConfigureAwait(false);
      Assert.NotInRange((ushort)response.Monitors[0]["monitorWidth"]!, ushort.MinValue, 0);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetMonitorList""
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
    ""requestType"": ""GetMonitorList"",
    ""responseData"": {
      ""monitors"": [
        {
          ""monitorHeight"": 1440,
          ""monitorIndex"": 0,
          ""monitorName"": ""\\\\.\\DISPLAY1(0)"",
          ""monitorPositionX"": 0,
          ""monitorPositionY"": 0,
          ""monitorWidth"": 2560
        }
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
