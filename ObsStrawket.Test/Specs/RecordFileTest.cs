using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test.Specs {
  public class RecordFileTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new RecordFileFlow());
    }
  }

  internal class RecordFileFlow : ITestFlow {
    public const string ChapterName = "Test chapter";

    public async Task RequestAsync(ObsClientSocket client) {
      await Task.Delay(100).ConfigureAwait(false);
      _ = await client.CreateRecordChapterAsync(ChapterName).ConfigureAwait(false);
      _ = await client.SplitRecordFileAsync().ConfigureAwait(false);
      var changed = await ClientFlow.WaitEventAsync<RecordFileChanged>(client).ConfigureAwait(false);
      Assert.NotEmpty(changed.NewOutputPath);
    }

    public async Task RespondAsync(MockServerSession session) {
      string guid = (await session.ReceiveRequestAsync("CreateRecordChapter", $$"""
{
  "chapterName": "{{ChapterName}}"
}
""").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("CreateRecordChapter", guid).ConfigureAwait(false);

      guid = (await session.ReceiveRequestAsync("SplitRecordFile").ConfigureAwait(false))!;
      await session.SendSuccessResponseAsync("SplitRecordFile", guid).ConfigureAwait(false);
      await session.SendAsync(/*lang=json,strict*/ """
{
  "op": 5,
  "d": {
    "eventType": "RecordFileChanged",
    "eventIntent": 64,
    "eventData": {
      "newOutputPath": "recording-2.mp4"
    }
  }
}
""").ConfigureAwait(false);
    }
  }
}
