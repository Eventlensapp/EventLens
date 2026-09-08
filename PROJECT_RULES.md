# EventLens AI — Project Rules

> Permanent engineering guide for human and AI-assisted development.
>
> **Status:** Authoritative  
> **Applies to:** Backend, frontend, database, infrastructure, tests, scripts, and documentation  
> **Primary rule:** Existing behavior and public contracts must be preserved unless a task explicitly authorizes a breaking change.

## 1. Project Overview

### Product vision

EventLens AI is a production-grade, multi-tenant event-photo SaaS platform. It enables organizations to create events, operate browser-based photo booths, process and enhance photos, publish galleries, engage guests, and turn event interactions into measurable business value.

The product should feel fast and approachable at an event while remaining secure, auditable, and operationally dependable for organizations managing many events.

### Target users

| User | Primary needs |
|---|---|
| Event companies and planners | Manage events, staff, branding, guests, and delivery |
| Photographers and booth operators | Reliable capture, session management, processing, and upload |
| Marketing agencies and brands | Lead capture, consent, campaigns, analytics, and conversion reporting |
| Wedding businesses | Branded experiences, guest galleries, sharing, and follow-up |
| Corporate teams | Controlled access, auditability, reporting, and branded activations |
| Schools and community organizations | Simple event setup, privacy, and accessible guest flows |
| Shopping malls and public activations | High-throughput kiosk operation and measurable engagement |
| Guests | Quick capture, editing, gallery access, download, and sharing |
| Platform administrators | Tenant support, safety, provider operations, and platform health |

### Business goals

- Deliver a dependable self-service event-photo platform.
- Convert event engagement into consent-aware customer relationships.
- Support recurring revenue through tiered subscriptions and usage-based capabilities.
- Reduce operator setup time and event-day failure rates.
- Provide extensible AI without locking the platform to one model provider.
- Scale from one booth to multi-event, multi-organization operations.
- Maintain strong tenant isolation, privacy, security, and auditability.

### Competitor inspiration

EventLens AI may learn from, but must not copy, the following products:

| Product | Inspiration |
|---|---|
| DSLRBooth | Professional booth workflows, camera reliability, and print-oriented operation |
| FotoShare Cloud | Cloud galleries, sharing, remote event delivery, and guest engagement |
| Simple Booth | Accessible setup, polished guest experience, and live-event activation |
| Snappic | Branded activations, lead capture, analytics, and marketing integrations |

### Differentiators

- Browser-first booth engine with offline-first capture and future DSLR adapters.
- One platform for capture, processing, gallery, CRM, automation, and analytics.
- Provider-neutral AI architecture with queued processing and auditable job history.
- Flexible, consent-aware lead capture and guest timelines.
- Organization-scoped multi-tenancy and role-based operations.
- Professional photo-strip and editor workflows using modern browser APIs.
- API-first backend that supports future web, kiosk, mobile, and partner clients.

## 2. Technology Stack

| Layer | Standard |
|---|---|
| Frontend | React 19, TypeScript, Vinext/Vite, React Router, React Query, Zustand |
| Styling | Tailwind CSS where feature architecture supports it; existing shared CSS design tokens and classes remain valid |
| Backend | ASP.NET Core 9 Web API, C# 13, Clean Architecture |
| Data access | Entity Framework Core 9, repository pattern, unit of work |
| Database | Microsoft SQL Server; SSMS for local administration |
| Authentication | JWT bearer access tokens, rotating refresh tokens, secure refresh cookie support |
| Validation/mapping | FluentValidation and AutoMapper |
| Observability | Serilog, structured request/error/authentication logging |
| API documentation | Swagger/OpenAPI with bearer authentication |
| Image processing | Browser Canvas/OffscreenCanvas; ImageSharp for server-side processing |
| QR generation | QRCoder |
| Client state | React Query for server state; Zustand for client/workflow state |
| AI | Provider abstractions, background queue, hosted worker, configurable HTTP providers |
| Storage | `IStorageService`; local storage for development, object-storage adapter for production |
| Deployment | ASP.NET service/container or managed compute, SQL Server/Azure SQL, object storage/CDN; frontend supports Sites/Cloudflare-compatible builds |

