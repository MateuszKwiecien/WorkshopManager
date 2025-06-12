using WorkshopManager.Data.Repositories;
using WorkshopManager.Models;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;

namespace WorkshopManager.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly WorkshopDbContext _context;

    public OrderService(IOrderRepository repo, WorkshopDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public Task<ServiceOrder?> GetOrderAsync(int id) => _repo.GetByIdAsync(id);
    public Task<IEnumerable<ServiceOrder>> GetAllOrdersAsync() => _repo.GetAllAsync();
    public Task AddOrderAsync(ServiceOrder order) => _repo.AddAsync(order);

    public async Task AssignMechanicAsync(int orderId, string mechanicId)
    {
        var order = await _context.ServiceOrders.FindAsync(orderId);
        if (order == null) return;
        order.MechanicId = mechanicId;
        await _context.SaveChangesAsync();
    }

    public async Task SetOrderDescriptionAsync(int orderId, string description)
    {
        var order = await _context.ServiceOrders.FindAsync(orderId);
        if (order == null) return;
        order.Description = description;
        await _context.SaveChangesAsync();
    }
}