namespace ObsStrawket {
  using ObsStrawket.DataTypes;
  using System;

  public class ObsWebSocketException : Exception {
    public ObsWebSocketException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
  }

  public class UnexpectedProtocolException : ObsWebSocketException {
    public UnexpectedProtocolException(string? message = null) : base(message ?? "It's not seem to be obs websocket message") { }
  }

  public class QueueCancelledException : ObsWebSocketException {
    public QueueCancelledException(string message = "Connection closed before operation", Exception? innerException = null) : base(message, innerException) { }
  }

  public class FailureResponseException : ObsWebSocketException {
    public IRequestResponse Response { get; set; }

    public FailureResponseException(IRequestResponse response) : base($"{response.RequestStatus.Code}: {response.RequestStatus.Comment}") {
      Response = response;
    }
  }

  public class AuthenticationFailureException : ObsWebSocketException {
    public AuthenticationFailureException(string? message = null) : base(message ?? "OBS Authentication failure") { }
  }
}
