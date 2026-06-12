using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetOutputSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetOutputSettingsFlow());
    }
  }

  class SetOutputSettingsFlow : ITestFlow {
    public static string OutputName => GetOutputListFlow.OutputName;
    public static string PropName => "test_prop";
    public static string PropValue => "unused";

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetOutputSettingsAsync(
        outputName: OutputName,
        outputSettings: new Dictionary<string, JsonElement?> {
          { PropName, PropValue.ToJsonElement() }
        }
      ).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "outputName": "Replay Buffer",
      "outputSettings": {
        "test_prop": "unused"
      }
    },
    "requestId": "{guid}",
    "requestType": "SetOutputSettings"
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
    "requestType": "SetOutputSettings"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
