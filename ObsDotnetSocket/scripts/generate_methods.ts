import { getJsonPath } from "./json_path.ts";

type ObsEvent = {
  description: string;
  eventType: string;
  eventSubscription: string;
  complexity: number;
  rpcVersion: string;
  deprecated: boolean;
  initialVersion: string;
  category: string;
  dataFields: {
    valueName: string;
    valueType: string;
    valueDescription: string;
  }[];
};

type RequestField = {
  valueName: string;
  valueType: string;
  valueDescription: string;
  valueRestrictions: null;
  valueOptional: boolean;
  valueOptionalBehavior: null;
};

type ObsRequest = {
  description: string;
  requestType: string;
  complexity: number;
  rpcVersion: string;
  deprecated: boolean;
  initialVersion: string;
  category: string;
  requestFields: RequestField[];
  responseFields: {
    valueName: string;
    valueType: string;
    valueDescription: string;
  }[];
};

const categoryOrder = [
  "General",
  "Config",
  "Sources",
  "Scenes",
  "Inputs",
  "Transitions",
  "Filters",
  "Scene Items",
  "Outputs",
  "Stream",
  "Record",
  "Media Inputs",
  "Ui",
  "High-Volume",
].map((x) => x.toLowerCase());

const readProtocolJson = () => {
  return Deno.readTextFile(getJsonPath());
};

const toParameterComment = (field: RequestField): string => {
  const { valueName, valueDescription } = field;
  return `/// <param name=${valueName}>${valueDescription}</param>`;
};

const toCSharpType = (type: string, description: string): string => {
  const substitutions = [
    ["Array", "IEnumerable"],
    ["String", "string"],
    ["Any", "object?"],
    ["Boolean", "bool"],
    ["Object", "Dictionary<string, object?>"],
  ];
  const integerSuspicious =
    /index|offset|milli|frame|numerator|denominator|pixel|width|height|quality|\bid\b|number of|version|duration/i;
  substitutions.push(
    description.match(integerSuspicious)
      ? ["Number", "int"]
      : ["Number", "double"],
  );

  return substitutions.reduce(
    (acc, [from, to]) => acc.replaceAll(from, to),
    type,
  );
};

const toParameters = (fields: RequestField[]): string => {
  const parameters = [];
  for (const field of fields) {
    const { valueName, valueOptional, valueType, valueDescription } = field;
    const type = toCSharpType(valueType, valueDescription);
    const nullableType = `${type}${valueOptional ? "?" : ""}`;
    const defaultValue = valueOptional ? " = null" : "";
    parameters.push(`${nullableType} ${valueName}${defaultValue}`);
  }
  parameters.push("bool skipResponse = false");
  parameters.push("CancellationToken? cancellation = null");
  return parameters.join(", ");
};

const toAssignment = (field: RequestField): string => {
  const name = field.valueName;
  const property = name === "requestType"
    ? "vendorRequestType"
    : field.valueName;
  return `${property[0].toUpperCase()}${property.slice(1)} = ${name}`;
};

const toBody = (request: ObsRequest) => {
  const lines = [];

  const { requestType, requestFields } = request;
  if (request.requestType === "TriggerHotkeyByKeySequence") {
    lines.push(
      "public Task TriggerHotkeyByKeySequenceAsync(string? keyId = null, bool? shift = null, bool? control = null, bool? alt = null, bool? command = null, bool skipResponse = false, CancellationToken? cancellation = null) {",
    );
    lines.push(
      "  return _clientSocket.RequestAsync(new TriggerHotkeyByKeySequence() { KeyId = keyId, KeyModifiers = new KeyModifiers() { Shift = shift, Control = control, Alt = alt, Command = command } }, skipResponse, cancellation);",
    );
  } else {
    const hasResponse = request.responseFields.length;
    const returnTypeParameter = `${requestType}Response`;
    const returnType = `Task${hasResponse ? `<${returnTypeParameter}?>` : ""}`;
    const parameters = toParameters(requestFields);
    const prefix = `public ${hasResponse ? "async" : ""}`;
    lines.push(`${prefix} ${returnType} ${requestType}Async(${parameters}) {`);

    const assignments = requestFields.map(toAssignment).join(", ");
    const call =
      `_clientSocket.RequestAsync(new ${requestType}() {${assignments}}, skipResponse, cancellation)`;
    const body = hasResponse
      ? `  return await ${call}.ConfigureAwait(false) as ${returnTypeParameter};`
      : `  return ${call};`;
    lines.push(body);
  }

  return lines;
};

const toMethod = (request: ObsRequest) => {
  const lines = [];

  lines.push("/// <summary>");
  lines.push(...request.description.split("\n").map((x) => `/// ${x}<br />`));
  lines.push(`/// Latest supported RPC version: ${request.rpcVersion}<br />`);
  lines.push(`/// Added in: ${request.initialVersion}`);
  lines.push("/// </summary>");

  lines.push(...request.requestFields.map(toParameterComment));
  lines.push(...toBody(request));
  lines.push("}");

  return "    " + lines.join("\n    ");
};

const makeRequestMethods = (json: string): string => {
  const protocol = JSON.parse(json);
  const requests = protocol.requests as ObsRequest[];
  requests.sort((a, b) =>
    categoryOrder.indexOf(a.category) - categoryOrder.indexOf(b.category)
  );

  const methods = requests.map(toMethod);

  return methods.join("\n\n");
};

const main = async () => {
  const json = await readProtocolJson();
  const result = makeRequestMethods(json);
  console.log(result);
};

main();
