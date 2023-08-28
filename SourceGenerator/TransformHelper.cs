using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SourceGenerator {
  internal static partial class TransformHelper {
    private static readonly Regex nullPattern = NullPattern();

    public static string ToPascalCase(string name) {
      return string.Concat(ToPascalCaseEnumerable(name));
    }

    public static string ToCSharpType(string type, string description) {
      string s1 = type
        .Replace("Array", "List")
        .Replace("String", "string")
        .Replace("Any", "object?")
        .Replace("Boolean", "bool")
        .Replace("Object", "Dictionary<string, object?>");
      bool seemsInteger = IntegerLikeWords().IsMatch(description);
      if (description.StartsWith("Average time in milliseconds")) {
        seemsInteger = false;
      }
      string s2 = s1.Replace("Number", seemsInteger ? "int" : "double");
      if (s2.Contains("int") && LargeIntegerLike().IsMatch(description)) {
        s2 = s2.Replace("int", "long");
      }
      if (!s2.EndsWith("?") && nullPattern.IsMatch(description)) {
        return $"{s2}?";
      }
      return s2;
    }

    public static string EscapeForXml(string unescaped) {
      var doc = new XmlDocument();
      var node = doc.CreateElement("root");
      node.InnerText = CrossPlatformNewLine().Replace(unescaped, Environment.NewLine);
      string xml = node.InnerXml;
      xml = Regex.Replace(xml, "`(.*?)`", "<c>$1</c>");
      return xml;
    }

    private static IEnumerable<char> ToPascalCaseEnumerable(string name) {
      bool isStart = true;

      foreach (char c in name) {
        if (isStart) {
          yield return char.ToUpperInvariant(c);
          isStart = false;
        }
        else if (c == ' ' || c == '_') {
          isStart = true;
        }
        else {
          yield return char.ToLowerInvariant(c);
        }
      }
    }

    [GeneratedRegex("index|offset|milli|frame|numerator|denominator|pixel|width|height|quality|\\bid\\b|number of|version|duration", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex IntegerLikeWords();
    [GeneratedRegex("\\bnull\\b")]
    private static partial Regex NullPattern();
    [GeneratedRegex("\\r\\n?|\\n")]
    private static partial Regex CrossPlatformNewLine();
    [GeneratedRegex("render|bytes|frames|duration|messages", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex LargeIntegerLike();
  }
}
