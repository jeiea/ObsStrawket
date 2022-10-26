using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using System;

namespace ObsStrawket {
  /// <summary>
  /// All library exceptions derive this unless it is thrown from more under the hood.
  /// </summary>
  public class ObsWebSocketException : Exception {
    /// <summary>
    /// 
    /// </summary>
    internal ObsWebSocketException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
  }

  /// <summary>
  /// Found unexpected byte during deserialization.
  /// </summary>
  public class UnexpectedResponseException : ObsWebSocketException {
    internal UnexpectedResponseException(string? message = null) : base(message ?? "It's not seem to be obs websocket message") { }
  }

  /// <summary>
  /// Request is cancelled due to premature socket close, etc.
  /// </summary>
  public class QueueCancelledException : ObsWebSocketException {
    internal QueueCancelledException(string message = "Connection closed before operation", Exception? innerException = null) : base(message, innerException) { }
  }

  /// <summary>
  /// Received unsuccessful response.
  /// </summary>
  public class FailureResponseException : ObsWebSocketException {
    /// <summary>
    /// Matching OBS response.
    /// </summary>
    public IRequestResponse Response { get; set; }

    internal FailureResponseException(IRequestResponse response) : base($"{response.RequestStatus.Code}: {response.RequestStatus.Comment}") {
      Response = response;
    }
  }

  /// <summary>
  /// OBS closed the connection.
  /// </summary>
  public class WebsocketCloseReceivedException : ObsWebSocketException {
    /// <summary>
    /// Websocket close code. See <see cref="WebSocketCloseCode"/>
    /// </summary>
    public readonly int? Code = null;

    internal WebsocketCloseReceivedException(string? message = null, int? code = null, Exception? innerException = null)
      : base(message ?? $"OBS closed connection: {code}", innerException) {
      Code = code;
    }
  }

  /// <summary>
  /// Failed to authenticate.
  /// </summary>
  public class AuthenticationFailureException : WebsocketCloseReceivedException {
    internal AuthenticationFailureException(string? message = null, Exception? innerException = null)
      : base(message ?? "OBS Authentication failure", (int)WebSocketCloseCode.AuthenticationFailed, innerException: innerException) { }
  }
}
