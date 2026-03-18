# Didox Module — Flows & Usage

## Actors

```mermaid
graph LR
    subgraph Actors
        OWN["🏢 Owner/Landlord<br/>Registers Didox account, exports contracts"]
        ADM["🔧 Admin<br/>Manages Didox accounts, retrieves documents"]
        SYS["⚙️ System<br/>Handles export events, token refresh"]
        DIDOX["🌐 Didox API<br/>External e-signing platform"]
    end
```

| Actor | Description | Key Actions |
|---|---|---|
| **Owner** | Property owner/landlord | Register in Didox, export contracts, sign/reject documents |
| **Admin** | System administrator | CRUD Didox accounts, retrieve documents in HTML/PDF/JSON |
| **System** | Background processes | Handle export events, refresh tokens, publish status changes |
| **Didox API** | External platform (didox.uz) | Accept documents, manage signatures, return document status |

---

## Flow 1: Didox Account Registration

```mermaid
sequenceDiagram
    actor Owner
    participant API as DidoxAuthEndpoint
    participant AUTH as DidoxAuthService
    participant UCLI as UserLookupService
    participant CLI as DidoxClient
    participant DB as DidoxDB
    participant ENC as StringEncryptor

    Owner->>API: POST /didox-auth/register {password, ...}
    API->>AUTH: RegisterAsync(request)

    AUTH->>AUTH: Check IsAuthorized
    AUTH->>UCLI: GetByIdAsync(ownerId)
    UCLI-->>AUTH: User data (TIN, PINFL, name)

    AUTH->>CLI: RegisterUserAsync(request)
    CLI->>CLI: POST to Didox API
    CLI-->>AUTH: Registration response

    alt Success
        AUTH->>ENC: Encrypt(password)
        ENC-->>AUTH: Encrypted password
        AUTH->>AUTH: Create DidoxAccount entity
        AUTH->>DB: Accounts.AddAsync()
        AUTH->>DB: SaveChangesAsync()
        AUTH-->>API: DidoxAccountResponse
        API-->>Owner: 201 Created
    else Failure
        AUTH-->>API: Error result
        API-->>Owner: 400 Bad Request
    end
```

---

## Flow 2: Token Acquisition & Caching

```mermaid
sequenceDiagram
    participant CALLER as Any Service
    participant AUTH as DidoxAuthService
    participant CACHE as FusionCache
    participant DB as DidoxDB
    participant CLI as DidoxClient
    participant ENC as StringEncryptor

    CALLER->>AUTH: GetActiveTokenAsync(ownerId)
    AUTH->>CACHE: GetOrSetAsync("didox_token_{ownerId}")

    alt Cache Hit (< 5 min old)
        CACHE-->>AUTH: Cached token
    else Cache Miss
        AUTH->>DB: Query DidoxToken WHERE OwnerId
        alt Token exists and not expired
            DB-->>AUTH: Existing token
        else Token expired or missing
            AUTH->>DB: Remove expired token
            AUTH->>DB: Query DidoxAccount
            AUTH->>ENC: Decrypt(account.Password)
            AUTH->>CLI: LoginWithPasswordAsync(login, password)
            CLI-->>AUTH: TokenResponse
            AUTH->>AUTH: Create DidoxToken entity
            AUTH->>DB: Tokens.Add(newToken)
            AUTH->>DB: SaveChangesAsync()
        end
        AUTH-->>CACHE: Store token (5 min TTL)
    end

    CACHE-->>AUTH: Active token
    AUTH-->>CALLER: Result<string>
```

**Caching Strategy:**

| Parameter | Value |
|---|---|
| Cache key | `didox_token_{ownerId}` |
| Duration | 5 minutes |
| Factory timeout | 10 seconds |
| Fail-safe | Enabled (serves stale on refresh failure) |
| Token TTL in DB | 5 hours (Unix timestamp) |

---

## Flow 3: Document Export to Didox

```mermaid
sequenceDiagram
    participant DOC as Document Module
    participant PUB as IntegrationEventPublisher
    participant WRK as DidoxExportWorker
    participant AUTH as DidoxAuthService
    participant CLI as DidoxClient
    participant HUB as SignalR Hub
    participant DIDOX as Didox API

    DOC->>PUB: Publish DocumentExportRequested

    Note over WRK: Idempotent handler<br/>prevents duplicates
    PUB->>WRK: HandleIdempotentAsync(event)
    WRK->>WRK: SetUser(event.InitiatedBy)
    WRK->>WRK: Validate TargetProvider == "Didox"

    WRK->>HUB: NotifyExportProgressAsync(10%)
    WRK->>AUTH: GetActiveTokenAsync(ownerId)
    AUTH-->>WRK: Token

    WRK->>HUB: NotifyExportProgressAsync(30%)
    WRK->>WRK: Validate PDF content exists

    WRK->>WRK: Build DocumentUploadRequest
    Note over WRK: Maps owner/signer TIN/PINFL<br/>Sets document metadata<br/>Embeds base64 PDF

    WRK->>HUB: NotifyExportProgressAsync(60%)
    WRK->>CLI: CreateCustomDocumentAsync(request, token)
    CLI->>DIDOX: POST custom document
    DIDOX-->>CLI: Response

    alt Success
        CLI-->>WRK: Response with ExternalId
        WRK->>PUB: Publish ProviderStatusChanged(Sent)
        WRK->>HUB: NotifyExportCompletedAsync(success)
    else Failure
        CLI-->>WRK: Error response
        WRK->>PUB: Publish ProviderStatusChanged(Failed)
        WRK->>HUB: NotifyExportFailedAsync(error)
    end

    WRK->>WRK: ClearUser()
```

---

