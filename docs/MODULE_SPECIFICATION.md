# EventLens AI Module Specification

> **Document type:** Functional product specification  
> **Audience:** Product, design, architecture, engineering, QA, operations, and AI coding assistants  
> **Engineering authority:** [`PROJECT_RULES.md`](../PROJECT_RULES.md)  
> **Scope:** Desired module behavior and boundaries. This document does not claim every item is implemented.

## Status and Contract Conventions

The repository contains foundations for several modules, but production completeness is determined only by the definition of done in `PROJECT_RULES.md`.

| Term | Meaning |
|---|---|
| Current foundation | Meaningful backend or frontend code exists today |
| Partial | Some workflows exist but the complete specification is not delivered |
| Planned | Future requirement; no implementation claim |
| Tenant-scoped | Access derives from authorized organization membership, directly or through an owned aggregate |
| Public | No login required, but scope, rate, privacy, and capability checks still apply |

Unless a module explicitly requires another pattern:

- JSON APIs return `ApiResponse<T>` and paged collections return `PagedResult<T>`.
- API routes remain unversioned internally until the external contract is published.
- Organization and resource IDs supplied by a client are untrusted.
- Long-running provider, media, report, print, messaging, and AI operations use durable jobs.
- Soft deletion, UTC timestamps, audit fields, SQL Server-compatible delete behavior, and tenant-first indexes apply.

## Module Map

| # | Module | Primary aggregate/boundary | Current direction |
|---:|---|---|---|
| 1 | User & Authentication | User/session/credential | Current foundation |
| 2 | Organization Management | Organization/membership | Current foundation |
| 3 | Event Management | Event | Current foundation |
| 4 | Booth Engine | BoothSession | Partial |
| 5 | Photo Processing | Photo/derived asset/recipe | Partial |
| 6 | Template Studio | Template/version | Partial |
| 7 | AI Studio | AIJob | Partial; provider-dependent |
| 8 | Gallery System | Gallery/album | Partial |
| 9 | Printing System | PrintJob/print station | Planned |
| 10 | CRM & Marketing | Guest/campaign/workflow | Partial |
| 11 | Analytics & Reporting | Metric event/aggregate/report | Partial |
| 12 | Billing & SaaS | Subscription/entitlement/invoice | Partial; provider-dependent |
| 13 | White Label Platform | Domain/theme/reseller | Planned |
| 14 | Enterprise Features | Identity connection/policy | Planned |
| 15 | Integrations & Developer Platform | Connection/API client/webhook | Selected abstractions/planned |

---

## 1. User & Authentication

### Purpose

Identify users securely, maintain revocable sessions, and establish the claims from which server-side authorization begins. Authentication proves identity; organization membership and permissions determine access.

### Target users

All authenticated personas, platform operations staff, organization owners, invited members, and developers using future API credentials.

### Features

| Capability | Specification | Status |
|---|---|---|
| Registration | First/last name, normalized email, strong password, optional phone, duplicate prevention | Foundation present |
| Login/logout | Credential validation, active-user check, auth event logging, session revocation | Foundation present |
| Sessions | Short access token, random hashed rotating refresh token, family replay protection | Foundation present |
| Verification | Time-limited one-use email verification and resend controls | Planned |
| Password reset | Non-enumerating request, one-use token, revoke existing sessions on completion | Planned |
| Social login | OIDC/OAuth providers linked to an existing or new user under safe collision rules | Planned |
| MFA | Authenticator/passkey and recovery-code strategy; policy enforcement | Planned |
| Profiles | Name, phone, image, locale, time zone, preferences, security summary | Partial |
| API keys | Scoped, expiring, hashed credentials with last-used metadata and revocation | Planned |
| Activity logs | User-visible sign-in/session/security activity; platform audit remains separate | Planned |
| Roles | Platform and tenant role claims; effective permissions calculated server-side | Foundation present |

### User workflows

1. **Register:** user submits valid details → account is created → verification is sent when enabled → a session is issued according to policy → user creates or joins an organization.
2. **Login:** credentials are validated without account enumeration → access and refresh tokens are issued → available organizations load → the user selects a tenant context.
3. **Refresh:** client presents the refresh cookie/token → server validates active hash and token family → old token is revoked → a replacement and short access token are issued.
4. **Logout:** current refresh token is revoked and client state is cleared. “Logout all devices” revokes every active family.
5. **Reset password:** request always returns a neutral response → one-use token opens reset screen → strong password is stored through the password service → sessions are revoked → security event is recorded.
6. **Accept invitation:** authenticated email must match the invitation; membership is created idempotently.

### Backend requirements

- Identity services remain behind `IAuthService`, password, token, current-user, and repository abstractions.
- Normalize email consistently and enforce its active uniqueness.
- Password policies are validated at the request boundary and hashed with an adaptive approved algorithm.
- Token issuance, rotation, replay detection, revocation, and expiry are transactional.
- Authentication logging must omit credentials and tokens.
- Future social providers map through provider-neutral external-login records.
- Account lockout/rate limiting should combine IP and account-safe controls.

### Frontend requirements

- Registration, login, verification, reset, invitation, MFA challenge, session management, and profile screens.
- Inline password requirements must exactly match backend validation.
- Session bootstrap loads user and tenant context; one refresh attempt may recover an expired access token.
- Session expiry returns the user to login without exposing protected stale data.
- Errors must distinguish actionable validation from safe generic authentication failure.

### Database entities

