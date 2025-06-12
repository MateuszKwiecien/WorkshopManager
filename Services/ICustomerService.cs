using WorkshopManager.Models;

namespace WorkshopManager.Services;

public interface ICustomerService
{
    Task<Customer?> GetCustomerAsync(int id);
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task AddCustomerAsync(Customer customer);
}