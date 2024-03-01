using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ObsStrawket.DataTypes {

  // https://github.com/obsproject/obs-websocket/blob/7893ae5caafecddb9589fe90719809b4f528f03e/src/utils/Obs_ArrayHelper.cpp#L339
  /// <summary>
  /// Output capability flags
  /// </summary>
  [Flags]
  [JsonConverter(typeof(JsonStringEnumMemberConverter))]
  public enum OutputFlags {

    /// <summary>
    /// Output has video.
    /// </summary>
    [EnumMember(Value = "OBS_OUTPUT_VIDEO")]
    Video = 1 << 0,

    /// <summary>
    /// Output has audio.
    /// </summary>
    [EnumMember(Value = "OBS_OUTPUT_AUDIO")]
    Audio = 1 << 1,

    /// <summary>
    /// Output is encoded.
    /// </summary>
    [EnumMember(Value = "OBS_OUTPUT_ENCODED")]
    Encoded = 1 << 2,

    /// <summary>
    /// Output requires a service object.
    /// </summary>
    [EnumMember(Value = "OBS_OUTPUT_SERVICE")]
    Service = 1 << 3,

    /// <summary>
    /// Output supports multiple audio tracks.
    /// </summary>
    [EnumMember(Value = "OBS_OUTPUT_MULTI_TRACK")]
    MultiTrack = 1 << 4,

    /// <summary>
    /// Output can be paused.
    /// </summary>
    [EnumMember(Value = "OBS_OUTPUT_CAN_PAUSE")]
    CanPause = 1 << 5,
  }

  // https://github.com/obsproject/obs-websocket/blob/5f8a0122bdd0146fdb33968f6bdf6ab624851e7a/src/utils/Obs_ArrayHelper.cpp#L335
  // https://github.com/obsproject/obs-studio/blob/master/docs/sphinx/reference-outputs.rst
  /// <summary>
  /// Outputs allow the ability to output the currently rendering audio/video.<br />
  /// Streaming and recording are two common examples of outputs, but not the only
  /// types of outputs.<br />Outputs can receive the raw data or receive encoded data.
  /// </summary>
  public class Output {

    /// <summary>
    /// Name of the output.
    /// </summary>
    [JsonPropertyName("outputName")]
    public string Name { get; set; } = "";

    /// <summary>
    /// Example: <c>ffmpeg_muxer</c>, <c>virtualcam_output</c>
    /// </summary>
    [JsonPropertyName("outputKind")]
    public string Kind { get; set; } = "";

    /// <summary>
    /// Video width.
    /// </summary>
    [JsonPropertyName("outputWidth")]
    public int Width { get; set; }

    /// <summary>
    /// Video height.
    /// </summary>
    [JsonPropertyName("outputHeight")]
    public int Height { get; set; }

    /// <summary>
    /// Whether it is recorded or streamed.
    /// </summary>
    [JsonPropertyName("outputActive")]
    public bool Active { get; set; }

    /// <summary>
    ///
    /// </summary>
    [JsonPropertyName("outputFlags")]
    [MessagePackFormatter(typeof(OutputFlagsMapFormatter))]
    public OutputFlags Flags { get; set; }
  }

  // https://github.com/obsproject/obs-websocket/blob/5f8a0122bdd0146fdb33968f6bdf6ab624851e7a/src/utils/Obs_ArrayHelper.cpp#L87
  /// <summary>
  /// Represents OBS scene.
  /// </summary>
  public class Scene {

    /// <summary>
    /// Scene name.
    /// </summary>
    [JsonPropertyName("sceneName")]
    public string Name { get; set; } = "";

    /// <summary>
    /// Scene index.
    /// </summary>
    [JsonPropertyName("sceneIndex")]
    public int Index { get; set; }
  }

  //https://github.com/obsproject/obs-websocket/blob/5f8a0122bdd0146fdb33968f6bdf6ab624851e7a/src/utils/Obs_ArrayHelper.cpp#L179
  /// <summary>
  /// OBS input, e.g. scene items.
  /// </summary>
  public class Input {

    /// <summary>
    /// Input name.
    /// </summary>
    [JsonPropertyName("inputName")]
    public string Name { get; set; } = "";

    /// <summary>
    /// Input kind. e.g. <c>color_source_v3</c>
    /// </summary>
    [JsonPropertyName("inputKind")]
    public string Kind { get; set; } = "";

    /// <summary>
    /// Input kind without version. e.g. <c>color_source</c>
    /// </summary>
    [JsonPropertyName("unversionedInputKind")]
    public string UnversionedKind { get; set; } = "";
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

  //https://github.com/obsproject/obs-websocket/blob/265899f76f88a5be74747308fff3d35347ce43c5/src/utils/Obs_ArrayHelper.cpp#L142
  /// <summary>
  /// Represents reindexed scene item.
  /// </summary>
  public class BasicSceneItem {

    [JsonPropertyName("sceneItemId")]
    public int Id { get; set; }

    [JsonPropertyName("sceneItemIndex")]
    public int Index { get; set; }
  }

  //https://github.com/obsproject/obs-websocket/blob/265899f76f88a5be74747308fff3d35347ce43c5/src/utils/Obs_ArrayHelper.cpp#L142
  /// <summary>
  /// Represents scene item.
  /// </summary>
  public class SceneItem : BasicSceneItem {

    [JsonPropertyName("sceneItemEnabled")]
    public bool? Enabled { get; set; }

    [JsonPropertyName("sceneItemLocked")]
    public bool? Locked { get; set; }

    [JsonPropertyName("sceneItemTransform")]
    public Dictionary<string, object>? Transform { get; set; }

    [JsonPropertyName("sceneItemBlendMode")]
    public BlendingType? BlendMode { get; set; }

    [JsonPropertyName("sourceName")]
    public string? SourceName { get; set; }

    [JsonPropertyName("sourceType")]
    public SourceType? SourceType { get; set; }

    [JsonPropertyName("inputKind")]
    public string? InputKind { get; set; }

    [JsonPropertyName("isGroup")]
    public bool? IsGroup { get; set; }
  }

  // https://github.com/obsproject/obs-websocket/blob/265899f76f88a5be74747308fff3d35347ce43c5/src/utils/Obs_ArrayHelper.cpp#L306-L307
  /// <summary>
  /// Represents source filter.
  /// </summary>
  public class SourceFilter {

    [JsonPropertyName("filterName")]
    public string Name { get; set; } = "";

    [JsonPropertyName("filterIndex")]
    public int Index { get; set; }

    [JsonPropertyName("filterKind")]
    public string Kind { get; set; } = "";

    [JsonPropertyName("filterEnabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("filterSettings")]
    public Dictionary<string, object?> Settings { get; set; } = new();
  }

  // https://github.com/obsproject/obs-websocket/blob/265899f76f88a5be74747308fff3d35347ce43c5/src/utils/Obs_ArrayHelper.cpp#L273
  /// <summary>
  /// Listed scene transition.
  /// </summary>
  public class AvailableTransition {

    /// <summary>
    /// Name of the transition.
    /// </summary>
    [JsonPropertyName("transitionName")]
    public string Name { get; set; } = "";

    /// <summary>
    /// Kind of the transition.
    /// </summary>
    [JsonPropertyName("transitionKind")]
    public string Kind { get; set; } = "";

    /// <summary>
    /// Whether the transition uses a fixed (unconfigurable) duration.
    /// </summary>
    [JsonPropertyName("transitionFixed")]
    public bool Fixed { get; set; }

    /// <summary>
    /// Whether the transition supports being configured.
    /// </summary>
    [JsonPropertyName("transitionConfigurable")]
    public bool Configurable { get; set; }
  }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