Do not introduce a competing framework or datastore without an architecture decision record (ADR) and explicit approval.

## 3. Solution Architecture

### Backend Clean Architecture

```text
EventLensAI.Domain
    ↑
EventLensAI.Application
    ↑
EventLensAI.Infrastructure
    ↑
EventLensAI.API
```

| Project | Responsibility |
|---|---|
| `EventLensAI.Domain` | Entities, value objects, enums, domain invariants, domain events |
| `EventLensAI.Application` | Use-case contracts, DTOs, validators, mappings, service interfaces, application exceptions |
| `EventLensAI.Infrastructure` | EF Core, repositories, identity, storage, providers, queues, external integrations |
| `EventLensAI.API` | HTTP transport, controllers, middleware, filters, configuration, Swagger |

### Frontend feature-based architecture

```text
app/
  components/             # Shared presentation and layout
  features/
    <feature>/
      api.ts
      types.ts
      components/
      hooks/
      store/
  pages/                  # Route-level composition
  store/                  # Truly global client state
```

Photo booth code remains under `app/booth` while it is a distinct runtime workflow. New substantial modules belong under `app/features/<module>`.

### Multi-tenant architecture

- `Organization` is the tenant boundary.
- Tenant-owned records must contain `OrganizationId`, or inherit tenant scope unambiguously through an authorized aggregate such as `Event`.
- Every tenant-scoped operation must validate current-user membership server-side.
- Client-provided organization, event, guest, or resource IDs are untrusted.
- SuperAdmin bypasses must be explicit, audited, and tested.
- Do not rely on frontend route guards for data isolation.

### Service boundaries

Modules own their domain behavior and public application contracts. Current boundaries include:

- Identity and access
- Organizations and membership
- Events and branding
- Booth sessions and capture registration
- Photo processing and templates
- Galleries and sharing
- AI processing
- CRM and marketing
- Analytics and reporting
- Billing and entitlements

Cross-module communication should use application interfaces, domain/application events, or durable jobs—not direct controller-to-controller calls.

### Dependency rules

- Domain references no other solution project.
- Application may reference Domain only.
- Infrastructure may reference Application and Domain.
- API composes Application and Infrastructure.
- Frontend never imports backend implementation code.
- Domain entities must not depend on ASP.NET, EF Core, logging, HTTP, or provider SDKs.
- External SDK types must not leak into application DTOs.
- Circular project or feature imports are prohibited.

## 4. Coding Standards

### Core principles

- **SOLID:** Keep responsibilities narrow; depend on abstractions at external boundaries.
- **DRY:** Share stable knowledge, not coincidental syntax.
- **Clean Code:** Prefer readable intent, small cohesive methods, early validation, and explicit state transitions.
- **YAGNI:** Build extension seams required by the roadmap; do not add speculative provider logic.
- **Compatibility:** Preserve existing APIs, migrations, stored data, and user workflows unless explicitly changing them.

### Naming

| Item | Convention | Example |
|---|---|---|
| C# types/methods/properties | PascalCase | `BoothSessionService` |
| C# parameters/locals | camelCase | `organizationId` |
| Interfaces | `I` prefix | `IStorageService` |
| Async methods | `Async` suffix | `GetByIdAsync` |
| TypeScript components/types | PascalCase | `PhotoEditor`, `GuestDto` |
| TypeScript functions/variables | camelCase | `loadGuests` |
| React hooks | `use` prefix | `useBoothStore` |
| Constants | descriptive; language-native casing | `RefreshCookie`, `MAX_RETRIES` |
| Database tables | snake_case plural | `booth_sessions` |
| Database columns | EF-defined PascalCase unless a migration establishes otherwise | `OrganizationId` |
| API routes | lowercase kebab-case nouns | `/api/photo-processing` |

### Files and folders

