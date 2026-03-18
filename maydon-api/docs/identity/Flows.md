# Identity Module — Flows

## Overview

This document describes the key operational flows of the Identity module, covering **registration, login, session management, account switching, invitation lifecycle, OTP flow, and binding**. Each flow maps to concrete API endpoints and application handlers.

---

## 1. Phone Number Registration Flow

```mermaid
sequenceDiagram
    actor C as Client
    participant R as POST /registration/check-phone-number
    participant S as POST /registration/phone-number-confirm
    participant P as POST /registration/create-password
    participant OTP as POST /otps
    participant BRK as Broker API
    participant DB as IdentityDbContext

    C->>R: {phoneNumber}
    R->>DB: Check if phone exists (UserState)
    R-->>C: 200 OK / 400 already exists

    C->>OTP: {phoneNumber, otpType: Registration}
    OTP->>DB: Create/Resend Otp entity
    OTP->>BRK: HTTP POST /broker-api/send (SMS)
    OTP-->>C: 200 OK

    C->>S: {phoneNumber, code}
    S->>DB: Verify Otp.Try(code)
    alt Code valid
        S->>DB: Create UserState (expiry)
        S-->>C: 200 OK (state key)
    else Code invalid
        S-->>C: 400 Bad Request
    end

    C->>P: {phoneNumber, password, stateKey}
    P->>DB: Validate UserState
    P->>DB: Hash password (HMAC-SHA512)
    P->>DB: Create User (RegisterType.PhoneNumber)
    P->>DB: Create Company (individual)
    P->>DB: Create Account (Owner)
    P->>DB: Create CompanyUser
    P->>DB: Create Session (refresh token)
    P->>DB: Generate JWT
    P-->>C: AuthenticationResponse {token, expiry, refreshToken}
```

---

## 2. eImzo Registration / Login Flow

```mermaid
sequenceDiagram
    actor C as Client
    participant CH as GET /auth/challenge
    participant R as POST /registration/eimzo
    participant L as POST /login/eimzo
    participant EI as eImzo API
    participant DB as IdentityDbContext

    C->>CH: Request challenge
    CH->>EI: Get PKCS#7 challenge
    CH-->>C: ChallengeResponse

    alt New User
        C->>R: {pkcs7Signature, challenge}
        R->>EI: Verify signature, extract user data
        R->>DB: Create User (RegisterType.EImzo)
        R->>DB: Create Company + Account + CompanyUser
        R->>DB: Create IntegrationService (EImzo)
        R->>DB: Create Session → JWT
        R-->>C: AuthenticationResponse
    else Existing User
        C->>L: {pkcs7Signature, challenge}
        L->>EI: Verify signature
        L->>DB: Find user by SerialNumber
        L->>DB: UpdateIfEmpty (fill missing fields)
        L->>DB: Get/Create Account → Session → JWT
        L-->>C: AuthenticationResponse
    end
```

---

## 3. OneId Registration / Login Flow

```mermaid
sequenceDiagram
    actor C as Client
    participant R as POST /registration/oneid
    participant L as POST /login/oneid
    participant OI as OneId API
    participant DB as IdentityDbContext

    alt New User
        C->>R: {code (OAuth authorization code)}
        R->>OI: Exchange code → access token
        R->>OI: Get user info (PINFL, name, etc.)
        R->>DB: Create User (RegisterType.OneID)
        R->>DB: Create Company + Account + CompanyUser
        R->>DB: Create IntegrationService (OneID)
        R->>DB: Create Session → JWT
        R-->>C: AuthenticationResponse
    else Existing User
        C->>L: {code}
        L->>OI: Exchange code → user info
        L->>DB: Find user by Pinfl
        L->>DB: UpdateIfEmpty
        L->>DB: Get/Create Account → Session → JWT
        L-->>C: AuthenticationResponse
    end
```

---

## 4. Phone Number Login Flow

