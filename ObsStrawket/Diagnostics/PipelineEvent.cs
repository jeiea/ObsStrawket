using System;

namespace ObsStrawket.Diagnostics {

  /// <summary>
  /// Severity of a <see cref="PipelineEvent"/>. Values increase with severity, so
  /// consumers can filter with comparisons such as <c>e.Level &gt;= PipelineLevel.Warning</c>.
  /// </summary>
  public enum PipelineLevel {
    /// <summary>Verbose progress trace, useful only while debugging the library.</summary>
    Debug = 0,
    /// <summary>Normal lifecycle milestone (connect, request, close).</summary>
    Info = 1,
    /// <summary>Unexpected but recoverable situation.</summary>
    Warning = 2,
    /// <summary>A failure that would otherwise be swallowed with no other propagation path.</summary>
    Error = 3,
  }

  /// <summary>
  /// Which consumer callback threw, for <see cref="EventHandlerFaulted"/>.
  /// </summary>
  public enum EventHandlerKind {
    /// <summary><see cref="ClientSocket.ConnectionStateChanged"/> handler.</summary>
    ConnectionStateChanged,
    /// <summary><see cref="ObsClientSocket.Event"/> handler.</summary>
    Event,
    /// <summary>A type-specific event handler.</summary>
    SpecificEvent,
  }

  /// <summary>
  /// A diagnostic notification emitted by the client while processing the websocket pipeline.
  /// Replaces the former <c>ILogger</c> dependency.
  /// </summary>
  /// <remarks>
  /// Thread safety: events are emitted from several threads (the caller, the receive loop, the
  /// dispatch pump, and the send pipeline). Handlers may therefore run concurrently and the
  /// emission order is not guaranteed. Write handlers to be thread-safe.
  /// </remarks>
  public abstract class PipelineEvent {
    /// <summary>Severity used for filtering.</summary>
    public abstract PipelineLevel Level { get; }

    /// <summary>Human readable description.</summary>
    public abstract string Message { get; }

    /// <summary>Associated exception, when the event carries one.</summary>
    public virtual Exception? Exception => null;

    /// <summary>Renders the level, message, and exception (if any) as one string.</summary>
    public override string ToString() {
      return Exception is null ? $"[{Level}] {Message}" : $"[{Level}] {Message}\n{Exception}";
    }
  }
}
