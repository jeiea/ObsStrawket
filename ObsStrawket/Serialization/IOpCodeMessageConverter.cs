using ObsStrawket.DataTypes;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObsStrawket.Serialization {

  internal class BatchResponseConverter : JsonConverter<List<IRequestResponse>> {

    public override List<IRequestResponse>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      if (reader.TokenType != JsonTokenType.StartArray) {
        throw new JsonException();
      }

      reader.Read();
      var list = new List<IRequestResponse>();
      while (reader.TokenType != JsonTokenType.EndArray) {
        list.Add(IRequestResponseConverter.Deserialize(ref reader, options));
        reader.Read();
      }

      return list;
    }

    public override void Write(Utf8JsonWriter writer, List<IRequestResponse> value, JsonSerializerOptions options) {
      throw new NotImplementedException();
    }
  }

  internal class IRequestResponseConverter : JsonConverter<IRequestResponse> {

    public static void WriteFull(Utf8JsonWriter writer, IRequestResponse response, JsonSerializerOptions options) {
      if (response is RawRequestResponse) {
        JsonSerializer.Serialize(writer, response, response.GetType(), options);
        return;
      }

      writer.WriteStartObject();

      writer.WritePropertyName("d");
      WritePayload(writer, options, response);

      writer.WriteNumber("op", (int)response.Op);

      writer.WriteEndObject();
    }

    public override IRequestResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      return Deserialize(ref reader, options);
    }

    /// <summary>
    /// It is used on <see cref="RequestBatchResponse.Results"/>.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, IRequestResponse response, JsonSerializerOptions options) {
      WritePayload(writer, options, response);
    }

    internal static IRequestResponse Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      var rawReader = reader;
      JsonConverterHelper.CheckObjectStart(ref reader);

      var typeReader = JsonConverterHelper.SeekByKey(reader, "requestType");
      if (typeReader.GetString() is not string typeName) {
        throw new UnexpectedResponseException("Request type is null.");
      }

      if (DataTypeMapping.RequestToTypes.TryGetValue(typeName, out var mapping)) {
        var dataReader = reader;
        var responseType = mapping.Response;
        bool hasData = JsonConverterHelper.SeekByKey(ref dataReader, "responseData");
        var response = hasData
          ? (JsonSerializer.Deserialize(ref dataReader, responseType, options) as RequestResponse ?? JsonConverterHelper.CreateInstance<RequestResponse>(responseType))
          : JsonConverterHelper.CreateInstance<RequestResponse>(responseType);
        response.RequestId = JsonConverterHelper.SeekByKey(reader, "requestId").GetString() ?? "(null)";
        dataReader = JsonConverterHelper.SeekByKey(reader, "requestStatus");
        response.RequestStatus = JsonSerializer.Deserialize<RequestStatusObject>(ref dataReader, options) ?? throw new UnexpectedResponseException("Request status is null.");

        reader.Skip();
        return response;
      }
      else {
        reader.Skip();
        return JsonSerializer.Deserialize<RawRequestResponse>(ref rawReader, options) ?? throw new UnexpectedResponseException("");
      }
    }

    internal static void WritePayload(Utf8JsonWriter writer, JsonSerializerOptions options, IRequestResponse response) {
      writer.WriteStartObject();

      writer.WriteString("requestType", response.RequestType);
      writer.WriteString("requestId", response.RequestId);

      writer.WritePropertyName("requestStatus");
      JsonSerializer.Serialize(writer, response.RequestStatus, options);

      writer.WritePropertyName("responseData");
      JsonSerializer.Serialize(writer, response, response.GetType(), options);

      writer.WriteEndObject();
    }
  }

  internal class BatchRequestConverter : JsonConverter<List<IRequest>> {

    public override List<IRequest>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, List<IRequest> value, JsonSerializerOptions options) {
      writer.WriteStartArray();

      foreach (var request in value) {
        IRequestConverter.WritePayload(writer, request, options);
      }

      writer.WriteEndArray();
    }
  }

  // for batch
  internal class IRequestConverter : JsonConverter<IRequest> {

    public static void WritePayload(Utf8JsonWriter writer, IRequest request, JsonSerializerOptions options) {
      writer.WriteStartObject();

      writer.WriteString("requestType", request.RequestType);
      writer.WriteString("requestId", request.RequestId);

      bool isNotEmpty = DataTypeMapping.RequestToTypes.TryGetValue(request.RequestType, out var mapping) && !mapping.IsRequestEmpty;
      if (isNotEmpty) {
        writer.WritePropertyName("requestData");
        JsonSerializer.Serialize(writer, request, request.GetType(), options);
      }

      writer.WriteEndObject();
    }

    public static IRequest Deserialize(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      var rawReader = reader;
      JsonConverterHelper.CheckObjectStart(ref reader);

      var subReader = JsonConverterHelper.SeekByKey(reader, "requestType");
      if (subReader.GetString() is not string typeName) {
        throw new UnexpectedResponseException("Request type is null.");
      }
      subReader = JsonConverterHelper.SeekByKey(reader, "requestId");
      string? requestId = subReader.GetString();

      if (DataTypeMapping.RequestToTypes.TryGetValue(typeName, out var mapping) && mapping != null) {
        var dataReader = reader;
        var requestType = mapping.Request;
        bool hasEventData = JsonConverterHelper.SeekByKey(ref dataReader, "requestData");
        var request = hasEventData
          ? (JsonSerializer.Deserialize(ref dataReader, requestType, options) as IRequest ?? JsonConverterHelper.CreateInstance<IRequest>(requestType))
          : JsonConverterHelper.CreateInstance<IRequest>(requestType);
        reader.Skip();
        request.RequestId = requestId ?? "(null)";
        return request;
      }
      else {
        var request = JsonSerializer.Deserialize<RawRequest>(ref rawReader, options) ?? throw new UnexpectedResponseException("");
        reader.Skip();
        request.RequestId = requestId ?? "(null)";
        return request;
      }
    }

    public override IRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      JsonConverterHelper.CheckObjectStart(ref reader);

      var dataReader = JsonConverterHelper.SeekByKey(reader, "d");
      reader.Skip();

      return Deserialize(ref dataReader, options);
    }

    public override void Write(Utf8JsonWriter writer, IRequest request, JsonSerializerOptions options) {
      WritePayload(writer, request, options);
    }

    internal static void WriteFull(Utf8JsonWriter writer, IRequest request, JsonSerializerOptions options) {
      if (request is RawRequest) {
        JsonSerializer.Serialize(writer, request, typeof(RawRequest), options);
        return;
      }

      writer.WriteStartObject();

      writer.WritePropertyName("d");
      WritePayload(writer, request, options);

      writer.WriteNumber("op", (int)request.Op);

      writer.WriteEndObject();
    }
  }

  internal class IOpCodeMessageConverter : JsonConverter<IOpCodeMessage> {

    public override IOpCodeMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      JsonConverterHelper.CheckObjectStart(ref reader);

      var opReader = JsonConverterHelper.SeekByKey(reader, "op");
      var dataReader = JsonConverterHelper.SeekByKey(reader, "d");
      reader.Skip();

      return opReader.GetInt32() switch {
        (int)OpCode.Hello => JsonSerializer.Deserialize<Hello>(ref dataReader, options),
        (int)OpCode.Identify => JsonSerializer.Deserialize<Identify>(ref dataReader, options),
        (int)OpCode.Identified => JsonSerializer.Deserialize<Identified>(ref dataReader, options),
        (int)OpCode.Reidentify => JsonSerializer.Deserialize<Reidentify>(ref dataReader, options),
        (int)OpCode.Event => DeserializeEvent(ref dataReader, options),
        (int)OpCode.Request => IRequestConverter.Deserialize(ref dataReader, options),
        (int)OpCode.RequestResponse => IRequestResponseConverter.Deserialize(ref dataReader, options),
        (int)OpCode.RequestBatch => JsonSerializer.Deserialize<RequestBatch>(ref dataReader, options),
        (int)OpCode.RequestBatchResponse => JsonSerializer.Deserialize<RequestBatchResponse>(ref dataReader, options),
        _ => throw new NotImplementedException(),
      };
    }

    public override void Write(Utf8JsonWriter writer, IOpCodeMessage value, JsonSerializerOptions options) {
      switch (value) {
      case IRequest request:
        IRequestConverter.WriteFull(writer, request, options);
        return;

      case IRequestResponse response:
        IRequestResponseConverter.WriteFull(writer, response, options);
        return;

      case RawEvent:
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
        return;
      }

      writer.WriteStartObject();

      writer.WritePropertyName("d");
      switch (value) {
      case IObsEvent obsEvent:
        writer.WriteString("eventType", obsEvent.EventType);
        writer.WriteNumber("eventIntent", (int)obsEvent.EventIntent);
        writer.WritePropertyName("eventData");
        JsonSerializer.Serialize(writer, obsEvent, obsEvent.GetType(), options);
        break;

      case IRequestResponse response:
        writer.WriteString("requestType", response.RequestType);
        writer.WriteString("requestId", response.RequestId);

        writer.WritePropertyName("requestStatus");
        JsonSerializer.Serialize(writer, response.RequestStatus, options);

        writer.WritePropertyName("responseData");
        JsonSerializer.Serialize(writer, response, response.GetType(), options);
        break;

      default:
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
        break;
      }

      writer.WriteNumber("op", (int)value.Op);

      writer.WriteEndObject();
    }

    internal static IObsEvent DeserializeEvent(ref Utf8JsonReader reader, JsonSerializerOptions options) {
      var rawReader = reader;
      JsonConverterHelper.CheckObjectStart(ref reader);

      var typeReader = JsonConverterHelper.SeekByKey(reader, "eventType");
      if (typeReader.GetString() is not string typeName) {
        throw new UnexpectedResponseException("Event type is null.");
      }

      if (DataTypeMapping.EventToTypes.TryGetValue(typeName, out var eventType)) {
        var dataReader = reader;
        bool hasEventData = JsonConverterHelper.SeekByKey(ref dataReader, "eventData");
        var ev = hasEventData
          ? (JsonSerializer.Deserialize(ref dataReader, eventType, options) as IObsEvent ?? JsonConverterHelper.CreateInstance<IObsEvent>(eventType))
          : JsonConverterHelper.CreateInstance<IObsEvent>(eventType);
        reader.Skip();
        return ev;
      }
      else {
        reader.Skip();
        return JsonSerializer.Deserialize<RawEvent>(ref rawReader, options) ?? throw new UnexpectedResponseException("");
      }
    }
  }
}