- One primary public type or cohesive type family per file.
- C# filenames use PascalCase and match the primary type.
- React component filenames use PascalCase; hooks and utilities use camelCase.
- Avoid generic dumping grounds such as `Helpers`, `Utils`, or `Common` unless the contents are genuinely cross-cutting.
- Place code in the owning module, not the most convenient folder.

### Comments and documentation

- Comments explain *why*, invariants, browser/provider limitations, or security decisions.
- Do not narrate obvious code.
- XML documentation is required for public APIs where Swagger or SDK consumers benefit.
- Each new external integration requires configuration and operational documentation.
- Architectural deviations require an ADR under `docs/architecture/`.
- Update README/run instructions when setup changes.

## 5. Backend Standards

### Controllers

- Controllers translate HTTP to application calls; they do not contain business logic or EF queries.
- Use `[ApiController]`, explicit routes, appropriate authorization policies, and cancellation tokens.
- Return `ApiResponse<T>` consistently.
- Document response types and non-obvious behavior for Swagger.
- Use resource-oriented status codes: `201` create, `202` queued, `204` empty success where appropriate.

```csharp
[HttpGet("{id:guid}")]
public async Task<ActionResult<ApiResponse<EventDto>>> Get(
    Guid id,
    CancellationToken cancellationToken) =>
    Ok(ApiResponse<EventDto>.Ok(
        await service.GetAsync(id, cancellationToken)));
```

### Services

- Application services implement use cases and enforce business/tenant rules.
- Keep interfaces in Application and implementations in the appropriate layer.
- Accept `CancellationToken` on all I/O-bound operations.
- External delivery must occur through abstractions and durable background jobs.
- No fake production behavior: disabled providers must fail clearly or remain unavailable.

### Repositories and unit of work

- Use repositories for aggregate-specific persistence queries.
- Use `IRepository<T>` only for genuinely generic CRUD.
- Complex filters belong in focused repositories/specifications, not controllers.
- A use case commits once through `IUnitOfWork.SaveChangesAsync`.
- Never expose `IQueryable` outside the persistence boundary.

### DTOs

- Do not expose EF entities directly.
- Request and response contracts are separate when their responsibilities differ.
- Avoid provider-specific and persistence-specific types.
- Treat public DTO changes as API contract changes.

### Entity Framework Core

- Use `EventLensDbContext`.
- Define entity mappings with `IEntityTypeConfiguration<T>`.
- Specify lengths, precision, required fields, indexes, delete behavior, and table names.
- Use `AsNoTracking` for read-only queries.
- Avoid N+1 queries and unbounded materialization.
- Use SQL Server-compatible cascade rules; prevent cycles and multiple cascade paths with `Restrict`/`NoAction`.
- Never edit an applied migration. Create a new migration.
- Generate and review SQL before production rollout.

### FluentValidation

- Validate transport/application requests with FluentValidation.
- Validators must cover required values, bounds, formats, date relationships, and JSON/schema validity.
- Domain entities still enforce invariants; validation is not a replacement for domain rules.

### AutoMapper

- Use AutoMapper for stable, mechanical mappings.
- Prefer explicit mapping when authorization, aggregation, localization, or computed fields are involved.
- Mapping profiles live in Application.

### Exceptions and middleware

- Use application exceptions such as `NotFoundException`, `UnauthorizedException`, and `ConflictException`.
- `ExceptionMiddleware` is the single HTTP error translation boundary.
- Never return stack traces, SQL details, tokens, secrets, or internal provider responses.
- Log unexpected and database errors with the trace identifier.

### Logging

- Use structured Serilog templates: `logger.LogInformation("Event {EventId} published", eventId)`.
- Log requests, errors, authentication events, external job transitions, and database failures.
- Never log passwords, JWTs, refresh tokens, API keys, full consent payloads, or sensitive personal data.
- Include tenant/resource IDs where safe and useful.

### API versioning and Swagger

