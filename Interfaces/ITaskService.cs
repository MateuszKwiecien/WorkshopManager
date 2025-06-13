using WorkshopManager.DTOs;

namespace WorkshopManager.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<ServiceTaskDto>> ListAsync(int orderId);
    Task AddAsync(ServiceTaskDto dto);
    Task DeleteAsync(int id);
}