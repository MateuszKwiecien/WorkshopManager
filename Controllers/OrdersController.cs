using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Services;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Models;

public class OrdersController : Controller
{
    private readonly IOrderService _service;
    private readonly EntityMapper _mapper = new EntityMapper();

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _service.GetAllOrdersAsync();
        var orderDtos = orders.Select(o => _mapper.ServiceOrderToDto(o)).ToList();
        return View(orderDtos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _service.GetOrderAsync(id);
        if (order == null) return NotFound();
        var dto = _mapper.ServiceOrderToDto(order);
        return View(dto);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var order = _mapper.DtoToServiceOrder(dto);
        await _service.AddOrderAsync(order);
        return RedirectToAction(nameof(Index));
    }

    // Add Edit/Delete actions as needed, using the mapper for conversions
}