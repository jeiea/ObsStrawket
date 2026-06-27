using System.IO;

namespace SourceGenerator {
  internal static class GeneratedText {
    public const string NewLine = "\n";

    public static StreamWriter CreateText(string path) {
      return new StreamWriter(path) {
        NewLine = NewLine,
      };
    }

    public static StringWriter CreateStringWriter() {
      return new StringWriter {
        NewLine = NewLine,
      };
    }

    public static string NormalizeNewLine(string text) {
      return text.ReplaceLineEndings(NewLine);
    }
  }
}
