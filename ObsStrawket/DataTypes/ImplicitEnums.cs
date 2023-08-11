using MessagePack;
using MessagePack.Formatters;
using System.Runtime.Serialization;

namespace ObsStrawket.DataTypes {
  // Input: search NLOHMANN_JSON_SERIALIZE_ENUM
  // https://gchq.github.io/CyberChef/#recipe=Find_/_Replace(%7B'option':'Regex','string':'%5E%5C%5Cs%2B'%7D,'',true,false,true,false)Find_/_Replace(%7B'option':'Regex','string':'NLOH.*?_ENUM%5C%5C(obs_?(%5C%5Cw%2B),.*?%7B(.*?)%7D%5C%5C)'%7D,'public%20enum%20$1%20%7B$2%7D',true,true,false,true)Find_/_Replace(%7B'option':'Regex','string':'%5C%5C%7B(%5C%5Cw%2B_(%5C%5Cw%2B)),.*?%5C%5C%7D,'%7D,'%20%20%5BEnumMember(Value%20%3D%20%22$1%22)%5D%5C%5Cn%20%20$2,',true,false,true,false)Subsection('%5C%5Cn%20%20%5C%5Cw%2B,',true,true,false)To_Lower_case()To_Upper_case('Word')Merge(true)Subsection('(?%3C%3Dpublic%20enum%20)%5C%5Cw%2B',true,true,false)To_Camel_case(false)To_Upper_case('Word')Merge(true)Regular_expression('User%20defined','%5C%5Cn%5C%5Cw%2B,',false,false,true,false,false,false,'Highlight%20matches')&input=Ck5MT0hNQU5OX0pTT05fU0VSSUFMSVpFX0VOVU0ob2JzX3NvdXJjZV90eXBlLCB7CgkJCQkJCSAgICAgIHtPQlNfU09VUkNFX1RZUEVfSU5QVVQsICJPQlNfU09VUkNFX1RZUEVfSU5QVVQifSwKCQkJCQkJICAgICAge09CU19TT1VSQ0VfVFlQRV9GSUxURVIsICJPQlNfU09VUkNFX1RZUEVfRklMVEVSIn0sCgkJCQkJCSAgICAgIHtPQlNfU09VUkNFX1RZUEVfVFJBTlNJVElPTiwgIk9CU19TT1VSQ0VfVFlQRV9UUkFOU0lUSU9OIn0sCgkJCQkJCSAgICAgIHtPQlNfU09VUkNFX1RZUEVfU0NFTkUsICJPQlNfU09VUkNFX1RZUEVfU0NFTkUifSwKCQkJCQkgICAgICB9KQoKTkxPSE1BTk5fSlNPTl9TRVJJQUxJWkVfRU5VTShvYnNfbW9uaXRvcmluZ190eXBlLAoJCQkgICAgIHsKCQkJCSAgICAge09CU19NT05JVE9SSU5HX1RZUEVfTk9ORSwgIk9CU19NT05JVE9SSU5HX1RZUEVfTk9ORSJ9LAoJCQkJICAgICB7T0JTX01PTklUT1JJTkdfVFlQRV9NT05JVE9SX09OTFksICJPQlNfTU9OSVRPUklOR19UWVBFX01PTklUT1JfT05MWSJ9LAoJCQkJICAgICB7T0JTX01PTklUT1JJTkdfVFlQRV9NT05JVE9SX0FORF9PVVRQVVQsICJPQlNfTU9OSVRPUklOR19UWVBFX01PTklUT1JfQU5EX09VVFBVVCJ9LAoJCQkgICAgIH0pCgpOTE9ITUFOTl9KU09OX1NFUklBTElaRV9FTlVNKG9ic19tZWRpYV9zdGF0ZSwgewoJCQkJCQkgICAgICB7T0JTX01FRElBX1NUQVRFX05PTkUsICJPQlNfTUVESUFfU1RBVEVfTk9ORSJ9LAoJCQkJCQkgICAgICB7T0JTX01FRElBX1NUQVRFX1BMQVlJTkcsICJPQlNfTUVESUFfU1RBVEVfUExBWUlORyJ9LAoJCQkJCQkgICAgICB7T0JTX01FRElBX1NUQVRFX09QRU5JTkcsICJPQlNfTUVESUFfU1RBVEVfT1BFTklORyJ9LAoJCQkJCQkgICAgICB7T0JTX01FRElBX1NUQVRFX0JVRkZFUklORywgIk9CU19NRURJQV9TVEFURV9CVUZGRVJJTkcifSwKCQkJCQkJICAgICAge09CU19NRURJQV9TVEFURV9QQVVTRUQsICJPQlNfTUVESUFfU1RBVEVfUEFVU0VEIn0sCgkJCQkJCSAgICAgIHtPQlNfTUVESUFfU1RBVEVfU1RPUFBFRCwgIk9CU19NRURJQV9TVEFURV9TVE9QUEVEIn0sCgkJCQkJCSAgICAgIHtPQlNfTUVESUFfU1RBVEVfRU5ERUQsICJPQlNfTUVESUFfU1RBVEVfRU5ERUQifSwKCQkJCQkJICAgICAge09CU19NRURJQV9TVEFURV9FUlJPUiwgIk9CU19NRURJQV9TVEFURV9FUlJPUiJ9LAoJCQkJCSAgICAgIH0pCgpOTE9ITUFOTl9KU09OX1NFUklBTElaRV9FTlVNKG9ic19ib3VuZHNfdHlwZSwgewoJCQkJCQkgICAgICB7T0JTX0JPVU5EU19OT05FLCAiT0JTX0JPVU5EU19OT05FIn0sCgkJCQkJCSAgICAgIHtPQlNfQk9VTkRTX1NUUkVUQ0gsICJPQlNfQk9VTkRTX1NUUkVUQ0gifSwKCQkJCQkJICAgICAge09CU19CT1VORFNfU0NBTEVfSU5ORVIsICJPQlNfQk9VTkRTX1NDQUxFX0lOTkVSIn0sCgkJCQkJCSAgICAgIHtPQlNfQk9VTkRTX1NDQUxFX09VVEVSLCAiT0JTX0JPVU5EU19TQ0FMRV9PVVRFUiJ9LAoJCQkJCQkgICAgICB7T0JTX0JPVU5EU19TQ0FMRV9UT19XSURUSCwgIk9CU19CT1VORFNfU0NBTEVfVE9fV0lEVEgifSwKCQkJCQkJICAgICAge09CU19CT1VORFNfU0NBTEVfVE9fSEVJR0hULCAiT0JTX0JPVU5EU19TQ0FMRV9UT19IRUlHSFQifSwKCQkJCQkJICAgICAge09CU19CT1VORFNfTUFYX09OTFksICJPQlNfQk9VTkRTX01BWF9PTkxZIn0sCgkJCQkJICAgICAgfSkKCk5MT0hNQU5OX0pTT05fU0VSSUFMSVpFX0VOVU0ob2JzX2JsZW5kaW5nX3R5cGUsIHsKCQkJCQkJCXtPQlNfQkxFTkRfTk9STUFMLCAiT0JTX0JMRU5EX05PUk1BTCJ9LAoJCQkJCQkJe09CU19CTEVORF9BRERJVElWRSwgIk9CU19CTEVORF9BRERJVElWRSJ9LAoJCQkJCQkJe09CU19CTEVORF9TVUJUUkFDVCwgIk9CU19CTEVORF9TVUJUUkFDVCJ9LAoJCQkJCQkJe09CU19CTEVORF9TQ1JFRU4sICJPQlNfQkxFTkRfU0NSRUVOIn0sCgkJCQkJCQl7T0JTX0JMRU5EX01VTFRJUExZLCAiT0JTX0JMRU5EX01VTFRJUExZIn0sCgkJCQkJCQl7T0JTX0JMRU5EX0xJR0hURU4sICJPQlNfQkxFTkRfTElHSFRFTiJ9LAoJCQkJCQkJe09CU19CTEVORF9EQVJLRU4sICJPQlNfQkxFTkRfREFSS0VOIn0sCgkJCQkJCX0pCg