`User`, `Role`, future `Permission`, `RolePermission`, `RefreshToken`, `EmailVerificationToken`, `PasswordResetToken`, `ExternalLogin`, `MfaCredential`, `RecoveryCode`, `ApiKey`, `UserActivity`, and `AuditLog`.

Indexes include normalized email, active refresh-token hash, user/token-family status, external provider subject, API-key hash, and security activity timestamps.

### API requirements

- `POST /api/auth/register`, `/login`, `/refresh`, `/logout`.
- Planned: `/verify-email`, `/resend-verification`, `/forgot-password`, `/reset-password`, `/mfa/challenge`, `/mfa/recovery`.
- `GET/PUT /api/users/me`; session list/revoke endpoints.
- API-key endpoints are organization-aware and require elevated permission.
- Auth endpoints have stricter rate limits and safe, documented error envelopes.

### Security considerations

- High-entropy signing keys and credentials come from secret management.
- Refresh cookies are Secure and HttpOnly with an appropriate SameSite policy.
- Prevent email enumeration, credential stuffing, token replay, session fixation, and account-link takeover.
- MFA secrets and recovery material are encrypted or irreversibly protected.
- SuperAdmin access requires stronger controls and produces immutable audit events.

### Future enhancements

Passkeys/WebAuthn, risk-based authentication, device trust, delegated identity, regional sessions, SCIM, just-in-time provisioning, and customer-controlled authentication policies.

---

## 2. Organization Management

### Purpose

Define the SaaS tenant boundary and provide controlled collaboration, branding, storage, subscription context, and organizational structure.

### Target users

Organization Owners, Managers, platform administrators, invited staff, agencies, and future enterprise administrators.

### Features

- Organization profile, unique slug, contacts, country, address, locale/time zone, status, and soft deletion.
- Team membership, invitations, role changes, removal, and ownership transfer.
- Departments and branches with delegated scope (planned).
- Storage consumption/limit and subscription/entitlement summary.
- Brand Kit containing logos, colors, typography, approved assets, and defaults.
- Organization security and audit views.
- Multi-organization switching without sharing cached data across tenants.

### User workflows

1. A registered user creates an organization and becomes Owner.
2. The Owner configures brand and defaults, then invites staff with the minimum role.
3. An invited user signs in with the invited email and accepts once.
4. An Owner changes a member’s role or removes access; active authorization reflects the change promptly.
5. A user belonging to multiple organizations switches tenant context; queries and mutations reload under the selected tenant.
6. An authorized owner reviews storage and plan usage before creating resource-heavy work.

### Backend requirements

- Every use case verifies membership or explicit SuperAdmin elevation.
- Membership and invitation changes are audited.
- Owner removal/transfer rules prevent orphaned organizations.
- Organization deletion is recoverable initially and must account for retention, billing, public URLs, and purge workflows.
- Brand assets use validated storage uploads; configuration is versionable.
- Feature availability calls the centralized entitlement service.

### Frontend requirements

- Organization picker, create/edit settings, member list, invitation and role dialogs.
- Brand-kit editor with accessible previews and file validation feedback.
- Storage/plan summary and clear upgrade or remediation guidance.
- Empty state for users with no organizations and safe cache invalidation on switch.

### Database entities

`Organization`, `OrganizationMember`, `OrganizationInvitation`, `Role`, future `Department`, `Branch`, `BrandKit`, `BrandAsset`, `OwnershipTransfer`, and organization-linked `AuditLog`.

Tenant indexes cover organization/status, organization/member user, active invitation email, branch/department hierarchy, and slug uniqueness.

### API requirements

- `GET/POST /api/organizations`; `GET/PUT/DELETE /api/organizations/{id}`.
- Member list, invite, accept, remove, and role-change endpoints.
- Planned department, branch, ownership-transfer, brand-kit, storage, security, and audit endpoints.
- Responses expose plan/storage summaries without exposing provider or payment secrets.

### Security considerations

- Never trust a selected organization stored in the browser.
- Prevent horizontal access via member, event, asset, invitation, and billing IDs.
- Sensitive owner, role, domain, billing, and security changes may require re-authentication.
- Invitation tokens are random, hashed, expiring, one-use, and bound to email.

### Future enhancements

Organization groups, franchises, reseller/customer hierarchy, approval workflows, custom roles, bulk member provisioning, enterprise organizational units, and data residency.

---

## 3. Event Management

### Purpose

Represent the complete operational context for a real event and connect schedules, branding, booths, guests, assets, galleries, analytics, and limits.

### Target users

Owners, Event Managers, Photographers, Booth Operators, Designers, Marketing Managers, and read-only Viewers.

### Features

- Event create/read/update/delete, search, filtering, sorting, pagination, duplication, publish, run, complete, and archive.
- Event type, venue, address, coordinates, time zone, start/end dates, description, and capacity.
- Cover, logo, colors, brand inheritance, registration/download/sharing/AI/QR settings.
- Event templates/presets and reusable checklists.
- Public event page and QR code.
- Multiple booth configurations and operator assignments.
- Event limits for guests, photos, storage, and plan-governed capabilities.

### User workflows

1. Manager creates an event manually or from an approved preset.
2. Branding and policy inherit from the organization and may be overridden where permitted.
3. Manager assigns venue, schedule, staff, booth configurations, template, gallery, and guest settings.
4. Validation highlights unresolved launch requirements.
5. Publishing creates a controlled public URL and QR code.
6. During the event, authorized users monitor booths and activity.
7. After completion, media, gallery, reporting, retention, and archive workflows continue.

