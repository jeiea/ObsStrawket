import { fromFileUrl } from "https://deno.land/std@0.152.0/path/mod.ts";

function getScriptPath(relative: string): string {
  const jsonUrl = new URL(`upstream_sources/${relative}`, import.meta.url).href;
  return fromFileUrl(jsonUrl);
}

export const protocolJsonPath = getScriptPath("protocol.json");
export const obsHeaderPath = getScriptPath("Obs.h");
