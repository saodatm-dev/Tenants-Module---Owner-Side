# Tenants Module Specification: Owner Side

## 1. Module Overview

### What is the Tenants Module?
The Tenants module is a centralized, CRM-like operational hub designed for property owners to manage their confirmed tenants. It transitions the owner’s perspective from "filling a vacancy" to "managing a long-term relationship." It acts as a structured database and workspace for all tenant-related data, encompassing active renters, past tenants, and their complete rental history. 

### Who uses it?
* **Property Owners (Landlords):** Managing their own portfolio of properties.
* **Property Managers / Administrative Staff:** Assisting owners in day-to-day operations, requiring quick access to tenant contacts, contract statuses, and historical notes.

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

---

## 2. 5 Whys Analysis

To deeply understand the core needs driving the Tenants module, we apply the "5 Whys" technique across three different foundational problems owners face. 

### Option 1: The Problem of Fragmented Tenant Tracking
* **Why 1:** *Why do owners lose track of crucial tenant information and unit occupancy statuses?* Because tenant data (contacts, IDs, agreements) is scattered across various generic apps like Excel, WhatsApp, and physical paper.
* **Why 2:** *Why is data scattered?* Because there hasn't been a single, accessible system of record that unifies the applicant converting into an active renter.
* **Why 3:** *Why is there no single system of record used?* Because owners often use one tool to find the tenant (classifieds) and revert to manual offline methods for the actual management.
* **Why 4:** *Why do they revert to manual methods after finding a tenant?* Because historic digital tools focused almost exclusively on marketing the property, leaving the post-signature operational phase unsupported.
* **Why 5:** *Why was the post-signature operational phase unsupported?* Because building a robust operational CRM requires complex integrations involving legal contracts and continuous lifecycle state management, which classified platforms did not prioritize.

### Option 2: The Problem of Evaluating Tenant Reliability
* **Why 1:** *Why do owners struggle to accurately assess whether they should renew an existing tenant's lease or what deposit to return?* Because they do not have an easily accessible, structured history of the tenant’s behavior, late payments, and incidents.
* **Why 2:** *Why don't they have this history?* Because incidents or late payments are reacted to in the moment and rarely logged systematically for future review.
* **Why 3:** *Why aren't incidents logged systematically?* Because compiling a "tenant profile" manually feels like administrative overhead that busy owners don't have time for.
* **Why 4:** *Why does it feel like administrative overhead?* Because the relationship is viewed purely transactionally month-to-month rather than as a continuous tracked profile within a unified digital system.
* **Why 5:** *Why is it viewed transitionally without tracking?* Because small-to-medium property management has traditionally lacked accessible software tools designed around the "human element" of leasing (the Tenant Profile), focusing instead purely on the physical asset.

### Option 3: The Problem of Clunky Turnover and Departures
* **Why 1:** *Why do owners experience high friction, vacancy gaps, and confusion when a lease ends and a tenant moves out?* Because owners lack a proactive, automated pipeline telling them exactly when tenants are advancing toward their move-out dates.
* **Why 2:** *Why do they lack this pipeline?* Because expiration dates and renewal discussions are tracked only in static contract PDFs or relying on human memory.
* **Why 3:** *Why are dates trapped in static files or memory?* Because the system managing the contract is not dynamically linked to the system managing the tenant's active state.
* **Why 4:** *Why are these systems not dynamically linked?* Because traditionally, Contracts, Tenants, and Listings are treated as siloed data points rather than interconnected entities that share state changes.
* **Why 5:** *Why are they treated as siloed data points?* Because legacy or generic software (like spreadsheets) cannot enforce relational business logic where a contract's end date automatically updates a tenant's status and consequently triggers listing availability.

---

## 3. Competitor Research & Analysis

Understanding how market leaders handle the "Tenants" module helps position our solution effectively. The analysis spans enterprise Western solutions to localized CIS classified giants.

### 🇺🇸 Yardi Systems (Yardi Voyager / Yardi Breeze)
* **Overview:** The enterprise giant of property management. Yardi covers everything from construction to accounting and tenant management.
* **Tenants Module Approach:** Tenants are deeply embedded in a massive ERP system. The "Resident" profile tracks complex accounting ledgers, work orders, communications, and legal proceedings.
* **Critical Analysis:** Extremely robust but suffers from interface bloat and a steep learning curve. The rigid, ledger-first approach can feel overly bureaucratic for smaller portfolios.
* **Takeaway:** Excellent standard for data comprehensiveness, but we must avoid their complexity to serve SMB owners.

