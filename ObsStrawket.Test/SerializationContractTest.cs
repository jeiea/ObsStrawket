using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using System.Text.Json;
using Xunit;

namespace ObsStrawket.Test {

  public class SerializationContractTest {
    private const string UnknownOpCodeJson = """{"op":4,"d":{}}""";

    [Fact]
    public void StringEnumPreservesProtocolValue() {
      const string Json = "\"OBS_WEBSOCKET_OUTPUT_STARTED\"";

      Assert.Equal(Json, JsonSerializer.Serialize(ObsOutputState.Started));
      Assert.Equal(
        ObsOutputState.Started,
        JsonSerializer.Deserialize<ObsOutputState>(Json));
    }

    [Fact]
    public void UnknownOpCodeReportsProtocolValue() {
      var exception = Assert.Throws<ObsProtocolException>(
        static () => JsonSerializer.Deserialize<IOpCodeMessage>(UnknownOpCodeJson));

      Assert.Contains("4", exception.Message);
    }
  }
}