## Flow 4: Document Signing via Didox

```mermaid
sequenceDiagram
    participant CALLER as Document Module
    participant PROV as DidoxDocumentProviderService
    participant AUTH as DidoxAuthService
    participant CLI as DidoxClient
    participant DIDOX as Didox API

    CALLER->>PROV: SignDocumentAsync(docId, externalId, signatureData, userId)
    PROV->>PROV: Validate providerName == "Didox"
    PROV->>AUTH: GetActiveTokenAsync(userId)
    AUTH-->>PROV: Token

    PROV->>CLI: SignDocumentAsync(signatureData, externalId, token)
    CLI->>DIDOX: POST sign document
    DIDOX-->>CLI: Response

    alt Success
        CLI-->>PROV: Success
        PROV-->>CALLER: Result.Success()
    else Failure
        CLI-->>PROV: Error
        PROV-->>CALLER: Result.Failure(error)
    end
```

---

## Flow 5: Document Status Polling

```mermaid
sequenceDiagram
    participant CALLER as Document Module
    participant PROV as DidoxDocumentProviderService
    participant AUTH as DidoxAuthService
    participant CLI as DidoxClient
    participant DIDOX as Didox API

    CALLER->>PROV: GetDocumentStatusAsync(externalId, ownerUserId)
    PROV->>AUTH: GetActiveTokenAsync(ownerUserId)
    AUTH-->>PROV: Token

    PROV->>CLI: GetDidoxJsonDoc(externalId, token)
    CLI->>DIDOX: GET document JSON
    DIDOX-->>CLI: JSON response

    PROV->>PROV: Parse JSON status field
    PROV->>PROV: Determine isSigned / isRejected
    PROV->>PROV: Extract rejectionReason / signedAt

    PROV-->>CALLER: DocumentProviderStatus
    Note over CALLER: Caller updates contract<br/>status based on result
```

**Status Parsing Logic:**

| JSON `status` field | `IsSigned` | `IsRejected` |
|---|---|---|
| Contains "signed" | `true` | `false` |
| Contains "rejected" or "cancelled" | `false` | `true` |
| Other | `false` | `false` |

---

## Flow 6: Account CRUD Operations

```mermaid
sequenceDiagram
    actor Admin
    participant API as DidoxAccountEndpoint
    participant H as CQRS Handlers
    participant DB as DidoxDB
    participant ENC as StringEncryptor

    Admin->>API: GET /didox-accounts
    API->>H: GetDidoxAccountsQuery
    H->>DB: Query with pagination
    H-->>API: PagedList<DidoxAccountResponse>

    Admin->>API: POST /didox-accounts {login, password, tin, pinfl}
    API->>H: CreateDidoxAccountCommand
    H->>H: Check IsAuthorized
    H->>ENC: Encrypt(password)
    H->>DB: Save DidoxAccount
    H-->>API: 201 DidoxAccountResponse

    Admin->>API: PUT /didox-accounts/{id} {login?, password?}
    API->>H: UpdateDidoxAccountCommand
    H->>DB: Update DidoxAccount
    H-->>API: 204 No Content

    Admin->>API: DELETE /didox-accounts/{id}
    API->>H: DeleteDidoxAccountCommand
    H->>DB: Soft delete DidoxAccount
    H-->>API: 204 No Content
```

---

## API Endpoints Reference

### Didox Accounts (`/api/v1/didox-accounts`)

| Method | Path | Name | Description | Response |
|---|---|---|---|---|
| `GET` | `/` | GetDidoxAccounts | Paginated list of all Didox accounts | `PagedList<DidoxAccountResponse>` |
| `GET` | `/{id}` | GetDidoxAccountById | Get a specific Didox account | `DidoxAccountResponse` |
| `POST` | `/` | CreateDidoxAccount | Create a new Didox account | `201 DidoxAccountResponse` |
| `PUT` | `/{id}` | UpdateDidoxAccount | Update account credentials | `204` |
| `DELETE` | `/{id}` | DeleteDidoxAccount | Soft-delete a Didox account | `204` |

### Didox Auth (`/api/v1/didox-auth`)

| Method | Path | Name | Description | Response |
|---|---|---|---|---|
| `POST` | `/register` | DidoxRegistration | Register user in Didox + create local account | `201 DidoxAccountResponse` |

### Didox Documents (`/api/v1/didox-documents`)

| Method | Path | Name | Description | Response |
|---|---|---|---|---|
| `GET` | `/{id}/html` | GetDocumentHtml | Retrieve document as HTML | `text/html` |
| `GET` | `/{id}/pdf` | GetDocumentPdf | Retrieve document as PDF | `application/pdf` |
| `GET` | `/{id}/json` | GetDocumentJson | Retrieve raw document JSON | `application/json` |

---

## Integration Events

| Event | Direction | Handler | Purpose |
|---|---|---|---|
| `DocumentExportRequested` | Document → Didox | `DidoxExportWorker` | Upload contract PDF to Didox for signing |
| `ProviderStatusChanged` | Didox → Document | Document handlers | Report upload success/failure, status changes |

---

## Security Considerations

| Aspect | Implementation |
|---|---|
| **Password storage** | Encrypted via `IStringEncryptor` (not hashed — needs decryption for API calls) |
| **Token caching** | In-memory FusionCache, scoped per owner, 5-minute TTL |
| **Auth checks** | Manual `IsAuthorized` checks in service layer (not endpoint-level permissions) |
| **Idempotency** | `DidoxExportWorker` extends `IdempotentIntegrationEventHandlerBase` to prevent duplicate exports |
| **Background safety** | `DidoxAuthService` creates new DI scopes via `IServiceScopeFactory` for background operations |
