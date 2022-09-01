namespace ObsStrawket.Test.Real {
  using ObsStrawket.Test.Utilities;
  using System;
  using System.Threading.Tasks;
  using Xunit;

  public class RealObsTest {
    private readonly bool _shouldSkip = false;

    public RealObsTest() {
      _shouldSkip = Environment.GetEnvironmentVariable("CI") != null;
      Console.WriteLine("");
    }

    [Fact()]
    public async Task TestNormalAsync() {
      if (_shouldSkip) {
        return;
      }
      await new ClientFlow().RunClientAsync(new Uri("ws://127.0.0.1:4455")).ConfigureAwait(false);
    }

    [Fact()]
    public async Task TestBadRequestAsync() {
      if (_shouldSkip) {
        return;
      }
      await ClientFlow.RequestBadAsync(new Uri("ws://127.0.0.1:4455")).ConfigureAwait(false);
    }

    [Fact()]
    public async Task JustMonitorObsEventAsync() {
      if (_shouldSkip) {
        return;
      }
      var client = ClientFlow.GetDebugClient();
      await client.ConnectAsync(new Uri("ws://127.0.0.1:4455"), "ahrEYXzXKytCIlpI").ConfigureAwait(false);
      var taskSource = new TaskCompletionSource<object>();
      client.Closed += (reason) => {
        taskSource.SetResult(reason);
      };
      await client.CloseAsync().ConfigureAwait(false);
      object reason = await taskSource.Task.ConfigureAwait(false);
      Console.WriteLine(reason);
    }
  }
}
