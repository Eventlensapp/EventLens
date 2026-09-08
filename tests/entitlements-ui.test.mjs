import test from"node:test";import assert from"node:assert/strict";import{readFileSync}from"node:fs";const read=p=>readFileSync(new URL(`../${p}`,import.meta.url),"utf8");
test("admin plan editor exposes limits and feature gates",()=>{const x=read("app/features/entitlements/PlanEditor.tsx");for(const v of["Usage limits","Feature entitlements","maximumEvents","featureKeys"])assert.ok(x.includes(v))});
test("organization subscription shows plans and no-payment notice",()=>{const x=read("app/pages/EntitlementPages.tsx");assert.match(x,/Available plans/);assert.match(x,/No payment is processed/)});
test("usage page displays limits and progress",()=>{const x=read("app/pages/EntitlementPages.tsx");assert.match(x,/entitlementApi\.usage/);assert.match(x,/Unlimited/)});
test("all entitlement routes are registered",()=>{const x=read("app/App.tsx");for(const v of["/admin/plans","/admin/plans/create","/organization/subscription","/organization/usage"])assert.ok(x.includes(v))});