- Current routes are unversioned during the foundation phase.
- Before publishing external client contracts, adopt `/api/v1/...` or header-based versioning consistently.
- Breaking changes require a new API version and migration guidance.
- Swagger must expose bearer authorization and accurate schemas, summaries, response codes, and examples for complex requests.

## 6. Frontend Standards

### React and TypeScript

- Use function components and hooks.
- TypeScript strictness must not be weakened.
- Avoid `any`; use `unknown` plus narrowing at trust boundaries.
- Keep render logic pure and side effects in hooks/event handlers.
- Lazy-load heavy route features such as editors and AI workspaces.

### Tailwind CSS and shared styling

- Prefer reusable semantic components and design tokens over repeated utility strings.
- Tailwind may be used in feature code, but do not rewrite established shared CSS solely for consistency.
- Avoid arbitrary colors and spacing when a token exists.

### State ownership

| State | Tool |
|---|---|
| Remote/API state | React Query |
| Booth/editor workflow state | Feature-scoped Zustand |
| Small local UI state | React `useState`/`useReducer` |
| URL/filter state | React Router/search parameters |

- Do not duplicate React Query data into Zustand.
- Stores expose actions rather than allowing arbitrary mutation.
- Persist only necessary non-sensitive state.

### React Query

- Use stable query-key factories per feature.
- Invalidate the narrowest relevant keys after mutations.
- Represent loading, empty, error, and stale states intentionally.
- Cancel obsolete requests where practical.

### React Router

- Routes are composed in `App.tsx` until route modules justify extraction.
- Route protection improves UX but does not replace API authorization.
- Deep links and browser navigation must work.

### Components and hooks

- Route pages compose features; they should not become business-logic containers.
- Components should have focused props and accessible behavior.
- Custom hooks own reusable stateful behavior.
- Browser resources—camera streams, timers, workers, subscriptions—must be released during cleanup.

### Forms

- Prefer schema-driven reusable fields for lead capture and surveys.
- Provide inline validation, submission state, and an error summary when appropriate.
- Never imply consent through preselected marketing checkboxes.
- Conditional fields must remain keyboard and screen-reader accessible.

### Error boundaries

- Add error boundaries around route-level and heavy asynchronous features.
- Show a recoverable user message and log a traceable diagnostic.
- Do not display raw server/provider errors.

### Responsive design and accessibility

- Design mobile-first; support tablet kiosk and desktop administration.
- Target WCAG 2.2 AA.
- Use semantic HTML, labels, visible focus, sufficient contrast, and keyboard operation.
- Touch targets should generally be at least 44×44 CSS pixels.
- Respect reduced-motion preferences.
- Camera workflows require non-visual status announcements and keyboard shortcuts.

## 7. Database Standards

### Base entity and audit fields

Tenant entities generally inherit `BaseEntity`:

```text
Id
CreatedAt
UpdatedAt
CreatedBy
UpdatedBy
IsDeleted
```

- IDs are GUIDs generated by the application unless a module explicitly requires another strategy.
- Store timestamps in UTC.
- `EventLensDbContext.SaveChangesAsync` owns automatic audit timestamps and actor IDs.

### Soft delete

- Use soft deletion for recoverable business data.
- Apply global query filters where appropriate.
- Repository `Delete` operations should call the domain soft-delete behavior.
- Security, privacy, and retention policies may require irreversible purge jobs; those must be explicit and audited.
- Unique indexes must account for soft-deleted records when business rules require reuse.

### Relationships

- Configure every important relationship explicitly.
- Required dependents do not automatically imply cascade delete.
- Default to `Restrict`/`NoAction` across aggregate or tenant boundaries.
- Review SQL Server multiple-cascade-path behavior before migration approval.

### Multi-tenancy

- Tenant-owned tables require `OrganizationId` or a provable tenant chain.
- Every frequently queried tenant table needs an index beginning with `OrganizationId`.
- Unique business keys are normally unique per organization, not globally.
- Background jobs must restore and validate tenant context.
- Future row-level security may supplement, not replace, application authorization.

### Index strategy

