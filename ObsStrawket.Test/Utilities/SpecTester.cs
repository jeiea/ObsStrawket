using System;
using System.Threading;
using System.Threading.Tasks;

namespace ObsStrawket.Test.Utilities {

  internal interface ITestFlow {

    Task RequestAsync(ObsClientSocket client);

    Task RespondAsync(MockServerSession session);
  }

  internal class SpecTester {

    public static async Task TestAsync(ITestFlow flow, bool useChannel = true) {
      var taskSource = new TaskCompletionSource();
      using var server = new MockServer().Run(handler: async (context, cancellation) => {
        try {
          var (webSocketContext, session) = await MockServer.HandshakeAsync(context, cancellation).ConfigureAwait(false);
          await flow.RespondAsync(session).ConfigureAwait(false);
          await MockServer.CloseQuietlyAsync(webSocketContext.WebSocket, cancellation).ConfigureAwait(false);
          taskSource.SetResult();
        }
        catch (Exception ex) {
          taskSource.SetException(ex);
        }
      });

      async Task RunClientAsync() {
        var client = ClientFlow.GetDebugClient(useChannel: useChannel);
        await client.ConnectAsync(server.Uri, MockServer.Password).ConfigureAwait(false);
        await flow.RequestAsync(client).ConfigureAwait(false);
        await client.CloseAsync().ConfigureAwait(false);
      }

      await WhenAllFailFast(taskSource.Task, RunClientAsync()).ConfigureAwait(false);
    }

    public static Task WhenAllFailFast(params Task[] tasks) {
      ArgumentNullException.ThrowIfNull(tasks);

      if (tasks.Length == 0) {
        return Task.CompletedTask;
      }

      int remaining = tasks.Length;
      var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

      for (int i = 0; i < tasks.Length; i++) {
        var task = tasks[i] ?? throw new ArgumentException(
          $"The {nameof(tasks)} argument included a null value.",
          nameof(tasks)
        );
        HandleCompletion(task);
      }
      return tcs.Task;

      async void HandleCompletion(Task task) {
        try {
          await task.ConfigureAwait(false);
          if (Interlocked.Decrement(ref remaining) == 0) {
            _ = tcs.TrySetResult();
          }
        }
        catch (OperationCanceledException) {
          _ = tcs.TrySetCanceled();
        }
        catch (Exception ex) {
          _ = tcs.TrySetException(ex);
        }
      }
    }
  }
}
