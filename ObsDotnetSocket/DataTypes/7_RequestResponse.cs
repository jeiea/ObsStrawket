namespace ObsDotnetSocket.DataTypes {
  using MessagePack;
  using System.Collections.Generic;

  [MessagePackObject]
  public class RequestResponse<T> : IOpcodeMessage where T : new() {
    [IgnoreMember]
    public WebSocketOpCode Op => WebSocketOpCode.RequestResponse;

    [Key("requestType")]
    public string RequestType { get; set; } = "";

    [Key("requestId")]
    public string RequestId { get; set; } = "";

    [Key("requestStatus")]
    public RequestStatus RequestStatus { get; set; } = new();
  }

  [MessagePackObject]
  public class RequestResponse : RequestResponse<Dictionary<string, object?>> {
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
