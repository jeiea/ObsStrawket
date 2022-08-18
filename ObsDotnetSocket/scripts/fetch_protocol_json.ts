import { getJsonPath } from "./json_path.ts";

const url =
  "https://raw.githubusercontent.com/obsproject/obs-websocket/master/docs/generated/protocol.json";

const main = async () => {
  const response = await fetch(url);
  const json = await response.text();
  // https://github.com/obsproject/obs-websocket/pull/975
  const patch = json.replace("rconfig", "config");

  await Deno.writeTextFile(getJsonPath(), patch);
};

main();
