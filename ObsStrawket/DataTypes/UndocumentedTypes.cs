namespace ObsStrawket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;
  using ObsStrawket.Serialization;
  using System;
  using System.Runtime.Serialization;

  // GetOutputList
  // https://github.com/obsproject/obs-websocket/blob/5f8a0122bdd0146fdb33968f6bdf6ab624851e7a/src/utils/Obs_ArrayHelper.cpp#L335
  [MessagePackObject]
  public class Output {
    [Key("outputName")]
    public string Name { get; set; } = "";

    [Key("outputKind")]
    public string Kind { get; set; } = "";

    [Key("outputWidth")]
    public int Width { get; set; }

    [Key("outputHeight")]
    public int Height { get; set; }

    [Key("outputActive")]
    public bool Active { get; set; }

    [Key("outputFlags")]
    [MessagePackFormatter(typeof(OutputFlagsMapFormatter))]
    public OutputFlags Flags { get; set; }
  }

  // https://github.com/obsproject/obs-websocket/blob/7893ae5caafecddb9589fe90719809b4f528f03e/src/utils/Obs_ArrayHelper.cpp#L339
  [Flags]
  [MessagePackFormatter(typeof(EnumAsStringFormatter<OutputFlags>))]
  public enum OutputFlags {
    [EnumMember(Value = "OBS_OUTPUT_VIDEO")]
    Video = 1 << 0,
    [EnumMember(Value = "OBS_OUTPUT_AUDIO")]
    Audio = 1 << 1,
    [EnumMember(Value = "OBS_OUTPUT_ENCODED")]
    Encoded = 1 << 2,
    [EnumMember(Value = "OBS_OUTPUT_SERVICE")]
    Service = 1 << 3,
    [EnumMember(Value = "OBS_OUTPUT_MULTI_TRACK")]
    MultiTrack = 1 << 4,
    [EnumMember(Value = "OBS_OUTPUT_CAN_PAUSE")]
    CanPause = 1 << 5,
  }

  // https://github.com/obsproject/obs-websocket/blob/5f8a0122bdd0146fdb33968f6bdf6ab624851e7a/src/utils/Obs_ArrayHelper.cpp#L87
  [MessagePackObject]
  public class Scene {
    [Key("sceneName")]
    public string Name { get; set; } = "";

    [Key("sceneIndex")]
    public int Index { get; set; }
  }

  //https://github.com/obsproject/obs-websocket/blob/5f8a0122bdd0146fdb33968f6bdf6ab624851e7a/src/utils/Obs_ArrayHelper.cpp#L179
  [MessagePackObject]
  public class Input {
    [Key("inputName")]
    public string Name { get; set; } = "";

    [Key("inputKind")]
    public string Kind { get; set; } = "";

    [Key("unversionedInputKind")]
    public string UnversionedKind { get; set; } = "";
  }
}
