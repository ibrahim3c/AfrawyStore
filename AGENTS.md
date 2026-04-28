# AGENTS.md — AFRAWY STORE Agent Guidelines

> Instructions for agentic coding agents working in this repository.
> Based on `docs/PRD_AfrawyStore_EN.md` and `docs/tasks.md` (v1.1, 2025).

---

## Project Overview

**AFRAWY STORE** — Local web-based store management system for a tools & paints shop.
- **Tech Stack:** ASP.NET Core MVC (.NET 8) · Entity Framework Core 8 · SQL Server · Bootstrap RTL · Vanilla JS + jQuery
- **Architecture:** Onion Architecture + Generic Repository + Unit of Work
- **Interface:** Full Arabic (RTL), Cairo font, Bootstrap RTL stylesheet
- **Status:** Milestones 0-6 complete. Remaining: 7 (Reports), 8 (Alerts), 9 (Security), 10 (Polishing)

### Layer Dependency Rule
```
Domain (Core) <- Application <- Infrastructure <- Web (Presentation)
```
**The Domain layer must NEVER depend on any other layer.**

---

## Build / Run / Migrate Commands

### Build
```bash
# Full solution build
dotnet build AfrawyStore.sln

# Single project build
dotnet build AfrawyStore.Web/AfrawyStore.Web.csproj

# Release build
dotnet build AfrawyStore.sln -c Release
```

### Run
```bash
# Run from the Web project (contains Program.cs)
dotnet run --project AfrawyStore.Web/AfrawyStore.Web.csproj

# Default URL: https://localhost:5001 or http://localhost:5000
```

### Database Migrations (EF Core)
```bash
# Create new migration - run from Web project dir
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# Remove last migration (unapplied)
dotnet ef migrations remove
```

### Test (when test project exists)
```bash
# Run all tests
dotnet test

# Run single test (by name filter)
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run tests with detailed output
dotnet test -v detailed
```

### Additional Commands
```bash
# Restore packages
dotnet restore AfrawyStore.sln

# Clean build artifacts
dotnet clean AfrawyStore.sln

# List available commands
dotnet ef --help
```

---

## Code Style Guidelines

### Language Standard
- **C# 12 / .NET 8** with `Nullable` and `ImplicitUsings` enabled in all projects.
- Do NOT add `var` for concrete types when explicit typing improves readability.

### Project Structure (follow existing pattern)
```
AfrawyStore.Domain/           # Entities, Enums, Domain Interfaces only
AfrawyStore.Application/     # Services, DTOs, Service Interfaces, IUnitOfWork
AfrawyStore.Infrastructure/   # EF Core, Repositories, UnitOfWork impl
AfrawyStore.Web/            # Controllers, Views, wwwroot, ViewModels
```

### Namespace Conventions
```csharp
// Domain layer - no using statements from outer layers
namespace AfrawyStore.Domain.Entities;
namespace AfrawyStore.Domain.Enums;
namespace AfrawyStore.Domain.Interfaces;

// Application layer
namespace AfrawyStore.Application.DTOs;
namespace AfrawyStore.Application.Interfaces.Services;
namespace AfrawyStore.Application.Interfaces.Persistence;
namespace AfrawyStore.Application.Services;

// Infrastructure layer
namespace AfrawyStore.Infrastructure.Data;
namespace AfrawyStore.Infrastructure.Repositories;
namespace AfrawyStore.Infrastructure.UnitOfWork;
namespace AfrawyStore.Infrastructure.DependencyInjection;

// Web layer
namespace AfrawyStore.Web.Controllers;
namespace AfrawyStore.Web.ViewModels;
```

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Class / Interface | PascalCase | `ProductService`, `IProductRepository` |
| Method | PascalCase | `GetByIdAsync`, `CreateProductAsync` |
| Property / Field | PascalCase | `ProductName`, `_unitOfWork` |
| Parameter | camelCase | `productId`, `searchTerm` |
| Private field | `_camelCase` | `_unitOfWork`, `_context` |
| Interface prefix | `I` + PascalCase | `IProductService` |
| Async method suffix | `Async` | `GetProductsAsync` |
| DTO suffix | `Dto` | `ProductDto`, `CreateProductDto` |

