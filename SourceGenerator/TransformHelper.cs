using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace SourceGenerator {
  internal static class TransformHelper {
    private static readonly Regex nullPattern = new(@"\bnull\b");

    public static string ToPascalCase(string category) {
      return string.Concat(ToPascalCaseEnumerable(category));
    }

    public static string ToCSharpType(string type, string description) {
      string s1 = type
        .Replace("Array", "List")
        .Replace("String", "string")
        .Replace("Any", "object?")
        .Replace("Boolean", "bool")
        .Replace("Object", "Dictionary<string, object?>");
      bool seemsInteger = Regex.IsMatch(description, @"index|offset|milli|frame|numerator|denominator|pixel|width|height|quality|\bid\b|number of|version|duration", RegexOptions.IgnoreCase);
      if (description.StartsWith("Average time in milliseconds")) {
        seemsInteger = false;
      }
      string s2 = s1.Replace("Number", seemsInteger ? "int" : "double");
      if (!s2.EndsWith("?") && nullPattern.IsMatch(description)) {
        return $"{s2}?";
      }
      return s2;
    }

    public static string EscapeForXml(string unescaped) {
      var doc = new XmlDocument();
      var node = doc.CreateElement("root");
      node.InnerText = Regex.Replace(unescaped, @"\r\n?|\n", Environment.NewLine);
      string xml = node.InnerXml;
      xml = Regex.Replace(xml, "`(.*?)`", "<c>$1</c>");
      return xml;
    }

    private static IEnumerable<char> ToPascalCaseEnumerable(string category) {
      bool isStart = true;
      foreach (char c in category) {
        if (isStart) {
          yield return char.ToUpper(c);
          isStart = false;
        }
        else if (c == ' ') {
          isStart = true;
        }
        else {
          yield return c;
        }
      }
    }

  }
}
