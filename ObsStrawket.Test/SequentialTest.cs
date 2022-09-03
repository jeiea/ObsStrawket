using ObsStrawket.DataTypes.Predefineds;
using ObsStrawket.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ObsStrawket.Test {
  public class SequentialTest {
    [Fact]
    public async Task TestAsync() {
      var cancellation = new CancellationTokenSource();
      using var server = new MockServer().Run(cancellation.Token);
      var client = new ObsClientSocket();
      await new ClientFlow().RunClientAsync(server.Uri, client, cancellation: cancellation.Token);
      await new ClientFlow().RunClientAsync(server.Uri, client, cancellation: cancellation.Token);
    }
  }
}
