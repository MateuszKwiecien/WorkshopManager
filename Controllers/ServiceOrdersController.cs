// Controllers/ServiceOrdersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers
{
    [Authorize]   // dostęp tylko dla zalogowanych
    public class ServiceOrdersController : Controller
    {
        private readonly IOrderService      _orders;
        private readonly ICustomerService   _customers;
        private readonly IVehicleService    _vehicles;
        private readonly ITaskService       _tasks;      // ← NOWE
        private readonly IUsedPartService   _parts;      // ← NOWE

        public ServiceOrdersController(
            IOrderService      orders,
            ICustomerService   customers,
            IVehicleService    vehicles,
            ITaskService       tasks,       // ← NOWE
            IUsedPartService   parts)       // ← NOWE
        {
            _orders    = orders;
            _customers = customers;
            _vehicles  = vehicles;
            _tasks     = tasks;
            _parts     = parts;
        }

        /*──────────────────────  LISTA  ─────────────────────*/
        public async Task<IActionResult> Index(string? status = null)
        {
            ViewBag.FilterStatus = status ?? "All";
            return View(await _orders.GetAllAsync(status));
        }

        /*──────────────────────  SZCZEGÓŁ  ───────────────────*/
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orders.GetAsync(id);
            return order is null ? NotFound() : View(order);
        }

        /*──────────────────────  CREATE  ─────────────────────*/
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillSelectListsAsync();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                await FillSelectListsAsync(dto.CustomerId);
                return View(dto);
            }

            await _orders.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        /*──────────────────────  EDIT  ───────────────────────*/
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orders.GetAsync(id);
            if (order is null) return NotFound();

            await FillSelectListsAsync(order.CustomerId, order.VehicleId);
            return View(order);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceOrderDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await FillSelectListsAsync(dto.CustomerId, dto.VehicleId);
                return View(dto);
            }

            var ok = await _orders.UpdateAsync(id, dto);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }

        /*──────────────────────  DELETE  ─────────────────────*/
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orders.GetAsync(id);
            return order is null ? NotFound() : View(order);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _orders.DeleteAsync(id);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }

        /*─────────  Zadania (ServiceTask)  ─────────*/
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTask(ServiceTaskDto dto)
        {
            await _tasks.AddAsync(dto);
            return RedirectToAction(nameof(Details), new { id = dto.OrderId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(int id, int orderId)
        {
            await _tasks.DeleteAsync(id);
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        /*─────────  Części (UsedPart)  ─────────────*/
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPart(UsedPartDto dto)
        {
            await _parts.AddAsync(dto);
            return RedirectToAction(nameof(Details), new { id = dto.OrderId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePart(int id, int orderId)
        {
            await _parts.DeleteAsync(id);
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        /*──────────────────────  HELPER  ─────────────────────*/
        private async Task FillSelectListsAsync(int selCustomer = 0, int selVehicle = 0)
        {
            var customers = await _customers.GetAllAsync(null);
            ViewBag.CustomerList =
                new SelectList(customers, "Id", "FullName", selCustomer);

            var vehicles = await _vehicles.GetAllAsync(selCustomer);
            ViewBag.VehicleList =
                new SelectList(vehicles, "Id", "RegistrationNumber", selVehicle);
        }
    }
}
