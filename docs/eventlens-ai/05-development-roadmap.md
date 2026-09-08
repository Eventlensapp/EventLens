# Development Roadmap

## Delivery Principles

- Build vertical, deployable slices rather than completing every database table before user value.
- Keep every phase behind an explicit acceptance gate.
- Establish security, tenant isolation, observability and testing before high-volume media workflows.
- Treat AI, billing, email, storage and DSLR support as adapters with contract tests.
- Release capture and media workflows on real target devices early.
- Do not promise production dates until Phase 0 discovery resolves provider, compliance and hosting choices.

## Phase 0 — Discovery and Architecture Approval

### Outcomes

- approve bounded contexts and modular-monolith approach;
- define MVP personas and prioritized workflows;
- select Fabric.js or Konva.js through a focused prototype;
- select job processing, email, object storage, AI and billing providers;
- document privacy, consent, retention and content moderation requirements;
- define service-level objectives and deployment environments;
- approve the API, tenant model and data architecture in this package.

### Deliverables

- signed architecture decision records;
- threat model and data classification;
- UX flows for organization onboarding, event creation, capture and gallery;
- initial OpenAPI contract;
- product analytics taxonomy;
- prioritized backlog with estimates.

### Gate

No production module work starts until tenant isolation, identity flow and media lifecycle are approved.

## Phase 1 — Platform Foundation

### Scope

- monorepo and CI pipeline;
- API and Worker hosts;
- React application shell;
- PostgreSQL migrations and local object-storage environment;
- structured logging, OpenTelemetry and health checks;
- Problem Details and validation pipeline;
- transactional outbox;
- identity: register, login, email verification, refresh rotation, reset password;
- organizations, membership, invitation and role policies;
- plan/entitlement read model with development plans.

### Tests

- architecture dependency tests;
- identity abuse and refresh-token reuse tests;
- cross-tenant authorization integration suite;
- migration and rollback checks;
- API contract tests.

### Exit Criteria

A verified user can create an organization, invite a teammate, switch organization context and access only authorized tenant resources.

## Phase 2 — Event Operations and Capture MVP

### Scope

- event CRUD, types, venue, timezone, branding and slug;
- event device registration and health;
- browser camera permission and device selection;
- front/rear camera, webcam, mirror and countdown;
- photo mode and burst capture;
- photo sessions;
- signed direct uploads;
- media finalization, thumbnails and normalization;
- basic 2/3/4-photo strip generation;
- event QR code.

### Tests

- target matrix: iOS Safari, Android Chrome, desktop Chrome/Edge/Safari;
- interrupted upload, duplicate finalize and offline recovery;
- image size/type security;
- capture performance and memory pressure.

### Exit Criteria

An operator can configure an event, open a booth on a supported browser, capture a multi-shot session, generate a strip and view the uploaded result.

## Phase 3 — Editor and Gallery

### Scope

- canvas document schema and versioning;
- layer selection, move, resize, rotate, crop and ordering;
- text, stickers, backgrounds, frames and filters;
- tenant template creation and immutable published versions;
- PNG/JPEG and print-ready PDF rendering;
- public/private galleries;
- gallery moderation and item publishing;
- guest QR access, favorites, download and share tracking;
- live gallery notifications through SignalR;
- projector/slideshow mode.

### Exit Criteria

An event manager can design a reusable branded template, apply it to captures, publish moderated photos and operate a realtime guest gallery/slideshow.

## Phase 4 — AI Studio

### Scope

- provider-neutral AI contracts and capability catalog;
- AI job queue, progress, cancellation and retry;
- entitlement reservation and cost ledger;
- enhancement, background removal and upscale;
- approved style presets: anime, cartoon, cinematic, vintage, fantasy and royal;
- background presets and custom generation;
- props pipeline;
- safety/moderation controls;
- immutable AI provenance and outputs.

### Rollout

1. internal test organization;
2. opted-in beta tenants with quotas;
3. provider failover and cost alerts;
4. general availability by plan.

### Exit Criteria

AI jobs are asynchronous, observable, metered, provider-independent and never allow one tenant to access another tenant's inputs or outputs.

## Phase 5 — Guest CRM and Marketing

### Scope

- configurable guest capture forms;
- separate consent records;
- encrypted contact fields;
- feedback;
- guest filtering and export;
- transactional follow-up email;
- consent-aware campaigns, scheduling and reporting;
- suppression, unsubscribe and provider webhook processing.

