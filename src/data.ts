export const profile = {
  name: "Omar Shipl",
  role: "Backend Engineer & ERP Architect",
  tagline: "I build distributed systems that handle real retail operations — from atomic stock deductions to event-driven loyalty pipelines.",
  github: "https://github.com/3mure",
  liveDemo: "https://management.runasp.net",
  swagger: "https://management.runasp.net/swagger",
  summary:
    "Backend-focused engineer graduating with a full-stack Enterprise Resource Planning platform built on ASP.NET Core 8 microservices. I design for correctness under concurrency, clean domain boundaries, and infrastructure that survives production traffic.",
};

export const stats = [
  { value: "5", label: "Microservices" },
  { value: "60+", label: "API Endpoints" },
  { value: "8", label: "Domain Areas" },
  { value: "100%", label: "CQRS Coverage" },
];

export const services = [
  {
    id: "erp",
    name: "ERP Core",
    role: "Operational backbone",
    description:
      "Catalog, inventory, sales orders, reports, loyalty, and alerts unified in one backend. MediatR CQRS with vertical slice features and EF Core against SQL Server.",
    points: [
      "Multi-warehouse inventory with min/max thresholds",
      "Atomic stock deduction on order creation",
      "Stock transfers and real-time adjustments",
      "Sales, finance, and POS shift reporting",
    ],
    accent: "sky",
  },
  {
    id: "auth",
    name: "Auth Service",
    role: "Identity & access",
    description:
      "JWT Bearer authentication with ASP.NET Core Identity. OTP-based password reset, profile management, and role-based authorization across services.",
    points: [
      "JWT issuance and refresh flow",
      "Email OTP password recovery",
      "Profile updates with image handling",
      "Role claims propagated gateway-wide",
    ],
    accent: "emerald",
  },
  {
    id: "ordering",
    name: "Ordering Service",
    role: "Cart & fulfillment",
    description:
      "Shopping cart, order lifecycle, delivery addresses, and shipment tracking. Publishes integration events for payment and stock consumers.",
    points: [
      "Cart add, update, remove, checkout flows",
      "Order confirm, status history, reorder",
      "User address book with default selection",
      "Shipment creation and live tracking",
    ],
    accent: "amber",
  },
  {
    id: "payment",
    name: "Payment Service",
    role: "Stripe integration",
    description:
      "Payment intent creation and processing through a provider abstraction. Consumes order events and emits payment succeeded/failed events.",
    points: [
      "Stripe payment intent lifecycle",
      "Provider-agnostic IPaymentProvider",
      "Payment status lookup by order",
      "Event-driven success/failure handling",
    ],
    accent: "violet",
  },
  {
    id: "notification",
    name: "Notification Service",
    role: "Event-driven comms",
    description:
      "Consumes MassTransit events — order status changes, low stock, offer created — and persists notifications with read-state tracking.",
    points: [
      "RabbitMQ consumers for domain events",
      "Per-user notification store",
      "Mark read / mark all read endpoints",
      "Email delivery via MailKit",
    ],
    accent: "rose",
  },
  {
    id: "gateway",
    name: "API Gateway",
    role: "Single entry point",
    description:
      "Routes client requests to downstream microservices with JWT forwarding, CORS, and centralized configuration for the service mesh.",
    points: [
      "Path-based routing to all services",
      "Token forwarding and validation",
      "Centralized CORS and rate policy",
      "Health and readiness aggregation",
    ],
    accent: "slate",
  },
];

export const techStack = [
  { name: "ASP.NET Core 8", category: "Backend" },
  { name: "C#", category: "Backend" },
  { name: "Entity Framework Core", category: "Data" },
  { name: "SQL Server", category: "Data" },
  { name: "Redis", category: "Data" },
  { name: "MediatR", category: "Architecture" },
  { name: "CQRS", category: "Architecture" },
  { name: "gRPC", category: "Communication" },
  { name: "MassTransit", category: "Communication" },
  { name: "RabbitMQ", category: "Communication" },
  { name: "JWT Bearer", category: "Security" },
  { name: "ASP.NET Identity", category: "Security" },
  { name: "FluentValidation", category: "Quality" },
  { name: "Serilog", category: "Quality" },
  { name: "Docker", category: "DevOps" },
  { name: "Swagger / OpenAPI", category: "DevOps" },
];

export const architecture = {
  layers: [
    {
      title: "Client applications",
      detail: "Management dashboard, e-commerce storefront, and POS clients.",
    },
    {
      title: "ERP Service (core)",
      detail: "Catalog, inventory, orders, reports, loyalty, alerts. MediatR CQRS with EF Core and Redis cache.",
    },
    {
      title: "Integration layer",
      detail: "gRPC contracts for catalog and promotion. RabbitMQ events via MassTransit with a transactional outbox.",
    },
    {
      title: "Data & infrastructure",
      detail: "SQL Server for persistence, Redis for distributed cache, RabbitMQ for async messaging.",
    },
  ],
};

export const apiAreas = [
  { area: "Products", count: 8, methods: "GET, POST, PUT, DELETE" },
  { area: "Inventory", count: 4, methods: "GET, POST" },
  { area: "Orders", count: 8, methods: "GET, POST, PUT" },
  { area: "Cart", count: 6, methods: "GET, POST, PUT, DELETE" },
  { area: "Delivery", count: 9, methods: "GET, POST, PUT, DELETE" },
  { area: "Auth", count: 9, methods: "GET, POST, PUT" },
  { area: "Payment", count: 3, methods: "GET, POST" },
  { area: "Notifications", count: 3, methods: "GET, POST" },
];

export const principles = [
  {
    title: "Correctness under concurrency",
    body: "Atomic stock deductions and transactional outbox publishing so orders and inventory never drift apart.",
  },
  {
    title: "Vertical slice architecture",
    body: "Each feature owns its command, handler, validator, and DTO — no shared god-controllers or anemic models.",
  },
  {
    title: "Event-driven boundaries",
    body: "Services communicate through RabbitMQ integration events, keeping coupling low and failures isolated.",
  },
  {
    title: "Honest about scope",
    body: "Documented limitations and a planned cleanup roadmap — the repo is shared as-is for academic review.",
  },
];
