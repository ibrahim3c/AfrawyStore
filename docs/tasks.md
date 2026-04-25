# AFRAWY STORE — Project Milestones

> Auto-generated from `PRD_AfrawyStore_EN.md` v1.1
> Status legend: `[ ]` Not started · `[/]` In progress · `[x]` Done

---

## Milestone 0 — Foundation & Architecture ✅ (Done)
> Domain entities, generic repo, UoW, DbContext, basic DI wiring.

- [x] 0.1 Domain entities (User, Category, Product, Inventory, InventoryLog, Sale, SaleItem, BaseEntity)
- [x] 0.2 Enums (UserRole, PaymentMethod, SaleStatus, InventoryChangeType)
- [x] 0.3 IGenericRepository + GenericRepository
- [x] 0.4 Specialized repositories (Product, Category, Inventory, Sale)
- [x] 0.5 IUnitOfWork + UnitOfWork
- [x] 0.6 AppDbContext + EF configurations
- [x] 0.7 ProductDto + IProductService + ProductService
- [x] 0.8 EF Core initial migration + database creation
- [x] 0.9 DI registration (InfrastructureServiceExtensions)

---

## Milestone 1 — Authentication & User Management ✅ (Done)
> Login, roles, session, user CRUD (Admin only).

- [x] 1.1 ASP.NET Core Identity integration (or custom auth with hashed passwords)
- [x] 1.2 AccountController — Login / Logout actions with real auth
- [x] 1.3 Login view wired to backend (currently a mockup)
- [x] 1.4 Role-based authorization ([Authorize], Admin/Employee)
- [x] 1.5 UsersController — CRUD (Admin only)
- [x] 1.6 User views: list, create/edit form
- [x] 1.7 Password change functionality
- [x] 1.8 Session timeout configuration

---

## Milestone 2 — Category Management ✅ (Done)
> Full CRUD for categories, including subcategories.

- [x] 2.1 ICategoryService + CategoryService
- [x] 2.2 CategoryDto
- [x] 2.3 CategoriesController — Index, Create, Edit, Delete
- [x] 2.4 Category views: list table, create/edit form
- [x] 2.5 Parent/child category support (self-referencing FK)
- [x] 2.6 Prevent deleting a category that has products

---

## Milestone 3 — Product Management ✅ (Done)
> Full CRUD, search/filter, image upload, status toggle.

- [x] 3.1 ProductsController — Create, Edit, Delete, Details actions
- [x] 3.2 Product views: Create/Edit form (two-column layout), Details view
- [x] 3.3 Search & filter (by name, SKU, category, status)
- [x] 3.4 Image upload to `wwwroot/uploads/products/`
- [x] 3.5 Live profit margin indicator on create/edit form
- [x] 3.6 SKU uniqueness validation
- [x] 3.7 SellingPrice ≥ CostPrice validation
- [x] 3.8 Bulk active/inactive toggle
- [x] 3.9 Pagination (default 20 rows)
- [x] 3.10 Auto-create Inventory record on product creation

---

## Milestone 4 — Inventory Management
> Stock tracking, adjustments, low-stock alerts.

- [ ] 4.1 IInventoryService + InventoryService
- [ ] 4.2 InventoryController — Index, Adjust
- [ ] 4.3 Inventory views: list (color-coded status), adjustment form/modal
- [ ] 4.4 Stock-in, adjustment, and correction logging (InventoryLogs)
- [ ] 4.5 MinimumStock threshold per product
- [ ] 4.6 Low-stock alert logic (CurrentStock ≤ MinimumStock)
- [ ] 4.7 Alert badge in sidebar / navbar
- [ ] 4.8 Stock cannot drop below zero (validation)

---

## Milestone 5 — Sales & Point of Sale
> POS interface, cart, checkout, receipt, void.

- [ ] 5.1 ISaleService + SaleService
- [ ] 5.2 SaleDto + SaleItemDto
- [ ] 5.3 SalesController — Index, New, Detail, Void actions
- [ ] 5.4 POS view (Sales/New) — wired to real product search + cart logic
- [ ] 5.5 `sales-pos.js` — real-time cart, quantity controls, total calculation
- [ ] 5.6 Stock availability check before sale completion
- [ ] 5.7 Inventory deduction on sale confirmation
- [ ] 5.8 Inventory restoration on sale void
- [ ] 5.9 Price/cost snapshot in SaleItems
- [ ] 5.10 Discount field support
- [ ] 5.11 Payment method selection (Cash / Card / Other)
- [ ] 5.12 Sale Detail / receipt view (printable)
- [ ] 5.13 Sales list with date filter, search, pagination
- [ ] 5.14 Void restricted to Admin only

---

## Milestone 6 — Dashboard (Data-Driven)
> Replace static mockup data with real queries.

- [ ] 6.1 DashboardViewModel (totals, chart data, latest sales, low-stock list)
- [ ] 6.2 DashboardController — aggregate queries via services
- [ ] 6.3 Wire summary cards to real data
- [ ] 6.4 Wire Chart.js to real last-7-days sales data
- [ ] 6.5 Wire latest transactions table to real sales
- [ ] 6.6 Wire low-stock panel to real inventory alerts

---

## Milestone 7 — Reports & Export
> Sales, inventory, profit/loss, low-stock reports with PDF/CSV export.

- [ ] 7.1 IReportService + ReportService
- [ ] 7.2 ReportViewModel
- [ ] 7.3 ReportsController — tab-based report views
- [ ] 7.4 Daily/periodic sales report (date range filter)
- [ ] 7.5 Inventory status report
- [ ] 7.6 Profit & loss report (by period, product, category)
- [ ] 7.7 Low-stock report
- [ ] 7.8 CSV export
- [ ] 7.9 PDF export (DinkToPdf or iTextSharp)

---

## Milestone 8 — Alerts Service
> Centralized alert logic consumed by all pages.

- [ ] 8.1 IAlertService + AlertService
- [ ] 8.2 Low-stock count injected into layout (navbar badge)
- [ ] 8.3 Dashboard alert card linked to `/Inventory?filter=low`
- [ ] 8.4 Row-level badges on product and inventory pages

---

## Milestone 9 — Security, Validation & Performance
> Hardening pass across the entire application.

- [ ] 9.1 Anti-forgery tokens on all POST forms
- [ ] 9.2 All routes protected with [Authorize]
- [ ] 9.3 Role-gated routes ([Authorize(Roles = "Admin")])
- [ ] 9.4 Input sanitization review
- [ ] 9.5 Database indexes (SKU, CategoryId, SaleDate, ProductId)
- [ ] 9.6 Pagination on all list pages
- [ ] 9.7 Dashboard queries use SQL aggregation

---

## Milestone 10 — Backup, Polish & Deployment
> Final touches before go-live.

- [ ] 10.1 Automatic daily SQL Server backup
- [ ] 10.2 Manual backup trigger from settings
- [ ] 10.3 Print styles for receipts (80mm thermal)
- [ ] 10.4 Responsive breakpoints (sidebar collapse ≤1024px)
- [ ] 10.5 Skeleton loaders for data-heavy pages
- [ ] 10.6 Docker + docker-compose setup (optional)
- [ ] 10.7 Final QA pass & bug fixes
