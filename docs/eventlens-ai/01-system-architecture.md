# System Architecture

## 1. Product Boundary

EventLens AI is a multi-tenant event-experience SaaS platform. Its core business hierarchy is:

```text
Platform
└── Organization (tenant and billing boundary)
    ├── Members and invitations
    ├── Subscription, usage and entitlements
    └── Events
        ├── Guests and consent
        ├── Photo sessions and media
        ├── Templates, stickers and branding
        ├── Galleries and live experiences
        ├── AI jobs and memory books
        └── Analytics and marketing activity
```

An organization is the tenant boundary. A user may belong to multiple organizations with a different role in each. Platform-level Super Admin authorization is separate from organization membership.

## 2. Architectural Style

### Initial deployment: modular monolith

Start with a modular monolith using Clean Architecture and explicit bounded contexts. This provides transactional consistency and operational simplicity while preventing accidental coupling. Modules communicate through application contracts and domain/integration events, not direct access to another module's persistence internals.

The initial deployable units are:

- **Web App:** React/TypeScript operator, guest and public-gallery experiences.
- **API Host:** ASP.NET Core REST endpoints and SignalR hubs.
- **Worker Host:** background processing, outbox dispatch, media processing and scheduled tasks.
- **PostgreSQL:** transactional and analytical source data.
- **Object Storage:** photos, videos, GIFs, thumbnails, templates and exports.
- **Cache/Backplane:** Redis when horizontal scale or SignalR fan-out requires it.
- **Email Provider:** verification, invitations, password reset and campaigns.
- **AI Providers:** accessed only behind the AI provider abstraction.
- **Payment Provider:** Stripe-ready adapter behind billing interfaces.

Modules may later be extracted into services when independent scale, fault isolation, team ownership or release cadence justifies the added operational cost.

## 3. Bounded Contexts

| Context | Responsibilities | Owns |
|---|---|---|
| Identity | Registration, login, verification, passwords, access and refresh tokens | Users, credentials, refresh tokens |
| Tenancy | Organizations, members, invitations, roles and permissions | Organizations, members, invitations |
| Billing | Plans, subscriptions, entitlements, usage and payments | Subscriptions, payments, usage ledger |
| Events | Event lifecycle, venue, branding, URLs and device settings | Events, event settings |
| Capture | Sessions, capture modes, device metadata and upload initiation | Photo sessions, capture records |
| Media | Assets, variants, metadata, storage and retention | Photos, media variants |
| Design | Templates, layers, stickers, categories and versions | Templates, template versions, stickers |
| AI Studio | AI requests, orchestration, providers, moderation and costs | AI jobs, outputs, usage |
| Galleries | Public/private access, favorites, downloads and sharing | Galleries, gallery items, shares |
| Live Experience | Realtime event channels, slideshow and notifications | SignalR contracts, ephemeral presence |
| Guests/CRM | Guest identity, consent, feedback and campaigns | Event guests, consent, campaigns |
| Memory Book | Curation, story generation, albums and montage jobs | Memory books, chapters, selections |
| Analytics | Event ingestion, aggregation and reporting | Analytics events, daily aggregates |
| Notifications | In-app, email and operational notifications | Notifications, delivery state |

## 4. Clean Architecture Layers

Each backend module follows:

- **Domain:** aggregates, value objects, domain events, invariants and policies. No infrastructure dependencies.
- **Application:** commands, queries, handlers, validators, DTOs, authorization requirements and interfaces.
- **Infrastructure:** EF Core mappings, repositories, storage/provider adapters, email, AI, billing and messaging.
- **Presentation:** versioned REST endpoints, SignalR hubs, request mapping and transport-specific authorization.

Cross-module reads should use public query contracts. Cross-module writes should use commands or events. Shared code is limited to stable primitives such as IDs, time, pagination, errors, tenancy context and event contracts.

## 5. Frontend Architecture

The frontend is divided into route-level product areas:

- public marketing and pricing;
- authentication and onboarding;
- organization administration;
- event operations dashboard;
- capture station;
- template/editor studio;
- AI studio;
- gallery management;
- public/guest gallery;
- live slideshow/projector;
- CRM and campaigns;
- analytics;
- billing and platform administration.

State ownership:

- **React Query:** all server state, caching, invalidation and optimistic mutations.
- **Zustand:** capture-engine state, editor state, device preferences and short-lived UI workflows.
- **React Router:** route boundaries, organization/event context and guarded routes.
- **Fabric.js or Konva.js:** versioned design document, layers and interactive transforms.
- **Framer Motion:** intentional transitions only; never business-state ownership.

All network access passes through generated or typed API clients. Components do not construct endpoint URLs directly.

## 6. Media and Capture Lifecycle

```text
Device capture
  → local preview and optional local processing
  → request signed upload
  → upload original directly to object storage
  → finalize media record
  → enqueue thumbnails/normalization/moderation
  → publish MediaReady integration event
  → update gallery and SignalR subscribers
  → optional AI/design/export jobs
```

Principles:

