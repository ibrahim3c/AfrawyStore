namespace AfrawyStore.Application.DTOs;

/// <summary>
/// Main view model passed to the Dashboard/Index view.
/// </summary>
public class DashboardViewModel
{
    // ── Summary Cards ──────────────────────────────────
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public decimal TodaySales { get; set; }
    public int TodaySalesCount { get; set; }
    public int LowStockCount { get; set; }

    // ── Chart Data ─────────────────────────────────────
    /// <summary>Last 7 days of sales — one entry per day, ordered oldest→newest.</summary>
    public List<DailySalesDto> Last7DaysSales { get; set; } = new();

    // ── Transactions Table ─────────────────────────────
    /// <summary>5 most recent sales for the "Latest Operations" table.</summary>
    public List<SaleDto> LatestSales { get; set; } = new();

    // ── Low Stock Panel ────────────────────────────────
    public List<LowStockItemDto> LowStockItems { get; set; } = new();
}

/// <summary>
/// One bar in the sales chart (one calendar day).
/// </summary>
public class DailySalesDto
{
    public DateTime Date { get; set; }
    /// <summary>Arabic short day name, e.g. "السبت"</summary>
    public string DayLabel { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

/// <summary>
/// Row in the low-stock alert panel.
/// </summary>
public class LowStockItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public string Unit { get; set; } = string.Empty;
}
