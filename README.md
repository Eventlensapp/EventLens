# vinext-starter

EventLens AI's Booth Engine includes a tenant-scoped photo strip and template
rendering foundation. Open `/templates` to compose the active booth session's
local captures. Architecture and security details are in
`docs/PHOTO_BOOTH_ENGINE.md`.

A clean full-stack starter running on
[vinext](https://github.com/cloudflare/vinext), with optional Cloudflare D1 and
Drizzle support.

## Prerequisites

- Node.js `>=22.13.0`

## Quick Start

```bash
npm install
npm run dev
npm run build
```

This starter does not use `wrangler.jsonc`.

## Included Shape

- edit site code under `app/`
- `.openai/hosting.json` declares optional Sites D1 and R2 bindings
- `vite.config.ts` simulates declared bindings for local development
- `db/schema.ts` starts intentionally empty
- `examples/d1/` contains an optional D1 example surface
- `drizzle.config.ts` supports local migration generation when needed

## Workspace Auth Headers

OpenAI workspace sites can read the current user's email from
`oai-authenticated-user-email`.

SIWC-authenticated workspace sites may also receive
`oai-authenticated-user-full-name` when the user's SIWC profile has a non-empty
`name` claim. The full-name value is percent-encoded UTF-8 and is accompanied by
`oai-authenticated-user-full-name-encoding: percent-encoded-utf-8`.

Treat the full name as optional and fall back to email when it is absent:

```tsx
import { headers } from "next/headers";

export default async function Home() {
  const requestHeaders = await headers();
  const email = requestHeaders.get("oai-authenticated-user-email");
  const encodedFullName = requestHeaders.get("oai-authenticated-user-full-name");
  const fullName =
    encodedFullName &&
    requestHeaders.get("oai-authenticated-user-full-name-encoding") ===
      "percent-encoded-utf-8"
      ? decodeURIComponent(encodedFullName)
      : null;

  const displayName = fullName ?? email;
  // ...
}
```

## Optional Dispatch-Owned ChatGPT Sign-In

Import the ready-to-use helpers from `app/chatgpt-auth.ts` when the site needs
optional or required ChatGPT sign-in:

- Use `getChatGPTUser()` for optional signed-in UI.
- Use `requireChatGPTUser(returnTo)` for server-rendered pages that should send
  anonymous visitors through Sign in with ChatGPT.
- Use `chatGPTSignInPath(returnTo)` and `chatGPTSignOutPath(returnTo)` for
  browser links or actions.
- Pass a same-origin relative `returnTo` path for the destination after sign-in
  or sign-out. The helper validates and safely encodes it.
- Mark protected pages with `export const dynamic = "force-dynamic"` because
  they depend on per-request identity headers.

Dispatch owns `/signin-with-chatgpt`, `/signout-with-chatgpt`, `/callback`, the
OAuth cookies, and identity header injection. Do not implement app routes for
those reserved paths. Routes that do not import and call the helper remain
anonymous-compatible.

SIWC establishes identity only; it does not prove workspace membership. Use the
Sites hosting platform's access policy controls for workspace-wide restrictions,
or enforce explicit server-side membership or allowlist checks.

Use SIWC for account pages, user-specific dashboards, saved records, and write
actions tied to the current ChatGPT user. Leave public content anonymous.

## Useful Commands

- `npm run dev`: start local development
- `npm run build`: verify the vinext build output
- `npm test`: build the starter and verify its rendered loading skeleton
- `npm run db:generate`: generate Drizzle migrations after schema changes

## Learn More

- [vinext Documentation](https://github.com/cloudflare/vinext)
- [Drizzle D1 Guide](https://orm.drizzle.team/docs/get-started/d1-new)
# Organization team management

Authenticated organization Owners and Managers can manage members at
`/organizations/{organizationId}/members` and invitations at
`/api/invitations`. Invitation tokens are stored only as SHA-256 hashes.
The development email adapter logs delivery metadata but never logs the raw
token; replace `IEmailService` with a production provider and configure the
public application URL before deployment.

The browser team screen is `/organizations/{organizationId}/team`. Invitation
acceptance uses `/invitations/{token}`. Owners are protected from removal or
role replacement without a future ownership-transfer workflow.

## Organization access administration

Owners manage ownership transfer and per-member permission overrides at
`/organizations/{organizationId}/security` (or `/access` for the current
organization). A nominated existing member must accept a transfer within two
days; only then does the previous Owner become Manager. Default permissions are
seeded by role and member overrides are stored independently and audited.
## Organization departments and branches

Organization Owners and Managers can create, update, deactivate, archive, and assign existing
organization members to departments and physical branches. All reads validate organization
membership, all mutations validate Owner/Manager access, and head, manager, and assignment
users must belong to the same organization. Department names and branch codes are unique
inside their organization. Archive and assignment removal use soft deletion to preserve history.

Frontend routes:

- `/organizations/{organizationId}/departments`
- `/organizations/{organizationId}/branches`

REST resources:

- `/api/organizations/{organizationId}/departments`
- `/api/departments/{departmentId}/members`
- `/api/organizations/{organizationId}/branches`
- `/api/branches/{branchId}/members`
## Organization Brand Kit

The organization Brand Kit is the canonical tenant-scoped branding source. It stores logo
variants, accessible colour tokens, typography, watermark behavior, QR defaults, and email
identity settings. Future event, template, gallery, printing, CRM, white-label, and AI modules
should consume `IBrandKitService`/the Brand Kit API rather than copying organization colours.

Brand assets are validated PNG, SVG, JPG, or WEBP files stored through `IStorageService`.
Metadata and version links remain in `brand_assets`; archive operations use soft deletion.
SVG uploads reject active/scriptable content. The current configurable maximum is 10 MB.

Themes and presets snapshot reusable configuration JSON. Theme activation deactivates every
other theme in the same organization in one unit of work, ensuring one active default.

Frontend routes:

- `/organizations/{organizationId}/branding`
- `/organizations/{organizationId}/branding/assets`
- `/organizations/{organizationId}/branding/themes`

API root: `/api/organizations/{organizationId}/branding`.
## Storage Management and Digital Asset Library

The Storage module is the application-level boundary for new platform files. Future modules
must use `IFileService`, `IFolderService`, and `IStorageQuotaService`; provider-specific keys
remain behind the existing low-level `IStorageService` adapter. Internal storage paths are
never returned by Storage APIs.

Files are tenant-scoped, quota checked, SHA-256 deduplicated, scanned through `IFileScanner`,
categorized, tagged, version-ready, audited, and soft-deleted. The development adapter uses a
basic signature scanner that rejects executable extensions and the EICAR test signature; a
production antivirus adapter can replace it without changing application services.

Folders support nesting, cycle prevention, rename, move, archive, and restore. Storage usage
is derived from active file metadata and synchronized with organization quota counters.
Private preview/download requests always validate organization membership.

Frontend routes: `/storage`, `/storage/files`, `/storage/folders`, `/storage/uploads`, and
`/storage/trash`. API root: `/api/storage`.
## Subscription Management and Feature Entitlements

The entitlement engine is separate from payment and invoice processing. It defines the Free,
Creator, Professional, Business, Enterprise, and Custom Enterprise plans, their feature flags,
and resource ceilings. `IEntitlementService` is the mandatory server-side feature gate for new
modules; plan names must never be compared inside feature code.

`ISubscriptionService` manages trials, active/expired/cancelled/suspended/pending states and
entitlement-only plan changes. `IUsageTrackingService` records idempotent referenced usage and
enforces monthly limits. `IPaymentProvider` remains an unimplemented future billing boundary;
the entitlement APIs do not create checkouts, invoices, charges, or provider subscriptions.

Platform plan administration requires SuperAdmin. Organization subscription and usage reads
validate tenant membership; organization plan changes require Owner or Manager membership.

Frontend routes: `/admin/plans`, `/admin/plans/create`, `/admin/plans/{id}`,
`/organization/subscription`, and `/organization/usage`. API root: `/api/entitlements`.
## Event Operations Management

Event Management Phase 5 adds an execution layer for preparation checklists, service-calculated readiness, staff shifts, physical venue zones and booth placements, plus in-app notifications. It does not contain camera capture or booth-engine behavior.

The operations REST resources are under `/api/events/{eventId}/checklists`, `/readiness`, `/staff`, `/zones`, and `/placements`; item updates use `/api/checklists`, `/api/zones`, and `/api/placements`. Notifications are available at `/api/notifications`. The frontend routes are `/events/:id/operations`, `/checklist`, `/staff`, and `/placements`.

All access is checked against the event organization on the server. Owners and Managers can modify operations; Viewers have read-only access; Photographers and Booth Operators require an event staff assignment, and Booth Operators see only checklist tasks assigned to them. The migration `AddEventOperationsManagement` creates the six Phase 5 SQL Server tables and their tenant-safe, non-cascading relationships.

## Booth platform foundation

Module 4 Phase 0 adds a tenant-safe booth control plane at `/booth`, with setup,
settings, and diagnostics pages. It detects browser capabilities and safe
hardware metadata, guides explicit permission requests, validates booth
configuration, initializes the offline data foundation, and exposes a guarded
runtime state machine. It intentionally does not add media capture, GIF,
boomerang, video, DSLR, AI, or photo-processing behavior.

The backend endpoints are under `/api/booth`. The
`AddBoothPlatformFoundation` migration adds configuration, capability, device,
and health-check tables. See [Photo Booth Engine](docs/PHOTO_BOOTH_ENGINE.md)
for the architecture, browser support model, permissions, and Phase 0 limits.

### Camera foundation

Booth Engine Phase 1 adds `/booth/camera`, a preview-only camera management
workspace. It supports browser/web/mobile camera discovery, selection,
resolution fallback, mirroring, aspect ratios, fullscreen preview, capability
inspection, lifecycle recovery, and organization-scoped preferences. The
`AddCameraFoundation` migration adds `camera_preferences`. No frames are
captured, processed, persisted, or uploaded in this phase. See
[Photo Booth Engine](docs/PHOTO_BOOTH_ENGINE.md) for lifecycle and compatibility
details.

### Booth session engine

Booth Engine Phase 2 adds `/booth/session` for guest, operator, and test-session
orchestration. It provides validated session and runtime state machines, secure
restore tokens, activity history, kiosk idle behavior, automatic expiry,
recovery, and operator reset. The `AddBoothSessionEngine` migration extends
`booth_sessions` and creates `booth_session_activities`. This phase manages
workflow state only and does not capture or process media.

## QR and Public Event Experience

Event Management Phase 6 adds secure, storage-backed SVG QR codes and a mobile-first guest entry page at `/e/:token`. QR values use cryptographically random opaque tokens and never contain database identifiers. Owners and Managers configure QR appearance, expiry, public availability, optional password protection, guest sessions, and future sharing permissions from `/events/:id/qr` and `/events/:id/access`.

Public requests are rate-limited and validate token uniqueness, active state, QR expiry, access-policy expiry, and PBKDF2 password hashes. The public DTO returns only guest-safe event, venue, schedule, and inherited branding information. Anonymous guest session tokens are separately generated, while QR scans, page views, and session starts are stored for tenant-authorized analytics. Gallery and download actions intentionally remain unavailable until their owning modules are implemented.
