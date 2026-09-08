import assert from "node:assert/strict";
import fs from "node:fs";
import test from "node:test";

const app=fs.readFileSync("app/App.tsx","utf8");
const page=fs.readFileSync("app/pages/BoothSessionEngine.tsx","utf8");
const components=fs.readFileSync("app/features/session-engine/components.tsx","utf8");
const api=fs.readFileSync("app/features/session-engine/api.ts","utf8");
const store=fs.readFileSync("app/features/session-engine/store.ts","utf8");

test("session engine route and event-focused components are present",()=>{
 assert.match(app,/path="\/booth\/session"/);
 for(const name of["SessionController","SessionStatus","IdleScreen","OperatorPanel","SessionTimer","RecoveryDialog"]){assert.match(components,new RegExp(`function ${name}`));assert.match(page,new RegExp(name))}
});
test("operator workflow connects every session lifecycle action",()=>{
 for(const action of["create","start","pause","resume","complete","cancel","reset"]){assert.match(api,new RegExp(`${action}:`))}
 assert.match(page,/OperatorPanel/);assert.match(page,/sessionEngineApi\.cancel/);assert.match(page,/sessionEngineApi\.reset/);
});
test("idle mode and recovery are device-local and clear temporary workflow state",()=>{
 assert.match(page,/timeoutSeconds\*1000/);assert.match(store,/eventlens_booth_restore_token/);assert.match(api,/sessions\/recover/);assert.match(store,/localStorage\.removeItem/);
});
test("phase two contains no media capture or processing",()=>{
 for(const source of[page,components,api,store])assert.doesNotMatch(source,/getUserMedia|drawImage|toBlob|MediaRecorder|ImageCapture|photo-processing/);
});
