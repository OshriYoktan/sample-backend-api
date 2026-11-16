# AGENTS.md: conventions for AI agents working in this repository

## Architecture
Layered ASP.NET Core Web API (.NET 10) for an Orders domain:

`Controllers/` → `Services/` (business rules) → `Repositories/` (EF Core queries) → `Data/OrdersDbContext`

- **Controllers** are thin: route, call a service or repository, map to an HTTP result. Read-only lookups may call a repository directly; anything with business rules goes through a service.
- **Services** hold business rules and throw `OrderValidationException` for rule violations (controllers translate it to 400).
- **Repositories** own all EF Core querying. Use `AsNoTracking()` for reads. An interface and its implementation live in the same file.
- **DTOs** are records in `Dtos/Dtos.cs`. Entities are never returned from controllers.
- **Mapping** is manual, via extension methods in `Mapping/DtoMappings.cs` (`ToDto()`). No AutoMapper here.
- **Time:** inject `TimeProvider`; never call `DateTime.UtcNow` directly (keeps tests deterministic).

## Conventions
- Routes: `api/{plural-resource}`, nested reads such as `api/customers/{id}/orders`.
- Every async method takes a `CancellationToken ct`.
- Missing resources return `404 NotFound()`; validation failures return `400` with `ProblemDetails`.
- New repository and service registrations go in `Program.cs` as `AddScoped`.

## Tests
- xUnit in `tests/Orders.Api.Tests`, using the EF Core InMemory provider with the seed data from `Data/SeedData.cs`.
- Run with: `dotnet test`

## Data
SQL Server (LocalDB in development). The schema is created with `EnsureCreated()` in Development, so there are no migrations yet.
