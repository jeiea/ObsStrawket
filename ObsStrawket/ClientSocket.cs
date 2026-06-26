using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {

  /// <summary>
  /// Low level client interface.
  /// </summary>
  public class ClientSocket : IDisposable {
    private const int _supportedRpcVersion = 1;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<IHasRequestId>> _pendingRequests = new();
    private readonly SemaphoreSlim _connectSemaphore = new(1);
    private readonly object _stateLock = new();
    private ClientWebSocket _clientWebSocket = new();
    private CancellationTokenSource? _cancellation;
    private ObsConnectionState _connectionState = new(ObsConnectionPhase.Disconnected, null, null);
    private int _pipelineEventLevel = (int)PipelineLevel.Warning;
    private Action<PipelineEvent>? _pipelineEvent;
    private SendPipeline _sender;
    private ReceivePipeline _receiver;
    private Channel<IObsEvent> _events = Channel.CreateUnbounded<IObsEvent>();

    /// <summary>
    /// Create low level OBS websocket client.
    /// </summary>
    public ClientSocket() {
      _sender = new(_clientWebSocket);
      _receiver = new(_clientWebSocket);
    }

    /// <summary>
    /// Diagnostic notifications emitted while processing the websocket pipeline.
    /// </summary>
    /// <remarks>
    /// Handlers may run concurrently from several threads and the emission order is not
    /// guaranteed; write them to be thread-safe. See <see cref="PipelineEvent"/>.
    /// </remarks>
    public event Action<PipelineEvent>? PipelineEvent {
      add {
        AddPipelineEventHandler(value);
        UpdatePipelineEmitter();
      }
      remove {
        RemovePipelineEventHandler(value);
        UpdatePipelineEmitter();
      }
    }

    /// <summary>
    /// Minimum severity emitted through <see cref="PipelineEvent"/>.
    /// </summary>
    public PipelineLevel PipelineEventLevel {
      get => (PipelineLevel)Volatile.Read(ref _pipelineEventLevel);
      set {
        Volatile.Write(ref _pipelineEventLevel, (int)value);
        UpdatePipelineEmitter();
      }
    }

    /// <summary>
    /// Fired whenever <see cref="ConnectionState"/> changes. Handler exceptions are reported
    /// through <see cref="PipelineEvent"/> and do not interrupt the websocket pipeline.
    /// </summary>
    /// <remarks>
    /// Handlers run synchronously during the state transition. Do not synchronously block on
    /// <see cref="ConnectAsync"/> or <see cref="CloseAsync"/> from a handler.
    /// </remarks>
    public event EventHandler<ObsConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Current lifecycle snapshot of the websocket connection.
    /// </summary>
    public ObsConnectionState ConnectionState => Volatile.Read(ref _connectionState);

    /// <summary>
    /// Whether it is connected to websocket server
    /// </summary>
    public bool IsConnected => ConnectionState.Phase == ObsConnectionPhase.Connected;

    /// <summary>
    /// It emits all of received OBS events.
    /// </summary>
    public ChannelReader<IObsEvent> Events => _events.Reader;

    /// <summary>
    /// Set addition websocket options.
    /// </summary>
    public Action<ClientWebSocket> SetOptions { get; set; } = static delegate { };

    internal static Uri DefaultUri => new("ws://127.0.0.1:4455");

    private Task MessageLoop { get; set; } = Task.CompletedTask;

    internal Task PipelineCompletion => Task.WhenAll(
      _receiver.ReceiveTask ?? Task.CompletedTask,
      _sender.SendTask ?? Task.CompletedTask,
      MessageLoop
    );

    /// <summary>
    ///  Connect to OBS websocket server.
    /// </summary>
    /// <param name="uri">Destination uri. Use <c>ws://localhost:4455</c> if null.</param>
    /// <param name="password">Password for handshake.</param>
    /// <param name="events">Event categories to subscribe.</param>
    /// <param name="cancellation">Token for cancellation.</param>
    /// <returns><see langword="true"/> when connected; <see langword="false"/> when OBS rejects authentication.</returns>
    /// <exception cref="OperationCanceledException">The caller cancels the operation.</exception>
    /// <exception cref="ObsConnectionException">The connection cannot be established.</exception>
    /// <exception cref="ObsProtocolException">OBS sends an invalid handshake response.</exception>
    public async Task<bool> ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken cancellation = default
    ) {
      cancellation.ThrowIfCancellationRequested();

      var url = uri ?? DefaultUri;
      await _connectSemaphore.WaitAsync(cancellation).ConfigureAwait(false);
      try {
        if (ConnectionState.Phase == ObsConnectionPhase.Connected) {
          try {
            await CloseInternalAsync().ConfigureAwait(false);
          }
          catch (Exception ex) {
            EmitPipelineEvent(new CloseExceptionIgnored(ex));
          }
        }

        SetConnectionState(ObsConnectionPhase.Connecting, url);
        _events = Channel.CreateUnbounded<IObsEvent>();
        _cancellation = new();
        _clientWebSocket = new ClientWebSocket();
        _clientWebSocket.Options.AddSubProtocol("obswebsocket.json");
        _sender = new(_clientWebSocket) { Emit = GetPipelineEmitter() };
        _receiver = new(_clientWebSocket) { Emit = GetPipelineEmitter() };
        var messages = _receiver.Messages;
        SetOptions(_clientWebSocket);

        await _clientWebSocket.ConnectAsync(url, cancellation).ConfigureAwait(false);

        _receiver.Run(_cancellation.Token);
        _sender.Start();
        var hello = await ReceiveMessageAsync(messages, cancellation).ConfigureAwait(false) as Hello
          ?? throw new ObsProtocolException("OBS did not send a Hello message during handshake.");
        if (hello.RpcVersion > _supportedRpcVersion) {
          EmitPipelineEvent(new UnsupportedRpcVersion(hello.RpcVersion, _supportedRpcVersion));
        }

        var identify = new Identify() {
          RpcVersion = _supportedRpcVersion,
          EventSubscriptions = events,
          Authentication = MakeOneTimePass(password, hello.Authentication),
        };
        await _sender.SendAsync(identify, cancellation).ConfigureAwait(false);

        var identified = await ReceiveMessageAsync(messages, cancellation).ConfigureAwait(false);
        if (identified is not Identified) {
          throw new ObsProtocolException($"Identified message expected, but received {identified}.");
        }

        MessageLoop = LoopReceiveAsync(messages, _cancellation.Token);
        SetConnectionState(ObsConnectionPhase.Connected, url);
        return true;
      }
      catch (Exception exception) when (GetChannelFailure(exception) is ObsAuthenticationException failure) {
        Reset();
        SetConnectionState(ObsConnectionPhase.Faulted, url, failure);
        return false;
      }
      catch (OperationCanceledException) when (cancellation.IsCancellationRequested) {
        Reset();
        SetConnectionState(ObsConnectionPhase.Disconnected, url);
        throw;
      }
      catch (Exception exception) {
        var failure = NormalizeConnectionFailure(
          exception,
          "Failed to connect to the OBS websocket server.");
        Reset(eventException: failure);
        SetConnectionState(ObsConnectionPhase.Faulted, url, failure);
        throw failure;
      }
      finally {
        _ = _connectSemaphore.Release();
      }
    }

    /// <summary>
    /// Low level request method. It can send <see cref="RawRequest"/>.
    /// </summary>
    /// <param name="request">Request data.</param>
    /// <param name="cancellation">Token for cancellation</param>
    /// <returns>Response from websocket server.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The client is not connected.</exception>
    /// <exception cref="OperationCanceledException">The caller cancels the operation.</exception>
    /// <exception cref="ObsRequestException">OBS rejects the request.</exception>
    /// <exception cref="ObsConnectionException">The connection fails before a response is received.</exception>
    /// <exception cref="ObsProtocolException">OBS sends an invalid response.</exception>
    public Task<IRequestResponse> RequestAsync(IRequest request, CancellationToken cancellation = default) {
      return RequestAsync<IRequestResponse>(request, cancellation);
    }

    /// <summary>
    /// Batch request method. It can send <see cref="RequestBatch"/>.
    /// </summary>
    /// <param name="batchRequest">Requests to batch.</param>
    /// <param name="cancellation">Token for cancellation.</param>
    /// <returns>Response from websocket server.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="batchRequest"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The client is not connected.</exception>
    /// <exception cref="OperationCanceledException">The caller cancels the operation.</exception>
    /// <exception cref="ObsConnectionException">The connection fails before a response is received.</exception>
    /// <exception cref="ObsProtocolException">OBS sends an invalid response.</exception>
    public Task<IRequestBatchResponse> RequestAsync(IRequestBatch batchRequest, CancellationToken cancellation = default) {
      return RequestAsync<IRequestBatchResponse>(batchRequest, cancellation);
    }

    private async Task<TResponse> RequestAsync<TResponse>(
      IHasRequestId request,
      CancellationToken cancellation
    ) where TResponse : class, IHasRequestId {
#pragma warning disable CA1510 // ArgumentNullException.ThrowIfNull is unavailable on netstandard2.0.
      if (request == null) {
        throw new ArgumentNullException(nameof(request));
      }
#pragma warning restore CA1510
      if (!IsConnected) {
        throw new InvalidOperationException("WebSocket is not connected");
      }

      using var operationCancellation = LinkInstanceCancellation(cancellation);
      operationCancellation.Token.ThrowIfCancellationRequested();
      request.RequestId = $"{Guid.NewGuid()}";
      var waiter = new TaskCompletionSource<IHasRequestId>(
        TaskCreationOptions.RunContinuationsAsynchronously
      );
      if (!_pendingRequests.TryAdd(request.RequestId, waiter)) {
        throw new InvalidOperationException($"Duplicate request id: {request.RequestId}");
      }

      using var registration = cancellation.Register(() => {
        _ = TryCompletePendingRequest(
          request.RequestId,
          pendingRequest => pendingRequest.TrySetCanceled(cancellation)
        );
      });

      try {
        try {
          await _sender.SendAsync(request, operationCancellation.Token).ConfigureAwait(false);
        }
        catch (Exception exception) {
          Exception failure = cancellation.IsCancellationRequested
            ? new OperationCanceledException(cancellation)
            : NormalizeConnectionFailure(
              exception,
              "Failed to send an OBS websocket request.");
          _ = TryCompletePendingRequest(
            request.RequestId,
            pendingRequest => failure is OperationCanceledException
              ? pendingRequest.TrySetCanceled(cancellation)
              : pendingRequest.TrySetException(failure)
          );
        }
        var response = await waiter.Task.ConfigureAwait(false);
        return response as TResponse
          ?? throw new ObsProtocolException(
            $"Expected {typeof(TResponse).Name}, but received {response.GetType().Name}."
          );
      }
      finally {
        _ = _pendingRequests.TryRemove(request.RequestId, out _);
      }
    }

    /// <summary>
    /// Close this connection. Pending requests fail with
    /// <see cref="ObsConnectionClosedException"/>.
    /// </summary>
    public async Task CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = "Client closed websocket", Exception? exception = null) {
      await _connectSemaphore.WaitAsync().ConfigureAwait(false);
      try {
        var connectionState = ConnectionState;
        if (connectionState.Phase == ObsConnectionPhase.Connected) {
          await CloseInternalAsync(status, description, exception).ConfigureAwait(false);
        }
        else if (connectionState.Phase == ObsConnectionPhase.Faulted) {
          SetConnectionState(ObsConnectionPhase.Disconnected, connectionState.Uri);
        }
      }
      finally {
        _ = _connectSemaphore.Release();
      }
    }

    /// <summary>
    /// Dispose this forever.
    /// </summary>
    public void Dispose() {
      var exception = new ObsConnectionClosedException("Socket disposed.");
      var connectionState = ConnectionState;
      if (connectionState.Phase == ObsConnectionPhase.Connected) {
        SetConnectionState(ObsConnectionPhase.Closing, connectionState.Uri);
      }
      Reset(exception, exception);
      SetConnectionState(ObsConnectionPhase.Disconnected, connectionState.Uri);
      _connectSemaphore.Dispose();
      GC.SuppressFinalize(this);
    }

    private static async Task<IOpCodeMessage> ReceiveMessageAsync(
      ChannelReader<Task<IOpCodeMessage>> receiver, CancellationToken cancellation
    ) {
      var deserialization = await receiver.ReadAsync(cancellation).ConfigureAwait(false);
      return await deserialization.ConfigureAwait(false);
    }

    private static string? MakeOneTimePass(string? password, HelloAuthentication? auth) {
      if (auth == null) {
        return null;
      }
      if (password == null) {
        throw new ObsAuthenticationException("OBS requested a password.");
      }
      string base64Secret = ApplySha256Base64($"{password}{auth.Salt}");
      return ApplySha256Base64($"{base64Secret}{auth.Challenge}");
    }

    private static string ApplySha256Base64(string rawData) {
      byte[] data = Encoding.UTF8.GetBytes(rawData);
#if NETSTANDARD2_0
      using var sha256Hash = SHA256.Create();
      byte[] bytes = sha256Hash.ComputeHash(data);
#else
      byte[] bytes = SHA256.HashData(data);
#endif
      return Convert.ToBase64String(bytes);
    }

    private CancellationTokenSource LinkInstanceCancellation(CancellationToken cancellation) {
      var instanceCancellation = _cancellation?.Token ?? CancellationToken.None;
      return CancellationTokenSource.CreateLinkedTokenSource(cancellation, instanceCancellation);
    }

    private async Task CloseInternalAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure, string? description = null, Exception? exception = null) {
      var connectionState = ConnectionState;
      if (connectionState.Phase != ObsConnectionPhase.Connected) {
        return;
      }

      var uri = connectionState.Uri;
      if (exception == null) {
        SetConnectionState(ObsConnectionPhase.Closing, uri);
      }

      try {
        var socket = _clientWebSocket;
        if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived) {
          await socket.CloseOutputAsync(status, description, default).ConfigureAwait(false);
        }
      }
      catch (OperationCanceledException) { }
      catch (WebSocketException) {
        // The connection may die between the state check and the close output send.
      }
      finally {
        // Without this, pending requests outlive the connection and their awaiters hang.
        var pendingException = exception
          ?? new ObsConnectionClosedException(description ?? "Client closed the websocket connection.");
        Reset(pendingException, exception);
        SetConnectionState(
          exception == null ? ObsConnectionPhase.Disconnected : ObsConnectionPhase.Faulted,
          uri,
          exception);
      }
    }

    private async Task LoopReceiveAsync(ChannelReader<Task<IOpCodeMessage>> messages, CancellationToken cancellation = default) {
      var token = cancellation;

      try {
        var events = _events.Writer;

        while (_clientWebSocket.State != WebSocketState.Closed && !token.IsCancellationRequested) {
          bool isAvailable = await messages.WaitToReadAsync(default).ConfigureAwait(false);
          if (!isAvailable) {
            break;
          }
          var message = await ReceiveMessageAsync(messages, default).ConfigureAwait(false);
          if (message == null) {
            break;
          }

          await DispatchAsync(message, events, token).ConfigureAwait(false);
        }
        if (!token.IsCancellationRequested) {
          await CloseAsync(exception: new ObsConnectionClosedException(
            "OBS closed the websocket connection.")).ConfigureAwait(false);
        }
      }
      catch (Exception exception) {
        var failure = NormalizeConnectionFailure(
          exception,
          "The OBS websocket connection terminated.");
        await CloseAsync(exception: failure).ConfigureAwait(false);
      }
    }

    private async Task DispatchAsync(
      IOpCodeMessage message, ChannelWriter<IObsEvent> events, CancellationToken token
    ) {
      switch (message) {
      case IObsEvent ev:
        if (ev is RawEvent rawEvent && CanEmitPipelineEvent(PipelineLevel.Warning)) {
          EmitPipelineEvent(new RawEventReceived(JsonSerializer.Serialize(rawEvent)));
        }
        await events.WriteAsync(ev, token).ConfigureAwait(false);
        break;

      case IHasRequestId response:
        if (response is RawRequestResponse rawResponse && CanEmitPipelineEvent(PipelineLevel.Warning)) {
          EmitPipelineEvent(new RawResponseReceived(JsonSerializer.Serialize(rawResponse)));
        }
        bool responseMatched = TryCompletePendingRequest(
          response.RequestId,
          request => response is IRequestResponse { RequestStatus.Result: false } failed
            ? request.TrySetException(new ObsRequestException(failed))
            : request.TrySetResult(response)
        );
        if (!responseMatched) {
          EmitPipelineEvent(response is IRequestBatchResponse
            ? new OrphanBatchResponse(response.RequestId)
            : new OrphanResponse(response.RequestId));
        }
        break;

      default:
        EmitPipelineEvent(new UnknownMessageType(message.GetType()));
        break;
      }
    }

    private void Reset(Exception? pendingException = null, Exception? eventException = null) {
      _sender.Stop();

      _cancellation?.Cancel();
      _cancellation?.Dispose();
      _cancellation = null;

      CompletePendingRequests(pendingException);
      _ = _events.Writer.TryComplete(eventException);

      _clientWebSocket.Dispose();
    }

    private void SetConnectionState(
      ObsConnectionPhase phase,
      Uri? uri = null,
      Exception? exception = null
    ) {
      EventHandler<ObsConnectionStateChangedEventArgs>? handler;
      ObsConnectionStateChangedEventArgs args;

      lock (_stateLock) {
        var oldState = _connectionState;
        var newState = new ObsConnectionState(
          phase,
          uri ?? oldState.Uri,
          phase == ObsConnectionPhase.Faulted ? exception : null
        );
        if (oldState.Phase == newState.Phase) {
          _connectionState = newState;
          return;
        }

        _connectionState = newState;
        handler = ConnectionStateChanged;
        args = new(oldState, newState);
      }

      try {
        handler?.Invoke(this, args);
      }
      catch (Exception handlerException) {
        EmitPipelineEvent(new EventHandlerFaulted(
          EventHandlerKind.ConnectionStateChanged,
          null,
          handlerException));
      }
    }

    private bool TryCompletePendingRequest(
      string requestId,
      Func<TaskCompletionSource<IHasRequestId>, bool> complete
    ) {
      return _pendingRequests.TryRemove(requestId, out var pendingRequest)
        && complete(pendingRequest);
    }

    private void CompletePendingRequests(Exception? exception) {
      foreach (string requestId in _pendingRequests.Keys) {
        _ = TryCompletePendingRequest(
          requestId,
          request => exception == null
            ? request.TrySetCanceled()
            : request.TrySetException(exception)
        );
      }
    }

    private static Exception? GetChannelFailure(Exception exception) {
      return exception is ChannelClosedException { InnerException: { } inner }
        ? inner
        : exception;
    }

    private static ObsWebSocketException NormalizeConnectionFailure(
      Exception exception,
      string message
    ) {
      var failure = GetChannelFailure(exception);
      return failure switch {
        ObsWebSocketException obsException => obsException,
        _ => new ObsConnectionException(message, failure),
      };
    }

    private void AddPipelineEventHandler(Action<PipelineEvent>? handler) {
      if (handler == null) {
        return;
      }

      Action<PipelineEvent>? previous;
      Action<PipelineEvent> next;
      do {
        previous = _pipelineEvent;
        next = (Action<PipelineEvent>)Delegate.Combine(previous, handler);
      } while (Interlocked.CompareExchange(ref _pipelineEvent, next, previous) != previous);
    }

    private void RemovePipelineEventHandler(Action<PipelineEvent>? handler) {
      if (handler == null) {
        return;
      }

      Action<PipelineEvent>? previous;
      Action<PipelineEvent>? next;
      do {
        previous = _pipelineEvent;
        next = (Action<PipelineEvent>?)Delegate.Remove(previous, handler);
      } while (Interlocked.CompareExchange(ref _pipelineEvent, next, previous) != previous);
    }

    private Action<PipelineEvent>? GetPipelineEmitter() {
      return CanEmitPipelineEvent(PipelineLevel.Info) ? EmitPipelineEvent : null;
    }

    private void UpdatePipelineEmitter() {
      var emitter = GetPipelineEmitter();
      _sender.Emit = emitter;
      _receiver.Emit = emitter;
    }

    private void EmitPipelineEvent(PipelineEvent diagnostic) {
      if (!ShouldEmitPipelineEvent(diagnostic.Level)) {
        return;
      }

      var handler = _pipelineEvent;
      if (handler == null) {
        return;
      }

      try {
        handler(diagnostic);
      }
      catch {
        // Diagnostic handler failures must not affect the websocket pipeline.
      }
    }

    internal bool ShouldEmitPipelineEvent(PipelineLevel level) {
      return level >= PipelineEventLevel;
    }

    private bool CanEmitPipelineEvent(PipelineLevel level) {
      return _pipelineEvent != null && ShouldEmitPipelineEvent(level);
    }
  }
}
