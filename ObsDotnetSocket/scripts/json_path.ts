import { fromFileUrl } from "https://deno.land/std@0.152.0/path/mod.ts";

export const getJsonPath = () => {
  const jsonUrl = new URL("protocol.json", import.meta.url).href;
  return fromFileUrl(jsonUrl);
};
