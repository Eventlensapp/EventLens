# Repository Structure

## Proposed Monorepo

```text
eventlens-ai/
├── apps/
│   ├── web/                         # React + TypeScript + Vite
│   │   ├── public/
│   │   ├── src/
│   │   │   ├── app/                # providers, router, query client
│   │   │   ├── assets/
│   │   │   ├── components/         # shared presentational components
│   │   │   ├── features/
│   │   │   │   ├── auth/
│   │   │   │   ├── organizations/
│   │   │   │   ├── events/
│   │   │   │   ├── capture/
│   │   │   │   ├── editor/
│   │   │   │   ├── ai-studio/
│   │   │   │   ├── galleries/
│   │   │   │   ├── live/
│   │   │   │   ├── memory-books/
│   │   │   │   ├── crm/
│   │   │   │   ├── analytics/
│   │   │   │   └── billing/
│   │   │   ├── layouts/
│   │   │   ├── lib/                # API client, errors, auth helpers
│   │   │   ├── routes/
│   │   │   ├── stores/             # device/editor UI state only
│   │   │   ├── styles/
│   │   │   ├── test/
│   │   │   └── types/
│   │   └── vite.config.ts
│   ├── api/
│   │   ├── EventLens.Api/
│   │   │   ├── Endpoints/
│   │   │   ├── Hubs/
│   │   │   ├── Middleware/
│   │   │   ├── OpenApi/
│   │   │   └── Program.cs
│   │   └── EventLens.Api.csproj
│   └── worker/
│       ├── EventLens.Worker/
│       │   ├── Jobs/
│       │   ├── Consumers/
│       │   └── Program.cs
│       └── EventLens.Worker.csproj
├── src/
│   ├── BuildingBlocks/
│   │   ├── EventLens.Domain/
│   │   ├── EventLens.Application/
│   │   ├── EventLens.Infrastructure/
│   │   ├── EventLens.Contracts/
│   │   └── EventLens.Testing/
│   └── Modules/
│       ├── Identity/
│       │   ├── EventLens.Identity.Domain/
│       │   ├── EventLens.Identity.Application/
│       │   ├── EventLens.Identity.Infrastructure/
│       │   ├── EventLens.Identity.Presentation/
│       │   └── EventLens.Identity.Contracts/
│       ├── Tenancy/
│       ├── Billing/
│       ├── Events/
│       ├── Capture/
│       ├── Media/
│       ├── Design/
│       ├── AIStudio/
│       ├── Galleries/
│       ├── Guests/
│       ├── MemoryBooks/
│       ├── Analytics/
│       └── Notifications/
├── tests/
│   ├── ArchitectureTests/
│   ├── UnitTests/
│   │   └── Modules/
│   ├── IntegrationTests/
│   │   └── Modules/
│   ├── ApiContractTests/
│   └── EndToEndTests/
├── database/
│   ├── migrations/                 # reviewed migration artifacts
│   ├── seeds/                      # development-only seed data
│   └── diagrams/
├── deploy/
│   ├── docker/
│   │   ├── api.Dockerfile
│   │   ├── worker.Dockerfile
│   │   └── web.Dockerfile
│   ├── nginx/
│   ├── compose/
│   ├── kubernetes/                 # only when deployment requires it
│   └── scripts/
├── docs/
│   ├── architecture/
│   │   ├── decisions/              # ADRs
│   │   ├── diagrams/
│   │   └── threat-model/
│   ├── api/
│   ├── operations/
│   └── product/
├── .github/
│   └── workflows/
├── Directory.Build.props
├── Directory.Packages.props
├── EventLens.sln
├── docker-compose.yml
└── README.md
```

## Backend Module Template

```text
EventLens.<Module>.Domain/
├── Aggregates/
├── Entities/
├── ValueObjects/
├── Events/
├── Policies/
├── Errors/
└── Repositories/                   # domain-facing abstractions only

EventLens.<Module>.Application/
├── Abstractions/
├── Authorization/
├── Commands/<UseCase>/
├── Queries/<UseCase>/
├── EventHandlers/
├── IntegrationEventHandlers/
├── Models/
└── Validation/

EventLens.<Module>.Infrastructure/
├── Persistence/
│   ├── Configurations/
│   ├── Migrations/
│   └── Repositories/
├── Providers/
├── Jobs/
├── DependencyInjection.cs
└── ModuleDbContext.cs

EventLens.<Module>.Presentation/
├── Endpoints/V1/
├── Hubs/
├── Contracts/
└── DependencyInjection.cs
```

## Dependency Rules

```text
Domain ← Application ← Presentation
   ↑           ↑
   └──── Infrastructure
```

- Domain references no module infrastructure or presentation project.
- Application references its domain and stable building blocks.
- Infrastructure implements application/domain abstractions.
- Presentation references application contracts, not EF Core.
- Modules do not reference another module's Infrastructure project.
- Cross-module integration contracts live in the publishing module's Contracts project.
- Architecture tests enforce these rules.

## Frontend Feature Template

```text
features/events/
├── api/
│   ├── eventKeys.ts
│   ├── eventQueries.ts
│   └── eventMutations.ts
├── components/
├── hooks/
├── routes/
├── schemas/                         # client validation
├── state/                           # transient workflow state
├── types/
└── index.ts                         # feature public API
```

Feature modules import shared UI and platform libraries but not another feature's internal files. Shared business interactions move to typed application-level orchestration rather than circular feature imports.

## Required Architecture Decision Records

Before implementation, create ADRs for:

1. modular monolith and module boundaries;
2. tenant isolation strategy;
3. authentication and refresh-token transport;
4. object-storage abstraction and media lifecycle;
5. background job engine and transactional outbox;
6. SignalR scale-out choice;
7. editor engine selection: Fabric.js versus Konva.js;
8. AI provider routing and cost accounting;
9. billing/entitlement model;
10. analytics event model and retention.

