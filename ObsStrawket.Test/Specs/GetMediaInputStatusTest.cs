using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetMediaInputStatusTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetMediaInputStatusFlow()).ConfigureAwait(false);
    }
  }

  class GetMediaInputStatusFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetMediaInputStatusAsync(inputName: CreateInputFlow.MediaInputName).ConfigureAwait(false);

      Assert.Equal(MediaState.Playing, response.MediaState);
      Assert.Equal(20000, response.MediaDuration);
      Assert.InRange((int)response.MediaCursor!, 15000, 20000);
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""inputName"": ""Media source""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetMediaInputStatus""
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
    ""requestType"": ""GetMediaInputStatus"",
    ""responseData"": {
      ""mediaCursor"": 15100,
      ""mediaDuration"": 20000,
      ""mediaState"": ""OBS_MEDIA_STATE_PLAYING""
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
