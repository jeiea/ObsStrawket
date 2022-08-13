namespace ObsDotnetSocket.Test {
  using ObsDotnetSocket.DataTypes;
  using Xunit;

  public class BasicTest {
    [Fact]
    public async Task TestHello() {
      var connection = await Connection.Connect(password: "FRjsBxjFw8LL1nlL").ConfigureAwait(false);
      var result = await connection.Receive<IOpcodeMessage>().ConfigureAwait(false);
      Console.WriteLine(result);
    }
  }
}
