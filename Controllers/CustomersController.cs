using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Models;
using WorkshopManager.DTOs;
using WorkshopManager.Mappers;
using WorkshopManager.Services;
using X.PagedList;
using X.PagedList.Extensions;

public class CustomersController : Controller
{
    private readonly ICustomerService _service;
    private readonly EntityMapper _mapper = new EntityMapper();

    public CustomersController(ICustomerService service)
    {
        _service = service;
    }

    // public async Task<IActionResult> Index()
    // {
    //     var customers = await _service.GetAllCustomersAsync();
    //     var customerDtos = customers.Select(c => _mapper.CustomerToDto(c)).ToList();
    //     return View(customerDtos);
    // }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _service.GetByIdAsync(id);
        if (customer == null) return NotFound();
        var dto = _mapper.CustomerToDto(customer);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var customer = _mapper.DtoToCustomer(dto);
        await _service.AddCustomerAsync(customer);
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Index(string? search, int? page)
    {
        var customers = await _service.GetAllCustomersAsync();
        if (!string.IsNullOrEmpty(search))
        {
            customers = customers.Where(c => c.FullName.Contains(search) || c.Email.Contains(search));
        }
        int pageSize = 10;
        int pageNumber = page ?? 1;
        ViewBag.CurrentFilter = search;
        var customerDtos = customers.Select(c => _mapper.CustomerToDto(c));
        return View(customerDtos.ToPagedList(pageNumber, pageSize));
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _service.GetByIdAsync(id);
        if (customer == null) return NotFound();
        var dto = _mapper.CustomerToDto(customer);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CustomerDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var customer = _mapper.DtoToCustomer(dto);
        await _service.UpdateCustomerAsync(customer);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _service.GetByIdAsync(id);
        if (customer == null) return NotFound();
        var dto = _mapper.CustomerToDto(customer);
        return View(dto);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteCustomerAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // Add Edit/Delete actions similarly, using the mapper for conversions
}