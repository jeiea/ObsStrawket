namespace ObsDotnetSocket.DataTypes
{
    using MessagePack;

    [MessagePackObject]
    public class Identified : IOpCodeMessage
    {
        [IgnoreMember]
        public OpCode Op => OpCode.Identified;

        [Key("negotiatedRpcVersion")]
        public int NegotiatedRpcVersion { get; set; }
    }
}
