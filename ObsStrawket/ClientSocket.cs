using Microsoft.Extensions.Logging;
using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {
  public class ClientSocket : IDisposable {
    internal static Uri DefaultUri => new("ws://127.0.0.1:4455");

    private const int _supportedRpcVersion = 1;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<IRequestResponse>> _requests = new();
    private readonly SemaphoreSlim _writeSemaphore = new(1);
    private readonly ILogger? _logger;

    private ClientWebSocket _clientWebSocket = new();
    private CancellationTokenSource _cancellation = new();
    private bool _isOpen = false;
    private SendPipeline _sender;
    private ReceivePipeline _receiver;
    private Channel<IObsEvent> _events = Channel.CreateUnbounded<IObsEvent>();
    private Task? _receiveLoop;

    public bool IsConnected => _clientWebSocket.State == WebSocketState.Open;

    public ChannelReader<IObsEvent> Events => _events.Reader;

    public Action<ClientWebSocket> SetOptions { get; set; } = delegate { };

    public ClientSocket(ILogger? logger = null) {
      _logger = logger;
      _sender = new(_clientWebSocket);
      _receiver = new(_clientWebSocket);
    }

    public async Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken cancellation = default
    ) {
      cancellation.ThrowIfCancellationRequested();

      var url = uri ?? DefaultUri;
      _logger?.LogInformation("ConnectAsync to {}.", url);
      await _writeSemaphore.WaitAsync(cancellation).ConfigureAwait(false);
      try {
        if (_isOpen) {
          try {
            await CloseInternalAsync(exception: new ObsWebSocketException($"ConnectAsync to {url}")).ConfigureAwait(false);
          }
          catch (Exception ex) {
            _logger?.LogWarning("Ignore close exception: {}", ex);
          }
        }

        _events = Channel.CreateUnbounded<IObsEvent>();
        _cancellation = new();
        _clientWebSocket = new ClientWebSocket();
        _clientWebSocket.Options.AddSubProtocol("obswebsocket.msgpack");
        _sender = new(_clientWebSocket, _logger);
        _receiver = new(_clientWebSocket, _logger);
        SetOptions(_clientWebSocket);

        await _clientWebSocket.ConnectAsync(url, cancellation).ConfigureAwait(false);
        _isOpen = true;

        _receiver.Run(_cancellation.Token);
        _sender.Start();
        var hello = (Hello)(await _receiver.Messages.ReadAsync(cancellation).ConfigureAwait(false))!;
        if (hello == null) {
          throw new ObsWebSocketException(GetCloseMessage() ?? "Handshake failure");
        }
        if (hello.RpcVersion > _supportedRpcVersion) {
          _logger?.LogWarning("OBS RPC version({hello}) is newer than supported version({supported}).", hello.RpcVersion, _supportedRpcVersion);
        }

        var identify = new Identify() {
          RpcVersion = _supportedRpcVersion,
          EventSubscriptions = events,
          Authentication = MakeOneTimePass(password, hello.Authentication),
        };
        await _sender.SendAsync(identify, cancellation).ConfigureAwait(false);

        if (await _receiver.Messages.ReadAsync(cancellation).ConfigureAwait(false) is not Identified identified) {
          throw new AuthenticationFailureException(GetCloseMessage());
        }

        _receiveLoop = LoopReceiveAsync(_cancellation.Token);
        _logger?.LogInformation("ConnectAsync to {} complete.", url);
      }
      finally {
        _writeSemaphore.Release();
      }
    }

    public async Task<IRequestResponse> RequestAsync(IRequest request, CancellationToken cancellation = default) {
      if (!IsConnected) {
        throw new InvalidOperationException("WebSocket is not connected");
      }

      using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellation, _cancellation.Token);
      var token = source.Token;
      token.ThrowIfCancellationRequested();

      string guid = $"{Guid.NewGuid()}";
      request.RequestId = guid;
      _logger?.LogInformation("RequestAsync {} start.", request.GetType().Name);

      var waiter = new TaskCompletionSource<IRequestResponse>();
      _requests[guid] = waiter;

      await SendSafeAsync(request, token).ConfigureAwait(false);
      var response = await waiter.Task.ConfigureAwait(false);

      _logger?.LogInformation("RequestAsync {} finished.", request.GetType().Name);
      return response;
    }

    public async Task CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = "Client closed websocket", Exception? exception = null) {
      _logger?.LogInformation("CloseAsync exception: {}", exception?.Message);
      await _writeSemaphore.WaitAsync().ConfigureAwait(false);
      try {
        await CloseInternalAsync(status, description, exception).ConfigureAwait(false);
        _logger?.LogInformation("CloseAsync complete.");
      }
      finally {
        _writeSemaphore.Release();
      }
    }

    public void Dispose() {
      Reset(new ObsWebSocketException("Socket disposed"));
      _writeSemaphore.Dispose();
      GC.SuppressFinalize(this);
    }

    private async Task SendSafeAsync(IRequest request, CancellationToken token) {
      await _writeSemaphore.WaitAsync(token).ConfigureAwait(false);
      try {
        await _sender.SendAsync(request, token).ConfigureAwait(false);
      }
      finally {
        _writeSemaphore.Release();
      }
    }

    private async Task CloseInternalAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = null, Exception? exception = null) {
      if (!_isOpen) {
        return;
      }

      try {
        var socket = _clientWebSocket;
        if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived) {
          await socket.CloseOutputAsync(status, description, default).ConfigureAwait(false);
        }
        Reset(exception ?? new QueueCancelledException("User closed socket"));
      }
      catch (OperationCanceledException) { }
      finally {
        _isOpen = false;
      }
    }

    private async Task LoopReceiveAsync(CancellationToken cancellation = default) {
      using var _1 = _logger?.BeginScope(nameof(LoopReceiveAsync));
      var token = cancellation;

      try {
        var receiver = _receiver;
        var events = _events.Writer;

        while (_clientWebSocket.State != WebSocketState.Closed && !token.IsCancellationRequested) {
          bool isAvailable = await receiver.Messages.WaitToReadAsync(default).ConfigureAwait(false);
          if (!isAvailable) {
            break;
          }
          var message = await receiver.Messages.ReadAsync(default).ConfigureAwait(false);
          _logger?.LogDebug("Received message: {}", message?.GetType().Name ?? "null");
          if (message == null) {
            break;
          }

          await DispatchAsync(message, events, token).ConfigureAwait(false);
        }
        _logger?.LogDebug("Close. webSocket.State: {}, cancellation: {}",
          _clientWebSocket.State, token.IsCancellationRequested);
        await CloseAsync(exception: new QueueCancelledException("Server closed socket"));
      }
      catch (Exception exception) {
        await CloseAsync(exception: new QueueCancelledException(innerException: exception)).ConfigureAwait(false);
        _logger?.LogDebug(exception, "Queue cancelled");
      }
      _logger?.LogDebug("Exit. IsCancellationRequested: {}", token.IsCancellationRequested);
    }

    private async Task DispatchAsync(
      IOpCodeMessage message, ChannelWriter<IObsEvent> events, CancellationToken token
    ) {
      switch (message) {
      case IObsEvent ev:
        await events.WriteAsync(ev, token).ConfigureAwait(false);
        break;
      case RequestResponse response:
        if (_requests.TryRemove(response.RequestId, out var request)) {
          if (response.RequestStatus.Result) {
            request.SetResult(response);
          }
          else {
            request.SetException(new FailureResponseException(response));
          }
        }
        else {
          _logger?.LogWarning("Dispatch: Failed to remove completed task");
        }
        break;
      default:
        _logger?.LogWarning("Dispatch: Unknown message type: {}", message.GetType());
        break;
      }
    }

    private void Reset(Exception? exception = null) {
      _sender.Stop();

      _cancellation.Cancel();
      _cancellation.Dispose();

      if (exception != null) {
        foreach (var request in _requests.Values) {
          request.TrySetException(exception);
        }
      }
      else {
        foreach (var request in _requests.Values) {
          request.TrySetCanceled();
        }
      }
      _requests.Clear();
      _events.Writer.TryComplete(exception);

      _clientWebSocket.Dispose();
    }

    private string? GetCloseMessage() {
      if (_clientWebSocket.CloseStatus == null) {
        return null;
      }
      return $"{_clientWebSocket.CloseStatus}: {_clientWebSocket.CloseStatusDescription}";
    }

    private static string? MakeOneTimePass(string? password, HelloAuthentication? auth) {
      if (auth == null) {
        return null;
      }
      if (password == null) {
        throw new ObsWebSocketException("Password requested.");
      }
      string base64Secret = ApplySha256Base64($"{password}{auth.Salt}");
      return ApplySha256Base64($"{base64Secret}{auth.Challenge}");
    }

    private static string ApplySha256Base64(string rawData) {
      using var sha256Hash = SHA256.Create();
      byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
      return Convert.ToBase64String(bytes);
    }
  }
}
