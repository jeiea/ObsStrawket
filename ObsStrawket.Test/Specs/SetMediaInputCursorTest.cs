using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetMediaInputCursorTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetMediaInputCursorFlow()).ConfigureAwait(false);
    }
  }

  class SetMediaInputCursorFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      await client.SetMediaInputCursorAsync(
        inputName: CreateInputFlow.MediaInputName,
        mediaCursor: 5 * 1000
      ).ConfigureAwait(false);
      await Task.Delay(100).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Media source"",
      ""mediaCursor"": 5000
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetMediaInputCursor""
  },
  ""op"": 6
}").ConfigureAwait(false);
      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""SetMediaInputCursor""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
