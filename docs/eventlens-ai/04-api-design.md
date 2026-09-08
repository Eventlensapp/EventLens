# API Design

## 1. Standards

- Base path: `/api/v1`
- JSON uses camelCase.
- Timestamps use ISO 8601 UTC.
- IDs are opaque UUID strings.
- Errors use `application/problem+json` following RFC 9457.
- Collection pagination uses opaque cursor pagination for high-volume resources.
- Write endpoints accept `Idempotency-Key` where retries could duplicate work.
- Concurrency-sensitive updates use `ETag`/`If-Match` or an explicit version field.
- OpenAPI is generated from the API and checked into published documentation.

Example error:

```json
{
  "type": "https://eventlens.ai/problems/validation",
  "title": "Validation failed",
  "status": 400,
  "traceId": "01J...",
  "errors": {
    "eventName": ["Event name is required."]
  }
}
```

## 2. Authentication

```text
POST   /auth/register
POST   /auth/login
POST   /auth/refresh
POST   /auth/logout
POST   /auth/logout-all
POST   /auth/verify-email
POST   /auth/resend-verification
POST   /auth/forgot-password
POST   /auth/reset-password
GET    /auth/me
```

Access tokens are short lived. Refresh-token rotation invalidates the previous token and detects reuse at the token-family level. Password reset and email verification responses do not reveal whether an account exists.

## 3. Organizations and Membership

```text
GET    /organizations
POST   /organizations
GET    /organizations/{organizationId}
PATCH  /organizations/{organizationId}
GET    /organizations/{organizationId}/members
PATCH  /organizations/{organizationId}/members/{memberId}
DELETE /organizations/{organizationId}/members/{memberId}
GET    /organizations/{organizationId}/invitations
POST   /organizations/{organizationId}/invitations
DELETE /organizations/{organizationId}/invitations/{invitationId}
POST   /invitations/{token}/accept
GET    /organizations/{organizationId}/usage
GET    /organizations/{organizationId}/entitlements
```

## 4. Events

```text
GET    /organizations/{organizationId}/events
POST   /organizations/{organizationId}/events
GET    /organizations/{organizationId}/events/{eventId}
PATCH  /organizations/{organizationId}/events/{eventId}
DELETE /organizations/{organizationId}/events/{eventId}
POST   /organizations/{organizationId}/events/{eventId}/publish
POST   /organizations/{organizationId}/events/{eventId}/archive
GET    /organizations/{organizationId}/events/{eventId}/branding
PUT    /organizations/{organizationId}/events/{eventId}/branding
GET    /organizations/{organizationId}/events/{eventId}/settings
PUT    /organizations/{organizationId}/events/{eventId}/settings
GET    /organizations/{organizationId}/events/{eventId}/qr-code
```

Event creation includes name, type, timezone-aware date range, venue, initial theme, colors and requested slug. Logo upload uses the media upload flow.

## 5. Devices and Capture

```text
POST   /events/{eventId}/devices/register
POST   /events/{eventId}/devices/{deviceId}/heartbeat
DELETE /events/{eventId}/devices/{deviceId}

POST   /events/{eventId}/sessions
GET    /events/{eventId}/sessions/{sessionId}
POST   /events/{eventId}/sessions/{sessionId}/captures
POST   /events/{eventId}/sessions/{sessionId}/complete
POST   /events/{eventId}/sessions/{sessionId}/cancel
```

Capture creation records mode (`photo`, `gif`, `boomerang`, `video`), sequence position and device metadata. DSLR support should use a separate local bridge contract; browsers do not receive unrestricted native camera access.

## 6. Media Upload and Photos

```text
POST   /events/{eventId}/media/uploads
POST   /events/{eventId}/media/uploads/{uploadId}/complete
DELETE /events/{eventId}/media/uploads/{uploadId}

GET    /events/{eventId}/photos
GET    /events/{eventId}/photos/{photoId}
PATCH  /events/{eventId}/photos/{photoId}
DELETE /events/{eventId}/photos/{photoId}
POST   /events/{eventId}/photos/{photoId}/restore
GET    /events/{eventId}/photos/{photoId}/download
```

Upload initiation request:

```json
{
  "fileName": "capture.jpg",
  "contentType": "image/jpeg",
  "sizeBytes": 3819221,
  "checksumSha256": "base64...",
  "purpose": "originalCapture"
}
```

The response contains a short-lived provider-neutral upload instruction. Completion verifies object metadata before marking media ready.

## 7. Templates, Stickers and Designs

```text
GET    /template-categories
GET    /templates
POST   /organizations/{organizationId}/templates
GET    /templates/{templateId}
POST   /templates/{templateId}/versions
POST   /templates/{templateId}/versions/{versionId}/publish
POST   /templates/{templateId}/clone

GET    /stickers
POST   /organizations/{organizationId}/stickers

POST   /events/{eventId}/designs
GET    /events/{eventId}/designs/{designId}
PUT    /events/{eventId}/designs/{designId}
POST   /events/{eventId}/designs/{designId}/render
GET    /events/{eventId}/designs/{designId}/exports/{exportId}
```

The editor persists a versioned, provider-neutral design document. Render endpoints support PNG, JPEG and print-ready PDF.

## 8. AI Studio