- Media bytes should not transit the API unless a provider limitation requires it.
- The API issues short-lived, content-type- and size-restricted upload grants.
- Original assets are immutable; edits create variants with provenance.
- Database records store object keys, checksums, MIME type, size, dimensions and processing state—not public provider URLs.
- Provider-specific object storage logic is behind `IObjectStorage`.
- S3, Azure Blob and Cloudinary adapters implement the same application contracts.
- Retention and deletion operate through asynchronous, auditable jobs.

## 7. AI Processing Architecture

`IAIImageProvider` is the provider-neutral boundary. Application use cases submit normalized jobs:

```text
AIJob
├── operation: enhance | remove-background | style-transfer | add-props | upscale
├── input media/version
├── normalized parameters
├── provider selection policy
├── safety/moderation status
├── cost and usage reservation
└── output media/version or failure
```

The pipeline:

1. validates tenant entitlement and event policy;
2. reserves usage atomically;
3. validates consent and media ownership;
4. records an idempotent queued job;
5. selects a provider based on capability, region, cost and health;
6. processes asynchronously with bounded retries;
7. stores immutable output and provenance;
8. settles usage and provider cost;
9. publishes completion through SignalR and notifications.

AI requests never execute inline in a user-facing HTTP transaction. Provider keys remain server-side. Raw biometric inference should not be introduced without a separate privacy and legal review.

## 8. Realtime Architecture

SignalR hubs:

- `/hubs/events` — operator notifications and event health;
- `/hubs/galleries` — new/updated gallery items;
- `/hubs/slideshow` — projector playback and moderation state;
- `/hubs/jobs` — AI/export job progress for authorized operators.

Group names use opaque internal IDs, not slugs. Joining a group requires server-side tenant and event authorization. Public galleries receive only approved, published item payloads. Redis backplane or a managed SignalR service is introduced before multi-instance production scale.

## 9. Background Processing

The Worker Host runs:

- outbox publishing;
- media normalization and thumbnails;
- AI orchestration;
- GIF/video/boomerang assembly;
- strip and PDF exports;
- gallery publication updates;
- email verification and campaigns;
- analytics aggregation;
- memory-book curation;
- retention/deletion;
- subscription reconciliation.

Jobs use idempotency keys, leases, retry policies and dead-letter state. Business changes and outbox messages commit in the same PostgreSQL transaction.

## 10. Security Architecture

- Short-lived JWT access tokens; rotated, hashed refresh tokens with token-family reuse detection.
- Email verification before privileged organization actions.
- Passwords hashed using ASP.NET Core Identity's current strong defaults.
- Role plus permission-policy authorization; never rely on frontend guards.
- Tenant resolution from authenticated membership and route resource, not an untrusted tenant header alone.
- Every tenant-owned query includes `OrganizationId`; defense-in-depth query filters are supplemented by explicit authorization tests.
- Rate limits for login, password reset, invitations, public gallery access and upload grants.
- Signed upload/download URLs with short expiry.
- Content validation uses MIME sniffing, size limits and media decoding; filenames are never trusted.
- CSRF is relevant when credentials are cookie-based; refresh tokens should use secure, HTTP-only, same-site cookies where architecture permits.
- Secrets come from a production secret store.
- Audit records cover authentication, membership, event publication, media deletion, gallery privacy and billing changes.
- Guest consent, marketing consent and photo release are separate timestamped records.
- Privacy workflows include tenant export, guest removal, media retention and account deletion.

## 11. Reliability and Observability

- RFC 9457 Problem Details for API failures.
- Correlation, request and job IDs propagated through logs and events.
- Structured Serilog output with sensitive-field redaction.
- OpenTelemetry traces, metrics and logs.
- Health probes distinguish liveness, readiness and dependency health.
- Idempotency keys on capture finalization, AI submission, payment webhooks and exports.
- Optimistic concurrency tokens on mutable aggregates.
- Time stored as UTC; organization/event timezone stored as an IANA timezone.
- Database migrations run as an explicit release step, not automatically on every application startup.
- Backup, point-in-time recovery and object-storage lifecycle policies are environment requirements.

## 12. Deployment Topology

```text
Internet
   │
CDN / WAF
   │
Nginx / ingress
   ├── React static application
   ├── ASP.NET Core API + SignalR
   └── signed media delivery

API/Worker private network
   ├── PostgreSQL
   ├── Redis
   ├── Object storage
   └── external adapters (AI, email, billing)
```

Containers:

- `eventlens-web`
- `eventlens-api`
- `eventlens-worker`
- `eventlens-migrations`
- `nginx`
- local-development PostgreSQL, Redis and S3-compatible storage

CI/CD stages: restore → static checks → unit tests → architecture tests → integration tests → frontend tests → container build → security scan → migration review → staging deployment → smoke/E2E tests → approved production rollout.

## 13. Extraction Triggers

Keep modules in the modular monolith until measurable conditions justify extraction. Likely first candidates are Media/AI workers and Analytics because their scaling and failure profiles differ from transactional APIs. Billing and Identity should remain carefully controlled boundaries even if they share the initial deployment.

