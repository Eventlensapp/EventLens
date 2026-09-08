# EventLens AI Feature Roadmap

> **Document type:** Phased product roadmap  
> **Authority:** [`PROJECT_RULES.md`](../PROJECT_RULES.md) governs engineering decisions.  
> **Planning rule:** A phase describes direction and sequencing; it does not authorize implementation or guarantee a date.

## How to Read This Roadmap

| Label | Meaning |
|---|---|
| Foundation present | A meaningful implementation exists in the repository but still requires normal production hardening |
| Partial | Some vertical slices exist; the phase is not complete |
| Planned | Product and architecture direction only |
| P0 | Required for secure, dependable core operation |
| P1 | High product value after its dependencies are stable |
| P2 | Strategic expansion or scale capability |

All phases retain the repository’s ASP.NET Core 9/C# 13 Clean Architecture backend, EF Core 9 with SQL Server, React 19/TypeScript feature architecture, organization tenancy, and RBAC. Every phase must satisfy the definition of done in `PROJECT_RULES.md`.

## Roadmap at a Glance

| Phase | Theme | Current state | Priority |
|---:|---|---|---|
| 1 | Foundation | Foundation present | P0 |
| 2 | Event Management | Foundation present | P0 |
| 3 | Photo Booth Engine | Partial | P0 |
| 4 | Photo Processing | Partial | P1 |
| 5 | Template Studio | Partial | P1 |
| 6 | AI Studio | Partial/provider-dependent | P1 |
| 7 | Gallery Platform | Partial | P0 |
| 8 | Print Engine | Planned | P1 |
| 9 | CRM and Marketing | Partial | P1 |
| 10 | Analytics | Partial | P1 |
| 11 | Billing SaaS | Partial/provider-dependent | P0 |
| 12 | White Label | Planned | P2 |
| 13 | Enterprise | Planned | P2 |
| 14 | Integrations | Abstractions/selected foundations | P1–P2 |
| 15 | Developer Ecosystem | Planned | P2 |

## Phase 1 — Foundation

**Objective:** Establish a secure, auditable multi-tenant SaaS core on which every feature can depend.

### Features

- Registration, login, logout, access tokens, rotating refresh tokens, and password hashing.
- User profiles, active/inactive state, and session lifecycle.
- Roles, permissions, and authorization policies.
- Organization creation, membership, invitations, and ownership.
- SQL Server/SSMS local foundation with reviewed EF Core migrations.
- Base entities, UTC audit fields, soft delete, tenant-aware indexes.
- Structured logging, request correlation, exception handling, validation, Swagger, CORS, rate-limit structure, and secure headers.
- Audit logging for authentication and sensitive tenant operations.

### Dependencies

- Secret-management strategy for non-development JWT keys.
- SQL Server environments and migration deployment process.
- Email adapter for verification, password reset, and invitations.
- Security threat model and tenant-isolation test suite.

### Impact

| Area | Required impact |
|---|---|
| Backend | Consolidate identity and authorization policies; complete verification/reset; add permission service and audit search |
| Frontend | Real session bootstrap, organization selection, invitation/reset screens, protected routes, accessible errors |
| Database | Users, roles, permissions, memberships, refresh tokens, invitations, audit logs, indexes and retention fields |

**Exit criteria:** Automated tenant-boundary tests pass; token rotation/revocation is proven; migrations deploy to clean and upgraded databases; no development secrets are required in production.

**Priority:** P0.

## Phase 2 — Event Management

**Objective:** Make an event the operational aggregate connecting branding, staff, booths, guests, assets, and reporting.

### Features

- Event CRUD, lifecycle states, duplication, archive, search, filtering, sorting, and pagination.
- Event templates and reusable setup presets.
- Event branding: logos, colors, cover assets, messages, languages, and brand-kit inheritance.
- Event QR codes and controlled public event pages.
- Scheduling, time zones, venues, addresses, event types, capacity, and operational limits.
- Multiple booth definitions per event with assigned operators and configuration.
- Event settings for capture, gallery, downloads, sharing, AI, print, and registration.

### Dependencies

- Phase 1 tenant authorization and audit.
- Storage service for event assets.
- Stable public URL strategy and QR signing/expiration rules.

### Impact

