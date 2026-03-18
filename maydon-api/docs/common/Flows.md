# Common Module — Flows

## Overview

The Common module exposes **read-only public endpoints** via `Maydon.Host`. All write operations (Create, Update, Remove, Reorder) exist as CQRS handlers in the Application layer but are **not exposed** through the Host API — they are consumed internally (e.g., admin panel, seeding, or direct handler invocation).

---

## Public API Endpoints

All endpoints are **anonymous** (`AllowAnonymous()`) and return **paginated** results.

| Method | Route              | Handler                          | Auth        | Response                          |
|--------|--------------------|----------------------------------|-------------|-----------------------------------|
| GET    | `/commons/`        | `GetBanksQueryHandler`           | Anonymous   | `PagedList<GetBanksResponse>`     |
| GET    | `/commons/`        | `GetCurrenciesQueryHandler`      | Anonymous   | `PagedList<GetCurrenciesResponse>`|
| GET    | `/commons/`        | `GetRegionsQueryHandler`         | Anonymous   | `PagedList<GetRegionsResponse>`   |
| GET    | `/commons/`        | `GetDistrictsQueryHandler`       | Anonymous   | `PagedList<GetDistrictsResponse>` |
| GET    | `/commons/`        | `GetLanguagesQueryHandler`       | Anonymous   | `PagedList<GetLanguagesResponse>` |

> [!NOTE]
> Each endpoint class sets `GroupName => AssemblyReference.Instance` (`"commons"`), so routes are prefixed with `/commons/`. The individual resource paths depend on the endpoint registration order and group nesting in `Maydon.Host`.

---

## CQRS Operations (Application Layer)

### Per Aggregate Operations

Each of the 4 translatable aggregates (Bank, Currency, Region, District) + Language supports:

| Operation     | Type      | Handler                          | Raises Event                     |
|---------------|-----------|----------------------------------|----------------------------------|
| **Create**    | Command   | `Create*CommandHandler`          | `CreateOrUpdate*DomainEvent`     |
| **Update**    | Command   | `Update*CommandHandler`          | `CreateOrUpdate*DomainEvent`     |
| **Remove**    | Command   | `Remove*CommandHandler`          | `Remove*DomainEvent`             |
| **Get**       | Query     | `Get*QueryHandler`               | —                                |
| **GetById**   | Query     | `GetById*QueryHandler`           | —                                |
| **MoveUp**    | Command   | `MoveUp*CommandHandler`          | —                                |
| **MoveDown**  | Command   | `MoveDown*CommandHandler`        | —                                |
| **SetOrder**  | Command   | `SetOrder*CommandHandler`        | —                                |

---

## Key Flows

### 1. Create Translatable Entity (Bank example)

```mermaid
sequenceDiagram
    participant Client
    participant Handler as CreateBankCommandHandler
    participant Bank as Bank Entity
    participant DomainEvent as CreateOrUpdateBankDomainEventHandler
    participant DB as CommonDbContext

    Client->>Handler: CreateBankCommand (mfo, tin, translates[])
    Handler->>Bank: new Bank(mfo, tin, ..., translates)
    Bank->>Bank: Raise(CreateOrUpdateBankDomainEvent)
    Handler->>DB: Banks.Add(bank)
    Handler->>DB: SaveChangesAsync()
    Note over DB: IPrePublishDomainEvent dispatched BEFORE save
    DB->>DomainEvent: Handle(CreateOrUpdateBankDomainEvent)
    DomainEvent->>DB: Find existing translates by BankId
    alt Translate exists for language
        DomainEvent->>DB: Update existing BankTranslate
    else New language
        DomainEvent->>DB: Add new BankTranslate
    end
    DB-->>Client: Result.Success (Bank.Id)
```

### 2. Remove Entity with Translate Cleanup

```mermaid
sequenceDiagram
    participant Client
    participant Handler as RemoveBankCommandHandler
    participant Bank as Bank Entity
    participant DomainEvent as RemoveBankDomainEventHandler
    participant DB as CommonDbContext

    Client->>Handler: RemoveBankCommand (Id)
    Handler->>DB: Banks.FindAsync(id)
    Handler->>Bank: bank.Remove()
    Bank->>Bank: Raise(RemoveBankDomainEvent)
    Handler->>DB: SaveChangesAsync()
    Note over DB: IPrePublishDomainEvent dispatched
    DB->>DomainEvent: Handle(RemoveBankDomainEvent)
    DomainEvent->>DB: BankTranslates.Where(t => t.BankId == id)
    DomainEvent->>DB: Soft-delete all translate records
    DB-->>Client: Result.Success
```

### 3. Reorder (MoveUp/MoveDown)

```mermaid
sequenceDiagram
    participant Client
    participant Handler as MoveUpCommandHandler
    participant DB as CommonDbContext

    Client->>Handler: MoveUpCommand (Id)
    Handler->>DB: Find entity by Id
    Handler->>DB: Find entity with Order = current - 1
    Handler->>Handler: Swap Order values
    Handler->>DB: SaveChangesAsync()
    DB-->>Client: Result.Success
```

### 4. Paginated Read (All Entities)

```mermaid
sequenceDiagram
    participant Client
    participant Host as Maydon.Host Endpoint
    participant Handler as GetBanksQueryHandler
    participant DB as CommonDbContext

    Client->>Host: GET /commons/ ?page=1&pageSize=10
    Host->>Handler: GetBanksQuery (page, pageSize)
    Handler->>DB: Banks.AsNoTracking()
    Note over DB: Global filters auto-applied:<br/>IsDeleted, Translate (language)
    DB-->>Handler: Filtered & paginated results
    Handler-->>Host: PagedList<GetBanksResponse>
    Host-->>Client: 200 OK (paginated JSON)
```

---

## Domain Event Side Effects

All domain events are `IPrePublishDomainEvent` — they are dispatched **before** `SaveChangesAsync` commits, ensuring translate records are part of the same transaction.

| Event                               | Handler                                   | Side Effect                            |
|-------------------------------------|-------------------------------------------|----------------------------------------|
| `CreateOrUpdateBankDomainEvent`     | `CreateOrUpdateBankDomainEventHandler`    | Upserts `BankTranslate` records        |
| `RemoveBankDomainEvent`             | `RemoveBankDomainEventHandler`            | Soft-deletes `BankTranslate` records   |
| `UpsertCurrencyDomainEvent`         | `UpsertCurrencyDomainEventHandler`        | Upserts `CurrencyTranslate` records    |
| `RemoveCurrencyDomainEvent`         | `RemoveCurrencyDomainEventHandler`        | Soft-deletes `CurrencyTranslate` records|
| `UpsertRegionDomainEvent`           | `UpsertRegionDomainEventHandler`          | Upserts `RegionTranslate` records      |
| `RemoveRegionDomainEvent`           | `RemoveRegionDomainEventHandler`          | Soft-deletes `RegionTranslate` records |
| `UpsertDistrictDomainEvent`         | `UpsertDistrictDomainEventHandler`        | Upserts `DistrictTranslate` records    |
| `RemoveDistrictDomainEvent`         | `RemoveDistrictDomainEventHandler`        | Soft-deletes `DistrictTranslate` records|
