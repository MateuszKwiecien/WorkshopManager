// Controllers/PartsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize(Roles = "Admin,Recepcjonista")]
public class PartsController : Controller
{
    private readonly IPartService _service;

    public PartsController(IPartService service) => _service = service;

    /*──────── LISTA ─────────────────────────────────────────*/

    // GET: /Parts?search=filtr
    public async Task<IActionResult> Index(string? search = null)
        => View(await _service.GetAllAsync(search));

    /*──────── SZCZEGÓŁ ──────────────────────────────────────*/

    // GET: /Parts/Details/5
    public async Task<IActionResult> Details(int id)
        => (await _service.GetAsync(id)) is { } dto
              ? View(dto)
              : NotFound();

    /*──────── CREATE ───────────────────────────────────────*/

    // GET: /Parts/Create
    [HttpGet]
    public IActionResult Create() => View();

    // POST: /Parts/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _service.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    /*──────── EDIT ─────────────────────────────────────────*/

    // GET: /Parts/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
        => (await _service.GetAsync(id)) is { } dto
              ? View(dto)
              : NotFound();

    // POST: /Parts/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PartDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);

        var ok = await _service.UpdateAsync(id, dto);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }

    /*──────── DELETE ───────────────────────────────────────*/

    // GET: /Parts/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
        => (await _service.GetAsync(id)) is { } dto
              ? View(dto)
              : NotFound();

    // POST: /Parts/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }
}