| Area | Required impact |
|---|---|
| Backend | Event application service, venue/booth boundaries, public bootstrap endpoints, entitlement checks |
| Frontend | Event list/detail wizard, calendar/timeline, branding preview, booth assignment, public page |
| Database | Events, event settings, venues, booth configurations, event-template links, composite tenant/date indexes |

**Exit criteria:** Owners and delegated managers can create, publish, operate, and archive an event without cross-tenant exposure.

**Priority:** P0.

## Phase 3 — Photo Booth Engine

**Objective:** Deliver a fast, resilient guest capture experience across browser cameras while preserving a professional-camera extension boundary.

### Features

- Browser webcam and mobile camera through `getUserMedia`.
- Permission guidance, device enumeration, front/back switching, HD constraints, mirror mode, live preview, reconnect, and cleanup.
- Session lifecycle, guest association, offline-first state, delayed synchronization, and multiple concurrent booths.
- Countdown options, audio cues, flash, keyboard/touch controls, capture progress, and configurable pauses.
- Capture modes: photo, GIF, boomerang, short video, burst, slow motion, time lapse, and live photo.
- Canvas/OffscreenCanvas high-resolution capture, thumbnails, previews, and 2/3/4-photo strips.
- Future DSLR adapter/bridge with capability discovery and health reporting.

### Dependencies

- Published event bootstrap and booth configuration.
- Browser/device compatibility matrix and kiosk permissions.
- Storage/upload phase, although automatic upload remains an explicit workflow decision.

### Impact

| Area | Required impact |
|---|---|
| Backend | Booth-session and capture registration APIs, idempotent offline sync, booth/device status, SignalR operational events |
| Frontend | Camera adapters, state machine, capture strategies, workers, recovery UX, kiosk shell, accessibility announcements |
| Database | Booth sessions, captures, booth/device definitions, sync identifiers, timestamps, status and event indexes |

**Exit criteria:** Supported devices complete repeatable capture sessions under camera denial, reconnect, and temporary network loss; resources are always released.

**Priority:** P0.

## Phase 4 — Photo Processing

**Objective:** Provide dependable, non-destructive tools for producing high-quality branded output.

### Features

- Crop, rotate, resize, aspect-ratio presets, and output dimensions.
- Filters, exposure/color controls, beauty effects, background blur, and quality presets.
- Frames, borders, watermarks, logos, dates, stickers, and text.
- Preview, undo/redo, comparison, export, batch application, and original preservation.
- Browser Canvas/OffscreenCanvas processing and server ImageSharp processing where appropriate.
- Derived assets, thumbnails, processing history, and output provenance.

### Dependencies

- Phase 3 capture output.
- Storage abstraction and image metadata policy.
- Color, EXIF, privacy, and format requirements.

### Impact

| Area | Required impact |
|---|---|
| Backend | Processing recipes/jobs, secure source/output references, ImageSharp worker, storage quotas |
| Frontend | Non-destructive editor, worker-backed rendering, responsive controls, mobile gestures |
| Database | Photo assets, derived versions, recipes, processing jobs, export records |

**Exit criteria:** Original files remain immutable; repeatable recipes produce verified outputs; large images do not block the UI.

**Priority:** P1.

## Phase 5 — Template Studio

**Objective:** Let designers build reusable, governed experiences for booth, gallery, social, and print output.

### Features

- Template library, categories, preview, premium/entitlement metadata, versions, and duplication.
- Drag-and-drop editor with layers, ordering, locking, grouping, alignment, guides, snap, safe zones, and rulers.
- Images, shapes, masks, frames, text, fonts, stickers, QR placeholders, and photo slots.
- Animation timelines for digital output.
- Brand kit with colors, logos, typography, and approved assets.
- Output variants for vertical/horizontal strips, social formats, screens, and print sizes.

### Dependencies

- Phase 4 renderer/recipe model.
- Asset licensing, font embedding, and storage.
- Entitlement service for premium templates.

### Impact

| Area | Required impact |
|---|---|
| Backend | Template/version services, validation schema, asset management, preview rendering, publication workflow |
| Frontend | Layer editor, inspector, asset browser, keyboard shortcuts, autosave, conflict handling |
| Database | Templates, versions, layers/configuration JSON, brand kits, assets, publications |

**Exit criteria:** A reviewed version renders consistently in browser and server output; published versions cannot be silently changed.

**Priority:** P1.

## Phase 6 — AI Studio

**Objective:** Offer governed, provider-neutral AI creativity without compromising event reliability or cost control.

