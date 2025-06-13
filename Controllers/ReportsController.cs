using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IReportService    _reports;
    private readonly ICustomerService  _customers;
    private readonly IVehicleService   _vehicles;

    public ReportsController(IReportService   reports,
                             ICustomerService customers,
                             IVehicleService  vehicles)
    {
        _reports   = reports;
        _customers = customers;
        _vehicles  = vehicles;
    }

    /* ───── Formularz + tabela ───── */
    public async Task<IActionResult> Index(int? customerId,
                                           int? vehicleId,
                                           int? year,
                                           int? month)
    {
        ViewBag.CustomerList = new SelectList(
            await _customers.GetAllAsync(null), "Id", "FullName", customerId);

        var vehSource = customerId is null
            ? await _vehicles.GetAllAsync(0)
            : await _vehicles.GetAllAsync(customerId.Value);

        ViewBag.VehicleList = new SelectList(vehSource, "Id", "RegistrationNumber", vehicleId);

        ViewBag.Years = Enumerable.Range(DateTime.Today.Year - 5, 6)
                                  .Reverse()
                                  .Select(y => new SelectListItem(y.ToString(), y.ToString()))
                                  .ToList();

        ViewBag.Months = Enumerable.Range(1, 12)
                                   .Select(m => new SelectListItem(
                                       $"{m:00} – {new DateTime(1, m, 1):MMMM}",
                                       m.ToString()))
                                   .ToList();

        var rows = await _reports.GetAsync(customerId, vehicleId, year, month);
        return View(rows);
    }

    /* ───── Eksport PDF ───── */
    [HttpPost]
    public async Task<IActionResult> ExportPdf(int? customerId,
                                               int? vehicleId,
                                               int? year,
                                               int? month)
    {
        var rows = await _reports.GetAsync(customerId, vehicleId, year, month);

        var title = "Raport kosztów";
        if (year != null)  title += $" {year}";
        if (month != null) title += $"-{month:00}";
        if (vehicleId != null)  title += $" / pojazd";
        else if (customerId != null) title += $" / klient";

        var pdf = await _reports.ExportPdfAsync(rows, title);

        return File(pdf, "application/pdf", "raport.pdf");
    }
}
