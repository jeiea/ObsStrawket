namespace ObsStrawket.Test.Real {
  using ObsStrawket.Test.Specs;
  using ObsStrawket.Test.Utilities;
  using System;
  using System.Threading.Tasks;
  using Xunit;

  public class RealObsTest {
    private readonly bool _shouldSkip = false;

    public RealObsTest() {
      _shouldSkip = Environment.GetEnvironmentVariable("CI") != null;
    }

    [Fact]
    public async Task TestNormalAsync() {
      if (_shouldSkip) {
        return;
      }
      await new ClientFlow().RunClientAsync(new Uri("ws://127.0.0.1:4455")).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestBadRequestAsync() {
      if (_shouldSkip) {
        return;
      }
      await ClientFlow.RequestBadAsync(new Uri("ws://127.0.0.1:4455")).ConfigureAwait(false);
    }

    [Fact]
    public async Task JustMonitorObsEventAsync() {
      if (_shouldSkip) {
        return;
      }
      var client = ClientFlow.GetDebugClient(useChannel: true);
      await client.ConnectAsync(new Uri("ws://127.0.0.1:4455"), "ahrEYXzXKytCIlpI").ConfigureAwait(false);
      await new ToggleRecordFlow().RequestAsync(client).ConfigureAwait(false);
      return;
    }
  }
}