### Backend requirements

- `IEventService` enforces tenant membership, role, lifecycle transitions, date validity, slug uniqueness, limits, and entitlement.
- Public bootstrap returns only the data needed by a booth/public page.
- QR content uses canonical controlled URLs.
- Event duplication copies approved configuration but not sessions, guests, analytics, or secret links.
- Event-owned repositories use tenant/event indexes and bounded queries.

### Frontend requirements

- Event list with filters and empty/error/loading states.
- Multi-step event configuration or focused detail tabs.
- Branding/public-page preview, QR download, booth assignment, validation checklist, and lifecycle actions.
- Date/time controls make the event time zone explicit.

### Database entities

`Event`, `EventSettings`, future `Venue`, `EventTemplate`, `EventBooth`, `EventStaffAssignment`, `EventChecklist`, and public-link metadata. Existing sessions, photos, guests, galleries, metrics, and subscriptions reference or inherit the event’s organization.

### API requirements

- Event search and detail; organization-scoped create; update/delete.
- Publish, archive, duplicate, settings, QR generation, and QR image.
- Planned venue, staff, booth-definition, checklist, public page, and validation endpoints.
- Public endpoints are rate-limited and return no private tenant details.

### Security considerations

- Event ID authorization derives from the owning organization.
- Public visibility, guest registration, download, sharing, and AI flags are enforced server-side.
- Published URLs must not make private events enumerable.
- Lifecycle actions are role-gated and audited.

### Future enhancements

Calendar views, recurring event series, venue library, event runbooks, staffing schedules, approvals, ticketing links, multi-day programs, localization, and capacity forecasting.

---

## 4. Booth Engine

### Purpose

Operate a professional, accessible, browser-first capture flow that tolerates device and network problems and can later integrate DSLR hardware.

### Target users

Guests, Booth Operators, Photographers, Event Managers, and remote operations staff.

### Features

- Webcam/mobile camera permission, enumeration, HD constraints, front/back switching, mirror mode, preview, reconnect, and cleanup.
- Booth sessions with event, optional guest, status, start/complete time, capture mode, photo count, countdown, and template.
- Single photo, 2/3/4 strips, GIF, boomerang, short video, burst; planned slow motion, time lapse, and live photo.
- Animated/audio countdown, flash, inter-shot pause, progress, retake, accept, download, and next-step routing.
- Canvas/OffscreenCanvas capture, preview, thumbnail, high-resolution output, and local memory/offline state.
- Multi-booth configuration and future DSLR adapter.

### User workflows

1. Guest opens `/booth/{eventSlug}` or operator launches an assigned booth.
2. Public bootstrap validates event availability and returns permitted modes/branding.
3. Camera permission is requested in context; denial receives actionable recovery instructions.
4. Guest selects an allowed mode/countdown and starts a session.
5. The state machine counts down, captures each shot, announces progress, and generates the result.
6. Guest retakes or accepts. Upload is an explicit action; network absence keeps an identifiable pending session.
7. Completion synchronizes idempotently and clears media resources.

### Backend requirements

- Booth bootstrap, create/get/update/complete session, and capture-registration services.
- Idempotency keys/local session IDs prevent duplicate synchronization.
- Capture-mode strategies avoid switch statements scattered across modules.
- Real-time booth health/activity events are authorized by event/organization.
- A future `ICameraAdapter` bridge exposes capabilities and signed device communication without placing vendor code in Application.

### Frontend requirements

- Feature-scoped Zustand state machine for camera, countdown, photos, session, and template selection.
- Camera adapters, capture strategies, strip generator, workers, keyboard shortcuts, touch targets, and screen-reader announcements.
- Cleanup tracks, timers, object URLs, workers, and large buffers.
- Offline status and retry are visible; unsynced captures are not represented as uploaded.

### Database entities

`BoothSession`, future `BoothDefinition`, `BoothDevice`, `CaptureRecord`, `SessionSyncRecord`, and operational health events. Large media stays in storage, not SQL Server.

### API requirements

- Public event booth bootstrap.
- Authorized or securely public session creation according to event policy.
- Session retrieval, status transitions, capture registration, completion, and offline sync.
- SignalR hub for authorized operational updates.

### Security considerations

- Camera access requires browser permission and secure context.
- Public session creation is rate-limited, event-scoped, bounded, and abuse-monitored.
- Media metadata and guest association are private by default.
- Device/DSLR bridges use signed, short-lived credentials and explicit assignment.

### Future enhancements

Kiosk lockdown, local durable media via IndexedDB, device fleet health, remote configuration, DSLR/tether bridge, green screen, group capture, depth input, attendant mode, and printer handoff.

---

## 5. Photo Processing

### Purpose

Produce consistent, high-quality, branded derivatives while preserving immutable originals and an auditable recipe.

### Target users

Guests, Photographers, Designers, Booth Operators, and automated workflows.

### Features

Crop, rotate, resize, filters, exposure/color adjustments, beauty effects, blur, frames, borders, watermarks, logos, dates, stickers, text, photo strips, thumbnails, preview, undo/redo, compare, export, and server/batch processing.

### User workflows

1. User selects a capture or gallery photo.
2. Editor loads a working preview while preserving the original.
3. Operations update a non-destructive recipe with undo/redo.
4. Preview renders interactively in the browser.
5. Export validates dimensions, entitlement, and storage, then renders a durable derived asset.
6. The result may proceed to gallery, AI, template, print, or download.

### Backend requirements

