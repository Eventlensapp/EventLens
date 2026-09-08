import test from"node:test";import assert from"node:assert/strict";import{readFile}from"node:fs/promises";
const source=await readFile(new URL("../app/pages/OrganizationTeam.tsx",import.meta.url),"utf8");
test("team page represents loading and invitation flows",()=>{assert.match(source,/Loading team/);assert.match(source,/teamApi\.invite/);assert.match(source,/Pending invitations/)});
test("team page supports role and removal workflows",()=>{assert.match(source,/teamApi\.role/);assert.match(source,/teamApi\.remove/);assert.match(source,/const isOwner = member\.role === "Owner"/);assert.match(source,/disabled=\{isOwner\}/)});
