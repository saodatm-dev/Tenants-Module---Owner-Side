# Tenants Module Specification: Owner Side

## 1. Module Overview

### What is the Tenants Module?
The Tenants module is the property owner's central database for managing confirmed tenants. It shifts the focus from tenant acquisition to long-term relationship management. It stores all data for active and past renters, providing a complete rental history workspace.

### Who uses it?
* **Property Owners:** To manage their own property portfolios.
* **Property Managers:** To access tenant contacts, contract states, and operational notes for day-to-day management.

### What problems does it solve?
* **Data Accessibility:** Eliminates the need to search through contracts in the "Contracts" section to access important documents.
* **Occupancy Tracking:** Shows exactly who occupies which property, upcoming move-outs, and past tenancies at a glance.
* **Tenant History:** Builds a historical record of tenant behavior (payments, property care, compliance) to inform lease renewal decisions.
* **Operational Efficiency:** Consolidates property, contract, and payment data into a single, actionable view.

### Relationship with Other Modules
The Tenants module works closely with other system components but serves a distinct purpose from Applications.

*   **Applications:** Manages potential tenants. Once an application is approved and a contract is signed, the applicant converts into a Tenant. Applications module handles acquisition; Tenants module handles retention of renters.
*   **Listings:** The tenant's lifecycle directly triggers actions in the Listings module. For example, when an active tenant's status changes to "Moving Out", the system can automatically prepare a new listing advert for that specific property so you can find a replacement tenant without downtime.
*   **Contracts:** Contracts bind tenants to properties. Contract dates (Start, End) automatically define the tenant's lifecycle status (e.g., Active, Expiring Soon).
*   **Payments:** The module aggregates payment data to show the tenant's current balance, payment history, and financial debt.

---

## 2. Core Terminology

Defining these key concepts removes ambiguity about the scope of the module, specifically detailing who becomes a tenant, when they become a tenant, and their governing status.

*   **Applicant:** A prospective tenant whose application has been submitted but who has not yet signed a lease. Applicants are managed in the Applications module; once both parties sign the contract, the applicant converts into a Tenant and appears in this module.
*   **Tenant:** A person or entity that has signed a lease agreement. Before signing, they are an "Applicant"; the moment both parties sign the contract (digitally or physically), the applicant becomes a "Tenant." A single tenant can rent multiple properties at the same time — each rental is governed by its own contract, but all are tracked under one tenant record since it is the same person.
*   **Rental Relationship:** The full history between an owner and a tenant. It starts when the lease is signed and remains in the system as a permanent record, even after the tenant moves out.
*   **Active Tenant:** A tenant who currently lives in the property, has a scheduled move-in date, or is overstaying past their lease end date. Active tenants generate rental income and carry current payment and lease obligations.
*   **Former Tenant:** A tenant whose lease has ended *and* who has completed the move-out process. Their profile is kept for reference (e.g., payment track record) but they no longer have access to the property.
*   **Property / Unit:** A property is a physical asset (e.g., a building or apartment complex) that contains one or more units. A unit is the specific space a tenant rents. Multiple tenants can rent within the same property, but each tenant is linked to a specific unit. Each unit can only have one active tenant at a time.
*   **Lease / Contract:** The legal agreement that connects a tenant to a property. The contract's start and end dates define the timeline of the rental relationship.
*   **Tenant Status:** The current operational state of the tenant (e.g., "Active", "Moving Out", "Former"). 
    * *Note on Expirations & Overstays:* When a contract is nearing its End Date, the system automatically notifies both the tenant and the owner. If the End Date passes without a formal renewal, the tenant does **not** automatically become a "Former Tenant." They remain functionally in an "Active" state—though visually tagged in the UI as *Expired / Overstaying*—until the owner manually logs a completed move-out. This ensures billing automation continues and owners never lose tracking of holdover tenants.

---

## 3. Tenant Lifecycle

This section defines the exact stages a tenant goes through in the system — from first contact to archived record.

### Lifecycle Stages

```
Applicant → Contract Signed → Active Tenant → Lease Ending → Move-Out Completed → Former Tenant
```

