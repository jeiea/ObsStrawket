namespace ObsDotnetSocket.Test {
  using Newtonsoft.Json.Linq;
  using Xunit;

  static class TestUtil {
    public static void AssertJsonEqual(string expected, string actual) {
      var expectation = JToken.Parse(expected);
      var actualJson = JToken.Parse(actual);
      Assert.True(JToken.DeepEquals(expectation, actualJson), $"JSON expected: {expectation}, actual: {actualJson}");
    }
  }
}
