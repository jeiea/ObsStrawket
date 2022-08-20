namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using System;

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
    public OutputFlags Flags { get; set; }
  }

  [Flags]
  public enum OutputFlags {
    OBS_OUTPUT_VIDEO = 1 << 0,
    OBS_OUTPUT_AUDIO = 1 << 1,
    OBS_OUTPUT_AV = OBS_OUTPUT_VIDEO | OBS_OUTPUT_AUDIO,
    OBS_OUTPUT_ENCODED = 1 << 2,
    OBS_OUTPUT_SERVICE = 1 << 3,
    OBS_OUTPUT_MULTI_TRACK = 1 << 4,
    OBS_OUTPUT_CAN_PAUSE = 1 << 5,
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