| # | Stage | Trigger | What Happens |
|---|-------|---------|--------------|
| 1 | **Applicant** | Owner receives a rental application | The person exists only in the Applications module. They have no tenant record yet. |
| 2 | **Contract Signed** | Both parties sign the lease digitally or add already signed lease | The system automatically creates a Tenant record and links it to the assigned property/unit. The tenant appears in this module for the first time. |
| 3 | **Active Tenant** | Contract start date arrives (or immediately, if the start date is today or earlier) | The tenant is marked as "Active." Billing begins, the property is marked as occupied, and the tenant gains property access. |
| 4 | **Lease Ending** | Contract end date is approaching (system sends notifications) | Both the owner and tenant are notified. The owner can renew the contract, start a new one, or prepare for move-out. |
| 5 | **Overstaying** | Contract end date passes without renewal or move-out | The tenant stays "Active" but is visually flagged as *Expired / Overstaying* in the UI. Billing continues. The tenant is **not** automatically removed. |
| 6 | **Move-Out Completed** | Owner manually logs a completed move-out | The tenant loses property access. The property is freed up for a new tenant. |
| 7 | **Former Tenant** | Move-out is recorded | The tenant's profile is archived. All history (payments, notes, documents) is preserved for reference. The record is not deleted automatically, without owner's command. |

### Key Rules

* **A tenant cannot exist without a contract.** The contract signing is the only event that creates a tenant record. There is no way to manually add a tenant without a signed lease.
* **Tenant creation is automatic.** When a contract is signed in the system, the tenant record is generated automatically — the owner does not need to create it separately.
* **Tenants are not deleted automatically.** When a tenancy ends and move-out is completed, the tenant is moved to "Former Tenant" status. Their full history remains in the system for reference (payment reliability, lease compliance, etc.).
* **Move-out is always manual.** The system never automatically converts an active tenant to a former tenant. The owner must explicitly confirm that the tenant has vacated. This prevents errors where a holdover tenant is accidentally removed from billing.
* **One unit, one active tenant.** Each unit can only have one active tenant at a time. A property with multiple units can have multiple tenants, each linked to their own unit. One tenant can also rent multiple units (within the same or different properties) — each rental has its own contract, but they are all linked to the same tenant record.

---

## 4. Tenant Status Model

Every tenant in the system has a status that reflects their current state. Statuses change based on contract dates or owner actions

### Status Definitions

| Status | Meaning | When It Appears | Changed By | Final? |
|--------|---------|-----------------|------------|--------|
| **Pending Move-In** | The contract is signed, but the start date has not arrived yet. | Automatically, when a contract is signed with a future start date. | System (automatic action). Changes to "Active" when the contract start date arrives. | No |
| **Active** | The tenant currently occupies the property and the lease is in effect. | Automatically, when the contract start date arrives or immediately if the signed contract's start date is today or in the past. | System (automatic action). | No |
| **Lease Ending** | The contract end date is approaching. The owner needs to decide whether to renew or prepare for move-out. | Automatically, when the contract end date falls within the configured reminder period (e.g., 30 days away). | System (automatic action). The owner can renew the contract (returns to "Active") or let it expire. | No |
| **Overstaying** | The contract has expired, but the tenant has not moved out and the lease has not been renewed. | Automatically, when the contract end date passes without renewal or a confirmed move-out. | System (automatic action). The owner resolves this by either renewing the contract or logging a move-out. | No |
| **Former** | The tenant has vacated and the move-out has been confirmed by the owner. The record is archived. | When the owner manually logs a completed move-out. | Owner (manual action). | Yes |
| **Removed** | The tenant was forced out due to a lease violation, non-payment, or legal action. | When the owner manually records the removal. | Owner (manual action). | Yes |

### Status Transitions

```
Pending Move-In → Active → Lease Ending → Overstaying → Former
                                    ↓                        ↑
                              (Renewed → Active)         Removed
                                                     (from any active status)
```

### Key Notes

* **"Pending Move-In" and "Active"** are set by the system based on contract dates. The owner does not manually change these.
* **"Lease Ending"** is a warning state, not a separate action by the owner. It triggers notifications but does not change the tenant's access or billing.
* **"Overstaying"** keeps the tenant fully active (billing continues, access remains). It is a visual indicator that the contract needs attention, not a penalty state.
* **"Former"** and **"Removed"** are the only final statuses. Once set, the tenant is archived and cannot return to an active state without a new contract.
* **"Removed"** is separate from "Former" to distinguish voluntary departures from forced removals. This distinction matters for tenant history and future rental decisions.
* The owner can mark a tenant as removed at any point while they are in an active status (Active, Lease Ending, or Overstaying).

