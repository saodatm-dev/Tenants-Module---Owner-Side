# Document Module — Entity Relationship Diagram

## Overview

The Document module manages **contract lifecycle** — from template-based generation through digital signing via Didox. It operates in the `documents` PostgreSQL schema and is the 4th migration module in the system.

---

## Entity Relationship Diagram

```mermaid
erDiagram
    ContractTemplate ||--o{ Contract : "generates"
    Contract ||--o{ ContractFinancialItem : "has"
    Contract ||--o{ ContractSigningEvent : "tracks"
    Contract ||--o{ ContractAttachment : "stores"
    Contract ||--o{ ContractProviderState : "syncs with"
    Contract ||--o{ Contract : "parent → children"

    ContractTemplate {
        guid Id PK
        guid TenantId FK "nullable — null = system template"
        guid CreatedByUserId FK
        string Code "unique template code"
        string Name "JSON — multilingual"
        string Description "JSON — multilingual, nullable"
        string Page "JSONB — page settings"
        string Theme "JSONB — styling"
        string Header "JSONB — nullable"
        string Footer "JSONB — nullable"
        string Bodies "JSONB — per-language blocks"
        string ManualFields "JSONB — user-fillable fields"
        enum Scope "System | Tenant"
        enum Category "LeaseAgreement | Sublease | Commercial | Service | Custom"
        bool IsActive "default true"
        int CurrentVersion "auto-incremented"
        datetime CreatedAt
        datetime UpdatedAt
        bool IsDeleted "soft delete"
        datetime DeletedAt "nullable"
        guid DeletedBy "nullable"
    }

    Contract {
        guid Id PK "UUIDv7"
        guid TenantId FK "tenant/organization"
        string ContractNumber "unique per tenant, nullable"
        guid TemplateId FK
        string Language "max 5 chars"
        string Body "JSONB — resolved blocks"
        guid LeaseId FK "Building module"
        guid RealEstateId FK "Building module"
        guid OwnerCompanyId FK "Identity module"
        guid ClientCompanyId FK "nullable"
        string OwnerInn "max 14"
        string OwnerPinfl "max 14"
        string ClientInn "max 14"
        string ClientPinfl "max 14"
        money MonthlyAmount "numeric 18,2"
        dateonly LeaseStartDate
        dateonly LeaseEndDate "nullable"
        enum Status "13 states"
        datetime ContractDate "default now()"
        guid CreatedByUserId FK
        int CurrentVersion "default 1"
        string RejectionReason "max 2000"
        datetime SignatureDeadline "nullable"
        datetime ExportedAt "nullable"
        datetime OwnerSignedAt "nullable"
        datetime ClientSignedAt "nullable"
        guid ParentId FK "self-ref, nullable"
        bool IsDeleted "soft delete"
        datetime DeletedAt "nullable"
        guid DeletedBy "nullable"
    }

    ContractFinancialItem {
        guid Id PK "UUIDv7"
        guid ContractId FK "cascade delete"
        enum Type "Rent | Deposit | Utility | Maintenance | Parking | Custom"
        string Name "max 256"
        money Amount "numeric 18,2"
        enum Frequency "OneTime | Monthly | Quarterly | Annual"
        int SortOrder
    }

    ContractSigningEvent {
        guid Id PK "UUIDv7"
        guid ContractId FK "cascade delete"
        enum Party "Owner | Client"
        enum Action "Signed | Rejected | Revoked"
        datetime OccurredAt
        string ExternalSignatureId "max 256, nullable"
    }

    ContractAttachment {
        guid Id PK "UUIDv7"
        guid ContractId FK "cascade delete"
        string FileName "max 256"
        string ObjectKey "max 512 — S3 key"
        string ContentType "max 128"
        long FileSize
        enum DocumentType "AttachmentDocumentType"
        datetime UploadedAt
        guid UploadedByUserId FK
    }

    ContractProviderState {
        string ContractId PK "composite key"
        string ProviderName PK "max 50 — e.g. Didox"
        string ExternalId "max 256, nullable"
        enum SyncStatus "Pending | Sent | Processing | Signed | Rejected | Failed | Expired"
        datetime LastUpdated
        string ErrorMessage "max 500, nullable"
    }
```

