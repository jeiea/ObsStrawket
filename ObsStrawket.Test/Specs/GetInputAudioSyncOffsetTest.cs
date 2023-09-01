using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetInputAudioSyncOffsetTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetInputAudioSyncOffsetFlow()).ConfigureAwait(false);
    }
  }

  class GetInputAudioSyncOffsetFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetInputAudioSyncOffsetAsync(inputName: CreateInputFlow.InputName).ConfigureAwait(false);
      Assert.Equal(SetInputAudioSyncOffsetFlow.InputAudioSyncOffset, response.InputAudioSyncOffset);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "inputName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "GetInputAudioSyncOffset"
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
    "requestType": "GetInputAudioSyncOffset",
    "responseData": {
      "inputAudioSyncOffset": 100
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
