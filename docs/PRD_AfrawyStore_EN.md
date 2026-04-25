# Product Requirements Document (PRD)
## Local Web-Based Store Management System
### Afrawy Tools & Paints Store — AFRAWY STORE

---

**Document Version:** 1.1  
**Date:** 2025  
**Status:** Draft  
**Prepared by:** Product Management

---

## Table of Contents

1. [Overview](#1-overview)
2. [Objectives](#2-objectives)
3. [Features](#3-features)
4. [Entities & Relationships (ERD)](#4-entities--relationships-erd)
5. [Pages & User Interface](#5-pages--user-interface)
6. [Tech Stack](#6-tech-stack)
7. [Additional Notes](#7-additional-notes)
8. [Project Structure](#8-project-structure)

---

## 1. Overview

### 1.1 Product Name
**AFRAWY STORE** — Local Web-Based Management System for a Tools & Paints Store

### 1.2 Product Summary
AFRAWY STORE is a locally-hosted web-based store management system, designed specifically for small and medium tools and paints shops. It provides a centralized platform for managing products, categories, inventory, and sales — fully in Arabic with RTL design — and is accessible via browser on the local network. The system is built on reliability, ease of use, and operational efficiency without relying on external cloud services.

### 1.3 Target Users
| Role | Description |
|------|-------------|
| **Admin** | Store owner or manager — full system privileges |
| **Employee** | Sales staff — limited access for daily operations |

### 1.4 Scope
- **In Scope:** Product management, category management, inventory tracking, sales processing, analytics dashboard, reports, low-stock alerts, role-based access control.
- **Out of Scope:** E-commerce / online selling, multi-branch management, supplier orders, customer loyalty programs, mobile app development.

### 1.5 Deployment Model
The system runs locally on the store's server or workstation. Optionally, it can be containerized with Docker and orchestrated via Kubernetes with a CI/CD pipeline from Jenkins.

### 1.6 Interface Language
The interface is fully in **Arabic** with right-to-left **(RTL)** direction support. The `dir="rtl"` attribute is applied at the HTML level, with Bootstrap RTL customization (`bootstrap.rtl.min.css`) and the **Cairo** font from Google Fonts.

---

## 2. Objectives

### 2.1 Business Objectives
- Eliminate paper-based manual inventory and sales tracking.
- Provide instant visibility into stock levels, daily sales performance, and profitability.
- Reduce stockouts through automated low-stock alerts.
- Simplify daily operations for employees with an easy-to-use Arabic interface.

### 2.2 Technical Objectives
- Deliver a fast and stable locally-hosted web application that requires no internet connection.
- Implement role-based access control to protect sensitive data.
- Ensure data integrity through relational database design and strict validation rules.
- Support future scalability via container-ready architecture.
- Apply **Onion Architecture** with **Generic Repository + Unit of Work** to ensure complete separation of layers and ease of testing.

### 2.3 Success Metrics
| Metric | Target |
|--------|--------|
| Page load time | Less than 2 seconds |
| Low-stock alert accuracy | 100% |
| User onboarding time | Less than 1 hour |
| System uptime | More than 99.5% (local) |
| Report generation time | Less than 5 seconds |

---

## 3. Features

### 3.1 Core Features

#### 3.1.1 Dashboard
- Summary cards: total products, categories, today's sales, number of low-stock products.
- Latest transactions table (last 10 sales).
- Low-stock products list with reorder indicators.
- Daily sales chart (bar chart by hour or day).

#### 3.1.2 Product Management (CRUD)
- Create, read, update, and delete products.
- Fields: name, SKU, description, category, cost price, selling price, unit of measure, image (optional).
- Search and filter by name, SKU, or category.
- Bulk status toggle (active / inactive).

#### 3.1.3 Category Management (CRUD)
- Create, read, update, and delete categories.
- Fields: category name, description, parent category (for subcategories).
- View all products within a specific category.

#### 3.1.4 Inventory Management (CRUD)
- Track current stock quantity per product.
- Record stock-in entries (restocking) with date, quantity, and supplier note.
- Record inventory adjustments (damage, loss, correction).
- Set minimum stock threshold per product.
- Trigger low-stock alerts when quantity falls below the minimum threshold.

#### 3.1.5 Sales Management (CRUD)
- Create new sales transactions with multiple items.
- Automatically deduct inventory upon sale confirmation.
- Calculate item subtotals, discounts, and transaction totals.
- Record payment method (cash, card, other).
- View, edit (same day), and void sales.
- Display receipt in a printable format.

### 3.2 Authentication & Authorization
- Secure login with username and hashed password (ASP.NET Core Identity).
- Role-based access control: Admin and Employee.
- Session management with configurable timeout.
- Password change functionality.

#### Role Permissions Matrix

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

### 3.3 Alerts & Notifications
- Low-stock alert banner on the dashboard.
- Low-stock badge / indicator on product and inventory pages.
- Alert threshold configurable per product.
- Visual color coding: green (good), yellow (warning), red (critical / low).

### 3.4 Profit Calculation
- Unit profit = selling price − cost price.
- Transaction profit = sum of sales item profits.
- Daily / monthly profit summary on the reports page.
- Profit margin percentage displayed in product details.

### 3.5 Reports
- Daily sales report (date range filter, totals, by category).
- Inventory status report (all products, stock levels, minimum threshold status).
- Profit & loss summary report (by period, by product or category).
- Low-stock report (products below minimum threshold).
- Export to PDF or CSV.

---

## 4. Entities & Relationships (ERD)

### 4.1 Entities & Fields

#### Users
| Field | Type | Constraints |
|-------|------|-------------|
| UserId | int | PK, Auto-increment |
| Username | nvarchar(50) | Not Null, Unique |
| PasswordHash | nvarchar(256) | Not Null |
| FullName | nvarchar(100) | Not Null |
| Role | nvarchar(20) | Admin / Employee |
| IsActive | bit | Default: true |
| CreatedAt | datetime | Default: GETDATE() |

#### Categories
| Field | Type | Constraints |
|-------|------|-------------|
| CategoryId | int | PK, Auto-increment |
| Name | nvarchar(100) | Not Null, Unique |
| Description | nvarchar(500) | Nullable |
| ParentCategoryId | int | FK → Categories (self-ref, nullable) |
| CreatedAt | datetime | Default: GETDATE() |

#### Products
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

#### Inventory
| Field | Type | Constraints |
|-------|------|-------------|
| InventoryId | int | PK, Auto-increment |
| ProductId | int | FK → Products, Unique (1-to-1) |
| CurrentStock | decimal(10,2) | Not Null, Default: 0 |
| MinimumStock | decimal(10,2) | Not Null, Default: 5 |
| LastUpdated | datetime | Auto-updated |

#### InventoryLogs
| Field | Type | Constraints |
|-------|------|-------------|
| LogId | int | PK, Auto-increment |
| ProductId | int | FK → Products |
| ChangeType | nvarchar(30) | StockIn / Adjustment / Sale |
| QuantityChange | decimal(10,2) | + for addition, − for deduction |
| Note | nvarchar(300) | Nullable |
| CreatedBy | int | FK → Users |
| CreatedAt | datetime | Default: GETDATE() |

#### Sales
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

#### SaleItems
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

### 4.2 Relationships Summary

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

### 4.3 Core Business Rules
- A product must belong to exactly one category.
- An inventory record is automatically created when a new product is added.
- Selling price must be ≥ cost price (enforced at the application level).
- Voiding a sale restores the corresponding inventory quantities.
- Stock cannot drop below zero (enforced at the application level).
- SaleItems store a snapshot of price and cost at the time of sale to preserve historical record accuracy.

---

## 5. Pages & User Interface

### 5.1 General Layout
All pages share a unified master layout **fully in Arabic RTL**:
- **Top Navigation Bar:** Store logo / name (Afrawy), current user, role badge, logout button.
- **Right Sidebar:** Navigation links (Dashboard, Products, Categories, Inventory, Sales, Reports, Users).
- **Main Content Area:** Page-specific content.
- **Footer:** System version, copyright.

#### RTL Settings
```html
<html lang="ar" dir="rtl">
<link rel="stylesheet" href="bootstrap.rtl.min.css">
<link href="https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700&display=swap" rel="stylesheet">
<style>body { font-family: 'Cairo', sans-serif; }</style>
```

### 5.2 Page Descriptions

#### Page 1: Login
- **URL:** `/Account/Login`
- **Access:** Public
- **Layout:** Centered card on a full-screen background.
- **Components:** Username field, password field, "Remember me" checkbox, login button.
- **Behavior:** Redirect to dashboard on success; inline error message on failure.

#### Page 2: Dashboard
- **URL:** `/Dashboard`
- **Access:** Admin, Employee
- **Layout:** Row of 4 summary cards ← two-column content area.
- **Components:** Cards (total products, categories, today's sales, low-stock count); bar chart for last 7 days; latest transactions table; low-stock products table.
- **Behavior:** Data refreshes on page load. Low-stock count card links to the inventory page with filter applied.

#### Page 3: Product List
- **URL:** `/Products`
- **Access:** Admin (CRUD), Employee (view only)
- **Layout:** Filter bar ← data table ← pagination.
- **Components:** Search bar, category dropdown, active/inactive toggle, "Add Product" button (Admin), table: SKU, name, category, selling price, stock, status, actions.
- **Behavior:** Edit and delete for Admin only.

#### Page 4: Create / Edit Product
- **URL:** `/Products/Create`, `/Products/Edit/{id}`
- **Access:** Admin only
- **Layout:** Two-column form.
- **Components:** SKU, name, description (textarea), category (dropdown), cost price, selling price (with live margin % display), unit, image upload, active toggle, save / cancel.
- **Behavior:** Validate all required fields; check SKU uniqueness on submit.

#### Page 5: Category List
- **URL:** `/Categories`
- **Access:** Admin only
- **Layout:** Simple table with an add button.
- **Components:** Table: name, parent category, description, product count, actions.
- **Behavior:** Deleting a category that contains products requires reassignment or blocks deletion.

#### Page 6: Create / Edit Category
- **URL:** `/Categories/Create`, `/Categories/Edit/{id}`
- **Access:** Admin only
- **Layout:** Single-column form.
- **Components:** Name, description, parent category (optional), save / cancel.

#### Page 7: Inventory List
- **URL:** `/Inventory`
- **Access:** Admin, Employee
- **Layout:** Filter bar ← table with color-coded stock status.
- **Components:** Search, stock status filter (all / low / critical), table: product name, SKU, current quantity, minimum threshold, status badge, last updated, actions.
- **Behavior:** Status calculated dynamically: green = above threshold, yellow = at threshold, red = below threshold.

#### Page 8: Inventory Adjustment
- **URL:** `/Inventory/Adjust/{productId}`
- **Access:** Admin (all types); Employee (stock-in only)
- **Layout:** Dialog modal or dedicated page.
- **Components:** Product name (read-only), change type dropdown, quantity field, note field, submit button.
- **Behavior:** Log entry recorded in InventoryLogs; CurrentStock updated.

#### Page 9: Sales List
- **URL:** `/Sales`
- **Access:** Admin, Employee
- **Layout:** Date filter + search ← table.
- **Components:** Date range filter, search by sale number, table: sale number, date, item count, total, payment, status, created by, actions.
- **Behavior:** Void action for Admin only and requires confirmation.

#### Page 10: New Sale (Point of Sale)
- **URL:** `/Sales/New`
- **Access:** Admin, Employee
- **Layout:** Split view — product search panel (right) + order summary panel (left) — with RTL consideration.
- **Components:** Product search, results list, "Add to Sale"; sale items with quantity editing, remove item, discount field, total, payment method selector, "Complete Sale" and "Cancel".
- **Behavior:** Total updates instantly. Stock availability checked before completion. On completion, inventory is deducted and sale is saved.

#### Page 11: Sale Details / Receipt
- **URL:** `/Sales/Detail/{id}`
- **Access:** Admin, Employee
- **Layout:** Printable receipt view + metadata.
- **Components:** Sale header (number, date, cashier), items table, totals, payment method, print button, void button (Admin only).

#### Page 12: Reports
- **URL:** `/Reports`
- **Access:** Admin (full); Employee (daily sales only)
- **Layout:** Tab navigation for report types.
- **Components:** Tab 1 — daily/periodic sales report; Tab 2 — inventory status report; Tab 3 — profit & loss report; Tab 4 — low-stock report. All tabs include CSV/PDF export.

#### Page 13: User Management
- **URL:** `/Users`
- **Access:** Admin only
- **Layout:** User list table + add user button.
- **Components:** Table: full name, username, role, status, actions. Add/edit form with full name, username, password (on creation), role, active toggle.

---

## 6. Tech Stack

### 6.1 Core Stack

| Layer | Technology | Version / Notes |
|-------|-----------|-----------------|
| Backend Framework | ASP.NET Core MVC | .NET 8 LTS |
| ORM | Entity Framework Core | EF Core 8 |
| Database | Microsoft SQL Server | 2019 or 2022 |
| Frontend | HTML5, CSS3, JavaScript (ES6+) | Vanilla JS + jQuery |
| UI Framework | Bootstrap RTL | v5.3 (bootstrap.rtl.min.css) |
| Arabic Font | Cairo | Google Fonts |
| Authentication | ASP.NET Core Identity | Built-in |
| Charts | Chart.js | v4.x |
| PDF Export | DinkToPdf / iTextSharp | Server-side |

### 6.2 Optional DevOps Stack

| Tool | Purpose |
|------|---------|
| Docker | Containerize web app and SQL Server |
| Jenkins | CI/CD pipeline: build, test, deploy |
| Kubernetes | Multi-container deployment orchestration |

### 6.3 Development Tools
| Tool | Purpose |
|------|---------|
| Visual Studio 2022 / VS Code | Primary development environment |
| Git (GitHub / GitLab / local) | Version control |
| NuGet | Backend package manager |
| npm | Frontend build tools (when needed) |
| EF Core Migrations | Versioned database schema management |

---

## 7. Additional Notes

### 7.1 Low-Stock Alert Logic
- Each product has a `MinimumStock` value stored in the Inventory table.
- An instant query on dashboard load identifies products where `CurrentStock ≤ MinimumStock`.
- Alerts appear as: a counter badge in the navigation bar, a dashboard card, a row-level badge in inventory and product pages, and a dedicated low-stock report.
- Admins set the minimum threshold per product from the inventory adjustment page.

### 7.2 Profit Calculation Logic

| Formula | Calculation |
|---------|-------------|
| Unit Profit | SellingPrice − CostPrice |
| Margin % | (SellingPrice − CostPrice) ÷ SellingPrice × 100 |
| Line Profit | Quantity × (UnitPrice − UnitCost) → stored in SaleItems.LineProfit |
| Transaction Profit | SUM(SaleItems.LineProfit) → stored in Sales.TotalProfit |
| Period Profit | SUM(Sales.TotalProfit) for the selected time range |

> **Note:** Cost and price are saved as a snapshot at the time of sale to ensure historical record accuracy when prices are updated later.

### 7.3 Data Validation Rules
- SKU must be unique across all products.
- Selling price must be ≥ cost price.
- Inventory quantity cannot be reduced below zero.
- Category names must be unique.
- Usernames must be unique and at least 4 characters long.
- All monetary values are stored with two decimal places.

### 7.4 Security Considerations
- Passwords are hashed using ASP.NET Core Identity's default PBKDF2 algorithm.
- All routes are protected with `[Authorize]`; role-restricted routes use `[Authorize(Roles = "Admin")]`.
- Anti-forgery tokens on all POST forms (CSRF protection).
- Inputs are sanitized to prevent SQL injection via EF Core parameterized queries.
- Session timeout is configurable (default: 8 hours).

### 7.5 Performance Considerations
- Indexed columns: `Products.SKU`, `Products.CategoryId`, `Sales.SaleDate`, `Inventory.ProductId`.
- Pagination on all list pages (default: 20 rows per page).
- Dashboard queries use SQL aggregation rather than in-memory calculation.

### 7.6 Backup & Recovery
- Automatic daily SQL Server backup to a configurable local folder.
- Admin can trigger a manual backup from the settings page.
- Backup files are named with a timestamp for easy identification.

---

## 8. Project Structure

The project follows **Onion Architecture** with the **Generic Repository + Unit of Work** pattern.

### 8.1 Onion Architecture Layers

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
│  DbContext · Generic Repo · UoW · Migrations · DI   │
└─────────────────────────────────────────────────────┘
```

> **Dependency Principle:** Outer layers depend on inner ones — Domain does not depend on any other layer.

### 8.2 Full Folder Structure

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
│   │
│   ├── ViewModels/
│   │   ├── DashboardViewModel.cs
│   │   ├── ProductViewModel.cs
│   │   ├── SaleViewModel.cs
│   │   └── ReportViewModel.cs
│   │
│   ├── Views/
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml         ← dir="rtl", lang="ar", Cairo font
│   │   │   ├── _Sidebar.cshtml
│   │   │   └── _Navbar.cshtml
│   │   ├── Dashboard/
│   │   ├── Products/
│   │   ├── Categories/
│   │   ├── Inventory/
│   │   ├── Sales/
│   │   ├── Reports/
│   │   └── Users/
│   │
│   ├── wwwroot/
│   │   ├── css/
│   │   │   ├── bootstrap.rtl.min.css
│   │   │   └── site.css
│   │   ├── js/
│   │   │   ├── site.js
│   │   │   └── sales-pos.js
│   │   └── uploads/products/
│   │
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
│
├── AfrawyStore.Application/                # Application Layer
│   ├── Interfaces/
│   │   ├── Services/
│   │   │   ├── IProductService.cs
│   │   │   ├── ICategoryService.cs
│   │   │   ├── IInventoryService.cs
│   │   │   ├── ISaleService.cs
│   │   │   ├── IReportService.cs
│   │   │   └── IAlertService.cs
│   │   └── Persistence/
│   │       └── IUnitOfWork.cs             ← Unit of Work Interface
│   │
│   ├── Services/
│   │   ├── ProductService.cs
│   │   ├── CategoryService.cs
│   │   ├── InventoryService.cs
│   │   ├── SaleService.cs
│   │   ├── ReportService.cs
│   │   └── AlertService.cs
│   │
│   └── DTOs/
│       ├── ProductDto.cs
│       ├── SaleDto.cs
│       ├── SaleItemDto.cs
│       └── InventoryAdjustDto.cs
│
├── AfrawyStore.Domain/                     # Domain Layer (Core)
│   ├── Entities/
│   │   ├── BaseEntity.cs                  ← Id, CreatedAt (shared properties)
│   │   ├── User.cs
│   │   ├── Category.cs
│   │   ├── Product.cs
│   │   ├── Inventory.cs
│   │   ├── InventoryLog.cs
│   │   ├── Sale.cs
│   │   └── SaleItem.cs
│   │
│   ├── Interfaces/
│   │   └── IGenericRepository.cs          ← Generic Repository Interface
│   │
│   └── Enums/
│       ├── UserRole.cs
│       ├── PaymentMethod.cs
│       ├── SaleStatus.cs
│       └── InventoryChangeType.cs
│
├── AfrawyStore.Infrastructure/             # Infrastructure Layer
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/
│   │       ├── ProductConfiguration.cs
│   │       ├── CategoryConfiguration.cs
│   │       ├── SaleConfiguration.cs
│   │       └── InventoryConfiguration.cs
│   │
│   ├── Repositories/
│   │   ├── GenericRepository.cs           ← Generic Repository Implementation
│   │   ├── ProductRepository.cs           ← Inherits GenericRepository<Product>
│   │   ├── CategoryRepository.cs
│   │   ├── InventoryRepository.cs
│   │   └── SaleRepository.cs
│   │
│   ├── UnitOfWork/
│   │   └── UnitOfWork.cs                  ← Unit of Work Implementation
│   │
│   ├── Migrations/
│   └── DependencyInjection/
│       └── InfrastructureServiceExtensions.cs
│
├── AfrawyStore.Tests/                      # Tests
│   ├── Unit/
│   │   ├── Services/
│   │   └── Repositories/
│   └── Integration/
│
├── docker-compose.yml                      # Docker (optional)
├── Dockerfile                              # Application container (optional)
├── Jenkinsfile                             # CI/CD pipeline (optional)
└── README.md
```

### 8.3 Generic Repository Design

```csharp
// Domain/Interfaces/IGenericRepository.cs
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}

// Infrastructure/Repositories/GenericRepository.cs
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    // ... method implementations
}
```

### 8.4 Unit of Work Design

```csharp
// Application/Interfaces/Persistence/IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    IProductRepository     Products     { get; }
    ICategoryRepository    Categories   { get; }
    IInventoryRepository   Inventory    { get; }
    ISaleRepository        Sales        { get; }
    Task<int> SaveChangesAsync();
}

// Infrastructure/UnitOfWork/UnitOfWork.cs
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IProductRepository   Products   { get; }
    public ICategoryRepository  Categories { get; }
    // ...

    public UnitOfWork(AppDbContext context, ...)
    {
        _context   = context;
        Products   = new ProductRepository(context);
        // ...
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
```

### 8.5 Key Architectural Decisions
- **Onion Architecture** ensures complete layer separation; the Domain layer is fully independent and does not rely on any external framework.
- **Generic Repository** provides standard CRUD operations for all entities and reduces code duplication.
- **Unit of Work** ensures transaction consistency across multiple repositories in a single operation.
- **Specialized Repositories** inherit from GenericRepository and add entity-specific queries.
- **Price Snapshots** in SaleItems preserve historical record accuracy regardless of subsequent price changes.
- **Soft Delete** for products and users (`IsActive`) to maintain referential integrity and audit trail.
- **EF Core Migrations** manage all schema changes in a versioned, repeatable manner.
- **Bootstrap RTL + Cairo font** provide a professional Arabic interface and comfortable user experience.

---

*End of Document — AFRAWY STORE PRD v1.1 | Confidential — For Internal Use Only*