### Import Organization
Order imports by category, then alphabetically:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Enums;
```

### Type Usage
- **Use nullable reference types** (`string?`, `Product?`) — nullable is enabled.
- **Use `decimal`** for all monetary values (prices, costs, totals).
- **Use `DateTime`** consistently — store as UTC in database (`DateTime.UtcNow`).
- **Use `int`** for IDs (auto-increment primary keys).
- **Use `bool`** for flags (`IsActive`, `IsDeleted`).

### Entity Design Pattern
```csharp
// Domain/Entities/Product.cs
namespace AfrawyStore.Domain.Entities;

public class Product : BaseEntity
{
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public decimal MinimumStock { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Category Category { get; set; } = null!;
    public Inventory Inventory { get; set; } = null!;
    public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
}
```

### Service Layer Pattern
```csharp
// Application/Services/ProductService.cs
namespace AfrawyStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            // ...
        };
    }
}
```

### Controller Pattern
```csharp
// Web/Controllers/ProductsController.cs
namespace AfrawyStore.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var result = await _productService.GetPagedProductsAsync(search, page, 20);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        // ...
    }
}
```

### Error Handling
- **DO:** Return `null` / `false` for not-found or validation failure in service methods.
- **DO:** Use `ModelState.AddModelError` in controllers for user-facing validation.
- **DO:** Log exceptions with meaningful context.
- **DO:** Return meaningful error messages in Arabic for the UI.
- **DON'T:** Throw generic `Exception` — throw specific exceptions or return result codes.

### Validation Rules (from PRD)
- SKU must be unique across all products.
- Selling price must be >= cost price.
- Inventory quantity cannot drop below zero.
- Category names must be unique.
- Usernames must be unique and at least 4 characters.
- All monetary values use 2 decimal places.

### Arabic RTL Conventions
- All user-facing strings in Arabic.
- Views use `dir="rtl"` and `lang="ar"`.
- Use Bootstrap RTL (`bootstrap.rtl.min.css`).
- Use Cairo font from Google Fonts.
- Number formatting: Egyptian pound (ج.م), right-aligned.

### Role-Based Access (from PRD)
| Feature | Admin | Employee |
|---------|-------|----------|
| Products CRUD | Yes | View only |
| Categories CRUD | Yes | No |
| Inventory Adjust | Yes | Stock-in only |
| Sales Void | Yes | No |
| Reports | Full | Limited |
| User Management | Yes | No |

### Business Rules to Preserve
- Products automatically create an Inventory record on creation.
- SaleItems store price/cost snapshot at time of sale.
- Voiding a sale restores inventory quantities.
- Low-stock alert when `CurrentStock <= MinimumStock`.
- SellingPrice must be >= CostPrice.

### Key Files and Patterns to Reference
- Domain entities: `AfrawyStore.Domain/Entities/*.cs`
- Service interfaces: `AfrawyStore.Application/Interfaces/Services/*.cs`
- Service implementations: `AfrawyStore.Application/Services/*.cs`
- Generic Repository: `AfrawyStore.Infrastructure/Repositories/GenericRepository.cs`
- Unit of Work: `AfrawyStore.Infrastructure/UnitOfWork/UnitOfWork.cs`
- DbContext: `AfrawyStore.Infrastructure/Data/AppDbContext.cs`
- View layout: `AfrawyStore.Web/Views/Shared/_Layout.cshtml`
- POS JavaScript: `AfrawyStore.Web/wwwroot/js/sales-pos.js`

### Common Anti-Patterns to Avoid
- DON'T add service references to Domain layer.
- DON'T use `ViewBag` / `ViewData` — use strongly-typed ViewModels.
- DON'T return anonymous objects from controllers — use DTOs.
- DON'T hardcode connection strings — use `appsettings.json`.
- DON'T skip `[ValidateAntiForgeryToken]` on POST actions.
- DON'T forget `await` on async methods.
- DON'T use `.Result` — use `await` instead.

---

## Quick Reference

| Task | Command |
|------|---------|
| Build solution | `dotnet build AfrawyStore.sln` |
| Run app | `dotnet run --project AfrawyStore.Web` |
| Add migration | `dotnet ef migrations add Name` |
| Update database | `dotnet ef database update` |
| Run tests | `dotnet test` |
| Single test | `dotnet test --filter "Name~TestName"` |

---

*Generated for agentic coding agents. Refer to `docs/PRD_AfrawyStore_EN.md` for full requirements.*