using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputAudioSyncOffsetTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputAudioSyncOffsetFlow());
    }
  }

  internal class SetInputAudioSyncOffsetFlow : ITestFlow {
    public static readonly int InputAudioSyncOffset = 100;

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetInputAudioSyncOffsetAsync(
        inputName: CreateInputFlow.InputName,
        inputAudioSyncOffset: InputAudioSyncOffset
      ).ConfigureAwait(false);
      var changed = await ClientFlow.WaitEventAsync<InputAudioSyncOffsetChanged>(client).ConfigureAwait(false);
      Assert.Equal(CreateInputFlow.InputName, changed.InputName);
      Assert.Equal(InputAudioSyncOffset, changed.InputAudioSyncOffset, TestUtil.Epsilon);

    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "inputAudioSyncOffset": 100,
      "inputName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "SetInputAudioSyncOffset"
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
    "requestType": "SetInputAudioSyncOffset"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "inputAudioSyncOffset": 100,
      "inputName": "Browser source"
    },
    "eventIntent": 8,
    "eventType": "InputAudioSyncOffsetChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
