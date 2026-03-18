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

Every tenant in the system has a status that reflects their current state. Statuses change based on contract dates, owner actions, or system automation.

### Status Definitions

| Status | Meaning | When It Appears | Changed By | Final? |
|--------|---------|-----------------|------------|--------|
| **Pending Move-In** | The contract is signed, but the start date has not arrived yet. | Automatically, when a contract is signed with a future start date. | System (automatic). Changes to "Active" when the contract start date arrives. | No |
| **Active** | The tenant currently occupies the unit and the lease is in effect. | Automatically, when the contract start date arrives or when a contract with today's date (or earlier) is signed. | System (automatic). | No |
| **Lease Ending** | The contract end date is approaching. The owner needs to decide whether to renew or prepare for move-out. | Automatically, when the system detects the contract is within the notification window (e.g., 30 days before expiry). | System (automatic). The owner can renew the contract (returns to "Active") or let it expire. | No |
| **Overstaying** | The contract has expired, but the tenant has not moved out and the lease has not been renewed. | Automatically, when the contract end date passes without renewal or a logged move-out. | System (automatic). The owner resolves this by either renewing the contract or logging a move-out. | No |
| **Former** | The tenant has vacated and the move-out has been confirmed by the owner. The record is archived. | When the owner manually logs a completed move-out. | Owner (manual action). | Yes |
| **Evicted** | The tenant has been removed due to a lease violation, non-payment, or legal action. | When the owner manually marks the tenant as evicted. | Owner (manual action). | Yes |

### Status Transitions

```
Pending Move-In → Active → Lease Ending → Overstaying → Former
                                    ↓                        ↑
                              (Renewed → Active)         Evicted
                                                     (from any active status)
```

### Key Notes

* **"Pending Move-In" and "Active"** are set by the system based on contract dates. The owner does not manually change these.
* **"Lease Ending"** is a warning state, not a separate action by the owner. It triggers notifications but does not change the tenant's access or billing.
* **"Overstaying"** keeps the tenant fully active (billing continues, access remains). It is a visual flag indicating the contract needs attention, not a penalty state.
* **"Former"** and **"Evicted"** are the only final statuses. Once set, the tenant is archived and cannot return to an active state without a new contract.
* **"Evicted"** is separate from "Former" to distinguish voluntary departures from forced removals. This distinction matters for tenant history and future rental decisions.
* An owner can log an eviction from any active status (Active, Lease Ending, or Overstaying).
