using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class InputDeinterlaceTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new InputDeinterlaceFlow());
    }
  }

  internal class InputDeinterlaceFlow : ITestFlow {
    public const string Mode = "OBS_DEINTERLACE_MODE_YADIF_2X";
    public const string FieldOrder = "OBS_DEINTERLACE_FIELD_ORDER_BOTTOM";

    public async Task RequestAsync(ObsClientSocket client) {
      _ = await client.SetInputDeinterlaceModeAsync(
        Mode,
        inputName: CreateInputFlow.MediaInputName
      ).ConfigureAwait(false);
      var mode = await client.GetInputDeinterlaceModeAsync(
        inputName: CreateInputFlow.MediaInputName
      ).ConfigureAwait(false);
      Assert.Equal(Mode, mode.InputDeinterlaceMode);

      _ = await client.SetInputDeinterlaceFieldOrderAsync(
        FieldOrder,
        inputName: CreateInputFlow.MediaInputName
      ).ConfigureAwait(false);
      var fieldOrder = await client.GetInputDeinterlaceFieldOrderAsync(
        inputName: CreateInputFlow.MediaInputName
      ).ConfigureAwait(false);
      Assert.Equal(FieldOrder, fieldOrder.InputDeinterlaceFieldOrder);
    }

    public async Task RespondAsync(MockServerSession session) {
      string guid = (await session.ReceiveRequestAsync("SetInputDeinterlaceMode", $$"""
{
  "inputName": "{{CreateInputFlow.MediaInputName}}",
  "inputDeinterlaceMode": "{{Mode}}"
}
""").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("SetInputDeinterlaceMode", guid).ConfigureAwait(false);

      guid = (await session.ReceiveRequestAsync("GetInputDeinterlaceMode", $$"""
{
  "inputName": "{{CreateInputFlow.MediaInputName}}"
}
""").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("GetInputDeinterlaceMode", guid, $$"""
{
  "inputDeinterlaceMode": "{{Mode}}"
}
""").ConfigureAwait(false);

      guid = (await session.ReceiveRequestAsync("SetInputDeinterlaceFieldOrder", $$"""
{
  "inputName": "{{CreateInputFlow.MediaInputName}}",
  "inputDeinterlaceFieldOrder": "{{FieldOrder}}"
}
""").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("SetInputDeinterlaceFieldOrder", guid).ConfigureAwait(false);

      guid = (await session.ReceiveRequestAsync("GetInputDeinterlaceFieldOrder", $$"""
{
  "inputName": "{{CreateInputFlow.MediaInputName}}"
}
""").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("GetInputDeinterlaceFieldOrder", guid, $$"""
{
  "inputDeinterlaceFieldOrder": "{{FieldOrder}}"
}
""").ConfigureAwait(false);
    }
  }
}
