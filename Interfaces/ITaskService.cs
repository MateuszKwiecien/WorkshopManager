using WorkshopManager.DTOs;

namespace WorkshopManager.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<ServiceTaskDto>> ListAsync(int orderId);

    Task<ServiceTaskDto?> GetAsync(int id);

    Task AddAsync(ServiceTaskDto dto);
    
    Task<bool> UpdateAsync(int id, ServiceTaskDto dto);

    Task<bool> DeleteAsync(int id);
    
    Task<IEnumerable<ServiceTaskDto>> GetManyAsync(IEnumerable<int> ids);
}