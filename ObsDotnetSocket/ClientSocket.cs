namespace ObsDotnetSocket {
  using ObsDotnetSocket.DataTypes;
  using ObsDotnetSocket.DataTypes.Predefineds;
  using System;
  using System.Collections.Concurrent;
  using System.Net.WebSockets;
  using System.Security.Cryptography;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  public class ClientSocket : IDisposable {
    #region Events
    public event Action<IEvent> Event = delegate { };

    public event Action<GeneralEvent> GeneralEvent = delegate { };
    public event Action<ExitStarted> ExitStarted = delegate { };
    public event Action<VendorEvent> VendorEvent = delegate { };

    public event Action<ConfigEvent> ConfigEvent = delegate { };
    public event Action<CurrentSceneCollectionChanging> CurrentSceneCollectionChanging = delegate { };
    public event Action<CurrentSceneCollectionChanged> CurrentSceneCollectionChanged = delegate { };
    public event Action<SceneCollectionListChanged> SceneCollectionListChanged = delegate { };
    public event Action<CurrentProfileChanging> CurrentProfileChanging = delegate { };
    public event Action<CurrentProfileChanged> CurrentProfileChanged = delegate { };
    public event Action<ProfileListChanged> ProfileListChanged = delegate { };

    public event Action<ScenesEvent> ScenesEvent = delegate { };
    public event Action<SceneCreated> SceneCreated = delegate { };
    public event Action<SceneRemoved> SceneRemoved = delegate { };
    public event Action<SceneNameChanged> SceneNameChanged = delegate { };
    public event Action<CurrentProgramSceneChanged> CurrentProgramSceneChanged = delegate { };
    public event Action<CurrentPreviewSceneChanged> CurrentPreviewSceneChanged = delegate { };
    public event Action<SceneListChanged> SceneListChanged = delegate { };

    public event Action<InputsEvent> InputsEvent = delegate { };
    public event Action<InputCreated> InputCreated = delegate { };
    public event Action<InputRemoved> InputRemoved = delegate { };
    public event Action<InputNameChanged> InputNameChanged = delegate { };
    public event Action<InputActiveStateChanged> InputActiveStateChanged = delegate { };
    public event Action<InputShowStateChanged> InputShowStateChanged = delegate { };
    public event Action<InputMuteStateChanged> InputMuteStateChanged = delegate { };
    public event Action<InputVolumeChanged> InputVolumeChanged = delegate { };
    public event Action<InputAudioBalanceChanged> InputAudioBalanceChanged = delegate { };
    public event Action<InputAudioSyncOffsetChanged> InputAudioSyncOffsetChanged = delegate { };
    public event Action<InputAudioTracksChanged> InputAudioTracksChanged = delegate { };
    public event Action<InputAudioMonitorTypeChanged> InputAudioMonitorTypeChanged = delegate { };
    public event Action<InputVolumeMeters> InputVolumeMeters = delegate { };

    public event Action<TransitionsEvent> TransitionsEvent = delegate { };
    public event Action<CurrentSceneTransitionChanged> CurrentSceneTransitionChanged = delegate { };
    public event Action<CurrentSceneTransitionDurationChanged> CurrentSceneTransitionDurationChanged = delegate { };
    public event Action<SceneTransitionStarted> SceneTransitionStarted = delegate { };
    public event Action<SceneTransitionEnded> SceneTransitionEnded = delegate { };
    public event Action<SceneTransitionVideoEnded> SceneTransitionVideoEnded = delegate { };

    public event Action<FiltersEvent> FiltersEvent = delegate { };
    public event Action<SourceFilterListReindexed> SourceFilterListReindexed = delegate { };
    public event Action<SourceFilterCreated> SourceFilterCreated = delegate { };
    public event Action<SourceFilterRemoved> SourceFilterRemoved = delegate { };
    public event Action<SourceFilterNameChanged> SourceFilterNameChanged = delegate { };
    public event Action<SourceFilterEnableStateChanged> SourceFilterEnableStateChanged = delegate { };

    public event Action<SceneItemsEvent> SceneItemsEvent = delegate { };
    public event Action<SceneItemCreated> SceneItemCreated = delegate { };
    public event Action<SceneItemRemoved> SceneItemRemoved = delegate { };
    public event Action<SceneItemListReindexed> SceneItemListReindexed = delegate { };
    public event Action<SceneItemEnableStateChanged> SceneItemEnableStateChanged = delegate { };
    public event Action<SceneItemLockStateChanged> SceneItemLockStateChanged = delegate { };
    public event Action<SceneItemSelected> SceneItemSelected = delegate { };
    public event Action<SceneItemTransformChanged> SceneItemTransformChanged = delegate { };

    public event Action<OutputsEvent> OutputsEvent = delegate { };
    public event Action<StreamStateChanged> StreamStateChanged = delegate { };
    public event Action<RecordStateChanged> RecordStateChanged = delegate { };
    public event Action<ReplayBufferStateChanged> ReplayBufferStateChanged = delegate { };
    public event Action<VirtualcamStateChanged> VirtualcamStateChanged = delegate { };
    public event Action<ReplayBufferSaved> ReplayBufferSaved = delegate { };

    public event Action<MediaInputsEvent> MediaInputsEvent = delegate { };
    public event Action<MediaInputPlaybackStarted> MediaInputPlaybackStarted = delegate { };
    public event Action<MediaInputPlaybackEnded> MediaInputPlaybackEnded = delegate { };
    public event Action<MediaInputActionTriggered> MediaInputActionTriggered = delegate { };

    public event Action<UiEvent> UiEvent = delegate { };
    public event Action<StudioModeStateChanged> StudioModeStateChanged = delegate { };
    #endregion

    private const int _supportedRpcVersion = 1;

    private readonly Socket _socket;
    private readonly ClientWebSocket _clientWebSocket;
    private readonly SemaphoreSlim _sendSemaphore = new(1, 1);
    private readonly ConcurrentDictionary<string, TaskCompletionSource<RequestResponse>> _requests = new();

    private CancellationTokenSource _cancellation = new();

    public string? CloseDescription { get => _clientWebSocket.CloseStatusDescription; }

    public ClientSocket(ClientWebSocket? client = null) {
      _clientWebSocket = client ?? new ClientWebSocket();
      _clientWebSocket.Options.AddSubProtocol("obswebsocket.msgpack");
      _socket = new Socket(_clientWebSocket);
    }

    public async Task ConnectAsync(
      Uri? uri = null,
      string? password = null,
      EventSubscription events = EventSubscription.All,
      CancellationToken? cancellation = null
    ) {
      var token = cancellation ?? CancellationToken.None;
      await _clientWebSocket.ConnectAsync(uri ?? Socket.defaultUri, token).ConfigureAwait(false);

      var hello = (Hello)(await _socket.ReceiveAsync(token).ConfigureAwait(false))!;
      if (hello.RpcVersion > _supportedRpcVersion) {
        // TODO: Log
      }

      var identify = new Identify() {
        RpcVersion = _supportedRpcVersion,
        EventSubscriptions = events,
        Authentication = MakeOneTimePass(password, hello.Authentication),
      };
      await _socket.SendAsync(identify, token).ConfigureAwait(false);
      try {
        var identified = (Identified)(await _socket.ReceiveAsync(token))!;
        _cancellation = new();
        _ = RunReceiveLoopAsync();
      }
      catch (Exception ex) {
        throw ex;
      }
    }

    public async Task CloseAsync() {
      await _sendSemaphore.WaitAsync(_cancellation.Token).ConfigureAwait(false);
      try {
        await _socket.CloseAsync(_cancellation.Token).ConfigureAwait(false);
      }
      finally {
        _sendSemaphore.Release();
      }
    }

    private async Task RunReceiveLoopAsync() {
      while (!_cancellation.IsCancellationRequested) {
        var message = await _socket.ReceiveAsync(_cancellation.Token).ConfigureAwait(false);
        if (_cancellation.IsCancellationRequested) {
          break;
        }
        if (message == null) {
          await CloseAsync().ConfigureAwait(false);
          _requests.Clear();
          _cancellation.Cancel();
          return;
        }

        _ = Task.Run(() => Dispatch(message));
      }
    }

    private void Dispatch(IOpcodeMessage message) {
      switch (message) {
      case IEvent obsEvent:
        Event(obsEvent);
        switch (message) {
        case GeneralEvent general:
          DispatchGeneralEvent(general);
          break;
        case ConfigEvent config:
          DispatchConfigEvent(config);
          break;
        }
        break;
      case RequestResponse response:
        _requests[response.RequestId].SetResult(response);
        break;
      default:
        // TODO: Log
        break;
      }
    }

    private void DispatchGeneralEvent(GeneralEvent general) {
      GeneralEvent(general);
      switch (general) {
      case ExitStarted exit:
        ExitStarted(exit);
        break;
      case VendorEvent vendor:
        VendorEvent(vendor);
        break;
      }
    }
    private void DispatchConfigEvent(ConfigEvent config) {
      ConfigEvent(config);
      switch (config) {
      }
    }
    public async Task<RequestResponse?> RequestAsync(IRequest request, bool skipResponse = false, CancellationToken? cancellation = null) {
      using var source = CancellationTokenSource.CreateLinkedTokenSource(
        cancellation ?? CancellationToken.None,
        _cancellation.Token
      );
      var token = source.Token;
      token.ThrowIfCancellationRequested();

      string guid = $"{Guid.NewGuid()}";
      request.RequestId = guid;

      TaskCompletionSource<RequestResponse>? waiter = null;
      bool willWaitResponse = !skipResponse
          && DataTypeMapping.RequestToTypes.TryGetValue(request.RequestType, out var typeMapping)
          && typeMapping.Item2 != typeof(RequestResponse);
      if (willWaitResponse) {
        waiter = new();
        _requests[guid] = waiter;
      }
      await _sendSemaphore.WaitAsync(token).ConfigureAwait(false);

      try {
        token.ThrowIfCancellationRequested();

        await _socket.SendAsync(request, token).ConfigureAwait(false);
      }
      finally {
        _sendSemaphore.Release();
      }

      return willWaitResponse ? await waiter!.Task.ConfigureAwait(false) : null;
    }

    public void Dispose() {
      _cancellation.Cancel();
      _cancellation.Dispose();
      _sendSemaphore.Dispose();
      _socket.Dispose();
    }

    private static string? MakeOneTimePass(string? password, HelloAuthentication? auth) {
      if (auth == null) {
        return null;
      }
      if (password == null) {
        throw new Exception("Password requested.");
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