- Index foreign keys, tenant/filter combinations, normalized lookup fields, statuses, and scheduling timestamps.
- Use composite indexes in actual query order.
- Avoid indexing low-selectivity fields alone.
- Measure query plans before adding speculative indexes.
- Pagination must have a deterministic indexed ordering.

## 8. Security Standards

### Authentication and JWT

- Validate issuer, audience, lifetime, signing key, and clock skew.
- Signing keys must be high-entropy secrets from environment/secret management.
- Access tokens are short-lived.
- Refresh tokens are random, stored hashed, rotated, revocable, and preferably transported in Secure, HttpOnly cookies.
- Never store production secrets in committed configuration.

### Authorization

- Use named policies and roles: SuperAdmin, Owner, Manager, Photographer, Editor, Viewer, and Guest where applicable.
- Validate organization membership and resource ownership inside every tenant-scoped use case.
- Prevent horizontal privilege escalation by loading the resource and verifying its tenant.
- Audit sensitive administrative and marketing actions.

### File uploads

- Validate authenticated tenant, declared use, extension, MIME signature, size, dimensions, and filename.
- Generate server-side storage names; never trust paths from clients.
- Scan untrusted files before publication.
- Store files outside the application executable directory in production.
- Use signed URLs and least-privilege storage credentials.

### Platform protections

- Maintain rate limiting for authentication, public lead capture, uploads, AI jobs, and messaging.
- Apply secure headers: HSTS, CSP, frame restrictions, MIME sniff prevention, referrer policy, and permissions policy.
- CORS uses an explicit allowlist; never combine wildcard origins and credentials.
- Require HTTPS outside local development.

### Sensitive data

- Classify personal, consent, credential, payment, and biometric-adjacent data.
- Encrypt in transit and at rest; use field-level protection where threat modeling requires it.
- Do not put sensitive data in logs, URLs, analytics events, or client persistence.
- Marketing delivery must check current consent and suppression state at send time.
- Record consent version, timestamp, source/IP where lawful, and privacy-policy acceptance.
- Support export, correction, retention, and deletion workflows.

## 9. API Standards

### Response envelope

All JSON endpoints use:

```json
{
  "success": true,
  "message": "Guest created.",
  "data": {}
}
```

Use `ApiResponse<T>.Ok` and `ApiResponse<T>.Fail`; do not invent endpoint-specific envelopes.

### Pagination

Paged responses use `PagedResult<T>` with:

```text
items, page, pageSize, totalCount, totalPages
```

- Page numbers start at 1.
- Default page size is 20.
- Maximum page size is 100 unless a documented export endpoint applies.
- Large/changing feeds should use cursor pagination.

### Filtering, search, and sorting

- Use query parameters and typed request records.
- Search terms are trimmed and bounded.
- Sorting uses an allowlist; never pass raw column names into SQL.
- Date filters use explicit UTC semantics.
- Collection ordering must be deterministic.

### Errors

| Status | Meaning |
|---|---|
| `400` | Malformed or validation failure |
| `401` | Missing/invalid authentication |
| `403` | Authenticated but not permitted |
| `404` | Resource unavailable in authorized scope |
| `409` | Conflict or database business constraint |
| `429` | Rate limit exceeded |
| `500` | Unexpected server failure |

Error bodies use the standard response envelope and a safe trace ID. Validation responses identify fields and messages without internal details.

### Bulk operations

- Set explicit batch limits.
- Validate tenant ownership for every item.
- Return per-item outcomes when partial success is allowed.
- Use transactions only when all-or-nothing behavior is required.
- Move long-running imports/exports to queued jobs.

## 10. UI/UX Design System

### Design philosophy

EventLens AI should feel premium, calm, creative, and operationally clear. Event-day actions prioritize speed and confidence; administration prioritizes scanability and insight. Avoid generic dashboard clutter.

### Core visual tokens