- Storage abstraction, photo service, processing recipe validator, ImageSharp-based server renderer, and queued processing for heavy work.
- Hash/source/version information supports idempotency and provenance.
- EXIF orientation and sensitive metadata are handled intentionally.
- Quota reservation occurs before output commit and is reconciled on failure.

### Frontend requirements

- Canvas renderer separated from React components.
- OffscreenCanvas/Web Worker where supported.
- Layer/operation controls with keyboard access, responsive inspector, accurate zoom, and memory cleanup.
- Preview quality may differ for speed but export dimensions/quality are explicit.

### Database entities

`Photo`, `PhotoAsset`, `PhotoVersion`, `ProcessingRecipe`, `ProcessingJob`, `ExportRecord`, and storage usage records. Recipes may use validated versioned JSON plus queryable metadata.

### API requirements

Photo metadata/detail, signed upload/download, recipe validate/save, preview where server-required, queue render, job status, export, and derived-version management.

### Security considerations

Validate every upload’s tenant, content signature, size, dimensions, filename, and purpose. Use generated storage names, malware scanning, signed URLs, output moderation where needed, and strict event/gallery authorization.

### Future enhancements

RAW workflows, advanced beauty/color tools, LUTs, color profiles, masking, collaborative review, batch recipes, smart crops, restoration, and professional lab export.

---

## 6. Template Studio

### Purpose

Create reusable, versioned layouts and experiences that produce consistent booth, gallery, social, animation, and print output.

### Target users

Designers, Organization Owners, Event Managers, Photographers, marketplace creators, and platform curators.

### Features

Template library, categories, access level, previews, drag/drop, layers, order/lock/group, alignment, guides, snap, fonts, text, photo slots, shapes, stickers, masks, animation, brand kit, safe zones, dimensions, variants, versioning, draft/review/publish, and duplication.

### User workflows

1. Designer starts from blank, library, or organization template.
2. Canvas is configured for output target and dimensions.
3. Designer adds layers/assets and binds dynamic fields or photo slots.
4. Validator checks missing assets, overflow, safe zones, fonts, and output compatibility.
5. Preview renders representative data.
6. A version is published and assigned to events; later edits create a new version.

### Backend requirements

- Template/version aggregate, schema validation, asset ownership, preview renderer, publication workflow, entitlement checks, and immutable published versions.
- Configuration format is explicit and migratable; clients declare supported schema versions.
- Premium/marketplace licensing is enforced server-side.

### Frontend requirements

- High-performance canvas, layers panel, property inspector, rulers/guides, asset/font browser, zoom/pan, history, autosave, conflict detection, and responsive fallback.
- Keyboard shortcuts and screen-reader-accessible layer operations.

### Database entities

`Template`, `TemplateVersion`, `TemplateAsset`, `TemplateAssignment`, `BrandKit`, `FontAsset`, `TemplatePublication`, and future marketplace licensing records.

### API requirements

Search/list/detail/create/duplicate, draft save, version list, validate, preview, publish/archive, asset/font operations, and event assignment.

### Security considerations

Validate configuration JSON, external URLs, fonts, SVG/script content, asset ownership, premium access, and publication permission. Prevent untrusted active content in rendered output.

### Future enhancements

Real-time collaboration, comments/approval, responsive templates, data-bound designs, AI generation, marketplace publishing, localization, and reusable components.

---

## 7. AI Studio

### Purpose

Provide creative and analytical AI through a safe, provider-neutral, queued capability layer.

### Target users

Guests where enabled, Photographers, Designers, Event Managers, Marketing Managers, and organization administrators.

### Features

Background removal/replacement, enhancement, upscale, style transfer, cartoon/anime/sketch/vintage looks, props, lighting, color correction, object removal, generative fill, collage, smart selection, memory books, highlight generation, templates, editable prompts, history, retries, live progress, moderation, and credit usage.

### User workflows

1. User selects an authorized input and capability.
2. UI loads provider-independent parameters and shows credit/cost implications.
3. Backend verifies tenant, event, entitlement, consent/policy, input, and provider capability.
4. A durable job and credit reservation are created; API returns `202`.
5. Worker resolves a provider and reports progress through SignalR.
6. Output is validated/moderated, stored, and presented for review.
7. Accepting output creates a photo version; failure releases/reconciles reserved usage.

### Backend requirements

- `IAIProvider`, resolver, routing, prompt definitions, queue, hosted/independent worker, retry/dead-letter, cancellation, timeouts, idempotency, moderation, and metering.
- Provider errors are translated to safe status; raw provider data and secrets never leave Infrastructure.
- Job lifecycle and attempts survive restart.

### Frontend requirements

- Capability-aware studio, presets, parameters, before/after, job queue/history, live progress, cancellation/retry, provider-unavailable and insufficient-credit states.
- Never imply completion until a persisted output exists.

### Database entities

`AIJob`, `AIJobAttempt`, `AIOutput`, `PromptDefinition`, `PromptVersion`, `ProviderUsage`, `CreditReservation`, and moderation records.

### API requirements

Capability discovery, queue job by capability, job list/detail/cancel/retry, output accept/reject, prompt/admin configuration, and authorized SignalR events.

### Security considerations

Minimize personal data sent to providers, enforce tenant/provider region policy, moderate inputs/outputs, prevent prompt injection into privileged actions, use signed asset access, redact logs, and audit costs and publication.

### Future enhancements

AI Event Assistant, Memory Book, Highlight Reel, Template Generator, cost/latency routing, self-hosted providers, model governance, batch workflows, and explainable business recommendations.

