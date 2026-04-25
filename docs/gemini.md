# Gemini AI — Project Context & Instructions
## AFRAWY STORE — Local Web-Based Store Management System

> **Source:** `PRD_AfrawyStore_EN.md` v1.1
> **Purpose:** This file provides Gemini AI with the full project context, architecture, and coding conventions for the Afrawy Store project. Use this as the primary reference when generating code, answering questions, or making architectural decisions.

---

## 1. Project Overview

**AFRAWY STORE** is a locally-hosted web-based management system for a tools & paints shop. It manages products, categories, inventory, and sales — fully in Arabic with RTL design — accessible via browser on the local network.

### Target Users
| Role | Description |
|------|-------------|
| **Admin** | Store owner/manager — full system privileges |
| **Employee** | Sales staff — limited access for daily operations |

### Scope
- **In Scope:** Product CRUD, category CRUD, inventory tracking, sales/POS, analytics dashboard, reports, low-stock alerts, role-based access control.
- **Out of Scope:** E-commerce, multi-branch, supplier orders, loyalty programs, mobile app.

### Deployment
Locally hosted on the store's server/workstation. No internet required.

---

## 2. Tech Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Backend | ASP.NET Core MVC | .NET 8 LTS |
| ORM | Entity Framework Core | EF Core 8 |
| Database | Microsoft SQL Server | 2019/2022 |
| Frontend | HTML5, CSS3, JavaScript (ES6+) | Vanilla JS + jQuery |
| UI Framework | Bootstrap RTL | v5.3 (`bootstrap.rtl.min.css`) |
| Arabic Font | Cairo | Google Fonts |
| Auth | ASP.NET Core Identity | Built-in |
| Charts | Chart.js | v4.x |
| PDF Export | DinkToPdf / iTextSharp | Server-side |

### Development Tools
- Visual Studio 2022 / VS Code
- Git, NuGet, npm, EF Core Migrations

---

## 3. Architecture — Onion Architecture

The project follows **Onion Architecture** with **Generic Repository + Unit of Work** pattern.

### Layer Diagram
```
┌─────────────────────────────────────────────────────┐
│               Presentation Layer                    │
│           AfrawyStore.Web (MVC)                     │
│    Controllers · Views · ViewModels · wwwroot       │
├─────────────────────────────────────────────────────┤
│              Application Layer                      │
│          AfrawyStore.Application                    │
│    Services · DTOs · Interfaces · Mappings          │
├─────────────────────────────────────────────────────┤
│               Domain Layer  (Core)                  │
│            AfrawyStore.Domain                       │
│      Entities · Domain Interfaces · Enums           │
├─────────────────────────────────────────────────────┤
│            Infrastructure Layer                     │
│        AfrawyStore.Infrastructure                   │
│  DbContext · Generic Repo · UoW · Migrations · DI  │
└─────────────────────────────────────────────────────┘
```

> **Dependency Rule:** Outer layers depend on inner layers. Domain has ZERO external dependencies.

### Key Architectural Decisions
1. **Generic Repository** — Standard CRUD for all entities, reduces duplication.
2. **Unit of Work** — Transaction consistency across multiple repositories.
3. **Specialized Repositories** — Inherit from GenericRepository, add entity-specific queries.
4. **Price Snapshots** — SaleItems store price/cost at time of sale for historical accuracy.
5. **Soft Delete** — Products and Users use `IsActive` flag (no hard delete).
6. **EF Core Migrations** — All schema changes versioned and repeatable.

---

## 4. Project Structure

