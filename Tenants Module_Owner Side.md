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

*   **Tenant:** An individual or entity that has signed a legally binding lease agreement. An individual is strictly an "Applicant" until the contract is digitally or physically signed by both parties; the exact moment of signature is when they convert into a "Tenant". The system tracks tenants on a strict 1-to-1 basis with units (e.g., if multiple unrelated adults rent in the same building or apartment, they each rent a separate specific unit and are tracked independently).
*   **Rental Relationship:** The active and historical lifecycle between the owner and the tenant. It begins at the lease signing and continues indefinitely as a historical record even after the tenant vacates.
*   **Active Tenant:** A tenant who currently occupies a property, holds a future move-in date, or is in an "overstay" period. They are the immediate source of active rental income and hold current legal obligations.
*   **Former Tenant:** A tenant whose lease has expired or been terminated, *and* who has completed the move-out process. Their profile is archived for historical reference (e.g., past payment reliability) but is stripped of active property access rights.
*   **Property / Unit:** The specific physical asset rented by the tenant. The property maintains a strict 1-to-1 relationship with the tenant entity.
*   **Lease / Contract:** The governing legal agreement linking a Tenant to a Property. The contract's Start and End dates act as the primary timeline for the Rental Relationship.
*   **Tenant Status:** The current operational state of the tenant (e.g., "Active", "Moving Out", "Former"). 
    * *Note on Expirations:* If a contract's End Date passes without formal renewal, the tenant does **not** automatically become a "Former Tenant." To accommodate overstays, they remain legally tracked as an "Active Tenant" until the owner manually logs a completed move-out. Automatically converting them to "Former" would prematurely stop billing automation and cause owners to lose tracking of holdover tenants.
