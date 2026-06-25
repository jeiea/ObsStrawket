using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputAudioTracksTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputAudioTracksFlow());
    }
  }

  internal class SetInputAudioTracksFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetInputAudioTracksAsync(
        inputName: CreateInputFlow.InputName,
        inputAudioTracks: new Dictionary<string, bool> {
          { "2", false },
        }
      ).ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<InputAudioTracksChanged>(client).ConfigureAwait(false);
      Assert.True(changed.InputAudioTracks["1"]);
      Assert.False(changed.InputAudioTracks["2"]);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "inputAudioTracks": {
        "2": false
      },
      "inputName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "SetInputAudioTracks"
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
    "requestType": "SetInputAudioTracks"
  },
  "op": 7
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "inputAudioTracks": {
        "1": true,
        "2": false,
        "3": true,
        "4": true,
        "5": true,
        "6": true
      },
      "inputName": "Browser source"
    },
    "eventIntent": 8,
    "eventType": "InputAudioTracksChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);
    }
  }
}
