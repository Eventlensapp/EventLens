import assert from "node:assert/strict";
import fs from "node:fs";
import test from "node:test";

const app = fs.readFileSync("app/App.tsx", "utf8");
const page = fs.readFileSync("app/pages/BoothFoundation.tsx", "utf8");
const runtime = fs.readFileSync("app/features/booth-foundation/runtime.ts", "utf8");
const api = fs.readFileSync("app/features/booth-foundation/api.ts", "utf8");

test("booth foundation exposes all required protected routes", () => {
  for (const route of ["/booth", "/booth/setup", "/booth/settings", "/booth/diagnostics"]) {
    assert.match(app, new RegExp(`path="${route.replace("/", "\\/")}"`));
  }
});

test("hardware inspection remains metadata-only in phase zero", () => {
  assert.match(runtime, /enumerateDevices/);
  assert.doesNotMatch(runtime, /getUserMedia\s*\(/);
  assert.doesNotMatch(runtime, /new\s+MediaRecorder|MediaRecorder\s*\(/);
});

test("browser-owned permission and offline foundations are initialized", () => {
  assert.match(runtime, /\.permissions\.query/);
  assert.match(runtime, /indexedDB\.open/);
  assert.match(page, /PermissionList/);
  assert.match(api, /permissions\/request/);
});

test("configuration, diagnostics and runtime state use the booth API", () => {
  for (const endpoint of ["configuration", "diagnostics", "state", "offline"]) {
    assert.match(api, new RegExp(`/api/booth/${endpoint}`));
  }
});