```mermaid
sequenceDiagram
    actor C as Client
    participant L as POST /login/phone-number
    participant DB as IdentityDbContext

    C->>L: {phoneNumber, password}
    L->>DB: Find User by PhoneNumber
    alt User found
        L->>L: PasswordHasher.Verify(password, hash, salt)
        alt Password valid
            L->>DB: Get default Account
            L->>DB: Upsert Session (by IP + UserAgent)
            L->>DB: Generate JWT (8 claims)
            L-->>C: AuthenticationResponse {token, refreshToken, expiry, accountType}
        else Password invalid
            L-->>C: 400 Bad Request
        end
    else User not found
        L-->>C: 400 Bad Request
    end
```

---

## 5. Token Refresh Flow

```mermaid
sequenceDiagram
    actor C as Client
    participant R as POST /auth/refresh-token
    participant DB as IdentityDbContext

    C->>R: {refreshToken} (AllowAnonymous)
    R->>DB: Find Session by RefreshToken (decrypted)
    alt Session found & not expired & not terminated
        R->>DB: Update Session (new refresh token + expiry)
        R->>DB: Generate new JWT
        R-->>C: AuthenticationResponse (new tokens)
    else Invalid / expired / terminated
        R-->>C: 400 Bad Request
    end
```

---

## 6. Logout Flow

```mermaid
sequenceDiagram
    actor C as Client
    participant L as POST /auth/logout
    participant DB as IdentityDbContext

    C->>L: {sessionId} (Authenticated)
    L->>DB: Find Session
    L->>DB: Session.Terminate()
    L->>L: Remove auth cookie
    L-->>C: 200 OK
```

---

## 7. Forgot Password Flow

```mermaid
sequenceDiagram
    actor C as Client
    participant F as POST /auth/forgot-password
    participant FC as POST /auth/forgot-password-confirm
    participant BRK as Broker API
    participant DB as IdentityDbContext

    C->>F: {phoneNumber} (AllowAnonymous)
    F->>DB: Find User by PhoneNumber
    F->>DB: Create/Resend Otp
    F->>BRK: HTTP POST /broker-api/send (SMS)
    F-->>C: 200 OK (state key)

    C->>FC: {phoneNumber, code, newPassword} (AllowAnonymous)
    FC->>DB: Verify OTP code
    FC->>DB: User.ChangePassword(hash, salt)
    FC->>DB: Create Session → JWT
    FC-->>C: AuthenticationResponse
```

---

## 8. Account Switching Flow

```mermaid
sequenceDiagram
    actor C as Client
    participant A as POST /accounts/change/{key}
    participant DB as IdentityDbContext

    C->>A: key = account identifier (Authenticated)
    A->>DB: Find Account by key for current user
    alt Account found & active
        A->>DB: Create new Session for target Account
        A->>DB: Generate JWT with target Account's claims
        A->>A: Set auth cookie
        A-->>C: AuthenticationResponse
    else Not found / inactive
        A-->>C: 400 Bad Request
    end
```

---

## 9. Invitation Lifecycle Flow

```mermaid
sequenceDiagram
    actor Owner as Company Owner
    actor Recip as Recipient User
    participant API as Invitation Endpoints
    participant DB as IdentityDbContext

    Owner->>API: POST / {senderId, recipientId/phone, roleId, content, expiry}
    API->>DB: Create Invitation (status=Sent)
    Note over DB: Raises CreateInvitationDomainEvent

    Recip->>API: GET / (filtered by recipientId via global query filter)
    API-->>Recip: List of invitations

    alt Accept
        Recip->>API: POST /accept/{id}
        API->>DB: Invitation.Accept()
        Note over DB: Raises AcceptInvitationDomainEvent
        Note over DB: Creates Account + CompanyUser for recipient
    else Reject
        Recip->>API: POST /reject {id, reason}
        API->>DB: Invitation.Reject(reason)
    else Cancel (by sender)
        Owner->>API: POST /cancel/{id}
        API->>DB: Invitation.Cancel()
    end
```

---

## 10. Account Creation Flow (Owner / Client)

