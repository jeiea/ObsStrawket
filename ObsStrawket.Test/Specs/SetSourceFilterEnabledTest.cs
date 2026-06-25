using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetSourceFilterEnabledTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetSourceFilterEnabledFlow());
    }
  }

  internal class SetSourceFilterEnabledFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetSourceFilterEnabledAsync(
        sourceName: CreateInputFlow.InputName,
        filterName: CreateSourceFilterFlow.FilterName,
        filterEnabled: false
      ).ConfigureAwait(false);

      var changed = await ClientFlow.WaitEventAsync<SourceFilterEnableStateChanged>(client).ConfigureAwait(false);
      Assert.False(changed.FilterEnabled);
      Assert.Equal(CreateSourceFilterFlow.FilterName, changed.FilterName);
      Assert.Equal("Browser source", changed.SourceName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(/*lang=json,strict*/ """
{
  "d": {
    "requestData": {
      "filterEnabled": false,
      "filterName": "test filter name",
      "sourceName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "SetSourceFilterEnabled"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "d": {
    "eventData": {
      "filterEnabled": false,
      "filterName": "test filter name",
      "sourceName": "Browser source"
    },
    "eventIntent": 32,
    "eventType": "SourceFilterEnableStateChanged"
  },
  "op": 5
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
    "requestType": "SetSourceFilterEnabled"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
