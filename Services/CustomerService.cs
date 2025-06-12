using WorkshopManager.Data.Repositories;
using WorkshopManager.Models;

namespace WorkshopManager.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repo;
    public CustomerService(ICustomerRepository repo) => _repo = repo;

    public Task<Customer?> GetCustomerAsync(int id) => _repo.GetByIdAsync(id);
    public Task<IEnumerable<Customer>> GetAllCustomersAsync() => _repo.GetAllAsync();
    public Task AddCustomerAsync(Customer customer) => _repo.AddAsync(customer);
}