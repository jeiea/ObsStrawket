using ObsStrawket.Test.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class SetProfileParameterTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetProfileParameterFlow());
    }
  }

  class SetProfileParameterFlow : ITestFlow {
    public static List<(string Category, string Name, string Value, string? Default)> AppliedParameters = [
      ("Output", "Mode", "Advanced", "Simple"),
      ("AdvOut", "RecRB", "true", "false"),
      ("AdvOut", "RecEncoder", "obs_x264", "none"),
      ("AdvOut", "RecFormat2", "hybrid_mp4", "hybrid_mp4"),
      ("AdvOut", "RecSplitFile", "true", null),
      ("AdvOut", "RecSplitFileType", "Manual", null),
    ];

    public async Task RequestAsync(ObsClientSocket client) {
      foreach (var (category, name, value, _) in AppliedParameters) {
        await client.SetProfileParameterAsync(
          parameterCategory: category,
          parameterName: name,
          parameterValue: value
        ).ConfigureAwait(false);
      }
    }

    public async Task RespondAsync(MockServerSession session) {
      foreach (var (category, name, value, _) in AppliedParameters) {
        string guid = (await session.ReceiveRequestAsync("SetProfileParameter", $$"""
{
  "parameterCategory": "{{category}}",
  "parameterName": "{{name}}",
  "parameterValue": "{{value}}"
}
""").ConfigureAwait(false))!;
        await session.SendSuccessResponseAsync("SetProfileParameter", guid).ConfigureAwait(false);
      }
    }
  }
}