```mermaid
sequenceDiagram
    actor C as Authenticated User
    participant A as POST /accounts/create-owner or create-client
    participant DB as IdentityDbContext

    C->>A: (Authenticated)
    A->>DB: Create new Company for user
    A->>DB: Create Account (type=Owner/Client)
    A->>DB: Create CompanyUser (isOwner=true/false)
    A->>DB: Assign system role (Owner/Client)
    A->>DB: Create Session → JWT
    A->>A: Set auth cookie
    A-->>C: AuthenticationResponse (switched context)
```

---

## 11. Binding Flow (eImzo / OneId)

```mermaid
sequenceDiagram
    actor C as Authenticated User
    participant B as POST /bindings/eimzo or /bindings/oneid
    participant EXT as External Service (eImzo / OneId)
    participant DB as IdentityDbContext

    C->>B: {signature/code} (HasPermission)
    B->>EXT: Verify identity
    B->>DB: Update User fields (Tin, Pinfl, SerialNumber, etc.)
    B->>DB: Create IntegrationService record
    B-->>C: true
```

---

## API Endpoint Catalog

### Authentication Group (`/auth`) — AllowAnonymous unless noted

| Method | Route | Handler | Auth |
|---|---|---|---|
| `GET` | `/challenge` | `GetEImzoChallengeQueryHandler` | Anonymous |
| `POST` | `/logout` | `LogoutCommandHandler` | **Authenticated** |
| `POST` | `/refresh-token` | `RefreshTokenCommandHandler` | Anonymous |
| `POST` | `/forgot-password` | `PhoneNumberForgotPasswordCommandHandler` | Anonymous |
| `POST` | `/forgot-password-confirm` | `PhoneNumberForgotPasswordConfirmCommandHandler` | Anonymous |
| `POST` | `/eimzo-mobile` | `EImzoMobileAuthCommandHandler` | Anonymous |

### Login Group (`/login`) — All AllowAnonymous

| Method | Route | Handler |
|---|---|---|
| `POST` | `/phone-number` | `PhoneNumberLoginCommandHandler` |
| `POST` | `/oneid` | `OneIdLoginCommandHandler` |
| `POST` | `/eimzo` | `EImzoLoginCommandHandler` |
| `POST` | `/eimzo-mobile` | `EImzoMobileLoginCommandHandler` |

### Registration Group (`/registration`) — All AllowAnonymous

| Method | Route | Handler |
|---|---|---|
| `POST` | `/check-phone-number` | `CheckPhoneNumberCommandHandler` |
| `POST` | `/phone-number-confirm` | `PhoneNumberRegistrationCommandHandler` |
| `POST` | `/create-password` | `PhoneNumberRegistrationConfirmCommandHandler` |
| `POST` | `/oneid` | `OneIdRegistrationCommandHandler` |
| `POST` | `/eimzo` | `EImzoRegistrationCommandHandler` |
| `POST` | `/eimzo-mobile` | `EImzoMobileRegistrationCommandHandler` |

### Accounts Group (`/accounts`) — Authenticated

| Method | Route | Handler | Permission |
|---|---|---|---|
| `GET` | `/my` | `GetMyAccountsQueryHandler` | — |
| `POST` | `/change/{key}` | `ChangeAccountCommandHandler` | — (commented out) |
| `POST` | `/create-owner` | `CreateOwnerAccountCommandHandler` | — (commented out) |
| `POST` | `/create-client` | `CreateClientAccountCommandHandler` | — (commented out) |
| `POST` | `/{userId}/deactivate` | `DeactivateAccountCommandHandler` | — |
| `POST` | `/{userId}/activate` | `ActivateAccountCommandHandler` | — |

### Users Group (`/users`) — Authenticated

| Method | Route | Handler | Permission |
|---|---|---|---|
| `GET` | `/` | `GetUsersQueryHandler` | `users:list` |
| `GET` | `/{id}` | `GetUserByIdQueryHandler` | `users:get-by-id` |
| `GET` | `/profile` | `ProfileQueryHandler` | — |
| `GET` | `/permissions` | `GetPermissionsQueryHandler` | — |
| `PUT` | `/profile` | `UpdateProfileCommandHandler` | — |

