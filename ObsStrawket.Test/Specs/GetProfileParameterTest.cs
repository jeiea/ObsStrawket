using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetProfileParameterTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetProfileParameterFlow()).ConfigureAwait(false);
    }
  }

  class GetProfileParameterFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      foreach (var (category, name, value, def) in SetProfileParameterFlow.AppliedParameters) {
        var response = await client.GetProfileParameterAsync(
          parameterCategory: category,
          parameterName: name
        ).ConfigureAwait(false);
        Assert.Equal(value, response.ParameterValue);
        Assert.Equal(def, response.DefaultParameterValue);
      }
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""parameterCategory"": ""Output"",
      ""parameterName"": ""Mode""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetProfileParameter""
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
    ""requestType"": ""GetProfileParameter"",
    ""responseData"": {
      ""defaultParameterValue"": ""Simple"",
      ""parameterValue"": ""Advanced""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""parameterCategory"": ""AdvOut"",
      ""parameterName"": ""RecRB""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetProfileParameter""
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
    ""requestType"": ""GetProfileParameter"",
    ""responseData"": {
      ""defaultParameterValue"": ""false"",
      ""parameterValue"": ""true""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);

      guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""parameterCategory"": ""AdvOut"",
      ""parameterName"": ""RecEncoder""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetProfileParameter""
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
    ""requestType"": ""GetProfileParameter"",
    ""responseData"": {
      ""defaultParameterValue"": ""none"",
      ""parameterValue"": ""obs_x264""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
