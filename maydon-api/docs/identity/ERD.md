# Identity Module — Entity Relationship Diagram

## Overview

The Identity module manages **user accounts, companies, roles, permissions, sessions, invitations, OTPs, and bank properties**. It operates in the `identity` PostgreSQL schema. All entities inherit from a shared `Entity` base class providing `Id`, `CreatedBy`, `UpdatedBy`, `CreatedAt`, `UpdatedAt`, `DeletedAt`, `IsDeleted`.

---

## Entity Relationship Diagram

```mermaid
erDiagram
    User ||--o{ Account : "has"
    User ||--o{ CompanyUser : "member of"
    User ||--o{ Company : "owns (OwnerId)"
    User ||--o{ Invitation : "receives (RecipientId)"

    Company ||--o{ Account : "tenant of"
    Company ||--o{ CompanyUser : "has members"
    Company ||--o{ BankProperty : "has bank details"
    Company ||--o{ Invitation : "sends (SenderId)"
    Company ||--o{ Role : "scoped roles (TenantId)"

    Account ||--o{ Session : "has sessions"
    Account }o--|| Role : "assigned role"

    Role ||--o{ RolePermission : "has permissions"
    Role ||--o{ Invitation : "offered role"

    Permission ||--o{ RolePermission : "granted via"

    User {
        Guid Id PK
        string FirstName "max 100"
        string LastName "max 100"
        string MiddleName "max 100"
        string PhoneNumber "encrypted, max 20"
        bytes Password "nullable"
        bytes Salt "nullable"
        string Tin "encrypted, max 15"
        string Pinfl "encrypted, max 14"
        string SerialNumber "encrypted, max 100"
        string PassportNumber "encrypted, max 20"
        DateOnly BirthDate "nullable"
        Guid RegionId FK "nullable"
        Guid DistrictId FK "nullable"
        string Address "max 500"
        string ObjectName "max 300 — avatar S3 key"
        bool IsVerified
        RegisterType RegisterType "PhoneNumber | EImzo | OneID"
        bool IsActive "default true"
    }

    Company {
        Guid Id PK
        Guid OwnerId FK "→ User.Id"
        string Name "required, max 200"
        string Tin "max 15"
        string SerialNumber "encrypted, max 100"
        bool IsVerified
        Guid RegionId FK "nullable"
        Guid DistrictId FK "nullable"
        string Address "max 500"
        RegisterType RegisterType
        string ObjectName "max 200 — logo S3 key"
        bool IsActive "default true"
    }

    Account {
        Guid Id PK
        Guid TenantId FK "→ Company.Id"
        Guid UserId FK "→ User.Id"
        Guid RoleId FK "→ Role.Id"
        AccountType Type "Client | Owner | Agent"
        bool IsDefault
        bool IsActive "default true"
    }

    Role {
        Guid Id PK
        Guid TenantId FK "→ Company.Id, nullable"
        string Name "encrypted, required, max 100"
        RoleType Type "System | Client | Owner"
        bool IsActive "default true"
    }

    RolePermission {
        Guid Id PK
        Guid RoleId FK "→ Role.Id"
        Guid PermissionId FK "→ Permission.Id (Core)"
    }

    CompanyUser {
        Guid Id PK
        Guid CompanyId FK "→ Company.Id"
        Guid UserId FK "→ User.Id"
        bool IsOwner
        bool IsActive "default true"
    }

    Session {
        Guid Id PK
        Guid AccountId FK "→ Account.Id"
        string RefreshToken "encrypted, required, max 500"
        DateTime RefreshTokenExpiryTime
        bool IsTerminated
        string DeviceInfo "encrypted, max 500"
        string IpAddress "encrypted, max 50"
    }

    Invitation {
        Guid Id PK
        Guid SenderId FK "→ Company.Id"
        Guid RecipientId FK "→ User.Id, nullable"
        Guid RoleId FK "→ Role.Id"
        string ReceipientPhoneNumber "max 20"
        string Content "required, max 500"
        string Key "max 100"
        DateTime ExpiredTime
        InvitationStatus Status "Sent | Received | Accepted | Canceled | Rejected"
        string Reason "max 500 — reject reason"
    }

    Otp {
        Guid Id PK
        string PhoneNumber "encrypted, required, max 20"
        string Code "encrypted, required, max 10"
        DateTime NextAvailableTime "nullable"
        ushort SentMessageCount "default 1"
        ushort Tries "default 1"
        ushort MaxTries "default 4"
        OtpStatus Status "Active | Received | Waiting | Block | NotApplied"
    }

    OtpContent {
        Guid Id PK
        OtpType OtpType "Registration | RestorePassword | InviteByPhoneNumber | InviteByUserId"
        Guid LanguageId FK
        string LanguageShortCode "required, max 10"
        string Content "encrypted, required, max 500"
    }

    BankProperty {
        Guid Id PK
        Guid TenantId FK "→ Company.Id"
        Guid BankId FK "→ Common.Bank"
        string BankName "required, max 200"
        string BankMFO "required, max 10"
        string AccountNumber "encrypted, required, max 30"
        bool IsMain
        bool IsPublic "default true"
    }

    UserState {
        Guid Id PK
        string PhoneNumber "encrypted, max 20"
        bool IsRegistration "default true"
        DateTime ExpiredTime
        bool IsActive "default true"
    }

    IntegrationService {
        Guid Id PK
        IntegrationServiceType Type "EImzo | OneID"
        string Value "encrypted, required, max 1000"
    }

    Permission {
        Guid Id PK
        string Instance "module name"
        string Group "permission group"
        string Name "encrypted"
        bool IsSystem
        bool IsActive
    }
```

