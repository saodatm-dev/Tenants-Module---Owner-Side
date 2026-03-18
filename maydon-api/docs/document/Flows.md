# Document Module — Flows & Usage

## Actors

```mermaid
graph LR
    subgraph Actors
        OWN["🏢 Owner/Landlord<br/>Creates contracts, signs"]
        CLI["👤 Client/Tenant<br/>Reviews, signs or rejects"]
        ADM["🔧 Admin<br/>Manages templates, syncs"]
        SYS["⚙️ System<br/>Background jobs, events"]
        DIDOX["🌐 Didox<br/>External e-signing provider"]
    end
```

| Actor | Description | Key Actions |
|---|---|---|
| **Owner** | Property owner/landlord user | Create contracts, edit body, export to Didox, sign, upload attachments |
| **Client** | Tenant/lessee user | Review contract, sign or reject |
| **Admin** | System administrator | Manage templates, sync from Didox, view all contracts |
| **System** | Background processes | Expire unsigned contracts, process integration events |
| **Didox** | External e-signing API | Receive documents, track signatures, send status callbacks |

---

## Flow 1: Contract Creation (Happy Path)

```mermaid
sequenceDiagram
    actor Owner
    participant UI as Frontend
    participant API as ContractEndpoint
    participant PF as PrefillHandler
    participant PR as PlaceholderResolver
    participant BG as BuildingGateway
    participant OR as OwnerResolver
    participant CH as CreateHandler
    participant DB as DocumentDB
    participant NUM as NumberGenerator

    Owner->>UI: Select template + lease
    UI->>API: GET /prefill?templateId&leaseId&lang
    API->>PF: PrefillContractQuery

    PF->>PR: ResolveAllAsync(leaseId, tenantId)
    PR->>BG: GetLeaseInfoAsync()
    BG-->>PR: LeaseInfo + LeaseItems
    PR->>BG: GetRealEstateInfoAsync()
    BG-->>PR: RealEstateInfo
    PR->>OR: ResolveOwnerValuesAsync()
    OR-->>PR: Company data
    PR-->>PF: Placeholder values

    PF->>PF: Resolve placeholders in template body
    PF-->>API: PrefillContractResponse
    API-->>UI: Resolved body + manual fields
    
    Owner->>UI: Review, edit body, fill manual fields
    UI->>API: POST /contracts {templateId, leaseId, lang, body}
    API->>CH: CreateContractCommand

    CH->>NUM: GenerateAsync()
    NUM-->>CH: Contract number
    CH->>CH: Contract.Create(...)
    CH->>CH: AddFinancialItems (per LeaseItem)
    CH->>DB: SaveChangesAsync()
    CH->>CH: RaiseCreatedEvent()
    CH-->>API: ContractResponse
    API-->>UI: 201 Created
```

---

## Flow 2: Didox Export & Signing

```mermaid
sequenceDiagram
    actor Owner
    participant API as ContractEndpoint
    participant EXP as ExportHandler
    participant DB as DocumentDB
    participant EH as ContractExportHandler
    participant RE as RenderingEngine
    participant GOT as Gotenberg
    participant DIDOX as Didox API
    participant HUB as SignalR Hub
    participant SYN as SyncHandler

    Owner->>API: POST /{id}/export-didox
    API->>EXP: ExportContractToDidoxCommand
    EXP->>DB: Load contract
    EXP->>EXP: contract.ExportToDidox()
    Note over EXP: Status → Sent<br/>Sets SignatureDeadline
    EXP->>DB: SaveChangesAsync()
    EXP->>EXP: Publish InitiateDocumentExport
    EXP-->>API: 202 Accepted

    Note over EH: Integration event handler
    EH->>DB: Load contract + template
    EH->>RE: Render body → HTML
    RE->>GOT: HTML → PDF
    GOT-->>RE: PDF bytes
    EH->>EH: Publish DocumentExportRequested
    EH->>DB: AddOrUpdateProviderState("Didox", Pending)
    EH->>HUB: NotifyExportProgressAsync()

    Note over DIDOX: External processing
    DIDOX-->>EH: Webhook / poll response

    Owner->>API: POST /{id}/sync-from-didox
    API->>SYN: SyncContractFromDidoxCommand
    SYN->>DIDOX: GetDocumentStatusAsync()
    DIDOX-->>SYN: DocumentProviderStatus

    alt Owner Signed
        SYN->>DB: contract.RecordOwnerSigned()
        SYN->>HUB: NotifyStatusChangeAsync()
    else Client Signed (FullySigned)
        SYN->>DB: contract.RecordClientSigned()
        SYN->>HUB: NotifyStatusChangeAsync()
    else Rejected
        SYN->>DB: contract.RecordRejected()
        SYN->>HUB: NotifyStatusChangeAsync()
    end

    SYN->>DB: SaveChangesAsync()
```