---

## 8. Gallery System

### Purpose

Publish and deliver event media through fast, branded, privacy-controlled, measurable experiences.

### Target users

Guests, Viewers, Event Managers, Photographers, Marketing Managers, and clients.

### Features

Public/private galleries, QR access, password or signed links, albums, collections, live updates, photo detail, favorites, likes/comments where enabled, moderation, search/filter, downloads, sharing, branding, expiration, guest-specific access, and analytics.

### User workflows

1. Manager configures visibility, delivery, interaction, moderation, and expiration.
2. Publishing creates canonical gallery/QR access.
3. Guest opens link, completes required identity/consent step, and browses optimized assets.
4. Views, favorites, shares, and downloads are recorded under privacy-safe identity rules.
5. New approved photos appear live.
6. Manager moderates, reorganizes, exports, or closes the gallery.

### Backend requirements

- Gallery/album services, privacy policy enforcement, signed URLs, thumbnail/CDN strategy, pagination, moderation, interaction ingestion, and SignalR publication.
- Unique/repeat metrics use documented identifiers and retention.
- Private gallery responses avoid existence disclosure.

### Frontend requirements

- Responsive public shell, masonry/grid/list options, lightbox, keyboard/touch navigation, progressive images, download/share states, password/access flow, favorites, and accessible empty/error views.

### Database entities

`Gallery`, `Album`, `Collection`, `GalleryPhoto`, `GalleryAccessGrant`, `Favorite`, `Reaction`, `Comment`, `GalleryView`, `Download`, `Share`, and moderation records.

### API requirements

Authorized gallery configuration and organization; public bootstrap/list/detail; access-token/password exchange; interaction and download endpoints; QR redirect/image; live update hub.

### Security considerations

Rate-limit public access, use opaque/signed identifiers, authorize original downloads separately, sanitize comments, moderate content, prevent scraping where practical, honor deletion/retention, and avoid sensitive analytics fingerprinting.

### Future enhancements

Face-assisted opt-in retrieval, proofing/selection, client approval, commerce, live slideshow, guest uploads, collaborative albums, watermark-on-demand, and regional CDN controls.

---

## 9. Printing System

### Purpose

Reliably route rendered assets to event printers through a recoverable, hardware-neutral queue.

### Target users

Booth Operators, Photographers, Event Managers, technical staff, and guests where self-service printing is enabled.

### Features

Instant print, printer/station registration, discovery, capabilities, media status, print templates, copy/size/crop/color settings, queue, priority, retry, cancel, history, reprint, quotas, and DNP/Canon/Mitsubishi adapter support.

### User workflows

1. Operator installs/authorizes a local print station and assigns printers to an event/booth.
2. System validates media/profile and prints a test page.
3. Accepted booth output creates an idempotent print job.
4. Station claims, prints, and acknowledges the job.
5. Failures expose actionable operator recovery and bounded retry.
6. Reprints require permission and are recorded against limits.

### Backend requirements

- `IPrintProvider`/station abstraction, signed station authentication, durable queue, leases, heartbeats, retries, duplicate prevention, status events, and print metering.
- Vendor code is isolated in bridge/adapters.

### Frontend requirements

- Printer setup wizard, diagnostics, test print, queue, media warnings, preview, cancel/retry/reprint, and kiosk-friendly status.
- Browser-only clients must clearly explain when a local bridge is required.

### Database entities

`PrintStation`, `Printer`, `PrinterCapability`, `PrintProfile`, `PrintJob`, `PrintAttempt`, `PrintTemplateAssignment`, and `PrintUsage`.

### API requirements

Station register/heartbeat/capabilities, printer assignment, queue/claim/acknowledge/fail, operator list/action, test print, and SignalR status.

### Security considerations

Stations receive scoped, revocable credentials. Jobs use signed short-lived asset references. Prevent unauthorized remote printing, command injection, path access, repeated jobs, and tenant-crossed station assignment.

### Future enhancements

Print-lab fulfillment, paid print orders, color calibration, fleet monitoring, consumable prediction, routing/failover, and certified adapter marketplace.

---

## 10. CRM & Marketing

### Purpose

Create consent-aware guest relationships and automate relevant event communications while preserving privacy and delivery accountability.

### Target users

Marketing Managers, Event Managers, Organization Owners, campaign reviewers, and guests managing their preferences.

### Features

Guest database, identity/contact points, lead forms, surveys, attendance/check-in, consent, tags, segments, favorite templates, engagement timeline, campaigns, email/SMS/WhatsApp, coupons, referrals, automation, suppression, preferences, and reporting.

### User workflows

1. Manager builds an event form with required purpose and optional channel consent.
2. Guest submits; server validates schema, consent version, event, deduplication rules, and rate limits.
3. Guest profile/timeline receives check-in, booth, gallery, download, and campaign activity.
4. Marketer defines a segment and reviews estimated audience under current consent.
5. Campaign is drafted, tested, approved, scheduled, and queued.
6. Delivery webhooks update status; unsubscribe/suppression applies immediately.
7. Automation reacts to safe events idempotently and exposes history.

### Backend requirements

- Guest/contact identity model, form schemas, consent service, segmentation engine, campaign/automation state machines, provider-neutral messaging, outbox, retry/dead-letter, webhook processing, preference and deletion workflows.
- Send-time consent and suppression checks are mandatory.

### Frontend requirements

- Guest list/profile/timeline, form/survey builder, tag/segment builder, campaign composer/preview/test, automation canvas, coupon/referral management, and consent audit display.

