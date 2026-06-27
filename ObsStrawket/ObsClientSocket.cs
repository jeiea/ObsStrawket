using ObsStrawket.DataTypes;
using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Diagnostics;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ObsStrawket {

  /// <summary>
  /// High level client interface.
  /// </summary>
  public partial class ObsClientSocket : IDisposable {

    private readonly ClientSocket _clientSocket;

    private readonly SemaphoreSlim _connectSemaphore = new(1);

    private Action<PipelineEvent>? _pipelineEvent;

    private Task? _dispatch;

    /// <summary>
    /// Create OBS websocket client. It can be reused unless <see cref="Dispose"/> is called.
    /// </summary>
    /// <param name="client">Lower level client for custom behavior.</param>
    /// <param name="useChannel">Use channel for event receive.<br />
    /// Caution: If <see cref="Events"/> is not consumed it will cause memory leak.
    /// </param>
    public ObsClientSocket(ClientSocket? client = null, bool useChannel = false) {
      _clientSocket = client ?? new ClientSocket();
      if (!useChannel) {
        _dispatch = Task.CompletedTask;
      }
    }

    /// <summary>
    /// Diagnostic notifications emitted while processing the websocket pipeline. Forwards the
    /// underlying <see cref="ClientSocket.PipelineEvent"/> and adds dispatch-stage notifications.
    /// </summary>
    /// <remarks>
    /// Handlers may run concurrently from several threads and the emission order is not
    /// guaranteed; write them to be thread-safe. See <see cref="Diagnostics.PipelineEvent"/>.
    /// </remarks>
    public event Action<PipelineEvent>? PipelineEvent {
      add {
        bool subscribe = _pipelineEvent == null && value != null;
        _pipelineEvent += value;
        if (subscribe) {
          _clientSocket.PipelineEvent += Emit;
        }
      }
      remove {
        _pipelineEvent -= value;
        if (_pipelineEvent == null) {
          _clientSocket.PipelineEvent -= Emit;
        }
      }
    }

    /// <summary>
    /// Minimum severity emitted through <see cref="PipelineEvent"/>.
    /// </summary>
    public PipelineLevel PipelineEventLevel {
      get => _clientSocket.PipelineEventLevel;
      set => _clientSocket.PipelineEventLevel = value;
    }

    /// <summary>
    /// Fired whenever <see cref="ConnectionState"/> changes. This is independent of the
    /// event dispatch mode and also fires when the client is created with <c>useChannel</c>.
    /// </summary>
    /// <remarks>
    /// Handlers run synchronously during the state transition. Do not synchronously block on
    /// <see cref="ConnectAsync"/> or <see cref="CloseAsync"/> from a handler.
    /// </remarks>
    public event EventHandler<ObsConnectionStateChangedEventArgs>? ConnectionStateChanged {
      add => _clientSocket.ConnectionStateChanged += value;
      remove => _clientSocket.ConnectionStateChanged -= value;
    }

    /// <summary>
    /// Whether it is connected to OBS
    /// </summary>
    public bool IsConnected => _clientSocket.IsConnected;

    /// <summary>
    /// Current lifecycle snapshot of the websocket connection.
    /// </summary>
    public ObsConnectionState ConnectionState => _clientSocket.ConnectionState;

    /// <summary>
    /// It emits all received OBS events. It can be used only when <see cref="ObsClientSocket"/>
    /// is created with <c>useChannel</c>. Abnormal completion throws a
    /// <see cref="ChannelClosedException"/> whose inner exception contains the normalized cause.
    /// </summary>
    public ChannelReader<IObsEvent> Events => _dispatch != null
          ? throw new InvalidOperationException("Create first ObsClientSocket with useChannel: true")
          : _clientSocket.Events;

    /// <summary>
    /// Connect to OBS websocket server.
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
      var target = uri ?? ClientSocket.DefaultUri;
      await _connectSemaphore.WaitAsync(cancellation).ConfigureAwait(false);
      try {
        bool connected = await _clientSocket.ConnectAsync(
          target,
          password,
          events,
          cancellation
        ).ConfigureAwait(false);
        if (connected && _dispatch != null) {
          _dispatch = DispatchEventAsync(_dispatch, _clientSocket.Events);
        }
        return connected;
      }
      finally {
        _ = _connectSemaphore.Release();
      }
    }

    /// <summary>
    /// Close this connection. Pending requests fail with
    /// <see cref="ObsConnectionClosedException"/>.
    /// </summary>
    public async Task CloseAsync() {
      await _clientSocket.CloseAsync().ConfigureAwait(false);
      if (_dispatch != null) {
        await _dispatch.ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Low level typed request method.
    /// </summary>
    /// <typeparam name="TResponse">Expected response type.</typeparam>
    /// <param name="request">Request data.</param>
    /// <param name="cancellation">Token for cancellation</param>
    /// <returns>Response from websocket server.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The client is not connected.</exception>
    /// <exception cref="OperationCanceledException">The caller cancels the operation.</exception>
    /// <exception cref="ObsRequestException">OBS rejects the request.</exception>
    /// <exception cref="ObsConnectionException">The connection fails before a response is received.</exception>
    /// <exception cref="ObsProtocolException">OBS sends an invalid response.</exception>
    public Task<TResponse> RequestAsync<TResponse>(
      IRequest<TResponse> request,
      CancellationToken cancellation = default
    ) where TResponse : class, IRequestResponse {
      return _clientSocket.RequestAsync(request, cancellation);
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
      return _clientSocket.RequestAsync(batchRequest, cancellation);
    }

    /// <summary>
    /// Dispose this forever.
    /// </summary>
    public void Dispose() {
      _clientSocket.PipelineEvent -= Emit;
      _clientSocket.Dispose();
      _connectSemaphore.Dispose();
      GC.SuppressFinalize(this);
    }

    private async Task DispatchEventAsync(Task former, ChannelReader<IObsEvent> events) {
      await former.ConfigureAwait(false);

      try {
        while (await events.WaitToReadAsync().ConfigureAwait(false)) {
          DispatchEvent(await events.ReadAsync().ConfigureAwait(false));
        }
      }
      catch (Exception exception) {
        _ = exception;
      }
    }

    private void Emit(PipelineEvent diagnostic) {
      if (!_clientSocket.ShouldEmitPipelineEvent(diagnostic.Level)) {
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
        // Isolate consumer exceptions so the dispatch pump keeps running.
      }
    }

    #region Event dispatch

    private void DispatchEvent(IObsEvent message) {
      try {
        Event(message);
      }
      catch (Exception ex) {
        Emit(new EventHandlerFaulted(EventHandlerKind.Event, message.EventType, ex));
      }
      try {
        DispatchSpecificEvent(message);
      }
      catch (Exception ex) {
        Emit(new EventHandlerFaulted(EventHandlerKind.SpecificEvent, message.EventType, ex));
      }
    }

    private void DispatchSpecificEvent(IObsEvent message) {
      switch (message) {
      case CanvasesEvent canvases:
        DispatchCanvasesEvent(canvases);
        break;

      case GeneralEvent general:
        DispatchGeneralEvent(general);
        break;

      case ConfigEvent config:
        DispatchConfigEvent(config);
        break;

      case ScenesEvent scenes:
        DispatchScenesEvent(scenes);
        break;

      case InputsEvent input:
        DispatchInputsEvent(input);
        break;

      case TransitionsEvent transitions:
        DispatchTransitionsEvent(transitions);
        break;

      case FiltersEvent filters:
        DispatchFiltersEvent(filters);
        break;

      case SceneItemsEvent sceneItems:
        DispatchSceneItemsEvent(sceneItems);
        break;

      case OutputsEvent outputs:
        DispatchOutputsEvent(outputs);
        break;

      case MediaInputsEvent mediaInputs:
        DispatchMediaInputsEvent(mediaInputs);
        break;

      case UiEvent ui:
        DispatchUiEvent(ui);
        break;

      default:
        Emit(new UnclassifiedEventIgnored(message.EventType));
        break;
      }
    }

    private void DispatchCanvasesEvent(CanvasesEvent canvases) {
      CanvasesEvent(canvases);
      switch (canvases) {
      case CanvasCreated ev:
        CanvasCreated(ev);
        break;

      case CanvasRemoved ev:
        CanvasRemoved(ev);
        break;

      case CanvasNameChanged ev:
        CanvasNameChanged(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchGeneralEvent(GeneralEvent general) {
      GeneralEvent(general);
      switch (general) {
      case ExitStarted exit:
        ExitStarted(exit);
        break;

      case CustomEvent custom:
        CustomEvent(custom);
        break;

      case VendorEvent vendor:
        VendorEvent(vendor);
        break;
      default:
        break;
      }
    }

    private void DispatchConfigEvent(ConfigEvent config) {
      ConfigEvent(config);
      switch (config) {
      case CurrentSceneCollectionChanging ev:
        CurrentSceneCollectionChanging(ev);
        break;

      case CurrentSceneCollectionChanged ev:
        CurrentSceneCollectionChanged(ev);
        break;

      case SceneCollectionListChanged ev:
        SceneCollectionListChanged(ev);
        break;

      case CurrentProfileChanging ev:
        CurrentProfileChanging(ev);
        break;

      case CurrentProfileChanged ev:
        CurrentProfileChanged(ev);
        break;

      case ProfileListChanged ev:
        ProfileListChanged(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchScenesEvent(ScenesEvent scene) {
      ScenesEvent(scene);
      switch (scene) {
      case SceneCreated ev:
        SceneCreated(ev);
        break;

      case SceneRemoved ev:
        SceneRemoved(ev);
        break;

      case SceneNameChanged ev:
        SceneNameChanged(ev);
        break;

      case CurrentProgramSceneChanged ev:
        CurrentProgramSceneChanged(ev);
        break;

      case CurrentPreviewSceneChanged ev:
        CurrentPreviewSceneChanged(ev);
        break;

      case SceneListChanged ev:
        SceneListChanged(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchInputsEvent(InputsEvent input) {
      InputsEvent(input);
      switch (input) {
      case InputCreated ev:
        InputCreated(ev);
        break;

      case InputRemoved ev:
        InputRemoved(ev);
        break;

      case InputNameChanged ev:
        InputNameChanged(ev);
        break;

      case InputActiveStateChanged ev:
        InputActiveStateChanged(ev);
        break;

      case InputShowStateChanged ev:
        InputShowStateChanged(ev);
        break;

      case InputMuteStateChanged ev:
        InputMuteStateChanged(ev);
        break;

      case InputVolumeChanged ev:
        InputVolumeChanged(ev);
        break;

      case InputAudioBalanceChanged ev:
        InputAudioBalanceChanged(ev);
        break;

      case InputAudioSyncOffsetChanged ev:
        InputAudioSyncOffsetChanged(ev);
        break;

      case InputAudioTracksChanged ev:
        InputAudioTracksChanged(ev);
        break;

      case InputAudioMonitorTypeChanged ev:
        InputAudioMonitorTypeChanged(ev);
        break;

      case InputVolumeMeters ev:
        InputVolumeMeters(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchTransitionsEvent(TransitionsEvent transitions) {
      TransitionsEvent(transitions);
      switch (transitions) {
      case CurrentSceneTransitionChanged ev:
        CurrentSceneTransitionChanged(ev);
        break;

      case CurrentSceneTransitionDurationChanged ev:
        CurrentSceneTransitionDurationChanged(ev);
        break;

      case SceneTransitionStarted ev:
        SceneTransitionStarted(ev);
        break;

      case SceneTransitionEnded ev:
        SceneTransitionEnded(ev);
        break;

      case SceneTransitionVideoEnded ev:
        SceneTransitionVideoEnded(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchFiltersEvent(FiltersEvent filters) {
      FiltersEvent(filters);
      switch (filters) {
      case SourceFilterListReindexed ev:
        SourceFilterListReindexed(ev);
        break;

      case SourceFilterCreated ev:
        SourceFilterCreated(ev);
        break;

      case SourceFilterRemoved ev:
        SourceFilterRemoved(ev);
        break;

      case SourceFilterNameChanged ev:
        SourceFilterNameChanged(ev);
        break;

      case SourceFilterEnableStateChanged ev:
        SourceFilterEnableStateChanged(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchSceneItemsEvent(SceneItemsEvent sceneItems) {
      SceneItemsEvent(sceneItems);
      switch (sceneItems) {
      case SceneItemCreated ev:
        SceneItemCreated(ev);
        break;

      case SceneItemRemoved ev:
        SceneItemRemoved(ev);
        break;

      case SceneItemListReindexed ev:
        SceneItemListReindexed(ev);
        break;

      case SceneItemEnableStateChanged ev:
        SceneItemEnableStateChanged(ev);
        break;

      case SceneItemLockStateChanged ev:
        SceneItemLockStateChanged(ev);
        break;

      case SceneItemSelected ev:
        SceneItemSelected(ev);
        break;

      case SceneItemTransformChanged ev:
        SceneItemTransformChanged(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchOutputsEvent(OutputsEvent outputs) {
      OutputsEvent(outputs);
      switch (outputs) {
      case StreamStateChanged ev:
        StreamStateChanged(ev);
        break;

      case RecordStateChanged ev:
        RecordStateChanged(ev);
        break;

      case RecordFileChanged ev:
        RecordFileChanged(ev);
        break;

      case ReplayBufferStateChanged ev:
        ReplayBufferStateChanged(ev);
        break;

      case VirtualcamStateChanged ev:
        VirtualcamStateChanged(ev);
        break;

      case ReplayBufferSaved ev:
        ReplayBufferSaved(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchMediaInputsEvent(MediaInputsEvent mediaInputs) {
      MediaInputsEvent(mediaInputs);
      switch (mediaInputs) {
      case MediaInputPlaybackStarted ev:
        MediaInputPlaybackStarted(ev);
        break;

      case MediaInputPlaybackEnded ev:
        MediaInputPlaybackEnded(ev);
        break;

      case MediaInputActionTriggered ev:
        MediaInputActionTriggered(ev);
        break;
      default:
        break;
      }
    }

    private void DispatchUiEvent(UiEvent ui) {
      UiEvent(ui);
      switch (ui) {
      case StudioModeStateChanged ev:
        StudioModeStateChanged(ev);
        break;

      case ScreenshotSaved ev:
        ScreenshotSaved(ev);
        break;
      default:
        break;
      }
    }

    #endregion Event dispatch
  }
}
