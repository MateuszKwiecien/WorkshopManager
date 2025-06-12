using WorkshopManager.Data;
using WorkshopManager.Data.Repositories;
using WorkshopManager.Models;

namespace WorkshopManager.Services;

public class CustomerService : ICustomerService
{
    private readonly WorkshopDbContext _context;

    private readonly ICustomerRepository _repo;
    public CustomerService(ICustomerRepository repo) => _repo = repo;
    
    public Task<Customer?> GetCustomerAsync(int id) => _repo.GetByIdAsync(id);
    public Task<IEnumerable<Customer>> GetAllCustomersAsync() => _repo.GetAllAsync();
    public Task AddCustomerAsync(Customer customer) => _repo.AddAsync(customer);
    public Task<Customer?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task UpdateCustomerAsync(Customer customer) => _repo.UpdateAsync(customer);
    public Task DeleteCustomerAsync(int id) => _repo.DeleteAsync(id);

    // public Task<Customer?> GetCustomerAsync(int id) => _repo.GetByIdAsync(id);
    // public Task<IEnumerable<Customer>> GetAllCustomersAsync() => _repo.GetAllAsync();
    // public Task AddCustomerAsync(Customer customer) => _repo.AddAsync(customer);
    // public async Task<Customer?> GetByIdAsync(int id)
    //     => await _context.Customers.FindAsync(id);
    //
    // public async Task UpdateCustomerAsync(Customer customer)
    // {
    //     _context.Customers.Update(customer);
    //     await _context.SaveChangesAsync();
    // }
    //
    // public async Task DeleteCustomerAsync(int id)
    // {
    //     var customer = await _context.Customers.FindAsync(id);
    //     if (customer != null)
    //     {
    //         _context.Customers.Remove(customer);
    //         await _context.SaveChangesAsync();
    //     }
    // }
}