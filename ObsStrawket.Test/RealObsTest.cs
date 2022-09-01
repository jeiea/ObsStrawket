namespace ObsStrawket.Test {
  using ObsStrawket.Test.Utilities;
  using System;
  using System.Threading.Tasks;
  using Xunit;
  using Xunit.Abstractions;

  public class RealObsTest {
    [Fact]
    public async Task TestNormalAsync() {
      await new ClientFlow().RunClientAsync(new Uri("ws://127.0.0.1:4455")).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestBadRequestAsync() {
      await ClientFlow.RequestBadAsync(new Uri("ws://127.0.0.1:4455")).ConfigureAwait(false);
    }

    [Fact]
    public async Task JustMonitorObsEventAsync() {
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
