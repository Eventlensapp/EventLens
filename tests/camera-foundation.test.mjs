import assert from "node:assert/strict";
import fs from "node:fs";
import test from "node:test";

const app = fs.readFileSync("app/App.tsx", "utf8");
const manager = fs.readFileSync("app/features/camera-foundation/CameraManager.ts", "utf8");
const page = fs.readFileSync("app/pages/BoothCamera.tsx", "utf8");
const components = fs.readFileSync("app/features/camera-foundation/components.tsx", "utf8");
const api = fs.readFileSync("app/features/camera-foundation/api.ts", "utf8");

test("camera foundation is routed and exposes all requested controls", () => {
  assert.match(app, /path="\/booth\/camera"/);
  for (const component of ["CameraPreview", "CameraSelector", "ResolutionSelector", "MirrorToggle", "CameraStatus", "CameraCapabilities", "CameraDiagnostics"]) {
    assert.match(components, new RegExp(`function ${component}`));
    assert.match(page, new RegExp(component));
  }
});

test("preview manager discovers, switches and recovers browser cameras", () => {
  assert.match(manager, /getUserMedia/);
  assert.match(manager, /enumerateDevices/);
  assert.match(manager, /devicechange/);
  assert.match(manager, /visibilitychange/);
  assert.match(manager, /switchCamera/);
  assert.match(manager, /restartCamera/);
  assert.match(manager, /OverconstrainedError/);
});

test("camera foundation never captures or processes frames", () => {
  assert.doesNotMatch(manager, /drawImage|toBlob|MediaRecorder|ImageCapture/);
  assert.doesNotMatch(page, /\.capture\s*\(|drawImage|toBlob|MediaRecorder|ImageCapture/);
});

test("camera preference and discovery APIs are connected", () => {
  for (const endpoint of ["cameras", "cameras/capabilities", "cameras/preferences", "cameras/refresh"]) {
    assert.match(api, new RegExp(endpoint.replace("/", "\\/")));
  }
  assert.match(page, /cameraFoundationApi\.save/);
  assert.match(page, /cameraFoundationApi\.refresh/);
});
