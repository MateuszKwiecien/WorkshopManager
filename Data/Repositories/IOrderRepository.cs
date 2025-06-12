namespace WorkshopManager.Data.Repositories;
using WorkshopManager.Models;

public interface IOrderRepository
{
    Task<ServiceOrder?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceOrder>> GetAllAsync();
    Task AddAsync(ServiceOrder order);
}