using WorkshopManager.DTOs;

namespace WorkshopManager.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<ServiceOrderDto>> GetAllAsync(string? status);
    Task<ServiceOrderDto?>             GetAsync(int id);
    Task<int>                          AddAsync(ServiceOrderDto dto);
    Task<bool>                         UpdateAsync(int id, ServiceOrderDto dto);
    Task<bool>                         DeleteAsync(int id);
}