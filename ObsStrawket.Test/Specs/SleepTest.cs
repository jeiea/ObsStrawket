using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  //public class SleepTest {
  //  [Fact]
  //  public async Task TestAsync() {
  //    await SpecTester.TestAsync(new SleepFlow()).ConfigureAwait(false);
  //  }
  //}

  class SleepFlow : ITestFlow {
    public async Task RequestAsync(ObsClientSocket client) {
      var response = await client.SleepAsync(sleepMillis: 17, sleepFrames: 1).ConfigureAwait(false);
      return;
    }

    public async Task RespondAsync(MockServerSession session) {
      string? guid = await session.ReceiveAsync(@"").ConfigureAwait(false);
      await session.SendAsync(@"".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
