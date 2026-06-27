# ERP System — Graduation Project

A full-stack **Enterprise Resource Planning (ERP)** platform built with **ASP.NET Core 8**, designed for retail and e-commerce operations. The **ERP service is the core** of this solution — it unifies product catalog, inventory, sales, reporting, loyalty, and external integrations in one backend.

> **Note on this repository:** I ran into issues publishing this project to GitHub during my graduation timeline and did not have enough time to fully resolve them
>  (cleanup, secret management, CI setup, and final polish).
>  This repo is shared as-is for portfolio and academic review. Some configuration files may still need sanitization before production use — see [Security notice](#security-notice) below.

---

## Live demo

| Resource | URL |
|----------|-----|
| ERP API (production) | https://management.runasp.net |
| Swagger / REST docs | https://management.runasp.net/swagger |
| gRPC contract info | https://management.runasp.net/grpc/info |
| Health check | https://management.runasp.net/health |

---

## Architecture overview

```
┌─────────────────────────────────────────────────────────────────┐
│                     Client applications                          │
│   (Management dashboard · E-commerce storefront · POS clients)   │
└────────────────────────────┬────────────────────────────────────┘
                             │ REST · gRPC · JWT
┌────────────────────────────▼────────────────────────────────────┐
│  ★ ERP Service (core)                                           │
│  Catalog · Inventory · Orders · Reports · Loyalty · Alerts      │
│  MediatR (CQRS) · EF Core · Redis cache · MassTransit outbox      │
└──────┬──────────────────────────────┬───────────────────────────┘
       │ gRPC (catalog, promotion)     │ RabbitMQ events
       ▼                               ▼
┌──────────────┐              ┌─────────────────────┐
│ Erp.Grpc     │              │ Notification Service │
│ Client SDK   │              │ API Gateway          │
└──────────────┘              └─────────────────────┘
       │
       ▼
┌──────────────┐
│ SQL Server   │
│ Redis        │
└──────────────┘
```

---

## ERP core — what it does

The **ERP** project (`/ERP`) is not just a product catalog. It is the operational backbone of the system:

### Catalog & retail
- Product, category, brand, banner, and occasion management
- Branch-scoped product listings and stock visibility
- Retail roles and POS-oriented data model (customers, payment methods, shifts)

### Inventory & warehouse
- Multi-warehouse inventory with min/max stock thresholds
- Stock transfers between warehouses
- Inventory adjustments and real-time stock validation
- Low-stock alerts and dashboard notifications (background worker)

### Sales & ordering
- Sales order creation with atomic stock deduction
- Invoice generation, returns, and order lookup APIs
- Integration with external storefronts via REST and gRPC

### Reports & analytics
- Sales, finance, employee, supplier, and POS shift reports
- Export capabilities for operational reporting
- Top employee and top/underperforming supplier insights

### Loyalty & promotions
- Loyalty points rules and redemption (gRPC promotion service)
- Coupon validation and adjusted pricing for integrated clients

### Integration layer
- **gRPC services:** `CatalogGrpc`, `PromotionGrpc` — consumed by `Erp.Grpc.Client`
- **Message bus:** RabbitMQ via MassTransit (order events, stock events, loyalty updates)
- **Transactional outbox:** reliable event publishing with EF Core

---

## Solution structure

| Project | Role |
|---------|------|
| **ERP** | Core backend — catalog, inventory, orders, reports, loyalty, gRPC |
| **BuildingBlocks** | Shared entities, integration events, proto definitions, middleware |
| **Erp.Grpc.Client** | Reference .NET SDK for external apps to call ERP gRPC APIs |
| **API Gateway** | Entry point for microservice routing |
| **Notification Service** | Event-driven notifications |

---

## Tech stack

- **.NET 8** · ASP.NET Core Web API
- **Entity Framework Core 8** + SQL Server
- **MediatR** — CQRS / vertical slice features
- **FluentValidation** · **Serilog**
- **gRPC** (Grpc.AspNetCore) — catalog & promotion contracts
- **MassTransit + RabbitMQ** — async integration events
- **Redis** — distributed caching
- **JWT Bearer** authentication
- **Swagger / OpenAPI**
- **Docker** support

---

## Getting started (local)

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Redis (optional for cache)
- RabbitMQ (optional for messaging features)

### Run ERP

```bash
cd ERP
dotnet restore
dotnet ef database update
dotnet run
```

Configure connection strings and secrets via **User Secrets** or environment variables — do not commit real credentials.

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
```

Swagger UI is available at `/swagger` when running locally.

### Connect an external app via gRPC

See [Erp.Grpc.Client/README.md](Erp.Grpc.Client/README.md) for SDK setup and example usage.

---

## API highlights (ERP)

| Area | Example endpoints |
|------|-------------------|
| Products | Branch-scoped product queries, stock management |
| Inventory | CRUD, transfers, adjustments |
| Orders | `POST /api/v1/orders`, `GET /api/v1/orders/{id}` |
| Reports | Sales, finance, employee, supplier, POS shift |
| Alerts | Dashboard stock and operational alerts |
| gRPC | Product lookup, stock validation, coupons, loyalty |

Full contract: `GET /grpc/info`

---

## Security notice

This repository was prepared under graduation deadline pressure. **Before using in production:**

1. Rotate all database, Redis, RabbitMQ, JWT, and deployment credentials
2. Move secrets to environment variables or Azure Key Vault / User Secrets
3. Remove or redact any credentials still present in `appsettings*.json` or publish profiles
4. Review `.gitignore` and ensure sensitive files are excluded going forward

Do not use committed configuration values as-is in any live environment.

---

## Known limitations (honest scope)

- GitHub publishing and repo hygiene were deferred due to graduation timeline
- Some microservices were consolidated into the ERP core (e.g. ordering flows merged into ERP)
- Test coverage and CI/CD pipeline are minimal
- Documentation is portfolio-oriented rather than production-grade

These items are planned for post-graduation cleanup.

---

## Author

**Omar Shipl** — Graduation project  
GitHub: [3mure]  


---

## License

Academic / portfolio use. Contact the author for other usage.
