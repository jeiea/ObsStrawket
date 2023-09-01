using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
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
    public static List<(string Category, string Name, string Value, string Default)> AppliedParameters = new() {
      ("Output", "Mode", "Advanced", "Simple"),
      ("AdvOut", "RecRB", "true", "false"),
      ("AdvOut", "RecEncoder", "obs_x264", "none"),
    };

    public async Task RequestAsync(ObsClientSocket client) {
      foreach (var (category, name, value, _) in AppliedParameters) {
        await client.SetProfileParameterAsync(
          parameterCategory: category,
          parameterName: name,
          parameterValue: value
        ).ConfigureAwait(false);
      }
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "parameterCategory": "Output",
      "parameterName": "Mode",
      "parameterValue": "Advanced"
    },
    "requestId": "{guid}",
    "requestType": "SetProfileParameter"
  },
  "op": 6
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
    "requestType": "SetProfileParameter"
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "parameterCategory": "AdvOut",
      "parameterName": "RecRB",
      "parameterValue": "true"
    },
    "requestId": "{guid}",
    "requestType": "SetProfileParameter"
  },
  "op": 6
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
    "requestType": "SetProfileParameter"
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "parameterCategory": "AdvOut",
      "parameterName": "RecEncoder",
      "parameterValue": "obs_x264"
    },
    "requestId": "{guid}",
    "requestType": "SetProfileParameter"
  },
  "op": 6
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
    "requestType": "SetProfileParameter"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
