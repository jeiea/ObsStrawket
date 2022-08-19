import { obsHeaderPath, protocolJsonPath } from "./commons.ts";

const rawRoot =
  "https://raw.githubusercontent.com/obsproject/obs-websocket/master";

main();

async function main(): Promise<void> {
  await Promise.all([
    saveProtocolJson(),
    saveObsHeader(),
  ]);
}

async function saveObsHeader(): Promise<void> {
  const obsHeader = await fetchText(`${rawRoot}/src/utils/Obs.h`);
  await Deno.writeTextFile(obsHeaderPath, obsHeader);
}

async function saveProtocolJson(): Promise<void> {
  const protocolJson = await requestProtocolJson();
  await Deno.writeTextFile(protocolJsonPath, protocolJson);
}

async function requestProtocolJson(): Promise<string> {
  const url = `${rawRoot}/docs/generated/protocol.json`;
  const json = await fetchText(url);
  // https://github.com/obsproject/obs-websocket/pull/975
  const patch = json.replace("rconfig", "config");
  return patch;
}

async function fetchText(url: string): Promise<string> {
  const response = await fetch(url);
  return response.text();
}