### Features

- Background removal and replacement.
- Enhancement, upscale, face/portrait refinement, lighting, and color correction.
- Cartoon, anime, family-animation-inspired, sketch, vintage, and other licensed style presets.
- AI props, generative fill, magic/object removal, noise reduction, and smart selection.
- Editable prompts, negative prompts, before/after, job queue, progress, retry, history, and moderation.
- Provider routing based on capability, region, cost, latency, health, and organization entitlement.
- Credit reservation/consumption and idempotent billing.

### Dependencies

- Phase 4 assets and Phase 5 templates.
- Provider adapters and credentials.
- Durable production queue, moderation, privacy review, and cost metering.

### Impact

| Area | Required impact |
|---|---|
| Backend | `IAIProvider` adapters, resolver, durable jobs, retries/dead letters, safe prompt definitions, usage events |
| Frontend | AI workspace, capability discovery, consent/cost messaging, live SignalR progress, output review |
| Database | AI jobs, attempts, prompt versions, provider metadata, outputs, moderation and credit records |

**Exit criteria:** Jobs recover after restart, never double-charge, expose honest provider status, and require review before public publication where policy requires.

**Priority:** P1.

## Phase 7 — Gallery Platform

**Objective:** Deliver event media quickly through branded, privacy-aware, measurable galleries.

### Features

- Public and private galleries, QR access, albums, and collections.
- Live photo arrival, responsive thumbnails, favorites, likes/comments where enabled, sharing, and downloads.
- Password, signed-link, expiration, moderation, download, and social controls.
- Guest-specific retrieval and optional identity/consent gates.
- Gallery branding, SEO/no-index controls, accessibility, and CDN delivery.

### Dependencies

- Event publication, storage/CDN, photo/derived assets.
- Privacy and moderation policy.

### Impact

| Area | Required impact |
|---|---|
| Backend | Gallery authorization, signed URLs, album/collection APIs, interaction events, SignalR publication |
| Frontend | Public responsive gallery, lightbox, search/filter, favorites, sharing and download UX |
| Database | Galleries, albums, collections, photo links, favorites, comments, views, shares, downloads |

**Exit criteria:** Private content cannot be enumerated; large galleries remain fast; interactions feed consent-aware analytics.

**Priority:** P0.

## Phase 8 — Print Engine

**Objective:** Add reliable event-day printing without placing hardware-specific logic in the business layer.

### Features

- Instant printing, printer discovery/management, status, media, and capability reporting.
- Durable print queue, retries, cancellation, priority, duplicate prevention, and operator recovery.
- Print templates, copies, size/crop profiles, color settings, history, and reprint permission.
- Adapter strategy for OS printing and professional devices including DNP, Canon, and Mitsubishi.
- Remote print station/bridge for browser and cloud workflows.

### Dependencies

- Phase 4 rendering and Phase 5 print templates.
- Local bridge security, signed jobs, device certification, and spooler strategy.

### Impact

| Area | Required impact |
|---|---|
| Backend | Print-job orchestration, adapter contracts, station authorization, SignalR status |
| Frontend | Operator queue, printer setup, diagnostics, preview, reprint controls |
| Database | Printers/stations, capabilities, print jobs, attempts, templates, history |

**Exit criteria:** Jobs are idempotent and recoverable; a disconnected printer cannot lose or silently duplicate paid output.

**Priority:** P1.

## Phase 9 — CRM and Marketing

**Objective:** Convert consented event engagement into respectful, actionable customer relationships.

### Features

- Guest database, lead capture, configurable forms, surveys, check-ins, and attendance.
- Consent records, privacy policy versions, tags, segments, preferences, suppression, and data requests.
- Email, SMS, and WhatsApp campaigns.
- Coupons, referrals, loyalty concepts, and attribution.
- Trigger/condition/action automation with drafts, approvals, schedules, retries, and reporting.

### Dependencies

- Foundation identity/tenancy, events, gallery interactions.
- Verified delivery-provider adapters, consent/legal review, unsubscribe and suppression handling.

### Impact

| Area | Required impact |
|---|---|
| Backend | Guest identity resolution, segmentation queries, campaign/automation engine, durable delivery jobs |
| Frontend | Guest profiles/timeline, form builder, segments, campaign composer, journey editor |
| Database | Guests, contacts, consent, forms/responses, tags, segments, campaigns, deliveries, workflows |