---

## 5. Tenant Profile (Cabinet)

When the owner selects a tenant, a detail panel (the "cabinet") opens, showing everything about that tenant in one place. The cabinet is organized into tabs so the owner can quickly navigate to the information they need.

### Tab Structure

| # | Tab | What It Shows |
|---|-----|---------------|
| 1 | **Overview** | Summary card: tenant name, phone, email, current status badge, linked property/unit, contract dates, and current balance (owed or overpaid). This is the default tab when the cabinet opens. |
| 2 | **Property / Unit** | The property or unit the tenant occupies. Shows address, unit number, floor, area (m²), and occupancy dates. If the tenant rents multiple units, each rental is listed separately with its own contract reference. |
| 3 | **Contract** | The active lease agreement: contract number, start and end dates, monthly rent amount, deposit amount, payment schedule, and renewal history. Includes a link to open the full contract in the Contracts module. Past contracts (if any) are shown below the active one in chronological order. |
| 4 | **Payments** | Payment history table: date, amount, method, and status (paid, overdue, partially paid). Shows current balance at the top. Includes a link to the Payments module for detailed financial records. |
| 5 | **Documents** | All documents tied to this tenant: signed contracts, invoices, and any files uploaded by the owner. Documents can be uploaded, downloaded, and previewed. |
| 6 | **Notes** | Free-form notes the owner or property manager can add about the tenant. Every note records the date and the person who wrote it. |
| 7 | **History** | A timeline of all recorded activity for this tenant: status changes, contract renewals, payment events, notes added, documents uploaded. Each entry shows the date, event type, and a brief description. This tab cannot be edited — it is a system-generated audit trail. |

### Key Rules

* The **Overview** tab always opens by default when the cabinet is selected.
* **Notes** are for internal use by the owner and their property managers.
* **History** is read-only. It is automatically generated by the system whenever a tracked event occurs (status change, payment received, document uploaded, etc.).
* If a tenant has **multiple active rentals** (multiple units), the Property/Unit and Contract tabs show each rental separately, clearly labeled.

---

## 6. Owner Actions

This section lists what the owner can do within the Tenants module. Actions are intentionally limited — the module is designed for monitoring and reference, not as a workflow tool.

### Available Actions

| Action | Where | Description |
|--------|-------|-------------|
| **View tenant profile** | Tenant list or card | Opens the tenant cabinet to view all details. |
| **Search tenants** | Top search bar | Search by tenant name, phone number, or property address. Results update as the owner types. |
| **Filter by status** | Filter bar | Filter the tenant list by one or more statuses (Active, Pending Move-In, Lease Ending, Overstaying, Former, Removed). |
| **Filter by property** | Filter bar | Filter tenants by a specific property or unit. |
| **Add a note** | Notes tab in cabinet | Write a free-form internal note about the tenant. |
| **Upload a document** | Documents tab in cabinet | Attach a file (PDF, image, scan) to the tenant's record. |
| **Contact tenant** | Overview tab in cabinet | Initiate a call or message using the tenant's stored phone number or email. |
| **Mark as removed** | Cabinet action menu | Manually change the tenant's status to "Removed." Requires confirmation. |
| **Log move-out** | Cabinet action menu | Manually record that the tenant has vacated. Changes status to "Former." Requires confirmation. |
| **Renew contract** | Contract tab in cabinet | Initiates a contract renewal flow (redirects to the Contracts module). On completion, the tenant's status returns to "Active." |

### What the Owner Cannot Do

* **Manually create a tenant.** Tenants are only created automatically when a contract is signed.
* **Delete a tenant record.** Records are permanent. Former and removed tenants are archived, not deleted.
* **Change a tenant's status to Active or Pending Move-In.** These statuses are controlled by the system based on contract dates.
* **Edit the History tab.** The event log is system-generated and cannot be modified.

---

## 7. Views & Navigation

The Tenants module offers two views for browsing tenants, plus a search and filtering system.

### Card Grid View (Default)

The primary view. Tenants are displayed as individual cards in a grid layout.

**Each card shows:**
* Tenant photo (or initials placeholder)
* Full name
* Current status badge (color-coded)
* Linked property and unit
* Monthly rent amount
* Contract end date

