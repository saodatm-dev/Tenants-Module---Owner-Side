# Tenants Module Specification: Owner Side

## 1. Module Overview

### What is the Tenants Module?
The Tenants module is a centralized operational hub designed for property owners to manage their confirmed tenants. It transitions the owner’s perspective from "filling a vacancy" to "managing a long-term relationship." It acts as a structured database and workspace for all tenant-related data, encompassing active renters, past tenants, and their complete rental history. 

### Who uses it?
* **Property Owners (Landlords):** Managing their own portfolio of properties.
* **Property Managers:** Assisting owners in day-to-day operations, requiring quick access to tenant contacts, contract statuses, and historical notes.

### What problems does it solve for the owner?
* **Data Fragmentation:** Eliminates the need to search through emails, messaging apps (Telegram/WhatsApp), and physical folders to find tenant details, passport copies, or emergency contacts.
* **Lifecycle Tracking:** Provides immediate visibility into who is currently occupying which unit, who is moving out soon, and who has vacated.
* **Relationship & Risk Management:** Accumulates a historical record of the tenant's behavior (payment punctuality, property care, rule adherence), allowing the owner to make informed decisions about lease renewals.
* **Operational Efficiency:** Streamlines communication and administrative tasks by having all context (unit, contract, payment status) in one view.

### Relationship with Other Modules
The Tenants module is highly interconnected with the rest of the property management ecosystem. It is distinctly different from the Applications module.

*   **Applications (Pre-contract stage):** The Applications module handles *potential* tenants. Once an applicant is approved and a lease is signed, the applicant seamlessly transitions (converts) into a Tenant. The Applications module is the top of the funnel; the Tenants module is the retention lifecycle.
*   **Listings / Properties (Spatial context):** Tenants must be linked to a specific physical asset (Building/Unit). The status of a Tenant directly affects the Property status (e.g., if a tenant's status changes to "Moving Out," the listing might automatically prepare to be republished).
*   **Contracts / Leasing (Legal context):** The Contract is the governing document that bounds a Tenant to a Property. The Dates within the contract (Start Date, End Date) automatically drive the Tenant's lifecycle statuses (Active, Expiring Soon, Past).
*   **Payments (Financial context):** The Tenants module aggregates payment data to display the tenant’s balance, payment history, and financial reliability. A tenant's profile is the primary lens for viewing their individual ledger.
