using Microsoft.EntityFrameworkCore;
using WorkshopManager.Models;

namespace WorkshopManager.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly WorkshopDbContext _context;
    public OrderRepository(WorkshopDbContext context) => _context = context;

    public async Task<ServiceOrder?> GetByIdAsync(int id) =>
        await _context.ServiceOrders.FindAsync(id);

    public async Task<IEnumerable<ServiceOrder>> GetAllAsync() =>
        await _context.ServiceOrders.ToListAsync();

    public async Task AddAsync(ServiceOrder order)
    {
        _context.ServiceOrders.Add(order);
        await _context.SaveChangesAsync();
    }
}