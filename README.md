# ObsStrawket

Personal .NET implementation of
[obs-websocket](https://github.com/obsproject/obs-websocket) protocol v5.

## Can I use this in production?

No. I verified few events and requests. And I can change interfaces later.

But I won't stop you from using this if this works for you.

## Difference to [obs-websocket-dotnet](https://github.com/BarRaider/obs-websocket-dotnet)

This library uses System.Net.Websockets, compared to websocketsharp used by
obs-websocket-dotnet.

Of course it supports v5 protocol right now.

## Example

```csharp
var client = new ObsClientSocket();
await client.ConnectAsync(new Uri("ws://localhost:4455"), "ahrEYXzXKytCIlpI");

var version = await client.GetVersionAsync(cancellation);
Assert.Contains("bmp", version.SupportedImageFormats);

// Listen specific event.
client.RecordStateChanged += (changed) => {
  switch (changed.OutputState) {
  case OutputState.Unknown:
  case OutputState.Starting:
  case OutputState.Started:
  case OutputState.Stopping:
  case OutputState.Stopped:
  case OutputState.Paused:
  case OutputState.Resumed:
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
await client.StartRecordAsync(cancellation);

await client.CloseAsync();
```
