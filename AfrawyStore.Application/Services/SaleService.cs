using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Application.Services;

public class SaleService : ISaleService
{
    private readonly IUnitOfWork _unitOfWork;

    public SaleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Search active products with stock info for POS product picker.
    /// Returns only active products. Out-of-stock products are included but flagged.
    /// </summary>
    public async Task<IEnumerable<PosProductDto>> SearchProductsForPosAsync(string? term)
    {
        var inventories = await _unitOfWork.Inventory.GetAllWithProductAsync();

        var results = inventories
            .Where(i => i.Product.IsActive);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var lowerTerm = term.ToLower();
            results = results.Where(i =>
                i.Product.Name.ToLower().Contains(lowerTerm) ||
                i.Product.SKU.ToLower().Contains(lowerTerm));
        }

        return results.Select(i => new PosProductDto
        {
            Id = i.Product.Id,
            Name = i.Product.Name,
            SKU = i.Product.SKU,
            SellingPrice = i.Product.SellingPrice,
            CostPrice = i.Product.CostPrice,
            CurrentStock = i.CurrentStock,
            Unit = i.Product.Unit,
            ImagePath = i.Product.ImagePath
        }).ToList();
    }

    /// <summary>
    /// Create a new sale: validate stock, snapshot prices, deduct inventory, log changes.
    /// Returns (SaleId, null) on success or (null, errorMessage) on failure.
    /// </summary>
    public async Task<(int? SaleId, string? Error)> CreateSaleAsync(CreateSaleDto dto, int userId)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            return (null, "يجب إضافة منتج واحد على الأقل.");

        // Gather all products and their inventory
        var saleItems = new List<SaleItem>();
        decimal subTotal = 0;
        decimal totalProfit = 0;

        foreach (var item in dto.Items)
        {
            var inventory = await _unitOfWork.Inventory.GetByProductIdAsync(item.ProductId);
            if (inventory == null)
                return (null, $"المنتج برقم {item.ProductId} غير موجود في المخزون.");

            if (!inventory.Product.IsActive)
                return (null, $"المنتج '{inventory.Product.Name}' غير نشط.");

            // 5.6: Stock availability check
            if (inventory.CurrentStock < item.Quantity)
                return (null, $"الكمية المطلوبة من '{inventory.Product.Name}' ({item.Quantity}) أكبر من المتوفر ({inventory.CurrentStock}).");

            // 5.9: Snapshot prices at sale time
            var unitPrice = inventory.Product.SellingPrice;
            var unitCost = inventory.Product.CostPrice;
            var lineTotal = unitPrice * item.Quantity;
            var lineProfit = (unitPrice - unitCost) * item.Quantity;

            saleItems.Add(new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                UnitCost = unitCost,
                LineTotal = lineTotal,
                LineProfit = lineProfit
            });

            subTotal += lineTotal;
            totalProfit += lineProfit;
        }

        // 5.10: Apply discount
        var discount = dto.Discount;
        if (discount > subTotal)
            return (null, "الخصم لا يمكن أن يكون أكبر من إجمالي المبلغ.");

        var totalAmount = subTotal - discount;
        totalProfit -= discount;

        // Create the sale entity
        var sale = new Sale
        {
            SaleDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            TotalProfit = totalProfit,
            Discount = discount,
            PaymentMethod = dto.PaymentMethod,
            Status = SaleStatus.Completed,
            CreatedById = userId,
            Note = dto.Note,
            SaleItems = saleItems
        };

        await _unitOfWork.Sales.AddAsync(sale);

        // 5.7: Deduct inventory for each item
        foreach (var item in dto.Items)
        {
            var inventory = await _unitOfWork.Inventory.GetByProductIdAsync(item.ProductId);
            if (inventory == null) continue;

            inventory.CurrentStock -= item.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;
            _unitOfWork.Inventory.Update(inventory);

            // Log the inventory change
            var log = new InventoryLog
            {
                ProductId = item.ProductId,
                ChangeType = InventoryChangeType.Sale,
                QuantityChange = -item.Quantity,
                Note = $"بيع - فاتورة جديدة",
                CreatedById = userId
            };
            await _unitOfWork.InventoryLogs.AddAsync(log);
        }

        var saved = await _unitOfWork.SaveChangesAsync();
        if (saved > 0)
            return (sale.Id, null);

        return (null, "حدث خطأ أثناء حفظ عملية البيع.");
    }

    /// <summary>
    /// Void a completed sale: restore inventory, update status.
    /// Returns null on success or error message on failure.
    /// </summary>
    public async Task<string?> VoidSaleAsync(int saleId, int userId)
    {
        var sale = await _unitOfWork.Sales.GetSaleWithItemsAsync(saleId);
        if (sale == null)
            return "عملية البيع غير موجودة.";

        if (sale.Status == SaleStatus.Voided)
            return "عملية البيع ملغاة بالفعل.";

        // Mark as voided
        sale.Status = SaleStatus.Voided;
        _unitOfWork.Sales.Update(sale);

        // 5.8: Restore inventory for each item
        foreach (var saleItem in sale.SaleItems)
        {
            var inventory = await _unitOfWork.Inventory.GetByProductIdAsync(saleItem.ProductId);
            if (inventory == null) continue;

            inventory.CurrentStock += saleItem.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;
            _unitOfWork.Inventory.Update(inventory);

            // Log the inventory restoration
            var log = new InventoryLog
            {
                ProductId = saleItem.ProductId,
                ChangeType = InventoryChangeType.Adjustment,
                QuantityChange = saleItem.Quantity,
                Note = $"إلغاء بيع - فاتورة #{saleId}",
                CreatedById = userId
            };
            await _unitOfWork.InventoryLogs.AddAsync(log);
        }

        var saved = await _unitOfWork.SaveChangesAsync();
        return saved > 0 ? null : "حدث خطأ أثناء إلغاء عملية البيع.";
    }

    /// <summary>
    /// Get full sale detail with items for receipt view.
    /// </summary>
    public async Task<SaleDetailDto?> GetSaleDetailAsync(int saleId)
    {
        var sale = await _unitOfWork.Sales.GetSaleWithItemsAsync(saleId);
        if (sale == null) return null;

        var subTotal = sale.SaleItems.Sum(si => si.LineTotal);

        return new SaleDetailDto
        {
            Id = sale.Id,
            SaleDate = sale.SaleDate,
            SubTotal = subTotal,
            TotalAmount = sale.TotalAmount,
            TotalProfit = sale.TotalProfit,
            Discount = sale.Discount,
            PaymentMethod = sale.PaymentMethod,
            Status = sale.Status,
            CreatedByName = sale.CreatedBy?.FullName ?? "غير معروف",
            Note = sale.Note,
            Items = sale.SaleItems.Select(si => new SaleItemDto
            {
                Id = si.Id,
                ProductId = si.ProductId,
                ProductName = si.Product?.Name ?? "منتج محذوف",
                ProductSKU = si.Product?.SKU ?? "-",
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                UnitCost = si.UnitCost,
                LineTotal = si.LineTotal,
                LineProfit = si.LineProfit
            }).ToList()
        };
    }

    /// <summary>
    /// Get paginated/filtered sales list.
    /// </summary>
    public async Task<(IEnumerable<SaleDto> Sales, int TotalCount)> GetPagedSalesAsync(SalesFilterDto filter)
    {
        var (sales, totalCount) = await _unitOfWork.Sales.GetPagedSalesAsync(
            filter.SearchTerm, filter.DateFrom, filter.DateTo,
            filter.Status, filter.Page, filter.PageSize);

        var dtos = sales.Select(s => new SaleDto
        {
            Id = s.Id,
            SaleDate = s.SaleDate,
            TotalAmount = s.TotalAmount,
            TotalProfit = s.TotalProfit,
            Discount = s.Discount,
            PaymentMethod = s.PaymentMethod,
            Status = s.Status,
            CreatedByName = s.CreatedBy?.FullName ?? "غير معروف",
            ItemCount = s.SaleItems.Count,
            Note = s.Note
        }).ToList();

        return (dtos, totalCount);
    }
}
