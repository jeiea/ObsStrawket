using System.Collections.Generic;
using System.Threading.Tasks;

namespace ObsStrawket.Test.Utilities {
  public static class TestUtil {
    public const double Epsilon = 0.00000003;

    public static object ToDoubleOrInt(object boxed) {
      return boxed switch {
        float => (double)(float)boxed,
        byte => (int)(byte)boxed,
        ushort => (int)(ushort)boxed,
        _ => boxed,
      };
    }

    public static Task WhenAnyThrowsAsync(params Task[] tasks) {
      return WhenAnyThrowsAsync(tasks as IEnumerable<Task>);
    }

    public static async Task WhenAnyThrowsAsync(IEnumerable<Task> tasks) {
      var runnings = new List<Task>(tasks);
      while (runnings.Count > 0) {
        var finished = await Task.WhenAny(runnings).ConfigureAwait(false);
        await finished.ConfigureAwait(false);
        runnings.Remove(finished);
      }
    }
  }
}
