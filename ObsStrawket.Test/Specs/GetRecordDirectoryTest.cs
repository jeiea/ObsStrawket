using ObsStrawket.Test.Utilities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetRecordDirectoryTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetRecordDirectoryFlow()).ConfigureAwait(false);
    }
  }

  class GetRecordDirectoryFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetRecordDirectoryAsync().ConfigureAwait(false);
      Assert.True(Directory.Exists(response.RecordDirectory), "Record directory does not exist.");
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""op"": 6,
  ""d"": {
    ""requestType"": ""GetRecordDirectory"",
    ""requestId"": ""{guid}""
  }
}").ConfigureAwait(false);

    await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""GetRecordDirectory"",
    ""responseData"": {
      ""recordDirectory"": ""{cd}""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid).Replace("{cd}", Directory.GetCurrentDirectory().Replace(@"\", @"\\")));
    }
  }
}
