import assert from "node:assert/strict";
import { access, readFile } from "node:fs/promises";
import test from "node:test";

const templateRoot = new URL("../", import.meta.url);

async function render() {
  const workerUrl = new URL("../dist/server/index.js", import.meta.url);
  workerUrl.searchParams.set("test", `${process.pid}-${Date.now()}`);
  const { default: worker } = await import(workerUrl.href);
  return worker.fetch(
    new Request("http://localhost/", { headers: { accept: "text/html" } }),
    { ASSETS: { fetch: async () => new Response("Not found", { status: 404 }) } },
    { waitUntil() {}, passThroughOnException() {} },
  );
}

test("server-renders the EventLens application document", async () => {
  const response = await render();
  assert.equal(response.status, 200);
  assert.match(response.headers.get("content-type") ?? "", /^text\/html\b/i);

  const html = await response.text();
  assert.match(html, /<title>Prism AI .* Event Photo Booth Studio<\/title>/i);
  assert.match(html, /name="description"/i);
  assert.match(html, /Event Photo Booth Studio/i);
  assert.match(html, /property="og:title"/i);
  assert.match(html, /name="twitter:card"/i);
  assert.match(html, /href="\/favicon\.svg"/i);
  assert.doesNotMatch(html, /Your site is taking shape|Building your site/i);
});

test("production application no longer depends on starter preview scaffolding", async () => {
  const [page, layout, packageJson] = await Promise.all([
    readFile(new URL("../app/page.tsx", import.meta.url), "utf8"),
    readFile(new URL("../app/layout.tsx", import.meta.url), "utf8"),
    readFile(new URL("../package.json", import.meta.url), "utf8"),
  ]);

  assert.match(page, /<App\s*\/>/);
  assert.match(layout, /generateMetadata/);
  assert.match(layout, /Event Photo Booth Studio/);
  assert.match(packageJson, /"build":\s*"vinext build"/);
  assert.doesNotMatch(page + layout, /SkeletonPreview|codex-preview/);
  await assert.rejects(access(new URL("app/_sites-preview/SkeletonPreview.tsx", templateRoot)));
  await assert.rejects(access(new URL("public/_sites-preview", templateRoot)));
});
