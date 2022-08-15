namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using MessagePack.Formatters;
  using ObsDotnetSocket.DataTypes.Predefineds;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  class RequestFormatter : IMessagePackFormatter<IRequest> {
    public static readonly RequestFormatter Instance = new();

    internal static readonly Dictionary<string, (Type, Type)> NameToTypes = new (Type, Type)[] {
      // General Requests
      new (typeof(GetVersion), typeof(GetVersionResponse)),
      new (typeof(GetStats), typeof(GetStatsResponse)),
      new (typeof(BroadcastCustomEvent), typeof(RequestResponse)),
      new (typeof(CallVendorRequest), typeof(CallVendorRequestResponse)),
      new (typeof(GetHotkeyList), typeof(GetHotkeyListResponse)),
      new (typeof(TriggerHotkeyByName), typeof(RequestResponse)),
      new (typeof(TriggerHotkeyByKeySequence), typeof(RequestResponse)),
      new (typeof(Sleep), typeof(RequestResponse)),
      // Config Requests
      new (typeof(GetPersistentData), typeof(GetPersistentDataResponse)),
      new (typeof(SetPersistentData), typeof(RequestResponse)),
      new (typeof(GetSceneCollectionList), typeof(GetSceneCollectionListResponse)),
      new (typeof(SetCurrentSceneCollection), typeof(RequestResponse)),
      new (typeof(CreateSceneCollection), typeof(RequestResponse)),
      new (typeof(GetProfileList), typeof(GetProfileListResponse)),
      new (typeof(SetCurrentProfile), typeof(RequestResponse)),
      new (typeof(CreateProfile), typeof(RequestResponse)),
      new (typeof(RemoveProfile), typeof(RequestResponse)),
      new (typeof(GetProfileParameter), typeof(GetProfileParameterResponse)),
      new (typeof(SetProfileParameter), typeof(RequestResponse)),
      new (typeof(GetVideoSettings), typeof(GetVideoSettingsResponse)),
      new (typeof(SetVideoSettings), typeof(RequestResponse)),
      new (typeof(GetStreamServiceSettings), typeof(GetStreamServiceSettingsResponse)),
      new (typeof(SetStreamServiceSettings), typeof(RequestResponse)),
      // Sources Requests
      new (typeof(GetSourceActive), typeof(GetSourceActiveResponse)),
      new (typeof(GetSourceScreenshot), typeof(GetSourceScreenshotResponse)),
      new (typeof(SaveSourceScreenshot), typeof(SaveSourceScreenshotResponse)),
      // Scenes Requests
      new (typeof(GetSceneList), typeof(GetSceneListResponse)),
      new (typeof(GetGroupList), typeof(GetGroupListResponse)),
      new (typeof(GetCurrentProgramScene), typeof(GetCurrentProgramSceneResponse)),
      new (typeof(SetCurrentProgramScene), typeof(RequestResponse)),
      new (typeof(GetCurrentPreviewScene), typeof(GetCurrentPreviewSceneResponse)),
      new (typeof(SetCurrentPreviewScene), typeof(RequestResponse)),
      new (typeof(CreateScene), typeof(RequestResponse)),
      new (typeof(RemoveScene), typeof(RequestResponse)),
      new (typeof(SetSceneName), typeof(RequestResponse)),
      new (typeof(GetSceneSceneTransitionOverride), typeof(GetSceneSceneTransitionOverrideResponse)),
      new (typeof(SetSceneSceneTransitionOverride), typeof(RequestResponse)),
      // Inputs Requests
      new (typeof(GetInputList), typeof(GetInputListResponse)),
      new (typeof(GetInputKindList), typeof(GetInputKindListResponse)),
      new (typeof(GetSpecialInputs), typeof(GetSpecialInputsResponse)),
      new (typeof(CreateInput), typeof(CreateInputResponse)),
      new (typeof(RemoveInput), typeof(RequestResponse)),
      new (typeof(SetInputName), typeof(RequestResponse)),
      new (typeof(GetInputDefaultSettings), typeof(GetInputDefaultSettingsResponse)),
      new (typeof(GetInputSettings), typeof(GetInputSettingsResponse)),
      new (typeof(SetInputSettings), typeof(RequestResponse)),
      new (typeof(GetInputMute), typeof(GetInputMuteResponse)),
      new (typeof(SetInputMute), typeof(RequestResponse)),
      new (typeof(ToggleInputMute), typeof(ToggleInputMuteResponse)),
      new (typeof(GetInputVolume), typeof(GetInputVolumeResponse)),
      new (typeof(SetInputVolume), typeof(RequestResponse)),
      new (typeof(GetInputAudioBalance), typeof(GetInputAudioBalanceResponse)),
      new (typeof(SetInputAudioBalance), typeof(RequestResponse)),
      new (typeof(GetInputAudioSyncOffset), typeof(GetInputAudioSyncOffsetResponse)),
      new (typeof(SetInputAudioSyncOffset), typeof(RequestResponse)),
      new (typeof(GetInputAudioMonitorType), typeof(GetInputAudioMonitorTypeResponse)),
      new (typeof(SetInputAudioMonitorType), typeof(RequestResponse)),
      new (typeof(GetInputAudioTracks), typeof(GetInputAudioTracksResponse)),
      new (typeof(SetInputAudioTracks), typeof(RequestResponse)),
      new (typeof(GetInputPropertiesListPropertyItems), typeof(GetInputPropertiesListPropertyItemsResponse)),
      new (typeof(PressInputPropertiesButton), typeof(RequestResponse)),
      // Transitions Requests
      new (typeof(GetTransitionKindList), typeof(GetTransitionKindListResponse)),
      new (typeof(GetSceneTransitionList), typeof(GetSceneTransitionListResponse)),
      new (typeof(GetCurrentSceneTransition), typeof(GetCurrentSceneTransitionResponse)),
      new (typeof(SetCurrentSceneTransition), typeof(RequestResponse)),
      new (typeof(SetCurrentSceneTransitionDuration), typeof(RequestResponse)),
      new (typeof(SetCurrentSceneTransitionSettings), typeof(RequestResponse)),
      new (typeof(GetCurrentSceneTransitionCursor), typeof(GetCurrentSceneTransitionCursorResponse)),
      new (typeof(TriggerStudioModeTransition), typeof(RequestResponse)),
      new (typeof(SetTBarPosition), typeof(RequestResponse)),
      // Filters Requests
      new (typeof(GetSourceFilterList), typeof(GetSourceFilterListResponse)),
      new (typeof(GetSourceFilterDefaultSettings), typeof(GetSourceFilterDefaultSettingsResponse)),
      new (typeof(CreateSourceFilter), typeof(RequestResponse)),
      new (typeof(RemoveSourceFilter), typeof(RequestResponse)),
      new (typeof(SetSourceFilterName), typeof(RequestResponse)),
      new (typeof(GetSourceFilter), typeof(GetSourceFilterResponse)),
      new (typeof(SetSourceFilterIndex), typeof(RequestResponse)),
      new (typeof(SetSourceFilterSettings), typeof(RequestResponse)),
      new (typeof(SetSourceFilterEnabled), typeof(RequestResponse)),
      // Scene Items Requests
      new (typeof(GetSceneItemList), typeof(GetSceneItemListResponse)),
      new (typeof(GetGroupSceneItemList), typeof(GetGroupSceneItemListResponse)),
      new (typeof(GetSceneItemId), typeof(GetSceneItemIdResponse)),
      new (typeof(CreateSceneItem), typeof(CreateSceneItemResponse)),
      new (typeof(RemoveSceneItem), typeof(RequestResponse)),
      new (typeof(DuplicateSceneItem), typeof(DuplicateSceneItemResponse)),
      new (typeof(GetSceneItemTransform), typeof(GetSceneItemTransformResponse)),
      new (typeof(SetSceneItemTransform), typeof(RequestResponse)),
      new (typeof(GetSceneItemEnabled), typeof(GetSceneItemEnabledResponse)),
      new (typeof(SetSceneItemEnabled), typeof(RequestResponse)),
      new (typeof(GetSceneItemLocked), typeof(GetSceneItemLockedResponse)),
      new (typeof(SetSceneItemLocked), typeof(RequestResponse)),
      new (typeof(GetSceneItemIndex), typeof(GetSceneItemIndexResponse)),
      new (typeof(SetSceneItemIndex), typeof(RequestResponse)),
      new (typeof(GetSceneItemBlendMode), typeof(GetSceneItemBlendModeResponse)),
      new (typeof(SetSceneItemBlendMode), typeof(RequestResponse)),
      // Outputs Requests
      new (typeof(GetVirtualCamStatus), typeof(GetVirtualCamStatusResponse)),
      new (typeof(ToggleVirtualCam), typeof(ToggleVirtualCamResponse)),
      new (typeof(StartVirtualCam), typeof(RequestResponse)),
      new (typeof(StopVirtualCam), typeof(RequestResponse)),
      new (typeof(GetReplayBufferStatus), typeof(GetReplayBufferStatusResponse)),
      new (typeof(ToggleReplayBuffer), typeof(ToggleReplayBufferResponse)),
      new (typeof(StartReplayBuffer), typeof(RequestResponse)),
      new (typeof(StopReplayBuffer), typeof(RequestResponse)),
      new (typeof(SaveReplayBuffer), typeof(RequestResponse)),
      new (typeof(GetLastReplayBufferReplay), typeof(GetLastReplayBufferReplayResponse)),
      new (typeof(GetOutputList), typeof(RequestResponse)),
      new (typeof(GetOutputStatus), typeof(GetOutputStatusResponse)),
      new (typeof(ToggleOutput), typeof(ToggleOutputResponse)),
      new (typeof(StartOutput), typeof(RequestResponse)),
      new (typeof(StopOutput), typeof(RequestResponse)),
      new (typeof(GetOutputSettings), typeof(GetOutputSettingsResponse)),
      new (typeof(SetOutputSettings), typeof(RequestResponse)),
      // Stream Requests
      new (typeof(GetStreamStatus), typeof(GetStreamStatusResponse)),
      new (typeof(ToggleStream), typeof(ToggleStreamResponse)),
      new (typeof(StartStream), typeof(RequestResponse)),
      new (typeof(StopStream), typeof(RequestResponse)),
      new (typeof(SendStreamCaption), typeof(RequestResponse)),
      // Record Requests
      new (typeof(GetRecordStatus), typeof(GetRecordStatusResponse)),
      new (typeof(ToggleRecord), typeof(RequestResponse)),
      new (typeof(StartRecord), typeof(RequestResponse)),
      new (typeof(StopRecord), typeof(StopRecordResponse)),
      new (typeof(ToggleRecordPause), typeof(RequestResponse)),
      new (typeof(PauseRecord), typeof(RequestResponse)),
      new (typeof(ResumeRecord), typeof(RequestResponse)),
      // Media Inputs Requests
      new (typeof(GetMediaInputStatus), typeof(GetMediaInputStatusResponse)),
      new (typeof(SetMediaInputCursor), typeof(RequestResponse)),
      new (typeof(OffsetMediaInputCursor), typeof(RequestResponse)),
      new (typeof(TriggerMediaInputAction), typeof(RequestResponse)),
      // Ui Requests
      new (typeof(GetStudioModeEnabled), typeof(GetStudioModeEnabledResponse)),
      new (typeof(SetStudioModeEnabled), typeof(RequestResponse)),
      new (typeof(OpenInputPropertiesDialog), typeof(RequestResponse)),
      new (typeof(OpenInputFiltersDialog), typeof(RequestResponse)),
      new (typeof(OpenInputInteractDialog), typeof(RequestResponse)),
      new (typeof(GetMonitorList), typeof(GetMonitorListResponse)),
      new (typeof(OpenVideoMixProjector), typeof(RequestResponse)),
      new (typeof(OpenSourceProjector), typeof(RequestResponse)),
    }.ToDictionary(t => t.Item1.Name, t => t);

    public IRequest Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) {
      string requestType = FormatterUtil.SeekByKey(reader, "requestType").ReadString();
      string requestId = FormatterUtil.SeekByKey(reader, "requestId").ReadString();
      var peeker = FormatterUtil.SeekByKey(reader, "responseData");

      var data = NameToTypes.TryGetValue(requestType, out var type)
        ? (MessagePackSerializer.Deserialize(type.Item1, ref peeker, options) as IRequest)!
        : MessagePackSerializer.Deserialize<Request>(ref peeker, options);
      data.RequestId = requestId;

      reader.Skip();
      return data;
    }

    public void Serialize(ref MessagePackWriter writer, IRequest value, MessagePackSerializerOptions options) {
      writer.WriteMapHeader(2);
      writer.Write("op");
      writer.Write((int)value.Op);
      writer.Write("d");

      if (value is not RawRequest) {
        writer.WriteMapHeader(3);
        writer.Write("requestType");
        writer.Write(value.RequestType);
        writer.Write("requestId");
        writer.Write(value.RequestId);
        writer.Write("requestData");
      }

      MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
    }
  }
}