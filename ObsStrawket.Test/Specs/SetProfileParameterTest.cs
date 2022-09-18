using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetProfileParameterTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetProfileParameterFlow()).ConfigureAwait(false);
    }
  }

  class SetProfileParameterFlow : ITestFlow {
    public static string ParameterValue => "test parameter value";

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetProfileParameterAsync(parameterCategory: "test parameter category", parameterName: "test parameter name", parameterValue: ParameterValue).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""parameterCategory"": ""test parameter category"",
      ""parameterName"": ""test parameter name"",
      ""parameterValue"": ""test parameter value""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetProfileParameter""
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
    ""requestType"": ""SetProfileParameter""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
