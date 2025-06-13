using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WorkshopManager.Data;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;
using QuestPDF.Infrastructure;


namespace WorkshopManager.Services;

public class ReportService : IReportService
{
    private readonly WorkshopDbContext _db;

    public ReportService(WorkshopDbContext db) => _db = db;

    /* ■■■  dane do tabeli  ■■■ */
    public async Task<IEnumerable<ReportRowDto>> GetAsync(
        int? customerId, int? vehicleId, int? year, int? month)
    {
        var q = _db.ServiceOrders
                   .Include(o => o.Customer)
                   .Include(o => o.Vehicle)
                   .Include(o => o.Tasks)
                   .Include(o => o.UsedParts)
                   .AsQueryable();

        if (customerId is not null) q = q.Where(o => o.CustomerId == customerId);
        if (vehicleId  is not null) q = q.Where(o => o.VehicleId  == vehicleId);
        if (year  is not null)      q = q.Where(o => o.CreatedAt.Year  == year);
        if (month is not null)      q = q.Where(o => o.CreatedAt.Month == month);

        return await q.OrderBy(o => o.CreatedAt)
                    .Select(o => new ReportRowDto(
                        o.CreatedAt,
                        o.Customer.FullName,
                        o.Vehicle.RegistrationNumber,
                        o.Tasks.Sum(t => t.Price),
                        o.UsedParts.Sum(p => p.Quantity * p.UnitPrice),
                        o.Tasks.Sum(t => t.Price) +
                        o.UsedParts.Sum(p => p.Quantity * p.UnitPrice)))

                      .ToListAsync();
    }

    /* ■■■  PDF  ■■■ */
    public async Task<byte[]> ExportPdfAsync(IEnumerable<ReportRowDto> rows,
                                             string title)
    {
        var data = rows.ToList();

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Header().Text(title).FontSize(18).Bold();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(70);   // data
                        c.RelativeColumn(2);    // klient
                        c.RelativeColumn(1);    // pojazd
                        c.ConstantColumn(60);   // roboc.
                        c.ConstantColumn(60);   // części
                        c.ConstantColumn(70);   // razem
                    });

                    // nagłówek
                    table.Header(h =>
                    {
                        h.Cell().Element(StyleHeader).Text("Data");
                        h.Cell().Element(StyleHeader).Text("Klient");
                        h.Cell().Element(StyleHeader).Text("Pojazd");
                        h.Cell().Element(StyleHeader).AlignRight().Text("Roboc.");
                        h.Cell().Element(StyleHeader).AlignRight().Text("Części");
                        h.Cell().Element(StyleHeader).AlignRight().Text("Razem");
                    });

                    // wiersze
                    foreach (var r in data)
                    {
                        table.Cell().Text(r.Date.ToString("yyyy-MM-dd"));
                        table.Cell().Text(r.Customer);
                        table.Cell().Text(r.Vehicle);
                        table.Cell().AlignRight().Text(r.Labor.ToString("C"));
                        table.Cell().AlignRight().Text(r.Parts.ToString("C"));
                        table.Cell().AlignRight().Text(r.Total.ToString("C"));
                    }

                    // suma
                    table.Footer(f =>
                    {
                        var sumLab  = data.Sum(x => x.Labor);
                        var sumPart = data.Sum(x => x.Parts);
                        var sumTot  = data.Sum(x => x.Total);

                        f.Cell().ColumnSpan(3).AlignRight().Text("Suma:");
                        f.Cell().AlignRight().Text(sumLab.ToString("C"));
                        f.Cell().AlignRight().Text(sumPart.ToString("C"));
                        f.Cell().AlignRight().Text(sumTot.ToString("C"));
                    });

                    IContainer StyleHeader(IContainer c) =>
                        c.DefaultTextStyle(s => s.SemiBold()).Background(Colors.Grey.Lighten3);
                });
            });
        });

        return doc.GeneratePdf();
    }
}
