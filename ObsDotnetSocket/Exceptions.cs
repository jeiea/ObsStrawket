namespace ObsDotnetSocket {
  using ObsDotnetSocket.DataTypes;
  using System;
  using System.Net.WebSockets;

  public class ObsWebSocketException : Exception {
    public ObsWebSocketException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
  }

  public class UnexpectedProtocolException : ObsWebSocketException {
    public UnexpectedProtocolException(string? message = null) : base(message ?? "It's not seem to be obs websocket message") { }
  }

  public class FailureResponseException : ObsWebSocketException {
    public IRequestResponse Response { get; set; }

    public FailureResponseException(IRequestResponse response) : base($"{response.RequestStatus.Code}: {response.RequestStatus.Comment}") {
      Response = response;
    }
  }

  public class AuthenticationFailureException : ObsWebSocketException {
    public AuthenticationFailureException(WebSocketCloseStatus? closeStatus, string closeStatusDescription) : base($"${closeStatus}: ${closeStatusDescription}") {
      CloseStatus = closeStatus;
      CloseStatusDescription = closeStatusDescription;
    }

    public WebSocketCloseStatus? CloseStatus { get; set; }

    public string CloseStatusDescription { get; set; } = "";
  }
}
