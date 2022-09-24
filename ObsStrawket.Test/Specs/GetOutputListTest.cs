using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetOutputListTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetOutputListFlow()).ConfigureAwait(false);
    }
  }

  class GetOutputListFlow : ITestFlow {
    public static string OutputName => "virtualcam_output";

    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.GetOutputListAsync().ConfigureAwait(false);
      Assert.NotEmpty(response.Outputs);
      Assert.True(response.Outputs[0].Flags.HasFlag(DataTypes.OutputFlags.Video));
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestType"": ""GetOutputList""
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
    ""requestType"": ""GetOutputList"",
    ""responseData"": {
      ""outputs"": [
        {
          ""outputActive"": false,
          ""outputFlags"": {
            ""OBS_OUTPUT_AUDIO"": true,
            ""OBS_OUTPUT_ENCODED"": true,
            ""OBS_OUTPUT_MULTI_TRACK"": true,
            ""OBS_OUTPUT_SERVICE"": false,
            ""OBS_OUTPUT_VIDEO"": true
          },
          ""outputHeight"": 0,
          ""outputKind"": ""ffmpeg_muxer"",
          ""outputName"": ""adv_file_output"",
          ""outputWidth"": 0
        },
        {
          ""outputActive"": false,
          ""outputFlags"": {
            ""OBS_OUTPUT_AUDIO"": true,
            ""OBS_OUTPUT_ENCODED"": true,
            ""OBS_OUTPUT_MULTI_TRACK"": true,
            ""OBS_OUTPUT_SERVICE"": false,
            ""OBS_OUTPUT_VIDEO"": true
          },
          ""outputHeight"": 0,
          ""outputKind"": ""replay_buffer"",
          ""outputName"": ""Replay Buffer"",
          ""outputWidth"": 0
        },
        {
          ""outputActive"": false,
          ""outputFlags"": {
            ""OBS_OUTPUT_AUDIO"": false,
            ""OBS_OUTPUT_ENCODED"": false,
            ""OBS_OUTPUT_MULTI_TRACK"": false,
            ""OBS_OUTPUT_SERVICE"": false,
            ""OBS_OUTPUT_VIDEO"": true
          },
          ""outputHeight"": 720,
          ""outputKind"": ""virtualcam_output"",
          ""outputName"": ""virtualcam_output"",
          ""outputWidth"": 1280
        }
      ]
    }
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