**Exit criteria:** Delivery checks current consent at send time; duplicate guests can be safely resolved; every message is traceable.

**Priority:** P1.

## Phase 10 — Analytics

**Objective:** Turn event, booth, gallery, guest, AI, and business activity into timely decisions.

### Features

- Organization dashboard and event, booth, gallery, guest, photo, QR, AI, and revenue analytics.
- Real-time activity through SignalR.
- Hourly activity, funnels, cohorts, device/location dimensions, template popularity, session duration, and utilization.
- Daily, weekly, monthly, custom, and scheduled reports.
- CSV, Excel, and PDF export with reusable line, bar, pie, area, heatmap, and timeline charts.
- Filters for organization, event, date, type, template, and photographer.

### Dependencies

- Consistent event taxonomy from prior modules.
- Privacy-safe analytics event pipeline and aggregation jobs.
- Revenue metrics depend on Phase 11.

### Impact

| Area | Required impact |
|---|---|
| Backend | Metric ingestion, rollups, cache, background aggregation, tenant-safe report/export jobs |
| Frontend | Dashboard, reusable accessible charts, filters, live feed, report center |
| Database | Raw/normalized metric events, hourly/daily aggregates, report definitions/runs, analytics indexes |

**Exit criteria:** Dashboards avoid expensive raw scans; numbers are reproducible, tenant-safe, time-zone-aware, and documented.

**Priority:** P1.

## Phase 11 — Billing SaaS

**Objective:** Enforce sustainable plans, usage limits, and provider-neutral subscription lifecycles.

### Features

- Free, Creator, Business, and Enterprise plan catalog.
- Central feature permissions and quotas; no hard-coded plan checks.
- Trials, grace periods, create/upgrade/downgrade/cancel/pause/resume/renew.
- Metering for events, photos, AI, storage, galleries, downloads, users, API calls, exports, and QR scans.
- Coupons, discounts, referrals, taxes, invoices, payment records, refunds, and dunning.
- Provider abstraction for Stripe, PayPal, Paddle, and Razorpay.
- Signed, idempotent webhooks and audited Super Admin actions.

### Dependencies

- Foundation organizations and audit.
- Reliable usage-event producers in each module.
- Payment provider, tax, currency, invoicing, and legal configuration.

### Impact

| Area | Required impact |
|---|---|
| Backend | Entitlement service, subscription state machine, metering, invoice service, payment/webhook adapters |
| Frontend | Pricing, subscription, usage, invoices, history, payment methods, checkout/upgrade |
| Database | Plans/features, subscriptions, usage, credits, invoices, line items, payments, coupons, webhook inbox |

**Exit criteria:** Concurrency cannot bypass limits; webhook replay is harmless; financial actions are audited and reconcilable.

**Priority:** P0 for entitlement integrity; provider checkout follows.

## Phase 12 — White Label

**Objective:** Let agencies deliver a governed branded platform experience without code forks.

### Features

- Custom domains with ownership verification, certificates, status, and renewal.
- Theme, logos, naming, email identity, gallery/booth branding, and removal of platform marks by entitlement.
- Client and agency portals.
- Reseller hierarchy, delegated tenant provisioning, plan packaging, and usage views.

### Dependencies

- Phase 11 entitlements.
- DNS/certificate automation, email-domain verification, theme token system.

### Impact

| Area | Required impact |
|---|---|
| Backend | Domain resolver, tenant theme/configuration, reseller boundaries, certificate jobs |
| Frontend | Runtime theming, branded auth/portal shells, agency management |
| Database | Domains, verification, themes, email identities, reseller/client relationships |

**Exit criteria:** Hostname resolution cannot leak tenants; branding is accessible; failed certificates have operational alerts.

**Priority:** P2.

## Phase 13 — Enterprise

**Objective:** Meet identity, governance, audit, and operational requirements of large organizations.

### Features

- SSO with Azure AD/Entra ID, Google Workspace, Okta, and standards-based OIDC/SAML.
- Domain discovery, enforced SSO, MFA policy, SCIM provisioning, and group mapping.
- Departments, branches, advanced permissions, approval workflows, and delegated administration.
- Compliance controls, retention/legal hold, audit search/export, regional policy, security reporting, and SLA support.

### Dependencies

- Mature authorization, audit, white-label domains, and enterprise plan.
- Security/legal reviews and identity-provider test environments.

