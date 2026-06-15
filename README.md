# ObsStrawket

[![prerelease shield](https://img.shields.io/nuget/vpre/ObsStrawket)](https://www.nuget.org/packages/ObsStrawket) ![Downloads count](https://img.shields.io/nuget/dt/ObsStrawket)

.NET client of [obs-websocket](https://github.com/obsproject/obs-websocket) v5.

## Installation

```powershell
dotnet add package ObsStrawket
```

## Example

```csharp
var client = new ObsClientSocket();

// The client reports internal pipeline activity through this.
client.PipelineEvent += (e) => {
  if (e.Level >= PipelineLevel.Warning) {
    Console.Error.WriteLine(e);
  }
};

bool connected = await client.ConnectAsync(
  new Uri("ws://localhost:4455"),
  "ahrEYXzXKytCIlpI"
);
if (!connected) {
  return;
}

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
