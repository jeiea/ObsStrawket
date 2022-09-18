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
      var response = await client.GetProfileParameterAsync(parameterCategory: "test parameter category", parameterName: "test parameter name").ConfigureAwait(false);
      Assert.Equal(SetProfileParameterFlow.ParameterValue, response.ParameterValue);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""parameterCategory"": ""test parameter category"",
      ""parameterName"": ""test parameter name""
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
      ""defaultParameterValue"": null,
      ""parameterValue"": ""test parameter value""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
