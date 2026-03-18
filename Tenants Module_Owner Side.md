# Tenants Module Specification: Owner Side

## 1. Module Overview

### What is the Tenants Module?
The Tenants module is the property owner's central database for managing confirmed tenants. It shifts the focus from tenant acquisition to long-term relationship management. It stores all data for active and past renters, providing a complete rental history workspace.

### Who uses it?
* **Property Owners:** To manage their own property portfolios.
* **Property Managers:** To access tenant contacts, contract states, and operational notes for day-to-day management.

### What problems does it solve?
* **Data Accessibility:** Eliminates the need to search through contracts in the "Contracts" section to access important documents.
* **Occupancy Tracking:** Shows exactly who occupies which unit, upcoming move-outs, and past tenancies at a glance.
* **Tenant History:** Builds a historical record of tenant behavior (payments, property care, compliance) to inform lease renewal decisions.
* **Operational Efficiency:** Consolidates unit, contract, and payment data into a single, actionable view.

### Relationship with Other Modules
The Tenants module works closely with other system components but serves a distinct purpose from Applications.

*   **Applications (Acquisition):** Manages potential tenants. Once an application is approved and a lease is signed, the applicant converts into a Tenant. Applications handle acquisition; Tenants handle retention.
*   **Properties (Location):** Tenants link directly to a specific building or unit. A tenant's status automatically updates the property's availability (e.g., a "Moving Out" tenant triggers a listing update).
*   **Contracts (Legal Rules):** Contracts bind tenants to properties. Contract dates (Start, End) automatically update the tenant's lifecycle status (e.g., Active, Expiring Soon).
*   **Payments (Financials):** The module aggregates payment data to show the tenant's current balance, payment history, and financial reliability.
