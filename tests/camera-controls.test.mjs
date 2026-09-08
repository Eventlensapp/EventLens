import test from"node:test";import assert from"node:assert/strict";import fs from"node:fs";
const app=fs.readFileSync("app/App.tsx","utf8"),page=fs.readFileSync("app/pages/BoothCameraControls.tsx","utf8"),adapter=fs.readFileSync("app/features/camera-controls/browserAdapter.ts","utf8"),api=fs.readFileSync("app/features/camera-controls/api.ts","utf8");
test("professional camera control route and panel are connected",()=>{assert.match(app,/booth\/camera-controls/);for(const control of["Zoom","Manual focus","Exposure","Brightness","Contrast","White balance","Torch"])assert.match(page,new RegExp(control,"i"))});
test("unsupported browser controls are capability gated",()=>{assert.match(adapter,/getCapabilities/);assert.match(page,/disabled=\{!cap/);assert.match(page,/Unavailable/)});
test("profile apply reset create and delete are wired",()=>{for(const action of["apply","reset","create","remove"])assert.match(api,new RegExp(`${action}:`));assert.match(page,/Save current profile/)});
test("browser adapter contains no vendor sdk or media recording",()=>{assert.doesNotMatch(adapter,/Canon|Nikon|Sony|MediaRecorder|ImageCapture|toBlob|drawImage/)});