| Token | Suggested value/use |
|---|---|
| Primary | Indigo/violet (`#6C5CE7`) |
| Primary highlight | Soft violet (`#8B7CFF`) |
| Accent | Magenta/pink used sparingly for creative features |
| Success | Emerald/green |
| Warning | Amber |
| Danger | Red |
| Light surface | White and cool neutral |
| Dark surface | Deep navy/violet (`#15152C`) |
| Text | Near-black in light mode; near-white in dark mode |

Existing product tokens and contrast-tested variants take precedence over hard-coded values.

### Typography and spacing

- Use the established sans-serif product font stack.
- Maintain a clear type hierarchy with restrained weight changes.
- Base spacing uses a 4px grid; common steps are 4, 8, 12, 16, 20, 24, 32, and 48px.
- Cards generally use 16–24px radius and consistent interior spacing.
- Dense data views may reduce spacing but must retain touch and accessibility requirements.

### Icons and motion

- Use one consistent icon family or the established shared icon components.
- Icons require accessible labels when meaning is not accompanied by text.
- Motion communicates state, countdown, capture, progress, or hierarchy.
- Typical transitions are 150–250ms; avoid ornamental blocking animation.
- Respect `prefers-reduced-motion`.

### Dark mode and responsive behavior

- All new shared UI must support light and dark themes.
- Test contrast in both themes.
- Mobile-first layouts progressively enhance for tablet and desktop.
- Booth/kiosk screens prioritize large controls, minimal navigation, and recovery from interruption.

## 11. AI Architecture Standards

### Provider abstraction

- Features depend on `IAIProvider`, `IAIProviderResolver`, and selection abstractions.
- Provider names, endpoints, credentials, timeouts, and supported job types are configuration.
- Do not branch feature code on provider SDK types.
- Adding a provider should require an adapter and registration, not changes throughout the product.

### Queue processing

- AI work is asynchronous and durable.
- API endpoints validate, authorize, persist a job, enqueue it, and return `202 Accepted`.
- Hosted workers process jobs with bounded concurrency, cancellation, timeout, and retry policies.
- Production queues must support restart recovery and dead-letter handling.

### Job lifecycle

```text
Queued → Processing → Completed
                   ↘ Failed → Retried/Dead-lettered
```

- Persist provider, job type, tenant/resource IDs, timestamps, attempts, safe error summaries, and output references.
- State transitions must be idempotent and auditable.
- Never bill or consume quotas twice during a retry.

### Prompt storage and safety

- Store versioned prompt definitions outside controllers.
- Record prompt-definition version and non-sensitive parameters with the job.
- Do not store secrets or unnecessary personal data in prompts.
- Validate generated outputs before publication.
- Provider moderation and platform safety controls remain enabled unless a reviewed requirement says otherwise.

### Future providers

Support is expected for multiple hosted and self-managed providers. Capability discovery and routing must be based on supported job types, tenant entitlement, cost, latency, region, and availability—not hard-coded UI assumptions.

## 12. Performance Standards

### Frontend

- Lazy-load route-level editors, AI tools, reports, and other heavy features.
- Split provider/editor code by feature.
- Avoid unnecessary React renders with focused selectors and stable props.
- Use OffscreenCanvas and Web Workers for heavy browser image operations when available.
- Release media streams, object URLs, timers, and large buffers promptly.

### Images and storage

- Produce appropriate thumbnails and responsive variants.
- Preserve originals separately from derived assets.
- Use efficient formats where compatible and avoid repeated re-encoding.
- Deliver public assets through CDN/object storage with cache headers and signed access where required.

### Caching

- React Query owns client request caching.
- Cache stable server data with explicit tenant-aware keys and invalidation.
- Never cache authorization decisions or sensitive tenant results across users.

### Database

- Project only required columns.
- Use `AsNoTracking` for reads.
- Paginate collections.
- Batch writes carefully and avoid row-by-row network calls.
- Profile slow queries and examine SQL Server execution plans.
- Set command timeouts intentionally for reporting jobs rather than globally increasing them.

## 13. Testing Standards

