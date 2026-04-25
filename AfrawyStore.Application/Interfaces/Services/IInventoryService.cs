using AfrawyStore.Application.DTOs;

namespace AfrawyStore.Application.Interfaces.Services;

public interface IInventoryService
{
    Task<IEnumerable<InventoryDto>> GetAllInventoryAsync();
    Task<InventoryAdjustDto?> GetAdjustFormAsync(int productId);
    Task<string> AdjustStockAsync(InventoryAdjustDto adjustDto, int userId);
    Task<IEnumerable<InventoryLogDto>> GetLogsAsync(int productId);
    Task<int> GetLowStockCountAsync();
}
