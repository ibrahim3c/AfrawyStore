using AfrawyStore.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AfrawyStore.Web.Helpers;

/// <summary>
/// Builds PDF reports using QuestPDF (MIT licence, pure .NET, no native dependencies).
/// All documents are RTL with Arabic-compatible font (Helvetica fallback — replace with
/// an embedded Arabic font file if full shaping is required in production).
/// </summary>
public static class PdfExportHelper
{
    static PdfExportHelper()
    {
        // Community / non-commercial licence — set once at app start.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // ─────────────────────────────────────────────────────────────
    // Sales
    // ─────────────────────────────────────────────────────────────
    public static byte[] BuildSalesPdf(SalesReportDto data, ReportFilterDto filter)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => BuildHeader(c,
                    "تقرير المبيعات",
                    filter.DateFrom, filter.DateTo));

                page.Content().Column(col =>
                {
                    // Summary cards row
                    col.Item().Row(row =>
                    {
                        SummaryCard(row, "إجمالي العمليات", data.TotalSalesCount.ToString());
                        SummaryCard(row, "الإيرادات",       $"{data.TotalRevenue:F2} ج.م");
                        SummaryCard(row, "الربح",           $"{data.TotalProfit:F2} ج.م");
                        SummaryCard(row, "هامش الربح",      $"{data.ProfitMarginPercent:F1}%");
                    });

                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(2); // date
                            cols.RelativeColumn(1); // count
                            cols.RelativeColumn(2); // revenue
                            cols.RelativeColumn(2); // profit
                            cols.RelativeColumn(2); // discount
                        });

                        // Header
                        static IContainer CellHeader(IContainer c) =>
                            c.Background("#1e293b").Padding(5).AlignRight();
                        static IContainer CellBody(IContainer c, int row) =>
                            c.Background(row % 2 == 0 ? "#f8fafc" : "#ffffff").Padding(5).AlignRight();

                        table.Header(h =>
                        {
                            h.Cell().Element(CellHeader).Text("التاريخ")    .FontColor("#ffffff").Bold();
                            h.Cell().Element(CellHeader).Text("العمليات")  .FontColor("#ffffff").Bold();
                            h.Cell().Element(CellHeader).Text("الإيرادات") .FontColor("#ffffff").Bold();
                            h.Cell().Element(CellHeader).Text("الربح")     .FontColor("#ffffff").Bold();
                            h.Cell().Element(CellHeader).Text("الخصم")     .FontColor("#ffffff").Bold();
                        });

