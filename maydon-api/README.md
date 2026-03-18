# Maydon API

**Modular .NET monolith** for real estate and property management. Built with Clean Architecture, CQRS, and Domain-Driven Design.

---

## 📚 Documentation

| Module | Description | Docs |
|--------|-------------|------|
| **Identity** | Authentication, authorization, user management, multi-tenancy | [ERD](docs/identity/ERD.md) · [Architecture](docs/identity/Architecture.md) · [Flows](docs/identity/Flows.md) |
| **Common** | Shared reference data — regions, districts, languages, banks, currencies | [ERD](docs/common/ERD.md) · [Architecture](docs/common/Architecture.md) · [Flows](docs/common/Flows.md) |
| **Building** | Real estate, listings, leases, meters, communal bills | [ERD](docs/building/ERD.md) · [Architecture](docs/building/Architecture.md) · [Flows](docs/building/Flows.md) |
| **Document** | Document storage and management | [ERD](docs/document/ERD.md) · [Architecture](docs/document/Architecture.md) · [Flows](docs/document/Flows.md) |
| **Didox** | Didox fiscal integration | [ERD](docs/didox/ERD.md) · [Architecture](docs/didox/Architecture.md) · [Flows](docs/didox/Flows.md) |
| **Deployment Flow** | Deployment flow and rollback strategy | [Deployment Flow](docs/deployment/deployment-flow.html) |

---

## 🏗️ Tech Stack

- **.NET 10** / C# 13
- **PostgreSQL** (Npgsql + EF Core)
- **Redis** (caching / sessions)
- **MinIO** (file storage)
- **JWT** (authentication)

## 📁 Project Structure

```
src/
├── Api/Maydon.Host/        # ASP.NET Core Minimal API host
├── Core/                   # Shared kernel (domain, application, infrastructure)
├── Identity/               # Identity module (auth, users, roles)
├── Common/                 # Common reference data module
├── Building/               # Building & real estate module
├── Document/               # Document management module
└── Didox/                  # Fiscal integration module
```
