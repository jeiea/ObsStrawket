namespace ObsDotnetSocket.Test {
  using ObsDotnetSocket.DataTypes;
  using Xunit;

  public class BasicTest {
    [Fact]
    public async Task TestHelloAsync() {
      var connection = await Connection.ConnectAsync(password: "ahrEYXzXKytCIlpI").ConfigureAwait(false);
      var result = await connection.ReceiveAsync<IOpcodeMessage>().ConfigureAwait(false);
      Console.WriteLine(result);
    }
  }
}
