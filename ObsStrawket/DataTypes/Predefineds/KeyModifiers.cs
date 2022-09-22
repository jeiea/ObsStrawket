using MessagePack;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Data for representing OBS hotkey combination.
  /// </summary>
  [MessagePackObject]
  public class KeyModifiers {
    /// <summary>
    /// Press Shift | None | Not pressed<br />
    /// </summary>
    [Key("shift")]
    public bool? Shift { get; set; }
    /// <summary>
    /// Press CTRL | None | Not pressed<br />
    /// </summary>
    [Key("control")]
    public bool? Control { get; set; }
    /// <summary>
    /// Press ALT | None | Not pressed<br />
    /// </summary>
    [Key("alt")]
    public bool? Alt { get; set; }
    /// <summary>
    /// Press CMD (Mac) | None | Not pressed<br />
    /// </summary>
    [Key("command")]
    public bool? Command { get; set; }
  }
}
