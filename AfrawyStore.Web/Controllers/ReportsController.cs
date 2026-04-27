using System.Text;
using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Services;
using AfrawyStore.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // ─────────────────────────────────────────────────────────
    // GET /Reports  — Main tab-based page
    // ─────────────────────────────────────────────────────────
    public async Task<IActionResult> Index(
        string tab = "sales",
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int? categoryId = null)
    {
        var filter = new ReportFilterDto
        {
            ReportType = tab,
            DateFrom   = dateFrom,
            DateTo     = dateTo,
            CategoryId = categoryId,
        };

        ViewBag.ActiveTab  = tab;
        ViewBag.DateFrom   = dateFrom?.ToString("yyyy-MM-dd");
        ViewBag.DateTo     = dateTo?.ToString("yyyy-MM-dd");
        ViewBag.IsAdmin    = User.IsInRole("Admin");

        // Load the data for the active tab
        switch (tab)
        {
            case "inventory":
                if (!User.IsInRole("Admin")) return Forbid();
                ViewBag.InventoryData = await _reportService.GetInventoryReportAsync();
                break;

            case "profit":
                if (!User.IsInRole("Admin")) return Forbid();
                ViewBag.ProfitData = await _reportService.GetProfitLossReportAsync(filter);
                break;

            case "lowstock":
                if (!User.IsInRole("Admin")) return Forbid();
                ViewBag.LowStockData = await _reportService.GetLowStockReportAsync();
                break;

            default: // "sales" — accessible to Employee too
                ViewBag.SalesData = await _reportService.GetSalesReportAsync(filter);
                break;
        }

        return View(filter);
    }

    // ─────────────────────────────────────────────────────────
    // 7.8  CSV Export — GET /Reports/ExportCsv
    // ─────────────────────────────────────────────────────────
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportCsv(
        string tab = "sales",
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var filter = new ReportFilterDto { DateFrom = dateFrom, DateTo = dateTo };
        var bom    = Encoding.UTF8.GetPreamble(); // UTF-8 BOM for correct Arabic in Excel

        switch (tab)
        {
            case "inventory":
            {
                var data = await _reportService.GetInventoryReportAsync();
                var csv  = BuildInventoryCsv(data);
                var bytes = bom.Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
                return File(bytes, "text/csv; charset=utf-8", "inventory_report.csv");
            }
            case "profit":
            {
                var data = await _reportService.GetProfitLossReportAsync(filter);
                var csv  = BuildProfitCsv(data);
                var bytes = bom.Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
                return File(bytes, "text/csv; charset=utf-8", "profit_loss_report.csv");
            }
            case "lowstock":
            {
                var data = await _reportService.GetLowStockReportAsync();
                var csv  = BuildLowStockCsv(data);
                var bytes = bom.Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
                return File(bytes, "text/csv; charset=utf-8", "lowstock_report.csv");
            }
            default: // sales
            {
                var data = await _reportService.GetSalesReportAsync(filter);
                var csv  = BuildSalesCsv(data);
                var bytes = bom.Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
                return File(bytes, "text/csv; charset=utf-8", "sales_report.csv");
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    // 7.9  PDF Export — GET /Reports/ExportPdf
    // ─────────────────────────────────────────────────────────
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportPdf(
        string tab = "sales",
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var filter = new ReportFilterDto { DateFrom = dateFrom, DateTo = dateTo };

        byte[] pdfBytes = tab switch
        {
            "inventory" => PdfExportHelper.BuildInventoryPdf(
                              await _reportService.GetInventoryReportAsync()),
            "profit"    => PdfExportHelper.BuildProfitPdf(
                              await _reportService.GetProfitLossReportAsync(filter), filter),
            "lowstock"  => PdfExportHelper.BuildLowStockPdf(
                              await _reportService.GetLowStockReportAsync()),
            _           => PdfExportHelper.BuildSalesPdf(
                              await _reportService.GetSalesReportAsync(filter), filter),
        };

        var fileName = $"{tab}_report_{DateTime.Today:yyyyMMdd}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

    // ─────────────────────────────────────────────────────────
    // CSV Builders
    // ─────────────────────────────────────────────────────────
    private static string BuildSalesCsv(SalesReportDto data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("التاريخ,عدد العمليات,الإيرادات,الربح,الخصم");
        foreach (var row in data.DailyRows)
        {
            sb.AppendLine($"{row.DateLabel},{row.SaleCount},{row.Revenue:F2},{row.Profit:F2},{row.Discount:F2}");
        }
        sb.AppendLine($"الإجمالي,{data.TotalSalesCount},{data.TotalRevenue:F2},{data.TotalProfit:F2},{data.TotalDiscount:F2}");
        return sb.ToString();
    }

    private static string BuildInventoryCsv(IEnumerable<InventoryReportDto> data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("المنتج,الكود,الفئة,الوحدة,المخزون الحالي,الحد الأدنى,القيمة,الحالة");
        foreach (var row in data)
        {
            sb.AppendLine($"{row.ProductName},{row.SKU},{row.CategoryName},{row.Unit}," +
                          $"{row.CurrentStock:F2},{row.MinimumStock:F2},{row.StockValue:F2},{row.StockStatusLabel}");
        }
        return sb.ToString();
    }

    private static string BuildProfitCsv(ProfitLossReportDto data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("الفئة,الإيرادات,التكلفة,الربح,هامش الربح%");
        foreach (var row in data.CategoryRows)
        {
            sb.AppendLine($"{row.Name},{row.Revenue:F2},{row.Cost:F2},{row.Profit:F2},{row.MarginPercent:F1}%");
        }
        sb.AppendLine($"الإجمالي,{data.TotalRevenue:F2},{data.TotalCost:F2},{data.GrossProfit:F2},{data.ProfitMarginPercent:F1}%");
        return sb.ToString();
    }

    private static string BuildLowStockCsv(IEnumerable<LowStockReportDto> data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("المنتج,الكود,الفئة,الوحدة,المخزون الحالي,الحد الأدنى,العجز,الحالة");
        foreach (var row in data)
        {
            sb.AppendLine($"{row.ProductName},{row.SKU},{row.CategoryName},{row.Unit}," +
                          $"{row.CurrentStock:F2},{row.MinimumStock:F2},{row.Deficit:F2},{row.StatusLabel}");
        }
        return sb.ToString();
    }
}