| Test level | Purpose |
|---|---|
| Unit | Domain invariants, services, validators, mappings, reducers/stores, pure image/layout logic |
| Integration | EF mappings, repositories, authentication, authorization, middleware, API contracts, provider adapters |
| End-to-end | Registration/login, organization/event setup, booth capture, processing, gallery access, CRM consent/check-in |

### Rules

- Every bug fix should include a regression test where practical.
- New domain behavior and security boundaries require tests.
- Integration tests use an isolated SQL Server database compatible with production behavior.
- External providers are replaced by contract-faithful fakes at test boundaries only.
- Tests must be deterministic and independent of execution order.
- Do not use arbitrary sleeps; wait on observable state.

### Naming

C#:

```text
MethodName_StateUnderTest_ExpectedBehavior
```

TypeScript:

```text
describe("feature", () => {
  it("does expected behavior when condition", ...)
})
```

## 14. Git Workflow

### Branches

| Type | Format |
|---|---|
| Feature | `feature/<ticket>-short-description` |
| Fix | `fix/<ticket>-short-description` |
| Maintenance | `chore/<ticket>-short-description` |
| Release | `release/<version>` |
| Hotfix | `hotfix/<ticket>-short-description` |

Use lowercase kebab-case. Keep branches focused and short-lived.

### Commits

Use Conventional Commits:

```text
feat(crm): add consent-aware guest check-in
fix(photos): prevent SQL Server cascade path
docs(architecture): define AI job lifecycle
test(auth): cover refresh-token rotation
```

- Commit intentional, reviewable units.
- Do not commit secrets, generated build output, local databases, logs, or editor state.
- Migration and model changes belong in the same logical commit.

### Pull requests

PRs must include:

- Problem and solution summary
- Scope and explicit non-goals
- Screenshots/video for UI changes
- API and migration notes
- Security/privacy impact
- Test evidence
- Deployment/configuration changes
- Rollback considerations

At least one reviewer should verify tenant isolation, authorization, migrations, and external contracts when relevant. CI must pass before merge.

## 15. Future Modules and Roadmap

| Module | Current direction | Roadmap |
|---|---|---|
| Booth Engine | Browser camera, sessions, countdown, burst capture, strip generation | Offline persistence, kiosk hardening, DSLR adapter, print station |
| Photo Processing | Canvas/ImageSharp processing and exports | Worker pipelines, color management, advanced batch processing |
| Template Editor | Template configuration and editor foundations | Drag/drop layers, safe zones, print presets, marketplace |
| AI Studio | Provider-neutral queued jobs | More providers, cost routing, moderation, model/version controls |
| Gallery | Event gallery and QR foundations | Live sync, favorites, moderation, signed downloads, sharing analytics |
| CRM | Guest, consent, forms, attendance, segmentation foundations | Campaign delivery, automation execution, loyalty, referrals, retention tooling |
| Analytics | Module-level counters and dashboard foundations | Unified event funnel, cohorts, attribution, scheduled BI reports |
| Billing | Not yet implemented | Plans, entitlements, metering, invoices, taxes, dunning |
| White-label | Not yet implemented | Custom domains, themes, email identity, tenant apps |
| Mobile Apps | Not yet implemented | Operator app, guest companion, offline synchronization |
| Admin Portal | Not yet implemented | Tenant support, moderation, provider health, feature flags, audit search |

### Roadmap rules

- Roadmap presence does not authorize implementation.
- Build modules behind clear service boundaries and entitlement checks.
- Billing and white-label work must not be mixed into unrelated feature changes.
- Each module requires security, privacy, observability, testing, migration, and operational readiness before production release.

## Definition of Done

A feature is complete only when:

- Architecture and tenant boundaries are respected.
- Authorization and validation are implemented server-side.
- API/UI loading, error, empty, and success states are handled.
- Logging is useful and free of secrets/sensitive personal data.
- Relevant unit/integration/E2E tests pass.
- Database migrations are reviewed and reversible where practical.
- Swagger and operational documentation are updated.
- Accessibility and responsive behavior are verified.
- Backend and frontend production builds pass.
- No fake provider or business behavior is presented as production functionality.

