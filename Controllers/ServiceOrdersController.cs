// Controllers/ServiceOrdersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkshopManager.DTOs;
using WorkshopManager.Interfaces;

namespace WorkshopManager.Controllers
{
    [Authorize]                                     // dostęp tylko dla zalogowanych
    public class ServiceOrdersController : Controller
    {
        private readonly IOrderService    _orders;
        private readonly ICustomerService _customers;
        private readonly IVehicleService  _vehicles;
        private readonly ITaskService     _tasks;
        private readonly IUsedPartService _usedParts;
        private readonly IPartService _partsCatalog;   // katalog Parts

        public ServiceOrdersController(
            IOrderService    orders,
            ICustomerService customers,
            IVehicleService  vehicles,
            ITaskService     tasks,
            IUsedPartService usedParts,
            IPartService     partsCatalog)   // ← NOWE
        {
            _orders       = orders;
            _customers    = customers;
            _vehicles     = vehicles;
            _tasks        = tasks;
            _usedParts    = usedParts;
            _partsCatalog = partsCatalog;    // ← NOWE
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
            // pola tylko-do-odczytu usuwamy z walidacji
            ModelState.Remove(nameof(dto.CustomerName));
            ModelState.Remove(nameof(dto.VehicleReg));

            if (!ModelState.IsValid)
            {
                await FillSelectListsAsync(dto.CustomerId, dto.VehicleId);
                return View(dto);
            }

            dto = dto with
            {
                Status    = "New",
                CreatedAt = DateTime.UtcNow
            };

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

            ModelState.Remove(nameof(dto.CustomerName));
            ModelState.Remove(nameof(dto.VehicleReg));

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
            if (dto.OrderId == 0)
                return BadRequest("Brak OrderId");

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
            await _usedParts.AddAsync(dto);
            return RedirectToAction(nameof(Details), new { id = dto.OrderId });
        }
        
        // POST: /ServiceOrders/AddExistingParts
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExistingParts(int orderId, int[] partIds)
        {
            if (partIds.Length == 0)
                return RedirectToAction(nameof(Details), new { id = orderId });

            var catalog = await _partsCatalog.GetManyAsync(partIds);

            foreach (var p in catalog)
            {
                // ‼ używamy NAZWANYCH argumentów — kolejność już nie myli
                var dto = new UsedPartDto(
                    Id:        0,          // EF Core sam nada
                    OrderId:   orderId,    // FK do ServiceOrders
                    PartId:    p.Id,       // FK do katalogu Parts
                    Quantity:  1,
                    UnitPrice: p.UnitPrice,
                    PartName:  p.Name);

                await _usedParts.AddAsync(dto);
            }

            return RedirectToAction(nameof(Details), new { id = orderId });
        }



        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePart(int id, int orderId)
        {
            await _usedParts.DeleteAsync(id);
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        /*──────────────────────  HELPERS  ────────────────────*/
        private async Task FillSelectListsAsync(int selectedCustomer = 0,
                                                int selectedVehicle  = 0)
        {
            var customers = await _customers.GetAllAsync(null);
            ViewBag.CustomerList =
                new SelectList(customers, "Id", "FullName", selectedCustomer);

            var vehicles = await _vehicles.GetAllAsync(0);     // wszystkie
            ViewBag.VehicleList =
                new SelectList(vehicles, "Id", "RegistrationNumber", selectedVehicle);
        }
    }
}
