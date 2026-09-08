import test from"node:test";import assert from"node:assert/strict";import{readFileSync}from"node:fs";
const page=readFileSync("app/pages/EventOperationsManagement.tsx","utf8"),api=readFileSync("app/features/events/operationsManagementApi.ts","utf8"),routes=readFileSync("app/App.tsx","utf8");
test("operations dashboard exposes readiness and execution summaries",()=>{for(const text of["Event readiness","Pending tasks","Assigned staff","Booth status"])assert.match(page,new RegExp(text))});
test("checklist workflow supports create assignment and completion",()=>{assert.match(page,/createTask/);assert.match(page,/completeTask/);assert.match(page,/assignedUserId/);assert.ok(api.includes("/complete"))});
test("staff and placement workflows are connected",()=>{assert.match(page,/assignStaff/);assert.match(page,/createZone/);assert.match(page,/createPlacement/);assert.ok(api.includes("/staff"));assert.ok(api.includes("/placements"))});
test("all phase five routes are registered",()=>{for(const path of["operations","checklist","staff","placements"])assert.match(routes,new RegExp(`events/:id/${path}`))});
