namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using System.Collections.Generic;

  public interface IRequestResponse : IOpcodeMessage {
    public string RequestType { get; }

    public string RequestId { get; }

    public RequestStatus RequestStatus { get; }
  }

  [MessagePackObject]
  public class RequestResponse : IRequestResponse {
    [IgnoreMember]
    public OpCode Op => OpCode.RequestResponse;

    [Key("requestType")]
    public string RequestType { get => GetType().Name; }

    [Key("requestId")]
    public string RequestId { get; } = "";

    [Key("requestStatus")]
    public RequestStatus RequestStatus { get; } = new();
  }

  [MessagePackObject]
  public class RawRequestResponse : IRequestResponse {
    [IgnoreMember]
    public OpCode Op => OpCode.RequestResponse;

    [Key("requestType")]
    public string RequestType { get; set; } = "";

    [Key("requestId")]
    public string RequestId { get; set; } = "";

    [Key("requestStatus")]
    public RequestStatus RequestStatus { get; set; } = new();

    [Key("responseData")]
    public Dictionary<string, object?>? ResponseData { get; set; }
  }

  [MessagePackObject]
  public class RequestStatus {
    [Key("result")]
    public bool Result { get; set; }

    [Key("code")]
    public RequestStatusCode Code { get; set; }

    [Key("comment")]
    public string? Comment { get; set; }
  }
}
