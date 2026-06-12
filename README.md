# ObsStrawket

[![prerelease shield](https://img.shields.io/nuget/vpre/ObsStrawket)](https://www.nuget.org/packages/ObsStrawket) ![Downloads count](https://img.shields.io/nuget/dt/ObsStrawket)

.NET implementation of [obs-websocket](https://github.com/obsproject/obs-websocket) protocol v5.

## Installation

```powershell
dotnet add package ObsStrawket --prerelease
```

Targets `netstandard2.0` and `net8.0`.

## Example

```csharp
var client = new ObsClientSocket();

// The client reports internal pipeline activity through this.
client.PipelineEvent += (e) => {
  if (e.Level >= PipelineLevel.Warning) {
    Console.Error.WriteLine(e);
  }
};

await client.ConnectAsync(new Uri("ws://localhost:4455"), "ahrEYXzXKytCIlpI");

var version = await client.GetVersionAsync();
Assert.Contains("bmp", version.SupportedImageFormats);

// Listen specific event.
client.RecordStateChanged += (changed) => {
  switch (changed.OutputState) {
  case ObsOutputState.Starting:
  case ObsOutputState.Started:
  case ObsOutputState.Stopping:
  case ObsOutputState.Stopped:
  case ObsOutputState.Paused:
  case ObsOutputState.Resumed:
    break;
  }
};

// Listen all events and filter.
client.Event += (ev) => {
  switch (ev) {
  case RecordStateChanged changed:
    break;
  }
};

await client.StartRecordAsync();
await client.CloseAsync();
```

## Updating generated protocol sources

The source generator normally uses the tracked `SourceGenerator/Upstream/protocol.json` and
`SourceGenerator/Upstream/Obs.h` files without network access. Their obs-websocket commit and
source paths are recorded in `SourceGenerator/Upstream/upstream-revision.json`.

To update both files from the latest obs-websocket default-branch commit and regenerate the
client API:

```powershell
mise update-protocol
```

Other generator tasks (such as regenerating from the tracked revision) are defined in
`mise.toml`; run `mise tasks` to list them.