```text
GET    /ai/capabilities
POST   /events/{eventId}/ai/jobs
GET    /events/{eventId}/ai/jobs
GET    /events/{eventId}/ai/jobs/{jobId}
POST   /events/{eventId}/ai/jobs/{jobId}/cancel
POST   /events/{eventId}/ai/jobs/{jobId}/retry
```

Request:

```json
{
  "photoId": "uuid",
  "operation": "styleTransfer",
  "preset": "cinematic",
  "parameters": {
    "strength": 0.72
  }
}
```

The initial response is `202 Accepted` with a job resource URL. Provider names and raw provider parameters are not part of the public contract.

## 9. Galleries

Operator API:

```text
GET    /events/{eventId}/galleries
POST   /events/{eventId}/galleries
GET    /events/{eventId}/galleries/{galleryId}
PATCH  /events/{eventId}/galleries/{galleryId}
POST   /events/{eventId}/galleries/{galleryId}/publish
POST   /events/{eventId}/galleries/{galleryId}/unpublish
PUT    /events/{eventId}/galleries/{galleryId}/items
```

Public/guest API:

```text
GET    /public/events/{slug}
POST   /public/events/{slug}/access
GET    /public/events/{slug}/photos
GET    /public/events/{slug}/photos/{photoId}
POST   /public/events/{slug}/photos/{photoId}/favorite
DELETE /public/events/{slug}/photos/{photoId}/favorite
POST   /public/events/{slug}/photos/{photoId}/download
POST   /public/events/{slug}/photos/{photoId}/share
```

Private gallery access exchanges a PIN/access code for a short-lived, gallery-scoped session. Search capabilities must not expose face recognition without explicit separate approval.

## 10. Guests and CRM

```text
GET    /events/{eventId}/guests
POST   /events/{eventId}/guests
GET    /events/{eventId}/guests/{guestId}
PATCH  /events/{eventId}/guests/{guestId}
DELETE /events/{eventId}/guests/{guestId}
POST   /events/{eventId}/guests/{guestId}/consents
POST   /events/{eventId}/guests/{guestId}/feedback
POST   /events/{eventId}/guests/export

GET    /organizations/{organizationId}/campaigns
POST   /organizations/{organizationId}/campaigns
POST   /organizations/{organizationId}/campaigns/{campaignId}/schedule
POST   /organizations/{organizationId}/campaigns/{campaignId}/cancel
GET    /organizations/{organizationId}/campaigns/{campaignId}/report
```

Campaign recipient selection always enforces current channel-specific consent and suppression rules server-side.

## 11. Analytics

```text
GET    /organizations/{organizationId}/analytics/overview
GET    /events/{eventId}/analytics/overview
GET    /events/{eventId}/analytics/timeseries
GET    /events/{eventId}/analytics/templates
GET    /events/{eventId}/analytics/ai-usage
POST   /events/{eventId}/analytics/export
```

Internal analytics ingestion is not a general public endpoint. Trusted web/device clients submit a narrow allowlist of events with server-added tenant and event context.

## 12. Memory Books

```text
GET    /events/{eventId}/memory-books
POST   /events/{eventId}/memory-books
GET    /events/{eventId}/memory-books/{memoryBookId}
PATCH  /events/{eventId}/memory-books/{memoryBookId}
POST   /events/{eventId}/memory-books/{memoryBookId}/generate
PUT    /events/{eventId}/memory-books/{memoryBookId}/items
POST   /events/{eventId}/memory-books/{memoryBookId}/export
```

## 13. Billing

```text
GET    /billing/plans
GET    /organizations/{organizationId}/billing/subscription
POST   /organizations/{organizationId}/billing/checkout
POST   /organizations/{organizationId}/billing/portal
GET    /organizations/{organizationId}/billing/invoices
POST   /webhooks/billing/{provider}
```

Webhook endpoints verify signatures against the raw request body, store receipt IDs for deduplication and process changes asynchronously.

## 14. Platform Administration

```text
GET    /admin/organizations
GET    /admin/users
GET    /admin/jobs
POST   /admin/jobs/{jobId}/retry
GET    /admin/provider-health
GET    /admin/audit-log
```

These endpoints require platform-scoped Super Admin authorization and enhanced audit logging.

## 15. SignalR Contracts

### Client methods

- `JoinEvent(eventId)`
- `LeaveEvent(eventId)`
- `JoinGallery(galleryId, gallerySessionToken?)`
- `JoinSlideshow(galleryId, gallerySessionToken?)`
- `JoinJob(jobId)`

### Server events

- `PhotoCaptured`
- `PhotoProcessingUpdated`
- `GalleryItemPublished`
- `GalleryItemRemoved`
- `SlideshowStateChanged`
- `AIJobProgressed`
- `AIJobCompleted`
- `EventConfigurationChanged`

Payloads are minimal DTOs and contain no storage-provider keys or unauthorized guest data.

## 16. Integration Events

Versioned internal contracts include:

- `Identity.UserRegistered.v1`
- `Tenancy.MemberInvited.v1`
- `Events.EventPublished.v1`
- `Capture.SessionCompleted.v1`
- `Media.AssetReady.v1`
- `Media.AssetDeleted.v1`
- `AI.JobCompleted.v1`
- `Galleries.ItemPublished.v1`
- `Billing.SubscriptionChanged.v1`
- `Guests.MarketingConsentChanged.v1`

Consumers are idempotent and persist processed message IDs where side effects require exactly-once business behavior.