  /// <summary>
  /// Represents OBS source type.
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<SourceType>))]
  public enum SourceType {
    /// <summary>
    /// Input source
    /// </summary>
    [EnumMember(Value = "OBS_SOURCE_TYPE_INPUT")]
    Input,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_SOURCE_TYPE_FILTER")]
    Filter,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_SOURCE_TYPE_TRANSITION")]
    Transition,
    /// <summary>
    /// Scene source
    /// </summary>
    [EnumMember(Value = "OBS_SOURCE_TYPE_SCENE")]
    Scene,
  }

  /// <summary>
  /// The desktop audio monitoring type.
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<MonitoringType>))]
  public enum MonitoringType {
    /// <summary>
    /// Do not monitor
    /// </summary>
    [EnumMember(Value = "OBS_MONITORING_TYPE_NONE")]
    None,
    /// <summary>
    /// Send to monitor device, no outputs
    /// </summary>
    [EnumMember(Value = "OBS_MONITORING_TYPE_MONITOR_ONLY")]
    Only,
    /// <summary>
    /// Send to monitor device and outputs
    /// </summary>
    [EnumMember(Value = "OBS_MONITORING_TYPE_MONITOR_AND_OUTPUT")]
    Output,
  }

  /// <summary>
  /// State of the media.
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<MediaState>))]
  public enum MediaState {
    /// <summary>
    /// Invalid or unsupported state.
    /// </summary>
    [EnumMember(Value = "OBS_MEDIA_STATE_NONE")]
    None,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_MEDIA_STATE_PLAYING")]
    Playing,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_MEDIA_STATE_OPENING")]
    Opening,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_MEDIA_STATE_BUFFERING")]
    Buffering,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_MEDIA_STATE_PAUSED")]
    Paused,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_MEDIA_STATE_STOPPED")]
    Stopped,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_MEDIA_STATE_ENDED")]
    Ended,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_MEDIA_STATE_ERROR")]
    Error,
  }

  /// <summary>
  /// The bounding box type of a scene item. Bounding boxes are used to
  /// stretch/position the source relative to a specific bounding box of a specific size.
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<BoundsType>))]
  public enum BoundsType {
    /// <summary>
    /// No bounding box
    /// </summary>
    [EnumMember(Value = "OBS_BOUNDS_NONE")]
    None,
    /// <summary>
    /// Stretch to the bounding box without preserving aspect ratio
    /// </summary>
    [EnumMember(Value = "OBS_BOUNDS_STRETCH")]
    Stretch,
    /// <summary>
    /// Scales with aspect ratio to inner bounding box rectangle
    /// </summary>
    [EnumMember(Value = "OBS_BOUNDS_SCALE_INNER")]
    Inner,
    /// <summary>
    /// Scales with aspect ratio to outer bounding box rectangle
    /// </summary>
    [EnumMember(Value = "OBS_BOUNDS_SCALE_OUTER")]
    Outer,
    /// <summary>
    /// Scales with aspect ratio to the bounding box width
    /// </summary>
    [EnumMember(Value = "OBS_BOUNDS_SCALE_TO_WIDTH")]
    Width,
    /// <summary>
    /// Scales with aspect ratio to the bounding box height
    /// </summary>
    [EnumMember(Value = "OBS_BOUNDS_SCALE_TO_HEIGHT")]
    Height,
    /// <summary>
    /// Scales with aspect ratio, but only to the size of the source maximum
    /// </summary>
    [EnumMember(Value = "OBS_BOUNDS_MAX_ONLY")]
    Only,
  }

  /// <summary>
  /// The blend mode of a scene item.
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<BlendingType>))]
  public enum BlendingType {
    /// <summary>
    /// Overlay.
    /// </summary>
    [EnumMember(Value = "OBS_BLEND_NORMAL")]
    Normal,
    /// <summary>
    /// Add RGB value.
    /// </summary>
    [EnumMember(Value = "OBS_BLEND_ADDITIVE")]
    Additive,
    /// <summary>
    /// Subtract RGB value.
    /// </summary>
    [EnumMember(Value = "OBS_BLEND_SUBTRACT")]
    Subtract,
    /// <summary>
    /// the values of the pixels in the two layers are inverted, multiplied, and then inverted again.
    /// The result is the opposite of Multiply: wherever either layer was darker than white,
    /// the composite is brighter.
    /// </summary>
    [EnumMember(Value = "OBS_BLEND_SCREEN")]
    Screen,
    /// <summary>
    /// takes the RGB channel values from 0 to 1 of each pixel in the top layer and
    /// multiples them with the values for the corresponding pixel from the bottom layer.
    /// </summary>
    [EnumMember(Value = "OBS_BLEND_MULTIPLY")]
    Multiply,
    /// <summary>
    /// It selects the maximum of each component from the foreground and background pixels.
    /// </summary>
    [EnumMember(Value = "OBS_BLEND_LIGHTEN")]
    Lighten,
    /// <summary>
    /// Use a pixel that retains the smallest components of the foreground and background pixels. 
    /// </summary>
    [EnumMember(Value = "OBS_BLEND_DARKEN")]
    Darken,
  }

  /// <summary>
  /// Area where data are stored.
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<DataRealm>))]
  public enum DataRealm {
    /// <summary>
    /// For global data.
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_DATA_REALM_GLOBAL")]
    Global,
    /// <summary>
    /// For data per profile.
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_DATA_REALM_PROFILE")]
    Profile,
  }

  /// <summary>
  /// Represents projector for a specific output video mix.
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<VideoMixType>))]
  public enum VideoMixType {
    /// <summary>
    /// Studio mode preview
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_VIDEO_MIX_TYPE_PREVIEW")]
    Preview,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_VIDEO_MIX_TYPE_PROGRAM")]
    Program,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "OBS_WEBSOCKET_VIDEO_MIX_TYPE_MULTIVIEW")]
    Multiview,
  }

  /// <summary>
  /// Type of streaming options
  /// </summary>
  [MessagePackFormatter(typeof(EnumAsStringFormatter<StreamServiceType>))]
  public enum StreamServiceType {
    /// <summary>
    /// </summary>
    [EnumMember(Value = "rtmp_common")]
    RtmpCommon,
    /// <summary>
    /// </summary>
    [EnumMember(Value = "rtmp_custom")]
    RtmpCustom,
  }
}