                        int rowIdx = 0;
                        foreach (var r in data.DailyRows)
                        {
                            int ri = rowIdx++;
                            table.Cell().Element(c => CellBody(c, ri)).Text(r.DateLabel);
                            table.Cell().Element(c => CellBody(c, ri)).Text(r.SaleCount.ToString());
                            table.Cell().Element(c => CellBody(c, ri)).Text($"{r.Revenue:F2}");
                            table.Cell().Element(c => CellBody(c, ri)).Text($"{r.Profit:F2}");
                            table.Cell().Element(c => CellBody(c, ri)).Text($"{r.Discount:F2}");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("تقرير AFRAWY STORE — صفحة ");
                    t.CurrentPageNumber();
                    t.Span(" من ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    // ─────────────────────────────────────────────────────────────
    // Inventory
    // ─────────────────────────────────────────────────────────────
    public static byte[] BuildInventoryPdf(IEnumerable<InventoryReportDto> data)
    {
        var rows = data.ToList();
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(c => BuildHeader(c, "تقرير حالة المخزون", null, null));

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                    });

                    static IContainer H(IContainer c) =>
                        c.Background("#1e293b").Padding(4).AlignRight();
                    IContainer B(IContainer c, int r, string status) =>
                        c.Background(status == "out" ? "#fee2e2" : status == "low" ? "#fef9c3" : (r % 2 == 0 ? "#f8fafc" : "#ffffff"))
                         .Padding(4).AlignRight();

                    table.Header(h =>
                    {
                        h.Cell().Element(H).Text("المنتج")      .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الكود")       .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الفئة")       .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الوحدة")      .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الحالي")      .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الأدنى")      .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الحالة")      .FontColor("#fff").Bold();
                    });

                    int ri = 0;
                    foreach (var r in rows)
                    {
                        int i = ri++;
                        string s = r.StockStatus;
                        table.Cell().Element(c => B(c, i, s)).Text(r.ProductName);
                        table.Cell().Element(c => B(c, i, s)).Text(r.SKU);
                        table.Cell().Element(c => B(c, i, s)).Text(r.CategoryName);
                        table.Cell().Element(c => B(c, i, s)).Text(r.Unit);
                        table.Cell().Element(c => B(c, i, s)).Text($"{r.CurrentStock:F2}");
                        table.Cell().Element(c => B(c, i, s)).Text($"{r.MinimumStock:F2}");
                        table.Cell().Element(c => B(c, i, s)).Text(r.StockStatusLabel);
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("تقرير AFRAWY STORE — صفحة ");
                    t.CurrentPageNumber();
                    t.Span(" من ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    // ─────────────────────────────────────────────────────────────
    // Profit & Loss
    // ─────────────────────────────────────────────────────────────
    public static byte[] BuildProfitPdf(ProfitLossReportDto data, ReportFilterDto filter)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => BuildHeader(c,
                    "تقرير الأرباح والخسائر", filter.DateFrom, filter.DateTo));

                page.Content().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        SummaryCard(row, "الإيرادات",    $"{data.TotalRevenue:F2}");
                        SummaryCard(row, "التكلفة",      $"{data.TotalCost:F2}");
                        SummaryCard(row, "الربح الإجمالي", $"{data.GrossProfit:F2}");
                        SummaryCard(row, "هامش الربح",  $"{data.ProfitMarginPercent:F1}%");
                    });

                    col.Item().PaddingTop(10).Text("حسب الفئة").Bold().FontSize(11);
                    col.Item().PaddingTop(5).Table(table =>
                        BuildProfitTable(table, data.CategoryRows));

                    col.Item().PaddingTop(14).Text("أفضل المنتجات (أعلى 20)").Bold().FontSize(11);
                    col.Item().PaddingTop(5).Table(table =>
                        BuildProfitTable(table, data.ProductRows));
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("تقرير AFRAWY STORE — صفحة ");
                    t.CurrentPageNumber();
                    t.Span(" من ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    // ─────────────────────────────────────────────────────────────
    // Low-Stock
    // ─────────────────────────────────────────────────────────────
    public static byte[] BuildLowStockPdf(IEnumerable<LowStockReportDto> data)
    {
        var rows = data.ToList();
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => BuildHeader(c, "تقرير المخزون المنخفض", null, null));

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1);
                    });

                    static IContainer H(IContainer c) =>
                        c.Background("#1e293b").Padding(5).AlignRight();
                    IContainer B(IContainer c, int r, string s) =>
                        c.Background(s == "out" ? "#fee2e2" : "#fef9c3")
                         .Padding(5).AlignRight();

                    table.Header(h =>
                    {
                        h.Cell().Element(H).Text("المنتج")    .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الكود")     .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الفئة")     .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الوحدة")    .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الحالي")    .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("الأدنى")    .FontColor("#fff").Bold();
                        h.Cell().Element(H).Text("العجز")     .FontColor("#fff").Bold();
                    });

                    int ri = 0;
                    foreach (var r in rows)
                    {
                        int i = ri++;
                        string s = r.Status;
                        table.Cell().Element(c => B(c, i, s)).Text(r.ProductName);
                        table.Cell().Element(c => B(c, i, s)).Text(r.SKU);
                        table.Cell().Element(c => B(c, i, s)).Text(r.CategoryName);
                        table.Cell().Element(c => B(c, i, s)).Text(r.Unit);
                        table.Cell().Element(c => B(c, i, s)).Text($"{r.CurrentStock:F2}");
                        table.Cell().Element(c => B(c, i, s)).Text($"{r.MinimumStock:F2}");
                        table.Cell().Element(c => B(c, i, s)).Text($"{r.Deficit:F2}");
                    }
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("تقرير AFRAWY STORE — صفحة ");
                    t.CurrentPageNumber();
                    t.Span(" من ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    // ─────────────────────────────────────────────────────────────
    // Shared helpers
    // ─────────────────────────────────────────────────────────────
    private static void BuildHeader(IContainer container, string title,
        DateTime? dateFrom, DateTime? dateTo)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text("AFRAWY STORE")
                    .Bold().FontSize(14).FontColor("#1e293b");
                row.ConstantItem(200).AlignRight().Text(
                    $"تاريخ الطباعة: {DateTime.Today:dd/MM/yyyy}")
                    .FontSize(9).FontColor("#64748b");
            });

            col.Item().Text(title).Bold().FontSize(16);

            if (dateFrom.HasValue || dateTo.HasValue)
            {
                var from = dateFrom?.ToString("dd/MM/yyyy") ?? "—";
                var to   = dateTo?.ToString("dd/MM/yyyy")   ?? "—";
                col.Item().Text($"الفترة: {from} إلى {to}")
                    .FontSize(9).FontColor("#64748b");
            }

            col.Item().PaddingTop(4).LineHorizontal(1).LineColor("#e2e8f0");
        });
    }

    private static void SummaryCard(RowDescriptor row, string label, string value)
    {
        row.RelativeItem().Border(1).BorderColor("#e2e8f0")
           .Background("#f8fafc").Padding(8).Column(col =>
           {
               col.Item().Text(label).FontSize(8).FontColor("#64748b");
               col.Item().Text(value).Bold().FontSize(12);
           });
    }

    private static void BuildProfitTable(TableDescriptor table, List<ProfitLossRowDto> rows)
    {
        table.ColumnsDefinition(cols =>
        {
            cols.RelativeColumn(2);
            cols.RelativeColumn(2);
            cols.RelativeColumn(2);
            cols.RelativeColumn(2);
            cols.RelativeColumn(1);
        });

        static IContainer H(IContainer c) =>
            c.Background("#1e293b").Padding(5).AlignRight();
        static IContainer B(IContainer c, int r) =>
            c.Background(r % 2 == 0 ? "#f8fafc" : "#ffffff").Padding(5).AlignRight();

        table.Header(h =>
        {
            h.Cell().Element(H).Text("الاسم")       .FontColor("#fff").Bold();
            h.Cell().Element(H).Text("الإيرادات")   .FontColor("#fff").Bold();
            h.Cell().Element(H).Text("التكلفة")     .FontColor("#fff").Bold();
            h.Cell().Element(H).Text("الربح")       .FontColor("#fff").Bold();
            h.Cell().Element(H).Text("الهامش%")     .FontColor("#fff").Bold();
        });

        int ri = 0;
        foreach (var r in rows)
        {
            int i = ri++;
            table.Cell().Element(c => B(c, i)).Text(r.Name);
            table.Cell().Element(c => B(c, i)).Text($"{r.Revenue:F2}");
            table.Cell().Element(c => B(c, i)).Text($"{r.Cost:F2}");
            table.Cell().Element(c => B(c, i)).Text($"{r.Profit:F2}");
            table.Cell().Element(c => B(c, i)).Text($"{r.MarginPercent:F1}%");
        }
    }
}
