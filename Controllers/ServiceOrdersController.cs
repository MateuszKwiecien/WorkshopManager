// Controllers/ServiceOrdersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers
{
    [Authorize]                                   // dostęp tylko dla zalogowanych
    public class ServiceOrdersController : Controller
    {
        private readonly IOrderService     _orders;
        private readonly ICustomerService  _customers;
        private readonly IVehicleService   _vehicles;

        public ServiceOrdersController(
            IOrderService    orders,
            ICustomerService customers,
            IVehicleService  vehicles)
        {
            _orders    = orders;
            _customers = customers;
            _vehicles  = vehicles;
        }

        // GET: /ServiceOrders?status=Open
        public async Task<IActionResult> Index(string? status = null)
        {
            var list = await _orders.GetAllAsync(status);
            ViewBag.FilterStatus = status ?? "All";
            return View(list);
        }

        // GET: /ServiceOrders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orders.GetAsync(id);
            return order is null ? NotFound() : View(order);
        }

        /*──────────────────────  Create  ──────────────────────*/

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillSelectListsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        /*──────────────────────  Edit  ────────────────────────*/

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orders.GetAsync(id);
            if (order is null) return NotFound();

            await FillSelectListsAsync(order.CustomerId, order.VehicleId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        /*──────────────────────  Delete  ──────────────────────*/

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orders.GetAsync(id);
            return order is null ? NotFound() : View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _orders.DeleteAsync(id);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }

        /*──────────────────────  Helpers  ─────────────────────*/

        /// <summary>
        /// Ładuje listy klientów i pojazdów do ViewBag do widoków Create/Edit.
        /// </summary>
        private async Task FillSelectListsAsync(int selectedCustomerId = 0, int selectedVehicleId = 0)
        {
            var customers = await _customers.GetAllAsync(null);
            ViewBag.CustomerList = new SelectList(customers, "Id", "FullName", selectedCustomerId);

            // Jeśli wybrano klienta – filtrujemy tylko jego pojazdy
            var vehicles = await _vehicles.GetAllAsync(selectedCustomerId);
            ViewBag.VehicleList = new SelectList(vehicles, "Id", "RegistrationNumber", selectedVehicleId);
        }
    }
}
