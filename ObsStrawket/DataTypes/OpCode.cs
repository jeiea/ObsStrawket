namespace ObsStrawket.DataTypes {
  /// <summary>
  /// OBS websocket protocol message types
  /// </summary>
  public enum OpCode {
    /// <summary>
    /// <see cref="DataTypes.Hello"/>
    /// </summary>
    Hello = 0,
    /// <summary>
    /// <see cref="DataTypes.Identify"/>
    /// </summary>
    Identify = 1,
    /// <summary>
    /// <see cref="DataTypes.Identified"/>
    /// </summary>
    Identified = 2,
    /// <summary>
    /// <see cref="DataTypes.Reidentify"/>
    /// </summary>
    Reidentify = 3,
    /// <summary>
    /// <see cref="ObsEvent"/>
    /// </summary>
    Event = 5,
    /// <summary>
    /// <see cref="DataTypes.Request"/>
    /// </summary>
    Request = 6,
    /// <summary>
    /// <see cref="DataTypes.RequestResponse"/>
    /// </summary>
    RequestResponse = 7,
    /// <summary>
    /// TBD
    /// </summary>
    RequestBatch = 8,
    /// <summary>
    /// TBD
    /// </summary>
    RequestBatchResponse = 9,
  }
}
