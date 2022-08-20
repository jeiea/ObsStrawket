namespace ObsDotnetSocket.Test {
  using MessagePack;
  using ObsDotnetSocket.DataTypes;
  using ObsDotnetSocket.DataTypes.Predefineds;
  using ObsDotnetSocket.Serialization;
  using ObsDotnetSocket.WebSocket4Net;
  using System;
  using System.Linq;
  using System.Net;
  using System.Net.WebSockets;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Channels;
  using System.Threading.Tasks;
  using Xunit;
  using Xunit.Abstractions;

  public class RealObsTest {
    private readonly ITestOutputHelper _output;

    public RealObsTest(ITestOutputHelper output) {
      _output = output;
    }

    [Fact]
    public async Task TestSystemNetWebSocketsAsync() {
      await CommonFlow.RunClientAsync(new Uri("ws://127.0.0.1:4455")).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestWebSocket4NetAsync() {
      await CommonFlow.RunClientAsync(new Uri("ws://127.0.0.1:4455"), new WebSocket4NetSocket()).ConfigureAwait(false);
    }

    [Fact]
    public async Task JustMonitorObsEventAsync() {
      var methods = typeof(MessagePackSerializer).GetMethods().ToList();
      methods = methods.Where(x => x.Name == nameof(MessagePackSerializer.SerializeToJson)).ToList();
      var method = methods[1];
      var client = new ObsClientSocket();
      var source = new TaskCompletionSource<object?>();
      client.Event += (@event) => {
        object? result = method.MakeGenericMethod(@event.GetType()).Invoke(@event, new object[] {
          @event,
          MessagePackSerializerOptions.Standard.WithResolver(OpCodeMessageResolver.Instance), default(CancellationToken)
        });
        _output.WriteLine($"{result}");
      };
      client.Closed += (reason) => {
        source.SetResult(reason);
      };
      try {
        await client.ConnectAsync(new Uri("ws://127.0.0.1:4455"), "ahrEYXzXKytCIlpI").ConfigureAwait(false);
      }
      catch (Exception ex) {
        source.TrySetException(ex);
      }
      await source.Task.ConfigureAwait(false);
    }
  }
}
