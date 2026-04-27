using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Enums;
using AfrawyStore.Domain.Interfaces;
using AfrawyStore.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace AfrawyStore.Infrastructure.Repositories;

public class SaleRepository : GenericRepository<Sale>, ISaleRepository
{
    public SaleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Sale?> GetSaleWithItemsAsync(int saleId)
    {
        return await _context.Set<Sale>()
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == saleId);
    }

    public async Task<(IEnumerable<Sale> Sales, int TotalCount)> GetPagedSalesAsync(
        string? searchTerm, DateTime? dateFrom, DateTime? dateTo,
        SaleStatus? status, int page, int pageSize)
    {
        var query = _context.Set<Sale>()
            .Include(s => s.CreatedBy)
            .Include(s => s.SaleItems)
            .AsQueryable();

        // Filter by search term (sale ID or note)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            if (int.TryParse(searchTerm, out int saleId))
            {
                query = query.Where(s => s.Id == saleId);
            }
            else
            {
                var lowerTerm = searchTerm.ToLower();
                query = query.Where(s => s.Note != null && s.Note.ToLower().Contains(lowerTerm));
            }
        }

        // Filter by date range
        if (dateFrom.HasValue)
        {
            query = query.Where(s => s.SaleDate >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            // Include the full end day
            var endOfDay = dateTo.Value.Date.AddDays(1);
            query = query.Where(s => s.SaleDate < endOfDay);
        }

        // Filter by status
        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var sales = await query
            .OrderByDescending(s => s.SaleDate)
            .ThenByDescending(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (sales, totalCount);
    }

    public async Task<List<Sale>> GetSalesSinceDateAsync(DateTime fromDate)
    {
        return await _context.Set<Sale>()
            .Where(s => s.Status == SaleStatus.Completed && s.SaleDate >= fromDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetLatestSalesAsync(int count)
    {
        return await _context.Set<Sale>()
            .Include(s => s.CreatedBy)
            .Include(s => s.SaleItems)
            .OrderByDescending(s => s.SaleDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesForReportAsync(DateTime? dateFrom, DateTime? dateTo)
    {
        var query = _context.Set<Sale>()
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                    .ThenInclude(p => p.Category)
            .Where(s => s.Status == SaleStatus.Completed)
            .AsQueryable();

        if (dateFrom.HasValue)
            query = query.Where(s => s.SaleDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(s => s.SaleDate < dateTo.Value.Date.AddDays(1));

        return await query.OrderBy(s => s.SaleDate).ToListAsync();
    }
}