---

## Enums Reference

| Enum | Values |
|---|---|
| `RegisterType` | `PhoneNumber = 0`, `EImzo`, `OneID` |
| `AccountType` | `Client = 0`, `Owner`, `Agent` |
| `RoleType` | `System = 0`, `Client`, `Owner` (flags) |
| `InvitationStatus` | `Sent = 0`, `Received`, `Accepted`, `Canceled`, `Rejected` |
| `OtpStatus` | `Active = 0`, `Received`, `Waiting`, `Block`, `NotApplied` |
| `OtpType` | `Registration = 1`, `RestorePassword`, `InviteByPhoneNumber`, `InviteByUserId` |
| `IntegrationServiceType` | `EImzo`, `OneID` |

---

## OTP State Machine

```mermaid
stateDiagram-v2
    [*] --> Active : Created
    Active --> Waiting : Sent (1 min cooldown)
    Waiting --> Active : Cooldown elapsed
    Active --> Received : Code verified
    Active --> Block : MaxTries reached (30 min)
    Waiting --> Block : MaxSendCount reached (30 min)
    Block --> Active : Block time elapsed + Reset
    Active --> NotApplied : Unused
```

**OTP Constants:**

| Parameter | Value |
|---|---|
| Send cooldown | 1 minute |
| Block duration | 30 minutes |
| Max tries per OTP | 4 |
| Max messages per OTP | 3 |

---

## Invitation State Machine

```mermaid
stateDiagram-v2
    [*] --> Sent : Created
    Sent --> Received : Recipient views
    Received --> Accepted : Recipient accepts
    Received --> Rejected : Recipient rejects (with reason)
    Sent --> Canceled : Sender cancels
```

---

## Database Details

| Property | Value |
|---|---|
| **Schema** | `identity` |
| **Naming Convention** | `snake_case` (EF Npgsql convention) |
| **Migration History Table** | `migration_history` in `identity` schema |
| **Encryption** | Column-level via `[EncryptColumn]` attribute (AES-256) |
| **Soft Delete** | Global query filter on `IsDeleted` (all entities) |
| **Active Filter** | Global query filter on `IsActive` (User, Role, Company) |
| **Base Class** | `Entity` (Id, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt, DeletedAt, IsDeleted) |

### Encrypted Fields Summary

| Entity | Encrypted Fields |
|---|---|
| **User** | `PhoneNumber`, `Tin`, `Pinfl`, `SerialNumber`, `PassportNumber` |
| **Role** | `Name` |
| **Session** | `RefreshToken`, `DeviceInfo`, `IpAddress` |
| **Otp** | `PhoneNumber`, `Code` |
| **OtpContent** | `Content` |
| **BankProperty** | `AccountNumber` |
| **UserState** | `PhoneNumber` |
| **IntegrationService** | `Value` |
| **Company** | `SerialNumber` |

### Cross-Module Foreign Keys (Logical)

| Column | Source Entity | Target Module | Target Entity |
|---|---|---|---|
| `RegionId` | User, Company | Common | `Region` |
| `DistrictId` | User, Company | Common | `District` |
| `BankId` | BankProperty | Common | `Bank` |
| `PermissionId` | RolePermission | Core | `Permission` |
| `LanguageId` | OtpContent | Common | `Language` |

---

## Domain Events

| Event | Entity | When | Type |
|---|---|---|---|
| `UpsertUserPostDomainEvent` | User | Created / Updated / Profile updated | Post |
| `UpsertUserPreDomainEvent` | User | Before create/update | Pre |
| `DeleteUserPostDomainEvent` | User | Soft deleted | Post |
| `DeleteUserPreDomainEvent` | User | Before delete | Pre |
| `CreateOrUpdateCompanyPostDomainEvent` | Company | Created / Updated / Activated / Deactivated | Post |
| `DeleteCompanyPostDomainEvent` | Company | Soft deleted | Post |
| `CreateOrUpdateRolePostDomainEvent` | Role | Created / Updated / Activated / Deactivated | Post |
| `DeleteRolePostDomainEvent` | Role | Soft deleted | Post |
| `UpsertCompanyUserPostDomainEvent` | CompanyUser | Created | Post |
| `UpsertCompanyUserPreDomainEvent` | CompanyUser | Before create | Pre |
| `DeleteCompanyUserPostDomainEvent` | CompanyUser | Removed | Post |
| `DeleteCompanyUserPreDomainEvent` | CompanyUser | Before remove | Pre |
| `CreateAccountPreDomainEvent` | Account | Before create | Pre |
| `DefaultAccountPreDomainEvent` | Account | Before default change | Pre |
| `DeleteAccountPreDomainEvent` | Account | Before delete | Pre |
| `CreateInvitationDomainEvent` | Invitation | Created | Post |
| `AcceptInvitationDomainEvent` | Invitation | Accepted | Post |
| `RemoveMainBankPropertyDomainEvent` | BankProperty | Main bank removed | Post |
