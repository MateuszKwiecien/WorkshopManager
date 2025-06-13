using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize]
public class PartsController : Controller
{
    private readonly IPartService _service;
    private readonly ILogger<PartsController> _log;

    public PartsController(IPartService service,
                           ILogger<PartsController> log)
    {
        _service = service;
        _log     = log;
    }

    public async Task<IActionResult> Index()
        => View(await _service.GetAllAsync(null));

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        try
        {
            await _service.AddAsync(dto);
            _log.LogInformation("Part {Name} added", dto.Name);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Create(Part) failed for {@Dto}", dto);
            return View("Error");
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _service.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PartDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);

        try
        {
            var ok = await _service.UpdateAsync(id, dto);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Edit(Part) failed for {@Dto}", dto);
            return View("Error");
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Delete(Part) failed for Id={Id}", id);
            return View("Error");
        }
    }
}
