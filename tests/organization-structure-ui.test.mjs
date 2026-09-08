import test from"node:test";import assert from"node:assert/strict";import{readFileSync}from"node:fs";
const read=p=>readFileSync(new URL(`../${p}`,import.meta.url),"utf8");
test("department UI implements CRUD and membership flow",()=>{const x=read("app/pages/OrganizationDepartments.tsx");for(const value of["departmentApi.create","departmentApi.update","departmentApi.archive","departmentApi.addMember","departmentApi.removeMember"])assert.match(x,new RegExp(value.replace(".","\\.")))});
test("branch UI implements CRUD and membership flow",()=>{const x=read("app/pages/OrganizationBranches.tsx");for(const value of["branchApi.create","branchApi.update","branchApi.archive","branchApi.addMember","branchApi.removeMember"])assert.match(x,new RegExp(value.replace(".","\\.")))});
test("organization structure routes are registered",()=>{const x=read("app/App.tsx");assert.match(x,/organizations\/:id\/departments/);assert.match(x,/organizations\/:id\/branches/)});
