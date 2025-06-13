using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize]
public class ServiceOrdersController : Controller
{
    private readonly IOrderService      _orders;
    private readonly ICustomerService   _customers;
    private readonly IVehicleService    _vehicles;
    private readonly ITaskService       _tasks;
    private readonly IUsedPartService   _usedParts;
    private readonly IPartService       _partsCatalog;
    private readonly ILogger<ServiceOrdersController> _log;

    public ServiceOrdersController(IOrderService    orders,
                                   ICustomerService customers,
                                   IVehicleService  vehicles,
                                   ITaskService     tasks,
                                   IUsedPartService parts,
                                   IPartService     partsCatalog,
                                   ILogger<ServiceOrdersController> log)
    {
        _orders       = orders;
        _customers    = customers;
        _vehicles     = vehicles;
        _tasks        = tasks;
        _usedParts    = parts;
        _partsCatalog = partsCatalog;
        _log          = log;
    }

    public async Task<IActionResult> Index(string? status = null)
    {
        ViewBag.FilterStatus = status ?? "All";
        return View(await _orders.GetAllAsync(status));
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _orders.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    public async Task<IActionResult> Create()
    {
        await FillSelectListsAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceOrderDto dto)
    {
        ModelState.Remove(nameof(dto.CustomerName));
        ModelState.Remove(nameof(dto.VehicleReg));

        if (!ModelState.IsValid)
        {
            await FillSelectListsAsync(dto.CustomerId, dto.VehicleId);
            return View(dto);
        }

        dto = dto with { Status = "New", CreatedAt = DateTime.UtcNow };

        try
        {
            await _orders.AddAsync(dto);
            _log.LogInformation("ServiceOrder #{Id} created", dto.Id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Create(ServiceOrder) failed {@Dto}", dto);
            await FillSelectListsAsync(dto.CustomerId, dto.VehicleId);
            return View("Error");
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _orders.GetAsync(id);
        if (dto is null) return NotFound();

        await FillSelectListsAsync(dto.CustomerId, dto.VehicleId);
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceOrderDto dto)
    {
        if (id != dto.Id) return BadRequest();

        ModelState.Remove(nameof(dto.CustomerName));
        ModelState.Remove(nameof(dto.VehicleReg));

        if (!ModelState.IsValid)
        {
            await FillSelectListsAsync(dto.CustomerId, dto.VehicleId);
            return View(dto);
        }

        try
        {
            var ok = await _orders.UpdateAsync(id, dto);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Edit(ServiceOrder) failed {@Dto}", dto);
            await FillSelectListsAsync(dto.CustomerId, dto.VehicleId);
            return View("Error");
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _orders.GetAsync(id);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var ok = await _orders.DeleteAsync(id);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Delete(ServiceOrder) failed Id={Id}", id);
            return View("Error");
        }
    }

    /*──────── Zadania ────────*/
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTask(ServiceTaskDto dto)
    {
        try
        {
            await _tasks.AddAsync(dto);
            return RedirectToAction(nameof(Details), new { id = dto.OrderId });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "AddTask failed {@Dto}", dto);
            return View("Error");
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTask(int id, int orderId)
    {
        await _tasks.DeleteAsync(id);
        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    /*──────── Części ────────*/
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExistingParts(int orderId, int[] partIds)
    {
        if (partIds.Length == 0)
            return RedirectToAction(nameof(Details), new { id = orderId });

        var catalog = await _partsCatalog.GetManyAsync(partIds);

        try
        {
            foreach (var p in catalog)
            {
                var dto = new UsedPartDto(
                    0,             // id (auto)
                    orderId,
                    p.Id,
                    1,             // Qty domyślnie 1
                    p.UnitPrice,
                    p.Name);

                await _usedParts.AddAsync(dto);
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "AddExistingParts failed order={Order} parts={Ids}", orderId, string.Join(",", partIds));
            return View("Error");
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePart(int id, int orderId)
    {
        await _usedParts.DeleteAsync(id);
        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    /*──────── helper ────────*/
    private async Task FillSelectListsAsync(int selectedCustomer = 0, int selectedVehicle = 0)
    {
        var customers = await _customers.GetAllAsync(null);
        ViewBag.CustomerList = new SelectList(customers, "Id", "FullName", selectedCustomer);

        var vehicles = await _vehicles.GetAllAsync(0);
        ViewBag.VehicleList = new SelectList(vehicles, "Id", "RegistrationNumber", selectedVehicle);
    }
}
