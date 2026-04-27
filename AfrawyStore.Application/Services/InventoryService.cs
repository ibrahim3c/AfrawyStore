using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Persistence;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<InventoryDto>> GetAllInventoryAsync()
    {
        var inventories = await _unitOfWork.Inventory.GetAllWithProductAsync();
        return inventories.Select(i => new InventoryDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            ProductSKU = i.Product.SKU,
            CategoryName = i.Product.Category?.Name ?? string.Empty,
            Unit = i.Product.Unit,
            CurrentStock = i.CurrentStock,
            MinimumStock = i.Product.MinimumStock,
            LastUpdated = i.LastUpdated
        }).ToList();
    }

    public async Task<InventoryAdjustDto?> GetAdjustFormAsync(int productId)
    {
        var inventory = await _unitOfWork.Inventory.GetByProductIdAsync(productId);
        if (inventory == null) return null;

        return new InventoryAdjustDto
        {
            ProductId = inventory.ProductId,
            ProductName = inventory.Product.Name,
            ProductSKU = inventory.Product.SKU,
            CurrentStock = inventory.CurrentStock,
            Unit = inventory.Product.Unit,
            MinimumStock = inventory.Product.MinimumStock,
            ChangeType = InventoryChangeType.StockIn,
            Quantity = 0
        };
    }

    public async Task<string> AdjustStockAsync(InventoryAdjustDto adjustDto, int userId)
    {
        var inventory = await _unitOfWork.Inventory.GetByProductIdAsync(adjustDto.ProductId);
        if (inventory == null)
            return "سجل المخزون غير موجود.";

        // Calculate new stock
        decimal newStock;
        if (adjustDto.ChangeType == InventoryChangeType.StockIn)
        {
            newStock = inventory.CurrentStock + adjustDto.Quantity;
        }
        else // Adjustment (can be negative correction)
        {
            newStock = inventory.CurrentStock - adjustDto.Quantity;
        }

        // Stock cannot drop below zero
        if (newStock < 0)
            return "لا يمكن أن يكون المخزون أقل من صفر. الكمية المتاحة: " + inventory.CurrentStock;

        // Update inventory
        inventory.CurrentStock = newStock;
        inventory.LastUpdated = DateTime.UtcNow;
        _unitOfWork.Inventory.Update(inventory);

        // Create log entry
        var log = new InventoryLog
        {
            ProductId = adjustDto.ProductId,
            ChangeType = adjustDto.ChangeType,
            QuantityChange = adjustDto.ChangeType == InventoryChangeType.StockIn ? adjustDto.Quantity : -adjustDto.Quantity,
            Note = adjustDto.Note,
            CreatedById = userId
        };
        await _unitOfWork.InventoryLogs.AddAsync(log);

        var success = await _unitOfWork.SaveChangesAsync() > 0;
        return success ? string.Empty : "حدث خطأ أثناء تحديث المخزون.";
    }

    public async Task<IEnumerable<InventoryLogDto>> GetLogsAsync(int productId)
    {
        var logs = await _unitOfWork.InventoryLogs.GetLogsByProductIdAsync(productId);
        return logs.Select(l => new InventoryLogDto
        {
            Id = l.Id,
            ChangeType = l.ChangeType,
            QuantityChange = l.QuantityChange,
            Note = l.Note,
            CreatedByName = l.CreatedBy?.FullName ?? "غير معروف",
            CreatedAt = DateTime.UtcNow // InventoryLog uses BaseEntity which doesn't have a timestamp; we use Id ordering
        }).ToList();
    }

    public async Task<int> GetLowStockCountAsync()
    {
        return await _unitOfWork.Inventory.GetLowStockCountAsync();
    }
}
