import test from"node:test";import assert from"node:assert/strict";import{readFile}from"node:fs/promises";
const source=await readFile(new URL("../app/pages/OrganizationSecurity.tsx",import.meta.url),"utf8");
test("access page supports permission administration",()=>{assert.match(source,/securityApi\.permissions/);assert.match(source,/securityApi\.set/);assert.match(source,/Member permissions/)});
test("access page supports ownership acceptance and cancellation",()=>{assert.match(source,/securityApi\.start/);assert.match(source,/securityApi\.accept/);assert.match(source,/securityApi\.cancel/)});
