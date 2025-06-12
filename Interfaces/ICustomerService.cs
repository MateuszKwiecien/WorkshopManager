using WorkshopManager.DTOs;

namespace WorkshopManager.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync(string? search);
        Task<CustomerDto?>             GetAsync(int id);
        Task<int>                      AddAsync(CustomerDto dto);
        Task<bool>                     UpdateAsync(int id, CustomerDto dto);
        Task<bool>                     DeleteAsync(int id);
    }
}