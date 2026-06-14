using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetPersistentDataTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetPersistentDataFlow());
    }
  }

  internal class SetPersistentDataFlow : ITestFlow {
    public static string TestSlot => "test slot";
    public static string TestSlotValue => "test slot value";

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetPersistentDataAsync(realm: DataTypes.DataRealm.Profile, slotName: TestSlot, slotValue: TestSlotValue).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "realm": "OBS_WEBSOCKET_DATA_REALM_PROFILE",
      "slotName": "test slot",
      "slotValue": "test slot value"
    },
    "requestId": "{guid}",
    "requestType": "SetPersistentData"
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
    "requestType": "SetPersistentData"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
