using MessagePack;
using System.Collections.Generic;
using System.Net.WebSockets;
using System;
using System.Buffers.Text;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ObsStrawket.DataTypes {
  /// <summary>
  /// Hello (OpCode 0)<br />
  /// Sent from: obs-websocket<br />
  /// Sent to: Freshly connected websocket client<br />
  /// Description: First message sent from the server immediately on client connection.<br />
  /// Contains authentication information if auth is required.Also contains RPC version for version negotiation.<br />
  /// Data Keys:<br />
  /// <code>{
  ///  "obsWebSocketVersion": string,
  ///  "rpcVersion": number,
  ///  "authentication": object(optional)
  /// }</code>
  /// </summary>
  [MessagePackObject]
  public class Hello : IOpCodeMessage {
    /// <summary>
    /// Hello (OpCode 0)
    /// </summary>
    [IgnoreMember]
    public OpCode Op => OpCode.Hello;

    /// <summary>
    /// Websocket server's library version.
    /// </summary>
    [Key("obsWebSocketVersion")]
    public string ObsWebSocketVersion { get; set; } = "";

    /// <summary>
    /// A version number which gets incremented on each breaking change to the obs-websocket protocol.
    /// Its usage in this context is to provide the current rpc version that the server would like to use.
    /// </summary>
    [Key("rpcVersion")]
    public int RpcVersion { get; set; }

    /// <summary>
    /// Authentication strings
    /// </summary>
    [Key("authentication")]
    public HelloAuthentication? Authentication { get; set; }
  }

  /// <summary>
  /// obs-websocket uses SHA256 to transmit authentication credentials.
  /// The server starts by sending an object in the authentication field of its Hello message data.
  /// The client processes the authentication challenge and responds via the authentication string in the Identify message data.<br />
  /// For this guide, we'll be using <c>supersecretpassword</c> as the password.
  /// The <c>authentication</c> object in <c>Hello</c> looks like this (example):<br />
  /// <code>{
  ///     "challenge": "+IxH4CnCiqpX1rM9scsNynZzbOe4KhDeYcTNS3PDaeY=",
  ///     "salt": "lM1GncleQOaCu9lT1yeUZhFYnqhsLLP1G5lAGo3ixaI="
  /// }</code>
  /// To generate the authentication string, follow these steps:
  /// <list type="number">
  /// <item>Concatenate the websocket password with the salt provided by the server(password + salt)</item>
  /// <item>Generate an SHA256 binary hash of the result and base64 encode it, known as a base64 secret.</item>
  /// <item>Concatenate the base64 secret with the challenge sent by the server(base64_secret + challenge)</item>
  /// <item>Generate a binary SHA256 hash of that result and base64 encode it.You now have your authentication string.</item>
  /// </list>
  /// For real-world examples of the authentication string creation, refer to the obs-websocket client libraries listed on the README.
  /// </summary>
  [MessagePackObject]
  public class HelloAuthentication {
    /// <summary>
    /// Additional input to a one-way function that hashes base64 secret.
    /// </summary>
    [Key("challenge")]
    public string Challenge { get; set; } = "";

    /// <summary>
    /// Random base64 string provided as an additional input to a one-way function that hashes password.
    /// </summary>
    [Key("salt")]
    public string Salt { get; set; } = "";
  }
}
