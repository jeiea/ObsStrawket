using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputPropertiesListPropertyItemsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputPropertiesListPropertyItemsFlow()).ConfigureAwait(false);
    }
  }

  class GetInputPropertiesListPropertyItemsFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputPropertiesListPropertyItemsAsync(
        inputName: CreateInputFlow.InputName,
        propertyName: "webpage_control_level"
      ).ConfigureAwait(false);
      Assert.Equal("No access to OBS", response.PropertyItems[0]["itemName"]);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Browser source"",
      ""propertyName"": ""webpage_control_level""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetInputPropertiesListPropertyItems""
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
    ""requestType"": ""GetInputPropertiesListPropertyItems"",
    ""responseData"": {
      ""propertyItems"": [
        {
          ""itemEnabled"": true,
          ""itemName"": ""No access to OBS"",
          ""itemValue"": 0
        },
        {
          ""itemEnabled"": true,
          ""itemName"": ""Read access to OBS status information"",
          ""itemValue"": 1
        },
        {
          ""itemEnabled"": true,
          ""itemName"": ""Read access to user information (current Scene Collection, Transitions)"",
          ""itemValue"": 2
        },
        {
          ""itemEnabled"": true,
          ""itemName"": ""Basic access to OBS (Save replay buffer, etc.)"",
          ""itemValue"": 3
        },
        {
          ""itemEnabled"": true,
          ""itemName"": ""Advanced access to OBS (Change scenes, Start/Stop replay buffer, etc.)"",
          ""itemValue"": 4
        },
        {
          ""itemEnabled"": true,
          ""itemName"": ""Full access to OBS (Start/Stop streaming without warning, etc.)"",
          ""itemValue"": 5
        }
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
