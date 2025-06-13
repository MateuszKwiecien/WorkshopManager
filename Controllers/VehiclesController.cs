// Controllers/VehiclesController.cs

using System.Diagnostics;
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

    [HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> Create(VehicleDto dto, IFormFile? photo)
{
    // 1. próbujemy znaleźć klienta po imieniu
    if (dto.CustomerId == 0 && !string.IsNullOrWhiteSpace(dto.CustomerName))
    {
        var match = (await _customers.GetAllAsync(null))
            .Where(c => c.FullName.Equals(dto.CustomerName.Trim(),
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (match.Count == 1)
        {
            dto = dto with
            {
                CustomerId = match[0].Id,
                CustomerName = match[0].FullName
            };
        }
        else
        {
            ModelState.AddModelError("CustomerName",
                match.Count == 0
                    ? "Nie znaleziono klienta o podanym imieniu."
                    : "Istnieje więcej niż jeden klient o tej nazwie – podaj dokładniej.");
        }
    }

    // 2. walidacja
    if (!ModelState.IsValid)
    {
        await FillCustomerSelectListAsync();
        return View(dto);
    }

    /* ▸  Handle file upload  */
    if (photo is { Length: > 0 })
    {
        var validTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!validTypes.Contains(photo.ContentType))
            ModelState.AddModelError("ImagePath", "Dozwolone formaty: JPG, PNG, WEBP");

        if (photo.Length > 5 * 1024 * 1024)        // 5 MB
            ModelState.AddModelError("ImagePath", "Maksymalny rozmiar to 5 MB");

        if (!ModelState.IsValid)
        {
            await FillCustomerSelectListAsync();
            return View(dto);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
        var savePath = Path.Combine("wwwroot", "uploads", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
        await using var stream = System.IO.File.Create(savePath);
        await photo.CopyToAsync(stream);

        dto = dto with { ImagePath = $"/uploads/{fileName}" };
        Debug.WriteLine($"New image path: {dto.ImagePath}");
    }

    // 3. zapis
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
    public async Task<IActionResult> Edit(int id, VehicleDto dto, IFormFile? photo)
    {
        if (id != dto.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await FillCustomerSelectListAsync(dto.CustomerId);
            return View(dto);
        }

        /* ▸  Handle file upload  */
        if (photo is { Length: > 0 })
        {
            var validTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!validTypes.Contains(photo.ContentType))
                ModelState.AddModelError("ImagePath", "Dozwolone formaty: JPG, PNG, WEBP");

            if (photo.Length > 5 * 1024 * 1024)        // 5 MB
                ModelState.AddModelError("ImagePath", "Maksymalny rozmiar to 5 MB");

            if (!ModelState.IsValid)
            {
                await FillCustomerSelectListAsync(dto.CustomerId);
                return View(dto);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var savePath = Path.Combine("wwwroot", "uploads", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
            await using var stream = System.IO.File.Create(savePath);
            await photo.CopyToAsync(stream);

            dto = dto with { ImagePath = $"/uploads/{fileName}" };
            Debug.WriteLine($"New image path: {dto.ImagePath}");
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