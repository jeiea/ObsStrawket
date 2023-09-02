using System.Text.Json.Serialization;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Data for representing OBS hotkey combination.
  /// </summary>
  public class KeyModifiers {
    /// <summary>
    /// Press Shift | None | Not pressed<br />
    /// </summary>
    [JsonPropertyName("shift")]
    public bool? Shift { get; set; }
    /// <summary>
    /// Press CTRL | None | Not pressed<br />
    /// </summary>
    [JsonPropertyName("control")]
    public bool? Control { get; set; }
    /// <summary>
    /// Press ALT | None | Not pressed<br />
    /// </summary>
    [JsonPropertyName("alt")]
    public bool? Alt { get; set; }
    /// <summary>
    /// Press CMD (Mac) | None | Not pressed<br />
    /// </summary>
    [JsonPropertyName("command")]
    public bool? Command { get; set; }
  }
}
