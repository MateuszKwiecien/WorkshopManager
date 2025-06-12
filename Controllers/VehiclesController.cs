// Controllers/VehiclesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize]
public class VehiclesController : Controller
{
    private readonly IVehicleService  _vehicles;
    private readonly ICustomerService _customers;

    public VehiclesController(IVehicleService v, ICustomerService c)
    {
        _vehicles  = v;
        _customers = c;
    }

    /*──────────────────────  LISTA  ──────────────────────*/
    public async Task<IActionResult> Index(int customerId = 0)
        => View(await _vehicles.GetAllAsync(customerId));

    /*──────────────────────  SZCZEGÓŁ  ───────────────────*/
    public async Task<IActionResult> Details(int id)
        => await _vehicles.GetAsync(id) is { } v ? View(v) : NotFound();

    /*──────────────────────  CREATE  ─────────────────────*/
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await FillCustomerSelectListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleDto dto)
    {
        if (!ModelState.IsValid)
        {
            await FillCustomerSelectListAsync(dto.CustomerId);
            return View(dto);
        }

        await _vehicles.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    /*──────────────────────  EDIT  ───────────────────────*/
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var v = await _vehicles.GetAsync(id);
        if (v is null) return NotFound();

        await FillCustomerSelectListAsync(v.CustomerId);
        return View(v);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleDto dto)
    {
        if (id != dto.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await FillCustomerSelectListAsync(dto.CustomerId);
            return View(dto);
        }

        var ok = await _vehicles.UpdateAsync(id, dto);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }

    /*──────────────────────  DELETE  ─────────────────────*/
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var v = await _vehicles.GetAsync(id);
        return v is null ? NotFound() : View(v);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ok = await _vehicles.DeleteAsync(id);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }

    /*──────────────────────  HELPER  ─────────────────────*/
    private async Task FillCustomerSelectListAsync(int selected = 0)
    {
        var cust = await _customers.GetAllAsync(null);
        ViewBag.CustomerList =
            new SelectList(cust, "Id", "FullName", selected);
    }
}