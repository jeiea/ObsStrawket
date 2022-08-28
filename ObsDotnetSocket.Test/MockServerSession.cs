namespace ObsDotnetSocket.Test {
  using MessagePack;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Net.WebSockets;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Tasks;
  using Xunit;

  class MockServerSession : IDisposable {
    private readonly ArraySegment<byte> _buffer = new(new byte[1024]);
    private readonly WebSocket _webSocket;
    private readonly CancellationToken _cancellation;
    private bool _isDisposed;

    public MockServerSession(WebSocket webSocket, CancellationToken cancellation) {
      _webSocket = webSocket;
      _cancellation = cancellation;
    }

    public Task SendAsync(string json) {
      _cancellation.ThrowIfCancellationRequested();
      var binary = WebSocketMessageType.Binary;
      byte[] buffer = MessagePackSerializer.ConvertFromJson(json, cancellationToken: _cancellation);
      _ = Task.Run(() => System.Diagnostics.Debug.WriteLine($"Mock send {Regex.Replace(json, @"\s+", "")}"));
      return _webSocket.SendAsync(new ArraySegment<byte>(buffer), binary, true, _cancellation);
    }

    public async Task<string?> ReceiveAsync(string expectedJson) {
      var result = await _webSocket.ReceiveAsync(_buffer, _cancellation).ConfigureAwait(false);
      _cancellation.ThrowIfCancellationRequested();
      string json = MessagePackSerializer.ConvertToJson(new ArraySegment<byte>(_buffer.Array!, 0, result.Count), cancellationToken: _cancellation);
      _ = Task.Run(() => System.Diagnostics.Debug.WriteLine($"Mock receive {Regex.Replace(json, @"\s+", "")}"));
      string? guid = Regex.Match(json, @"[0-9a-f]{8}-[0-9a-f]{4}[^""]*")?.Value;
      AssertJsonEqual(expectedJson.Replace("{guid}", guid ?? ""), json);
      return guid;
    }

    public void Dispose() {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          // dispose managed state (managed objects)
          _webSocket.Dispose();
        }

        // free unmanaged resources (unmanaged objects) and override finalizer
        _isDisposed = true;
      }
    }
    private static void AssertJsonEqual(string expected, string actual) {
      var expectation = JToken.Parse(expected);
      var actualJson = JToken.Parse(actual);
      Assert.True(JToken.DeepEquals(expectation, actualJson), $"JSON expected: {expectation}, actual: {actualJson}");
    }
  }
}
