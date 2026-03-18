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

*   **Tenant:** A person or entity that has signed a lease agreement. Before signing, they are an "Applicant"; the moment both parties sign the contract (digitally or physically), the applicant becomes a "Tenant." Each tenant is tied to exactly one unit — if several unrelated people rent in the same building, each one has their own unit and their own tenant record.
*   **Rental Relationship:** The full history between an owner and a tenant. It starts when the lease is signed and remains in the system as a permanent record, even after the tenant moves out.
*   **Active Tenant:** A tenant who currently lives in the property, has a scheduled move-in date, or is overstaying past their lease end date. Active tenants generate rental income and carry current payment and lease obligations.
*   **Former Tenant:** A tenant whose lease has ended *and* who has completed the move-out process. Their profile is kept for reference (e.g., payment track record) but they no longer have access to the property.
*   **Property / Unit:** The physical space a tenant rents. Each unit is linked to exactly one tenant at a time.
*   **Lease / Contract:** The legal agreement that connects a tenant to a property. The contract's start and end dates define the timeline of the rental relationship.
*   **Tenant Status:** The current operational state of the tenant (e.g., "Active", "Moving Out", "Former"). 
    * *Note on Expirations & Overstays:* When a contract is nearing its End Date, the system automatically notifies both the tenant and the owner. If the End Date passes without a formal renewal, the tenant does **not** automatically become a "Former Tenant." They remain functionally in an "Active" state—though visually tagged in the UI as *Expired / Overstaying*—until the owner manually logs a completed move-out. This ensures billing automation continues and owners never lose tracking of holdover tenants.