### Companies Group (`/companies`) — Authenticated

| Method | Route | Handler | Permission |
|---|---|---|---|
| `GET` | `/` | `GetCompaniesQueryHandler` | — |
| `GET` | `/{id}` | `GetCompanyByIdQueryHandler` | `companies:get-by-id` |
| `PUT` | `/logo` | `UpdateCompanyLogoCommandHandler` | `companies:logo` |
| `DELETE` | `/{id}` | `RemoveCompanyCommandHandler` | `companies:remove` |
| `GET` | `/users` | `GetCompanyUsersQueryHandler` | — |

### Invitations Group (`/invitations`) — Authenticated + RBAC

| Method | Route | Handler | Permission |
|---|---|---|---|
| `GET` | `/` | `GetInvitationsQueryHandler` | `invitations:list` |
| `GET` | `/{id}` | `GetInvitationByIdQueryHandler` | `invitations:get-by-id` |
| `POST` | `/` | `CreateInvitationCommandHandler` | `invitations:create` |
| `PUT` | `/` | `UpdateInvitationCommandHandler` | `invitations:update` |
| `DELETE` | `/{id}` | `RemoveInvitationCommandHandler` | `invitations:remove` |
| `POST` | `/accept/{id}` | `AcceptInvitationCommandHandler` | `invitations:update` |
| `POST` | `/cancel/{id}` | `CancelInvitationCommandHandler` | `invitations:update` |
| `POST` | `/reject` | `RejectInvitationCommandHandler` | `invitations:update` |

### OTPs Group (`/otps`)

| Method | Route | Handler | Auth |
|---|---|---|---|
| `POST` | `/` | `SendOtpCommandHandler` | Anonymous |

### Bindings Group (`/bindings`) — Authenticated + RBAC

| Method | Route | Handler | Permission |
|---|---|---|---|
| `POST` | `/oneid` | `OneIdBindingCommandHandler` | `bindings:one-id` |
| `POST` | `/eimzo` | `EImzoBindingCommandHandler` | `bindings:eimzo` |

---

## Session Upsert Strategy

The `BaseAuthenticationCommandHandler.UpsertSessionAsync` method implements a smart session reuse pattern:

1. **Check by SessionId** — if provided (e.g., during refresh), find the exact session
2. **Check by IP + UserAgent** — reuse existing session from same device
3. **Create new** — if no match found, create fresh session with new refresh token
4. **Update existing** — if match found, rotate refresh token and update expiry

This prevents session proliferation while still tracking unique devices.

---

## Domain Event Side Effects

| Event | Handler | Side Effect |
|---|---|---|
| `UpsertUserPostDomainEvent` | `UpsertUserPostDomainEventHandler` | Sync user to related modules |
| `DeleteUserPostDomainEvent` | `DeleteUserPostDomainEventHandler` | Cascade cleanup |
| `CreateOrUpdateCompanyPostDomainEvent` | Company event handler | Sync company data |
| `DeleteCompanyPostDomainEvent` | Company event handler | Cascade cleanup |
| `CreateAccountPreDomainEvent` | `CreateHostAccountPreDomainEventHandler` / `CreatetAccountPreDomainEventHandler` | Initialize account type-specific setup |
| `DefaultAccountPreDomainEvent` | `DefaultAccountPreDomainEventHandler` | Remove previous default flag |
| `DeleteAccountPreDomainEvent` | `DeleteAccountPreDomainEventHandler` | Terminate sessions, cleanup accounts |
| `AcceptInvitationDomainEvent` | Application handler | Create CompanyUser + Account for accepted invite |
| `UpsertCompanyUserPostDomainEvent` | CompanyUser event handler | Sync to related entities |
| `DeleteCompanyUserPostDomainEvent` | CompanyUser event handler | Cascade cleanup |
| `CreateOrUpdateRolePostDomainEvent` | Role event handler | Sync role permissions |
| `DeleteRolePostDomainEvent` | Role event handler | Cascade cleanup |
| `RemoveMainBankPropertyDomainEvent` | BankProperty event handler | Auto-promote next bank property to main |
