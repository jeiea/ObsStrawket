using ObsStrawket.DataTypes;
using System;

namespace ObsStrawket {
  /// <summary>
  /// Base type for failures reported by ObsStrawket.
  /// </summary>
  public class ObsWebSocketException : Exception {
    internal ObsWebSocketException(string message, Exception? innerException = null)
      : base(message, innerException) { }
  }

  /// <summary>
  /// Failed to establish or use the websocket connection.
  /// </summary>
  public class ObsConnectionException : ObsWebSocketException {
    internal ObsConnectionException(string message, Exception? innerException = null)
      : base(message, innerException) { }
  }

  /// <summary>
  /// OBS rejected the supplied authentication credentials.
  /// </summary>
  public sealed class ObsAuthenticationException : ObsConnectionException {
    internal ObsAuthenticationException(string message = "OBS authentication failed.", Exception? innerException = null)
      : base(message, innerException) { }
  }

  /// <summary>
  /// The websocket connection closed before the operation completed.
  /// </summary>
  public sealed class ObsConnectionClosedException : ObsConnectionException {
    internal ObsConnectionClosedException(
      string message,
      int? closeCode = null,
      string? closeDescription = null,
      Exception? innerException = null
    ) : base(message, innerException) {
      CloseCode = closeCode;
      CloseDescription = closeDescription;
    }

    /// <summary>
    /// Websocket close status code, if one was received.
    /// </summary>
    public int? CloseCode { get; }

    /// <summary>
    /// Websocket close description, if one was received.
    /// </summary>
    public string? CloseDescription { get; }
  }

  /// <summary>
  /// OBS sent data that does not conform to the negotiated protocol.
  /// </summary>
  public sealed class ObsProtocolException : ObsWebSocketException {
    internal ObsProtocolException(string message, Exception? innerException = null)
      : base(message, innerException) { }
  }

  /// <summary>
  /// OBS processed a request and rejected it.
  /// </summary>
  public sealed class ObsRequestException : ObsWebSocketException {
    internal ObsRequestException(IRequestResponse response)
      : base($"{response.RequestStatus.Code}: {response.RequestStatus.Comment}") {
      Response = response;
    }

    /// <summary>
    /// Response returned by OBS.
    /// </summary>
    public IRequestResponse Response { get; }
  }
}