### Database entities

`Guest`, `GuestContact`, `ConsentRecord`, `Form`, `FormField`, `FormSubmission`, `Survey`, `Tag`, `GuestTag`, `Segment`, `Campaign`, `CampaignMessage`, `Delivery`, `Suppression`, `Automation`, `AutomationRun`, `Coupon`, and `Referral`.

### API requirements

Guest search/detail/import/export, public form schema/submit/check-in, consent/preferences, tag/segment preview, campaign CRUD/test/schedule/cancel, automation CRUD/activate/history, and provider webhooks.

### Security considerations

Treat contact and consent data as sensitive. Enforce marketing purpose, minimize data, encrypt where required, restrict exports, rate-limit public forms, sanitize templates, verify delivery webhooks, support subject requests, and never infer consent.

### Future enhancements

Customer journeys, loyalty, lead scoring, omnichannel frequency caps, attribution, CRM synchronization, multilingual content, send-time optimization, and AI-assisted draft/segment suggestions with human approval.

---

## 11. Analytics & Reporting

### Purpose

Provide reproducible, tenant-safe operational and business intelligence without expensive dashboard-time scans.

### Target users

Owners, Managers, Photographers, Marketing Managers, enterprise analysts, clients, and authorized platform operators.

### Features

Organization KPIs; event timeline/hourly activity; booth usage; session duration; photos per guest; template and AI usage; gallery views/visitors; downloads/shares; photo popularity; likes/comments/prints/exports; guest/return/device/browser/OS/location; QR scans; leads; storage; future revenue; real-time feed; filters; pagination; daily/weekly/monthly/custom reports; PDF/Excel/CSV; line/bar/pie/area/heatmap/timeline charts.

### User workflows

1. User opens analytics under an active organization.
2. Default date range and authorized event scope load cached aggregates.
3. Filters update query keys and charts consistently.
4. Live SignalR events invalidate the relevant summaries.
5. User drills from aggregate to a bounded activity or entity list.
6. Export request creates or streams a tenant-safe report; large exports run as jobs.
7. Scheduled reports deliver through configured channels.

### Backend requirements

- Canonical metric taxonomy, idempotent ingestion, UTC occurrence time plus relevant time-zone interpretation, hourly/daily aggregation jobs, tenant-aware cache, indexed read models, report rendering, and activity pagination.
- Analytics authorization validates the organization and any event/template/photographer filter.
- Corrections/backfills are versioned and observable.

### Frontend requirements

- Accessible KPI cards, reusable charts with tabular alternatives, filter bar, event selector, loading/empty/error/stale states, live indicator, drill-down, activity feed, and export/report center.

### Database entities

`AnalyticsEvent`, `HourlyMetric`, `DailyMetric`, `PhotoStatistic`, `GuestStatistic`, `QrScan`, `ReportDefinition`, `ReportRun`, and scheduled-report delivery records. Dimensions are bounded and indexed; sensitive raw identifiers are minimized.

### API requirements

Dashboard, statistics, charts, activity feed, entity ranking, report create/list/status/download, and SignalR subscription endpoints. Filters use typed parameters and deterministic pagination.

### Security considerations

Tenant scope applies to cache keys, jobs, exports, and hubs. Suppress or aggregate small cohorts where re-identification is a risk. Validate location/device collection and retention. Spreadsheet exports must mitigate formula injection.

### Future enhancements

Cohorts, attribution, anomaly detection, benchmarks, warehouse export, semantic metrics layer, AI Business Insights, forecasting, staffing recommendations, and customer-defined dashboards.

---

## 12. Billing & SaaS

### Purpose

Manage plans, entitlements, usage, subscription lifecycles, invoices, payments, discounts, and licensing without coupling business logic to a payment provider.

### Target users

Organization Owners, finance contacts, Super Admin billing operators, support staff, and future resellers.

### Features

Free/Creator/Business/Enterprise plans; organization/event/member/AI/storage/gallery/template/branding/white-label/API/domain/support limits; centralized feature gating; metering; trials; grace; upgrade/downgrade/cancel/pause/resume/renew; invoices; taxes; discounts; coupons/referrals; payments/refunds; webhooks; credit grants; suspension; and audit.

### User workflows

1. Prospect compares plans and creates a Free organization.
2. Owner opens usage/entitlements and selects an upgrade.
3. Backend creates provider-neutral checkout/subscription intent.
4. Verified webhook is stored idempotently and advances subscription/invoice/payment state.
5. Entitlements update only from trusted state.
6. Downgrade performs impact preview and schedules the change if current usage exceeds limits.
7. Payment failure enters configured retry/grace flow; users receive honest remediation.
8. Authorized Super Admin actions require reason and audit.

### Backend requirements

- Plan catalog, `IFeaturePermissionService`, usage service, subscription state machine, invoice/tax/discount service, `IPaymentProvider`, webhook inbox, reconciliation, dunning jobs, and billing audit.
- Resource creation and provider calls reserve/check usage transactionally.
- No controller/service hard-codes plan names to decide a feature.

### Frontend requirements

- Pricing, current subscription, usage dashboard, upgrade/downgrade preview, checkout handoff, payment methods, billing history, invoices/download, coupon input, grace/payment-failure states, and admin views.
- Unconfigured providers are clearly unavailable.

### Database entities

