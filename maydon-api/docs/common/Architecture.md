# Common Module — Architecture

## Overview

The Common module provides **shared reference data** (regions, districts, languages, banks, currencies) and **authorization infrastructure** (role-permissions) consumed by all other modules. It follows the same layered architecture as the rest of the system.

> [!NOTE]
> This is a **data-only** module — it has no external integrations (no AWS SNS, no HTTP clients). All operations are database CRUD with i18n support via domain events.

---

## Folder Structure

```
src/Common/
├── Common.Domain/              # Entities, value objects, domain events
│   ├── AssemblyReference.cs    # Instance = "commons", schema name
│   ├── Banks/
│   │   ├── Bank.cs, BankTranslate.cs
│   │   └── Events/             # CreateOrUpdate, Remove
│   ├── Currencies/
│   │   ├── Currency.cs, CurrencyTranslate.cs
│   │   └── Events/             # CreateOrUpdate, Remove
│   ├── Districts/
│   │   ├── District.cs, DistrictTranslate.cs
│   │   └── Events/             # Upsert, Remove
│   ├── Languages/
│   │   └── Language.cs
│   ├── Regions/
│   │   ├── Region.cs, RegionTranslate.cs
│   │   └── Events/             # Upsert, Remove
│   └── RolePermissions/
│       └── RolePermission.cs
│
├── Common.Application/         # CQRS handlers, validators, responses
│   ├── DependencyInjection.cs  # Empty (no custom services to register)
│   ├── Core/
│   │   ├── Abstractions/Data/
│   │   │   └── ICommonDbContext.cs
│   │   └── Resources/          # Localization resources
│   ├── Banks/
│   │   ├── Create/             # Command + Handler + Validator + DomainEventHandler
│   │   ├── Update/             # Command + Handler + Validator
│   │   ├── Remove/             # Command + Handler + Validator + DomainEventHandler
│   │   ├── Get/                # Query + Handler + Response (paginated)
│   │   └── GetById/            # Query + Handler + Response
│   ├── Currencies/             # Same CRUD + MoveUp/MoveDown + SetOrder
│   ├── Districts/              # Same CRUD + MoveUp/MoveDown + SetOrder
│   ├── Languages/              # Same CRUD + MoveUp/MoveDown + SetOrder
│   └── Regions/                # Same CRUD + MoveUp/MoveDown + SetOrder
│
├── Common.Infrastructure/      # EF Core, authorization
│   ├── DependencyInjection.cs  # Registers DB + Authorization
│   ├── Authorization/
│   │   └── CommonPermissionProvider.cs
│   ├── Database/
│   │   └── CommonDbContext.cs  # Internal, uses IExecutionContextProvider
│   └── Extensions/
│       └── DatabaseExtensions.cs
```

---

## Layers

### Domain Layer (`Common.Domain`)

- **Rich domain model**: Each aggregate root (Bank, Currency, Region, District) follows the same pattern:
  - Private default constructor (EF Core)
  - Public constructor raises a `CreateOrUpdate*DomainEvent` with `LanguageValue` collection
  - `Update()` method raises the same upsert event
  - `Remove()` method raises a `Remove*DomainEvent`
  - `SetOrder(short order)` for manual sorting
  - `ICollection<*Translate> Translates` navigation property

- **Translation pattern**: All translatable entities use a `*Translate` child entity with:
  - `LanguageId` + `LanguageShortCode` + `Value`
  - FK back to parent entity
  - FK to `Language` entity (except `BankTranslate` which only has `BankId` FK)

- **`Language`** is special: no translate table, has `Activate()`/`Deactivate()` lifecycle methods

- **`RolePermission`**: Simple join entity (RoleId + PermissionId), no domain events, no update method

### Application Layer (`Common.Application`)

- **CQRS** via MediatR-style `ICommandHandler<TCommand>` and `IQueryHandler<TQuery, TResponse>`
- **Standard operations per aggregate**: Create, Update, Remove, Get (paginated), GetById
- **Ordering support**: `MoveUp` and `MoveDown` commands + `SetOrder` command for display ordering
- **Domain event handlers**:
  - `CreateOrUpdate*DomainEventHandler` → upserts `*Translate` records (matched by LanguageId)
  - `Remove*DomainEventHandler` → soft-deletes `*Translate` records
- **Validators**: FluentValidation for all commands
- **No custom services**: `DependencyInjection.cs` is empty

### Infrastructure Layer (`Common.Infrastructure`)

#### Database

- **`CommonDbContext`** (`internal sealed`): Inherits `DbContext`, implements `ICommonDbContext`
- **Schema**: `commons`
- **Compiled queries**: `RolePermissionNamesAsync` for efficient permission lookups
- **Global query filters** (via `SetGlobalQuery<T>`):

| Filter         | Entities                                                    |
|---------------|-------------------------------------------------------------|
| **IsDeleted** | All entities                                                |
| **IsInstance**| Permission (filters to `Instance == "commons"`)             |
| **IsActive**  | Language, Permission                                        |
| **Translate** | BankTranslate, CurrencyTranslate, DistrictTranslate, RegionTranslate, PermissionTranslate |

#### Authorization

- **`CommonPermissionProvider`**: Implements `IPermissionProvider` by querying `RolePermission → Permission` via compiled async query
- Registered internally in DI as authorization service

---

## Dependency Injection

```csharp
// Common.Infrastructure.DependencyInjection
services.AddAuthorizationInternal()   // Registers CommonPermissionProvider
        .AddDatabase(configuration);  // Registers CommonDbContext + Npgsql

// Common.Application.DependencyInjection
// Empty — no custom services (handlers auto-discovered by MediatR assembly scanning)
```

---

## Cross-Module Usage

The Common entities are **shared** and mapped into other modules' DbContexts as non-owned read references:

| Consumer Module | Common Entities Used                              |
|-----------------|---------------------------------------------------|
| **Identity**    | Language, RolePermission, Permission              |
| **Building**    | Region, RegionTranslate, District, DistrictTranslate, Language, Permission, RolePermission |
| **Document**    | Language                                          |
| **Didox**       | (via Core Permission system)                      |

> [!IMPORTANT]
> Other modules reference these entities directly (mapped via `DbSet` in their own `DbContext`s) rather than through a service layer. This is a deliberate design choice for static reference data that changes infrequently.

---

## Technology Stack

| Area           | Technology                            |
|----------------|---------------------------------------|
| Runtime        | .NET 10, C# 13                        |
| ORM            | EF Core (PostgreSQL via Npgsql)       |
| Schema         | `commons`                             |
| Validation     | FluentValidation                      |
| Messaging      | MediatR (CQRS + Domain Events)        |
| Auth           | Permission-based via compiled queries |
