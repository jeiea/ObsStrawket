using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class GetProfileParameterTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new GetProfileParameterFlow());
    }
  }

  internal class GetProfileParameterFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      foreach (var (category, name, value, def) in SetProfileParameterFlow.AppliedParameters) {
        var response = await client.GetProfileParameterAsync(
          parameterCategory: category,
          parameterName: name
        ).ConfigureAwait(false);
        Assert.Equal(value, response.ParameterValue);
        Assert.Equal(def, response.DefaultParameterValue);
      }
    }

    public async Task RespondAsync(MockServerSession session) {
      foreach (var (category, name, value, def) in SetProfileParameterFlow.AppliedParameters) {
        string defaultValue = def == null ? "null" : $"\"{def}\"";
        string guid = (await session.ReceiveRequestAsync("GetProfileParameter", $$"""
{
  "parameterCategory": "{{category}}",
  "parameterName": "{{name}}"
}
""").ConfigureAwait(false))!;
        await session.SendSuccessResponseAsync("GetProfileParameter", guid, $$"""
{
  "defaultParameterValue": {{defaultValue}},
  "parameterValue": "{{value}}"
}
""").ConfigureAwait(false);
      }
    }
  }
}