`PlanDefinition`, `PlanFeature`, `Subscription`, `SubscriptionChange`, `UsageRecord`, `UsageAggregate`, `CreditLedger`, `Invoice`, `InvoiceLine`, `Payment`, `PaymentMethodReference`, `Coupon`, `CouponRedemption`, `Refund`, `WebhookInbox`, and `BillingAudit`.

### API requirements

Plans, organization subscription/actions, usage, invoices, payments, payment methods, coupons/validation, checkout intent, signed provider webhooks, and permission/capability summaries.

### Security considerations

Never accept payment success, amount, plan, or entitlement from the client. Verify webhook signatures and timestamps, store/replay idempotently, minimize PCI scope through tokenized providers, authorize organization ownership, audit mutations, and prevent concurrent quota bypass.

### Future enhancements

Annual/custom contracts, seat-based pricing, overage, prepaid credit packs, regional taxes, purchase orders, credit notes, consolidated billing, reseller billing, revenue recognition support, and multiple currencies.

---

## 13. White Label Platform

### Purpose

Allow entitled agencies and enterprises to present a controlled branded experience across domains, portals, booths, galleries, and communications.

### Target users

Agency Owners, resellers, enterprise brand administrators, client administrators, and platform operations.

### Features

Custom domains, DNS verification, certificates, product naming, logos, icons, themes, email sender/domain, branded authentication, client portal, agency portal, reseller hierarchy, tenant provisioning, and platform-brand removal by entitlement.

### User workflows

1. Owner adds domain; platform provides verification record.
2. Background verification confirms ownership and provisions certificate.
3. Owner configures accessible theme and approved identity assets.
4. Preview shows auth, portal, booth, gallery, and communications.
5. Publishing versions configuration and activates host routing.
6. Agency creates/manages client tenants within delegated limits.

### Backend requirements

- Host-to-tenant resolver, domain verification/certificate jobs, theme/version service, email-domain integration, entitlement, reseller scope, cache invalidation, and fallback behavior.
- Domain and tenant resolution occurs before serving tenant-specific public content.

### Frontend requirements

- Runtime design tokens, branded shells, domain setup/status, theme editor/preview, client portal, and agency/reseller administration.
- Accessibility checks block clearly invalid theme combinations.

### Database entities

`CustomDomain`, `DomainVerification`, `CertificateState`, `WhiteLabelTheme`, `ThemeVersion`, `EmailIdentity`, `ResellerAccount`, `ResellerClient`, and provisioning audit.

### API requirements

Domain add/verify/status/remove, theme draft/preview/publish, email identity, reseller client/provisioning, and public tenant-brand bootstrap.

### Security considerations

Prevent domain takeover, host-header injection, cache poisoning, cross-tenant theme resolution, unsafe custom CSS/scripts, and unauthorized reseller elevation. Domain and sender changes are audited and may require re-authentication.

### Future enhancements

Native branded apps, reseller plan packaging, custom legal content, regional hosting, delegated support, partner billing, and theme marketplace.

---

## 14. Enterprise Features

### Purpose

Provide centralized identity, policy, governance, compliance evidence, and delegated administration for large organizations.

### Target users

Enterprise Owners, IT/identity administrators, security/compliance teams, auditors, department administrators, and support engineers.

### Features

SSO with Azure AD/Entra ID, Google Workspace, Okta, generic OIDC/SAML; enforced SSO; MFA policy; domain discovery; SCIM; group/role mapping; departments/branches; custom roles; approval workflows; retention/legal holds; audit search/export; IP/session policy; regional controls; and security reporting.

### User workflows

1. Enterprise admin configures identity metadata/secret and verifies organization domain.
2. Test mode validates login and mappings before enforcement.
3. Users authenticate at IdP and are provisioned/mapped under explicit rules.
4. SCIM updates or deactivates membership idempotently.
5. Policy engine evaluates role, organizational unit, resource, context, and approvals.
6. Auditor searches and exports authorized audit evidence.
7. Emergency recovery follows controlled, separately audited break-glass procedure.

### Backend requirements

- Provider-neutral enterprise identity contracts, encrypted configuration, metadata/key rotation, assertion validation, policy service, SCIM, organizational units, approval workflow, immutable audit export, retention/hold jobs, and operational alerts.

### Frontend requirements

- Identity connection wizard/test/enforcement, mapping UI, SCIM token lifecycle, custom-role/policy editor, department/branch administration, approvals, security dashboard, and audit explorer.

### Database entities

`IdentityConnection`, `VerifiedDomain`, `ExternalIdentity`, `ProvisioningMapping`, `ScimToken`, `OrganizationUnit`, `CustomRole`, `PermissionGrant`, `Policy`, `ApprovalRequest`, `RetentionPolicy`, `LegalHold`, and audit-export records.

### API requirements

SSO discovery/callback/metadata, connection configuration/test, SCIM resources, mapping, roles/permissions/policies, approvals, retention/holds, audit search/export, and security posture.

### Security considerations

Strictly validate issuer, audience, signature, nonce, state, redirect URI, time, and replay. Encrypt identity secrets, scope SCIM tokens, prevent lockout during SSO enforcement, protect break-glass access, and separate support from customer audit privileges.

### Future enhancements

Device posture, conditional access signals, customer-managed keys, private networking, SIEM export, DLP integrations, regional residency, delegated administration packages, and formal compliance certifications.

---

## 15. Integrations & Developer Platform

### Purpose

Connect EventLens AI to external storage, payments, communications, collaboration, CRM, calendars, automation, and customer software through stable, observable contracts.

### Target users

Organization Owners, technical administrators, developers, agencies, enterprise integration teams, and marketplace partners.

