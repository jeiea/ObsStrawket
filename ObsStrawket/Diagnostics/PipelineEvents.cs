using System;

namespace ObsStrawket.Diagnostics {

  /// <summary>
  /// The server reports an RPC version newer than the one this client supports.
  /// </summary>
  public sealed class UnsupportedRpcVersion : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public UnsupportedRpcVersion(int serverVersion, int supportedVersion) {
      ServerVersion = serverVersion;
      SupportedVersion = supportedVersion;
    }

    /// <summary>RPC version advertised by the server.</summary>
    public int ServerVersion { get; }

    /// <summary>Highest RPC version this client supports.</summary>
    public int SupportedVersion { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Warning;

    /// <inheritdoc/>
    public override string Message =>
      $"OBS RPC version({ServerVersion}) is newer than supported version({SupportedVersion}).";
  }

  /// <summary>
  /// An exception raised while closing a previous connection was ignored to let reconnection proceed.
  /// </summary>
  public sealed class CloseExceptionIgnored : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public CloseExceptionIgnored(Exception exception) {
      Exception = exception;
    }

    /// <inheritdoc/>
    public override Exception Exception { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Warning;

    /// <inheritdoc/>
    public override string Message => "Ignored exception while closing the previous connection.";
  }

  /// <summary>
  /// An event arrived that could not be matched to a known type.
  /// </summary>
  public sealed class RawEventReceived : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public RawEventReceived(string json) {
      Json = json;
    }

    /// <summary>Raw JSON payload of the event.</summary>
    public string Json { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Warning;

    /// <inheritdoc/>
    public override string Message => $"Received unclassified raw event: {Json}";
  }

  /// <summary>
  /// A response arrived that could not be matched to a known type.
  /// </summary>
  public sealed class RawResponseReceived : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public RawResponseReceived(string json) {
      Json = json;
    }

    /// <summary>Raw JSON payload of the response.</summary>
    public string Json { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Warning;

    /// <inheritdoc/>
    public override string Message => $"Received unclassified raw response: {Json}";
  }

  /// <summary>
  /// A response referenced a request id that has no pending waiter.
  /// </summary>
  public sealed class OrphanResponse : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public OrphanResponse(string requestId) {
      RequestId = requestId;
    }

    /// <summary>Request id carried by the response.</summary>
    public string RequestId { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Warning;

    /// <inheritdoc/>
    public override string Message => $"Failed to match response to a pending request: {RequestId}";
  }

  /// <summary>
  /// A batch response referenced a request id that has no pending waiter.
  /// </summary>
  public sealed class OrphanBatchResponse : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public OrphanBatchResponse(string requestId) {
      RequestId = requestId;
    }

    /// <summary>Request id carried by the batch response.</summary>
    public string RequestId { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Warning;

    /// <inheritdoc/>
    public override string Message => $"Failed to match batch response to a pending request: {RequestId}";
  }

  /// <summary>
  /// A message arrived whose op code maps to no known category.
  /// </summary>
  public sealed class UnknownMessageType : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public UnknownMessageType(Type messageType) {
      MessageType = messageType;
    }

    /// <summary>CLR type of the unhandled message.</summary>
    public Type MessageType { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Warning;

    /// <inheritdoc/>
    public override string Message => $"Unknown message type: {MessageType}";
  }

  /// <summary>
  /// A classified event had no specific dispatch path and was ignored.
  /// </summary>
  public sealed class UnclassifiedEventIgnored : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public UnclassifiedEventIgnored(string? eventType) {
      EventType = eventType;
    }

    /// <summary>The <c>eventType</c> string, if known.</summary>
    public string? EventType { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Warning;

    /// <inheritdoc/>
    public override string Message => $"Ignored unclassified event: {EventType}";
  }

  /// <summary>
  /// A consumer event handler threw. The exception is swallowed to keep the dispatch pump alive,
  /// so this event is the only signal that the handler failed.
  /// </summary>
  public sealed class EventHandlerFaulted : PipelineEvent {
    /// <summary>Creates the event.</summary>
    public EventHandlerFaulted(EventHandlerKind kind, string? eventType, Exception exception) {
      Kind = kind;
      EventType = eventType;
      Exception = exception;
    }

    /// <summary>Which callback threw.</summary>
    public EventHandlerKind Kind { get; }

    /// <summary>The <c>eventType</c> being dispatched, if applicable.</summary>
    public string? EventType { get; }

    /// <inheritdoc/>
    public override Exception Exception { get; }

    /// <inheritdoc/>
    public override PipelineLevel Level => PipelineLevel.Error;

    /// <inheritdoc/>
    public override string Message => EventType is null
      ? $"{Kind} event handler threw."
      : $"{Kind} event handler threw while dispatching {EventType}.";
  }
}
