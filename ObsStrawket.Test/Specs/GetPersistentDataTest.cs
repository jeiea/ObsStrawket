using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetPersistentDataTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetPersistentDataFlow()).ConfigureAwait(false);
    }
  }

  class GetPersistentDataFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetPersistentDataAsync(realm: DataRealm.Profile, slotName: SetPersistentDataFlow.TestSlot).ConfigureAwait(false);
      Assert.Equal(SetPersistentDataFlow.TestSlotValue, response.SlotValue);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "realm": "OBS_WEBSOCKET_DATA_REALM_PROFILE",
      "slotName": "test slot"
    },
    "requestId": "{guid}",
    "requestType": "GetPersistentData"
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
    "requestType": "GetPersistentData",
    "responseData": {
      "slotValue": "test slot value"
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
