namespace ObsDotnetSocket.WebSocket {
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  public interface IWebSocket : IDisposable {
    public Task SendAsync(
      ReadOnlyMemory<byte> memory,
      MessageType type,
      bool isContinued = false,
      CancellationToken cancellation = default
    );

    public Task<WebSocketReceival> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellation = default);

    public Task CloseAsync(int? code = null, string? reason = null, CancellationToken cancellation = default);
  }

  public interface IClientWebSocket : IWebSocket {
    public IClientWebSocketOptions Options { get; }

    public Task ConnectAsync(Uri uri, CancellationToken cancellation = default);
  }

  public interface IClientWebSocketOptions {
    public void AddSubProtocol(string subProtocol);
  }

  public record class WebSocketReceival(
    int Count, MessageType MessageType, bool IsContinued = false,
    int? CloseStatus = null, string? CloseStatusDescription = null
  ) { }

  public enum MessageType {
    Text = 0,
    Binary = 1,
    Close = 2,
  }
}
