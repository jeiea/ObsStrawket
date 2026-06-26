using System;

namespace ObsStrawket {

  /// <summary>
  /// Observable lifecycle phase of an OBS websocket connection.
  /// </summary>
  public enum ObsConnectionPhase {
    /// <summary>No active connection or connection attempt.</summary>
    Disconnected,
    /// <summary>A connect attempt is in progress.</summary>
    Connecting,
    /// <summary>The websocket is identified and ready to send requests.</summary>
    Connected,
    /// <summary>A local close operation is in progress.</summary>
    Closing,
    /// <summary>The connection attempt or active connection ended with a failure.</summary>
    Faulted,
  }

  /// <summary>
  /// Immutable snapshot of an OBS websocket connection lifecycle state.
  /// </summary>
  public sealed record class ObsConnectionState(
    ObsConnectionPhase Phase,
    Uri? Uri,
    Exception? Exception
  );

  /// <summary>
  /// Data for <see cref="ClientSocket.ConnectionStateChanged"/> and
  /// <see cref="ObsClientSocket.ConnectionStateChanged"/>.
  /// </summary>
  public sealed class ObsConnectionStateChangedEventArgs : EventArgs {
    /// <summary>Creates a connection state change notification.</summary>
    public ObsConnectionStateChangedEventArgs(
      ObsConnectionState oldState,
      ObsConnectionState newState
    ) {
      OldState = oldState;
      NewState = newState;
    }

    /// <summary>Connection state before the transition.</summary>
    public ObsConnectionState OldState { get; }

    /// <summary>Connection state after the transition.</summary>
    public ObsConnectionState NewState { get; }
  }
}
