using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers;

[Authorize(Roles = "Admin,Mechanik")]
public class TasksController : Controller
{
    private readonly ITaskService _srv;
    private readonly IOrderService _orders;

    public TasksController(ITaskService srv, IOrderService orders)
    {
        _srv = srv; _orders = orders;
    }

    /*──── LISTA (opcjonalnie filtr po OrderId) ────*/
    public async Task<IActionResult> Index(int orderId = 0)
        => View(await _srv.ListAsync(orderId));

    /*──── DETAILS ────*/
    public async Task<IActionResult> Details(int id)
        => (await _srv.GetAsync(id)) is { } dto ? View(dto) : NotFound();

    /*──── CREATE ────*/
    [HttpGet]
    public async Task<IActionResult> Create(int orderId = 0)
    {
        ViewBag.OrderList = new SelectList(await _orders.GetAllAsync(null),
                                           "Id", "Id", orderId);
        return View(new ServiceTaskDto { OrderId = orderId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceTaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.OrderList = new SelectList(await _orders.GetAllAsync(null),
                                               "Id", "Id", dto.OrderId);
            return View(dto);
        }

        await _srv.AddAsync(dto);
        return RedirectToAction(nameof(Index), new { orderId = dto.OrderId });
    }

    /*──── EDIT ────*/
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _srv.GetAsync(id);
        if (dto is null) return NotFound();

        ViewBag.OrderList = new SelectList(await _orders.GetAllAsync(null),
                                           "Id", "Id", dto.Id);
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceTaskDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.OrderList = new SelectList(await _orders.GetAllAsync(null),
                                               "Id", "Id", dto.OrderId);
            return View(dto);
        }

        var ok = await _srv.UpdateAsync(id, dto);
        return ok
            ? RedirectToAction(nameof(Index), new { orderId = dto.OrderId })
            : NotFound();
    }

    /*──── DELETE ────*/
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
        => (await _srv.GetAsync(id)) is { } dto ? View(dto) : NotFound();

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int orderId)
    {
        await _srv.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { orderId });
    }
}
