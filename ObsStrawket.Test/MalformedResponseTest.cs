using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class MalformedResponseTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new MalformedResponseFlow()).ConfigureAwait(false);
    }
  }

  class MalformedResponseFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var ev = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.Equal("OSOFJWEOIFJWEOIJF", (ev as RawEvent)!.EventData!["monitorType"]);

      var response = await client.GetRecordStatusAsync().ConfigureAwait(false);
      Assert.True(response.OutputActive);
      Assert.NotInRange(response.OutputBytes, int.MinValue, 0);
      Assert.NotInRange(response.OutputDuration, int.MinValue, 0);
      Assert.StartsWith("00:00:0", response.OutputTimecode);
      Assert.True(response.OutputPaused);

      await Assert.ThrowsAsync<QueueCancelledException>(() => client.GetRecordStatusAsync()).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      await session.SendAsync("""
{
  "d": {
    "eventData": {
      "inputName": "Browser source",
      "monitorType": "OSOFJWEOIFJWEOIJF"
    },
    "eventIntent": 8,
    "eventType": "InputAudioMonitorTypeChanged"
  },
  "op": 5
}
""").ConfigureAwait(false);

      string? guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetRecordStatus"
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
    "requestType": "GetRecordStatus",
    "responseData": {
      "outputActive": true,
      "outputBytes": 1469,
      "outputDuration": 49,
      "outputPaused": true,
      "outputTimecode": "00:00:00.049"
    }
  },
  "op": 7
}
""").ConfigureAwait(false);

      guid = await session.ReceiveAsync("""
{
  "d": {
    "requestId": "{guid}",
    "requestType": "GetRecordStatus"
  },
  "op": 6
}
""").ConfigureAwait(false);

      // This does not conform to the serialization format.
      // In this case requestId may not be matched and can cause hang.
      // So it should close connection.
      await session.SendAsync($$"""
{
  "d": {
    "requestId": "{{guid}}",
    "requestStatus": {
      "code": 100,
      "result": true
    },
    "requestType": "GetRecordStatus",
    "responseData": {
      "outputActive": true,
      "outputBytes": 1469,
      "outputDuration": 4796153459128734987162398746123978416448,
      "outputPaused": true,
      "outputTimecode": "133226484:58:36.448",
    }
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
