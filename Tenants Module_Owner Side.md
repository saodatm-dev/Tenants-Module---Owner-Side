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

*   **Applications (Acquisition):** Manages potential tenants. Once an application is approved and a contract is signed, the applicant converts into a Tenant. Applications module handles acquisition; Tenants module handles retention of renters.
*   **Listings (Marketing):** The tenant's lifecycle directly triggers actions in the Listings module. For example, when an active tenant's status changes to "Moving Out", the system can automatically prepare a new listing advert for that specific property so you can find a replacement tenant without downtime.
*   **Contracts (Legal Rules):** Contracts bind tenants to properties. Contract dates (Start, End) automatically update the tenant's lifecycle status (e.g., Active, Expiring Soon).
*   **Payments (Financials):** The module aggregates payment data to show the tenant's current balance, payment history, and financial reliability.
