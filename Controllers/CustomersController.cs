using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _srv;

    public CustomersController(ICustomerService service) => _srv = service;

    /*──── LISTA ────*/
    public async Task<IActionResult> Index(string? search = null)
        => View(await _srv.GetAllAsync(search));

    /*──── SZCZEGÓŁ ─*/
    public async Task<IActionResult> Details(int id)
        => await _srv.GetAsync(id) is { } c ? View(c) : NotFound();

    /*──── CREATE ───*/
    [HttpGet] public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        await _srv.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    /*──── EDIT ─────*/
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
        => await _srv.GetAsync(id) is { } c ? View(c) : NotFound();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CustomerDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);

        var ok = await _srv.UpdateAsync(id, dto);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }

    /*──── DELETE ───*/
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
        => await _srv.GetAsync(id) is { } c ? View(c) : NotFound();

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ok = await _srv.DeleteAsync(id);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }
}