### Impact

| Area | Required impact |
|---|---|
| Backend | Enterprise identity adapters, policy engine, SCIM, governance jobs, immutable audit export |
| Frontend | Enterprise settings, mappings, policy/approval interfaces, audit explorer |
| Database | Identity connections, mappings, policies, organizational units, holds, audit exports |

**Exit criteria:** Enforced SSO has recovery controls; provisioning and deprovisioning are timely; policy decisions are explainable and audited.

**Priority:** P2.

## Phase 14 — Integrations

**Objective:** Connect EventLens AI to customer systems through isolated, observable adapters.

### Features

- Payments: Stripe and PayPal, with Paddle/Razorpay under the billing abstraction.
- Storage/media: AWS S3, Azure Blob Storage, and Cloudinary.
- Automation/collaboration: Zapier, Make, Slack, and Microsoft Teams.
- CRM/marketing: HubSpot, Salesforce, Mailchimp, Twilio, and WhatsApp Business.
- Calendar: Google Calendar and Outlook.
- OAuth connection lifecycle, secret rotation, capability discovery, mapping, retries, health, and audit.

### Dependencies

- Owning module contracts, developer API/webhooks, secret management, durable jobs.
- Provider agreements, rate limits, test tenants, and compliance review.

### Impact

| Area | Required impact |
|---|---|
| Backend | Adapter interfaces, OAuth/token vault, outbox/inbox, retry/dead-letter and health monitoring |
| Frontend | Connection catalog, setup wizard, field mapping, sync status, diagnostics |
| Database | Connections, encrypted credentials/references, mappings, sync cursors, delivery attempts |

**Exit criteria:** Provider failure cannot corrupt core workflows; secrets never reach logs/clients; sync is replay-safe and observable.

**Priority:** P1 for storage and essential delivery; P2 for ecosystem breadth.

## Phase 15 — Developer Ecosystem

**Objective:** Enable customers and partners to build safely on stable EventLens AI contracts.

### Features

- Versioned REST API, granular API keys/scopes, OAuth applications, quotas, and developer portal.
- Signed, retryable webhooks with subscriptions, replay, delivery history, and rotation.
- TypeScript and .NET SDKs, examples, sandbox tenants, changelog, and compatibility policy.
- Plugin marketplace and template marketplace with packaging, review, signing, permissions, versioning, licensing, billing, and rollback.

### Dependencies

- Stable module contracts, Phase 11 usage metering, Phase 14 adapter operations, marketplace legal model.

### Impact

| Area | Required impact |
|---|---|
| Backend | API versioning, key/scopes, webhook platform, SDK schemas, plugin isolation and review metadata |
| Frontend | Developer portal, key/webhook management, logs, marketplace discovery/install/purchase |
| Database | API clients/keys, scopes, webhook subscriptions/deliveries, packages, versions, installs, licenses |

**Exit criteria:** Public contracts have compatibility guarantees; keys are revocable and scoped; plugins cannot bypass tenant or feature permissions.

**Priority:** P2.

## Cross-Phase Delivery Gates

Every phase must pass these gates before it is called production-ready:

1. **Security:** server-side authorization, tenant ownership, abuse controls, secret handling, and threat review.
2. **Data:** reviewed migration, SQL Server query/index review, retention classification, and rollback/forward-fix plan.
3. **Reliability:** idempotency where required, retry policy, operational diagnostics, and failure recovery.
4. **Experience:** responsive loading, empty, error, success, and recovery states; WCAG 2.2 AA targets.
5. **Observability:** structured logs, metrics, trace identifiers, job/provider visibility, and actionable alerts.
6. **Quality:** unit, integration, and critical end-to-end tests plus backend/frontend production builds.
7. **Documentation:** Swagger/API contracts, runbooks, configuration, migration, and user-facing limitations.
8. **Commercial integrity:** entitlement and usage behavior tested wherever a feature is plan-limited.

## Sequencing Guidance

- Phases are numbered by architectural dependency, not strict calendar order.
- Foundation, event management, core booth, gallery safety, and entitlement integrity remain P0.
- Photo processing, templates, AI, CRM, analytics, billing-provider work, and printing may proceed in vertical slices once shared dependencies stabilize.
- White-label, enterprise, broad integrations, and marketplace work should not distort core module boundaries.
- External-provider UI must remain unavailable or clearly unconfigured until a real adapter is operational.