---

## Enums Reference

### ContractStatus (13 states)

```mermaid
stateDiagram-v2
    [*] --> Draft
    Draft --> PendingSignature : Export to Didox
    Draft --> Sent : Direct send
    PendingSignature --> OwnerSigned : Owner signs
    PendingSignature --> RejectedByOwner : Owner rejects
    PendingSignature --> RejectedByClient : Client rejects
    PendingSignature --> ExpiredUnsigned : Deadline passes
    Sent --> OwnerSigned : Owner signs
    Sent --> Signed : Both sign
    OwnerSigned --> FullySigned : Client signs
    OwnerSigned --> RejectedByClient : Client rejects
    OwnerSigned --> ExpiredUnsigned : Deadline passes
    FullySigned --> Archived : Manual archive
    Draft --> Cancelled : User cancels
    PendingSignature --> Failed : System error
```

| Value | Int | Description |
|---|---|---|
| `Draft` | 0 | Initial state, editable |
| `PendingSignature` | 1 | Exported, awaiting signatures |
| `Sent` | 2 | Sent to external provider |
| `Signed` | 3 | Legacy — single-step signed |
| `Rejected` | 4 | Legacy — single-step rejected |
| `Archived` | 5 | Final archive state |
| `Failed` | 6 | System/export failure |
| `Cancelled` | 7 | User-cancelled |
| `OwnerSigned` | 8 | Owner signed, awaiting client |
| `FullySigned` | 9 | Both parties signed |
| `RejectedByOwner` | 10 | Owner rejected |
| `RejectedByClient` | 11 | Client rejected |
| `ExpiredUnsigned` | 12 | Deadline passed unsigned |

### Other Enums

| Enum | Values |
|---|---|
| `FinancialItemType` | Rent, Deposit, Utility, Maintenance, Parking, Custom |
| `FinancialFrequency` | OneTime, Monthly, Quarterly, Annual |
| `SigningParty` | Owner, Client |
| `SigningAction` | Signed, Rejected, Revoked |
| `ContractTemplateScope` | System (0), Tenant (1) |
| `ContractTemplateCategory` | LeaseAgreement, Sublease, Commercial, Service, Custom(99) |
| `ExternalSyncStatus` | Pending, Sent, Processing, Signed, Rejected, Failed, Expired |
| `DidoxDocumentStatus` | Draft, AwaitingPartnerSignature, AwaitingYourSignature, Signed, SignatureDeclined, Deleted, +11 more |
| `AttachmentDocumentType` | *(defined in Contract project)* |

---

## Database Details

| Property | Value |
|---|---|
| **Schema** | `documents` |
| **Naming Convention** | `snake_case` (EF  Npgsql convention) |
| **Money Columns** | `numeric(18,2)` via `MoneyToDecimalConverter` |
| **Status Storage** | String conversion (not integer) |
| **Soft Delete** | Global query filter on `IsDeleted` |
| **Migration Order** | 4 |

### Indexes

| Table | Index | Type |
|---|---|---|
| `contracts` | `tenant_id` | Regular |
| `contracts` | `lease_id` | Regular |
| `contracts` | `template_id` | Regular |
| `contracts` | `status` | Regular |
| `contracts` | `parent_id` | Regular |
| `contracts` | `created_by_user_id` | Regular |
| `contracts` | `(tenant_id, contract_number)` | Unique partial (`WHERE contract_number IS NOT NULL`) |

### Cross-Module Foreign Keys (Logical)

| Contract Column | Target Module | Target Entity |
|---|---|---|
| `LeaseId` | Building | `Lease` |
| `RealEstateId` | Building | `RealEstate` |
| `OwnerCompanyId` | Identity | `Company` |
| `ClientCompanyId` | Identity | `Company` |
| `CreatedByUserId` | Identity | `User` |
| `TenantId` | Identity | `Tenant/Organization` |
