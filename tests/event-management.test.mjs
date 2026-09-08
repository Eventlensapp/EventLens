import test from "node:test";
import assert from "node:assert/strict";
import {readFile} from "node:fs/promises";

const root=new URL("../",import.meta.url);
const read=path=>readFile(new URL(path,root),"utf8");

test("Event Management exposes all Phase 1 frontend routes",async()=>{
  const app=await read("app/App.tsx");
  for(const route of ["/events/create","/events/:id","/events/:id/settings","/events/:id/team"])
    assert.match(app,new RegExp(route.replace(/[/:]/g,m=>`\\${m}`)));
});

test("Event create workflow uses database event types and the core API",async()=>{
  const [form,api]=await Promise.all([read("app/features/events/EventForm.tsx"),read("app/features/events/api.ts")]);
  assert.match(form,/eventApi\.types/);
  assert.doesNotMatch(form,/Wedding|Birthday|Corporate/);
  assert.match(api,/\/api\/event-types/);
  assert.match(api,/\/api\/events/);
});

test("Event dashboard, cloning, and team assignment are connected",async()=>{
  const [dashboard,team,api]=await Promise.all([
    read("app/pages/EventDashboard.tsx"),read("app/pages/EventTeam.tsx"),read("app/features/events/api.ts")
  ]);
  assert.match(dashboard,/eventApi\.dashboard/);
  assert.match(dashboard,/eventApi\.clone/);
  assert.match(team,/eventApi\.assign/);
  assert.match(api,/members/);
});
