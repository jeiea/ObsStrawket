using ObsStrawket.Test.Utilities;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetRecordDirectoryTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetRecordDirectoryFlow()).ConfigureAwait(false);
    }
  }

  class SetRecordDirectoryFlow : ITestFlow {
    static string Directory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetRecordDirectoryAsync(Directory).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync($$"""
{
  "d": {
    "requestData": {
      "recordDirectory": "{{Directory.Replace("\\", "\\\\")}}"
    },
    "requestId": "{guid}",
    "requestType": "SetRecordDirectory"
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
    "requestType": "SetRecordDirectory"
  },
  "op": 7
}
""").ConfigureAwait(false);
    }
  }
}