```
AfrawyStore/
│
├── AfrawyStore.Web/                        # Presentation Layer
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── DashboardController.cs
│   │   ├── ProductsController.cs
│   │   ├── CategoriesController.cs
│   │   ├── InventoryController.cs
│   │   ├── SalesController.cs
│   │   ├── ReportsController.cs
│   │   └── UsersController.cs
│   ├── ViewModels/
│   │   ├── DashboardViewModel.cs
│   │   ├── ProductViewModel.cs
│   │   ├── SaleViewModel.cs
│   │   └── ReportViewModel.cs
│   ├── Views/
│   │   ├── Shared/ (_Layout.cshtml, _Sidebar.cshtml, _Navbar.cshtml)
│   │   ├── Dashboard/
│   │   ├── Products/
│   │   ├── Categories/
│   │   ├── Inventory/
│   │   ├── Sales/
│   │   ├── Reports/
│   │   └── Users/
│   ├── wwwroot/ (css/, js/, uploads/products/)
│   ├── appsettings.json
│   └── Program.cs
│
├── AfrawyStore.Application/                # Application Layer
│   ├── Interfaces/
│   │   ├── Services/ (IProductService, ICategoryService, etc.)
│   │   └── Persistence/ (IUnitOfWork)
│   ├── Services/ (ProductService, CategoryService, etc.)
│   └── DTOs/ (ProductDto, SaleDto, SaleItemDto, etc.)
│
├── AfrawyStore.Domain/                     # Domain Layer (Core)
│   ├── Entities/ (BaseEntity, User, Category, Product, Inventory, InventoryLog, Sale, SaleItem)
│   ├── Interfaces/ (IGenericRepository)
│   └── Enums/ (UserRole, PaymentMethod, SaleStatus, InventoryChangeType)
│
├── AfrawyStore.Infrastructure/             # Infrastructure Layer
│   ├── Data/ (AppDbContext, Configurations/)
│   ├── Repositories/ (GenericRepository, ProductRepository, etc.)
│   ├── UnitOfWork/ (UnitOfWork.cs)
│   ├── Migrations/
│   └── DependencyInjection/ (InfrastructureServiceExtensions.cs)
│
└── AfrawyStore.Tests/                      # Tests
    ├── Unit/ (Services/, Repositories/)
    └── Integration/
```

---

## 5. Domain Entities

### BaseEntity (shared)
```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```

### Users
| Field | Type | Constraints |
|-------|------|-------------|
| UserId | int | PK, Auto-increment |
| Username | nvarchar(50) | Not Null, Unique |
| PasswordHash | nvarchar(256) | Not Null |
| FullName | nvarchar(100) | Not Null |
| Role | nvarchar(20) | Admin / Employee |
| IsActive | bit | Default: true |
| CreatedAt | datetime | Default: GETDATE() |

### Categories
| Field | Type | Constraints |
|-------|------|-------------|
| CategoryId | int | PK, Auto-increment |
| Name | nvarchar(100) | Not Null, Unique |
| Description | nvarchar(500) | Nullable |
| ParentCategoryId | int | FK → Categories (self-ref, nullable) |
| CreatedAt | datetime | Default: GETDATE() |

### Products
| Field | Type | Constraints |
|-------|------|-------------|
| ProductId | int | PK, Auto-increment |
| SKU | nvarchar(50) | Not Null, Unique |
| Name | nvarchar(150) | Not Null |
| Description | nvarchar(1000) | Nullable |
| CategoryId | int | FK → Categories, Not Null |
| CostPrice | decimal(10,2) | Not Null |
| SellingPrice | decimal(10,2) | Not Null |
| Unit | nvarchar(30) | e.g.: piece, liter, kilo |
| ImagePath | nvarchar(300) | Nullable |
| IsActive | bit | Default: true |
| CreatedAt | datetime | Default: GETDATE() |

### Inventory
| Field | Type | Constraints |
|-------|------|-------------|
| InventoryId | int | PK, Auto-increment |
| ProductId | int | FK → Products, Unique (1-to-1) |
| CurrentStock | decimal(10,2) | Not Null, Default: 0 |
| MinimumStock | decimal(10,2) | Not Null, Default: 5 |
| LastUpdated | datetime | Auto-updated |

### InventoryLogs
| Field | Type | Constraints |
|-------|------|-------------|
| LogId | int | PK, Auto-increment |
| ProductId | int | FK → Products |
| ChangeType | nvarchar(30) | StockIn / Adjustment / Sale |
| QuantityChange | decimal(10,2) | + for addition, − for deduction |
| Note | nvarchar(300) | Nullable |
| CreatedBy | int | FK → Users |
| CreatedAt | datetime | Default: GETDATE() |

### Sales
| Field | Type | Constraints |
|-------|------|-------------|
| SaleId | int | PK, Auto-increment |
| SaleDate | datetime | Default: GETDATE() |
| TotalAmount | decimal(10,2) | Not Null |
| TotalProfit | decimal(10,2) | Calculated at save |
| Discount | decimal(10,2) | Default: 0 |
| PaymentMethod | nvarchar(30) | Cash / Card / Other |
| Status | nvarchar(20) | Completed / Voided |
| CreatedBy | int | FK → Users |
| Note | nvarchar(300) | Nullable |

