# EventLensAI Backend

The professional camera adapter layer is managed through `/api/booth/cameras` and `/booth/professional-cameras`. Browser cameras are available now. Canon, Nikon, Sony, DSLR, and mirrorless providers expose honest unavailable states until a signed workstation bridge implements the stable adapter contract.

Advanced booth media capture is available at `/booth/capture-modes`, with tenant-scoped lifecycle APIs under `/api/booth/media`. GIF, boomerang, video, burst, time-lapse, and live-photo media remain device-local; SQL stores secure metadata and processing state.

Professional browser camera controls are available at `/booth/camera-controls`. The operator panel detects active-track capabilities, applies supported controls live, and manages audited organization camera profiles. Vendor DSLR and mirrorless SDKs are intentionally excluded; future adapters implement the same stable control contracts without changing booth workflows.

Production-oriented ASP.NET Core 9 Web API foundation using Clean Architecture,
SQL Server, EF Core, JWT authentication, FluentValidation, AutoMapper, Serilog,
Swagger, repository/unit-of-work abstractions, security headers, CORS, and rate limiting.

## Projects

- `EventLensAI.Domain` — entities, enums, value objects, and domain primitives.
- `EventLensAI.Application` — interfaces, DTOs, validators, mappings, exceptions, and authentication use cases.
- `EventLensAI.Infrastructure` — EF Core, SQL Server, repositories, identity, storage, and processing services.
- `EventLensAI.API` — controllers, filters, middleware, configuration, and Swagger.

## Local prerequisites

- .NET 9 SDK
- SQL Server 2022 Developer or Express
- SQL Server Management Studio 20+
- EF Core CLI 9

> .NET 9 is used because it is an explicit project requirement. It is an STS
> release and is no longer supported as of this repository date. Before a public
> production launch, schedule an upgrade to the current LTS runtime.

## Run

```powershell
cd backend/EventLensAI
dotnet tool install --global dotnet-ef --version 9.*
dotnet ef database update --project src/EventLensAI.Infrastructure --startup-project src/EventLensAI.API
dotnet run --project src/EventLensAI.API
```

Swagger is available at `https://localhost:7180/swagger` in Development.

## Authentication

```text
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/logout
```

Access tokens last 15 minutes. Refresh tokens are random 512-bit values, stored
only as SHA-256 hashes, rotated on use, and protected against token-family reuse.
Browser clients also receive the refresh token as a Secure, HttpOnly, SameSite
cookie. Native clients may use the response value.

The initial migration seeds:

- SuperAdmin
- Owner
- Manager
- Photographer
- Guest
- Editor
- Viewer

## Organization and event APIs

Authenticated clients can manage organizations under `/api/organizations`,
including members, seven-day invitations, and role changes. Event CRUD,
filtering, sorting, pagination, publishing, archiving, duplication, settings,
and QR generation are exposed under `/api/events`. Public event links use
`/event/{slug}`.

Create, update, delete, publish, and archive operations record the actor, time,
and request IP. `InitialSqlServer` creates the complete current schema and role
seeds for SQL Server.

## SQL Server connection

The default development connection uses Windows authentication:

```text
Server=NAREN;Database=EventLensAI;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=True
```

For SQL Server Express, use `Server=localhost\SQLEXPRESS`. For LocalDB, use
`Server=(localdb)\MSSQLLocalDB`. Override this with
`ConnectionStrings__SQLServer` in deployed environments.

`Encrypt=False` is intended only for this local SQL Server/SSMS setup. Use
encrypted connections with a trusted server certificate in production.

### Apply with SSMS

If Windows authentication from `dotnet ef` is unavailable:

1. Connect to the local SQL Server in SSMS.
2. Create an empty database named `EventLensAI`.
3. Open `EventLensAI.SqlServer.sql`.
4. Select `EventLensAI` in the database dropdown and execute the script.
5. Refresh Databases → EventLensAI → Tables.

The generated script is idempotent and includes the complete schema, migration
history, indexes, relationships, and seeded roles.

If the application receives an SSPI authentication error, create a dedicated
SQL Server login in SSMS and provide its credentials through an environment
variable:

```powershell
$env:ConnectionStrings__SQLServer="Server=NAREN;Database=EventLensAI;User Id=eventlens_app;Password=YOUR_PASSWORD;Encrypt=False;MultipleActiveResultSets=True"
dotnet run --project src/EventLensAI.API
```

## Tests

```powershell
dotnet test EventLensAI.sln
```

The unit suite covers organization/event validation, event lifecycle rules, and
the event-management role boundary.

## Production configuration

Do not deploy `appsettings.json` secrets. Supply the settings shown in
`.env.example` using environment variables or a secret manager. Use a unique
high-entropy JWT signing key, restricted CORS origins, TLS, and a non-default
database account.

## Future migrations

```powershell
dotnet ef migrations add <Name> --project src/EventLensAI.Infrastructure --startup-project src/EventLensAI.API
```

## Event Management Phase 1

Event Core is a tenant-owned aggregate. `event_types` provides database-driven system and
future organization event types; `event_members` assigns organization users to event roles.
The event service verifies organization membership, branch/type/brand ownership, assigned
manager membership, lifecycle transitions, and the centralized event entitlement before a
write. Events are archived with soft deletion and can be restored.

Primary routes are `POST/GET /api/events`, `GET/PUT /api/events/{id}`,
`POST /api/events/{id}/archive|restore|clone`, `PUT /api/events/{id}/status`,
`GET /api/event-types`, and CRUD assignment routes under `/api/events/{id}/members`.
Organization Owner and Manager roles manage events; other organization members receive
read access only, subject to tenant membership.

### Event Management Phase 2

Dynamic event types combine global read-only system types with organization-owned custom
types. `event_templates` stores tenant-scoped reusable event type, brand profile, duration,
and JSON feature defaults. `event_template_usages` records which template produced an event
for future analytics without coupling templates to event history.

Owners and Managers may create, edit, clone, and archive organization templates and custom
event types. System event types/templates are protected from mutation. `POST /api/events`
accepts an optional `templateId`, validates template ownership, applies type/brand/duration
and stored feature defaults, and records usage. Template feature flags are configuration
only; Phase 2 does not activate Booth, Gallery, AI, CRM, or Printing modules.

### Event Management Phase 3

Brand resolution follows `System defaults -> Organization Brand Kit -> optional Brand Profile
-> EventBrandConfiguration overrides -> EventExperienceSettings`. Event overrides persist
only changed values; reads resolve missing colours and typography from the organization kit.

`event_assets` stores metadata and provider keys only. Bytes are written through
`IStorageService`; URLs are resolved at the application boundary. Branding, welcome images,
and sponsor logos must reference assets owned by the same event.

Owners, Managers, and Designers can modify branding, experience, assets, and sponsors.
Other organization members, including Viewers, have read-only access. Every operation loads
the event and verifies current-user organization membership.

### Event Management Phase 4

Event venues are distinct from organization branches. An event may have one primary venue
and multiple zones, each with address, coordinates, provider reference, contact, and capacity.
System venue types are shared globally while the schema supports future organization types.

Event schedule items form an ordered timeline with a dynamic schedule type, optional
same-event venue, UTC start/end, lifecycle status, and display order. The calendar query
service exposes bounded calendar entries without coupling the application to Google or
Outlook provider SDKs.

Owners and Managers can modify venue and schedule data. Other organization members,
including Photographers, Designers, and Viewers, have read-only access. Venue and type
references are always resolved through the owning event and organization.
