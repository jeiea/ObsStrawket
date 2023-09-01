using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetInputNameTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetInputNameFlow()).ConfigureAwait(false);
    }
  }

  class SetInputNameFlow : ITestFlow {
    private static readonly string _temporaryName = "test scene renamed";

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetInputNameAsync(inputName: CreateInputFlow.InputName, newInputName: _temporaryName).ConfigureAwait(false);
      var changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(CreateInputFlow.InputName, (changed as InputNameChanged)!.OldInputName);
      Assert.Equal(_temporaryName, (changed as InputNameChanged)!.InputName);

      await client.SetInputNameAsync(inputName: _temporaryName, newInputName: CreateInputFlow.InputName).ConfigureAwait(false);
      changed = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal(_temporaryName, (changed as InputNameChanged)!.OldInputName);
      Assert.Equal(CreateInputFlow.InputName, (changed as InputNameChanged)!.InputName);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "inputName": "Browser source",
      "newInputName": "test scene renamed"
    },
    "requestId": "{guid}",
    "requestType": "SetInputName"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "inputName": "test scene renamed",
      "oldInputName": "Browser source"
    },
    "eventIntent": 8,
    "eventType": "InputNameChanged"
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
    "requestType": "SetInputName"
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync("""
{
  "d": {
    "requestData": {
      "inputName": "test scene renamed",
      "newInputName": "Browser source"
    },
    "requestId": "{guid}",
    "requestType": "SetInputName"
  },
  "op": 6
}
""").ConfigureAwait(false);
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "inputName": "Browser source",
      "oldInputName": "test scene renamed"
    },
    "eventIntent": 8,
    "eventType": "InputNameChanged"
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
    "requestType": "SetInputName"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
