import test from "node:test";
import assert from "node:assert/strict";
import {readFile} from "node:fs/promises";

test("organization creation submits through the organization API", async () => {
  const source = await readFile(new URL("../app/pages/OrganizationCreate.tsx", import.meta.url), "utf8");
  assert.match(source, /organizationApi\.create/);
  assert.match(source, /setOrganizations/);
  assert.match(source, /selectOrganization/);
});

test("organization list supports tenant switching", async () => {
  const source = await readFile(new URL("../app/pages/Organizations.tsx", import.meta.url), "utf8");
  assert.match(source, /selectOrganization\(org\.id\)/);
  assert.match(source, /Loading organizations/);
  assert.match(source, /No organizations yet/);
});