### SaleItems
| Field | Type | Constraints |
|-------|------|-------------|
| SaleItemId | int | PK, Auto-increment |
| SaleId | int | FK → Sales, Not Null |
| ProductId | int | FK → Products, Not Null |
| Quantity | decimal(10,2) | Not Null |
| UnitPrice | decimal(10,2) | Snapshot at time of sale |
| UnitCost | decimal(10,2) | Snapshot at time of sale |
| LineTotal | decimal(10,2) | Quantity × UnitPrice |
| LineProfit | decimal(10,2) | Quantity × (UnitPrice − UnitCost) |

### Relationships
```
Users ──< InventoryLogs
Users ──< Sales
Categories ──< Products
Categories ──< Categories  (self-referencing: parent/child)
Products ──── Inventory       (1-to-1)
Products ──< InventoryLogs
Products ──< SaleItems
Sales ──< SaleItems
```

---

## 6. Core Design Patterns

### Generic Repository Interface
```csharp
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

### Unit of Work Interface
```csharp
public interface IUnitOfWork : IDisposable
{
    IProductRepository     Products     { get; }
    ICategoryRepository    Categories   { get; }
    IInventoryRepository   Inventory    { get; }
    ISaleRepository        Sales        { get; }
    Task<int> SaveChangesAsync();
}
```

---

## 7. Business Rules

1. A product must belong to exactly one category.
2. An inventory record is automatically created when a new product is added.
3. Selling price must be ≥ cost price (enforced at application level).
4. Voiding a sale restores the corresponding inventory quantities.
5. Stock cannot drop below zero (enforced at application level).
6. SaleItems store a snapshot of price/cost at time of sale for historical accuracy.
7. SKU must be unique across all products.
8. Category names must be unique.
9. Usernames must be unique and at least 4 characters long.
10. All monetary values stored with two decimal places.

---

## 8. Role Permissions Matrix

| Feature | Admin | Employee |
|---------|-------|----------|
| Dashboard | ✅ Full | ✅ View |
| Products — View | ✅ | ✅ |
| Products — Create/Edit/Delete | ✅ | ❌ |
| Categories — CRUD | ✅ | ❌ |
| Inventory — View | ✅ | ✅ |
| Inventory — Adjust | ✅ | Stock-in only |
| Sales — Create | ✅ | ✅ |
| Sales — Void | ✅ | ❌ |
| Reports | ✅ Full | Limited |
| User Management | ✅ | ❌ |

---

## 9. UI/UX Requirements

### RTL & Arabic Interface
```html
<html lang="ar" dir="rtl">
<link rel="stylesheet" href="bootstrap.rtl.min.css">
<link href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700&display=swap" rel="stylesheet">
<style>body { font-family: 'Cairo', sans-serif; }</style>
```

### Layout Structure
- **Top Nav Bar:** Logo/name (Afrawy), current user, role badge, logout.
- **Right Sidebar:** Navigation (Dashboard, Products, Categories, Inventory, Sales, Reports, Users).
- **Main Content:** Page-specific content.
- **Footer:** Version, copyright.

### Pages Summary

| # | Page | URL | Access |
|---|------|-----|--------|
| 1 | Login | `/Account/Login` | Public |
| 2 | Dashboard | `/Dashboard` | Admin, Employee |
| 3 | Product List | `/Products` | Admin (CRUD), Employee (view) |
| 4 | Product Create/Edit | `/Products/Create`, `/Products/Edit/{id}` | Admin |
| 5 | Category List | `/Categories` | Admin |
| 6 | Category Create/Edit | `/Categories/Create`, `/Categories/Edit/{id}` | Admin |
| 7 | Inventory List | `/Inventory` | Admin, Employee |
| 8 | Inventory Adjustment | `/Inventory/Adjust/{productId}` | Admin (all), Employee (stock-in) |
| 9 | Sales List | `/Sales` | Admin, Employee |
| 10 | New Sale (POS) | `/Sales/New` | Admin, Employee |
| 11 | Sale Details/Receipt | `/Sales/Detail/{id}` | Admin, Employee |
| 12 | Reports | `/Reports` | Admin (full), Employee (daily sales) |
| 13 | User Management | `/Users` | Admin |

---

## 10. Profit Calculation Logic

| Formula | Calculation |
|---------|-------------|
| Unit Profit | SellingPrice − CostPrice |
| Margin % | (SellingPrice − CostPrice) ÷ SellingPrice × 100 |
| Line Profit | Quantity × (UnitPrice − UnitCost) → stored in `SaleItems.LineProfit` |
| Transaction Profit | SUM(SaleItems.LineProfit) → stored in `Sales.TotalProfit` |
| Period Profit | SUM(Sales.TotalProfit) for selected time range |

---

## 11. Low-Stock Alert Logic

- Each product has a `MinimumStock` value in the Inventory table.
- Dashboard query: `WHERE CurrentStock <= MinimumStock`.
- Alerts appear as:
  - Counter badge in navbar
  - Dashboard summary card
  - Row-level badge on product & inventory pages
  - Dedicated low-stock report
- Color coding: 🟢 green (good), 🟡 yellow (warning), 🔴 red (critical/low).

---

## 12. Security Requirements

- Passwords hashed via ASP.NET Core Identity (PBKDF2).
- All routes protected with `[Authorize]`.
- Admin routes use `[Authorize(Roles = "Admin")]`.
- Anti-forgery tokens on all POST forms (CSRF).
- Inputs sanitized via EF Core parameterized queries.
- Session timeout: configurable (default 8 hours).

---

## 13. Performance Requirements

| Metric | Target |
|--------|--------|
| Page load time | < 2 seconds |
| Low-stock alert accuracy | 100% |
| User onboarding time | < 1 hour |
| System uptime | > 99.5% (local) |
| Report generation | < 5 seconds |

### Optimizations
- Indexed columns: `Products.SKU`, `Products.CategoryId`, `Sales.SaleDate`, `Inventory.ProductId`.
- Pagination on all list pages (default: 20 rows/page).
- Dashboard queries use SQL aggregation (not in-memory).

---

## 14. Data Validation Rules

- SKU: unique across all products.
- SellingPrice ≥ CostPrice.
- Inventory quantity ≥ 0.
- Category names: unique.
- Usernames: unique, minimum 4 characters.
- Monetary values: 2 decimal places.

---

## 15. Backup & Recovery

- Automatic daily SQL Server backup to configurable local folder.
- Admin manual backup trigger from settings page.
- Backup files named with timestamp.

---

## 16. Coding Conventions for Gemini

When generating code for this project, follow these conventions:

### C# / .NET
- Use C# 12 features with .NET 8.
- Follow Onion Architecture — never reference outer layers from inner layers.
- All async methods return `Task<T>` and use `Async` suffix.
- Use constructor injection for DI.
- Entity classes go in `AfrawyStore.Domain/Entities/`.
- Service interfaces go in `AfrawyStore.Application/Interfaces/Services/`.
- Service implementations go in `AfrawyStore.Application/Services/`.
- Repository implementations go in `AfrawyStore.Infrastructure/Repositories/`.
- Controllers go in `AfrawyStore.Web/Controllers/`.
- ViewModels go in `AfrawyStore.Web/ViewModels/`.

### Views (Razor/cshtml)
- Always use `dir="rtl"` and `lang="ar"` in layout.
- Use Bootstrap 5 RTL classes.
- Use Cairo font from Google Fonts.
- All UI text in Arabic.

### JavaScript
- Vanilla JS + jQuery.
- POS logic in `wwwroot/js/sales-pos.js`.
- General site JS in `wwwroot/js/site.js`.

### Database
- EF Core Code-First with Migrations.
- Use Fluent API for entity configurations (in `Data/Configurations/`).
- All monetary fields: `decimal(10,2)`.

---

## 17. Current Progress

Refer to `tasks.md` for the current milestone status. As of the latest update:

- ✅ **Milestone 0** (Foundation & Architecture) — Partially complete (entities, repos, UoW, DbContext done; migration & DI pending).
- ⬜ **Milestones 1–10** — Not started.

---

*Generated from PRD_AfrawyStore_EN.md v1.1 — AFRAWY STORE*
