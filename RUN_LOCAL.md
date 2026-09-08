# Run EventLens AI locally

## Prerequisites

- .NET SDK 9
- Node.js 22+
- SQL Server running as `NAREN`
- SQL Server Management Studio (SSMS)

Trust the ASP.NET development certificate once:

```powershell
dotnet dev-certs https --trust
```

## 1. Create/update the database

From the repository root:

```powershell
cd .\backend\EventLensAI
dotnet tool restore
dotnet ef database update --project .\src\EventLensAI.Infrastructure\EventLensAI.Infrastructure.csproj --startup-project .\src\EventLensAI.API\EventLensAI.API.csproj
```

The API uses Windows authentication and this connection:

```text
Server=NAREN;Database=EventLensAI;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=True
```

Open SSMS, connect to server `NAREN` with Windows Authentication, and refresh
Databases to inspect `EventLensAI`.

## 2. Start the API

In terminal 1:

```powershell
cd .\backend\EventLensAI
dotnet run --project .\src\EventLensAI.API\EventLensAI.API.csproj
```

Swagger: <https://localhost:7180/swagger>

## 3. Start the frontend

In terminal 2:

```powershell
cd <repository-root>
npm install
npm run dev
```

Open the URL printed by the frontend (normally <http://localhost:3000>).
Register through `/register`; registration creates the user, JWT session, and
first organization. The selected organization is then shared by Events, CRM,
Analytics, Billing, AI Studio, Gallery, and booth workflows.

## API address override

The default API address is `https://localhost:7180`. To use another address,
run this in the browser console and reload:

```js
localStorage.setItem("eventlens_api_origin", "https://your-api-host");
location.reload();
```

## Current external-provider boundaries

Core workflows run locally. AI processing and paid-plan checkout require real
provider adapters and credentials. The backend intentionally keeps those
providers disabled until configured; it does not simulate successful AI jobs or
payments. Camera access requires `localhost` or HTTPS and browser permission.