### 🇺🇸 Buildium
* **Overview:** A leading platform focused on property management companies rather than DIY individual landlords.
* **Tenants Module Approach:** Strong emphasis on the "Tenant Portal." The tenant module manages balances, maintenance requests, and broad communication tracking.
* **Critical Analysis:** Strikes a better balance of power and usability than Yardi. However, its terminology and structure heavily assume a property manager acting as an intermediary, rather than a direct owner-to-tenant relationship.
* **Takeaway:** The benchmark for operational CRM features (tracking issues and balances), but we need a more direct, simplified relationship model.

### 🇺🇸 TenantCloud
* **Overview:** Cloud-based software aimed directly at DIY landlords and smaller property managers.
* **Tenants Module Approach:** Treats the tenant as an active user of the platform. The module covers basic tracking, connection status, online payments, and simple maintenance requests.
* **Critical Analysis:** Very accessible and affordable, but its modular nature can sometimes feel disjointed. Features often feel like discrete widgets rather than a cohesive lifecycle flow. 
* **Takeaway:** Proves the DIY market needs these tools. We should aim for their accessibility but integrate the lifecycle stages (conversion from Application to Tenant to Ex-Tenant) more smoothly.

### 🇺🇸 Zillow (Zillow Rental Manager)
* **Overview:** The dominant US real estate marketplace, extending its reach down the funnel into property management.
* **Tenants Module Approach:** Started as a listing/lead generation platform. It now offers background checks, lease creation, and rent collection.
* **Critical Analysis:** Zillow is incredibly strong at the top of the funnel (finding applicants). However, its post-signature "Tenants" management remains relatively thin, acting more as a payment processor dashboard than a holistic CRM for recording incidents and tracking long-term behavior.
* **Takeaway:** Top-of-funnel platforms struggle to build deep operational tools. This is a primary opportunity to differentiate by offering a stronger operational layer than pure classifieds.

### 🇷🇺 ЦИАН (Сдаю/Сниму / ЦИАН.Аренда)
* **Overview:** The leading Russian classifieds portal for real estate.
* **Tenants Module Approach:** Expanding into transactional services, but still fundamentally a marketplace. Their rental management focuses on the transaction: verifying tenants, providing insurance, and processing payments.
* **Critical Analysis:** Like Zillow, CIAN excels at matching (Applications stage). Their "Tenant Management" is transactional and financial rather than operational. It lacks deep features for tracking the day-to-day lifespan of the tenancy, incident histories, or complex contract modifications over time.
* **Takeaway:** They solve the "trust" and "payment" problem but do not provide a structural CRM for the owner's daily life. 

### 🇷🇺 DomClick
* **Overview:** Sberbank's real estate ecosystem.
* **Tenants Module Approach:** Heavily skewed towards mortgages, buying, and selling. For rentals, they offer legal templates and secure transaction services (backed by the bank's infrastructure).
* **Critical Analysis:** Extremely strong legal and financial integration, leveraging Sberbank's trust. However, it is not an operational dashboard for property owners. It facilitates the *event* of signing a lease but does not manage the *process* of being a landlord.
* **Takeaway:** Demonstrates the value of institutional trust in real estate digital tools, but leaves the operational CRM space open.

### Synthesis & Conclusion
**The Market Gap:** There is a clear divide in the market. 
1. **The Marketplaces (Zillow, ЦИАН, DomClick)** are excellent at marketing Listings and gathering Applications but offer shallow, transactional post-lease Tenant management.
2. **The ERPs/PM Softwares (Yardi, Buildium)** offer deep post-lease operations but are often complicated, expensive, and geared towards institutional managers.

**Strategic Positioning for Maydon:** 
The Tenants module must bridge this gap. It needs to be as intuitive and accessible as the Marketplaces, but provide the actionable, CRM-style depth (history, status tracking, structured profiles) found in Buildium or TenantCloud. 

By clearly differentiating "Applications" (the acquisition engine, mirroring marketplace strengths) from "Tenants" (the retention and operations engine, mirroring PM software strengths), we provide a holistic, end-to-end ecosystem that neither side of the market currently perfects for the SMB owner.