### Gate

Legal review of consent copy, retention, unsubscribe and customer data-processing obligations.

## Phase 6 — Analytics and Operations

### Scope

- analytics event ingestion and daily aggregation;
- event and organization dashboards;
- visitors, captures, downloads, shares, template popularity, AI usage and engagement;
- report exports;
- operational admin console;
- queue/provider/storage health;
- tenant usage and support diagnostics;
- audit log views.

### Exit Criteria

Metrics are traceable to defined events, tenant-scoped, reproducible and reconciled against usage/billing where applicable.

## Phase 7 — Billing and Commercialization

### Scope

- FREE, CREATOR, BUSINESS and ENTERPRISE catalog;
- checkout and customer portal adapter;
- signed webhook ingestion;
- subscription reconciliation;
- feature permissions and usage enforcement;
- invoices and payment status;
- grace periods, downgrade behavior and overage policy;
- white-label entitlements.

### Exit Criteria

Entitlements are enforced server-side, webhooks are idempotent, usage can be reconciled and subscription failure modes are defined.

## Phase 8 — AI Memory Book

### Scope

- best-moment ranking signals;
- duplicate/quality filtering;
- human-in-the-loop selection;
- event story and captions;
- chapters/highlight collection;
- album export;
- short montage assembly with licensed audio handling;
- approval and regeneration workflows.

### Gate

Evaluation dataset, quality rubric, consent review and cost envelope approved before general availability.

## Phase 9 — Advanced Capture and Ecosystem

### Scope

- GIF, boomerang and video capture;
- offline-first capture queue and reconciliation;
- DSLR local bridge and signed device pairing;
- custom layouts and advanced print workflows;
- template marketplace moderation and licensing;
- S3, Azure Blob and Cloudinary production adapters;
- enterprise SSO and advanced organization policies;
- regional storage/data residency where commercially required.

## Workstreams Across All Phases

### Security

- threat-model updates;
- dependency and container scanning;
- secret scanning;
- authorization regression suite;
- rate-limit and abuse testing;
- penetration test before commercial launch.

### Quality

- domain unit tests;
- API integration tests with PostgreSQL;
- storage/AI/billing adapter contract tests;
- frontend component and accessibility tests;
- Playwright critical-path tests;
- load tests for uploads, galleries and SignalR.

### Operations

- dashboards and alerts;
- queue-depth and job-age objectives;
- backup/restore drills;
- runbooks;
- incident response;
- staged rollouts and rollback procedures.

## Proposed MVP Boundary

The first commercially testable release should include Phases 0–3 plus a limited enhancement-only slice from Phase 4:

- identity and organizations;
- event creation and branding;
- browser photo/burst capture;
- 2/3/4-shot strips;
- basic editor and template;
- direct media upload;
- public/private QR gallery;
- live gallery/slideshow;
- basic event analytics;
- one metered AI enhancement capability.

CRM campaigns, full AI styles, billing automation, memory books, video and DSLR integration should not block MVP validation.

## Initial Backlog Order

1. Architecture decisions and threat model.
2. Repository, CI and observability baseline.
3. Identity and tenant authorization.
4. Organization/event vertical slice.
5. Upload/media pipeline.
6. Camera proof on target devices.
7. Capture session and strip vertical slice.
8. Gallery publication and QR guest flow.
9. SignalR live update.
10. Editor document/render proof.
11. Entitlement and usage framework.
12. AI enhancement adapter.

## Definition of Done

A production feature is done only when:

- domain rules and tenant authorization are server-enforced;
- validation and Problem Details are defined;
- audit/analytics events are identified;
- logs contain no secrets or sensitive media URLs;
- unit, integration and relevant E2E tests pass;
- accessibility and responsive behavior are verified;
- migrations are reviewed;
- operational metrics and failure behavior exist;
- API and product documentation are updated;
- rollout and rollback paths are documented.

## Approval Checklist

- [ ] Product approves MVP boundary and user journeys.
- [ ] Engineering approves module boundaries and extraction triggers.
- [ ] Security approves identity, tenant isolation, uploads and public gallery access.
- [ ] Legal/privacy approves guest consent, retention and AI handling.
- [ ] Operations approves deployment topology, SLOs and recovery objectives.
- [ ] Finance/product approves plan entitlements and AI cost controls.
- [ ] Design approves editor engine prototype and template document model.

