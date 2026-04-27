namespace AfrawyStore.Application.DTOs;

// ─────────────────────────────────────────────────────────────
// Shared filter model used by Sales and Profit/Loss reports
// ─────────────────────────────────────────────────────────────
public class ReportFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo   { get; set; }
    /// <summary>Optional: filter by category (Profit/Loss report)</summary>
    public int? CategoryId    { get; set; }
    /// <summary>"sales" | "inventory" | "profit" | "lowstock"</summary>
    public string ReportType  { get; set; } = "sales";
}

// ─────────────────────────────────────────────────────────────
// 7.4  Sales Report
// ─────────────────────────────────────────────────────────────
public class SalesReportDto
{
    // Summary header cards
    public int     TotalSalesCount  { get; set; }
    public decimal TotalRevenue     { get; set; }
    public decimal TotalProfit      { get; set; }
    public decimal TotalDiscount    { get; set; }
    public decimal AverageSaleValue { get; set; }

    // Daily breakdown table rows
    public List<SalesReportRowDto> DailyRows { get; set; } = new();

    // Payment method breakdown
    public int CashCount  { get; set; }
    public int CardCount  { get; set; }
    public int OtherCount { get; set; }

    // Computed
    public decimal ProfitMarginPercent =>
        TotalRevenue > 0 ? Math.Round(TotalProfit / TotalRevenue * 100, 1) : 0;
}

public class SalesReportRowDto
{
    public DateTime Date       { get; set; }
    public string   DateLabel  { get; set; } = string.Empty;   // Arabic formatted
    public int      SaleCount  { get; set; }
    public decimal  Revenue    { get; set; }
    public decimal  Profit     { get; set; }
    public decimal  Discount   { get; set; }
}

// ─────────────────────────────────────────────────────────────
// 7.5  Inventory Status Report
// ─────────────────────────────────────────────────────────────
public class InventoryReportDto
{
    public int     ProductId      { get; set; }
    public string  ProductName    { get; set; } = string.Empty;
    public string  SKU            { get; set; } = string.Empty;
    public string  CategoryName   { get; set; } = string.Empty;
    public string  Unit           { get; set; } = string.Empty;
    public decimal CurrentStock   { get; set; }
    public decimal MinimumStock   { get; set; }
    public decimal CostPrice      { get; set; }
    public decimal SellingPrice   { get; set; }
    public DateTime LastUpdated   { get; set; }

    /// <summary>"ok" | "low" | "out"</summary>
    public string StockStatus =>
        CurrentStock <= 0 ? "out" :
        CurrentStock <= MinimumStock ? "low" : "ok";

    public string StockStatusLabel =>
        CurrentStock <= 0 ? "نفذ" :
        CurrentStock <= MinimumStock ? "منخفض" : "جيد";

    /// <summary>Total stock value at cost price</summary>
    public decimal StockValue => CurrentStock * CostPrice;
}

// ─────────────────────────────────────────────────────────────
// 7.6  Profit & Loss Report
// ─────────────────────────────────────────────────────────────
public class ProfitLossReportDto
{
    // Header summary
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost    { get; set; }
    public decimal GrossProfit  { get; set; }
    public decimal TotalDiscount { get; set; }
    public int     TotalSales   { get; set; }

    public decimal ProfitMarginPercent =>
        TotalRevenue > 0 ? Math.Round(GrossProfit / TotalRevenue * 100, 1) : 0;

    // Breakdown by category
    public List<ProfitLossRowDto> CategoryRows  { get; set; } = new();
    // Breakdown by product (top 20)
    public List<ProfitLossRowDto> ProductRows   { get; set; } = new();
}

public class ProfitLossRowDto
{
    public int     Id              { get; set; }
    public string  Name            { get; set; } = string.Empty;
    public decimal TotalQuantity   { get; set; }
    public decimal Revenue         { get; set; }
    public decimal Cost            { get; set; }
    public decimal Profit          { get; set; }
    public decimal MarginPercent   =>
        Revenue > 0 ? Math.Round(Profit / Revenue * 100, 1) : 0;
}

// ─────────────────────────────────────────────────────────────
// 7.7  Low-Stock Report
// ─────────────────────────────────────────────────────────────
public class LowStockReportDto
{
    public int     ProductId    { get; set; }
    public string  ProductName  { get; set; } = string.Empty;
    public string  SKU          { get; set; } = string.Empty;
    public string  CategoryName { get; set; } = string.Empty;
    public string  Unit         { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal Deficit      => Math.Max(0, MinimumStock - CurrentStock);

    /// <summary>"out" (stock == 0) or "low" (stock > 0 but ≤ minimum)</summary>
    public string Status =>
        CurrentStock <= 0 ? "out" : "low";

    public string StatusLabel =>
        CurrentStock <= 0 ? "نفذ من المخزون" : "مخزون منخفض";
}