---

## Flow 3: Contract Rejection

```mermaid
sequenceDiagram
    actor Party as Owner / Client
    participant API as ContractEndpoint
    participant RH as RejectHandler
    participant DB as DocumentDB
    participant HUB as SignalR Hub

    Party->>API: POST /{id}/reject {party, reason}
    API->>RH: RejectContractCommand

    RH->>DB: Load contract
    RH->>RH: contract.RecordRejected(party, now, reason)
    Note over RH: Status → RejectedByOwner<br/>or RejectedByClient
    RH->>RH: Add SigningEvent(Rejected)
    RH->>DB: SaveChangesAsync()
    RH->>HUB: NotifyStatusChangeAsync()
    RH-->>API: 204 No Content
```

---

## Flow 4: Contract Expiration (Background Job)

```mermaid
sequenceDiagram
    participant JOB as ExpireContractsJob
    participant DB as DocumentDB
    participant DIDOX as Didox API

    loop Every 1 hour
        JOB->>DB: Query contracts WHERE<br/>(OwnerSigned OR PendingSignature)<br/>AND SignatureDeadline < now()
        
        alt Found overdue contracts
            loop Each overdue contract
                JOB->>JOB: contract.MarkExpired()
                Note over JOB: Status → ExpiredUnsigned
                JOB->>JOB: UpdateProviderState("Didox", Expired)
            end
            JOB->>DB: SaveChangesAsync()
        end
    end
```

---

## Flow 5: Template Management

```mermaid
sequenceDiagram
    actor Admin
    participant API as ContractTemplateEndpoint
    participant TH as TemplateHandlers
    participant DB as DocumentDB
    participant RE as RenderingEngine
    participant GOT as Gotenberg

    Admin->>API: POST / {code, name, page, theme, bodies}
    API->>TH: CreateContractTemplateCommand
    TH->>DB: Save ContractTemplate
    TH-->>API: 201 Created

    Admin->>API: PUT /{id}/bodies/{lang} {blocks}
    API->>TH: UpdateContractTemplateBodyCommand
    TH->>DB: Update Bodies JSONB
    TH-->>API: 204 No Content

    Admin->>API: POST /{id}/preview {lang, manualValues}
    API->>RE: PreviewContractTemplatePdfQuery
    RE->>RE: Resolve placeholders with sample data
    RE->>RE: Render blocks → HTML
    RE->>GOT: HTML → PDF
    GOT-->>RE: PDF bytes
    RE-->>API: File(pdf)

    Admin->>API: GET /placeholders
    API->>TH: GetPlaceholderCatalogQuery
    TH-->>API: PlaceholderCatalogResponse
```

---

## Flow 6: PDF Generation

```mermaid
sequenceDiagram
    actor User
    participant API as ContractEndpoint
    participant QH as GeneratePdfHandler
    participant DB as DocumentDB
    participant RE as RenderingEngine
    participant GOT as Gotenberg

    User->>API: GET /{id}/pdf
    API->>QH: GenerateContractPdfQuery
    QH->>DB: Load Contract + Template
    QH->>RE: Render contract body with template styling
    RE->>RE: BlockRendererFactory routes each block
    RE->>RE: HtmlPageComposer assembles full page
    RE->>GOT: POST HTML → PDF
    GOT-->>RE: PDF bytes
    RE-->>API: File(application/pdf)
    API-->>User: contract-{id}.pdf
```

---

## Actor-Action Matrix

