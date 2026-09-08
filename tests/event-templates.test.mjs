import test from "node:test";
import assert from "node:assert/strict";
import {readFile} from "node:fs/promises";
const root=new URL("../",import.meta.url),read=p=>readFile(new URL(p,root),"utf8");

test("dynamic event type and template routes are registered",async()=>{
  const app=await read("app/App.tsx");
  for(const route of ["/event-types","/event-templates","/event-templates/create","/event-templates/:id"])
    assert.ok(app.includes(`path="${route}"`));
});
test("event type management uses server data and mutation endpoints",async()=>{
  const [page,api]=await Promise.all([read("app/pages/EventTypes.tsx"),read("app/features/events/api.ts")]);
  assert.match(page,/eventApi\.types/);assert.match(page,/eventApi\.createType/);
  assert.match(api,/\/api\/event-types/);
});
test("event creation has choose, configure, and review steps",async()=>{
  const page=await read("app/pages/EventCreate.tsx");
  assert.match(page,/Step \$\{step\} of 3/);assert.match(page,/Blank event/);
  assert.match(page,/Review generated configuration/);assert.match(page,/templateId/);
});
