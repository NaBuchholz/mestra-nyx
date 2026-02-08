# Copilot instructions for Mestra Nyx

## Big picture

- Solution is a multi-project .NET 9 setup: Domain → Application → Infrastructure → API (see src/MestraNyx.sln).
- Current API is a minimal ASP.NET Core app with a sample /weatherforecast endpoint in src/MestraNyx.API/Program.cs.
- Application and Domain are scaffolding-only right now (placeholders), but folders indicate CQRS-style organization (e.g., Campaigns/Commands, Queries, DTOs).
- Infrastructure references EF Core + Npgsql, so persistence is expected to live in src/MestraNyx.Infrastructure/Persistence (no DbContext yet).
- Web frontend folder src/mestra-nyx-web exists but only contains placeholders.
- Product vision (user-provided context): Clean Architecture + DDD-lite backend with CQRS/MediatR, EF Core + PostgreSQL, and a Next.js (App Router) frontend with React 19 + TS.
- Planned integrations (user-provided context, not implemented yet): Semantic Kernel, SignalR, Discord bot (DSharpPlus), Foundry VTT plugin, Obsidian content sync.

## Key dependencies and integration points

- API: MediatR, FluentValidation.AspNetCore, Serilog (console + file), OpenAPI (see src/MestraNyx.API/MestraNyx.API.csproj).
- Infrastructure: EF Core + Npgsql for PostgreSQL (see src/MestraNyx.Infrastructure/MestraNyx.Infrastructure.csproj).
- Dev container includes .NET 9, Node 20, and pnpm (see .devcontainer/devcontainer.json).

## Developer workflows (observed)

- Run API: dotnet run --project src/MestraNyx.API (uses launchSettings.json for http/https ports).
- Test the sample endpoint with the REST client file at src/MestraNyx.API/MestraNyx.API.http.

## Conventions to follow

- Keep cross-project references aligned with the layering:
  - Domain has no dependencies.
  - Application depends on Domain only.
  - Infrastructure depends on Domain + Application.
  - API depends on Application + Infrastructure.
- Place feature logic under Application/_ (e.g., Campaigns/Commands, Queries, DTOs) and infrastructure concerns under Infrastructure/_.
- If you add persistence, keep EF Core configuration under Infrastructure/Persistence and its subfolders.
