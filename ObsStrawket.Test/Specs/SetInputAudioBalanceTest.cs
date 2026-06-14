using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputAudioBalanceTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputAudioBalanceFlow());
    }
  }

  internal class SetInputAudioBalanceFlow : ITestFlow {
    public static readonly double InputAudioBalance = 0.6;

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetInputAudioBalanceAsync(
        inputName: CreateInputFlow.InputName,
        inputAudioBalance: InputAudioBalance
      ).ConfigureAwait(false);
      var changed = await ClientFlow.WaitEventAsync<InputAudioBalanceChanged>(client).ConfigureAwait(false);
      Assert.Equal(CreateInputFlow.InputName, changed.InputName);
      Assert.Equal(InputAudioBalance, changed.InputAudioBalance, TestUtil.Epsilon);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "inputAudioBalance": 0.6,
      "inputName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "SetInputAudioBalance"
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
    "requestType": "SetInputAudioBalance"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "inputAudioBalance": 0.6000000238418579,
      "inputName": "Browser source"
    },
    "eventIntent": 8,
    "eventType": "InputAudioBalanceChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
