# EventLens AI Backend Foundation

ASP.NET Core 9 Web API organized as a Clean Architecture solution. This foundation includes
PostgreSQL persistence, Entity Framework Core migrations, JWT bearer authentication,
PBKDF2-SHA512 password hashing, rotating refresh tokens, Swagger, and Serilog.

## Projects

- `PhotoBooth.Domain`: entities and domain rules; no external dependencies.
- `PhotoBooth.Application`: use cases, DTOs, and persistence/authentication abstractions.
- `PhotoBooth.Infrastructure`: EF Core, PostgreSQL, repositories, password hashing, and tokens.
- `PhotoBooth.Api`: versioned controllers, authentication pipeline, Swagger, and Serilog.

The project names retain the original scaffold namespace for now; product-facing API metadata
uses EventLens AI. A namespace rename can be handled as a separate mechanical change.

## Run

Requires the .NET 9 SDK and Docker.

```powershell
$env:POSTGRES_PASSWORD="choose-a-local-password"
$env:ConnectionStrings__PostgreSQL="Host=localhost;Port=5432;Database=eventlens;Username=postgres;Password=$env:POSTGRES_PASSWORD"
$env:EVENTLENS_POSTGRES=$env:ConnectionStrings__PostgreSQL
$env:Jwt__Key="choose-a-random-signing-key-of-at-least-32-characters"
docker compose up -d
dotnet ef database update --project src/PhotoBooth.Infrastructure --startup-project src/PhotoBooth.Api
dotnet run --project src/PhotoBooth.Api
```

Open `http://localhost:5080/swagger`.

Authentication endpoints:

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`
- `GET /api/v1/auth/me`

Refresh tokens are rotated, stored only as SHA-256 hashes in PostgreSQL, and returned both
in the response for native clients and as a secure HTTP-only cookie for browser clients.

Provide `ConnectionStrings__PostgreSQL`, `Jwt__Key`, `Jwt__Issuer`, and
`Jwt__Audience` through environment variables or a secret manager. The design-time
database factory reads `EVENTLENS_POSTGRES`. Never commit signing keys or database
passwords.

## Create future migrations

Install the EF CLI matching .NET 9, then run:

```powershell
dotnet tool install --global dotnet-ef --version 9.*
dotnet ef migrations add <MigrationName> --project src/PhotoBooth.Infrastructure --startup-project src/PhotoBooth.Api
```