Cards are sorted by status priority by default: Active first, then Lease Ending, Overstaying, Pending Move-In, Former, and Removed last. The owner can click any card to open the cabinet.

### List / Table View

An alternative view for owners with many tenants. Displays the same information in a sortable table format.

**Table columns:**
* Tenant name
* Status
* Property / Unit
* Contract start date
* Contract end date
* Monthly rent
* Current balance

The table supports sorting by any column (click the column header). Clicking a row opens the cabinet.

### Search

A text search bar at the top of the page. Searches across:
* Tenant name
* Phone number
* Property address

Results filter the current view in real time as the owner types.

### Filters

Filters are available in both views and can be combined:

| Filter | Options |
|--------|---------|
| **Status** | Active, Pending Move-In, Lease Ending, Overstaying, Former, Removed (multi-select) |
| **Property** | Dropdown of the owner's properties (single-select) |

Active filters are shown as removable chips above the tenant list. The "Reset" button clears all filters.

---

## 8. Module Integrations

The Tenants module does not operate in isolation. It is connected to four other system modules. Data flows between them automatically — the owner does not need to manually synchronize anything.

| Module | Relationship | Data Flow |
|--------|-------------|-----------|
| **Applications** | Source of tenants. | When an application reaches the "Contract Signed" stage, the system automatically creates a tenant record in this module. The applicant's name, contact info, and linked property are carried over. |
| **Contracts** | Defines the rental relationship timeline. | Contract start/end dates drive tenant status changes (Pending Move-In → Active → Lease Ending → Overstaying). Renewal or signing a new contract resets the status cycle. The tenant cabinet links directly to the full contract record. |
| **Payments** | Financial tracking. | Payment data (transactions, balances, overdue amounts) is pulled from the Payments module and displayed in the tenant's Payments tab. The Tenants module reads this data but does not manage payments directly. |
| **Listings** | Vacancy management. | When an active tenant's status changes to "Former" or "Removed," the system can notify the owner to prepare a new listing for the vacated unit. This helps minimize downtime between tenants. |

### Integration Rules

* **No duplicate records.** If the same person signs a second contract (for a different unit), a new rental entry is created under the *same* tenant record rather than creating a second tenant.
* **Data stays in its home module.** The Tenants module displays contract and payment data using read-only references. Editing a contract is done in the Contracts module; managing payments is done in the Payments module.
* **Deleting a property does not delete the tenant.** If a property or unit is removed from the system, the tenant record remains with a note indicating the property is no longer active.

---

## 9. Restrictions & Edge Cases

This section documents boundary conditions, limitations, and how the system handles unusual situations.

### Multi-Unit Tenants

A single tenant can rent more than one unit (within the same or different properties). Each rental has its own contract and is tracked independently. The tenant cabinet shows all active and past rentals under one profile.

* **Status is per-rental, not per-tenant.** If a tenant has two units and one contract is expiring, only that rental shows "Lease Ending." The other rental remains "Active."
* **The tenant list shows one row per tenant**, not per rental. The card or row displays the primary (most recent) rental. All rentals are visible inside the cabinet.

### Early Departure

If a tenant leaves before the contract end date, the owner manually logs the move-out. The status changes to "Former" regardless of the remaining contract duration. The contract itself is handled in the Contracts module (termination, penalties, etc.).

### Overstaying Tenants

When the lease expires and nothing happens (no renewal, no move-out), the tenant status changes to "Overstaying." This is intentionally not a blocker — the tenant remains active, billing continues, and no automatic actions are taken. This is designed for the common real-world scenario where lease renewals are delayed but the tenant stays.

### Property Deletion

If a property or unit is deleted from the system:
* Active tenants linked to that property are **not** automatically removed or archived.
* The tenant record stays, with the property field marked as "Property no longer in system."
* The owner must manually resolve the tenancy (log move-out or update the contract).

### Tenant Name Changes

If a tenant's legal name changes (e.g., after marriage), the owner cannot edit the name directly in the Tenants module. The name is pulled from the tenant's system profile (linked to their identity). Changes to personal identity data are handled through the system's profile management.

### No Automatic Deletion

Tenant records are never deleted automatically. Even after a tenant becomes "Former" or "Removed," their full record (payments, documents, notes, history) remains in the system indefinitely. The owner can request manual deletion only through system support, subject to legal data retention requirements.
