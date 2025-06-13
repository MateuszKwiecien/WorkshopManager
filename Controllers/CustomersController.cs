using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _srv;
    private readonly ILogger<CustomersController> _log;

    public CustomersController(ICustomerService service,
                               ILogger<CustomersController> log)
    {
        _srv = service;
        _log = log;
    }

    public async Task<IActionResult> Index()
        => View(await _srv.GetAllAsync(null));

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _srv.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        try
        {
            await _srv.AddAsync(dto);
            _log.LogInformation("Customer {Name} added", dto.FullName);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Create(Customer) failed for {@Dto}", dto);
            return View("Error");
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _srv.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CustomerDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);

        try
        {
            var ok = await _srv.UpdateAsync(id, dto);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Edit(Customer) failed for {@Dto}", dto);
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
            _log.LogError(ex, "Delete(Customer) failed for Id={Id}", id);
            return View("Error");
        }
    }
}