### Features

- Provider adapters for Stripe, PayPal, Paddle, Razorpay, AWS S3, Azure Blob, Cloudinary, Zapier, Make, Slack, Teams, HubSpot, Salesforce, Mailchimp, Twilio, WhatsApp Business, Google Calendar, and Outlook.
- OAuth/credential connection lifecycle, field mapping, sync direction, cursor, retry, health, and disconnect.
- Versioned REST API, scoped API keys/OAuth clients, rate limits, usage reporting, Swagger/developer documentation.
- Signed webhooks with subscriptions, filters, retries, replay, history, and secret rotation.
- TypeScript/.NET SDK direction, sandbox tenants, examples, changelog, and deprecation policy.
- Plugin and template marketplace direction.

### User workflows

1. Admin selects an integration and reviews requested permissions/data.
2. OAuth or secure credential flow completes; backend stores only protected references/secrets.
3. Admin configures scope, event mapping, direction, trigger, and test.
4. Durable jobs or outbox events perform sync.
5. Connection dashboard shows health, last success, failure, retry, and rate-limit state.
6. Developer creates a scoped API key or webhook, copies secret once, tests in sandbox, and monitors usage/delivery.
7. Marketplace customer reviews permissions/license and installs an approved version.

### Backend requirements

- Owning modules depend on narrow interfaces, not SDK types.
- OAuth token vault/rotation, secret management, outbox/inbox, durable sync jobs, retries/dead letters, idempotency, circuit breaking, rate-limit handling, and provider health.
- Public API versioning, scopes, key hashing, quotas, webhook signatures/replay, OpenAPI schema, and compatibility process.
- Plugins require package signing, permission manifest, isolation, review, version pinning, rollback, and kill switch.

### Frontend requirements

- Integration catalog, connection wizard, mapping, test, status/diagnostics, reconnect/disconnect, and permission disclosure.
- Developer portal for keys, scopes, webhooks, delivery logs, replay, usage, docs, SDKs, examples, and sandbox.
- Marketplace browsing, compatibility, license, permission, install/update/rollback, and review states.

### Database entities

`IntegrationConnection`, `EncryptedCredentialReference`, `IntegrationMapping`, `SyncCursor`, `SyncJob`, `SyncAttempt`, `OAuthState`, `ApiClient`, `ApiKey`, `ApiScopeGrant`, `WebhookSubscription`, `WebhookDelivery`, `PluginPackage`, `PluginVersion`, `PluginInstallation`, `MarketplaceListing`, and `License`.

### API requirements

- Integration catalog/connect/callback/configure/test/status/disconnect and provider webhook endpoints.
- External `/api/v1` resources with documented scopes, pagination, idempotency, limits, and errors.
- Webhook subscription, secret rotation, delivery list/detail/replay.
- Marketplace list/detail/install/update/remove APIs under entitlement and permission controls.

### Security considerations

Apply least privilege, encrypt credentials, redact logs, validate OAuth state/PKCE and webhook signatures, prevent SSRF in configurable endpoints, constrain redirects, isolate provider failures, scan/sign packages, limit plugin permissions, enforce tenant-scoped cache/jobs, and provide rapid credential/plugin revocation.

### Future enhancements

GraphQL read models where justified, event streaming, data-warehouse connectors, embedded apps, partner certification, revenue sharing, CLI tooling, local development tunnel, generated SDKs, and richer workflow actions.

---

## Cross-Module Workflows

### Event experience lifecycle

```text
Organization and entitlements
            ↓
Event configuration and branding
            ↓
Booth session → Capture → Processing/AI → Template render
            ↓                         ↓
        Print queue               Gallery publication
                                      ↓
                              Guest/CRM engagement
                                      ↓
                           Analytics and reporting
                                      ↓
                              Usage and billing
```

### Required consistency rules

| Concern | Cross-module rule |
|---|---|
| Tenant ownership | Resolve from the resource and verify server-side at every boundary |
| Entitlements | Call the centralized permission/usage services; never compare plan names in features |
| Audit | Record sensitive state changes with safe actor, tenant, resource, action, and trace context |
| Jobs | Persist before enqueue, process idempotently, expose status, bound retries, and reconcile usage |
| Storage | Preserve originals, use generated keys/signed access, and meter committed bytes |
| Consent | Capture explicit purpose/version and check current permission at delivery/publication time |
| Analytics | Emit a canonical event once; aggregation and billing consumers must be idempotent |
| Public access | Minimize data, use opaque/signed access, enforce event policy, rate limits, and retention |
| Providers | Infrastructure adapters only; disabled providers remain honestly unavailable |
| Deletion | Coordinate soft delete, retention, legal hold, provider cleanup, analytics policy, and final purge |

## Non-Functional Acceptance Baseline

Each module release must document and verify:

- Authorization matrix and tenant-isolation tests.
- Validation, abuse limits, privacy classification, and audit events.
- SQL Server schema, indexes, migration, query plans, and retention.
- Loading, empty, error, success, retry, offline, and provider-unavailable states.
- Accessibility and responsive behavior appropriate to admin, guest, and kiosk contexts.
- Structured logging, metrics, tracing, job/provider health, and support runbook.
- Unit, integration, critical end-to-end, production build, and migration evidence.
- Entitlement and usage behavior where the module consumes a limited resource.

This specification should be refined through focused feature specifications and ADRs. It must not be used to bypass the dependency, security, or definition-of-done rules in `PROJECT_RULES.md`.
