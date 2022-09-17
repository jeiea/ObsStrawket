using ObsStrawket.DataTypes;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ObsStrawket.Test.Specs {
  public class SetStreamServiceSettingsTest {
    [Fact]
    public async Task TestAsync() {
      await SpecTester.TestAsync(new SetStreamServiceSettingsFlow()).ConfigureAwait(false);
    }
  }

  record class StreamSettings(string Key, string Server, string Service);

  class SetStreamServiceSettingsFlow : ITestFlow {
    public static StreamSettings GetTestSettings() {
      string? environment = Environment.GetEnvironmentVariable("STREAM_SETTINGS");
      if (environment == null) {
        return new("a", "b", "c");
      }

      string[] parts = environment.Split("!");
      return new StreamSettings(parts[0], parts[1], parts[2]);
    }

    public async Task RequestAsync(ObsClientSocket client) {
      var settings = GetTestSettings();
      if (settings == null) {
        return;
      }

      await client.SetStreamServiceSettingsAsync(
        streamServiceType: StreamServiceType.RtmpCommon,
        streamServiceSettings: new Dictionary<string, object?>() {
          { "bwtest", true },
          { "key", settings.Key },
          { "server", settings.Server },
          { "service", settings.Service },
        }).ConfigureAwait(false);
    }

    public async Task RespondAsync(MockServerSession session) {
      var settings = GetTestSettings();
      if (settings == null) {
        return;
      }

      string? guid = await session.ReceiveAsync(@"{
  ""d"": {
    ""requestData"": {
      ""streamServiceSettings"": {
        ""bwtest"": true,
        ""key"": ""{key}"",
        ""server"": ""{server}"",
        ""service"": ""{service}""
      },
      ""streamServiceType"": ""rtmp_common""
    },
    ""requestId"": ""{guid}"",
    ""requestType"": ""SetStreamServiceSettings""
  },
  ""op"": 6
}"
.Replace("{key}", settings.Key)
.Replace("{server}", settings.Server)
.Replace("{service}", settings.Service)).ConfigureAwait(false);

      await session.SendAsync(@"{
  ""d"": {
    ""requestId"": ""{guid}"",
    ""requestStatus"": {
      ""code"": 100,
      ""result"": true
    },
    ""requestType"": ""SetStreamServiceSettings""
  },
  ""op"": 7
}".Replace("{guid}", guid)).ConfigureAwait(false);
    }
  }
}
