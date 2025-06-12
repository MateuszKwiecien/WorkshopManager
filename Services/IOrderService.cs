using WorkshopManager.Models;

namespace WorkshopManager.Services;

public interface IOrderService
{
    Task<ServiceOrder?> GetOrderAsync(int id);
    Task<IEnumerable<ServiceOrder>> GetAllOrdersAsync();
    Task AddOrderAsync(ServiceOrder order);
    Task AssignMechanicAsync(int orderId, string mechanicId);
    Task SetOrderDescriptionAsync(int orderId, string description);
}