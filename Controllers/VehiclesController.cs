using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize]
public class VehiclesController : Controller
{
    private readonly IVehicleService _srv;
    private readonly ICustomerService _customers;
    private readonly ILogger<VehiclesController> _log;

    public VehiclesController(IVehicleService  srv,
                              ICustomerService customers,
                              ILogger<VehiclesController> log)
    {
        _srv       = srv;
        _customers = customers;
        _log       = log;
    }

    public async Task<IActionResult> Index(int customerId = 0)
        => View(await _srv.GetAllAsync(customerId));

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _srv.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    public async Task<IActionResult> Create(int customerId = 0)
    {
        await FillSelectListAsync(customerId);
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleDto dto)
    {
        if (!ModelState.IsValid)
        {
            await FillSelectListAsync(dto.CustomerId);
            return View(dto);
        }

        try
        {
            await _srv.AddAsync(dto);
            _log.LogInformation("Vehicle {Reg} added", dto.RegistrationNumber);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Create(Vehicle) failed for {@Dto}", dto);
            await FillSelectListAsync(dto.CustomerId);
            return View("Error");
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _srv.GetAsync(id);
        if (dto is null) return NotFound();

        await FillSelectListAsync(dto.CustomerId);
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await FillSelectListAsync(dto.CustomerId);
            return View(dto);
        }

        try
        {
            var ok = await _srv.UpdateAsync(id, dto);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Edit(Vehicle) failed for {@Dto}", dto);
            await FillSelectListAsync(dto.CustomerId);
            return View("Error");
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _srv.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var ok = await _srv.DeleteAsync(id);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Delete(Vehicle) failed Id={Id}", id);
            return View("Error");
        }
    }

    /* helpers */
    private async Task FillSelectListAsync(int selected = 0)
    {
        var list = await _customers.GetAllAsync(null);
        ViewBag.CustomerList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            list, "Id", "FullName", selected);
    }
}
