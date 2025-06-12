using Microsoft.EntityFrameworkCore;
using WorkshopManager.Models;

namespace WorkshopManager.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly WorkshopDbContext _context;
    public CustomerRepository(WorkshopDbContext context) => _context = context;

    public async Task<Customer?> GetByIdAsync(int id) =>
        await _context.Customers.FindAsync(id);

    public async Task<IEnumerable<Customer>> GetAllAsync() =>
        await _context.Customers.ToListAsync();

    public async Task AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }
}