| Action | Owner | Client | Admin | System |
|---|:---:|:---:|:---:|:---:|
| **Create contract** | ✅ | ❌ | ✅ | ❌ |
| **Edit contract body** | ✅ (Draft) | ❌ | ✅ (Draft) | ❌ |
| **Regenerate contract** | ✅ (Draft) | ❌ | ✅ (Draft) | ❌ |
| **Export to Didox** | ✅ | ❌ | ✅ | ❌ |
| **Sign contract** | ✅ (via Didox) | ✅ (via Didox) | ❌ | ❌ |
| **Reject contract** | ✅ | ✅ | ❌ | ❌ |
| **Sync from Didox** | ❌ | ❌ | ✅ | ❌ |
| **Upload attachment** | ✅ | ❌ | ✅ | ❌ |
| **Download PDF** | ✅ | ✅ | ✅ | ❌ |
| **List contracts** | ✅ (own) | ✅ (own) | ✅ (all) | ❌ |
| **Create template** | ❌ | ❌ | ✅ | ❌ |
| **Update template** | ❌ | ❌ | ✅ | ❌ |
| **Delete template** | ❌ | ❌ | ✅ | ❌ |
| **Preview template PDF** | ❌ | ❌ | ✅ | ❌ |
| **Expire contracts** | ❌ | ❌ | ❌ | ✅ (hourly) |
| **Send SignalR notifications** | ❌ | ❌ | ❌ | ✅ |

---

## API Endpoints Reference

### Contracts (`/api/v1/documents/contracts`)

| Method | Path | Name | Description | Response |
|---|---|---|---|---|
| `GET` | `/{id}` | GetContractById | Get contract details | `ContractResponse` |
| `POST` | `/` | CreateContract | Create draft contract | `201 ContractResponse` |
| `PUT` | `/{id}/body` | UpdateContractBody | Edit JSONB body (Draft only) | `204` |
| `PUT` | `/{id}/regenerate` | RegenerateContract | Regenerate + bump version | `204` |
| `POST` | `/{id}/export-didox` | ExportContractToDidox | Export for signing | `202` |
| `POST` | `/{id}/reject` | RejectContract | Reject by party | `204` |
| `POST` | `/{id}/sync-from-didox` | SyncContractFromDidox | Poll Didox status | `204` |
| `GET` | `/{id}/pdf` | GetContractPdf | Download PDF | `application/pdf` |
| `POST` | `/{id}/attachments` | UploadContractAttachment | Upload file (≤10MB) | `201` |
| `GET` | `/{id}/attachments` | ListContractAttachments | List attachment metadata | `ContractAttachmentResponse[]` |
| `GET` | `/` | ListContracts | Paginated list with filters | `PagedContractResponse` |
| `GET` | `/prefill` | PrefillContract | Pre-fill template with data | `PrefillContractResponse` |

### Contract Templates (`/api/v1/documents/contract-templates`)

| Method | Path | Name | Description | Response |
|---|---|---|---|---|
| `GET` | `/` | GetContractTemplates | Paginated + filtered list | `PagedList<ListResponse>` |
| `GET` | `/{id}` | GetContractTemplateById | Full template with bodies | `ContractTemplateResponse` |
| `POST` | `/` | CreateContractTemplate | Create new template | `201 ContractTemplateResponse` |
| `PUT` | `/{id}` | UpdateContractTemplate | Update metadata + bump version | `204` |
| `PUT` | `/{id}/bodies/{lang}` | UpdateContractTemplateBody | Update single language body | `204` |
| `DELETE` | `/{id}` | DeleteContractTemplate | Soft-delete | `204` |
| `POST` | `/{id}/preview` | PreviewContractTemplatePdf | Generate PDF preview | `application/pdf` |
| `GET` | `/placeholders` | GetPlaceholderCatalog | Available placeholder catalog | `PlaceholderCatalogResponse` |

---

## Domain Events

| Event | When | Payload |
|---|---|---|
| `ContractCreatedDomainEvent` | Contract created | ContractId, TenantId, TemplateId, LeaseId |
| `ContractBodyUpdatedDomainEvent` | Body edited | ContractId |
| `ContractRegeneratedDomainEvent` | Body regenerated | ContractId, NewVersion |
| `ContractStatusChangedDomainEvent` | Status transition | ContractId, OldStatus, NewStatus |
| `ContractExportedToDidoxDomainEvent` | Exported to Didox | ContractId |
| `ContractExpiredDomainEvent` | Deadline passed | ContractId |
| `ContractOwnerSignedDomainEvent` | Owner signed | ContractId, SignedAt |
| `ContractClientSignedDomainEvent` | Client signed | ContractId, SignedAt |
| `ContractRejectedDomainEvent` | Party rejected | ContractId, Party, Reason |

## Integration Events

| Event | Direction | Purpose |
|---|---|---|
| `InitiateDocumentExport` | Internal → Export handler | Triggers PDF generation + Didox upload |
| `DocumentExportRequested` | Export handler → Didox worker | Carries PDF payload for external send |
| `ProviderStatusChanged` | Didox → Internal | Provider status update (sign/reject/fail) |
