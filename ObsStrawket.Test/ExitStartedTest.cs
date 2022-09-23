using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class ExitStartedTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new ExitStartedFlow()).ConfigureAwait(false);
    }
  }

  class ExitStartedFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var exitStarted = await client.Events.ReadAsync().ConfigureAwait(false);
      Assert.IsType<ExitStarted>(exitStarted);
    }

    public async Task RespondAsync(MockServerSession session) {
      await session.SendAsync(@"{
  ""d"": {
    ""eventIntent"": 1,
    ""eventType"": ""ExitStarted""
  },
  ""op"": 5
}").ConfigureAwait(false);
    }
  }
}
